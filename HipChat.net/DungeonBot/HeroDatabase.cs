using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DungeonBot
{
    /// <summary>
    /// Hero class that defines a player
    /// </summary>
    [Serializable]
    [DataContract]
    public class HeroDatabase : List<ResponseMessage>
    {
        /// <summary>
        /// The file name
        /// </summary>
        const string fileName = "HeroDatabase.xml";

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            HeroDatabase cleanedDB = new HeroDatabase();
            cleanedDB.AddRange(this.Where(a => !string.IsNullOrWhiteSpace(a.Message)).ToList());
            cleanedDB.TrimExcess();

            DataContractSerializer s = new DataContractSerializer(this.GetType());
            FileStream fs = new FileStream(fileName, FileMode.Create);
            s.WriteObject(fs, cleanedDB);
            fs.Close();
        }

        /// <summary>
        /// Gets or sets the _response message database.
        /// </summary>
        /// <value>
        /// The _response message database.
        /// </value>
        private static HeroDatabase _responseMessageDatabase { get; set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static HeroDatabase Load(bool forceReload = false)
        {
            if (_responseMessageDatabase != null && forceReload == false)
            {
                return _responseMessageDatabase;
            }

            if (File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                try
                {
                    DataContractSerializer s = new DataContractSerializer(typeof(HeroDatabase));
                    _responseMessageDatabase = s.ReadObject(fs) as HeroDatabase;
                    return _responseMessageDatabase;
                }
                finally
                {
                    fs.Close();
                }
            }

            // file doesn't exist, create and reload
            _responseMessageDatabase = new HeroDatabase();
            _responseMessageDatabase.Save();

            return _responseMessageDatabase;
        }
    }
}
