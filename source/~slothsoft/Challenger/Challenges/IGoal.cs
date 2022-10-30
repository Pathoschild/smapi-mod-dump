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

namespace Slothsoft.Challenger.Challenges; 

public interface IGoal {
    /// <summary>
    /// Returns the display name of this goal.
    /// </summary>
    /// <returns></returns>
    string GetDisplayName(Difficulty difficulty);
    
    /// <summary>
    /// Returns if some progress was made in completing this goal.
    /// </summary>
    /// <returns></returns>
    bool WasStarted();
    /// <summary>
    /// Returns a string describing the progress that was made in completing this goal.
    /// </summary>
    /// <returns></returns>
    string GetProgress(Difficulty difficulty);
    /// <summary>
    /// Returns if this goal was reached.
    /// </summary>
    /// <returns></returns>
    bool IsCompleted(Difficulty difficulty);
    /// <summary>
    /// This starts tracking the goal.
    /// </summary>
    void Start();
    /// <summary>
    /// This stops tracking the goal.
    /// </summary>
    void Stop();
}