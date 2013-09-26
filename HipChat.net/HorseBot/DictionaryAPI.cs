using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace HorseBot
{
    public class DictionaryAPI
    {
        public const string apiKeyDictionary = "f77535fb-3e90-46b5-a36b-f3cb962d5d32";
        public const string apiKeyThesaurus = "8388d1d7-0336-4700-a3ea-680cf376e246";

        /// <summary>
        /// Gets the definition response XML.
        /// </summary>
        /// <example>
        /// http://www.dictionaryapi.com/api/v1/references/thesaurus/xml/test?key=8388d1d7-0336-4700-a3ea-680cf376e246
        /// </example>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string GetDefinitionResponseXml( string term )
        {
            string termEncoded = HttpUtility.UrlEncode(term);

            string requestUri = string.Format( "http://www.dictionaryapi.com/api/v1/references/collegiate/xml/{0}?key={1}", termEncoded, apiKeyDictionary );

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create( requestUri );

            using ( HttpWebResponse response = (HttpWebResponse)request.GetResponse() )
            {
                using ( Stream receiveStream = response.GetResponseStream() )
                {
                    Encoding encode = System.Text.Encoding.GetEncoding( "utf-8" );
                    using ( StreamReader readStream = new StreamReader( receiveStream, encode ) )
                    {
                        return readStream.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first definition.
        /// </summary>
        /// <param name="xmlResponse">The XML response.</param>
        /// <returns></returns>
        public string GetFirstDefinitionHtml( string xmlResponse )
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml( xmlResponse );
            var entryNode = doc.SelectSingleNode( "entry_list/entry[1]" );
            if ( entryNode == null )
            {
                return null;
            }

            var wordNode = entryNode.SelectSingleNode( "ew" );
            var defNode = entryNode.SelectSingleNode( "def" );
            string definitionReponse = string.Empty;
            int senseCount = 0;
            foreach (var node in defNode.ChildNodes.OfType<XmlNode>())
            {
                if ( node.Name == "dt" )
                {
                    definitionReponse += node.InnerXml.Trim()
                        .TrimStartChar(':')
                        .Replace("<sx>", "<pre>")
                        .Replace( "</sx>", "</pre>" )
                        .Replace("<it>", "<i>")
                        .Replace( "</it>", "</i>" )
                        + Environment.NewLine;
                }
                else if ( node.Name == "sd" )
                {
                    // SENSE DIVIDER
                    definitionReponse += "<b> - " + node.InnerText.Trim() + " - </b>" + Environment.NewLine;
                }
                else if ( node.Name == "sn" )
                {
                    // SENSE NUMBER
                    senseCount++;
                    if ( senseCount > 1 )
                    {
                        break;
                    }
                }
            }

            return string.Format( "{0} - {1}", wordNode.InnerText, definitionReponse ).Replace( "\n", "<br />" ).Replace( "\r", string.Empty );
        }
    }
}
