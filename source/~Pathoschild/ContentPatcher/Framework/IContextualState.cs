/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.Common.Utilities;

namespace ContentPatcher.Framework
{
    /// <summary>Diagnostic info about a contextual object.</summary>
    internal interface IContextualState
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether the instance is valid in general (ignoring the context).</summary>
        bool IsValid { get; }

        /// <summary>Whether <see cref="IsValid"/> and the instance is applicable in the current context.</summary>
        bool IsInScope { get; }

        /// <summary>Whether <see cref="IsInScope"/> and there are no issues preventing the contextual from being used.</summary>
        bool IsReady { get; }

        /// <summary>The unknown tokens required by the instance, if any.</summary>
        IInvariantSet InvalidTokens { get; }

        /// <summary>The valid tokens required by the instance which aren't available in the current context, if any.</summary>
        IInvariantSet UnreadyTokens { get; }

        /// <summary>The tokens which are provided by a mod which isn't installed, if any.</summary>
        IInvariantSet UnavailableModTokens { get; }

        /// <summary>Error phrases indicating why the instance is not ready to use, if any.</summary>
        IInvariantSet Errors { get; }
    }
}
