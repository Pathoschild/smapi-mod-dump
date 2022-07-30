/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ToolbarIcons;

using StardewModdingAPI;

/// <inheritdoc />
internal class ToolbarIconsIntegration : ModIntegration<IToolbarIconsApi>
{
    private const string ModUniqueId = "furyx639.ToolbarIcons";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ToolbarIconsIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ToolbarIconsIntegration(IModRegistry modRegistry)
        : base(modRegistry, ToolbarIconsIntegration.ModUniqueId)
    {
    }
}