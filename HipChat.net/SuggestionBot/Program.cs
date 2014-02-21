using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HipChat;

namespace SuggestionBot
{
    class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main( string[] args )
        {
            // HipChat initialization
            HipChatClient hipChatClient = new HipChatClient( "54f19307e4626c9578d9dcb267ea19", 222901, "SuggestionBot" );
            DateTime startupDateTime = DateTime.Now;
            CommandDatabase commands = new CommandDatabase();
            Nouns _nouns = new Nouns();
            DateTime lastSuggestion = DateTime.Now;
            double suggestionMinutes = 10;
            bool keepRunning = true;

            // rate limit is 100 API requests per 5 minutes
            int minWaitTimeSeconds = 5 * 60 / 100;
            DateTime lastGotDateTime = DateTime.UtcNow.Subtract( new TimeSpan( 0, 0, 5 ) );

            while ( keepRunning )
            {
                try
                {
                    System.Threading.Thread.Sleep( ( minWaitTimeSeconds * 2 ) * 1000 );


                    //Send suggestion :)
                    if ( ( DateTime.Now - lastSuggestion ).TotalMinutes >= suggestionMinutes )
                    {
                        hipChatClient.SendMessage( _nouns.GetRandomNoun() + " " + _nouns.GetRandomNoun() + " Simulator 2014");
                        lastSuggestion = DateTime.Now;
                    }

                    List<HipChat.Entities.Message> recentMessageList = hipChatClient.ListHistoryAsNativeObjects();
                    recentMessageList = recentMessageList.OrderBy( a => a.Date ).ToList();
                    if ( recentMessageList.Count > 0 )
                    {
                        var lastMessage = recentMessageList.Last();
                        recentMessageList = recentMessageList.Where( a => a.Date > lastGotDateTime ).Where( a => a.From.Name != "SuggestionBot" ).ToList();
                        lastGotDateTime = lastMessage.Date;
                    }

                    #region CommandProcessing

                    foreach ( var messageItem in recentMessageList )
                    {
                        // clean up the message a little
                        messageItem.Text = messageItem.Text.Replace( "\\n", string.Empty ).Trim();

                        Console.ResetColor();
                        Console.WriteLine( messageItem.From.Name + ": " + messageItem.Text );

                        CommandDatabase.Command commandType = commands.GetCommand( messageItem );
                        if ( commandType != CommandDatabase.Command.Unknown )
                        {
                            /* Respond to the RandomAnswerXX class of messagecatagory */
                            string enumName = Enum.GetName( typeof( CommandDatabase.Command ), commandType );

                            switch ( commandType )
                            {
                                /* Bot stuff */
                                case CommandDatabase.Command.BotStats:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( string.Format( "Running since {0}", startupDateTime.ToString( "F" ) ) );
                                        sb.AppendLine( string.Format( "Messages Sent {0}", hipChatClient.MessageSentCount ) );
                                        sb.AppendLine( string.Format( "Api Calls {0}", hipChatClient.ApiCallCount ) );
                                        hipChatClient.SendMessage( sb.ToString() );
                                    }
                                    break;

                                case CommandDatabase.Command.Help:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( "<pre>" );
                                        sb.AppendLine( "Help (Shows this)" );
                                        sb.AppendLine( "BotStats (Shows stats)" );
                                        sb.AppendLine( "BotQuit (Stops the bot program)" );
                                        sb.AppendLine();
                                        sb.AppendLine( "</pre>" );
                                        hipChatClient.SendMessageHtml( sb.ToString() );
                                        // sleep a little so they come out in the right order
                                        System.Threading.Thread.Sleep( 500 );
                                    }
                                    break;

                                case CommandDatabase.Command.BotQuit:
                                    hipChatClient.SendMessage( "OK, I'll quit now. Goodbye, " + messageItem.From.FirstName );
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    #endregion

                }
                catch ( Exception ex )
                {
                    hipChatClient.SendMessage( "Oh man, this happened: " + ex.Message );
                }
            }
        }
    }
}
