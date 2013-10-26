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
    [Serializable]
    [DataContract]
    public class Hero
    {
        /// <summary>
        /// gets or sets the hero's name.
        /// </summary>
        [DataMember]
        public string _heroName { get; set; }

        /// <summary>
        /// gets or sets the hero's health
        /// </summary>
        [DataMember]
        public int _health { get; set; }

        /// <summary>
        /// gets or sets the hero's base health
        /// </summary>
        [DataMember]
        public int _baseHealth { get; set; }

        /// <summary>
        /// gets or sets the hero's experience
        /// </summary>
        [DataMember]
        public int _exp { get; set; }

        /// <summary>
        /// gets or sets the hero's base experience
        /// </summary>
        [DataMember]
        public int _baseExp { get; set; }

        /// <summary>
        /// the hero's class
        /// </summary>
        public enum HeroClass
        {
            Warrior,

            Priest,

            Mage,

            Rogue
        }

        /// <summary>
        /// the hero's actions
        /// </summary>
        enum HeroAction
        {
            Attack,

            Heal,

            Inventory
        }
    }
}
