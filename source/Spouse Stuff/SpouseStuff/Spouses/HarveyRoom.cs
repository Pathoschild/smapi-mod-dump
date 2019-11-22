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
    class HarveyRoom : ISpouseRoom
    {
        public void InteractWithSpot(int tileX, int tileY, int faceDirection)
        {
            if (tileX == 38 && tileY == 14 && faceDirection == 0)
            {
                // Radio
                Game1.drawObjectQuestionDialogue("Hmm. How about I send something on air? Just what would I say to the world...", new List<Response>
                {
                    new Response("manypumpkins", "You can't have too many pumpkins, friends."),
                    new Response("neverfruitwine", "If you never tried Starfruit Wine you didn't live a life yet!"),
                    new Response("bodywant", "Get the body you always dreamed of. We have a cemetary here in Stardew Valley and I can help you dig.")
                });
            }
            else if (tileX == 39 && tileY == 13 && faceDirection == 0)
            {
                // Planes
                Game1.drawObjectDialogue("Harvey's model planes. They truly are a work of patience. Maybe we should try to make some of them fly one of these days?");
            }
            else if (
                (tileX == 37 && tileY == 16 && faceDirection == 2)
                ||
                (tileX == 36 && tileY == 16 && faceDirection == 2)
                ||
                (tileX == 35 && tileY == 17 && faceDirection == 1)
            )
            {
                // Table
                Game1.drawObjectDialogue("This is where Harvey builds and paints his plane models.");
            }
            else if (Utility.WithinRange(tileX, 38, 41) && tileY == 16 && faceDirection == 2)
            {
                // Books
                Game1.drawObjectDialogue("He should really treat his books better. Oh, this one's on radio repair!");
            }
            else if (
                (tileX == 40 && tileY == 14 && faceDirection == 0)
                ||
                (tileX == 39 && tileY == 13 && faceDirection == 1)
            )
            {
                // Book
                Game1.drawObjectDialogue("He should really treat his books better. Oh, this one's on human anatomy!");
            }
        }
    }
}
