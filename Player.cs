using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Christmas_Game
{

    public class Player
    {
        public enum Directions { Left, Right, Up, Down } //directions the player can move
        public string Name { get; private set; }
        public Player(string namer = "GooglyMoogly02654") //default name
        {
            Name = namer;
        }


        public double Energy { get; set; }
        public int Speed { get; set; }

        public Item? SelectedItem { get; set; }

        public Room CurrentRoom { get; set; }
        /// <summary>
        /// Attempts to move the player in the specified direction.
        /// </summary>
        /// <param name="direction">The Direction to try to move the player to</param>
        public void Move(Directions direction)
        {
            bool hasMoved = false;
            if (direction == Directions.Left) //check if the player can move in the direction
            {
                if (CurrentRoom!.Left != null)
                {
                    CurrentRoom = CurrentRoom.Left;
                    hasMoved = true; //if the player can move, move the player
                }
            }
            else if (direction == Directions.Right)
            {
                if (CurrentRoom!.Right != null)
                {
                    CurrentRoom = CurrentRoom.Right;
                    hasMoved = true;
                }
            }
            else if (direction == Directions.Up)
            {
                if (CurrentRoom!.Top != null)
                {
                    CurrentRoom = CurrentRoom.Top;
                    hasMoved = true;
                }
            }
            else if (direction == Directions.Down)
            {
                if (CurrentRoom!.Bottom != null)
                {
                    CurrentRoom = CurrentRoom.Bottom;
                    hasMoved = true;
                }
            }

            if (hasMoved) //only if the player has moved, subtract energy
            {
                if (this.SelectedItem != null)
                    Energy -= (1.00 / (Speed + this.SelectedItem!.Speed));
                else
                {
                    Energy -= (1.00 / Speed);
                }
            }

        }
    }
}
