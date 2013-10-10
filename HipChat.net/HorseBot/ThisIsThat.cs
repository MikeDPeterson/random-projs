using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace HorseBot
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ThisIsThatMessage
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Term { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Definition { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ThisIsThat : List<ThisIsThatMessage>
    {
        /// <summary>
        /// The file name
        /// </summary>
        const string fileName = "ThisIsThatMessageDatabase.xml";

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            ThisIsThat cleanedDB = new ThisIsThat();

            // keep only where both the Term and Definition are not blank
            cleanedDB.AddRange( this.Where( a => !string.IsNullOrWhiteSpace( a.Term ) && !string.IsNullOrWhiteSpace( a.Definition ) ).ToList() );
            cleanedDB.TrimExcess();

            DataContractSerializer s = new DataContractSerializer( this.GetType() );
            FileStream fs = new FileStream( fileName, FileMode.Create );
            s.WriteObject( fs, cleanedDB );
            fs.Close();
        }

        /// <summary>
        /// Gets or sets the ThisIsThat message database.
        /// </summary>
        /// <value>
        /// The ThisIsThat message database.
        /// </value>
        private static ThisIsThat _thisIsThat { get; set; }

        /// <summary>
        /// Determines whether the specified term contains term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>
        ///   <c>true</c> if the specified term contains term; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTerm( string term )
        {
            return GetEntry( term ) != null;
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string GetDefinition( string term )
        {
            var entry = GetEntry( term );
            if ( entry != null )
            {
                return entry.Definition;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string GetConfusedResponse( string term )
        {
            var entry = GetEntry( term );
            if ( entry != null )
            {
                // change "Blah blah blah" to "blah blah blah", but leave ""BLAH blah blah" alone since it probably is supposed to stay uppercased
                string firstWordOfTerm = term.SplitChar( ' ' ).First();
                if ( FirstCharToLower( firstWordOfTerm ) == firstWordOfTerm.ToLower() )
                {
                    term = FirstCharToLower( term );
                }

                return "But, I thought " + term + " was " + entry.Definition;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        private static ThisIsThatMessage GetEntry( string term )
        {
            var db = ThisIsThat.Load();

            var entry = db.FirstOrDefault( a => a.Term.Equals( term, StringComparison.OrdinalIgnoreCase ) );
            return entry;
        }

        /// <summary>
        /// Gets the straight response.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string GetStraightResponse( string term )
        {
            var entry = GetEntry( term );
            if ( entry != null )
            {
                return FirstCharToUpper( term ) + " is " + entry.Definition;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Firsts the char to upper.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static string FirstCharToUpper( string input )
        {
            if ( String.IsNullOrEmpty( input ) )
            {
                return null;
            }

            return input.First().ToString().ToUpper() + String.Join( "", input.Skip( 1 ) );
        }

        /// <summary>
        /// Firsts the char to lower.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static string FirstCharToLower( string input )
        {
            if ( String.IsNullOrEmpty( input ) )
            {
                return null;
            }

            return input.First().ToString().ToLower() + String.Join( "", input.Skip( 1 ) );
        }

        /// <summary>
        /// Adds the update.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="definition">The definition.</param>
        public void AddUpdate( string term, string definition )
        {
            var db = ThisIsThat.Load();

            var entry = GetEntry( term );

            if ( entry != null )
            {
                entry.Definition = definition.Trim();
            }
            else
            {
                entry = new ThisIsThatMessage { Term = term.Trim(), Definition = definition.Trim() };
                db.Add( entry );
            }


            db.Save();
            ThisIsThat.Load( true );
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static ThisIsThat Load( bool forceReload = false )
        {
            if ( _thisIsThat != null && forceReload == false )
            {
                return _thisIsThat;
            }

            if ( File.Exists( fileName ) )
            {
                FileStream fs = new FileStream( fileName, FileMode.OpenOrCreate );
                try
                {
                    DataContractSerializer s = new DataContractSerializer( typeof( ThisIsThat ) );
                    _thisIsThat = s.ReadObject( fs ) as ThisIsThat;
                    return _thisIsThat;
                }
                finally
                {
                    fs.Close();
                }
            }

            // file doesn't exist, create and seed and reload
            _thisIsThat = new ThisIsThat();
            _thisIsThat.Save();

            return _thisIsThat;
        }
    }
}
