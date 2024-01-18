using Christmas_Game;
using System;
using System.Collections.Generic;
using System.IO;

class LevelLoader
{
    /// <summary>
    /// Loads a level from a file
    /// </summary>
    /// <param name="fileName">The filepath from which the level should be loaded from</param>
    /// <returns></returns>
    public static Tuple<Room,Room> LoadLevel(string fileName)
    {
        Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        // Read all lines from the file
        string[] lines = File.ReadAllLines(fileName);
        // Parse each line and create Room objects
        foreach (var line in lines) //Format is like this: RoomNumber,LeftRoomNumber,RightRoomNumber,TopRoomNumber,BottomRoomNumber(,ItemName,ItemEffect,ItemEffectValue)
        {
            string[] parts = line.Split(',');
                
                
            if (parts.Length >= 5)
            {
                string itemName = "";
                string itemEffect = "";
                int itemEffectValue = 0;
                int roomNumber = int.Parse(parts[0]);
                int leftRoomNumber = int.Parse(parts[1]);
                int rightRoomNumber = int.Parse(parts[2]);
                int topRoomNumber = int.Parse(parts[3]);
                
                int bottomRoomNumber = int.Parse(parts[4]);
                if (parts.Length > 5)
                {
                    itemName = parts[5];
                    itemEffect = parts[6];
                    itemEffectValue = int.Parse(parts[7]);
                }
                
                Room currentRoom;
                if (!rooms.TryGetValue(roomNumber, out currentRoom!))
                {
                    currentRoom = new Room();
                    currentRoom.RoomNumber = roomNumber;
                    rooms.Add(roomNumber, currentRoom); //Add the room to the dictionary for later use (easy access)
                    
                }

                // Connect with left room
                if (leftRoomNumber != 0)
                {
                    Room leftRoom;
                    if (!rooms.TryGetValue(leftRoomNumber, out leftRoom!))
                    {
                        leftRoom = new Room();
                        leftRoom.RoomNumber = leftRoomNumber;
                        rooms.Add(leftRoomNumber, leftRoom);
                    }

                    currentRoom.Left = leftRoom;
                    leftRoom.Right = currentRoom; //Idk im suppossed to comment this but there really isn't much to say :...(
                }

                // Connect with right room
                if (rightRoomNumber != 0)
                {
                    Room rightRoom;
                    if (!rooms.TryGetValue(rightRoomNumber, out rightRoom!))
                    {
                        rightRoom = new Room();
                        rightRoom.RoomNumber = rightRoomNumber;
                        rooms.Add(rightRoomNumber, rightRoom);
                    }

                    currentRoom.Right = rightRoom;
                    rightRoom.Left = currentRoom;
                }

                // Connect with top room
                if (topRoomNumber != 0)
                {
                    Room topRoom;
                    if (!rooms.TryGetValue(topRoomNumber, out topRoom!))
                    {
                        topRoom = new Room();
                        topRoom.RoomNumber = topRoomNumber;
                        rooms.Add(topRoomNumber, topRoom);
                    }

                    currentRoom.Top = topRoom;
                    topRoom.Bottom = currentRoom;
                }

                // Connect with bottom room
                if (bottomRoomNumber != 0)
                {
                    Room bottomRoom;
                    if (!rooms.TryGetValue(bottomRoomNumber, out bottomRoom!))
                    {
                        bottomRoom = new Room();
                        bottomRoom.RoomNumber = bottomRoomNumber;
                        rooms.Add(bottomRoomNumber, bottomRoom);
                    }

                    currentRoom.Bottom = bottomRoom;
                    bottomRoom.Top = currentRoom;
                }
                if (itemName.Length > 0)
                {
                    currentRoom.RoomItem = new Item(itemName, itemEffect, itemEffectValue);
                }
            }
            else
            {
                Console.WriteLine("Invalid line format: " + line);
            }
        }
        return new(rooms[1],rooms[69]); //Return the Starting Room and the Ending Room end room cuz key = 69 :D
    }
}