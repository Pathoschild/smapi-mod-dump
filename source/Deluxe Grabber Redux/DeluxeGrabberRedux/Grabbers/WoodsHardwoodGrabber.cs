/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class WoodsHardwoodGrabber : MapGrabber
    {
        public WoodsHardwoodGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabItems()
        {
            if (!(Config.fellSecretWoodsStumps && Location is Woods woods && woods.stumps.Count > 0)) return false;
            var tool = Player.getToolFromName(Tools.Axe);

            var grabbed = false;
            for (var i = woods.stumps.Count - 1; i >= 0; i--)
            {
                var stump = woods.stumps[i];
                var tile = stump.tile.Value;
                var items = new List<SObject>();

                if (Location.HasUnlockedAreaSecretNotes(Player) && Game1.random.NextDouble() < 0.05)
                {
                    var secretNote = Location.tryToCreateUnseenSecretNote(Player);
                    if (secretNote != null) items.Add(secretNote);
                }
                Random r;
                if (Game1.IsMultiplayer)
                {
                    Game1.recentMultiplayerRandom = new Random((int)tile.X * 1000 + (int)tile.Y);
                    r = Game1.recentMultiplayerRandom;
                }
                else
                {
                    r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tile.X * 7 + (int)tile.Y * 11);
                }

                var stack = 2;

                if (tool != null)
                {
                    var power = Math.Max(1f, (tool.UpgradeLevel + 1) * 0.75f);
                    var shavingEnchantment = tool is Axe && tool.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double)(power / 12f);
                    var numHits = (int)Math.Ceiling(stump.health.Value / power);
                    if (shavingEnchantment) stack += numHits;
                }

                if (Player.professions.Contains(Farmer.forester) && r.NextDouble() < 0.5) stack++;
                items.Add(new SObject(ItemIds.Hardwood, stack));
                if (r.NextDouble() < 0.1) items.Add(new SObject(ItemIds.MahoganySeed, 1));
                if (Game1.random.NextDouble() <= 0.25 && Player.team.SpecialOrderRuleActive("DROP_QI_BEANS")) items.Add(new SObject(ItemIds.QiBeans, 1));

                if (TryAddItems(items))
                {
                    woods.stumps.RemoveAt(i);
                    grabbed = true;
                } 
            }
            return grabbed;
        }
    }
}
