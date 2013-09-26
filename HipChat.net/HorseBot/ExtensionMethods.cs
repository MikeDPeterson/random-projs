using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HorseBot
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Splits the case.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string SplitCase( this string str )
        {
            if ( str == null )
                return null;

            return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
        }

        /// <summary>
        /// Splits the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static string[] SplitChar( this string str, char c)
        {
            return str.Split( new char[] { c }, StringSplitOptions.RemoveEmptyEntries );
        }

        public static string TrimStartChar(this string str, char c)
        {
            return str.TrimStart( new char[] { c } );
        }
    }
}
