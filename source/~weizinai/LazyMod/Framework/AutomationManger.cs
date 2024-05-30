/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using LazyMod.Framework.Automation;
using LazyMod.Framework.Config;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LazyMod.Framework;

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
        ticksPerAction = config.Cooldown;
        InitAutomates();
        // 注册事件
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += OnDayEnding;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs dayStartedEventArgs)
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(true);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs updateTickedEventArgs)
    {
        if (!modEnable || !UpdateCooldown()) return;

        (automations[4] as AutoFishing)!.AutoMenuFunction();

        UpdateAutomate();
    }

    private bool UpdateCooldown()
    {
        skippedActionTicks++;
        if (skippedActionTicks < ticksPerAction) return false;

        skippedActionTicks = 0;
        return true;
    }

    private void UpdateAutomate()
    {
        if (!Context.IsPlayerFree) return;

        location = Game1.currentLocation;
        player = Game1.player;
        tool = player?.CurrentTool;
        item = player?.CurrentItem;

        if (location is null || player is null) return;

        tileCache.Clear();
        foreach (var automate in automations) automate.AutoDoFunction(location, player, tool, item);
        tileCache.Clear();
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs dayEndingEventArgs)
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(false);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.ToggleModStateKeybind.JustPressed() && Context.IsPlayerFree)
        {
            var message = modEnable ? new HUDMessage(I18n.Message_ModDisable()) : new HUDMessage(I18n.Message_ModEnable());
            message.noIcon = true;
            Game1.addHUDMessage(message);
            modEnable = !modEnable;
        }
    }

    private void InitAutomates()
    {
        automations.AddRange(new Automate[]
        {
            new AutoFarming(config, GetTileGrid),
            new AutoAnimal(config, GetTileGrid),
            new AutoMining(config, GetTileGrid),
            new AutoForaging(config, GetTileGrid),
            new AutoFishing(config, GetTileGrid),
            new AutoFood(config, GetTileGrid),
            new AutoOther(config, GetTileGrid)
        });
    }

    public void UpdateConfig(ModConfig newConfig)
    {
        foreach (var automation in automations)
        {
            automation.UpdateConfig(newConfig);
        }

        config = newConfig;
        ticksPerAction = config.Cooldown;
    }

    private List<Vector2> GetTileGrid(int range)
    {
        if (tileCache.TryGetValue(range, out var cache))
            return cache;

        var origin = player!.Tile;
        var grid = new List<Vector2>();
        for (var x = -range; x <= range; x++)
            for (var y = -range; y <= range; y++)
                grid.Add(new Vector2(origin.X + x, origin.Y + y));
        tileCache.Add(range, grid);
        return grid;
    }
}