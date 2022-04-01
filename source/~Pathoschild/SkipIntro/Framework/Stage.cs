/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.SkipIntro.Framework
{
    /// <summary>A step in the mod logic.</summary>
    internal enum Stage
    {
        /// <summary>No action needed.</summary>
        None,

        /// <summary>Skip the initial intro.</summary>
        SkipIntro,

        /// <summary>Transition from the title screen to the load screen.</summary>
        TransitionToLoad,

        /// <summary>Start transitioning to the co-op section.</summary>
        StartTransitionToCoop,

        /// <summary>Finish transitioning from the title screen to the co-op section.</summary>
        TransitionToCoop,

        /// <summary>Transition from the co-op section to the host screen.</summary>
        TransitionToCoopHost
    }
}
