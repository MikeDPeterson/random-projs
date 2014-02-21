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
        Random rand = new Random();

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
                
                int randomIndex = rand.Next( maxIndex );
                string message = this.ElementAt( randomIndex );
                return message;
            }
            else
            {
                return "I need to think of some stuff to say";
            }
        }
    }
}
