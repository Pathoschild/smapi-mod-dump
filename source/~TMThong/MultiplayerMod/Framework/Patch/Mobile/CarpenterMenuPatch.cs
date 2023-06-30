/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class CarpenterMenuPatch : IPatch
    {
        private readonly Type PATCH_TYPE = typeof(CarpenterMenu);

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(PATCH_TYPE, new Type[] { typeof(bool) }), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_ctor)));
        }

        private static void postfix_ctor(CarpenterMenu __instance)
        {
            List<BluePrint> blueprints = ModUtilities.Helper.Reflection.GetField<List<BluePrint>>(__instance, "blueprints").GetValue();
            int numCabins = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
            if (Game1.IsMasterGame && numCabins < Game1.CurrentPlayerLimit - 1)
            {
                blueprints.Add(new BluePrint("Stone Cabin"));
                blueprints.Add(new BluePrint("Plank Cabin"));
                blueprints.Add(new BluePrint("Log Cabin"));
            }
        }
    }
}
