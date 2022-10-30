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

public class NoGoal : IGoal {

    private readonly IModHelper _modHelper;
    
    public NoGoal(IModHelper modHelper) {
        _modHelper = modHelper;
    }

    public string GetDisplayName(Difficulty difficulty) {
        return _modHelper.Translation.Get(GetType().Name);
    }
    
    public bool IsCompleted(Difficulty difficulty) {
        return false;
    }

    public void Start() {
        // we don't need to start this
    }

    public void Stop() {
        // we don't need to stop this
    }

    public bool WasStarted() {
        return false;
    }

    public string GetProgress(Difficulty difficulty) {
        return "";
    }
}