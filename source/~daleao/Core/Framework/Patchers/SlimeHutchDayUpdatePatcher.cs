/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlimeHutchDayUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SlimeHutch>(nameof(SlimeHutch.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to color Slime Balls.</summary>
    [HarmonyPostfix]
    private static void SlimeHutchDayUpdatePostfix(SlimeHutch __instance)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());
        var slimes = __instance.characters.OfType<GreenSlime>().ToArray();
        foreach (var @object in __instance.Objects.Values)
        {
            if (@object.QualifiedItemId == QualifiedBigCraftableIds.SlimeBall)
            {
                @object.orderData.Set(getSlimeString(slimes.Choose(r)));
            }
        }

        string getSlimeString(GreenSlime slime)
        {
            var color = slime.color.Value;
            var isTiger = slime.Name == "Tiger Slime";
            return $"{color.PackedValue}/{isTiger}/{slime.firstGeneration.Value}/{slime.specialNumber.Value}";
        }
    }

    #endregion harmony patches
}
