/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Goals;
using Slothsoft.Challenger.Models;

namespace Slothsoft.Challenger.Challenges;

public abstract class BaseChallenge : IChallenge {

    private IRestriction[]? _restrictions;
    private IGoal? _goal;

    protected BaseChallenge(IModHelper modHelper, string id) {
        ModHelper = modHelper;
        Id = id;
    }
    public string Id { get; }
    protected IModHelper ModHelper { get; }
    public string DisplayName => ModHelper.Translation.Get(GetType().Name);

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

    public void Start(Difficulty difficulty) {
        foreach (var restriction in GetOrCreateRestrictions(difficulty)) {
            restriction.Apply();
        }

        GetGoal().Start();
    }

    private IEnumerable<IRestriction> GetOrCreateRestrictions(Difficulty difficulty) {
        _restrictions ??= CreateRestrictions(ModHelper, difficulty);
        return _restrictions;
    }

    protected abstract IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty);

    public void Stop() {
        if (_restrictions != null) {
            foreach (var restriction in _restrictions) {
                restriction.Remove();
            }

            _restrictions = null;
        }
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
        return GetGoal().GetProgress(difficulty);
    }

    public bool WasStarted() {
        return GetGoal().WasStarted();
    }
}