/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add quality to Ecologist Mushroom Boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (!__instance.IsMushroomBox() || __instance.heldObject.Value is null ||
            !Game1.MasterPlayer.HasProfession(Profession.Ecologist))
            return;

        __instance.heldObject.Value.Quality = Game1.MasterPlayer.GetEcologistForageQuality();
    }

    #endregion harmony patches
}