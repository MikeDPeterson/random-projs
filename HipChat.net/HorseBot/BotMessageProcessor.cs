using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipChat;
using HipChat.Entities;

namespace HorseBot
{
    /// <summary>
    /// 
    /// </summary>
    public class BotMessageProcessor
    {
        const string ourToken = "6e72e6e19bbb3580ea1b9751bc61f4";
        const string designersToken = "d41088f5baacd41de00d2110cb5d54";
        const string apiToken = ourToken;

        // rooms
        const int roomOfDoomId = 222901;
        const int debugRoomId = 284955;
        const int designersRoomId = 187059;

        const int currentRoomId = roomOfDoomId;

        const string botName = "HorseBot";
        HipChatClient hipChatClient = null;
        DateTime lastRandomAnswer = DateTime.MinValue;
        DateTime startupDateTime;

        DateTime lastCatFact = DateTime.MaxValue;
        CatFacts _catFacts = new CatFacts();
        double catFactMinutes = 60;

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

        /// <summary>
        /// Ares all numbers.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private bool AreAllNumbers( List<string> list )
        {
            if ( list.Count <= 1 )
            {
                return false;
            }

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

            /* Cat Facts */

            if ( message.Equals( "Cat Facts", StringComparison.OrdinalIgnoreCase ) || message.StartsWith( "Cat Facts ", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.CatFacts;
            }

            /* Karma */
            if ( message.EndsWith( "++" ) )
            {
                if ( message.Length > 2 )
                {
                    return MessageCategory.KarmaInc;
                }
            }

            if ( message.EndsWith( "--" ) )
            {
                if ( message.Length > 2 )
                {
                    return MessageCategory.KarmaDec;
                }
            }

            if ( message.StartsWith( "KarmaReport" ) || message.StartsWith( "Karma Report" ) )
            {
                return MessageCategory.KarmaReport;
            }

            /* Bot Stuff */
            if ( message.Equals( "BotStats", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.BotStats;
            }

            if ( message.Equals( "BotHelp", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.BotHelp;
            }

            if ( message.Equals( "BotQuit" ) )
            {
                return MessageCategory.BotQuit;
            }

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
            if ( message.StartsWith( "AddDefineResponse " ) )
            {
                return MessageCategory.AddDefineResponse;
            }

            if ( message.StartsWith( "AddMessage " ) )
            {
                return MessageCategory.AddMessage;
            }

            // Specific Questions
            if ( message.Equals( "What time is it?", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.SpecificWhatTimeIsIt;
            }

            /* Specific Phrase */
            if ( ResponseMessageDatabase.HasSpecificPhrase( message ) )
            {
                return MessageCategory.GetDefineResponse;
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

            /* Lookup Definition of term */

            if ( message.StartsWith( "Define ", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.Define;
            }

            if ( message.StartsWith( "Weather ", StringComparison.OrdinalIgnoreCase ) )
            {
                return MessageCategory.Weather;
            }

            if ( message.Contains("Chuck Norris") )
            {
                return MessageCategory.ChuckNorris;
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
            if ( lastCatFact < DateTime.Now )
            {
                hipChatClient.SendMessage( "Thank you for subscribing to Cat Facts!" );
            }
            else
            {
                hipChatClient.SendMessage( ResponseMessageDatabase.GetRandom( MessageCategory.JoinMessage ) );
            }

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

                    if ( ( DateTime.Now - lastCatFact ).TotalMinutes >= catFactMinutes )
                    {
                        hipChatClient.SendMessage( _catFacts.GetRandomCatFact() );
                        lastCatFact = DateTime.Now;
                    }

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

                        MessageCategory messageCategory = GetMessageCategory( messageItem );
                        if ( messageCategory != MessageCategory.Unknown )
                        {
                            /* Respond to the RandomAnswerXX class of messagecatagory */
                            string enumName = Enum.GetName( typeof( MessageCategory ), messageCategory );
                            if ( enumName.StartsWith( "RandomAnswer" ) )
                            {
                                hipChatClient.SendMessage( ResponseMessageDatabase.GetRandom( messageCategory ).Replace( "{0}", messageItem.From.FirstName ) );
                                continue;
                            }

                            switch ( messageCategory )
                            {
                                /* Math related */
                                case MessageCategory.MathAdd:
                                case MessageCategory.MathDivide:
                                case MessageCategory.MathMultiply:
                                case MessageCategory.MathSubtract:
                                    {
                                        decimal numericAnswer = 0;
                                        List<decimal> numericParts = messageItem.Text.Split( new char[] { '+', '-', '/', '*' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => decimal.Parse( a ) ).ToList();

                                        if ( messageCategory == MessageCategory.MathAdd )
                                        {
                                            // MathAdd
                                            foreach ( var decimalVal in numericParts )
                                            {
                                                numericAnswer += decimalVal;
                                            }
                                        }
                                        else if ( messageCategory == MessageCategory.MathDivide )
                                        {
                                            // MathDivide
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
                                        }
                                        else if ( messageCategory == MessageCategory.MathMultiply )
                                        {
                                            // MathMultiply
                                            numericAnswer = numericParts.FirstOrDefault();
                                            numericParts.RemoveAt( 0 );
                                            foreach ( var decimalVal in numericParts )
                                            {
                                                numericAnswer = numericAnswer * decimalVal;
                                            }
                                        }
                                        else if ( messageCategory == MessageCategory.MathSubtract )
                                        {
                                            // MathSubtract
                                            numericAnswer = numericParts.FirstOrDefault();
                                            numericParts.RemoveAt( 0 );
                                            foreach ( var decimalVal in numericParts )
                                            {
                                                numericAnswer -= decimalVal;
                                            }
                                        }

                                        hipChatClient.SendMessage( "Oh, that is probably " + numericAnswer.ToString() + ", " + messageItem.From.FirstName );
                                    }

                                    break;

                                /* Specific */
                                case MessageCategory.SpecificWhatTimeIsIt:
                                    hipChatClient.SendMessage( "I think it is about " + DateTime.Now.ToLongTimeString() );
                                    break;

                                case MessageCategory.GetDefineResponse:
                                    hipChatClient.SendMessage( ResponseMessageDatabase.GetRandom( messageCategory, messageItem.Text ).Replace( "{0}", messageItem.From.FirstName ) );
                                    break;

                                /* Programming */
                                case MessageCategory.AddDefineResponse:
                                    {
                                        /* Format is AddDefineResponse | Message | Response */
                                        List<string> msgParts = messageItem.Text.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
                                        if ( msgParts.Count() == 3 )
                                        {

                                            if ( ResponseMessageDatabase.AddMessage( MessageCategory.GetDefineResponse, msgParts[2].Trim(), msgParts[1].Trim() ) )
                                            {
                                                string response = string.Format( "Woohooo! I love DefineResponse. Thanks, {0}. Got it", messageItem.From.FirstName );
                                                hipChatClient.SendMessage( response );
                                            }
                                            else
                                            {
                                                string response = string.Format( "Ummm, {0}. I'm pretty sure I already have that exact one.", messageItem.From.FirstName );
                                                hipChatClient.SendMessage( response );
                                            }
                                        }
                                    }
                                    break;

                                case MessageCategory.AddMessage:
                                    {
                                        /* Format is AddMessage | MessageCategory | Message */
                                        List<string> msgParts = messageItem.Text.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
                                        if ( msgParts.Count() == 3 )
                                        {
                                            MessageCategory mc;
                                            if ( Enum.TryParse( msgParts[1].Trim(), out mc ) )
                                            {
                                                if ( ResponseMessageDatabase.AddMessage( mc, msgParts[2].Trim() ) )
                                                {
                                                    string response = string.Format( "Thanks, {0}! I went ahead and added that to the category {1}", messageItem.From.FirstName, Enum.GetName( typeof( MessageCategory ), mc ) );
                                                    hipChatClient.SendMessage( response );
                                                }
                                                else
                                                {
                                                    string response = string.Format( "Ummm, {0}. I'm pretty sure I already have that one.", messageItem.From.FirstName );
                                                    hipChatClient.SendMessage( response );
                                                }
                                            }
                                        }
                                    }
                                    break;

                                /* Karma */
                                case MessageCategory.KarmaInc:
                                    {
                                        KarmaDatabase.Load();
                                        KarmaDatabase.IncKarma( messageItem.Text.TrimEnd( new char[] { '+', '-' } ) );
                                    }
                                    break;

                                case MessageCategory.KarmaDec:
                                    {
                                        KarmaDatabase.Load();
                                        KarmaDatabase.DecKarma( messageItem.Text.TrimEnd( new char[] { '+', '-' } ) );
                                    }
                                    break;

                                case MessageCategory.KarmaReport:
                                    {
                                        var messageParts = messageItem.Text.Trim().SplitChar( '|' );
                                        var db = KarmaDatabase.Load();
                                        if ( messageParts.Length == 2 )
                                        {
                                            KarmaDatabase.GetKarmaScoreSummary( messageParts.Last().Trim() );
                                        }
                                        else
                                        {
                                            var top10 = db.Where( a => a.Score > 0 ).OrderByDescending( a => a.Score ).Take( 10 );
                                            var bottom10 = db.Where( a => a.Score < 0 ).OrderBy( a => a.Score ).Take( 10 );
                                            var controversial10 = db.Where( a => a.IncCount > 0 && a.DecCount > 0 ).OrderByDescending( a => a.IncCount + a.DecCount ).Take( 10 );

                                            // top 10
                                            string tableHtml = "<table>";
                                            tableHtml += "<tr><th>Phrase</th><th>++</th><th>--</th><th>total</th>";
                                            foreach ( var item in top10 )
                                            {
                                                tableHtml += string.Format( "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", item.Phrase, item.IncCount, item.DecCount, item.Score );
                                            }
                                            tableHtml += "</table>";

                                            hipChatClient.SendMessageHtml( "Top 10 " + Environment.NewLine + tableHtml );

                                            // sleep a little so they come out in the right order
                                            System.Threading.Thread.Sleep( 500 );

                                            // bottom 10
                                            tableHtml = "<table>";
                                            tableHtml += "<tr><th>Phrase</th><th>++</th><th>--</th><th>total</th>";
                                            foreach ( var item in bottom10 )
                                            {
                                                tableHtml += string.Format( "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", item.Phrase, item.IncCount, item.DecCount, item.Score );
                                            }
                                            tableHtml += "</table>";

                                            hipChatClient.SendMessageHtml( "Bottom 10 " + Environment.NewLine + tableHtml );

                                            // sleep a little so they come out in the right order
                                            System.Threading.Thread.Sleep( 500 );

                                            // controversial 10
                                            tableHtml = "<table>";
                                            tableHtml += "<tr><th>Phrase</th><th>++</th><th>--</th><th>total</th>";
                                            foreach ( var item in controversial10 )
                                            {
                                                tableHtml += string.Format( "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", item.Phrase, item.IncCount, item.DecCount, item.Score );
                                            }
                                            tableHtml += "</table>";

                                            hipChatClient.SendMessageHtml( "Controversial 10 " + Environment.NewLine + tableHtml );

                                        }
                                    }
                                    break;

                                /* Bot stuff */
                                case MessageCategory.BotStats:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( string.Format( "I've been running since {0}", startupDateTime.ToString( "F" ) ) );
                                        sb.AppendLine( string.Format( "Messages Sent {0}", hipChatClient.MessageSentCount ) );
                                        sb.AppendLine( string.Format( "Api Calls {0}", hipChatClient.ApiCallCount ) );
                                        hipChatClient.SendMessage( sb.ToString() );
                                    }
                                    break;

                                case MessageCategory.BotHelp:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( "<pre>" );
                                        sb.AppendLine( "BotHelp (shows this)" );
                                        sb.AppendLine( "define term (shows the definition of term)" );
                                        sb.AppendLine( "weather city (shows current weather for city)" );
                                        sb.AppendLine( "BotQuit (stops the bot program)" );
                                        sb.AppendLine( "BotStats (shows stats)" );
                                        sb.AppendLine( "KarmaReport | Phrase (or just KarmaReport)" );
                                        sb.AppendLine( "What time is it? (shows the current time)" );
                                        sb.AppendLine( "AddDefineResponse | some exact phrase | response when bot hears it" );
                                        sb.AppendLine( "AddMessage | JoinMessage | response when coming online" );
                                        sb.AppendLine( "AddMessage | RandomAnswerHow | response when a question starts with 'How '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhat | response when a question starts with 'What '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhatAre | response when a question starts with 'What are '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhatIs | response when a question starts with 'What is' " );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhatKind | response when a question starts with 'What kind '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhatTime | response when a question starts with 'What time '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhen | response when a question starts with 'When '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhere | response when a question starts with 'Where '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWho | response when a question starts with 'Who '" );
                                        sb.AppendLine( "AddMessage | RandomAnswerWhy | response when a question starts with 'Why '" );
                                        sb.AppendLine( "" );
                                        sb.AppendLine();
                                        sb.AppendLine( string.Format( "Want more help? Go look at the source code at {1}, {0} :)", messageItem.From.FirstName, "https://github.com/mikepetersonccv/random-projs/tree/master/HipChat.net/HorseBot" ) );
                                        sb.AppendLine( "</pre>" );
                                        hipChatClient.SendMessageHtml( sb.ToString() );
                                        // sleep a little so they come out in the right order
                                        System.Threading.Thread.Sleep( 500 );
                                    }
                                    break;

                                case MessageCategory.CatFacts:
                                    {
                                        if ( lastCatFact == DateTime.MaxValue )
                                        {
                                            lastCatFact = DateTime.MinValue;
                                            string[] messageParts = messageItem.Text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                                            if ( messageParts.Length == 3 )
                                            {
                                                double minutesParam;
                                                if ( double.TryParse( messageParts[2], out minutesParam ) )
                                                {
                                                    catFactMinutes = minutesParam;
                                                }
                                                else
                                                {
                                                    catFactMinutes = 60;
                                                }
                                            }

                                            hipChatClient.SendMessage( string.Format( "Thank you for subscribing to Cat Facts! You will get a random cat fact every {0} minutes", catFactMinutes ) );
                                        }
                                        else
                                        {
                                            hipChatClient.SendMessage( "I will stop sending you Cat Facts now." );
                                            lastCatFact = DateTime.MaxValue;
                                        }
                                    }
                                    break;

                                case MessageCategory.Define:
                                    {
                                        string term = messageItem.Text.Substring( "Define ".Length ).Trim();

                                        if ( !string.IsNullOrWhiteSpace( term ) )
                                        {
                                            DictionaryAPI dictionaryApi = new DictionaryAPI();
                                            string xmlResponse = dictionaryApi.GetDefinitionResponseXml( term );

                                            string def = dictionaryApi.GetFirstDefinitionHtml( xmlResponse );

                                            if ( string.IsNullOrWhiteSpace( def ) )
                                            {
                                                hipChatClient.SendMessage( string.Format( "Huh? Ummmm.... I have no idea what '{0}' means", term ) );
                                            }
                                            else
                                            {
                                                hipChatClient.SendMessageHtml( def );
                                            }


                                        }
                                    }
                                    break;

                                case MessageCategory.Weather:
                                    {
                                        string term = messageItem.Text.Substring( "Weather ".Length ).Trim();

                                        if ( !string.IsNullOrWhiteSpace( term ) )
                                        {
                                            WeatherAPI weatherApi = new WeatherAPI();
                                            string xmlResponse = weatherApi.GetWeatherXml( term );

                                            string weatherHtml = weatherApi.GetResponseHtml( xmlResponse );

                                            if ( string.IsNullOrWhiteSpace( weatherHtml ) )
                                            {
                                                hipChatClient.SendMessage( string.Format( "I can't get a weather report for {0}", term ) );
                                            }
                                            else
                                            {
                                                hipChatClient.SendMessageHtml( weatherHtml );
                                            }


                                        }
                                    }
                                    break;

                                case MessageCategory.ChuckNorris:
                                    {
                                        ChuckNorris chuckNorris = new ChuckNorris();
                                        if ( messageItem.Text.EndsWith( "~" ) )
                                        {
                                            hipChatClient.SendMessage( chuckNorris.GetRandom( messageItem.From.Name ) );
                                        }
                                        else
                                        {
                                            hipChatClient.SendMessage( chuckNorris.GetRandom( string.Empty ) );
                                        }
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
