/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Linq;

namespace BattleRoyale.Patches
{
    class FarmerReceiveDamage : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "damageMonster",
            new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) });

        public static void Prefix(
            GameLocation __instance,
            Rectangle areaOfEffect,
            ref int minDamage,
            ref int maxDamage,
            float critChance,
            float critMultiplier,
            Farmer who
        )
        {
            var players = Game1.getOnlineFarmers().Where(x => x != who && x.currentLocation == who.currentLocation);

            foreach (Farmer player in players)
            {
                if (player.GetBoundingBox().Intersects(areaOfEffect))
                {
                    if (maxDamage >= 0)
                    {
                        int damageAmount = Game1.random.Next(minDamage, maxDamage + 1);

                        bool crit = false;
                        if (who != null && Game1.random.NextDouble() < (critChance + who.LuckLevel * (critChance / 40f)))//Change?
                        {
                            crit = true;
                            __instance.playSound("crit");
                        }
                        damageAmount = (crit ? ((int)((float)damageAmount * critMultiplier)) : damageAmount);
                        damageAmount = Math.Max(1, damageAmount + ((who != null) ? (who.attack * 3) : 0));

                        FarmerUtils.TakeDamage(player, DamageSource.PLAYER, damageAmount, Game1.player.UniqueMultiplayerID);
                    }
                }
            }
        }
    }
}
