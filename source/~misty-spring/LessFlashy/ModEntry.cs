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
using LessFlashy.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LessFlashy;

public sealed class ModEntry : Mod
{
    internal static IModHelper Help { get; private set; }
    
    internal static ModConfig Config { get; private set; }
    
    internal static void Log(string msg, LogLevel lv = LogLevel.Trace) => Mon.Log(msg, lv);

    private static IMonitor Mon { get; set; }
    
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        helper.Events.Content.AssetRequested += Asset.Requested;

        Config = helper.ReadConfig<ModConfig>();
        Help = Helper;
        Mon = Monitor;

        var harmony = new Harmony(ModManifest.UniqueID);
        ProjectilePatches.Apply(harmony);
        GameLocationPatches.Apply(harmony);
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null)
            return;
        
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Animations.name"),
            tooltip: () => Helper.Translation.Get("config.Animations.description"),
            getValue: () => Config.Animations,
            setValue: value => Config.Animations = value,
            allowedValues: new[]{"All","Basic","NoFlash","Minimal"},
            formatAllowedValue: value => Helper.Translation.Get($"config.Animations.values.{value}")
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.BombLight.name"),
            tooltip: () => Helper.Translation.Get("config.BombLight.description"),
            getValue: () => Config.SoftLight,
            setValue: value => Config.SoftLight = value
        );

        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Menus",
            text: () => Helper.Translation.Get("config.Menus.name")
        );
        
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Monsters",
            text: () => Helper.Translation.Get("config.Monsters.name")
        );
        
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "World",
            text: () => Helper.Translation.Get("config.World.name")
        );
        
        #region menus
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Menus",
            pageTitle: () => Helper.Translation.Get("config.Menus.name")
        );
        
        configMenu.AddSectionTitle(
            mod:  ModManifest,
            text: () => Helper.Translation.Get("config.Menus.name")
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Forge.name"),
            tooltip: () => Helper.Translation.Get("config.Menus.description"),
            getValue: () => Config.Forge,
            setValue: value => Config.Forge = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Sewing.name"),
            tooltip: () => Helper.Translation.Get("config.Menus.description"),
            getValue: () => Config.Sewing,
            setValue: value => Config.Sewing = value
        );
        #endregion
        
        #region world
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "World",
            pageTitle: () => Helper.Translation.Get("config.World.name")
        );
        
        configMenu.AddSectionTitle(
            mod:  ModManifest,
            text: () => Helper.Translation.Get("config.World.name")
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.FishingBubbles.name"),
            tooltip: () => Helper.Translation.Get("config.FishingBubbles.description"),
            getValue: () => Config.FishingBubbles,
            setValue: value => Config.FishingBubbles = value,
            allowedValues: new[]{"All","Basic","NoFlash","Minimal"},
            formatAllowedValue: value => Helper.Translation.Get($"config.FishingBubbles.values.{value}")
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Lava.name"),
            tooltip: () => Helper.Translation.Get("config.Lava.description"),
            getValue: () => Config.Lava,
            setValue: value => Config.Lava = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Water.name"),
            tooltip: () => Helper.Translation.Get("config.Water.description"),
            getValue: () => Config.Water,
            setValue: value => Config.Water = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Rain.name"),
            tooltip: () => Helper.Translation.Get("config.Rain.description"),
            getValue: () => Config.Rain,
            setValue: value => Config.Rain = value,
            min: 0.0f,
            max: 1.0f,
            interval: 0.05f
        );
        #endregion
        
        #region npcs
        
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Monsters",
            pageTitle: () => Helper.Translation.Get("config.Monsters.name")
        );
        
        configMenu.AddSectionTitle(
            mod:  ModManifest,
            text: () => Helper.Translation.Get("config.Monsters.name")
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.SlimeBall.name"),
            tooltip: () => Helper.Translation.Get("config.SlimeBall.description"),
            getValue: () => Config.SlimeBall,
            setValue: value => Config.SlimeBall = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Bugs.name"),
            getValue: () => Config.Bugs,
            setValue: value => Config.Bugs = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Magma.name"),
            getValue: () => Config.Magma,
            setValue: value => Config.Magma = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Projectiles.name"),
            getValue: () => Config.Projectiles,
            setValue: value => Config.Projectiles = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Serpents.name"),
            getValue: () => Config.Serpents,
            setValue: value => Config.Serpents = value
        );
        #endregion
    }
}