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
    public class WeatherAPI
    {

        /// <summary>
        /// Gets the weather XML.
        /// </summary>
        /// <example>
        /// http://api.openweathermap.org/data/2.5/weather?q=London&mode=xml&units=imperial
        /// </example>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string GetWeatherXml( string term )
        {
            string termEncoded = HttpUtility.UrlEncode(term);

            string requestUri = string.Format( "http://api.openweathermap.org/data/2.5/weather?q={0}&mode=xml&units=imperial", termEncoded);

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
        public string GetResponseHtml( string xmlResponse )
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml( xmlResponse );
            }
            catch
            {
                return null;
            }

            var entryNode = doc.SelectSingleNode( "current" );
            if ( entryNode == null )
            {
                return null;
            }

            string resultHtml = string.Empty;

            var cityNode = entryNode.SelectSingleNode( "city" );
            if ( cityNode != null )
            {
                /*
                 
                 
                 */
                resultHtml += "<b>" + cityNode.Attributes["name"].Value + "</b><br />";
                var coord = cityNode.SelectSingleNode( "coord" ); 
                if ( coord != null )
                {

                    resultHtml += string.Format( "<a href='https://maps.google.com/?q={0},{1}&lci=weather'>map</a><br />", coord.Attributes["lat"].Value, coord.Attributes["lon"].Value );
                }
            }

            var temperatureNode = entryNode.SelectSingleNode( "temperature" );
            if ( temperatureNode != null )
            {
                resultHtml += temperatureNode.Attributes["value"].Value + "F<br />";
            }


            var humidityNode = entryNode.SelectSingleNode( "humidity" );
            if ( humidityNode != null )
            {
                resultHtml += humidityNode.Attributes["value"].Value + "% humidity<br />";
            }

            var windNode = entryNode.SelectSingleNode( "wind" );
            if ( windNode != null )
            {
                try
                {
                    resultHtml += windNode.SelectSingleNode( "speed" ).Attributes["name"].Value + " from the " + windNode.SelectSingleNode( "direction" ).Attributes["name"].Value + "<br />";
                }
                catch
                {
                    //
                }
            }

            var weatherNode = entryNode.SelectSingleNode( "weather" );
            if ( weatherNode != null )
            {
                resultHtml += "Weather: " + weatherNode.Attributes["value"].Value;
            }

            return resultHtml;
            
        }
    }
}
