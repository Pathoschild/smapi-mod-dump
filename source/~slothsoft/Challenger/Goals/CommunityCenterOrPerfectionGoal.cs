/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Challenges;

namespace Slothsoft.Challenger.Goals; 

/// <summary>
/// A goal that is "Community Center" for the easier difficulties
/// and "Perfection" else.
/// </summary>
public class CommunityCenterOrPerfectionGoal : IGoal {

    private readonly IGoal _communityCenterGoal;
    private readonly IGoal _perfectionGoal;
    
    public CommunityCenterOrPerfectionGoal(IModHelper modHelper) {
        _communityCenterGoal = new CommunityCenterGoal(modHelper);
        _perfectionGoal = new PerfectionGoal(modHelper);
    }

    public string GetDisplayName(Difficulty difficulty) {
        return GetGoal(difficulty).GetDisplayName(difficulty);
    }
    
    private IGoal GetGoal(Difficulty difficulty) {
        return difficulty == Difficulty.Hard ? _perfectionGoal : _communityCenterGoal;
    }
    
    public bool IsCompleted(Difficulty difficulty) {
        return GetGoal(difficulty).IsCompleted(difficulty);
    }

    public void Start() {
        // we don't need to start this
    }

    public void Stop() {
        // we don't need to stop this
    }

    public bool WasStarted() {
        return _communityCenterGoal.WasStarted();
    }

    public string GetProgress(Difficulty difficulty) {
        return GetGoal(difficulty).GetProgress(difficulty);
    }
}