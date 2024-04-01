/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Services.Integrations.Modded;

using StardewMods.ToolbarIcons.Framework.Interfaces;

/// <inheritdoc />
internal sealed class DynamicGameAssets : IMethodIntegration
{
    /// <inheritdoc />
    public string ModId => "spacechase0.DynamicGameAssets";

    /// <inheritdoc />
    public int Index => 3;

    /// <inheritdoc />
    public string HoverText => I18n.Button_DynamicGameAssets();

    /// <inheritdoc />
    public string MethodName => "OnStoreCommand";

    /// <inheritdoc />
    public object?[] Arguments => ["dga_store", Array.Empty<string>()];
}