using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipChat;

namespace HorseBot
{
    class Program
    {
        static void Main( string[] args )
        {
            new BotMessageProcessor().ProcessMessages();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BotMessageProcessor
    {
        const string apiToken = "6e72e6e19bbb3580ea1b9751bc61f4";
        const int roomOfDoomId = 222901;
        const string botName = "HorseBot";
        HipChatClient hipChatClient = null;
        DateTime lastRandomAnswer = DateTime.MinValue;

        public string GetMessageStartWith( MessageCategory messageCategory )
        {
            switch ( messageCategory )
            {
                case MessageCategory.RandomAnswerHow:
                    return "How ";
                case MessageCategory.RandomAnswerWhat:
                    return "What ";
                case MessageCategory.RandomAnswerWhatAre:
                    return "What are ";
                case MessageCategory.RandomAnswerWhatIs:
                    return "What is ";
                case MessageCategory.RandomAnswerWhatKind:
                    return "What kind ";
                case MessageCategory.RandomAnswerWhatTime:
                    return "What time ";
                case MessageCategory.RandomAnswerWhen:
                    return "When ";
                case MessageCategory.RandomAnswerWhere:
                    return "Where ";
                case MessageCategory.RandomAnswerWho:
                    return "Who ";
                case MessageCategory.RandomAnswerWhy:
                    return "Why ";
                default:
                    return null;
            }
        }

        private bool AreAllNumbers( List<string> list )
        {
            bool result = true;

            foreach ( string item in list )
            {
                decimal tempResult;
                if ( !decimal.TryParse( item, out tempResult ) )
                {
                    return false;
                }
            }

            return result;
        }

        public MessageCategory GetMessageCategory( string message )
        {
            MessageCategory result = MessageCategory.Unknown;

            /* Specific Answer */
            // math
            List<string> mathParts = message.Split( new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            if ( AreAllNumbers( mathParts ) )
            {
                return MessageCategory.MathAdd;
            }

            mathParts = message.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            if ( AreAllNumbers( mathParts ) )
            {
                return MessageCategory.MathSubtract;
            }

            mathParts = message.Split( new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            if ( AreAllNumbers( mathParts ) )
            {
                return MessageCategory.MathDivide;
            }

            mathParts = message.Split( new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            if ( AreAllNumbers( mathParts ) )
            {
                return MessageCategory.MathMultiply;
            }

            // Bot Programming
            if ( message.StartsWith( "DefineResponse " ) )
            {
                return MessageCategory.DefineResponse;
            }

            if ( message.StartsWith( "AddMessage " ) )
            {
                return MessageCategory.AddMessage;
            }

            if ( message.Equals( "Quit", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.Quit;
            }

            // Specific Questions
            if ( message.Equals( "What time is it?", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.SpecificWhatTimeIsIt;
            }

            /* Random Answers */
            if ( message.EndsWith( "?" ) )
            {
                foreach ( var item in Enum.GetValues( typeof( MessageCategory ) ) )
                {
                    string enumName = Enum.GetName( typeof( MessageCategory ), item );
                    string messagePrefix = enumName.Replace( "RandomAnswer", string.Empty ).SplitCase() + " ";
                    if ( message.StartsWith( messagePrefix, StringComparison.OrdinalIgnoreCase ) )
                    {
                        return (MessageCategory)item;
                    }

                }
            }

            return result;
        }

        public void ProcessMessages()
        {
            hipChatClient = new HipChatClient( apiToken, roomOfDoomId, botName );
            hipChatClient.SendMessage( ResponseMessageDatabase.GetRandom( MessageCategory.JoinMessage ) );

            bool keepRunning = true;

            // rate limit is 100 API requests per 5 minutes
            int minWaitTimeSeconds = 5 * 60 / 100;
            DateTime lastGotDateTime = DateTime.UtcNow.Subtract( new TimeSpan( 0, 0, 5 ) );

            while ( keepRunning )
            {
                try
                {
                    System.Threading.Thread.Sleep( ( minWaitTimeSeconds * 2 ) * 1000 );
                    Debug.WriteLine( DateTime.Now );

                    List<HipChat.Entities.Message> recentMessageList = hipChatClient.ListHistoryAsNativeObjects().OrderBy( a => a.Date ).ToList();
                    Debug.WriteLine( "got count: " + recentMessageList.Count );
                    if ( recentMessageList.Count > 0 )
                    {
                        var lastMessage = recentMessageList.Last();
                        Debug.WriteLine( "LastMessage: " + lastMessage.Date );
                        Debug.WriteLine( "LastMessage: " + lastMessage.Text );
                        recentMessageList = recentMessageList.Where( a => a.Date > lastGotDateTime ).Where( a => a.From.Name != botName ).ToList();
                        lastGotDateTime = lastMessage.Date;
                    }

                    Debug.WriteLine( "filtered count:" + recentMessageList.Count );
                    Debug.WriteLine( "lastGotDateTime:" + lastGotDateTime );

                    foreach ( var messageItem in recentMessageList )
                    {
                        Debug.WriteLine( messageItem.Text );
                        MessageCategory messageCategory = GetMessageCategory( messageItem.Text );
                        if ( messageCategory != MessageCategory.Unknown )
                        {
                            string enumName = Enum.GetName( typeof( MessageCategory ), messageCategory );
                            if ( enumName.StartsWith( "RandomAnswer" ) )
                            {
                                hipChatClient.SendMessage( ResponseMessageDatabase.GetRandom( messageCategory ) );
                                continue;
                            }

                            List<decimal> numericParts;
                            decimal numericAnswer = 0;

                            switch ( messageCategory )
                            {
                                case MessageCategory.MathAdd:
                                    numericParts = messageItem.Text.Split( new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => decimal.Parse( a ) ).ToList();
                                    foreach ( var decimalVal in numericParts )
                                    {
                                        numericAnswer += decimalVal;
                                    }
                                    if ( numericAnswer > 0 )
                                    {
                                        hipChatClient.SendMessage( numericAnswer.ToString() );
                                    }
                                    break;
                                case MessageCategory.MathDivide:
                                    numericParts = messageItem.Text.Split( new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => decimal.Parse( a ) ).ToList();
                                    numericAnswer = numericParts.FirstOrDefault();
                                    if ( numericAnswer > 0 )
                                    {
                                        numericParts.RemoveAt( 0 );
                                        foreach ( var decimalVal in numericParts )
                                        {
                                            if ( decimalVal == 0 )
                                            {
                                                // don't do division with zeros
                                                break;
                                            }
                                            else
                                            {
                                                numericAnswer = numericAnswer / decimalVal;
                                            }
                                        }
                                    }
                                    if ( numericAnswer > 0 )
                                    {
                                        hipChatClient.SendMessage( numericAnswer.ToString() );
                                    }
                                    break;
                                case MessageCategory.MathMultiply:
                                    numericParts = messageItem.Text.Split( new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => decimal.Parse( a ) ).ToList();
                                    numericAnswer = numericParts.FirstOrDefault();
                                    numericParts.RemoveAt( 0 );
                                    foreach ( var decimalVal in numericParts )
                                    {
                                        numericAnswer = numericAnswer * decimalVal;
                                    }

                                    hipChatClient.SendMessage( numericAnswer.ToString() );

                                    break;
                                case MessageCategory.MathSubtract:
                                    numericParts = messageItem.Text.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => decimal.Parse( a ) ).ToList();
                                    numericAnswer = numericParts.FirstOrDefault();
                                    numericParts.RemoveAt( 0 );
                                    foreach ( var decimalVal in numericParts )
                                    {
                                        numericAnswer -= decimalVal;
                                    }

                                    hipChatClient.SendMessage( numericAnswer.ToString() );

                                    break;
                                case MessageCategory.SpecificWhatTimeIsIt:
                                    hipChatClient.SendMessage( "I think it is about " + DateTime.Now.ToLongTimeString() );
                                    break;
                                case MessageCategory.Quit:
                                    hipChatClient.SendMessage( "I quit :(" );
                                    return;
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
