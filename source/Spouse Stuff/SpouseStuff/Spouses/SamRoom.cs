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
    class SamRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 36 && tileY == 14 && faceDirection == 0)
            )
            {
                // Bookcase
                Game1.drawObjectDialogue("Sam's got a decent collection of books here. Many different genres, too!");
            }
            else if (
                (tileX == 37 && tileY == 13 && faceDirection == 1)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 0)
            )
            {
                // Guitar
                Game1.drawObjectDialogue("This is Sam's electric guitar. Honestly, Sam's a wizard on this thing. I've never heard riffs quite like his.");
            }
            else if (
                (tileX == 39 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 40 && tileY == 14 && faceDirection == 0)
            )
            {
                // Keyboard
                Game1.drawObjectDialogue("Sam's keyboard. I rarely see him play it though, I mostly see him practice the guitar.");
            }
            else if (
                (tileX == 39 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 40 && tileY == 15 && faceDirection == 2)
            )
            {
                // Computer
                Game1.drawObjectDialogue("Looks like Sam's in the middle of editing a song on his computer. Better leave it alone.");
            }
            else if (tileX == 39 && tileY == 18 && faceDirection == 1)
            {
                // Plant
                Game1.drawObjectDialogue("Hmm. It could use a spot of water, I think.");
            }
        }
    }
}
