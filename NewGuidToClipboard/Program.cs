using System;
using System.Windows;

namespace NewGuidToClipboard
{
    
    class Program
    {
        [STAThread]
        static void Main( string[] args )
        {
            string newguid = Guid.NewGuid().ToString( "D" ).ToUpper();
            Clipboard.SetText( newguid );
            Console.WriteLine( "New Guid {0} is now in your clipboard", newguid );
        }
    }
}
