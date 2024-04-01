/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DynamicFestivalRewards;

public sealed class ModEntry : Mod
{
    internal static IMonitor Mon { get; set; }
    internal static ModConfig Config { get; set; }
    
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        
        Config = Helper.ReadConfig<ModConfig>();
        Mon = Monitor;
        
        var harmony = new Harmony(ModManifest.UniqueID);
        Patches.EventPatches.Apply(harmony);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        
        // register mod
        configMenu?.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
        
        
        configMenu?.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.MinValue.name"),
            tooltip: () => Helper.Translation.Get("config.MinValue.description"),
            getValue: () => Config.MinValue,
            setValue: value => Config.MinValue = value
        );
        
        configMenu?.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.MaxValue.name"),
            tooltip: () => Helper.Translation.Get("config.MaxValue.description"),
            getValue: () => Config.MaxValue,
            setValue: value => Config.MaxValue = value
        );
        
        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Game1.content.LoadString("Strings/StringsFromCSFiles:JukeboxRandomTrack"),
            tooltip: () => Helper.Translation.Get("config.Randomize.description"),
            getValue: () => Config.Randomize,
            setValue: value => Config.Randomize = value
        );
        
        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.UseYear.name"),
            tooltip: () => Helper.Translation.Get("config.UseYear.description"),
            getValue: () => Config.UseYearInstead,
            setValue: value => Config.UseYearInstead = value
        );
    }
}