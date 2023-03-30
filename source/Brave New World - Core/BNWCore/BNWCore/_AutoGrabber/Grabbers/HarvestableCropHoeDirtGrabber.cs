/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BNWCore.Grabbers
{
    class HarvestableCropHoeDirtGrabber : TerrainFeaturesMapGrabber
    {
        public HarvestableCropHoeDirtGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }
        public override bool GrabFeature(Vector2 tile, TerrainFeature feature)
        {
            if (ModEntry.Config.BNWCoreharvestCrops && feature is HoeDirt dirt && dirt.crop != null)
            {
                var crops = Helpers.HarvestCropFromHoeDirt(Player, dirt, tile, !ModEntry.Config.BNWCoreflowers, out int exp);
                var availableGrabbers = Helpers.GetNearbyObjectsToTile(tile, GrabberPairs, InternalConfig.harvestCropsRange, InternalConfig.harvestCropsRangeMode);
                if (TryAddItems(crops, availableGrabbers))
                {
                    if (dirt.crop.regrowAfterHarvest.Value == -1)
                    {
                        dirt.destroyCrop(tile, false, Location);
                    }
                    else
                    {
                        dirt.crop.fullyGrown.Value = true;
                        dirt.crop.dayOfCurrentPhase.Value = dirt.crop.regrowAfterHarvest.Value;
                    }
                    GainExperience(Skills.Farming, exp);
                    return true;
                }
                else
                {
                    return false;
                }
            } 
            else
            {
                return false;
            }
        }
    }
}
