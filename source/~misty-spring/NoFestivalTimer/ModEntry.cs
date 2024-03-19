/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using NoFestivalTimer.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NoFestivalTimer;

public class ModEntry : Mod
{
    private ModConfig Config { get; set; }
    
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.Content.AssetRequested += AssetRequested;
        helper.Events.Content.AssetsInvalidated += AssetInvalidated;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        
        Config = Helper.ReadConfig<ModConfig>();
        
        Mon = Monitor;
        Help = Helper;

        var harmony = new Harmony(ModManifest.UniqueID);

        EventPatches.Apply(harmony);
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
        
        configMenu?.AddKeybind(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Keybind.name"),
            tooltip: () => Helper.Translation.Get("config.Keybind.description"),
            getValue: () => Config.ToggleButton,
            setValue: value => Config.ToggleButton = value
            );
    }

    private void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        Exclusions =Helper.GameContent.Load<Dictionary<string, ExclusionData>>($"Mods/{Help.ModRegistry.ModID}/Exclusions");
    }

    private void AssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Help.ModRegistry.ModID}/Exclusions", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ExclusionData>()
                {
                    {"iceFishing", new(true,5,false)},
                    {"eggHunt", new(true,0,true)}
                },
                AssetLoadPriority.Low
            );
        }
    }
    
    private void AssetInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Help.ModRegistry.ModID}/Exclusions"))) 
            return;
        
        Exclusions =Helper.GameContent.Load<Dictionary<string, ExclusionData>>($"Mods/{Help.ModRegistry.ModID}/Exclusions");
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button != Config.ToggleButton)
            return;
        
        Toggle = !Toggle;
        var mode = Toggle ? "Strings/UI:Options_GamepadMode_ForceOn" : "Strings/UI:Options_GamepadMode_ForceOff";
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(mode),Toggle ? 3 : 2));
        Game1.playSound(Toggle ? "coin" : "cancel");
    }

    public static Dictionary<string, ExclusionData> Exclusions { get; set; } = new();

    internal static IModHelper Help { get; set; }
    internal static IMonitor Mon { get; set; }
    internal static bool Toggle { get; set; } = true;
}