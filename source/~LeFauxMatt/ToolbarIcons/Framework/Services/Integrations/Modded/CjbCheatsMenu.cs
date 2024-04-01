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
internal sealed class CjbCheatsMenu : IMethodIntegration
{
    /// <inheritdoc />
    public string ModId => "CJBok.CheatsMenu";

    /// <inheritdoc />
    public int Index => 2;

    /// <inheritdoc />
    public string HoverText => I18n.Button_CheatsMenu();

    /// <inheritdoc />
    public string MethodName => "OpenCheatsMenu";

    /// <inheritdoc />
    public object?[] Arguments => [0, true];
}