/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common;
using Common.Integrations;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using TestMod.Framework;

namespace TestMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    // private IClickableMenu? lastMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.Key.JustPressed())
            Game1.activeClickableMenu = new ShippingMenu(new List<Item>());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // if (Game1.activeClickableMenu is WheelSpinGame menu && lastMenu is not WheelSpinGame)
        // {
        //     Monitor.Log($"初始随机速度: {menu.arrowRotationVelocity}", LogLevel.Info);
        //
        //     menu.arrowRotationVelocity = Math.PI / 16.0;
        //     menu.arrowRotationVelocity += config.RandomInt * Math.PI / 256.0;
        //     if (config.RandomBool)
        //     {
        //         menu.arrowRotationVelocity += Math.PI / 64.0;
        //     }
        //
        //     Monitor.Log($"修改后随机速度({config.RandomInt}-{config.RandomBool}): {menu.arrowRotationVelocity}", LogLevel.Info);
        // }
        // lastMenu = Game1.activeClickableMenu;
        
        if (!Context.IsPlayerFree) return;

        for (int i = 0; i < 0; i++)
        {
            Helper.Reflection.GetMethod(Game1.game1, "UpdateControlInput").Invoke(Game1.currentGameTime);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => config.RandomInt,
            value => config.RandomInt = value,
            I18n.Config_RandomInt_Name,
            null,
            0,
            14
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.RandomBool,
            value => config.RandomBool = value,
            I18n.Config_RandomBool_Name
        );

        // configMenu.AddNumberOption(
        //     ModManifest,
        //     () => config.MineShaftMap,
        //     value => config.MineShaftMap = value,
        //     I18n.Config_MineShaftMap_Name,
        //     I18n.Config_MineShaftMap_ToolTip,
        //     40,
        //     60
        // );
        //
        // configMenu.AddNumberOption(
        //     ModManifest,
        //     () => config.VolcanoDungeonMap,
        //     value => config.VolcanoDungeonMap = value,
        //     I18n.Config_VolcanoDungeonMap_Name,
        //     I18n.Config_VolcanoDungeonMap_ToolTip,
        //     38,
        //     57
        // );
    }
}