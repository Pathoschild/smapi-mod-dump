using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores.Framework
{
    /// <summary>Add a new type of custom chore.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ChoreBuilder
    {
        /*********
        ** Fields
        *********/
        /// <summary>The factories which creates an instance of a chore type.</summary>
        private readonly IList<IChoreFactory> _choreFactories = new List<IChoreFactory>();

        /*********
        ** Public methods
        *********/
        /// <summary>Add a new type of custom chore.</summary>
        /// <param name="factory">A factory which creates an instance of a chore.</param>
        public void AddChoreFactory(IChoreFactory factory)
        {
            _choreFactories.Add(factory);
        }

        /// <summary>Returns an instance of a chore from the first factory that returns a non-null value.</summary>
        /// <param name="choreData">Chore content pack data.</param>
        public IChore GetChore(ChoreData choreData)
        {
            return _choreFactories.Select(choreFactory => choreFactory.GetChore(choreData))
                .FirstOrDefault(chore => chore != null);
        }

        /*********
        ** Private methods
        *********/
    }
}
