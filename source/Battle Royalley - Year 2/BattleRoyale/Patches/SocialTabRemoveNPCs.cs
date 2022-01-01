/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Patches
{
    class SocialTabRemoveNPCs : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(SocialPage), "", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });

        public static void Postfix(SocialPage __instance)
        {
            var r = ModEntry.BRGame.Helper.Reflection;
            var names = r.GetField<List<object>>(__instance, "names").GetValue();
            var sprites = r.GetField<List<ClickableTextureComponent>>(__instance, "sprites").GetValue();

            for (int i = names.Count - 1; i >= 0; i--)
            {
                if (names[i] is string)//NPCs are string, players are long
                {
                    names.RemoveAt(i);
                    sprites.RemoveAt(i);
                }
            }
        }
    }
}
