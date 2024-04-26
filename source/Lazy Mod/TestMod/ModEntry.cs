/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Integration;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TestMod.Framework;
using TestMod.Patches;

namespace TestMod;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new MineShaftPatcher(config), new VolcanoDungeonPatcher(config));
    }
    
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.MineShaftMap,
            value => config.MineShaftMap = value,
            I18n.Config_MineShaftMap_Name,
            I18n.Config_MineShaftMap_ToolTip,
            40,
            60
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => config.VolcanoDungeonMap,
            value => config.VolcanoDungeonMap = value,
            I18n.Config_VolcanoDungeonMap_Name,
            I18n.Config_VolcanoDungeonMap_ToolTip,
            38,
            57
        );
    }
}