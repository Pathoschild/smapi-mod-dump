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
internal sealed class Themer : IThemeHelper
{
    private readonly Lazy<IThemeHelper> themeHelper;

    /// <summary>Initializes a new instance of the <see cref="Themer"/> class.</summary>
    /// <param name="fauxCoreIntegration">Dependency used for FauxCore integration.</param>
    public Themer(FauxCoreIntegration fauxCoreIntegration) =>
        this.themeHelper = new Lazy<IThemeHelper>(() => fauxCoreIntegration.Api!.CreateThemeService());

    /// <inheritdoc />
    public IManagedTexture AddAsset(string path, IRawTextureData data) => this.themeHelper.Value.AddAsset(path, data);
}