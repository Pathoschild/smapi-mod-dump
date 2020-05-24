using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace DeluxeHats.Hats
{
    public static class ButterflyBow
    {
        public const string Name = "Butterfly Bow";
        public const string Description = "Spawns Butterflies around you while outside.\nPeople in the same outdoor area as you will gain 5 friendship every 7 seconds.";
        public static void Activate()
        {
            
            HatService.OnUpdateTicked = (e) =>
            {
                if (!Game1.currentLocation.isOutdoors || Game1.player.hasMenuOpen || !Game1.player.canMove || !Game1.game1.IsActive)
                {
                    return;
                }
                var critters = HatService.Helper.Reflection.GetField<List<Critter>>(Game1.currentLocation, "critters").GetValue();
                if (critters != null && (e.Ticks%30) == 0 && critters.Count < 340)
                {
                    var randomX = Game1.player.getTileLocation().X + Game1.random.Next(3);
                    var randomY = Game1.player.getTileLocation().Y + Game1.random.Next(3);
                    critters.Add(new Butterfly(new Vector2(randomX, randomY)));
                }
                if (e.Ticks % 480 == 0)
                {
                    foreach (var npc in Game1.currentLocation.getCharacters())
                    {
                        Game1.player.changeFriendship(5, npc);
                    }
                }
                
            };
        }

        public static void Disable()
        {
        }
    }
}
