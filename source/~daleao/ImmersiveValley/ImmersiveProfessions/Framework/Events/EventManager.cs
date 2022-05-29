/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Common.Extensions.Reflection;
using Events;
using Events.Content;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Multiplayer;
using Events.Player;
using Extensions;
using Framework.Events.Ultimate;
using Framework.Events.TreasureHunt;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of events for modded professions.</summary>
internal static class EventManager
{
    private static readonly Dictionary<Profession, List<Type>> EventsByProfession = new()
    {
        {Profession.Aquarist, new() {typeof(HostFishPondDataRequestedEvent)}},
        {Profession.Brute, new() {typeof(BruteWarpedEvent)}},
        {Profession.Conservationist, new() {typeof(HostConservationismDayEndingEvent)}},
        {Profession.Desperado, new() {typeof(DesperadoUpdateTickedEvent)}},
        {Profession.Piper, new() {typeof(PiperWarpedEvent)}},
        {Profession.Prospector, new() {typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Scavenger, new() {typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Spelunker, new() {typeof(SpelunkerWarpedEvent)}}
    };

    private static readonly List<IEvent> _ManagedEvents = new();

    /// <summary>Construct an instance.</summary>
    internal static void Init(IModEvents modEvents)
    {
        Log.D("[EventManager]: Gathering events...");
        
        // instantiate event classes
        var events = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract &&
                        t.GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).All(c => c.GetParameters().Length == 0))
            .ToList();

#if RELEASE
        events = events.Where(t => !t.Name.StartsWith("Debug")).ToList();
#endif

        Log.D($"[EventManager]: Found {events.Count} event classes. Initializing events...");
        foreach (var e in events.Select(t => (IEvent) t.RequireConstructor().Invoke(Array.Empty<object>())))
            _ManagedEvents.Add(e);

        Log.D("[EventManager]: Done. Hooking event runners...");
        
        // hook event raisers
        Ultimate.Ultimate.Activated += RaiseUltimateActivatedEvents;
        Ultimate.Ultimate.Deactivated += RaiseUltimateDectivatedEvents;
        Ultimate.Ultimate.ChargeInitiated += RaiseUltimateChargeInitiatedEvents;
        Ultimate.Ultimate.ChargeIncreased += RaiseUltimateChargeIncreasedEvents;
        Ultimate.Ultimate.Emptied += RaiseUltimateEmptiedEvents;

        TreasureHunt.TreasureHunt.Started += RaiseTreasureHuntStartedEvents;
        TreasureHunt.TreasureHunt.Ended += RaiseTreasureHuntEndedEvents;

        modEvents.Content.AssetRequested += RaiseAssetRequestedEvents;
        modEvents.Display.RenderedActiveMenu += RaiseRenderedActiveMenuEvents;
        modEvents.Display.RenderedHud += RaiseRenderedHudEvents;
        modEvents.Display.RenderedWorld += RaiseRenderedWorldEvents;
        modEvents.Display.RenderingHud += RaiseRenderingHudEvents;
        modEvents.GameLoop.DayEnding += RaiseDayEndingEvents;
        modEvents.GameLoop.DayStarted += RaiseDayStartedEvents;
        modEvents.GameLoop.GameLaunched += RaiseGameLaunchedEvents;
        modEvents.GameLoop.ReturnedToTitle += RaiseReturnedToTitleEvents;
        modEvents.GameLoop.SaveLoaded += RaiseSaveLoadedEvents;
        modEvents.GameLoop.Saving += RaiseSavingEvents;
        modEvents.GameLoop.UpdateTicked += RaiseUpdateTickedEvents;
        modEvents.Input.ButtonsChanged += RaiseButtonsChangedEvents;
        modEvents.Input.CursorMoved += RaiseCursorMovedEvents;
        modEvents.Multiplayer.ModMessageReceived += RaiseModMessageReceivedEvents;
        modEvents.Multiplayer.PeerConnected += RaisePeerConnectedEvents;
        modEvents.Multiplayer.PeerDisconnected += RaisePeerDisconnectedEvents;
        modEvents.Player.LevelChanged += RaiseLevelChangedEvents;
        modEvents.Player.Warped += RaiseWarpedEvents;

        Log.D("[EventManager]: Event initialization complete.");

#if DEBUG
        EnableAllStartingWith("Debug");
#endif
    }

    internal static IList<IEvent> Events => _ManagedEvents.AsReadOnly();

