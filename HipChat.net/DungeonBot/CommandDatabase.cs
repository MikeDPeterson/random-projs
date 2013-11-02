using System;

namespace DungeonBot
{      
    /// <summary>
    /// Commands that can be used from the chat client to interact with DungeonBot.
    /// </summary>
    public class CommandDatabase 
    {
        /// <summary>
        /// Commands...
        /// </summary>
        public enum Command
        {
            // bot commands
            BotStats,

            Help,

            BotQuit,

            // movement commands
            North,

            South,

            East,

            West,

            // game commands
            Map,

            CurrentRoom,

            GenerateDungeon,

            // misc.
            Unknown
        }
    
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public Command GetCommand( HipChat.Entities.Message commandItem )
        {
            string command = commandItem.Text;

            Command result = Command.Unknown;

            if ( command.Equals( "BotStats", StringComparison.OrdinalIgnoreCase ) )
            {
                return Command.BotStats;
            }

            if ( command.Equals( "Help", StringComparison.OrdinalIgnoreCase ) )
            {
                return Command.Help;
            }

            if ( command.Equals( "BotQuit" ) )
            {
                return Command.BotQuit;
            }

            if ( command.Equals( "North" ) )
            {
                return Command.North;
            }

            if ( command.Equals( "South" ) )
            {
                return Command.South;
            }

            if ( command.Equals( "East" ) )
            {
                return Command.East;
            }

            if ( command.Equals( "West" ) )
            {
                return Command.West;
            }

            if ( command.Equals( "Map" ) )
            {
                return Command.Map;
            }

            if ( command.Equals( "CurrentRoom" ) )
            {
                return Command.CurrentRoom;
            }

            if ( command.Equals( "GenerateDungeon" ) )
            {
                return Command.GenerateDungeon;
            }

        return result;
        }
    }
}
   