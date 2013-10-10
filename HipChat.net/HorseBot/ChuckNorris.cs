using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HorseBot
{
    /// <summary>
    /// http://api.icndb.com/jokes/random
    /// </summary>
    public class ChuckNorris
    {
        public string GetRandom(string fullName)
        {
            
            string requestUri = "http://api.icndb.com/jokes/random";

            string[] nameParts = fullName.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
            if ( nameParts.Length == 2 )
            {
                requestUri += string.Format( "?firstName={0}&lastName={1}", nameParts[0], nameParts[1] );
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create( requestUri );

            using ( HttpWebResponse response = (HttpWebResponse)request.GetResponse() )
            {
                using ( Stream receiveStream = response.GetResponseStream() )
                {
                    Encoding encode = System.Text.Encoding.GetEncoding( "utf-8" );
                    using ( StreamReader readStream = new StreamReader( receiveStream, encode ) )
                    {
                        string jsonResult = readStream.ReadToEnd();
                        var jr = System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(new MemoryStream(UTF8Encoding.Default.GetBytes(jsonResult)), new System.Xml.XmlDictionaryReaderQuotas());
                        while ( jr.Read() )
                        {
                            if ( jr.Name.Equals( "joke" ) )
                            {
                                jr.Read();
                                return jr.Value;
                            }
                        }

                        return "oops, I got nothin'";
                    }
                }
            }
        }
    }
}
