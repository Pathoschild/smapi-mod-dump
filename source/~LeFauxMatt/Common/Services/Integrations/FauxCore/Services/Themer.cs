/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

/// <inheritdoc />
internal sealed class Themer(FauxCoreIntegration fauxCore) : IThemeHelper
{
    private readonly IThemeHelper themeHelper = fauxCore.Api!.CreateThemeService();

    /// <inheritdoc />
    public void AddAssets(string[] assetNames) => this.themeHelper.AddAssets(assetNames);
}