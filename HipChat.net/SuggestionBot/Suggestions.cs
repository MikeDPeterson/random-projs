using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionBot
{
    /// <summary>
    /// 
    /// </summary>
    public class Nouns : List<string>
    {
        private Random rand = new Random( (int)( DateTime.Now.Ticks / 1000 ) );

        private Dictionary<string, DateTime> lastUsed = new Dictionary<string, DateTime>();

        public Nouns()
        {
            Add( "Cat" );
            Add( "Salad" );
            Add( "Android" );
            Add( "Muffin" );
            Add( "Sammich" );
            Add( "Surgeon" );
            Add( "Toenail" );
            Add( "Office" );
            Add( "Gamer" );
            Add( "Coffee" );
            Add( "Paper" );
            Add( "Ditch" );
            Add( "Pastor" );
            Add( "Car" );
            Add( "Hammer" );
            Add( "Designer" );
            Add( "Infrastructure" );
            Add( "Monkey" );
            Add( "Spiderman" );
            Add( "Pants" );
            Add( "Rifle" );
            Add( "Dreadlock" );
            Add( "Volunteer" );
            Add( "Scooter" );
            Add( "Rake" );
            Add( "Butterknife" );
            Add( "Tricycle" );
            Add( "Cancer" );
            Add( "Rap" );
            Add( "Christmas" );
            Add( "Hippie" );
            Add( "Casket" );
            Add( "Housewife" );
            Add( "Horsebot" );
            Add( "Fire" );
            Add( "Dog" );
            Add( "Mustard" );
            Add( "Superhero" );
            Add( "Hipster" );
            Add( "Worship" );
            Add( "Priest" );
            Add( "Communion" );
            Add( "Cracker" );
            Add( "Hamster" );
            Add( "Godzilla" );
            Add( "Albino" );
            Add( "Boat" );
            Add( "Infection" );
            Add( "Farm" );
            Add( "Landscaper" );
            Add( "Gamer" );
            Add( "Bunny" );
            Add( "Weezel" );
            Add( "Rat" );
            Add( "Airplane" );
            Add( "Simulac" );
            Add( "Baby" );
            Add( "Dubstep" );
            Add( "Prisoner" );
            Add( "Comedian" );
            Add( "Weatherman" );
            Add( "Grocer" );
            Add( "Honeybadger" );
            Add( "Accordian" );
            Add( "Banjo" );
            Add( "Milk" );
            Add( "Shriner" );
            Add( "Juice" );
            Add( "Icecube" );
            Add( "Canada" );
            Add( "Hotdog" );
            Add( "Kitten" );
            Add( "Gravity" );
            Add( "Nerd" );
            Add( "Grandma" );
            Add( "Corpse" );
            Add( "Cloud" );
            Add( "Dragon" );
            Add( "Pacman" );
            Add( "Wizard" );
            Add( "Nurse" );
            Add( "Bucket" );
            Add( "Dirt" );
            Add( "Vomit" );
            Add( "Oxygen" );
            Add( "Bacteria" );
            Add( "Chicken" );
            Add( "Cafeteria" );
            Add( "Coffee" );
            Add( "Hepatitis" );
            Add( "Explosion" );
            Add( "Apathy" );
            Add( "Lactose" );
            Add( "Hydrogen Peroxide" );
            Add( "Choir" );
            Add( "Boy Band" );
            Add( "Diet Coke" );
            Add( "Donkey Donkey Donkey" );
            Add( "Angelic" );
            Add( "Sticky" );
        }

        /// <summary>
        /// Gets the random noun.
        /// </summary>
        /// <returns></returns>
        public string GetRandomNoun()
        {
            int maxIndex = this.Count();
            if ( maxIndex > 0 )
            {
                DateTime lastUsedDateTime = DateTime.Now;
                int retries = 0;
                string message = "nothing";
                while ( DateTime.Now.Subtract( lastUsedDateTime ).Hours < 8 && retries < maxIndex )
                {
                    int randomIndex = rand.Next( maxIndex );
                    message = this.ElementAt( randomIndex );

                    if ( lastUsed.ContainsKey( message ) )
                    {
                        lastUsedDateTime = lastUsed[message];
                    }
                    else
                    {
                        lastUsed.Add( message, DateTime.Now );
                        lastUsedDateTime = DateTime.Now;
                        break;
                    }

                    retries++;
                }

                return message;
            }
            else
            {
                return "I need to think of some stuff to say";
            }
        }
    }
}
