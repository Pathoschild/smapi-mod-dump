/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using SObject = StardewValley.Object;

namespace PrismaticStatue.Patches;
internal class PerformToolActionPatch : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
           original: GetOriginalMethod<SObject>(nameof(SObject.performToolAction)),
           prefix: GetHarmonyMethod(nameof(prefix))
        );
    }

    private static void prefix(SObject __instance)
    {
        /// Prismatic statues in other animation frames should be treated as if they are in the first frame
        //if (__instance.bigCraftable.Value && ModEntry.IsStatueID(__instance.ParentSheetIndex))
        //    __instance.ParentSheetIndex = SpeedupStatue.ID.Value;
    }
}
