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
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace BNWCore
{
    class SlimeHutchGrabber : ObjectsMapGrabber
    {
        public SlimeHutchGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }
        public override bool GrabObject(Vector2 tile, SObject obj)
        {
            if (ModEntry.Config.BNWCoreslimeHutch && obj.Name == "Slime Ball")
            {
                List<SObject> items = new List<SObject>();
                var r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame + (int)tile.X * 77 + (int)tile.Y * 777 + 2);
                var amount = r.Next(10, 21);
                var slimes = new SObject(ItemIds.Slime, amount);
                items.Add(slimes);
                int extraSlimesAmount = 0;
                while (r.NextDouble() < 0.33) extraSlimesAmount++;
                var extraSlimes = new SObject(ItemIds.PetrifiedSlime, extraSlimesAmount);
                items.Add(extraSlimes);
                if (TryAddItems(items))
                {
                    Location.Objects.Remove(tile);
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
