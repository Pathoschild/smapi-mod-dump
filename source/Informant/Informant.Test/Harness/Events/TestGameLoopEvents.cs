/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using StardewModdingAPI.Events;

namespace StardewTests.Harness.Events;

public class TestGameLoopEvents : IGameLoopEvents {
    public event EventHandler<GameLaunchedEventArgs>? GameLaunched;
    public event EventHandler<UpdateTickingEventArgs>? UpdateTicking;
    public event EventHandler<UpdateTickedEventArgs>? UpdateTicked;
    public event EventHandler<OneSecondUpdateTickingEventArgs>? OneSecondUpdateTicking;
    public event EventHandler<OneSecondUpdateTickedEventArgs>? OneSecondUpdateTicked;
    public event EventHandler<SaveCreatingEventArgs>? SaveCreating;
    public event EventHandler<SaveCreatedEventArgs>? SaveCreated;
    public event EventHandler<SavingEventArgs>? Saving;
    public event EventHandler<SavedEventArgs>? Saved;
    public event EventHandler<SaveLoadedEventArgs>? SaveLoaded;
    public event EventHandler<DayStartedEventArgs>? DayStarted;
    public event EventHandler<DayEndingEventArgs>? DayEnding;
    public event EventHandler<TimeChangedEventArgs>? TimeChanged;
    public event EventHandler<ReturnedToTitleEventArgs>? ReturnedToTitle;
}