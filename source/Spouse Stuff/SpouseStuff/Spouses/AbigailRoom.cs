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
    class AbigailRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (Utility.WithinRange(tileX, 34, 37) && Utility.WithinRange(tileY, 13, 16) && faceDirection == 0)
            {
                // SNES
                Game1.currentLocation.performAction("Arcade_Prairie", Game1.player, new xTile.Dimensions.Location());
            }
            else if (tileX == 40 && tileY == 14 && faceDirection == 0)
            {
                // Sword
                Game1.drawObjectDialogue("Abigail's sword. It's remarkably well-balanced. With a bit more training, she could be unstoppable with it.");
            }
            else if (tileX == 39 && tileY == 14 && faceDirection == 0)
            {
                // Ouija Board
                Game1.drawObjectDialogue("A Spirit Board. The spirits don't seem very active today.");
            }
            else if (Utility.WithinRange(tileX, 36, 39) && tileY == 14 && faceDirection == 0)
            {
                // Guinea pig tank
                Game1.drawObjectDialogue("David Jr. Abby's guinea pig. He's adorable!");
            }
            else if (
                (tileX == 38 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 18 && faceDirection == 0)
                ||
                (Utility.WithinRange(tileX, 38, 41) && tileY == 16 && faceDirection == 2)
            )
            {
                // Drums
                Game1.drawObjectDialogue("Abigail's drumkit. I don't think I've ever actually seen her play. A shame, she's great on the flute afterall.");
            }
        }
    }
}
