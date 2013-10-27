using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonBot
{
    /// <summary>
    /// Defines a dungeon 
    /// </summary>
    class Dungeon
    {
        /// <summary>
        /// an array that holds the dungeon layout
        /// </summary>
        public Room[,] dungeonLayout = new Room[5, 5];

        /// <summary>
        /// dungeon constructor
        /// </summary>
        public Dungeon()
        {
            // todo
        }

        /// <summary>
        /// method to generate a new dungeon
        /// </summary>
        public void GenerateDungeon()
        {
            // todo
        }
    }

    /// <summary>
    /// defines a room in the dungeon
    /// </summary>
    public class Room
    {
        /// <summary>
        /// text to describe room when the hero enters
        /// </summary>
        public string _roomDescription;


    }
}
