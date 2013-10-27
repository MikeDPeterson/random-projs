using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerationTest
{
    /// <summary>
    /// class for generating and drawing mazes
    /// </summary>
    public class Maze
    {
        public int _mazeSize = 20;
        public _roomType[,] _mazeLayout = new _roomType[0, 0];
        
        public enum _roomType
        {
            Blocked = 0,

            Passable = 1,

            StartRoom = 2,

            BossRoom = 3
        }

        public Maze()
        {
            _mazeLayout = new _roomType[_mazeSize, _mazeSize];
            GenerateMaze( _mazeSize);
        }

        public void GenerateMaze( int mazeSize )
        {
            Random rnd = new Random();
            int rndRoomType = rnd.Next(0,3);

            for (int y = 0; y < mazeSize; y++)
            {
                for (int x = 0; x < mazeSize; x++ )
                {
                    _mazeLayout[x,y] = (_roomType)rndRoomType;
                    rndRoomType = rnd.Next(0, 3);
                }
            }
            
            //return _roomType;
        }

        public void DrawMaze()
        {
            // clear console before drawing
            Console.Clear();

            for (int y = 0; y < _mazeSize; y++)
            {
                for (int x = 0; x < _mazeSize; x++)
                {
                    Console.Write((int)_mazeLayout[x,y]);
                }
                Console.WriteLine();
            }
        }
    }
}
