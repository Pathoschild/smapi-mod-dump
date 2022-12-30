/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCtorPatcher"/> class.</summary>
    internal SlingshotCtorPatcher()
    {
        this.Target = this.RequireConstructor<Slingshot>(Type.EmptyTypes);
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);

        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
        base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireConstructor<Slingshot>(Type.EmptyTypes);
        base.UnapplyImpl(harmony);

        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
        base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Add Rascal ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotCtorPostfix(Slingshot __instance)
    {
        if (!Game1.player.HasProfession(Profession.Rascal))
        {
            return;
        }

        __instance.numAttachmentSlots.Value = 2;
        __instance.attachments.SetCount(2);
    }

    #endregion harmony patches
}
