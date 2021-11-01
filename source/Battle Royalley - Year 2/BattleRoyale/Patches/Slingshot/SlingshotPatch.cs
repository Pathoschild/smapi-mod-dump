/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Netcode;
using StardewValley.Projectiles;
using System;

namespace BattleRoyale.Patches
{
    class SlingshotPatch1 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(BasicProjectile), null, new Type[0]);

        public static void Postfix(BasicProjectile __instance)
        {
            var r = ModEntry.BRGame.Helper.Reflection;
            r.GetField<NetBool>(__instance, "damagesMonsters").SetValue(new NetBool(false));
        }
    }
}