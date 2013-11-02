using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HipChat;

namespace DungeonBot
{
    class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            const string apiToken = "7a695002c154abd55955d9ef8b9375";
            const int currentRoomId = 321236;
            const string botName = "DungeonBot";
            HipChatClient hipChatClient = null;
            DateTime startupDateTime;
            
            bool gameRunning = true;
            
            Dungeon dungeon = new Dungeon();

            CommandDatabase commands = new CommandDatabase();
            startupDateTime = DateTime.Now;

            hipChatClient = new HipChatClient( apiToken, currentRoomId, botName );

            // rate limit is 100 API requests per 5 minutes
            int minWaitTimeSeconds = 5 * 60 / 100;
            DateTime lastGotDateTime = DateTime.UtcNow.Subtract( new TimeSpan( 0, 0, 5 ) );

            while ( gameRunning == true )
            {
                dungeon.Visited( dungeon.currentRoom );
                
                try
                {
                    System.Threading.Thread.Sleep( ( minWaitTimeSeconds * 2 ) * 1000 );

                    List<HipChat.Entities.Message> recentMessageList = hipChatClient.ListHistoryAsNativeObjects();
                    recentMessageList = recentMessageList.OrderBy( a => a.Date ).ToList();
                    if ( recentMessageList.Count > 0 )
                    {
                        var lastMessage = recentMessageList.Last();
                        recentMessageList = recentMessageList.Where( a => a.Date > lastGotDateTime ).Where( a => a.From.Name != botName ).ToList();
                        lastGotDateTime = lastMessage.Date;
                    }

                    foreach ( var messageItem in recentMessageList )
                    {
                        // clean up the message a little
                        messageItem.Text = messageItem.Text.Replace( "\\n", string.Empty ).Trim();

                        Console.ResetColor();
                        Console.WriteLine( messageItem.From.Name + ": " + messageItem.Text );

                        CommandDatabase.Command commandType = commands.GetCommand( messageItem );
                        if ( commandType != CommandDatabase.Command.Unknown )
                        {
                            /* Respond to the RandomAnswerXX class of messagecatagory */
                            string enumName = Enum.GetName( typeof( CommandDatabase.Command ), commandType );

                            switch ( commandType )
                            {
                                /* Bot stuff */
                                case CommandDatabase.Command.BotStats:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( string.Format( "Running since {0}", startupDateTime.ToString( "F" ) ) );
                                        sb.AppendLine( string.Format( "Messages Sent {0}", hipChatClient.MessageSentCount ) );
                                        sb.AppendLine( string.Format( "Api Calls {0}", hipChatClient.ApiCallCount ) );
                                        hipChatClient.SendMessage( sb.ToString() );
                                    }
                                    break;

                                case CommandDatabase.Command.Help:
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.AppendLine( "<pre>" );
                                        sb.AppendLine( "Help (shows this)" );
                                        sb.AppendLine( "BotQuit (stops the bot program)" );
                                        sb.AppendLine( "BotStats (shows stats)" );
                                        sb.AppendLine( "CurrentRoom (shows current room of the dungeon)" );
                                        sb.AppendLine( "GenerateDungeon (generates a new dungeon - keep in mind you will start on floor 1 and lose any progress)" );
                                        sb.AppendLine();
                                        sb.AppendLine( "</pre>" );
                                        hipChatClient.SendMessageHtml( sb.ToString() );
                                        // sleep a little so they come out in the right order
                                        System.Threading.Thread.Sleep( 500 );
                                    }
                                    break;

                                case CommandDatabase.Command.BotQuit:
                                    hipChatClient.SendMessage( "OK, I'll quit now. Goodbye, " + messageItem.From.FirstName );
                                    gameRunning = false;
                                    break;
                                    
                                case CommandDatabase.Command.North:
                                    Dungeon.Location tmpLocation = dungeon.currentRoom;
                                    tmpLocation.y = tmpLocation.y - 1;
                                    if ( dungeon.GetRoomType( tmpLocation ) == Dungeon.Room.RoomType.Blocked )
                                    {
                                        hipChatClient.SendMessage( "That direction is blocked." );
                                    }
                                    else
                                    {
                                        dungeon.currentRoom.y -= 1;
                                    }
                                    break;

                                case CommandDatabase.Command.South:
                                    tmpLocation = dungeon.currentRoom;
                                    tmpLocation.y = tmpLocation.y + 1;
                                    if ( dungeon.GetRoomType( tmpLocation ) == Dungeon.Room.RoomType.Blocked )
                                    {
                                        hipChatClient.SendMessage( "That direction is blocked." );
                                    }
                                    else
                                    {
                                        dungeon.currentRoom.y += 1;
                                    }
                                    break;

                                case CommandDatabase.Command.East:
                                    tmpLocation = dungeon.currentRoom;
                                    tmpLocation.x = tmpLocation.x + 1;
                                    if ( dungeon.GetRoomType( tmpLocation ) == Dungeon.Room.RoomType.Blocked )
                                    {
                                        hipChatClient.SendMessage( "That direction is blocked." );
                                    }
                                    else
                                    {
                                        dungeon.currentRoom.x += 1;
                                    }
                                    break;

                                case CommandDatabase.Command.West:
                                    tmpLocation = dungeon.currentRoom;
                                    tmpLocation.x = tmpLocation.x - 1;
                                    if ( dungeon.GetRoomType( tmpLocation ) == Dungeon.Room.RoomType.Blocked )
                                    {
                                        hipChatClient.SendMessage( "That direction is blocked." );
                                    }
                                    else
                                    {
                                        dungeon.currentRoom.x -= 1;
                                    }
                                    break;
                                    
                                case CommandDatabase.Command.Map:
                                    StringBuilder map = new StringBuilder();
                                    map.AppendLine( "Floor: " + dungeon._dungeonFloor + "| LEGEND: S = Starting Area, R = Room, B = Boss Room, #/X = Blocked Room" );
                                    map.AppendLine( dungeon.DrawMap() );
                                    hipChatClient.SendMessage( map.ToString() );
                                    break;

                                case CommandDatabase.Command.GenerateDungeon:
                                    dungeon = new Dungeon();
                                    hipChatClient.SendMessage( "New dungeon created. Good luck!" );
                                    break;

                                case CommandDatabase.Command.CurrentRoom:
                                    hipChatClient.SendMessage( "Current room is : [" + dungeon.currentRoom.x + "] [" + dungeon.currentRoom.y + "]" );
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                catch ( Exception ex )
                {
                    hipChatClient.SendMessage( "Oh man, this happened: " + ex.Message );
                }
            }
        }
    }
}
