/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeluxeGrabberRedux.Grabbers
{
    class HarvestableCropHoeDirtGrabber : TerrainFeaturesMapGrabber
    {
        public HarvestableCropHoeDirtGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabFeature(Vector2 tile, TerrainFeature feature)
        {
            if (Config.harvestCrops && feature is HoeDirt dirt && dirt.crop != null)
            {
                var crops = Helpers.HarvestCropFromHoeDirt(Player, dirt, tile, !Config.flowers, out int exp);
                var availableGrabbers = Helpers.GetNearbyObjectsToTile(tile, GrabberPairs, Config.harvestCropsRange, Config.harvestCropsRangeMode);
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
