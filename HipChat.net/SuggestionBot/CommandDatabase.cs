using System;

namespace SuggestionBot
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

            return result;
        }
    }
}
