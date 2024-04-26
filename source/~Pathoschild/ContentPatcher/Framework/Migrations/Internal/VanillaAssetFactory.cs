/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Force.DeepCloner;
using StardewValley;

namespace ContentPatcher.Framework.Migrations.Internal
{
    /// <summary>A factory which produces a copy of an asset's original data without mod edits applied.</summary>
    /// <typeparam name="T">The asset type.</typeparam>
    internal class VanillaAssetFactory<T>
        where T : class
    {
        /*********
        ** Fields
        *********/
        /// <summary>The loaded asset data.</summary>
        private readonly Lazy<T> Data;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="load">The asset name to load.</param>
        public VanillaAssetFactory(Func<LocalizedContentManager, T> load)
        {
            this.Data = new Lazy<T>(() => VanillaAssetFactory<T>.LoadVanillaData(load));
        }

        /// <summary>Get a fresh copy of the asset with no edits applied.</summary>
        [return: NotNull]
        public T GetFreshCopy()
        {
            return this.Data.Value.DeepClone();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Load the vanilla data for an asset without mod edits applied.</summary>
        /// <typeparam name="T">The asset data type.</typeparam>
        /// <param name="load">Load the asset from a content manager.</param>
        public static T LoadVanillaData<T>(Func<LocalizedContentManager, T> load)
        {
            using var content = new LocalizedContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
            return load(content);
        }
    }
}
