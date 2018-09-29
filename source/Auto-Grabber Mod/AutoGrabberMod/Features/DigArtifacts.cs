using System.Linq;
using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AutoGrabberMod.Features
{
    using SVObject = StardewValley.Object;

    class DigArtifacts : Feature
    {
        public override string FeatureName => "Auto Dig Artifacts";

        public override string FeatureConfig => "dig";

        public override int Order => 1;

        public override bool IsAllowed => Utilities.Config.AllowAutoDig;

        public DigArtifacts()
        {
            Value = false;
        }

        public override void Action()
        {
            if (!IsAllowed || !(bool)Value) return;

            //dig up artifacts
            foreach (var loc in (Grabber.RangeEntireMap ? Utilities.GetLocationObjectTiles(Grabber.Location) : Grabber.NearbyTilesRange).ToArray())
            {
                if (Grabber.Location.Objects.ContainsKey(loc) && Grabber.Location.Objects[loc].ParentSheetIndex == 590 && !Grabber.Location.isTileHoeDirt(loc) && !Grabber.IsChestFull)
                {
                    Utilities.DigUpArtifactSpot((int)loc.X, (int)loc.Y, Grabber.Location);
                    Grabber.Location.Objects.Remove(loc);
                    Grabber.Location.terrainFeatures.Add(loc, new HoeDirt());
                    //Utilities.Monitor.Log($"    {Grabber.InstanceName} digging up artifact {loc.X},{loc.Y} {Grabber.Location.debris.Count()}", StardewModdingAPI.LogLevel.Trace);
                }
            }

            foreach (var debris in Grabber.Location.debris.ToArray())
            {
                if (debris == null) continue;
                var debriItem = new SVObject(debris.Chunks.First().debrisType, 1);
                //handle picking up a book
                if (debriItem.parentSheetIndex == 102)
                {
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(debriItem);
                }
                else if (!debriItem.Name.ToLower().Contains("error") && debriItem.ParentSheetIndex > 0)
                {
                    Grabber.GrabberChest.addItem(debriItem);
                }
                if (!debriItem.Name.ToLower().Contains("error") && debriItem.ParentSheetIndex > 0) Grabber.Location.debris.Remove(debris);
                //Utilities.Monitor.Log($"    **{Grabber.InstanceName} debris {debris.debrisType?.Value} {debriItem.Name} type {debriItem.Type} index {debriItem.ParentSheetIndex}", StardewModdingAPI.LogLevel.Trace);
            }
        }
    }
}
