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
using StardewValley.Objects;

namespace BNWCore.Grabbers
{
    class IndoorPotGrabber : ObjectsMapGrabber
    {
        public IndoorPotGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }
        public override bool GrabObject(Vector2 tile, StardewValley.Object obj)
        {
            if (ModEntry.Config.BNWCoreharvestCropsIndoorPots && obj is IndoorPot pot && pot.hoeDirt.Value.crop != null)
            {
                var dirt = pot.hoeDirt.Value;
                var crops = Helpers.HarvestCropFromHoeDirt(Player, dirt, tile, !ModEntry.Config.BNWCoreflowers, out int exp);
                var availableGrabbers = Helpers.GetNearbyObjectsToTile(tile, GrabberPairs, Grabber_Config.harvestCropsRange, Grabber_Config.harvestCropsRangeMode);
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
                } else
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
