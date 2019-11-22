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
    class SebastianRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 35 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 36 && tileY == 18 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 18 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 18 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 17 && faceDirection == 3)
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
                // Boardgame
                Game1.drawObjectDialogue("It's hard to tire on playing boardgames.");
            }
            else if (tileX == 40 && tileY == 14 && faceDirection == 0)
            {
                // Computer
                Game1.drawObjectDialogue("We should put some money aside for a better gaming computer for Sebastian. This one's getting on in years.");
            }
            else if (
                (tileX == 39 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 40 && tileY == 15 && faceDirection == 2)
            )
            {
                // Books
                Game1.drawObjectDialogue("There's something special about getting whisked away by a thrilling scifi story or a fantastic fantasy adventure.");
            }
        }
    }
}
