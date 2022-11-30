/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using Netcode;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Challenges;
using Slothsoft.Challenger.Common;
using Slothsoft.Challenger.Models;
using StardewModdingAPI.Events;
using StardewValley.Network;

namespace Slothsoft.Challenger.Goals;

public abstract class BaseGoal<TProgress> : IGoal
    where TProgress : class, INetObject<INetSerializable> {
    private string Id { get; }
    protected IModHelper ModHelper { get; }
    protected TProgress Progress { get; private set; }
    private readonly NetStringDictionary<TProgress, NetRef<TProgress>> _netProgresses;

    protected BaseGoal(IModHelper modHelper, string id, NetStringDictionary<TProgress, NetRef<TProgress>> netProgresses) {
        ModHelper = modHelper;
        Id = id;
        _netProgresses = netProgresses;
        Progress = ReadProgressType();
    }

    internal event EventHandler<TProgress> ProgressChanged;
    
    private TProgress ReadProgressType() {
        return _netProgresses.GetOrRead(Id) ?? Activator.CreateInstance<TProgress>();
    }

    protected void WriteProgressType(TProgress progress) {
        Progress = progress;
        _netProgresses.Write(Id, progress);
        ProgressChanged(this, progress);
    }

    public virtual string GetDisplayName(Difficulty difficulty) {
        return ModHelper.Translation.Get(GetType().Name);
    }

    public virtual void Start() {
        ModHelper.Events.GameLoop.DayEnding += OnDayEnding;
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e) {
        if (NetRefExtensions.IsHostPlayer()) {
            // the host player needs to store the data at the end of the day
            _netProgresses.Write(Id, Progress);
        }
    }

    public virtual void Stop() {
        ModHelper.Events.GameLoop.DayEnding -= OnDayEnding;
    }

    public abstract bool WasStarted();

    public abstract string GetProgress(Difficulty difficulty);

    public abstract bool IsCompleted(Difficulty difficulty);
}