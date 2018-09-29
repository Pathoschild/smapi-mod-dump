using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace AutoGrabberMod.Features
{
    class Fertilize : Feature
    {
        public override string FeatureName => "Auto Fertilize Soil";

        public override string FeatureConfig => "fertilize";

        public override int Order => 4;

        public override bool IsAllowed => Utilities.Config.AllowAutoFertilize;

        public Fertilize()
        {
            Value = false;
        }
        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;
            var dirts = Grabber.Dirts.ToArray();
            Utilities.Monitor.Log($"    {Grabber.InstanceName} Fertilizing tiles {dirts.Count()}", StardewModdingAPI.LogLevel.Trace);
            foreach (var dirt in dirts)
            {
                FertilizeSoil(dirt.Key, dirt.Value);
            }
        }

        private void FertilizeSoil(Vector2 tile, HoeDirt dirt)
        {
            if (dirt.fertilizer.Value == 0 && Grabber.NearbyChests.Any((chest) => chest.Fertilizers.Any()))
            {
                var fertilizer = Grabber.NextFertilizer;
                if (fertilizer != null
                    && dirt.canPlantThisSeedHere(fertilizer.Item.ParentSheetIndex, (int)tile.X, (int)tile.Y, true)
                    && dirt.plant(fertilizer.Item.ParentSheetIndex, (int)tile.X, (int)tile.Y, Game1.player, true, Grabber.Location))
                {
                    fertilizer.UseItem();
                }
            }
        }
    }
}
