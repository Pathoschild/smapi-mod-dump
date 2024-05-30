/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Services;

using StardewMods.FauxCore.Common.Interfaces;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
using StardewMods.FauxCore.Framework.Interfaces;

/// <inheritdoc />
internal sealed class ApiFactory : IApiFactory
{
    private readonly IAssetHandlerExtension assetHandler;
    private readonly IExpressionHandler expressionHandler;
    private readonly IModConfig modConfig;
    private readonly IThemeHelper themeHelper;

    /// <summary>Initializes a new instance of the <see cref="ApiFactory" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="expressionHandler">Dependency used for parsing expressions.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="themeHelper">Dependency used for swapping palettes.</param>
    public ApiFactory(
        IAssetHandlerExtension assetHandler,
        IExpressionHandler expressionHandler,
        IModConfig modConfig,
        IThemeHelper themeHelper)
    {
        this.assetHandler = assetHandler;
        this.expressionHandler = expressionHandler;
        this.modConfig = modConfig;
        this.themeHelper = themeHelper;
    }

    /// <inheritdoc />
    public object CreateApi(IModInfo modInfo) =>
        new FauxCoreApi(modInfo, this.assetHandler, this.expressionHandler, this.modConfig, this.themeHelper);
}