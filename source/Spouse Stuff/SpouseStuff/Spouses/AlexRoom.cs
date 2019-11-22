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
    class AlexRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 36 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 37 && tileY == 18 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 17 && faceDirection == 3)
                ||
                (tileX == 39 && tileY == 16 && faceDirection == 3)
                ||
                (tileX == 38 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 37 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 36 && tileY == 15 && faceDirection == 2)
            )
            {
                // Weight bench
                Game1.drawObjectDialogue("This is where Alex benchpresses. I enjoy spotting for him.");
            }
            else if (
                (tileX == 39 && tileY == 14 && faceDirection == 1)
                ||
                (tileX == 40 && tileY == 15 && faceDirection == 0)
            )
            {
                // Football
                Game1.drawObjectDialogue("The farm has plenty of space for playing football!");
            }
            else if (
                (tileX == 39 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 0)
            )
            {
                // Free weights
                Game1.drawObjectDialogue("Alex' free weights. They're quite heavy. Duh.");
            }
            else if (tileX == 35 && tileY == 14 && faceDirection == 0)
            {
                // Soccerball
                Game1.drawObjectDialogue("The farm has plenty of space for playing soccer!");
            }
            else if (
                (tileX == 36 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 14 && faceDirection == 0)
            )
            {
                // Dresser
                Game1.drawObjectDialogue("It's full of gym clothes. *Sniff* some of it is due for a wash.");
            }
        }
    }
}
