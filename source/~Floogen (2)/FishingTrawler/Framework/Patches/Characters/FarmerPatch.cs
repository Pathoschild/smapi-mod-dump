/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Resources;
using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace FishingTrawler.Framework.Patches.Characters
{
    internal class FarmerPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(Farmer);

        public FarmerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_ActiveObject", null), postfix: new HarmonyMethod(GetType(), nameof(IsCarringPostfix)));
        }

        private static void IsCarringPostfix(Farmer __instance, ref Object __result)
        {
            if (CoalClump.IsValid(__result) || SeaborneTackle.IsValid(__result))
            {
                __result = null;
            }
        }
    }
}
