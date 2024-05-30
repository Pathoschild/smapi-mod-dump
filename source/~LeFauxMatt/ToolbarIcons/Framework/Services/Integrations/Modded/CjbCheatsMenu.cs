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

using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.ToolbarIcons.Framework.Interfaces;

/// <inheritdoc />
internal sealed class CjbCheatsMenu : IMethodIntegration
{
    /// <inheritdoc />
    public object?[] Arguments => [0, true];

    /// <inheritdoc />
    public string HoverText => I18n.Button_CheatsMenu();

    /// <inheritdoc />
    public string Icon => VanillaIcon.QualityIridium.ToStringFast();

    /// <inheritdoc />
    public string MethodName => "OpenCheatsMenu";

    /// <inheritdoc />
    public string ModId => "CJBok.CheatsMenu";
}