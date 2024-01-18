//oh boi here we go again
//i really cba to comment this *sighs*


using System;
using System.DirectoryServices.ActiveDirectory;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection.Emit;


namespace Christmas_Game
{
    internal class Program
    {
        static Room finalRoom = new Room(); //Initialize final room to avoid null reference exception
        static int difficulty = 10; //Base difficulty since i cba to write so that difficulty is only set when dungeon mode is selected
        static Player Player1 = new Player(); //This is our Player be nice to him! We initialize him here so we can use him in the entire program, well class
        ///<summary>
        ///Main method that serves as the entry point for the Christmas Game program.
        ///</summary>
        static void Main(string[] args)
        {
            PrepareGame(false); //Darkmode the only way anyways false= black; true = white not that hard ok? ok.
            (Player, sbyte) myTempoTuple = StartUpScreen(); //Muteable tuple to get the player and the speed selected by the user
            Player1 = myTempoTuple.Item1; //Set the player to the player selected by the user
            Player1.Speed = myTempoTuple.Item2 + 1; //Set the speed to the speed selected by the user
            Player1.Energy = 10; //Energy is set to 10 since I think the game is most enjoyable when it's not too easy
            RefreshScreen(); //Refresh the screen to get rid of the startup screen
            GetOutPlayerName(); 
            (bool, int) myLevelAndMode = LevelSelector();
            if (myLevelAndMode.Item1)
            {
                difficulty = GetDiffultyMode(); //If dungeon mode was selected, retrive the difficulty in yet another very hot looking menu (if you get past the console application vibes that is)
            }
            if (myLevelAndMode.Item1)
            { 
                ProcedGenerator procedGenerator = new(7, 7); //Initialize the proced generator with a size of 7x7 (can be changed but remember that 8x8 is already gonna 30% bigger (exponential growth)
                Tuple<Room, Room> myLevel = procedGenerator.GenerateMap();
                Player1.CurrentRoom = myLevel.Item1; //Set the current room to the start room
                finalRoom = myLevel.Item2; //Set the final room to the final room
            }
            else
            {
                Tuple<Room, Room> myLevel = LoadLevel(myLevelAndMode.Item2 + 1);
                Player1.CurrentRoom = myLevel.Item1;
                finalRoom = myLevel.Item2;
            } 

            
            Roomdrawer myRoomDrawer = new Roomdrawer();
            
            GetOutRoomInformation();
            RefreshScreen();
            GetOutPlayerName();
            GetOutRoomInformation();




            MainGameLoop(Player1.CurrentRoom, finalRoom);
        }
        /// <summary>
        /// Main game loop; waits for player input and updates the game state accordingly.
        /// </summary>
        /// <param name="startRoom">Starting room for the player.</param>
        /// <param name="finalRoom">Final room for victory condition.</param>
        static void MainGameLoop(Room startRoom, Room finalRoom) // Main game loop; Here with every tick we wait for the Players input (ReadKey)
        {
            GetOutPlayerName(); //Get out the player name and energy and speed
            GetOutRoomInformation(); //Get out the room information like contained items and room number

            Roomdrawer.DrawBox(Player1.CurrentRoom); //Drawbox is just to visualize the room 
            (string, (int, int)) controls = ControlsUtils(startRoom); //Control utils helps the player to know about all the actions that they can do in the current room
            Player1.CurrentRoom = startRoom; //Set the current room to the start room (this is just to avoid null reference exceptions)
            //nice ^^
            while (true)
            {
                Console.SetCursorPosition(Console.BufferWidth - 1,Console.BufferHeight - 1); //Set the cursor position to the bottom right corner of the console window (just for looks)
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key) //switch for blazing speed! (not really but it's still faster than if else)
                {
                    case ConsoleKey.LeftArrow:
                        Player1.Move(Player.Directions.Left); //Checking if a move is even viable is done within the Move method
                        break;

                    case ConsoleKey.RightArrow:
                        Player1.Move(Player.Directions.Right);
                        break;

                    case ConsoleKey.UpArrow:
                        Player1.Move(Player.Directions.Up);
                        break;

                    case ConsoleKey.DownArrow:
                        Player1.Move(Player.Directions.Down);
                        break;

                    case ConsoleKey.Spacebar:
                        if (Player1.CurrentRoom.RoomItem != null)
                        {
                            Player1.SelectedItem = null; //The item the player carried before is lost
                            Player1.SelectedItem = Player1.CurrentRoom.RoomItem; //The item the player picked up is now the item the player carries
                            if(Player1.SelectedItem.Energy > 0) // If the item gives energy it is consumed right away
                            {                                
                                Player1.Energy += Player1.SelectedItem.Energy;
                            }
                            Player1.CurrentRoom.RoomItem = null; //The item in the room is now null since the player picked it up
                        }
                        break;

                    default:
                        break;


                }
                if(Player1.CurrentRoom == finalRoom)
                {
                    Console.Clear();
                    Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                    Victoreee();
                    for(int i = 0; i < Console.WindowHeight; i++)
                    {
                        for(int j = 0; j < Console.WindowWidth/10; j++)
                        {
                               Console.Write("YOU WON!!!");
                        }
                        Console.Write("\n");
                    }
                    break;
                }
                if(Player1.Energy <= 0) // If the player runs out of energy the game is over
                {
                    Console.Clear();
                    Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                    OutPutCenteredText("You lost :(");
                    break;
                }
                GetOutPlayerName();
                GetOutRoomInformation();

                Roomdrawer.DrawBox(Player1.CurrentRoom); //Draw the new room

                controls = ControlsUtils(Player1.CurrentRoom);
                ClearCurrentConsoleLine(Console.WindowHeight - 5, 4); //Clearing the last 5 console lines
                StartUpScreenHelper(controls.Item1, controls.Item2);
            }
        }
        /// <summary>
        /// Loads a specific level from the file.
        /// </summary>
        /// <param name="level">Level number to load.</param>
        /// <returns>Tuple of two Room objects representing the starting and final rooms.</returns>
        static Tuple<Room,Room> LoadLevel(int level)
        {
            return LevelLoader.LoadLevel(@"..\..\..\resx\levels\level" + level + ".in");
        }
        /// <summary>
        /// Displays controls and information about available actions in the current room.
        /// </summary>
        /// <param name="currentRoom">Current room the player is in.</param>
        /// <returns>Tuple containing a string of controls/actions and a tuple of (x, y) coordinates to display the information.</returns>
        static (string, (int, int)) ControlsUtils(Room currentRoom)
        {
            StringBuilder sb = new StringBuilder();
            string txtAct = "Actions";
            sb.Append('-', (Console.WindowWidth / 2) - (txtAct.Length / 2));
            sb.Append(txtAct);
            sb.Append('-', (Console.WindowWidth / 2) - (txtAct.Length / 2));

            string itemAppender = currentRoom.RoomItem != null ? "Pick up Item: Spacebar" : "";

            //Movement controls on left while item controls on right
            if (currentRoom.Left != null)
            {
                sb.AppendLine("Move Left: Left Arrow Key    " + itemAppender);
                itemAppender = ""; //Set it to empty string so that it doesn't get appended to the next line
            }
            if (currentRoom.Right != null)
            {
                sb.AppendLine("Move Right: Right Arrow Key  " + itemAppender);
                itemAppender = "";
            }
            if (currentRoom.Bottom != null)
            {
                sb.AppendLine("Move Down: Down Arrow Key    " + itemAppender);
                itemAppender = "";
            }
            if (currentRoom.Top != null)
            {
                sb.AppendLine("Move Up: Up Arrow Key    " + itemAppender);
                itemAppender = "";
            }

            string returner = sb.ToString();
            return (returner, (0, Console.WindowHeight - 6));
        }
        /// <summary>
        /// Does the selection screen for the main game
        /// </summary>
        /// <returns>Returns (bool, sbyte) (if player is dungeon mode/normal mode, if level which level mode base 0)</returns>
        static (bool, int) LevelSelector()
        {
            string[] speeds = ["level 1 - 1", "level 1 - 2", "level 1 - 3", "level 1 - 4", "level 1 - 5"];
            string normalText = "<<Mode : Normal>>";
            string dungeonText = "<<Mode : Dungeon>>";
            bool curPos = false; //true: mode field, false: name field
            bool isDung = false; //true: dungeon mode, false: level mode (predefined)


            (int, int) namePos = (5, Console.CursorTop + 1);
            StartUpScreenHelper(normalText, namePos);
            string? playerName = "";



            (int, int) speedPos = (5, Console.CursorTop + 2);
            StartUpScreenHelper("<<level 1 - 1>>", speedPos);

            Console.SetCursorPosition(namePos.Item1 + normalText.Length, namePos.Item2);
            sbyte level = 0; //0... 1-1,1... 1-1, 2...1-3, 3...1-4, 4... 1-5
            //honestly i dont even remember how this code works but it does and im not changing it
            while (true) //Awaiting user input
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); //true means that we have to process the input before it is displayed

                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (curPos)
                    {
                        if (level > 0)
                        {
                            level--;
                            StartUpScreenHelper("<<" + speeds[level] + ">>", speedPos);
                        };
                    }
                    else
                    {
                        if (isDung)
                        {
                            isDung = false;
                            StartUpScreenHelper(normalText, namePos);
                            StartUpScreenHelper("<<" + speeds[level] + ">>", speedPos);
                            Console.SetCursorPosition(namePos.Item1 + normalText.Length, namePos.Item2);
                        }
                    }

                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (curPos)
                    {
                        if (level < 4)
                        {
                            level++;
                            StartUpScreenHelper("<<" + speeds[level] + ">>", speedPos);
                        }
                    }
                    else
                    {
                        if (!isDung)
                        {
                            StartUpScreenHelper(dungeonText, namePos);
                            StartUpScreenHelper("", speedPos);
                            Console.SetCursorPosition(namePos.Item1 + dungeonText.Length, namePos.Item2);
                            isDung = true;
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Tab || keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (curPos)
                    {
                        Console.SetCursorPosition(namePos.Item1 + normalText.Length, namePos.Item2);
                        curPos = false;
                    }
                    else if (!isDung)
                    {
                        if (playerName == null) playerName = "";
                        Console.SetCursorPosition(speedPos.Item1 + speeds[level].Length + 4, speedPos.Item2);
                        curPos = true;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (curPos) break;
                    else
                    {
                        if (playerName == null) playerName = "";
                        Console.SetCursorPosition(speedPos.Item1 + speeds[level].Length + 4, speedPos.Item2);
                        curPos = true;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (!curPos && playerName!.Length > 0)
                    {
                        playerName = playerName.Remove(playerName.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                    else Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                }
                else
                {
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                }
            }
            return (isDung, level);
        }
        /// <summary>
        /// Refreshes the console screen.
        /// </summary>
        static void RefreshScreen()
        {
            Console.Clear();
            PrintHeader();
        }
        /// <summary>
        /// Prepares the game by setting up the console window and printing the header.
        /// </summary>
        /// <param name="colorScheme">Indicates the color scheme (true for white, false for black).</param>
        static void PrepareGame(bool colorScheme)
        {
            BootUpSequence(colorScheme); //White mode/dark mode
            SetUpConsoleWindow();
            PrintHeader();
            SetForegroundWindow(GetConsoleWindow());
        }
        /// <summary>
        /// Displays the startup screen, allowing the player to enter their name and choose the game speed.
        /// </summary>
        /// <returns>Tuple containing a Player object and a sbyte representing the chosen speed level.</returns>
        static (Player, sbyte) StartUpScreen() //return player and speedlevel
        { //Honestly this is just a mess but it works so I'm not gonna change it
            string[] speeds = ["very slow [1]", "slow [2]", "normal [3]", "fast [4]", "very fast [5]"];
            string playerText = "Enter Player Name: ";
            bool curPos = false; //true: speed field, false: name field



            (int, int) namePos = (5, Console.CursorTop + 1);
            StartUpScreenHelper(playerText, namePos);
            string? playerName = "";



            (int, int) speedPos = (5, Console.CursorTop + 2);
            StartUpScreenHelper("<<Speed : normal [3]>>", speedPos);

            Console.SetCursorPosition(namePos.Item1 + playerText.Length, namePos.Item2);
            sbyte speed = 2; //0... very slow, 1...slow, 2...normal, 3...fast, 4...very fast

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (curPos && speed > 0)
                    {
                        speed--;
                        StartUpScreenHelper("<<Speed : " + speeds[speed] + ">>", speedPos);
                    }
                    else Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);

                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (speed < 4 && curPos)
                    {
                        speed++;
                        StartUpScreenHelper("<<Speed : " + speeds[speed] + ">>", speedPos);
                    }
                    else
                    {
                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Tab || keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (curPos)
                    {
                        Console.SetCursorPosition(namePos.Item1 + playerName!.Length + playerText.Length, namePos.Item2);
                        curPos = false;
                    }
                    else
                    {
                        if (playerName == null) playerName = "";
                        Console.SetCursorPosition(speedPos.Item1 + speeds[speed].Length + 12, speedPos.Item2);
                        curPos = true;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (curPos) break;
                    else
                    {
                        if (playerName == null) playerName = "";
                        Console.SetCursorPosition(speedPos.Item1 + speeds[speed].Length + 12, speedPos.Item2);
                        curPos = true;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (!curPos && playerName!.Length > 0)
                    {
                        playerName = playerName.Remove(playerName.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                    else Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                }
                else
                {
                    if (!curPos)
                    {
                        playerName += keyInfo.KeyChar;
                        Console.Write(keyInfo.KeyChar);
                    }
                    else Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                }
            }
            return (playerName == "" ? new Player() : new Player(playerName), speed);
        }
        /// <summary>
        /// Helper method to display text on the startup screen at specified coordinates.
        /// </summary>
        /// <param name="textToOutput">Text to be displayed on the screen.</param>
        /// <param name="coordinates">Tuple of (x, y) coordinates where the text should be displayed.</param>
        static void StartUpScreenHelper(string textToOutput, (int, int) a)
        {
            Console.SetCursorPosition(a.Item1, a.Item2); // this will do for now fix later! Nvm it will stay like this
            ClearCurrentConsoleLine();
            Console.SetCursorPosition(a.Item1, a.Item2);
            Console.Write(textToOutput);
        }
        /// <summary>
        /// Clears the current console line.
        /// </summary>
        static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        /// <summary>
        /// Clears a specified number of console lines starting from the current line.
        /// </summary>
        /// <param name="line">Starting line to clear.</param>
        /// <param name="length">Number of lines to clear.</param>
        static void ClearCurrentConsoleLine(int line)//Clear console where cursor is
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth)); //String filled with ' ' characters for the length of the console window
            Console.SetCursorPosition(0, currentLineCursor);
        }
        /// <summary>
        /// Clears a specified number of console lines starting from the specified line.
        /// </summary>
        /// <param name="line">Starting line to clear.</param>
        /// <param name="length">Number of lines to clear.</param>
        static void ClearCurrentConsoleLine(int line, int length)//Clear console where cursor is and length lines below
        {
            int currentLineCursor = Console.CursorTop;
            for (int i = line; i <= line + length; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, currentLineCursor);
        }
        /// <summary>
        /// Displays player information, including name, energy, speed, and selected item.
        /// </summary>
        static void GetOutPlayerName() //Get out player information
        {
            string[] speeds = ["very slow ", "slow ", "normal ", "fast ", "very fast "];
            Console.SetCursorPosition(5, 13);
            ClearCurrentConsoleLine(); //Since the ClearCurrentConsoleLine doesnt set the cursor to starting position it has to be done explicitely
            Console.SetCursorPosition(5, Console.CursorTop);
            Console.WriteLine("Player: " + Player1.Name);
            ClearCurrentConsoleLine();
            Console.SetCursorPosition(5, Console.CursorTop);
            Console.SetCursorPosition(5, Console.CursorTop + 1);
            ClearCurrentConsoleLine();
            Console.SetCursorPosition(5, Console.CursorTop);
            Console.WriteLine("Energy: " + Math.Round(Player1.Energy, 2));
            ClearCurrentConsoleLine();
            Console.SetCursorPosition(5, Console.CursorTop);
            Console.WriteLine("Speed: " + (speeds[Player1.Speed]));
            Console.SetCursorPosition(5, Console.CursorTop);
            if (Player1.SelectedItem != null) //If the player has an item display it
            {
                Console.SetCursorPosition(5, Console.CursorTop);
                if (Player1.SelectedItem!.name != null)
                    Console.WriteLine("Item: " + Player1.SelectedItem!.name);
                Console.SetCursorPosition(5, Console.CursorTop);
                if (Player1.SelectedItem!.Energy != 0)
                    Console.WriteLine("Item energy: " + Player1.SelectedItem!.Energy);
                else if (Player1.SelectedItem!.Speed != 0)
                    Console.WriteLine("Item speed: " + Player1.SelectedItem!.Speed);
            }

        }
        /// <summary>
        /// Retrieves the selected difficulty mode from the player in a menu.
        /// </summary>
        /// <returns>Selected difficulty mode (0 for Normal, 1 for Hard).</returns>
        static int GetDiffultyMode() //Another menu to get the difficulty
        {
            int difficulty = 0;
            string[] difficulties = ["Normal", "Hard"];
            StartUpScreenHelper("<<Difficulty : Normal>>", (5, Console.CursorTop + 1));
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter) break;
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (difficulty > 0)
                    {
                        difficulty--;
                        StartUpScreenHelper("<<Difficulty : " + difficulties[difficulty] + ">>", (5, Console.CursorTop));
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (difficulty < 1)
                    {
                        difficulty++;
                        StartUpScreenHelper("<<Difficulty : " + difficulties[difficulty] + ">>", (5, Console.CursorTop));
                    }
                }
            }
            Console.Clear();
            return difficulty;
        }
        /// <summary>
        /// Displays information about the current room, including room number and contained items.
        /// </summary>
        static void GetOutRoomInformation() //Print room information
        {
            Console.SetCursorPosition(5, 20);
            ClearCurrentConsoleLine();
            Console.SetCursorPosition(5, 20);
            Console.WriteLine("Roomnumber: " + Player1.CurrentRoom.RoomNumber + (difficulty == 0 ? " Final Room Roomnumber: " + finalRoom.RoomNumber : ""));
           
            //Console.WriteLine("Current Room: " + player.CurrentRoom.RoomNumber); //IMPLEMENT LATER
            if (Player1.CurrentRoom.RoomItem != null)
            {
                Console.SetCursorPosition(5, Console.CursorTop);
                Console.WriteLine("Room Item: " + (Player1.CurrentRoom.RoomItem == null ? "-" : Player1.CurrentRoom.RoomItem.name));
                Console.SetCursorPosition(5, Console.CursorTop);
                Console.WriteLine("Room Item Effect: " + (Player1.CurrentRoom.RoomItem == null ? "-" : Player1.CurrentRoom.RoomItem.Speed > 0 ? "+" +Player1.CurrentRoom.RoomItem.Speed + " Speed" : "+" + Player1.CurrentRoom.RoomItem.Energy + " Energy"));
            }
            else
            {
                ClearCurrentConsoleLine(Console.CursorTop, 2);
            }

        }
        /// <summary>
        /// Sets up the console window size, position, and buffer.
        /// </summary>
        static void SetUpConsoleWindow()
        {

            Console.SetWindowSize(60, 40);
            if (OperatingSystem.IsWindows())
            {
                Console.SetBufferSize(60, 40);
                IntPtr ptr = GetConsoleWindow(); //IntPtr is a pointer to an object in this case a pointer to the console window
                MoveWindow(ptr, 700, 150, 520, 780, true); //Move the console window to the middle of the screen most of the values are random-ish though
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND); //Disable minimize button
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND); //Disable maximize button
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND); //Disable window resizing
            }
            else
            {
                for (int i = 0; i < 15; i++)
                {
                    Console.WriteLine("RUN THIS CODE ON WINDOWS ONLY FOR THE BEST USER EXPERIENCE!!!");
                }
            }
            Console.Clear();
        }
        /// <summary>
        /// Boots the Game up with a nice little loading screen
        /// </summary>
        /// <param name="colorScheme">Indicates the color scheme (true for white, false for black).</param>
        static void BootUpSequence(bool colorScheme)
        {
            SetUpConsole(colorScheme);
            Console.Clear();
            OutPutCenteredText("Welcome to the Christmas Game!"); //Welcome message kinda like a loading screen
            Task.Delay(2000).Wait();
            Console.Clear();
            OutPutCenteredText("Press any key to continue...");
            Console.ReadKey();
        }
        /// <summary>
        /// Prints the header for the Christmas Game.
        /// </summary>
        static void PrintHeader()
        {
            string filepath = @"..\..\..\resx\loadingscreenres\quistgameasset1.ast";
            StreamReader sr = new(filepath); //Maybe not optimal to read the file every time but ccomputer fast so it doesnt matter
            foreach (string line in ReadAllLines(sr))
            {
                Console.WriteLine(line);
            }
        }
        /// <summary>
        /// Treads all lines of a file
        /// </summary>
        /// <param name="reader">The streamreader that is reading it</param>
        /// <returns></returns>
        static string[] ReadAllLines(TextReader reader)
        {
            string? line;

            List<string> lines = new List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines.ToArray();
        }
        /// <summary>
        /// Centers text output on the console window.
        /// </summary>
        /// <param name="textToCenter">Text to be centered on the screen.</param>
        static void OutPutCenteredText(string textToCenter)
        {
            // Calculate the center position
            int centerX = Console.WindowWidth / 2 - textToCenter.Length / 2;
            int centerY = Console.WindowHeight / 2;

            // Set the cursor position to the center
            Console.SetCursorPosition(centerX, centerY);

            // Output the centered text
            Console.WriteLine(textToCenter);
        }
        /// <summary>
        /// Sets up the console window color scheme.
        /// </summary>
        /// <param name="colorScheme">Indicates the color scheme (true for white, false for black).</param>
        static void SetUpConsole(bool colorScheme) //0, false = black // 1,true = white
        {
            ConsoleColor foreColor = !colorScheme ? ConsoleColor.White : ConsoleColor.Black;
            ConsoleColor backColor = colorScheme ? ConsoleColor.White : ConsoleColor.Black;


            Console.Title = "Christmas Game";
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
        }
        static void Victoreee()
        {
            for (int i = 0; i < 10; i++)
            {
                LaunchWinningApp(@"..\..\..\resx\loadingscreenres\WinningApp.exe"); //Launch an army of windows in hopes of having it look cool but i dont think it does
                Thread.Sleep(400); //Also doing something like this heavily impedes UX but who cares its a school project
            }

        }
        /// <summary>
        /// Launches the winning application multiple times with a delay in between.
        /// </summary>
        /// <param name="filename">Path to the winning application executable.</param>
        static void LaunchWinningApp(string filename) //Yeah so this is where it gets uh-- thanks stack overflow
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo //ProcessStartInfo is a class that contains information about a process that is about to be started
                {
                    FileName = filename,
                    Arguments = $"/k {filename}",
                    UseShellExecute = true, //this is so it opens the windows as new applications and not as a new console window within this one
                    CreateNoWindow = false // there are more than these but those have standart values that are fine in this case
                };

                Process process = new Process { StartInfo = psi }; //actually starting the process giving it the startinfo we jsut initialized
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening new console window: {ex.Message}");
            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        //// !! THE FOLLOWING CODE WILL NOT WORK ON ANYTHING OTHER THAN WINDOWS !! ////
        //// IT IS DIRECTLY DERIFED FROM THE VERY DEPENDABLE MICROSOFT LEARN SITE  ////
        ////https://learn.microsoft.com/en-us/ LOOK UNDER user32.dll for mroe info ////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        const int MF_BYCOMMAND = 0x00000000, 
                  SC_MINIMIZE = 0xF020, 
                  SC_MAXIMIZE = 0xF030,
                  SC_SIZE = 0xF000;
    

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr point);
    }
}
