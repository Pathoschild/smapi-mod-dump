/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace RangeDisplay.Framework.RangeHandling
{
    /// <summary>Listens for the mod registry to be ready.</summary>
    internal interface IModRegistryListener
    {
        /*********
        ** Methods
        *********/

        /// <summary>Called when the mod registry is ready.</summary>
        /// <param name="registry">The mod registry.</param>
        void ModRegistryReady(IModRegistry registry);
    }
}
