/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace TehPers.CoreMod.Api.ContentPacks {
    public interface IContextSpecific {
        /// <summary>Whether this is valid in the given context.</summary>
        /// <param name="context">The context this will be used in.</param>
        /// <returns>True if valid in this context, false otherwise.</returns>
        bool IsValidInContext(IContext context);
    }
}