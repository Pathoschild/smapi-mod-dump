using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace AutoGrabberMod.Features
{
    class WaterFields : Feature
    {
        public override string FeatureName => "Auto Water";

        public override string FeatureConfig => "water";

        public override int Order => 6;

        public override bool IsAllowed => Utilities.Config.AllowAutoWater;

        public WaterFields()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;
            int totalCapacity = Grabber.TotalWateringCapacity;
            //Utilities.Monitor.Log($"    {Grabber.InstanceName} Total watering capacity {totalCapacity}", StardewModdingAPI.LogLevel.Trace);
            if (totalCapacity != 0)
            {
                foreach (KeyValuePair<Vector2, HoeDirt> pair in Grabber.Dirts.Take(totalCapacity).ToArray())
                {
                    if (totalCapacity <= 0) break;
                    pair.Value.state.Value = HoeDirt.watered;
                    if (Grabber.Location.Objects.ContainsKey(pair.Key) && Grabber.Location.Objects[pair.Key] is IndoorPot) Grabber.Location.Objects[pair.Key].showNextIndex.Value = true;
                    totalCapacity--;
                }
            }
        }
    }
}
