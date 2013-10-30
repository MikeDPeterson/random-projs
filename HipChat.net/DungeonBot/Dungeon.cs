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
    public class Dungeon
    {
        public int _dungeonSize = 6;
        
        /// <summary>
        /// an array that holds the dungeon layout
        /// </summary>
        public Room[,] _dungeonLayout = new Room[0, 0];

        /// <summary>
        /// dungeon constructor
        /// </summary>
        public Dungeon()
        {
            _dungeonLayout = new Room[_dungeonSize, _dungeonSize];
            GenerateDungeon();
        }

        /// <summary>
        /// method to generate a new dungeon
        /// </summary>
        public void GenerateDungeon()
        {
            // random generator
            Random rnd = new Random();
            int rndRoomType = rnd.Next(0, 1);

            // loop through dungeon array and create rooms
            for ( int y = 0; y < _dungeonSize; y++ )
            {
                for ( int x = 0; x < _dungeonSize; x++ )
                {
                    _dungeonLayout[x, y] = new Room();
                }
            }

            // loop through the array minus the border and fill with passable rooms
            for ( int y = 1; y < _dungeonSize - 1; y++ )
            {
                for ( int x = 1; x < _dungeonSize - 1; x++ )
                {
                    _dungeonLayout[x, y].roomType = Room.RoomType.Passable;
                }
            }

            // generate start room
            _dungeonLayout[rnd.Next(1, _dungeonSize - 1), rnd.Next(1, _dungeonSize - 1)].roomType = Room.RoomType.StartRoom;

            //generate boss room
            _dungeonLayout[rnd.Next(1, _dungeonSize - 1), rnd.Next(1, _dungeonSize - 1)].roomType = Room.RoomType.BossRoom;
        }

        /// <summary>
        /// validates a dungeon  
        /// </summary>
        public bool ValidDungeon()
        {
            int bossRoomCount = 0;
            int startRoomCount = 0;
            int bossLocationX = 0;
            int bossLocationY = 0;
            int startLocationX = 0;
            int startLocationY = 0;

            for ( int y = 0; y < _dungeonSize; y++ )
            {
                for ( int x = 0; x < _dungeonSize; x++ )
                {
                    if ( _dungeonLayout[x, y].roomType == Room.RoomType.BossRoom )
                    {
                        bossLocationX = x;
                        bossLocationY = y;
                        bossRoomCount++;
                    }
                    else if ( _dungeonLayout[x, y].roomType == Room.RoomType.StartRoom )
                    {
                        startLocationX = x;
                        startLocationY = y;
                        startRoomCount++;
                    }
                }
            }

            // checks for one start, one boss room and they are spaced appropriately
            if ( bossRoomCount < 1 |
                startRoomCount < 1 |
                ( Math.Pow(bossLocationX - startLocationX, 2) +
                Math.Pow(bossLocationY - startLocationY, 2) ) < 3 )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// draws the dungeon
        /// </summary>
        public void DrawDungeon()
        {
            // clear console before drawing
            Console.Clear();

            for ( int y = 0; y < _dungeonSize; y++ )
            {
                for ( int x = 0; x < _dungeonSize; x++ )
                {
                    Console.Write((int)_dungeonLayout[x, y].roomType);
                }
                Console.WriteLine();
            }
        }


        /// <summary>
        /// defines a room of a dungeon
        /// </summary>
        public class Room
        {
            public RoomType roomType;

            public enum RoomType
            {
                Blocked = 0,

                Passable = 1,

                StartRoom = 2,

                BossRoom = 3,

                SecretShop = 4
            }

            public Room()
            {
                roomType = RoomType.Blocked;
            }
        }
    }
}