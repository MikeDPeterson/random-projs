using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HorseBot
{
    /// <summary>
    /// http://catfacts-api.appspot.com/doc.html
    /// </summary>
    public class CatFacts
    {

        /// <summary>
        /// Gets the random cat fact.
        /// </summary>
        /// <returns></returns>
        public string GetRandomCatFact()
        {
            string requestUri = "http://catfacts-api.appspot.com/api/facts?number=1";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create( requestUri );

            // only wait 10 seconds
            request.Timeout = 10000;

            using ( HttpWebResponse response = (HttpWebResponse)request.GetResponse() )
            {
                using ( Stream receiveStream = response.GetResponseStream() )
                {
                    Encoding encode = System.Text.Encoding.GetEncoding( "utf-8" );
                    using ( StreamReader readStream = new StreamReader( receiveStream, encode ) )
                    {
                        string jsonResult = readStream.ReadToEnd();
                        var jr = System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader( new MemoryStream( UTF8Encoding.Default.GetBytes( jsonResult ) ), new System.Xml.XmlDictionaryReaderQuotas() );
                        while ( jr.Read() )
                        {
                            if ( jr.Name.Equals( "item" ) )
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
