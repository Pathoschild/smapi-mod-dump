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
    class ElliottRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 14 && faceDirection == 0)
            )
            {
                // Bookcase
                String[] texts =
                {
                    "Elliott's books. That man is never caught without a story to read, or a story to tell.",
                    "One day, someone will have a bookcase like this filled with Elliott's stories. I know I will!",
                    "Maybe we should get another bookcase?",
                };
                Game1.drawObjectDialogue(texts[new Random().Next(0, texts.Length)]);
            }
            else if (tileX == 40 && tileY == 14 && faceDirection == 0)
            {
                // Model Boat
                Game1.drawObjectDialogue("Sometimes inspiration takes Elliott to places other than writing. This is a beautiful model boat, made by hand.");
            }
            else if (tileX == 36 && tileY == 17 && faceDirection == 2)
            {
                // Plant
                Game1.drawObjectDialogue("Elliott's potted plant. I guess we all long for nature in our own way.");
            }
        }
    }
}

