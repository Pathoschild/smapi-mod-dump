/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Bookcase.Registration {

    /// <summary>
    /// A registry object that is used to hold IIdentifiable objects.
    /// </summary>
    /// <typeparam name="T">The type of object held by this registry.</typeparam>
    public class Registry<T> where T : IIdentifiable {

        /// <summary>
        /// The contents of the registry. This should not be accessed directly.
        /// </summary>
        internal readonly Dictionary<Identifier, T> contents = new Dictionary<Identifier, T>();

        /// <summary>
        /// A list of all the register callbacks. These are fired when an object is registered.
        /// </summary>
        public List<Action<Identifier, T>> RegisterCallbacks = new List<Action<Identifier, T>>();

        /// <summary>
        /// Registers a new object. The ID used is taken from the object's Identifier field.
        /// </summary>
        /// <param name="value">The object to register.</param>
        public void Register(T value) {

            Register(value.Identifier, value);
        }

        /// <summary>
        /// Gets an object by it's string ID. This creates a new identifier object so it should be used sparingly.
        /// </summary>
        /// <param name="id">The id of the object to get.</param>
        /// <returns>The object if it exists, otherwise null.</returns>
        public T Get(String id) {

            return Get(new Identifier(id));
        }

        /// <summary>
        /// Gets an object by it's id. 
        /// </summary>
        /// <param name="id">The ID of the object you're looking for.</param>
        /// <returns>The object if it exists, otherwise null.</returns>
        public T Get(Identifier id) {

            return contents[id];
        }

        /// <summary>
        /// Checks if an ID exists in the registry. This creates a new identifier object so it should be used sparingly.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <returns>Whether or not the ID is in the registry.</returns>
        public bool HasKey(String id) {

            return HasKey(new Identifier(id));
        }

        /// <summary>
        /// Checks if an ID exists in the registry.
        /// </summary>
        /// <param name="id">The ID to look for.</param>
        /// <returns>Whether or not the ID is in the registry.</returns>
        public bool HasKey(Identifier id) {

            return contents.ContainsKey(id);
        }

        /// <summary>
        /// Registers a new object to the registry.
        /// </summary>
        /// <param name="key">The ID to register the object to.</param>
        /// <param name="value">The object to register.</param>
        public void Register(Identifier key, T value) {

            // Prevent null keys or values from being used.
            if (key == null || value == null) {

                throw new ArgumentException("Someone tried to register something with a null key or value. This is not allowed!");
            }

            // Duplicate keys are not allowed currently.
            if (contents.ContainsKey(key)) {

                throw new ArgumentException($"Someone tried to register with duplicate registry ID {key}.");
            }

            // Registers the entry.
            contents.Add(key, value);

            // Invoke all of the register callback hooks.
            foreach (Action<Identifier, T> callback in RegisterCallbacks) {

                callback.Invoke(key, value);
            }

            // If the identifier hasn't been set, set it for them.
            if (value.Identifier == null) {

                value.Identifier = key;
            }
        }
    }
}
