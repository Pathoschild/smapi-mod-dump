/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using StardewValley;

namespace MoonShared.APIs
{
    public interface IXPDisplayApi
    {
        /// <summary>
        /// Registers a tool skill matcher, allowing XP Display to recognize new or modified tool-skill matches.
        /// </summary>
        /// <param name="matcher">A matcher, which for a given <see cref="Item"/> returns either a tuple with a valid <c>int SkillIndex</c> (for a vanilla skill), or with a valid <c>string SpaceCoreSkillName</c> (for a SpaceCore skill), or with <c>null</c> for both tuple values (for forcing an item to not be matched), or with <c>null</c> instead of the tuple (if the matcher does not care about this item).</param>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        void RegisterToolSkillMatcher(Func<Item, (int? SkillIndex, string? SpaceCoreSkillName)?> matcher);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
