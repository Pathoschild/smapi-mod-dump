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
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Accessors kept near fields.")]
internal sealed class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether or not players should be able to axe non-placed bushes.
    /// </summary>
    public bool CanAxeAllBushes { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not bushes in greenhouses should bear fruit all the time.
    /// </summary>
    public bool GreenhouseBushesAlwaysBloom { get; set; } = false;

    /// <summary>
    /// Gets or sets where the default shop location is.
    /// </summary>
    [GMCMDefaultVector(1, 7)]
    public Vector2 ShopLocation { get; set; } = new(1, 7);

    /// <summary>
    /// Gets or sets a value indicating whether or not the bush shop should have a little graphic.
    /// </summary>
    public bool ShowBushShopGraphic { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not npcs should be able to trample bushes in their way.
    /// </summary>
    public bool ShouldNPCsTrampleBushes { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to disable some placement rules.
    /// </summary>
    public bool RelaxedPlacement { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not mod data should be preserved.
    /// </summary>
    public bool PreserveModData { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not bushes should stack.
    /// </summary>
    public bool AllowBushStacking { get; set; } = true;

    private int shopCostScale = 5;

    /// <summary>
    /// Gets or sets a value indicating how to scale the shop cost.
    /// </summary>
    [GMCMRange(1, 10)]
    public int ShopCostScale
    {
        get => this.shopCostScale;
        set => this.shopCostScale = Math.Clamp(value, 1, 10);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not bushes should show up in the furniture catalogue.
    /// </summary>
    public bool BushesInFurnitureCatalogue { get; set; } = true;
}
