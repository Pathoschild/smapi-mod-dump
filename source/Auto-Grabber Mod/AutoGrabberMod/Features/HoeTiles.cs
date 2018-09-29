using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;

namespace AutoGrabberMod.Features
{
    class HoeTiles : Feature
    {
        public override string FeatureName => "Auto Hoe Nearby Tiles";

        public override string FeatureConfig => "hoe";

        public override int Order => 3;

        public override bool IsAllowed => Utilities.Config.AllowAutoHoe;

        public HoeTiles()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;
            //Utilities.Monitor.Log($"    {Grabber.InstanceName} Hoeing tiles? {(bool)Value}", StardewModdingAPI.LogLevel.Trace);
            if (Grabber.RangeEntireMap)
            {
                //Utilities.Monitor.Log($"    {Grabber.InstanceName} Hoeing Entire map", StardewModdingAPI.LogLevel.Trace);
                //Gets height and width of map based on tiles found on map
                int width = 0;
                int height = 0;
                while (Grabber.Location.isTileOnMap(new Vector2(width, 0))) width++;
                while (Grabber.Location.isTileOnMap(new Vector2(0, height))) height++;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++) Utilities.MakeHoeDirt(x, y, Grabber.Location);
                }
            }
            else
            {
                //Utilities.Monitor.Log($"    {Grabber.InstanceName} Hoeing Range {Grabber.Range}", StardewModdingAPI.LogLevel.Trace);
                foreach (Vector2 tile in Grabber.NearbyTilesRange) Utilities.MakeHoeDirt((int)tile.X, (int)tile.Y, Grabber.Location);
            }
        }
    }
}
