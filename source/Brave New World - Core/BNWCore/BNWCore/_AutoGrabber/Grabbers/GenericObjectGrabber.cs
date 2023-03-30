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
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace BNWCore.Grabbers
{
    class GenericObjectGrabber : ObjectsMapGrabber
    {
        public GenericObjectGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }
        private static readonly HashSet<int> ForageableItems = new HashSet<int>()
        {
            ItemIds.Truffle
        };
        public override bool GrabObject(Vector2 tile, SObject obj)
        {
            if (IsGrabbable(obj))
            {
                if (TryAddItem(Helpers.SetForageStatsBasedOnProfession(Player, obj, tile)))
                {
                    Location.Objects.Remove(tile);
                    GainExperience(Skills.Foraging, 7);
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
        private HashSet<int> GetBeachForageItems()
        {
            var items = new HashSet<int>() {393, 397, 152};
            if (Game1.content.Load<Dictionary<string, string>>("Data\\Locations").TryGetValue("Beach", out string beachData))
            {
                var splitBySeasons = beachData.Split('/');
                for (int seasonIdx = 0; seasonIdx < 4; seasonIdx++)
                {
                    var split = splitBySeasons[seasonIdx].Split(' ');
                    for (int whichItem = 0; whichItem < split.Length; whichItem += 2)
                    {
                        items.Add(Convert.ToInt32(split[whichItem]));
                    }
                }
            }
            return items;
        }
        private bool IsGrabbable(SObject obj)
        {
            if (obj.bigCraftable.Value) return false;
            else if (obj.isForage(null) && obj.ParentSheetIndex != ItemIds.GoldenWalnut) return true;
            else if (Location is Beach && GetBeachForageItems().Contains(obj.ParentSheetIndex)) return true;
            else return ForageableItems.Contains(obj.ParentSheetIndex);
        }
    }
}
