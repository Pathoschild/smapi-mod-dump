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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MagicSkillCode.API
{
    public interface IApi
    {
        /*********
        ** Accessors
        *********/
        /// <summary>An event raised when the player casts the analyze spell.</summary>
        event EventHandler OnAnalyzeCast;


        /*********
        ** Methods
        *********/
        /// <summary>Cause the player to forget all learned spell and reset earned spell points to zero. This should only be used in very specialized cases like Skill Prestige.</summary>
        /// <param name="manifest">The mod calling the API.</param>
        void ResetProgress(IManifest manifest);

        /// <summary>Reduce the player's spell points by the given amount (or add spell points if the <paramref name="count"/> is negative).</summary>
        /// <param name="manifest">The mod calling the API.</param>
        /// <param name="count">The number of spell points.</param>
        void UseSpellPoints(IManifest manifest, int count);
    }
}


