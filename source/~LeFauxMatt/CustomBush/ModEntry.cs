/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush;

using HarmonyLib;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.ContentPatcher;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.CustomBush.Framework.Services;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <inheritdoc />
    public override object GetApi(IModInfo mod) => this.CreateApi(mod);

    /// <inheritdoc />
    protected override void Init(Container container)
    {
        I18n.Init(this.Helper.Translation);
        container.RegisterSingleton(() => new Harmony(this.ModManifest.UniqueID));
        container.RegisterSingleton<IApiFactory, ApiFactory>();
        container.RegisterSingleton<AssetHandler>();
        container.RegisterSingleton<ContentPatcherIntegration>();
        container.RegisterSingleton<IEventManager, EventManager>();
        container.RegisterSingleton<FauxCoreIntegration>();
        container.RegisterSingleton<ModPatches>();
        container.RegisterSingleton<IPatchManager, FauxCoreIntegration>();
        container.RegisterSingleton<ISimpleLogging, FauxCoreIntegration>();
    }
}