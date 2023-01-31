/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace BetterIntegratedModItems.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Accessors kept near fields.")]
internal sealed class ModConfig
{
    public bool AffectDailyQuests { get; set; } = true;

    public bool AffectTrashBear { get; set; } = true;

    private int trashBearUnlockDays = 224;

    [GMCMInterval(7)]
    [GMCMRange(0, 500)]
    public int TrashBearUnlockDays
    {
        get => this.trashBearUnlockDays;
        set => this.trashBearUnlockDays = Math.Clamp(value, 0, 500);
    }
}
