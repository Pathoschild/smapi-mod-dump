/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class Horses
    {
        private const double horseChance = 0.2;

        private readonly List<Chest> unopenedChests;

        public Horses(List<Chest> chests)
        {
            unopenedChests = chests;
        }

        public void Update(object o, object e)
        {
            var opened = unopenedChests.Where(c => c.StardewChest.mutex.IsLocked() || c.StardewChest.mutex.IsLockHeld()).ToArray();
            foreach (var l in opened)
            {
                GameLocation location = l.Location;
                StardewValley.Objects.Chest chest = l.StardewChest;
                Vector2 tile = l.TilePosition;

                Console.WriteLine($"Opened chest, is outdoors = {location.IsOutdoors}");
                unopenedChests.Remove(l);
                if (location.IsOutdoors && Game1.random.NextDouble() <= horseChance && !l.BeenOpened)
                {
                    Console.WriteLine($"Spawning horse at {location.Name} @ ({tile.X}, {tile.Y})");

                    var horse = new StardewValley.Characters.Horse(Guid.NewGuid(), (int)tile.X, (int)tile.Y);
                    location.addCharacter(horse);
                }
                l.BeenOpened = true;
            }

            if (unopenedChests.Count == 0)
                ModEntry.Events.GameLoop.UpdateTicked -= Update;
        }
    }
}
