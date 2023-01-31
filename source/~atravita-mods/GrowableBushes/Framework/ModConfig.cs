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

using Microsoft.Xna.Framework;

namespace GrowableBushes.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    public bool CanAxeAllBushes { get; set; } = false;

    [GMCMDefaultIgnore]
    public Vector2 ShopLocation { get; set; } = new(1, 7);

    public bool ShouldNPCsTrampleBushes { get; set; } = true;

    public bool RelaxedPlacement { get; set; } = false;
}
