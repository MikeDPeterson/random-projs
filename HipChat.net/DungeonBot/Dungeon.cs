using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DungeonBot
{
    /// <summary>
    /// Defines a dungeon 
    /// </summary>
    public class Dungeon
    {
        /// <summary>
        /// Determines size of the dungeon.
        /// </summary>
        private int _dungeonSize = 6;

        /// <summary>
        /// Current floor of the dungeon.
        /// </summary>
        private int _dungeonFloor = 1;
        
        /// <summary>
        /// An array that holds the dungeon layout.
        /// </summary>
        private Room[,] _dungeonLayout = new Room[0, 0];

        /// <summary>
        /// Dungeon constructor that validates dungeon on creation.
        /// </summary>
        public Dungeon()
        {   
            _dungeonLayout = new Room[_dungeonSize, _dungeonSize];
            bool dungeonvalidated = false;

            while ( dungeonvalidated == false )
            {
                GenerateDungeon();
                if ( ValidDungeon() == true )
                {
                    dungeonvalidated = true;
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Method to generate a new dungeon.
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

            // generate bonus room
            _dungeonLayout[rnd.Next(1, _dungeonSize - 1), rnd.Next(1, _dungeonSize - 1)].roomType = Room.RoomType.BonusRoom;

            // generate start room
            _dungeonLayout[rnd.Next(1, _dungeonSize - 1), rnd.Next(1, _dungeonSize - 1)].roomType = Room.RoomType.StartRoom;

            // generate boss room
            _dungeonLayout[rnd.Next(1, _dungeonSize - 1), rnd.Next(1, _dungeonSize - 1)].roomType = Room.RoomType.BossRoom;
        }

        /// <summary>
        /// Validates a dungeon.  
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
        /// Draws the dungeon.
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
        /// Defines a room of a dungeon.
        /// </summary>
        [Serializable]
        [DataContract]
        public class Room
        {
            /// <summary>
            /// Gets or sets the room type
            /// </summary>
            [DataMember]
            public RoomType roomType { get; set; }

            /// <summary>
            /// Defines the room types
            /// </summary>
            public enum RoomType
            {
                Blocked = 0,

                Passable = 1,

                StartRoom = 2,

                BossRoom = 3,

                BonusRoom = 4
            }

            /// <summary>
            /// Gets or sets the room narrative.
            /// </summary>
            [DataMember]
            public string roomNarrative { get; set; }

            /// <summary>
            /// Constructor that creates a room.  The default room type is blocked type.
            /// </summary>
            public Room()
            {
                roomType = RoomType.Blocked;
            }
        }

        [Serializable]
        [DataContract]
        public class RoomDatabase : List<Room>
        {
            /// <summary>
            ///  The file name.
            /// </summary>
            const string fileName = "RoomDatabase.xml";

            private static RoomDatabase _roomDatabase { get; set; }

            public static RoomDatabase Load()
            {
                if ( File.Exists( fileName ) )
                {
                    FileStream fs = new FileStream( fileName, FileMode.Open );
                    try
                    {
                        DataContractSerializer s = new DataContractSerializer(typeof(RoomDatabase));
                        _roomDatabase = s.ReadObject( fs ) as RoomDatabase;
                        return _roomDatabase;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }

                _roomDatabase = new RoomDatabase();
                _roomDatabase.Seed();
                _roomDatabase.Save();

                return _roomDatabase;
            }

            public void Save()
            {
                RoomDatabase cleanedDB = new RoomDatabase();
                cleanedDB.ToString();

                DataContractSerializer s = new DataContractSerializer(this.GetType());
                FileStream fs = new FileStream(fileName, FileMode.Create);
                s.WriteObject(fs, cleanedDB);
                fs.Close();
            }

            private void Seed()
            {
                // starting rooms
                this.Add( new Room { roomNarrative = "You are now on the {0} floor of this dungeon.  You wonder how far down does it go.", roomType = Room.RoomType.StartRoom } );

                // passable i.e. regular rooms
                this.Add( new Room { roomNarrative = "The next room is very dim and it's hard to see.  In the corner you notice something shiny in the darkness.", roomType = Room.RoomType.Passable } );

                // boos rooms
                this.Add( new Room { roomNarrative = "As you walk into the room, you notice piles of papers everywhere.  You pick up a few sheets and see that they are filled with poorly written C# code.  You conclude that is the lair of the junior software developer!", roomType = Room.RoomType.BossRoom } );
                
                // bonus rooms
                this.Add( new Room { roomNarrative = "You have discovered a secret room!  Free XP for all!", roomType = Room.RoomType.BonusRoom } );

                // blocked rooms 
                this.Add( new Room { roomNarrative = "Somehow you managed to enter an area in the game that you should not be in.  Congratulations! You broke it!  I hope you feel good about yourself!", roomType = Room.RoomType.Blocked } );
            }
        }
    }
}