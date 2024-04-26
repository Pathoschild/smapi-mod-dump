/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewWebApi.Game.World;
using StardewWebApi.Game.Items;
using StardewWebApi.Game.NPCs;
using StardewWebApi.Server;

namespace StardewWebApi.Game.Events.Processors;
public record SaveLoadedEventData(
    string FarmName,
    string PlayerName
);

public record SavedEventData(
    string FarmName,
    string PlayerName
);

public abstract class DayEventDataBase
{
    private readonly DayInfo _dayInfo;

    public DayEventDataBase(DayInfo dayInfo)
    {
        _dayInfo = dayInfo;
    }

    public Date Date => _dayInfo.Date;

    public string Weather => _dayInfo.Weather;

    public IEnumerable<NPCStub> Birthdays => _dayInfo.Birthdays;
}

public class DayStartedEventData : DayEventDataBase
{
    public DayStartedEventData(DayInfo dayInfo) : base(dayInfo) { }
}

public class DayEndingEventData : DayEventDataBase
{
    public DayEndingEventData(DayInfo dayInfo) : base(dayInfo) { }
}

public record TimeChangedEventData(
    int OldTime,
    int NewTime
);

public class PlayerInventoryStackSizeChange
{
    private readonly ItemStackSizeChange _change;

    public PlayerInventoryStackSizeChange(ItemStackSizeChange change)
    {
        _change = change;
    }

    public BasicItem Item => BasicItem.FromItem(_change.Item)!;
    public int OldSize => _change.OldSize;
    public int NewSize => _change.NewSize;
}

public class PlayerInventoryChangedEventData
{
    private readonly InventoryChangedEventArgs _e;

    public PlayerInventoryChangedEventData(InventoryChangedEventArgs e)
    {
        _e = e;
    }

    public string PlayerName => _e.Player.Name;
    public IEnumerable<BasicItem> Added => BasicItem.FromItems(_e.Added);
    public IEnumerable<BasicItem> Removed => BasicItem.FromItems(_e.Removed);
    public IEnumerable<PlayerInventoryStackSizeChange> QuantityChanged => _e.QuantityChanged
        .Select(i => new PlayerInventoryStackSizeChange(i));
}

public record PlayerLevelChangedEventData(
    string PlayerName,
    string Skill,
    int OldLevel,
    int NewLevel
);

public record PlayerWarpedEventData(
    string PlayerName,
    string OldLocation,
    string NewLocation
);

public class DefaultEventProcessor : IEventProcessor
{
    public void Initialize()
    {
        SMAPIWrapper.Instance.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        SMAPIWrapper.Instance.Helper.Events.GameLoop.Saved += OnSaved;
        SMAPIWrapper.Instance.Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        SMAPIWrapper.Instance.Helper.Events.GameLoop.DayStarted += OnDayStarted;
        SMAPIWrapper.Instance.Helper.Events.GameLoop.DayEnding += OnDayEnding;
        SMAPIWrapper.Instance.Helper.Events.GameLoop.TimeChanged += OnTimeChanged;

        SMAPIWrapper.Instance.Helper.Events.Player.InventoryChanged += OnPlayerInventoryChanged;
        SMAPIWrapper.Instance.Helper.Events.Player.LevelChanged += OnPlayerLevelChanged;
        SMAPIWrapper.Instance.Helper.Events.Player.Warped += OnPlayerWarped;
    }

    public void InitializeGameData() { }

    // Nothing to process here because these are all SMAPI event handlers
    public void ProcessEvents() { }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("SaveLoaded", new SaveLoadedEventData
        (
            Game1.getFarm().DisplayName,
            Game1.player.Name
        ));
    }

    private void OnSaved(object? sender, SavedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("Saved", new SavedEventData
        (
            Game1.getFarm().DisplayName,
            Game1.player.Name
        ));
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        WebServer.Instance.SendGameEvent("ReturnedToTitle");
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("DayStarted", new DayStartedEventData
        (
            DayInfo.Today
        ));
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        WebServer.Instance.SendGameEvent("DayEnding", new DayEndingEventData
        (
            DayInfo.Today
        ));
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("TimeChanged", new TimeChangedEventData
        (
            e.OldTime,
            e.NewTime
        ));
    }

    private void OnPlayerInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("PlayerInventoryChanged", new PlayerInventoryChangedEventData(e));
    }

    private void OnPlayerLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("PlayerLevelChanged", new PlayerLevelChangedEventData
        (
            e.Player.Name,
            e.Skill.ToString(),
            e.OldLevel,
            e.NewLevel
        ));
    }

    private void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        WebServer.Instance.SendGameEvent("PlayerWarped", new PlayerWarpedEventData
        (
            e.Player.Name,
            e.OldLocation.Name,
            e.NewLocation.Name
        ));
    }
}