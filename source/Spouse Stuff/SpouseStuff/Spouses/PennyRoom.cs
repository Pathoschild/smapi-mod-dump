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
    class PennyRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 36 && tileY == 13 && faceDirection == 3)
            )
            {
                // Plant
                Game1.drawObjectDialogue("Penny's plant grows even better than any of my crops. She doesn't do anything halfway.");
            }
            else if (
                (tileX == 37 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 14 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 15 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 39 && tileY == 18 && faceDirection == 1)
            )
            {
                // Bookcases
                String[] texts =
                {
                    "I swear, every time I look at Penny's bookshelves, there's a new set of books I have never seen here before.",
                    "There is nobody with a passion for reading greater than Penny's. Wow.",
                    "Maybe we could expand the farmhouse a bit to allow for more bookcases. These ones are full to the brim!",
                    "So many books. I wonder how long it would take to read them all.",
                    "There's a special kind of peace to be found in a place full of knowledge and stories."
                };
                Game1.drawObjectDialogue(texts[new Random().Next(0, texts.Length)]);
            }
        }
    }
}
