/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.TestMod.Framework;

namespace weizinai.StardewValleyMod.TestMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    // private IClickableMenu? lastMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.Key.JustPressed())
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
            this.Helper.Reflection.GetMethod(Game1.game1, "UpdateControlInput").Invoke(Game1.currentGameTime);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(this.ModManifest,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        );

        configMenu.AddNumberOption(this.ModManifest,
            () => this.config.RandomInt,
            value => this.config.RandomInt = value,
            I18n.Config_RandomInt_Name,
            null,
            0,
            14
        );
        configMenu.AddBoolOption(this.ModManifest,
            () => this.config.RandomBool,
            value => this.config.RandomBool = value,
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