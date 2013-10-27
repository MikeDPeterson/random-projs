using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Maze Generation Test";

            Maze maze = new Maze();

            maze.DrawMaze();

            Console.Read();

        }
    }
}
