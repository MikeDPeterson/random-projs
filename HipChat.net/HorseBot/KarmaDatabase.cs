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
    [Serializable]
    [DataContract]
    public class KarmaPhraseStats
    {
        /// <summary>
        /// Gets or sets the phrase.
        /// </summary>
        /// <value>
        /// The phrase.
        /// </value>
        [DataMember]
        public string Phrase { get; set; }

        /// <summary>
        /// Gets or sets the inc count.
        /// </summary>
        /// <value>
        /// The inc count.
        /// </value>
        [DataMember]
        public int IncCount { get; set; }

        /// <summary>
        /// Gets or sets the dec count.
        /// </summary>
        /// <value>
        /// The dec count.
        /// </value>
        [DataMember]
        public int DecCount { get; set; }

        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>
        /// The last update.
        /// </value>
        [DataMember]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get
            {
                return IncCount - DecCount;
            }
        }

        /// <summary>
        /// Gets the score summary.
        /// </summary>
        /// <value>
        /// The score summary.
        /// </value>
        public string ScoreSummary
        {
            get
            {
                return string.Format( "{0} : ++{1} --{2} Total:{3} ", Phrase,  IncCount, DecCount, Score );
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
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
            KarmaDatabase cleanedDB = new KarmaDatabase();
            cleanedDB.AddRange(this.Where( a => !string.IsNullOrWhiteSpace( a.Phrase ) ));
            cleanedDB.TrimExcess();
            DataContractSerializer s = new DataContractSerializer( this.GetType() );
            FileStream fs = new FileStream( fileName, FileMode.Create );
            s.WriteObject( fs, cleanedDB );
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
        public static void IncKarma( string phrase )
        {
            var db = KarmaDatabase.Load();
            KarmaPhraseStats karmaPhraseStats = db.GetKarmaItemByPhrase( phrase );
            karmaPhraseStats.IncCount++;
            db.Save();
        }

        /// <summary>
        /// Decimals the karma.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        public static void DecKarma( string phrase )
        {
            var db = KarmaDatabase.Load();
            KarmaPhraseStats karmaPhraseStats = db.GetKarmaItemByPhrase( phrase );
            karmaPhraseStats.DecCount++;
            db.Save();
        }

        /// <summary>
        /// Gets the karma score summary.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <returns></returns>
        public static string GetKarmaScoreSummary( string phrase )
        {
            var db = KarmaDatabase.Load();
            KarmaPhraseStats karmaPhraseStats = db.GetKarmaItemByPhrase( phrase );
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
                try
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
                catch
                {
                    // just create a new one
                }
            }

            // file doesn't exist, create 
            _karmaDatabase = new KarmaDatabase();
            _karmaDatabase.Save();

            return _karmaDatabase;
        }
    }
}
