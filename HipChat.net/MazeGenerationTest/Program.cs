using System;
using System.Text;
using System.Threading;

using DungeonBot;

namespace MazeGenerationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // set title
            Console.Title = "Dungeon Generation Test";
            bool appRunning = true;

            while ( appRunning == true )
            {
                // create maze and draw to console
                Dungeon dungeon = new Dungeon();
                dungeon.DrawDungeon();

                Console.Clear();
                dungeon.GenerateDungeon();
                dungeon.DrawDungeon();

                if ( dungeon.ValidDungeon() == true )
                {
                    Console.WriteLine("\nValidation passed!");
                }
                else
                {
                    Console.WriteLine("\nValidation failed");
                }

                Console.WriteLine("\nPress Enter to generate a new maze...");
                Console.Read();
            }
        }
    }
}
