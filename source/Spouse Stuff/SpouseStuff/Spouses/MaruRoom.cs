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
    class MaruRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 34 && tileY == 14 && faceDirection == 1)
                ||
                (tileX == 35 && tileY == 15 && faceDirection == 0)
                ||
                (tileX == 36 && tileY == 14 && faceDirection == 3)
            )
            {
                // Book
                Game1.drawObjectDialogue("A technical manual. Something about field effect transistors.");
            }
            else if (
                (tileX == 36 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 13 && faceDirection == 3)
            )
            {
                // BlueComputer
                Game1.drawObjectDialogue("Whatever is supposed to run on this is showing a blue screen with an error message instead.");
            }
            else if (
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 13 && faceDirection == 1)
            )
            {
                // Telescope
                Game1.drawObjectDialogue("Maru's telescope. You can get such a clear view of the sky from Stardew Valley. Much better than back in the city.");
            }
            else if (
                (tileX == 39 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 40 && tileY == 14 && faceDirection == 0)
            )
            {
                // BlackComputer
                Game1.drawObjectDialogue("It's running something called Folding At Home. Better not touch.");
            }
            else if (
                (tileX == 40 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 39 && tileY == 16 && faceDirection == 1)
            )
            {
                // Contraption
                Game1.drawObjectDialogue("Maru's explained it to me twice and I still have no idea what this is.");
            }
            else if (
               (tileX == 39 && tileY == 16 && faceDirection == 2)
               ||
               (tileX == 38 && tileY == 17 && faceDirection == 1)
               ||
               (tileX == 39 && tileY == 18 && faceDirection == 0)
            )
            {
                // Hammer
                Game1.drawObjectDialogue("I wonder if Maru'll let me borrow her hammer to fix the fences.");
            }
            else if (tileX == 39 && tileY == 18 && faceDirection == 1)
            {
                // Plant
                Game1.drawObjectDialogue("Why go outside when you can bring outside in with you?");
            }
            else if (
                (tileX == 37 && tileY == 18 && faceDirection == 0)
                ||
                (tileX == 36 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 37 && tileY == 16 && faceDirection == 2)
                ||
                (tileX == 38 && tileY == 17 && faceDirection == 3)
            )
            {
                // Robot
                Game1.drawObjectDialogue("I for one welcome our new robot overlord. Once Maru finishes it, at least.");
            }
        }
    }
}
