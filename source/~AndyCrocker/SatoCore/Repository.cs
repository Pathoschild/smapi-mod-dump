/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using SatoCore.Attributes;
using SatoCore.Extensions;
using StardewModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SatoCore
{
    /// <summary>Represents a data model store.</summary>
    /// <typeparam name="T">The type of data models to store.</typeparam>
    /// <typeparam name="TIdentifier">The type of the property with the <see cref="IdentifierAttribute"/> in the model.</typeparam>
    public class Repository<T, TIdentifier> : IEnumerable<T>
        where T : class
    {
        /*********
        ** Fields
        *********/
        /// <summary>The monitor to use for logging.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The items stored in the repository.</summary>
        private readonly List<T> Items = new List<T>();


        /*********
        ** Accessors
        *********/
        /// <summary>Whether the repository is valid.</summary>
        public bool IsValid { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="monitor">The monitor to use for logging.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="monitor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/>'s identifier isn't valid or if <typeparamref name="T"/> has an invalid editable property.</exception>
        public Repository(IMonitor monitor)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));

            IsValid = ModelValidator.IsModelTypeValid<T>(Monitor);

            // ensure identifier is type TIdentifier
            var identifierProperty = typeof(T).GetIdentifierProperties().FirstOrDefault();
            if (identifierProperty?.PropertyType != typeof(TIdentifier))
            {
                Monitor.Log($"{nameof(TIdentifier)} ({typeof(TIdentifier).FullName}) doesn't match the identifier member ({identifierProperty.GetType().FullName})", LogLevel.Error);
                IsValid = false;
            }
        }

        /// <summary>Adds an item to the repository.</summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the repository is invalid.</exception>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!IsValid)
                throw new InvalidOperationException("Repository is invalid");

            // ensure all required members are present
            if (!ValidateRequiredProperties(item))
                return;

            // ensure item doesn't already exist in the repository
            var identifier = GetIdentifier(item);
            if (identifier == null)
            {
                Monitor.Log($"Tried to add an item ({typeof(T).Name}) without specifying the identifier", LogLevel.Error);
                return;
            }

            if (Get(identifier) != null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{identifier}' already exists (trying to add)", LogLevel.Error);
                return;
            }

            // resolve all tokens
            foreach (var tokenProperty in typeof(T).GetTokenProperties())
            {
                var destinationProperty = typeof(T).GetInstanceProperties().First(property => property.Name == tokenProperty.GetCustomAttribute<TokenAttribute>().OutputPropertyName);
                destinationProperty.SetValue(item, Utilities.ResolveToken(tokenProperty.GetValue(item).ToString(), out var errorMessage));
                if (errorMessage != null)
                    Monitor.Log(errorMessage, LogLevel.Error);
            }

            // set all null values to default values
            foreach (var property in typeof(T).GetDefaultableProperties())
                if (property.GetValue(item) == null)
                    property.SetValue(item, property.GetCustomAttribute<DefaultValueAttribute>().DefaultValue);

            // add item
            Items.Add(item);
        }

        /// <summary>Edits an item.</summary>
        /// <param name="item">The item containing the identifier of the item to edit, and the new values of the item.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the repository is invalid.</exception>
        public void Edit(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!IsValid)
                throw new InvalidOperationException("Repository is invalid");

            // ensure the item exists in the repository
            var identifier = GetIdentifier(item);
            if (identifier == null)
            {
                Monitor.Log($"Tried to edit an item ({typeof(T).Name}) without specifying the identifier", LogLevel.Error);
                return;
            }

            var itemToEdit = Get(identifier);
            if (itemToEdit == null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{identifier}' doesn't exists (trying to edit)", LogLevel.Error);
                return;
            }

            // edit properties
            foreach (var property in typeof(T).GetEditableProperties())
                property.SetValue(itemToEdit, property.GetValue(item) ?? property.GetValue(itemToEdit));
        }

        /// <summary>Deletes an item.</summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the repository is invalid.</exception>
        public void Delete(TIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!IsValid)
                throw new InvalidOperationException("Repository is invalid");

            // ensure the item exists in the repository
            var itemToDelete = Get(id);
            if (itemToDelete == null)
            {
                Monitor.Log($"An item ({typeof(T).Name}) with the id: '{id}' doesn't exists (trying to delete)", LogLevel.Error);
                return;
            }

            // delete item
            Items.Remove(itemToDelete);
        }

        /// <summary>Retrieves an item by id.</summary>
        /// <param name="id">The id of the item to retrieve.</param>
        /// <returns>An item with the id of <paramref name="id"/>, if it exists; otherwise, <see langword="null"/>.</returns>
        public T Get(TIdentifier id)
        {
            if (typeof(TIdentifier) == typeof(string))
                return Items.FirstOrDefault(item => GetIdentifier(item).ToString().ToLower() == id.ToString().ToLower());
            else
                return Items.FirstOrDefault(item => GetIdentifier(item).Equals(id));
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();


        /*********
        ** Private Methods
        *********/
        /// <summary>Retrieves the identifier of an item.</summary>
        /// <param name="item">The item whose identifier should be retrieved.</param>
        /// <returns>The identitifer of <paramref name="item"/>.</returns>
        private TIdentifier GetIdentifier(T item) => (TIdentifier)typeof(T).GetIdentifierProperties().FirstOrDefault()?.GetValue(item);

        /// <summary>Checks if all required properties are valid.</summary>
        /// <param name="item">The item whose properties should be checked.</param>
        /// <returns><see langword="true"/>, if all the required properties are valid; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This includes the identifier property.</remarks>
        private bool ValidateRequiredProperties(T item)
        {
            // retrieve required properties
            var properties = typeof(T).GetRequiredProperties().ToList();
            properties.Add(typeof(T).GetIdentifierProperties().First());

            // check required properties
            var isValid = true;
            foreach (var property in properties)
                if ((property.PropertyType == typeof(string) && string.IsNullOrWhiteSpace((string)property.GetValue(item)))
                    || property.GetValue(item) == null)
                {
                    Monitor.Log($"Tried to add an item ({typeof(T).Name}) without specifying '{property.Name}'", LogLevel.Error);
                    isValid = false;
                }

            return isValid;
        }
    }
}
