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
    class EmilyRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (
                (tileX == 35 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 36 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 13 && faceDirection == 3)
                ||
                (tileX == 37 && tileY == 18 && faceDirection == 3)
                ||
                (tileX == 36 && tileY == 17 && faceDirection == 2)
                ||
                (tileX == 35 && tileY == 17 && faceDirection == 2)
                ||
                (tileX == 34 && tileY == 18 && faceDirection == 1)
            )
            {
                // Bushes
                Game1.drawObjectDialogue("*Sniff* mmmh. The leaves on these bushes give the place a lovely scent.");
            }
            else if (
                (tileX == 40 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 13 && faceDirection == 1)
            )
            {
                // Palm Tree
                Game1.drawObjectDialogue("With a heat lamp we might be able to grow coconuts... Nah, I won't turn Emily's space into another part of the farm!");
            }
            else if (tileX == 38 && tileY == 18 && faceDirection == 1)
            {
                // Cactus
                Game1.drawObjectDialogue("Ouch! That'll teach me not to touch Emily's stuff. Is this where she gets her needles from? No, that would be silly.");
            }
            else if (
                (tileX == 39 && tileY == 13 && faceDirection == 3)
                ||
                (tileX == 38 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 37 && tileY == 13 && faceDirection == 1)
            )
            {
                // Rack
                Game1.drawObjectDialogue("Emily's... Rack? For tailoring, I suppose? She knows infinitely more about this stuff than me.");
            }
            else if (tileX == 37 && tileY == 13 && faceDirection == 0)
            {
                // Cactus
                Game1.drawObjectDialogue("It's a painting of Calico Desert. Emily probably got it from Sandy!");
            }
            else if (
                (tileX == 40 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 39 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 38 && tileY == 15 && faceDirection == 2)
                ||
                (tileX == 37 && tileY == 16 && faceDirection == 1)
                ||
                (tileX == 37 && tileY == 17 && faceDirection == 1)
                ||
                (tileX == 38 && tileY == 18 && faceDirection == 0)
            )
            {
                // Tailoring Table
                String[] texts =
                {
                    "This is where Emily cuts and sews and makes amazing clothes!",
                    "Emily's sewing table. Oh, that reminds me, I forgot to give her back the pair of scissors I borrowed.",
                    "I should get Emily some more wool to work with.",
                };
                Game1.drawObjectDialogue(texts[new Random().Next(0, texts.Length)]);
            }
        }
    }
}
