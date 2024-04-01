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
internal sealed class StardewAquarium : IMethodIntegration
{
    /// <inheritdoc />
    public int Index => 1;

    /// <inheritdoc />
    public string HoverText => I18n.Button_StardewAquarium();

    /// <inheritdoc />
    public string ModId => "Cherry.StardewAquarium";

    /// <inheritdoc />
    public string MethodName => "OpenAquariumCollectionMenu";

    /// <inheritdoc />
    public object?[] Arguments => ["aquariumprogress", Array.Empty<string>()];
}