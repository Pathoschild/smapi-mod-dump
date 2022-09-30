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
using StardewValley.Monsters;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(VampiricEnchantment), "_OnMonsterSlay")]
    internal class VampiricEnchantmentNerf
    {
        public static bool Prefix(Monster m, GameLocation location, Farmer who)
        {
            if (Game1.random.NextDouble() < 0.1)
            {
                int amount = Math.Max(1, (int)Math.Min((m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15 + 1)) * 0.1f, 30));
                who.health = Math.Min(who.maxHealth, Game1.player.health + amount);
                location.debris.Add(new Debris(amount, new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY()), Color.Lime, 1f, who));
                Game1.playSound("healSound");
            }

            return false;
        }
    }
}
