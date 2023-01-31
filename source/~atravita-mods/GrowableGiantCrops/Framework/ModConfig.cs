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

namespace GrowableGiantCrops.Framework;
public sealed class ModConfig
{
    [GMCMDefaultIgnore]
    public Vector2 ShopLocation { get; set; } = new(1, 7);

    public bool ShouldNPCsTrampleGiantCrops { get; set; } = true;

    public bool RelaxedPlacement { get; set; } = false;

    private int shovelEnergy = 7;

    [GMCMRange(0, 10)]
    public int ShovelEnergy
    {
        get => this.shovelEnergy;
        set => Math.Clamp(this.shovelEnergy, 0, 10);
    }
}
