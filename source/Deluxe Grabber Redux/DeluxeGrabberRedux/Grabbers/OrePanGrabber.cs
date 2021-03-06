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
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class OrePanGrabber : MapGrabber
    {
        public OrePanGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabItems()
        {
            // impl @ StardewValley::Pan::getPanitems
            if (!Config.orePan || Location.orePanPoint.Value.Equals(Point.Zero)) return false;
            var items = new List<Item>();

            int whichOre = ItemIds.CopperOre;
            int whichExtra = -1;
            Random r = new Random(Location.orePanPoint.X + Location.orePanPoint.Y * 1000 + (int)Game1.stats.DaysPlayed);
            double roll = r.NextDouble() - (double)Player.team.AverageLuckLevel() * 0.001 - Player.team.AverageDailyLuck();
            if (roll < 0.01)
            {
                whichOre = ItemIds.IridiumOre;
            }
            else if (roll < 0.241)
            {
                whichOre = ItemIds.GoldOre;
            }
            else if (roll < 0.6)
            {
                whichOre = ItemIds.IronOre;
            }
            int orePieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)Player.team.AverageLuckLevel() / 10f) + Player.team.AverageDailyLuck()) * 2.0);
            int extraPieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)Player.team.AverageLuckLevel() / 10f)) * 2.0);
            roll = r.NextDouble() - Player.team.AverageDailyLuck();
            if (roll < 0.4 + (double)Player.team.AverageLuckLevel() * 0.04)
            {
                roll = r.NextDouble() - Player.team.AverageDailyLuck();
                whichExtra = ItemIds.Coal;
                if (roll < 0.02 + (double)Player.team.AverageLuckLevel() * 0.002)
                {
                    whichExtra = ItemIds.Diamond;
                    extraPieces = 1;
                }
                else if (roll < 0.1)
                {
                    whichExtra = ItemIds.Emerald + r.Next(5) * 2;
                    extraPieces = 1;
                }
                else if (roll < 0.36)
                {
                    whichExtra = ItemIds.OmniGeode;
                    extraPieces = Math.Max(1, extraPieces / 2);
                }
                else if (roll < 0.5)
                {
                    whichExtra = ((r.NextDouble() < 0.3) ? ItemIds.FireQuartz : ((r.NextDouble() < 0.5) ? ItemIds.FrozenTear : ItemIds.EarthCrystal));
                    extraPieces = 1;
                }
                if (roll < (double)Player.team.AverageLuckLevel() * 0.002)
                {
                    items.Add(new Ring(ItemIds.LuckyRing));
                }
            }
            items.Add(new SObject(whichOre, orePieces));
            if (whichExtra != -1)
            {
                items.Add(new SObject(whichExtra, extraPieces));
            }
            if (Location is IslandNorth && (Game1.getLocationFromName("IslandNorth") as IslandNorth).bridgeFixed.Value && r.NextDouble() < 0.2)
            {
                items.Add(new SObject(ItemIds.FossilizedTail, 1));
            }
            else if (Location is IslandLocation && r.NextDouble() < 0.2)
            {
                items.Add(new SObject(ItemIds.TaroTuber, r.Next(2, 6)));
            }

            if (TryAddItems(items))
            {
                Location.orePanPoint.Value = Point.Zero;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
