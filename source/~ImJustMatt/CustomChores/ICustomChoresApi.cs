using System;
using System.Collections.Generic;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores
{
    public interface ICustomChoresApi
    {
        /// <summary>Adds new types of custom chore.</summary>
        /// <param name="factory">A factory which creates an instance of a chore type.</param>
        void AddChoreFactory(IChoreFactory factory);

        /// <summary>Get a list of chores.r</summary>
        /// <returns>List of chores available by name.</returns>
        IList<string> GetChores();

        /// <summary>Get a single chore.</summary>
        /// <returns>Single instance of chore object by name.</returns>
        ChoreData GetChore(string choreName);

        /// <summary>Performs a chore.</summary>
        /// <returns>True if chore is successfully performed.</returns>
        bool DoChore(string choreName);

        /// <summary>Checks if a chore can be done.</summary>
        /// <returns>True if current conditions allows chore to be done.</returns>
        bool CheckChore(string choreName, bool today = true);

        /// <summary>Gets chore tokens.</summary>
        /// <returns>Dictionary of chore tokens.</returns>
        IDictionary<string, Func<string>> GetChoreTokens(string choreName);
    }
}
