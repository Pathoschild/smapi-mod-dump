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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.FreeLock.Framework;

namespace weizinai.StardewValleyMod.FreeLock;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.viewportFreeze) return;
        
        var mouseX = Game1.getOldMouseX(false);
        var mouseY = Game1.getOldMouseY(false);
        var moveSpeed = this.config.MoveSpeed;
        var moveThreshold = this.config.MoveThreshold;
        
        // 水平移动
        if (mouseX < moveThreshold)
            Game1.panScreen(-moveSpeed, 0);
        else if (mouseX - Game1.viewport.Width >= -moveThreshold) 
            Game1.panScreen(moveSpeed, 0);
        
        // 垂直移动
        if (mouseY < moveThreshold)
            Game1.panScreen(0, -moveSpeed);
        else if (mouseY - Game1.viewport.Height >= -moveThreshold) 
            Game1.panScreen(0, moveSpeed);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.config.FreeLockKeybind.JustPressed())
        {
            Game1.viewportFreeze = !Game1.viewportFreeze;
            var message = new HUDMessage(Game1.viewportFreeze ? I18n.UI_ViewportUnlocked() : I18n.UI_ViewportLocked())
            {
                noIcon = true
            };
            Game1.addHUDMessage(message);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForFreeLock(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}