    /// <summary>Enable the specified <see cref="IEvent" /> types.</summary>
    /// <param name="eventTypes">A collection of <see cref="IEvent" /> types.</param>
    internal static void Enable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                Log.W($"[EventManager]: {type.Name} is not a valid IEvent type.");
                continue;
            }

            var e = _ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.W($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Enable();
            Log.D($"[EventManager]: Enabled {type.Name}.");
        }
    }

    /// <summary>Disable events from the event listener.</summary>
    /// <param name="eventTypes">The event types to be disabled.</param>
    internal static void Disable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                Log.W($"[EventManager]: {type.Name} is not a valid IEvent type.");
                continue;
            }

            var e = _ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.W($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Disable();
            Log.D($"[EventManager]: Disabled {type.Name}.");
        }
    }

    /// <summary>Enable all events required by the local player's current professions.</summary>
    internal static void EnableAllForLocalPlayer()
    {
        Log.D($"[EventManager]: Searching for events required by farmer {Game1.player.Name}...");
        foreach (var professionIndex in Game1.player.professions)
            try
            {
                EnableAllForProfession((Profession) professionIndex);
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {professionIndex} will be ignored.");
            }

        Log.D("[EventManager] Enabling other events...");
        if (Context.IsMultiplayer)
        {
            Enable(typeof(ToggledUltimateModMessageReceivedEvent));
            if (Context.IsMainPlayer)
               Enable(typeof(HostPeerConnectedEvent), typeof(HostPeerDisconnectedEvent));
        }

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
            Enable(typeof(VerifyHudThemeWarpedEvent));

        Log.D("[EventManager]: Done enabling local player events.");
    }

    /// <summary>Disable all non-static events.</summary>
    internal static void DisableAllForLocalPlayer()
    {
        Log.D("[EventManager]:  local player events...");
        var eventsToRemove = _ManagedEvents
            .Where(e => !e.GetType().Name.SplitCamelCase().First().IsAnyOf("Static", "Debug"))
            .Select(e => e.GetType())
            .ToArray();
        Disable(eventsToRemove);
        Log.D("[EventManager]: Done disabling local player events.");
    }

    /// <summary>Enable all events required by the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal static void EnableAllForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && !Context.IsMainPlayer ||
            !EventsByProfession.TryGetValue(profession, out var events)) return;

        Log.D($"[EventManager]: Enabling events for {profession}...");
        Enable(events.ToArray());
    }

    /// <summary>Disable all events related to the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal static void DisableAllForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _)
            || !EventsByProfession.TryGetValue(profession, out var events)) return;

        if (profession == Profession.Spelunker) events.Add(typeof(SpelunkerUpdateTickedEvent));

        List<Type> except = new();
        if (profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger) ||
            profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector))
            except.Add(typeof(TrackerButtonsChangedEvent));

        Log.D($"[EventManager]: Disabling {profession} events...");
        Disable(events.Except(except).ToArray());
    }

    /// <summary>Enable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal static void EnableAllStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for '{prefix}' events to be enabled...");
        var toBeEnabled = _ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        Log.D($"Found {toBeEnabled.Length} events. Enabling...");
        Enable(toBeEnabled);
    }

    /// <summary>Disable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal static void DisableAllStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for '{prefix}' events to be disabled...");
        var toBeDisabled = _ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        Log.D($"Found {toBeDisabled.Length} events. Disabling...");
        Disable(toBeDisabled);
    }

    /// <summary>Get an event instance of the specified event type.</summary>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    internal static T Get<T>() where T : IEvent
    {
        return _ManagedEvents.OfType<T>().FirstOrDefault();
    }

    /// <summary>Try to get an event instance of the specified event type.</summary>
    /// <param name="got">The matched event, if any.</param>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    /// <returns>Returns <c>True</c> if a matching event was found, or <c>False</c> otherwise.</returns>
    internal static bool TryGet<T>(out T got) where T : IEvent
    {
        got = Get<T>();
        return got is not null;
    }

    /// <summary>Check if the specified event type is enabled.</summary>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    internal static bool IsEnabled<T>() where T : IEvent
    {
        return TryGet<T>(out var got) && got.IsEnabled;
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal static IEnumerable<IEvent> GetAllEnabled()
    {
        return _ManagedEvents.Where(e => e.IsEnabled);
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal static IEnumerable<IEvent> GetAllEnabledForScreen(int screenId)
    {
        return _ManagedEvents.Where(e => e.IsEnabledForScreen(screenId));
    }

    /// <summary>Add a new <see cref="IEvent"/> instance to the list of managed events.</summary>
    /// <param name="event">An <see cref="IEvent"/> instance.</param>
    internal static void Manage(IEvent @event)
    {
        _ManagedEvents.Add(@event);
    }

    #region event runners

    // treasure hunt events
    private static void RaiseTreasureHuntStartedEvents(object sender, ITreasureHuntStartedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<TreasureHuntStartedEvent>())
            @event.OnStarted(sender, e);
    }

    private static void RaiseTreasureHuntEndedEvents(object sender, ITreasureHuntEndedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<TreasureHuntEndedEvent>())
            @event.OnEnded(sender, e);
    }

    // ultimate events
    private static void RaiseUltimateActivatedEvents(object sender, IUltimateActivatedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UltimateActivatedEvent>())
            @event.OnActivated(sender, e);
    }

    private static void RaiseUltimateDectivatedEvents(object sender, IUltimateDeactivatedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UltimateDeactivatedEvent>())
            @event.OnDeactivated(sender, e);
    }

    private static void RaiseUltimateChargeInitiatedEvents(object sender, IUltimateChargeInitiatedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UltimateChargeInitiatedEvent>())
            @event.OnChargeInitiated(sender, e);
    }

    private static void RaiseUltimateChargeIncreasedEvents(object sender, IUltimateChargeIncreasedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UltimateChargeIncreasedEvent>())
            @event.OnChargeIncreased(sender, e);
    }

    private static void RaiseUltimateEmptiedEvents(object sender, IUltimateEmptiedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UltimateEmptiedEvent>())
            @event.OnEmptied(sender, e);
    }

    // content events
    private static void RaiseAssetRequestedEvents(object sender, AssetRequestedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<AssetRequestedEvent>())
            @event.OnAssetRequested(sender, e);
    }

    // display events
    private static void RaiseRenderedActiveMenuEvents(object sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<RenderedActiveMenuEvent>())
            @event.OnRenderedActiveMenu(sender, e);
    }

    private static void RaiseRenderedHudEvents(object sender, RenderedHudEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<RenderedHudEvent>())
            @event.OnRenderedHud(sender, e);
    }

    private static void RaiseRenderedWorldEvents(object sender, RenderedWorldEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<RenderedWorldEvent>())
            @event.OnRenderedWorld(sender, e);
    }

    private static void RaiseRenderingHudEvents(object sender, RenderingHudEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<RenderingHudEvent>())
            @event.OnRenderingHud(sender, e);
    }

    // game loop events
    private static void RaiseDayEndingEvents(object sender, DayEndingEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<DayEndingEvent>())
            @event.OnDayEnding(sender, e);
    }

    private static void RaiseDayStartedEvents(object sender, DayStartedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<DayStartedEvent>())
            @event.OnDayStarted(sender, e);
    }

    private static void RaiseGameLaunchedEvents(object sender, GameLaunchedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<GameLaunchedEvent>())
            @event.OnGameLaunched(sender, e);
    }

    private static void RaiseReturnedToTitleEvents(object sender, ReturnedToTitleEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<ReturnedToTitleEvent>())
            @event.OnReturnedToTitle(sender, e);
    }

    private static void RaiseSaveLoadedEvents(object sender, SaveLoadedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<SaveLoadedEvent>())
            @event.OnSaveLoaded(sender, e);
    }

    private static void RaiseSavingEvents(object sender, SavingEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<SavingEvent>())
            @event.OnSaving(sender, e);
    }

    private static void RaiseUpdateTickedEvents(object sender, UpdateTickedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<UpdateTickedEvent>())
            @event.OnUpdateTicked(sender, e);
    }

    // input events
    private static void RaiseButtonsChangedEvents(object sender, ButtonsChangedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<ButtonsChangedEvent>())
            @event.OnButtonsChanged(sender, e);
    }

    private static void RaiseCursorMovedEvents(object sender, CursorMovedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<CursorMovedEvent>())
            @event.OnCursorMoved(sender, e);
    }

    // multiplayer events
    private static void RaiseModMessageReceivedEvents(object sender, ModMessageReceivedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<ModMessageReceivedEvent>())
            @event.OnModMessageReceived(sender, e);
    }

    private static void RaisePeerConnectedEvents(object sender, PeerConnectedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<PeerConnectedEvent>())
            @event.OnPeerConnected(sender, e);
    }

    private static void RaisePeerDisconnectedEvents(object sender, PeerDisconnectedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<PeerDisconnectedEvent>())
            @event.OnPeerDisconnected(sender, e);
    }

    // player events
    private static void RaiseLevelChangedEvents(object sender, LevelChangedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<LevelChangedEvent>())
            @event.OnLevelChanged(sender, e);
    }

    private static void RaiseWarpedEvents(object sender, WarpedEventArgs e)
    {
        foreach (var @event in _ManagedEvents.OfType<WarpedEvent>())
            @event.OnWarped(sender, e);
    }

    #endregion event runners
}