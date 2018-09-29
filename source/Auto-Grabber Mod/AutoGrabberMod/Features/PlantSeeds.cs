using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace AutoGrabberMod.Features
{
    class PlantSeeds : Feature
    {
        public override string FeatureName => "Auto Plant Seeds";

        public override string FeatureConfig => "seed";

        public override int Order => 5;

        public override bool IsAllowed => Utilities.Config.AllowAutoSeed;

        public PlantSeeds()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;
            var dirts = Grabber.Dirts.ToArray();
            Utilities.Monitor.Log($"    {Grabber.InstanceName} Seeding tiles {dirts.Count()}", StardewModdingAPI.LogLevel.Trace);
            foreach (var dirt in dirts)
            {
                PlantSeed(dirt.Key, dirt.Value);
            }
        }

        private void PlantSeed(Vector2 tile, HoeDirt dirt)
        {
            var seed = Grabber.NextSeed;
            if (seed != null)
            {               
                //Utilities.Monitor.Log($"  {Grabber.InstanceName} attempting to seed with: {seed.Item.ParentSheetIndex} {dirt.canPlantThisSeedHere(seed.Item.ParentSheetIndex, (int)tile.X, (int)tile.Y)}", StardewModdingAPI.LogLevel.Trace);
                if (dirt.canPlantThisSeedHere(seed.Item.ParentSheetIndex, (int)tile.X, (int)tile.Y)
                    && dirt.plant(seed.Item.ParentSheetIndex, (int)tile.X, (int)tile.Y, Game1.player, false, Grabber.Location))
                {
                    seed.UseItem();
                }
            }
        }
    }
}
