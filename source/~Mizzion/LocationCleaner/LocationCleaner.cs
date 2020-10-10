/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationCleaner.Framework;
using LocationCleaner.Framework.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LocationCleaner
{
    public class LocationCleaner : Mod
    {
        private LocationConfig _config;


        //Collection of data
        private List<LocationData> _data;
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<LocationConfig>();
            var events = helper.Events;

            //Events
            events.GameLoop.DayStarted += OnDayStarted; // Will be used to clear majority of the maps.
            events.Player.Warped += OnWarped; //Will control other areas, example mines

            
        }

        //Event Methods
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GrabLocationData();
            foreach (var i in _data)
            {
                Monitor.Log($"locName: {i.location.Name} => Objects: {i.locObj.Count} => Terrains: {i.locTerrain.Count} => Clumps: {i.locResource.Count}");
            }
        }

        private void OnWarped(object sener, WarpedEventArgs e)
        {
            //Coming soon
        }

        //Custom Methods to make life easier.

        private void GrabLocationData()
        {
            _data = new List<LocationData>();
            foreach (var loc in GetAllLocations())
            {
                var locData = new LocationData(loc.Name);
                _data.Add(locData);
            }
        }
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        private int CutWeeds()
        {
            if (Game1.random.NextDouble() < 0.5)
                return 771;
            if (Game1.random.NextDouble() < 0.05)
                return 770;

            return 0;
        }

        private int BreakStone(int x, int y)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
            if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
                return 535 + (Game1.stats.DaysPlayed <= 60U || random.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || random.NextDouble() >= 0.2 ? 0 : 2) : 1);
            if (random.NextDouble() < 0.035 * (Game1.player.professions.Contains(21) ? 2.0 : 1.0) &&
                Game1.stats.DaysPlayed > 1U)
                return 382;
            if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
                return 390;

            return 390;
        }

        private int GetSeed(Tree tree)
        {
            int seedId = 0;
            switch (tree.treeType.Value)
            {
                case 1://Oak
                    seedId = 309;
                    break;
                case 2://Maple
                    seedId = 310;
                    break;
                case 3://Pine
                    seedId = 311;
                    break;
                default:
                    seedId = 309;
                    break;
            }

            return seedId;
        }

        private Item DoItem(int itemId)
        {
            Item i = new SObject(itemId, 1);
            return i;
        }
        private Item DoItem(int itemId, int amount)
        {
            Item i = new SObject(itemId, amount);
            return i;
        }
        internal enum TreeType
        {
            Oak = Tree.bushyTree,
            Maple = Tree.leafyTree,
            Pine = Tree.pineTree,
            Palm = Tree.palmTree,
            BigMushroom = Tree.mushroomTree
        }

        internal enum TreeStage
        {
            Seed = StardewValley.TerrainFeatures.Tree.seedStage,
            Sprout = StardewValley.TerrainFeatures.Tree.sproutStage,
            Sapling = StardewValley.TerrainFeatures.Tree.saplingStage,
            Bush = StardewValley.TerrainFeatures.Tree.bushStage,
            SmallTree = StardewValley.TerrainFeatures.Tree.treeStage - 1, // an intermediate stage between bush and tree, no constant
            Tree = StardewValley.TerrainFeatures.Tree.treeStage
        }
    }
}
