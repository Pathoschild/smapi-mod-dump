/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Tools;

namespace FishingTrawler.Framework.Patches.Objects
{
    internal class FishingRodPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(FishingRod);

        public FishingRodPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "doDoneFishing", new[] { typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DoDoneFishingPrefix)));
        }

        private static bool DoDoneFishingPrefix(FishingRod __instance, bool consumeBaitAndTackle)
        {
            if (__instance.attachments[1] != null && SeaborneTackle.IsValid(__instance.attachments[1]))
            {
                __instance.attachments[1].uses.Value = int.MinValue;
            }

            return true;
        }
    }
}
