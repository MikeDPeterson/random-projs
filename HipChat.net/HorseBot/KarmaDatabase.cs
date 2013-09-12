using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HorseBot
{
    [Serializable]
    [DataContract]
    public class KarmaPhraseStats
    {
        [DataMember]
        public string Phrase { get; set; }

        [DataMember]
        public int IncCount { get; set; }

        [DataMember]
        public int DecCount { get; set; }

        [DataMember]
        public DateTime LastUpdate { get; set; }

        public int Score
        {
            get
            {
                return IncCount - DecCount;
            }
        }

        public string ScoreSummary
        {
            get
            {
                return string.Format( "{0} : ++{1} --{2} Total:{3} ", Phrase,  IncCount, DecCount, Score );
            }
        }
    }

    [Serializable]
    [DataContract]
    public class KarmaDatabase : List<KarmaPhraseStats>
    {
        /// <summary>
        /// The file name
        /// </summary>
        const string fileName = "KarmaDatabase.xml";

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            DataContractSerializer s = new DataContractSerializer( this.GetType() );
            FileStream fs = new FileStream( fileName, FileMode.Create );
            s.WriteObject( fs, this );
            fs.Close();
        }

        /// <summary>
        /// Gets the karma item by phrase.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <returns></returns>
        private KarmaPhraseStats GetKarmaItemByPhrase( string phrase )
        {
            KarmaPhraseStats karmaPhraseStats = this.FirstOrDefault( a => a.Phrase.Equals( phrase.Trim(), StringComparison.OrdinalIgnoreCase ) );

            if ( karmaPhraseStats == null )
            {
                karmaPhraseStats = new KarmaPhraseStats { Phrase = phrase.Trim() };
                this.Add( karmaPhraseStats );
            }
            return karmaPhraseStats;
        }

        /// <summary>
        /// Incs the karma.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        public void IncKarma( string phrase )
        {
            KarmaPhraseStats karmaPhraseStats = GetKarmaItemByPhrase( phrase );
            karmaPhraseStats.IncCount++;
            Save();
        }

        /// <summary>
        /// Decimals the karma.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        public void DecKarma( string phrase )
        {
            KarmaPhraseStats karmaPhraseStats = GetKarmaItemByPhrase( phrase );
            karmaPhraseStats.DecCount++;
            Save();
        }

        /// <summary>
        /// Gets the karma score summary.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <returns></returns>
        public string GetKarmaScoreSummary( string phrase )
        {
            KarmaPhraseStats karmaPhraseStats = GetKarmaItemByPhrase( phrase );
            return karmaPhraseStats.ScoreSummary;
        }

        /// <summary>
        /// Gets or sets the _response message database.
        /// </summary>
        /// <value>
        /// The _response message database.
        /// </value>
        private static KarmaDatabase _karmaDatabase { get; set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static KarmaDatabase Load( bool forceReload = false )
        {
            if ( _karmaDatabase != null && forceReload == false )
            {
                return _karmaDatabase;
            }

            if ( File.Exists( fileName ) )
            {
                FileStream fs = new FileStream( fileName, FileMode.OpenOrCreate );
                try
                {
                    DataContractSerializer s = new DataContractSerializer( typeof( KarmaDatabase ) );
                    _karmaDatabase = s.ReadObject( fs ) as KarmaDatabase;
                    return _karmaDatabase;
                }
                finally
                {
                    fs.Close();
                }
            }

            // file doesn't exist, create 
            _karmaDatabase = new KarmaDatabase();
            _karmaDatabase.Save();

            return _karmaDatabase;
        }
    }
}
