using Christmas_Game;
using System;
using System.Collections.Generic;
/// <summary>
/// Class for generating a procedurally generated map for the Christmas Game.
/// </summary>
public class ProcedGenerator
{
    private Room? startRoom; // The room where the player starts
    private Room? finalRoom; // The room where the player ends
    private int xSize, ySize; // The size of the map
    private List<Room> potFinalRooms = new List<Room>(); // The rooms that can be the final room
    private bool[,] visited; // The rooms that have been visited
    private int roomNumber; // The number of the room
    private Random Random = new Random();
    private Dictionary<int, Item> items;
    /// <summary>
    /// Initializes a new instance of the ProcedGenerator class with the specified map dimensions.
    /// </summary>
    /// <param name="xSize">The width of the map.</param>
    /// <param name="ySize">The height of the map.</param>
    public ProcedGenerator(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        StreamReader reader = new StreamReader(@"..\..\..\resx\loadingscreenres\items.in");    //List of all possible items
        string itemsList = reader.ReadToEnd();
        items = ParseItems(itemsList);
    }
    /// <summary>
    /// Generates a procedural map with a starting room and a final room.
    /// </summary>
    /// <returns>Tuple containing the starting room and final room.</returns>
    public Tuple<Room, Room> GenerateMap()
    {
        visited = new bool[xSize, ySize];
        int x = xSize / 2, y = ySize / 2;
        visited[x, y] = true;
        startRoom = new Room();
        roomNumber = 0;
        startRoom.RoomNumber = roomNumber++;
        DFS(x, y, visited, 5, startRoom);
        finalRoom = potFinalRooms[Random.Next(0, potFinalRooms.Count - 1)]; //select a random room from the list of potential final rooms
        return new Tuple<Room, Room>(startRoom, finalRoom);
    }
    /// <summary>
    /// Performs a Depth First Search algorithm to generate the map.
    /// </summary>
    /// <param name="x">Current x-coordinate.</param>
    /// <param name="y">Current y-coordinate.</param>
    /// <param name="visited">2D array indicating visited rooms.</param>
    /// <param name="origin">Direction from which the algorithm came.</param>
    /// <param name="room">Current room being processed.</param>
    private void DFS(int x, int y, bool[,] visited, int origin, Room room) //Simple implementation of Depth First Search Algorithm to generate the map
    {
        if (x <= 0 || y <= 0 || x >= visited.GetLength(0) || y >= visited.GetLength(1) || (visited[x - 1, y] && visited[x, y - 1] && visited[visited.GetLength(0) - 1, y] || visited[x, visited.GetLength(1) - 1]))
        {
            // Create a new instance of Room for potential final room
            potFinalRooms.Add(room); //All the corner rooms are potential final rooms
            return;
        }
        visited[x, y] = true;

        List<int> directions = GenerateRandomSequence(4);
        directions.Remove(origin);
        foreach (int i in directions)
        {
            switch (i)
            {
                case 1:
                    if (x > 0 && !visited[x - 1, y])
                    {
                        room.Left = new Room { RoomNumber = roomNumber++ };
                        room.Left.Right = room;
                        room = room.Left;
                        if (Random.Next(0, 100) < 10) //Spawning a rendom item with a 10% chance
                        {
                            room.RoomItem = items[Random.Next(1, items.Count)];
                        }
                        visited[x - 1, y] = true;
                        x--;
                        DFS(x, y, visited, 2, room);
                    }
                    break;
                case 2:
                    if (x < xSize - 1 && !visited[x + 1, y])
                    {
                        room.Right = new Room { RoomNumber = roomNumber++ };
                        room.Right.Left = room;
                        room = room.Right;
                        if (Random.Next(0, 100) < 10)
                        {
                            room.RoomItem = items[Random.Next(1, items.Count)];
                        }
                        visited[x + 1, y] = true;
                        x++;
                        DFS(x, y, visited, 1, room);
                    }
                    break;
                case 3:
                    if (y > 0 && !visited[x, y - 1])
                    {
                        room.Top = new Room { RoomNumber = roomNumber++ };
                        room.Top.Bottom = room;
                        room = room.Top;
                        if (Random.Next(0, 100) < 10)
                        {
                            room.RoomItem = items[Random.Next(1, items.Count)];
                        }
                        visited[x, y - 1] = true;
                        y--;
                        DFS(x, y, visited, 4, room);
                    }
                    break;
                case 4:
                    if (y < ySize - 1 && !visited[x, y + 1])
                    {
                        room.Bottom = new Room { RoomNumber = roomNumber++ };
                        room.Bottom.Top = room;
                        room = room.Bottom;
                        if (Random.Next(0, 100) < 10)
                        {
                            room.RoomItem = items[Random.Next(1, items.Count)];
                        }
                        visited[x, y + 1] = true;
                        y++;
                        DFS(x, y, visited, 3, room);
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// Generates a random sequence of numbers from 1 to maxNumber.
    /// </summary>
    /// <param name="maxNumber">The maximum number in the sequence.</param>
    /// <returns>The sequence of numbers.</returns>
    static List<int> GenerateRandomSequence(int maxNumber) //randomly generates a sequence of numbers from 1 to maxNumber
    {
        List<int> sequence = new List<int>();

        // Populate the sequence with numbers from 1 to maxNumber
        for (int i = 1; i <= maxNumber; i++)
        {
            sequence.Add(i);
        }

        // Shuffle the sequence using Fisher-Yates algorithm
        Random random = new Random();
        int n = sequence.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            int value = sequence[k];
            sequence[k] = sequence[n];
            sequence[n] = value;
        }

        return sequence;
    }
    /// <summary>
    /// Parses the input string containing item information.
    /// </summary>
    /// <param name="input">String containing item information.</param>
    /// <returns>Dictionary of items with their corresponding numbers.</returns>
    public static Dictionary<int, Item> ParseItems(string input)
    {
        Dictionary<int, Item> items = new Dictionary<int, Item>();
        input = input.Replace("\r", "");
        string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');

            if (parts.Length == 4 && int.TryParse(parts[0], out int number) && parts[1].Length >= 1 && (parts[2] == "S" || parts[2] == "E") && int.TryParse(parts[3], out int value))
            {
                string name = parts[1]; // Combine name and attribute
                Item item = new Item(name, parts[2][0].ToString(), value);
                items.Add(number, item);
            }
            else
            {
                Console.WriteLine("Invalid line format: " + line);
            }
        }

        return items;
    }
}
