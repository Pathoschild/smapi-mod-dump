/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.Automate;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate", "Automate")]
internal sealed class TrackedItemCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TrackedItemCtorPatcher"/> class.</summary>
    internal TrackedItemCtorPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.TrackedItem".ToType()
            .RequireConstructor(typeof(Item), typeof(Action<Item>), typeof(Action<Item>));
    }

    #region harmony patches

    /// <summary>Patch to fix collected rings from crab pots.</summary>
    [HarmonyPrefix]
    private static void TrackedItemCtorPrefix(ref Item item)
    {
        if (!item.ParentSheetIndex.IsAnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
        {
            return;
        }

        item = item.ParentSheetIndex switch
        {
            14 or 51 => new MeleeWeapon(item.ParentSheetIndex),
            _ => new Ring(item.ParentSheetIndex),
        };
    }

    #endregion harmony patches
}
