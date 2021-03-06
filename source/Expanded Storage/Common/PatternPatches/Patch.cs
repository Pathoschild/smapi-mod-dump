/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using Harmony;
using StardewModdingAPI;

// ReSharper disable StaticMemberInGenericType
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace ImJustMatt.Common.PatternPatches
{
    internal abstract class Patch<T>
    {
        private protected static IMonitor Monitor;
        private protected static T Config;

        internal Patch(IMonitor monitor, T config)
        {
            Monitor = monitor;
            Config = config;
        }

        protected internal abstract void Apply(HarmonyInstance harmony);
    }
}