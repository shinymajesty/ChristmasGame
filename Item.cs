using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Christmas_Game
{
    public class Item
    {
        public string name { get; set; }
        public int Energy { get; set; }
        public int Speed { get; set; }
        /// <summary>
        /// Constructor for the item class
        /// </summary>
        /// <param name="namer">Item name</param>
        /// <param name="qualifier">Item quality so if it is an Energy or a Speed item</param>
        /// <param name="value">The energy/speed value</param>
        public Item(string namer, string qualifier, int value) //constructor for the item class using the usual input format i have in my input files
        {
            name = namer;
            if (qualifier == "E")
            {
                Energy = value;
            }
            else if (qualifier == "S")
            {
                Speed = value;
            }
            else throw new Exception("Invalid item type");
        }

        public bool IsConsumeable { get; set; }

    }
}
