/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Items
{
    /// <inheritdoc cref="INamespaceRegistry"/>
    public class NamespaceRegistry : INamespaceRegistry
    {
        private readonly IMonitor monitor;
        private readonly Dictionary<string, INamespaceProvider> namespaceProviders;

        private bool reloadRequested;

        public NamespaceRegistry(
            IMonitor monitor,
            IEnumerable<INamespaceProvider> namespaceProviders
        )
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceProviders = namespaceProviders.ToDictionary(provider => provider.Name);

            monitor.Log($"Loaded {this.namespaceProviders.Count} namespaces:", LogLevel.Info);
            foreach (var name in this.namespaceProviders.Keys)
            {
                monitor.Log($" - {name}", LogLevel.Info);
            }
        }

        public IEnumerable<string> GetRegisteredNamespaces()
        {
            this.ReloadIfNeeded();

            return this.namespaceProviders.Keys;
        }

        public bool TryGetItemFactory(
            NamespacedKey key,
            [NotNullWhen(true)] out IItemFactory? factory
        )
        {
            this.ReloadIfNeeded();

            factory = default;
            return this.namespaceProviders.TryGetValue(key.Namespace, out var provider)
                && provider.TryGetItemFactory(key.Key, out factory);
        }

        public IEnumerable<NamespacedKey> GetKnownItemKeys()
        {
            this.ReloadIfNeeded();

            return this.namespaceProviders.Values.SelectMany(
                provider => provider.GetKnownItemKeys(),
                (provider, itemKey) => new NamespacedKey(provider.Name, itemKey)
            );
        }

        public void RequestReload()
        {
            this.reloadRequested = true;
        }

        private void ReloadIfNeeded()
        {
            // Only reload if requested
            if (!this.reloadRequested)
            {
                return;
            }

            this.reloadRequested = false;

            // Reload each namespace provider
            foreach (var provider in this.namespaceProviders.Values)
            {
                provider.Reload();
            }

            // Notify listeners
            this.OnReload?.Invoke(this, EventArgs.Empty);

            this.monitor.Log("Namespaces reloaded.", LogLevel.Info);
            this.monitor.Log(
                $"There are {this.GetKnownItemKeys().Count()} known item keys.",
                LogLevel.Info
            );
        }

        public event EventHandler? OnReload;
    }
}