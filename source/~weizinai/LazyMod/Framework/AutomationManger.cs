/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Automation;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal class AutomationManger
{
    private ModConfig config;
    private readonly List<Automate> automations = new();
    private readonly Dictionary<int, List<Vector2>> tileCache = new();

    private GameLocation? location;
    private Farmer? player;
    private Tool? tool;
    private Item? item;
    private bool modEnable = true;

    private int ticksPerAction;
    private int skippedActionTicks;

    public AutomationManger(IModHelper helper, ModConfig config)
    {
        // 初始化
        this.config = config;
        this.ticksPerAction = config.Cooldown;
        this.InitAutomates();
        // 注册事件
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs dayStartedEventArgs)
    {
        if (this.config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(true);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs updateTickedEventArgs)
    {
        if (!this.modEnable || !this.UpdateCooldown()) return;

        (this.automations[4] as AutoFishing)!.AutoMenuFunction();

        this.UpdateAutomate();
    }

    private bool UpdateCooldown()
    {
        this.skippedActionTicks++;
        if (this.skippedActionTicks < this.ticksPerAction) return false;

        this.skippedActionTicks = 0;
        return true;
    }

    private void UpdateAutomate()
    {
        if (!Context.IsPlayerFree) return;

        this.location = Game1.currentLocation;
        this.player = Game1.player;
        this.tool = this.player?.CurrentTool;
        this.item = this.player?.CurrentItem;

        if (this.location is null || this.player is null) return;

        this.tileCache.Clear();
        foreach (var automate in this.automations) automate.AutoDoFunction(this.location, this.player, this.tool, this.item);
        this.tileCache.Clear();
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs dayEndingEventArgs)
    {
        if (this.config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(false);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.ToggleModStateKeybind.JustPressed() && Context.IsPlayerFree)
        {
            var message = this.modEnable ? new HUDMessage(I18n.Message_ModDisable()) : new HUDMessage(I18n.Message_ModEnable());
            message.noIcon = true;
            Game1.addHUDMessage(message);
            this.modEnable = !this.modEnable;
        }
    }

    private void InitAutomates()
    {
        this.automations.AddRange(new Automate[]
        {
            new AutoFarming(this.config, this.GetTileGrid),
            new AutoAnimal(this.config, this.GetTileGrid),
            new AutoMining(this.config, this.GetTileGrid),
            new AutoForaging(this.config, this.GetTileGrid),
            new AutoFishing(this.config, this.GetTileGrid),
            new AutoFood(this.config, this.GetTileGrid),
            new AutoOther(this.config, this.GetTileGrid)
        });
    }

    public void UpdateConfig(ModConfig newConfig)
    {
        foreach (var automation in this.automations)
        {
            automation.UpdateConfig(newConfig);
        }

        this.config = newConfig;
        this.ticksPerAction = this.config.Cooldown;
    }

    private List<Vector2> GetTileGrid(int range)
    {
        if (this.tileCache.TryGetValue(range, out var cache))
            return cache;

        var origin = this.player!.Tile;
        var grid = new List<Vector2>();
        for (var x = -range; x <= range; x++)
            for (var y = -range; y <= range; y++)
                grid.Add(new Vector2(origin.X + x, origin.Y + y));
        this.tileCache.Add(range, grid);
        return grid;
    }
}