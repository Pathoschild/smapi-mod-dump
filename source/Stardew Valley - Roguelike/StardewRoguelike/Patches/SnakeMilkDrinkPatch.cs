/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using System;

namespace StardewRoguelike.Patches
{
    internal class SnakeMilkDrinkPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "reduceActiveItemByOne");

        public static bool Prefix(Farmer __instance)
        {
            if (__instance.CurrentItem is not null && __instance.CurrentItem.ParentSheetIndex == 803)
            {
                int toAdd = 25;

                Roguelike.TrueMaxHP = Math.Min(Roguelike.TrueMaxHP + toAdd, Roguelike.MaxHP + (Perks.HasPerk(Perks.PerkType.Defender) ? 25 : 0));

                if (Curse.HasCurse(CurseType.GlassCannon))
                    Game1.player.health = Math.Min(Game1.player.health + toAdd, Roguelike.TrueMaxHP / 2);
                else
                    Game1.player.health = Math.Min(Game1.player.health + toAdd, Roguelike.TrueMaxHP);
            }

            return true;
        }
    }
}
