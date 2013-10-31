using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HipChat;

namespace DungeonBot
{
    /// <summary>
    /// 
    /// </summary>
    public class BotMessageProcessor
    {
        const string apiToken = "7a695002c154abd55955d9ef8b9375";
        const int currentRoomId = 321236;
        const string botName = "DungeonBot";
        HipChatClient hipChatClient = null;
        DateTime startupDateTime;

        #region GetMessageCategory

        /// <summary>
        /// Gets the message category.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public MessageCategory GetMessageCategory( HipChat.Entities.Message messageItem )
        {
            string message = messageItem.Text;

            MessageCategory result = MessageCategory.Unknown;

            /* Bot Stuff */
            if ( message.Equals( "BotStats", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.BotStats;
            }

            if ( message.Equals( "BotHelp", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.Help;
            }

            if ( message.Equals( "BotQuit" ) )
            {
                return MessageCategory.BotQuit;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Processes the messages.
        /// </summary>
        public void ProcessMessages()
        {
            hipChatClient = new HipChatClient( apiToken, currentRoomId, botName );

            startupDateTime = DateTime.Now;

            bool keepRunning = true;

            // rate limit is 100 API requests per 5 minutes
            int minWaitTimeSeconds = 5 * 60 / 100;
            DateTime lastGotDateTime = DateTime.UtcNow.Subtract( new TimeSpan( 0, 0, 5 ) );

            while ( keepRunning )
            {                
                try
                {
                    System.Threading.Thread.Sleep( ( minWaitTimeSeconds * 2 ) * 1000 );

                    List<HipChat.Entities.Message> recentMessageList = hipChatClient.ListHistoryAsNativeObjects();
                    recentMessageList = recentMessageList.OrderBy( a => a.Date ).ToList();
                    if ( recentMessageList.Count > 0 )
                    {
                        var lastMessage = recentMessageList.Last();
                        recentMessageList = recentMessageList.Where( a => a.Date > lastGotDateTime ).Where( a => a.From.Name != botName ).ToList();
                        lastGotDateTime = lastMessage.Date;
                    }

                    foreach ( var messageItem in recentMessageList )
                    {
                        // clean up the message a little
                        messageItem.Text = messageItem.Text.Replace( "\\n", string.Empty ).Trim();

                        Console.ResetColor();
                        Console.WriteLine( messageItem.From.Name + ": " + messageItem.Text );

                        MessageCategory messageCategory = GetMessageCategory( messageItem );
                        if ( messageCategory != MessageCategory.Unknown )
                        {
                            /* Respond to the RandomAnswerXX class of messagecatagory */
                            string enumName = Enum.GetName( typeof( MessageCategory ), messageCategory );

                            switch ( messageCategory )
                            {
                                /* Bot stuff */
                                case MessageCategory.BotStats:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( string.Format( "Running since {0}", startupDateTime.ToString( "F" ) ) );
                                        sb.AppendLine( string.Format( "Messages Sent {0}", hipChatClient.MessageSentCount ) );
                                        sb.AppendLine( string.Format( "Api Calls {0}", hipChatClient.ApiCallCount ) );
                                        hipChatClient.SendMessage( sb.ToString() );
                                    }
                                    break;

                                case MessageCategory.Help:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( "<pre>" );
                                        sb.AppendLine( "Help (shows this)" );
                                        sb.AppendLine( "BotQuit (stops the bot program)" );
                                        sb.AppendLine( "BotStats (shows stats)" );
                                        sb.AppendLine( "" );
                                        sb.AppendLine();
                                        sb.AppendLine( string.Format( "Want to improve DungeonBot? Go look at the source code at {1}, {0}", messageItem.From.FirstName, "https://github.com/mikepetersonccv/random-projs/tree/master/HipChat.net/HorseBot" ) );
                                        sb.AppendLine( "</pre>" );
                                        hipChatClient.SendMessageHtml( sb.ToString() );
                                        // sleep a little so they come out in the right order
                                        System.Threading.Thread.Sleep( 500 );
                                    }
                                    break;

                                case MessageCategory.BotQuit:
                                    hipChatClient.SendMessage( "OK, I'll quit now. Goodbye, " + messageItem.From.FirstName );
                                    keepRunning = false;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                catch ( Exception ex )
                {
                    hipChatClient.SendMessage( "Oh man, this happened: " + ex.Message );
                }
            }
        }
    }
}
