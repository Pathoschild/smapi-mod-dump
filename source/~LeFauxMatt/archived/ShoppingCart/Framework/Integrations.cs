/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ShoppingCart.Framework;

using StardewMods.Common.Integrations.GenericModConfigMenu;
using StardewMods.Common.Integrations.StackQuality;

/// <summary>
///     Handles integrations with other mods.
/// </summary>
internal sealed class Integrations
{
#nullable disable
    private static Integrations instance;
#nullable enable

    private readonly GenericModConfigMenuIntegration genericModConfigMenu;
    private readonly StackQualityIntegration stackQualityIntegration;

    private Integrations(IModHelper helper)
    {
        this.genericModConfigMenu = new(helper.ModRegistry);
        this.stackQualityIntegration = new(helper.ModRegistry);
    }

    /// <summary>
    ///     Gets Generic Mod Config Menu integration.
    /// </summary>
    public static GenericModConfigMenuIntegration GMCM => Integrations.instance.genericModConfigMenu;

    /// <summary>
    ///     Gets Stack Quality integration.
    /// </summary>
    public static StackQualityIntegration StackQuality => Integrations.instance.stackQualityIntegration;

    /// <summary>
    ///     Initializes <see cref="Integrations" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="Integrations" /> class.</returns>
    public static Integrations Init(IModHelper helper)
    {
        return Integrations.instance ??= new(helper);
    }
}