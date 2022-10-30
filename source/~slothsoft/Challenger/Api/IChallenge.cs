/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

namespace Slothsoft.Challenger.Api;

public interface IChallenge {
    /// <summary>
    /// An ID that never ever changes, so we can be sure to identify challenges.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Returns the display name of this challenge.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// Returns the goal that should be reached.
    /// </summary>
    /// <returns>the goal.</returns>
    string GetGoalDisplayName(Difficulty difficulty);

    /// <summary>
    /// Returns a string explaining what you can or cannot do in this challenge.
    /// </summary>
    /// <returns></returns>
    string GetDisplayText(Difficulty difficulty);

    /// <summary>
    /// This applies all restriction this challenge has to the game and starts tracking the goal.
    /// </summary>
    void Start(Difficulty difficulty);

    /// <summary>
    /// This removes all restriction this challenge has from the game and stops tracking the goal.
    /// </summary>
    void Stop();

    /// <summary>
    /// Returns the object you wish to replace the magical object with.
    /// </summary>
    /// <returns>replacement object.</returns>
    MagicalReplacement GetMagicalReplacement(Difficulty difficulty);

    /// <summary>
    /// Returns if the challenge's goal is completed. 
    /// </summary>
    /// <returns></returns>
    bool IsCompleted(Difficulty difficulty);
    
    /// <summary>
    /// Returns if some progress was made in completing this challenge's goal.
    /// </summary>
    /// <returns></returns>
    /// 
    bool WasStarted();
    
    /// <summary>
    /// Returns a string describing the progress that was made in completing this challenge's goal.
    /// </summary>
    /// <returns></returns>
    string GetProgress(Difficulty difficulty);
}