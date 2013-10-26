using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DungeonBot
{
    /// <summary>
    /// Hero class that defines a player
    /// </summary>
    class Hero
    {
        public string _heroName = null;
        
        public int _health = 0;
        public int _baseHealth = 0;

        public int _exp = 0;
        public int _baseExp = 0;

        public PlayerClass _currentClass = PlayerClass.Warrior;

        public enum PlayerClass
        {
            Warrior,

            Priest,

            Mage,

            Rogue
        }

        enum PlayerActions
        {
            Attack,

            Heal,

            Inventory
        }
    }
}
