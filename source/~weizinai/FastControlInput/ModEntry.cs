/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.FastControlInput.Framework;
using weizinai.StardewValleyMod.FastControlInput.Patcher;

namespace weizinai.StardewValleyMod.FastControlInput;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new ModHooksPatcher(() => this.config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForFastControlInput(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}