using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Christmas_Game
{
    public class Room
    {
        public Room? Left { get; set; }
        public Room? Right { get; set; }
        public Room? Top { get; set; }
        public Room? Bottom { get; set; }
        public int RoomNumber { get; set; }
        public Item? RoomItem { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

    }
}
