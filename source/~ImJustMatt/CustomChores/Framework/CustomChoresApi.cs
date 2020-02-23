using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores.Framework
{
    public class CustomChoresApi : ICustomChoresApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>Adds new types of custom chore.</summary>
        private readonly ChoreBuilder _choreBuilders;

        /// <summary>Instances of custom chores created by content packs.</summary>
        private readonly IDictionary<string, IChore> _chores;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance</summary>
        /// <param name="choreBuilders">The custom chores.</param>
        /// /// <param name="chores"></param>
        internal CustomChoresApi(ChoreBuilder choreBuilders, IDictionary<string, IChore> chores)
        {
            _choreBuilders = choreBuilders;
            _chores = chores;
        }

        /// <summary>Adds new types of custom chore.</summary>
        /// <param name="factory">A factory which creates an instance of a chore type.</param>
        public void AddChoreFactory(IChoreFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            CustomChoresMod.Instance.Monitor.Log($"Adding chore factory: {factory.GetType().AssemblyQualifiedName}");
            _choreBuilders.AddChoreFactory(factory);
        }

        /// <summary>Get a list of chores.r</summary>
        /// <returns>List of chores available by name.</returns>
        public IList<string> GetChores() => _chores.Keys.ToList();

        /// <summary>Get a single chore.</summary>
        /// <returns>Single instance of chore object by name.</returns>
        public ChoreData GetChore(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return chore?.ChoreData;
        }

        /// <summary>Performs a chore.</summary>
        /// <returns>True if chore is successfully performed.</returns>
        public bool DoChore(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return !(chore is null) && chore.DoIt();
        }

        /// <summary>Checks if a chore can be done.</summary>
        /// <returns>True if current conditions allows chore to be done.</returns>
        public bool CheckChore(string choreName, bool today = true)
        {
            _chores.TryGetValue(choreName, out var chore);
            return !(chore is null) && chore.CanDoIt(today);
        }

        /// <summary>Gets chore tokens.</summary>
        /// <returns>Dictionary of chore tokens.</returns>
        public IDictionary<string, Func<string>> GetChoreTokens(string choreName)
        {
            _chores.TryGetValue(choreName, out var chore);
            return chore?.GetTokens();
        }
    }
}
