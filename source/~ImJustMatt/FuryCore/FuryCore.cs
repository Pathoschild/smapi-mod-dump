/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore;

using Common.Helpers;
using StardewModdingAPI;
using StardewMods.FuryCore.Models;
using StardewMods.FuryCore.Services;

/// <inheritdoc />
public class FuryCore : Mod
{
    /// <summary>
    ///     Gets the unique Mod Id.
    /// </summary>
    internal static string ModUniqueId { get; private set; }

    private ConfigData Config { get; set; }

    private ModServices Services { get; } = new();

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        FuryCore.ModUniqueId = this.ModManifest.UniqueID;
        Log.Monitor = this.Monitor;
        I18n.Init(this.Helper.Translation);
        this.Config = this.Helper.ReadConfig<ConfigData>();

        this.Services.Add(
            new AssetHandler(this.Helper),
            new CommandHandler(this.Helper, this.Services),
            new ConfigureGameObject(this.Config, this.Helper, this.ModManifest, this.Services),
            new CustomEvents(this.Helper, this.Services),
            new CustomTags(this.Config, this.Services),
            new GameObjects(this.Helper, this.Services),
            new HarmonyHelper(),
            new MenuComponents(this.Helper, this.Services),
            new MenuItems(this.Config, this.Helper, this.Services),
            new ModConfigMenu(this.Config, this.Helper, this.ModManifest),
            new HudComponents(this.Config, this.Helper, this.Services));
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new FuryCoreApi(this.Services);
    }
}