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
    class LeahRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 13 && faceDirection == 1)
            )
            {
                // Easel
                Game1.drawObjectDialogue("Leah's latest work in progress. I'm excited to see how it turns out!");
            }
            else if (
                (tileX == 40 && tileY == 16 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 15 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 14 && faceDirection == 1)
            )
            {
                // Paint
                Game1.drawObjectDialogue("She'd kill me if I knocked over her paint.");
            }
            else if (
                (tileX == 37 && tileY == 13 && faceDirection == 3)
                ||
                (tileX == 36 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 35 && tileY == 13 && faceDirection == 1)
            )
            {
                // Plant
                Game1.drawObjectDialogue("Years of care and love went into growing this plant. I'm almost jealous!");
            }
        }
    }
}
