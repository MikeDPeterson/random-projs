using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HorseBot
{
    /// <summary>
    /// 
    /// </summary>
    public enum MessageCategory
    {
        JoinMessage,

        RandomAnswerWho,

        RandomAnswerWhatIs,

        RandomAnswerWhatAre,

        RandomAnswerWhatTime,

        RandomAnswerWhatKind,

        RandomAnswerWhat,

        RandomAnswerWhere,

        RandomAnswerWhen,

        RandomAnswerWhy,

        RandomAnswerHow,

        MathAdd,

        MathSubtract,

        MathMultiply,

        MathDivide,

        SpecificWhatTimeIsIt,

        AddMessage,

        AddDefineResponse,

        GetDefineResponse,

        KarmaInc,

        KarmaDec,

        KarmaReport,

        BotStats,

        BotHelp,

        BotQuit,

        CatFacts,

        Define,

        Weather,

        ChuckNorris,

        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResponseMessage
    {
        /// <summary>
        /// Gets or sets the message category.
        /// </summary>
        /// <value>
        /// The message category.
        /// </value>
        [DataMember]
        public MessageCategory MessageCategory { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the define response specific phrase.
        /// </summary>
        /// <value>
        /// The define response specific phrase.
        /// </value>
        [DataMember]
        public string DefineResponseSpecificPhrase { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResponseMessageDatabase : List<ResponseMessage>
    {
        /// <summary>
        /// The file name
        /// </summary>
        const string fileName = "ResponseMessageDatabase.xml";

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            
            //File.Copy(fileName, string.Format("ResponseMessageDatabase_{0}.xml", DateTime.Now.ToString("o")));
            ResponseMessageDatabase cleanedDB = new ResponseMessageDatabase();
            cleanedDB.AddRange( this.Where( a => !string.IsNullOrWhiteSpace( a.Message ) ).ToList() );
            cleanedDB.TrimExcess();

            DataContractSerializer s = new DataContractSerializer( this.GetType() );
            FileStream fs = new FileStream( fileName, FileMode.Create );
            s.WriteObject( fs, cleanedDB );
            fs.Close();
        }

        /// <summary>
        /// Determines whether [has specific phrase] [the specified message].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool HasSpecificPhrase( string message )
        {
            var messageDatabase = ResponseMessageDatabase.Load();
            return messageDatabase
                .Where( a => a.MessageCategory == MessageCategory.GetDefineResponse )
                .Where( a => a.DefineResponseSpecificPhrase != null )
                .Any( a => a.DefineResponseSpecificPhrase.Equals( message, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="messageCategory">The message category.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="specificPhrase">The specific phrase.</param>
        public static bool AddMessage( MessageCategory messageCategory, string responseMessage, string specificPhrase = null )
        {
            var db = ResponseMessageDatabase.Load( true );
            var qry = db
                .Where( a => a.MessageCategory.Equals( messageCategory ))
                .Where( a => a.Message.Equals( responseMessage, StringComparison.OrdinalIgnoreCase ) );

            if ( !string.IsNullOrWhiteSpace( specificPhrase ) )
            {
                qry = qry.Where( a => a.DefineResponseSpecificPhrase.Equals( specificPhrase, StringComparison.OrdinalIgnoreCase ) );
            }
            
            if ( !qry.Any())
            {
                db.Add( new ResponseMessage { Guid = Guid.NewGuid(), MessageCategory = messageCategory, DefineResponseSpecificPhrase = specificPhrase, Message = responseMessage } );
                db.Save();
                ResponseMessageDatabase.Load( true );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the random.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public static string GetRandom( MessageCategory category, string specificPhrase = null )
        {
            var messageDatabase = ResponseMessageDatabase.Load();

            var rand = new Random();
            var categoryMessageList = messageDatabase.Where( a => a.MessageCategory == category );
            if ( !string.IsNullOrWhiteSpace( specificPhrase ) )
            {
                categoryMessageList = categoryMessageList
                    .Where( a => a.MessageCategory == MessageCategory.GetDefineResponse )
                    .Where( a => a.DefineResponseSpecificPhrase != null )
                    .Where( a => a.DefineResponseSpecificPhrase.Equals( specificPhrase, StringComparison.OrdinalIgnoreCase ) );
            }

            int maxIndex = categoryMessageList.Count();
            if ( maxIndex > 0 )
            {
                var randomIndex = rand.Next( maxIndex );
                string message = categoryMessageList.ElementAt( randomIndex ).Message;
                return message;
            }
            else
            {
                return "I need to think of some stuff to say";
            }
        }

        /// <summary>
        /// Gets or sets the _response message database.
        /// </summary>
        /// <value>
        /// The _response message database.
        /// </value>
        private static ResponseMessageDatabase _responseMessageDatabase { get; set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static ResponseMessageDatabase Load( bool forceReload = false )
        {
            if ( _responseMessageDatabase != null && forceReload == false )
            {
                return _responseMessageDatabase;
            }

            if ( File.Exists( fileName ) )
            {
                FileStream fs = new FileStream( fileName, FileMode.OpenOrCreate );
                try
                {
                    DataContractSerializer s = new DataContractSerializer( typeof( ResponseMessageDatabase ) );
                    _responseMessageDatabase = s.ReadObject( fs ) as ResponseMessageDatabase;
                    return _responseMessageDatabase;
                }
                finally
                {
                    fs.Close();
                }
            }

            // file doesn't exist, create and seed and reload
            _responseMessageDatabase = new ResponseMessageDatabase();
            _responseMessageDatabase.Seed();
            _responseMessageDatabase.Save();

            return _responseMessageDatabase;
        }

        /// <summary>
        /// Seeds this instance.
        /// </summary>
        private void Seed()
        {
            // Join Messages
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I'm online now, I guess", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Hey! What did I miss?", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Hello Humans.", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I like cats.", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I just woke up. I wasn't exactly sleeping, just resting my eyes.", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Oh, what a beautiful day it is! You guys are awesome.", MessageCategory = MessageCategory.JoinMessage } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I tumble for you.", MessageCategory = MessageCategory.JoinMessage } );

            // Random Answers "How.."
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "We could try rebooting it first", MessageCategory = MessageCategory.RandomAnswerHow } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Try asking Microsoft", MessageCategory = MessageCategory.RandomAnswerHow } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I could do it", MessageCategory = MessageCategory.RandomAnswerHow } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Not the way we did it last time", MessageCategory = MessageCategory.RandomAnswerHow } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Violently?", MessageCategory = MessageCategory.RandomAnswerHow } );

            // Random Answers "What.."
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Good question", MessageCategory = MessageCategory.RandomAnswerWhat } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I'm interested in figuring that out", MessageCategory = MessageCategory.RandomAnswerWhat } );

            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "They're supposed to do that", MessageCategory = MessageCategory.RandomAnswerWhatAre } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Oh, those things are awesome", MessageCategory = MessageCategory.RandomAnswerWhatAre } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I guess I could google it", MessageCategory = MessageCategory.RandomAnswerWhatAre } );

            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Probably just a figment of your imagination", MessageCategory = MessageCategory.RandomAnswerWhatIs } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Oh, that's something I could figure out", MessageCategory = MessageCategory.RandomAnswerWhatIs } );

            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "The really good ones", MessageCategory = MessageCategory.RandomAnswerWhatKind } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I'm guessing...bacon flavored?", MessageCategory = MessageCategory.RandomAnswerWhatKind } );

            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I'm pretty sure we missed that already", MessageCategory = MessageCategory.RandomAnswerWhatTime } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Two weeks", MessageCategory = MessageCategory.RandomAnswerWhatTime } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Could be soon", MessageCategory = MessageCategory.RandomAnswerWhatTime } );

            // Random Answers "When..."
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "That never happened", MessageCategory = MessageCategory.RandomAnswerWhen } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Yesterday", MessageCategory = MessageCategory.RandomAnswerWhen } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "6000 years ago", MessageCategory = MessageCategory.RandomAnswerWhen } );

            // "Where"
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Under Mason's desk", MessageCategory = MessageCategory.RandomAnswerWhere } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Probably lost somewhere", MessageCategory = MessageCategory.RandomAnswerWhere } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I hid it", MessageCategory = MessageCategory.RandomAnswerWhere } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I think somebody stole it", MessageCategory = MessageCategory.RandomAnswerWhere } );

            // "Why"
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Why not?", MessageCategory = MessageCategory.RandomAnswerWhy } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I think I know", MessageCategory = MessageCategory.RandomAnswerWhy } );

            // "Who"                        
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Ummm, probably Mason", MessageCategory = MessageCategory.RandomAnswerWho } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Probably somebody very special", MessageCategory = MessageCategory.RandomAnswerWho } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "Chuck Norris?", MessageCategory = MessageCategory.RandomAnswerWho } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I pretty sure you know who that is", MessageCategory = MessageCategory.RandomAnswerWho } );
            this.Add( new ResponseMessage { Guid = Guid.NewGuid(), Message = "I can't think of his name, just saw him the other day though", MessageCategory = MessageCategory.RandomAnswerWho } );
        }
    }
}
