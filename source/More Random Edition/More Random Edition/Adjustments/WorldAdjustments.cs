/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System.Collections.Generic;
using System.Linq;
using RandomizerItem = Randomizer.Item;
using StardewValleyNpc = StardewValley.NPC;
using SVChest = StardewValley.Objects.Chest;
using SVItem = StardewValley.Item;

namespace Randomizer
{
    /// <summary>
    /// Adjustments that are for the state of the game world in general
    /// </summary>
    public class WorldAdjustments
    {
        /// <summary>
        /// Fixes the item name that you get at the start of the game
        /// </summary>
        public static void FixParsnipSeedBox()
        {
            // We don't want to do this before we're initialized
            if (ItemList.Items == null)
            {
                return;
            }

            GameLocation farmHouse = Game1.getLocationFromName("FarmHouse");
            if (farmHouse?.Objects != null)
            {
                SVChest chest =
                    farmHouse.Objects.Values.Where(x =>
                        x.DisplayName == "Chest")
                        .Cast<SVChest>()
                        .Where(x => x.giftbox.Value)
                    .FirstOrDefault();

                if (chest != null && chest.items.Count == 1)
                {
                    SVItem itemInChest = chest.items[0];
                    if (itemInChest.ParentSheetIndex == (int)ObjectIndexes.ParsnipSeeds)
                    {
                        itemInChest.DisplayName = ItemList.GetItemName(ObjectIndexes.ParsnipSeeds);
                    }
                }
            }
        }

        /// <summary>
        /// Fixes the foragables on day 1 - the save file is created too quickly for it to be
        /// randomized right away, so we'll change them on the spot on the first day
        /// </summary>
        public static void ChangeDayOneForagables()
        {
            SDate currentDate = SDate.Now();
            if (currentDate.DaysSinceStart < 2)
            {
                List<GameLocation> locations = Game1.locations
                    .Concat(
                        from location in Game1.locations.OfType<BuildableGameLocation>()
                        from building in location.buildings
                        where building.indoors.Value != null
                        select building.indoors.Value
                    ).ToList();

                List<RandomizerItem> newForagables =
                    ItemList.GetForagables(Seasons.Spring)
                        .Where(x => x.ShouldBeForagable) // Removes the 1/1000 items
                        .Cast<RandomizerItem>().ToList();

                foreach (GameLocation location in locations)
                {
                    List<int> foragableIds = ItemList.GetForagables().Select(x => x.Id).ToList();
                    List<Vector2> tiles = location.Objects.Pairs
                        .Where(x => foragableIds.Contains(x.Value.ParentSheetIndex))
                        .Select(x => x.Key)
                        .ToList();

                    foreach (Vector2 oldForagableKey in tiles)
                    {
                        RandomizerItem newForagable = Globals.RNGGetRandomValueFromList(newForagables, Game1.random);
                        location.Objects[oldForagableKey].ParentSheetIndex = newForagable.Id;
                        location.Objects[oldForagableKey].Name = newForagable.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Identical to the logic in Farm.cs (spawnFlyingMonstersOffscreen) for the giant bat spawn, 
        /// but we check for the randomized weapon instead
        /// </summary>
        public static void TrySpawnGalaxySwordBat()
        {
            // Besdies the Randomize check, this logic is taken from Farm.performTenMinuteUpdate
            // to NOT try to spawn the bat at the wrong time, at the wrong farm
            if (!Globals.Config.Weapons.Randomize ||
                !Game1.spawnMonstersAtNight || 
                Game1.farmEvent != null || 
                Game1.timeOfDay < 1900 || 
                Game1.random.NextDouble() >= 0.25 - Game1.player.team.AverageDailyLuck() / 2.0)
            {
                return;
            }

            var currentLocation = Game1.currentLocation;
            if (currentLocation.Name != "Farm" ||
                !(Game1.random.NextDouble() < 0.25))
            {
                return;
            }

            if (WeaponRandomizer.Weapons.TryGetValue((int)WeaponIndexes.GalaxySword, out var galaxySword)) {
                if (Game1.player.CombatLevel >= 10 &&
                    Game1.random.NextDouble() < 0.01 &&
                    Game1.player.hasItemInInventoryNamed(galaxySword.Name))
                {
                    NetCollection<StardewValleyNpc> characters = currentLocation.characters;
                    Bat bat = new(Vector2.Zero * 64f, 9999)
                    {
                        focusedOnFarmers = true,
                        wildernessFarmMonster = true
                    };
                    characters.Add(bat);
                }
            }
        }
    }
}
