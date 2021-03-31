/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace ImJustMatt.Common.Integrations
{
    internal abstract class ModIntegration<T> where T : class
    {
        private readonly IModRegistry _modRegistry;
        private readonly string _modUniqueId;
        private T _api;

        internal ModIntegration(IModRegistry modRegistry, string modUniqueId)
        {
            _modRegistry = modRegistry;
            _modUniqueId = modUniqueId;
        }

        protected internal T API => _api ??= _modRegistry.GetApi<T>(_modUniqueId);
        protected internal bool IsLoaded => _modRegistry.IsLoaded(_modUniqueId);
    }
}