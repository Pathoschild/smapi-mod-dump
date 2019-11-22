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
    class HaleyRoom : ISpouseRoom
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
                // Camera
                Game1.drawObjectDialogue("Haley takes such amazing photographs with this camera.");
            }
            else if (
                (tileX == 36 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 14 && faceDirection == 0)
            )
            {
                // Pinboard
                Game1.drawObjectDialogue("We should use this pinboard more. Maybe a picture collage of the four seasons on the farm?");
            }
            else if (
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 14 && faceDirection == 0)
            )
            {
                // Dresser
                Game1.drawObjectDialogue("Haley still has a decent collection of beautiful clothes. She's got impeccable taste in fashion!");
            }
            else if (tileX == 40 && tileY == 14 && faceDirection == 0)
            {
                // Teddy
                Game1.drawObjectDialogue("Awwwwwwwww!");
            }
            else if (
                (tileX == 40 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 39 && tileY == 16 && faceDirection == 1)
            )
            {
                // Makeup Table
                Game1.drawObjectDialogue("Haley is incredible at makeup. I should ask her for some tips.");
            }
        }
    }
}