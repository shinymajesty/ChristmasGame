using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Christmas_Game
{
    public class Roomdrawer
    {   
        /// <summary>
        /// Draws the room
        /// </summary>
        /// <param name="room">The Room to be drawn</param>
        public static void DrawBox(Room room)
        {
            int width = 5; // Adjust the width of the box as needed
            string topline = room.Top == null ? "╔═══════════╗" : "╔════   ════╗";
            string bottomline = room.Bottom == null ? "╚═══════════╝" : "╚════   ════╝";
            bool hasLeft = room.Left != null;
            bool hasRight = room.Right != null;

            // Draw the top of the box
            Console.SetCursorPosition(20, 25);
            Console.Write(topline);
            Console.WriteLine();

            // Draw the sides of the box
            for (int i = 0; i < width; i++)
            {
                Console.SetCursorPosition(20, Console.CursorTop);
                if (hasLeft && ((i >= (width / 2)) && (i <= (width / 2))))
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("║");
                }
                Console.Write(new string(' ', width));
                if (room.RoomItem != null && i == width / 2) Console.Write("?");
                else Console.Write(" ");
                Console.Write(new string(' ', width));
                if (hasRight && ((i >= (width / 2)) && (i <= (width / 2))))
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("║");
                }
                Console.WriteLine();
            }

            // Draw the bottom of the box
            Console.SetCursorPosition(20, Console.CursorTop);
            Console.Write(bottomline);
            Console.WriteLine();
        }


    }
}

