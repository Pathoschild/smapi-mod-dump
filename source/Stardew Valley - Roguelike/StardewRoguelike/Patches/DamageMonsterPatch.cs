/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(
        typeof(GameLocation),
        "damageMonster",
        new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) })
    ]
    internal class DamageMonsterPatch
    {
        public static bool Prefix(ref float knockBackModifier, ref float critChance, ref float critMultiplier)
        {
            if (Perks.HasPerk(Perks.PerkType.Precision))
                critMultiplier *= 1.2f;

            if (Perks.HasPerk(Perks.PerkType.Desperado))
                critMultiplier += 1.5f;

            if (Curse.HasCurse(CurseType.PlayerKnockback))
                knockBackModifier *= 2;

            knockBackModifier = Math.Min(knockBackModifier, 2f);

            if (Curse.HasCurse(CurseType.MoreCritsLessDamage))
            {
                critChance = 0.25f;
                critMultiplier *= 0.5f;
            }

            return true;
        }

        public static void Postfix(bool __result, int minDamage, int maxDamage)
        {
            if (__result)
            {
                if (Curse.HasCurse(CurseType.BrittleCrown))
                {
                    int avg = (maxDamage + minDamage) / 2;
                    Game1.player.Money += Math.Max(1, avg / 10);
                    if (Game1.random.NextDouble() < 0.25)
                        Game1.playSound("coin");
                }

                if (Perks.HasPerk(Perks.PerkType.Leech) && Game1.random.NextDouble() < 0.25)
                    Game1.player.health = Math.Min(Game1.player.health + 1, Game1.player.maxHealth);
            }
        }
    }
}
