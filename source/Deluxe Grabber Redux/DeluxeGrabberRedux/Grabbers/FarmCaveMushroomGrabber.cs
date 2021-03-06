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
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class FarmCaveMushroomGrabber : ObjectsMapGrabber
    {
        private readonly Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetMushroomHarvest;

        public FarmCaveMushroomGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
            GetMushroomHarvest = Mod.Api.GetMushroomHarvest ?? DefaultGetMushroomHarvest;
        }

        private KeyValuePair<SObject, int> DefaultGetMushroomHarvest(SObject mushroom, Vector2 mushroomBoxTile, GameLocation location)
        {
            mushroom.Quality = SObject.lowQuality;
            return new KeyValuePair<SObject, int>(mushroom, 0);
        }

        public override bool GrabObject(Vector2 tile, SObject obj)
        {
            // impl @ StardewValley::Object::DayUpdate::case 128
            if (Config.farmCaveMushrooms && obj.ParentSheetIndex == ItemIds.MushroomBox && obj.heldObject.Value != null)
            {
                var harvest = GetMushroomHarvest(obj.heldObject.Value, tile, Location);
                if (TryAddItem(harvest.Key))
                {
                    obj.heldObject.Value = null;
                    GainExperience(Skills.Foraging, harvest.Value);
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
