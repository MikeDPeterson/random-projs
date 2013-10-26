using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DungeonBot
{
    /// <summary>
    /// 
    /// </summary>
    public enum MessageCategory
    {
        JoinMessage,

        BotStats,

        BotHelp,

        BotQuit,

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
            _responseMessageDatabase.Save();

            return _responseMessageDatabase;
        }
    }
}
