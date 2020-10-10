/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/SDVCustomChores
**
*************************************************/

using System;
using System.Collections.Generic;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores
{
    public interface IChore
    {
        ChoreData ChoreData { get; }

        /*********
        ** Public methods
        *********/
        /// <summary>Returns true if chore can be performed based on the current days conditions.</summary>
        /// <param name="today">Check eligibility based today's conditions.</param>
        bool CanDoIt(bool today = true);

        /// <summary>Performs the chore and returns true/false on success or failure.</summary>
        bool DoIt();

        /// <summary>Returns tokens for substitution.</summary>
        IDictionary<string, Func<string>> GetTokens();
    }
}
