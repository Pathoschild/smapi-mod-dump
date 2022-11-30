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
using System.Collections.Generic;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Common;
using Slothsoft.Challenger.Goals;
using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Objects;
using StardewModdingAPI.Events;

namespace Slothsoft.Challenger.Challenges;

public abstract class BaseChallenge : IChallenge {

    private IRestriction[]? _restrictions;
    private IGoal? _goal;
    private ChallengeInfo? _info;
    private Difficulty? _currentDifficulty;

    protected BaseChallenge(IModHelper modHelper, string id) {
        ModHelper = modHelper;
        Id = id;
    }
    
    public string Id { get; }
    protected IModHelper ModHelper { get; }
    public string DisplayName => ModHelper.Translation.Get(GetType().Name);
    internal event EventHandler<EventArgs>? ProgressChanged;
    
    protected virtual ChallengeInfo ChallengeInfo {
        get {
            _info ??= Game1.netWorldState.Value.GetChallengerState().ChallengeInfos.GetOrRead(Id) ?? new ChallengeInfo();
            return _info;
        }
    }

    internal void ProgressChangedInvoked() {
        ProgressChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual string GetDisplayText(Difficulty difficulty) {
        var result = "";
        foreach (var restriction in CreateRestrictions(ModHelper, difficulty)) {
            result += restriction.GetDisplayText();
        }

        var magicalReplacementName = FetchMagicalReplacementName(difficulty);
        if (magicalReplacementName.Length > 0) {
            // if empty it's the default or completely broken
            result += CommonHelpers.ToListString(ModHelper.Translation.Get("BaseChallenge.MagicalObject",
                new {item = magicalReplacementName}).ToString());
        }
        return result;
    }

    private string FetchMagicalReplacementName(Difficulty difficulty) {
        var magicalReplacement = GetMagicalReplacement(difficulty);
        if (magicalReplacement != MagicalReplacement.Default) {
            Game1.bigCraftablesInformation.TryGetValue(magicalReplacement.ParentSheetIndex, out var info);
            if (info != null) {
                var split = info.Split('/');
                if (split.Length > 8) {
                    return split[8];
                }

                ChallengerMod.Instance.Monitor.Log(
                    $"BaseChallenge found info string of {magicalReplacement.ParentSheetIndex} with missing name: {info}",
                    LogLevel.Error);
            } else {
                ChallengerMod.Instance.Monitor.Log(
                    $"BaseChallenge could not find info string of {magicalReplacement.ParentSheetIndex}",
                    LogLevel.Error);
            }
        }
        return "";
    }

    public virtual void Start(Difficulty difficulty) {
        _currentDifficulty = difficulty;
        
        foreach (var restriction in GetOrCreateRestrictions(difficulty)) {
            restriction.Apply();
        }
        
        GetGoal().Start();
        ModHelper.Events.GameLoop.DayEnding += OnDayEnding;
        ProgressChanged += OnProgressChanged;
    }

    private IEnumerable<IRestriction> GetOrCreateRestrictions(Difficulty difficulty) {
        _restrictions ??= CreateRestrictions(ModHelper, difficulty);
        return _restrictions;
    }

    protected abstract IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty);

    private void OnDayEnding(object? sender, DayEndingEventArgs e) {
        // only when you get to the end of the day the challenge will start - so
        // you can switch it during the day or check other challenges etc.
        if (ChallengeInfo.StartedOn < 0) {
            ChallengeInfo.StartedOn = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth).TotalDays;
            Game1.netWorldState.Value.GetChallengerState().ChallengeInfos.Write(Id, ChallengeInfo);
        }
    }
    
    internal void OnProgressChanged(object? sender, EventArgs e) {
        if (_currentDifficulty == null) return; // shouldn't happen
        // check if we have now finished the the challenge
        var currentDifficulty = (int) _currentDifficulty!.Value;
        for (var difficulty = 0; difficulty <= currentDifficulty; difficulty++) {
            if (GetGoal().IsCompleted((Difficulty) difficulty)) {
                if (!ChallengeInfo.CompletedOn.ContainsKey(difficulty)) {
                    // we have finished the challenge for the first time, so mark the day
                    ChallengeInfo.SetCompletedOnDate((Difficulty) difficulty, new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth));
                    Game1.netWorldState.Value.GetChallengerState().ChallengeInfos.Write(Id, ChallengeInfo);
                }
                if (difficulty == currentDifficulty) {
                    ProgressChanged -= OnProgressChanged;
                }
            }
        }
    }
    
    public virtual void Stop() {
        ModHelper.Events.GameLoop.DayEnding -= OnDayEnding;
        ProgressChanged -= OnProgressChanged;
        GetGoal().Stop();
        
        if (_restrictions != null) {
            foreach (var restriction in _restrictions) {
                restriction.Remove();
            }

            _restrictions = null;
        }
        _currentDifficulty = null;
    }

    public virtual MagicalReplacement GetMagicalReplacement(Difficulty difficulty) {
        return MagicalReplacement.Default;
    }

    public string GetGoalDisplayName(Difficulty difficulty) {
        return GetGoal().GetDisplayName(difficulty);
    }

    protected internal IGoal GetGoal() {
        _goal ??= CreateGoal(ModHelper);
        return _goal;
    }

    protected virtual IGoal CreateGoal(IModHelper modHelper) {
        return new PerfectionGoal(ModHelper);
    }

    public bool IsCompleted(Difficulty difficulty) {
        return GetGoal().IsCompleted(difficulty);
    }

    public string GetProgress(Difficulty difficulty) {
        var completedOnDate = ChallengeInfo.GetCompletedOnDate(difficulty);
        if (completedOnDate != null) {
            return ModHelper.Translation.Get("BaseChallenge.CompletedOn", new {
                Date = completedOnDate.Localize(),
            }).ToString();
        }
        return GetGoal().GetProgress(difficulty);
    }

    public bool WasStarted() {
        return GetGoal().WasStarted();
    }

    public WorldDate? GetCompletedDate(Difficulty difficulty) {
        return ChallengeInfo.GetCompletedOnDate(difficulty); 
    }
}