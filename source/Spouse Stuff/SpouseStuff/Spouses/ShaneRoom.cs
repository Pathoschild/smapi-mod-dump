using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Minigames;
using SpouseStuff.Spouses;
using SpouseStuff;

namespace SpouseStuff.Spouses
{
    class ShaneRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 35 && tileY == 14 && faceDirection == 1)
                ||
                (tileX == 36 && tileY == 15 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 14 && faceDirection == 3)
                ||
                (tileX == 40 && tileY == 16 && faceDirection == 2)
            )
            {
                // Beer
                Game1.drawObjectDialogue("Depression sadly still hits Shane every now and then. But we'll get through it together.");
            }
            else if (tileX == 38 && tileY == 14 && faceDirection == 0)
            {
                // Fridge
                Game1.drawObjectDialogue("Shane's man cave fridge. Always stocked with ice cold JoJa Cola!");
            }
            else if (tileX == 39 && tileY == 16 && faceDirection == 1)
            {
                // ???
                Game1.drawObjectDialogue("I don't even know what that is. Pizza? Just a slice in a triangular box, left behind in ages past?");
            }
            else if (
                (tileX == 39 && tileY == 15 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 1)
            )
            {
                // Game Station
                Game1.currentMinigame = new MineCart(5,3);
            }
            else if (tileX == 39 && tileY == 15 && faceDirection == 1 && Game1.currentMinigame == null) // Avoid tripping this interaction while playing on the console.
            {
                // Books
                Game1.drawObjectDialogue("We should get Shane a bookshelf for these.");
            }
            else if (
                (tileX == 39 && tileY == 16 && faceDirection == 2)
                ||
                (tileX == 38 && tileY == 17 && faceDirection == 1)
            )
            {
                // Free weight
                Game1.drawObjectDialogue("Exercise can be a good way of clearing your head.");
            }
            else if (
                (tileX == 38 && tileY == 17 && faceDirection == 2)
                ||
                (tileX == 37 && tileY == 17 && faceDirection == 2)
            )
            {
                // Plant
                Game1.drawObjectDialogue("Nothing puts your mind at ease quite like caring for something living. I should know, I'm a farmer!");
            }
            else if (tileX == 36 && tileY == 17 && faceDirection == 2)
            {
                // Parcel
                Game1.drawObjectDialogue("We should put this in the attic. Also, we should get an attic.");
            }
        }
    }
}
