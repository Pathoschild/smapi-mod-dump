/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using StardewModdingAPI;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that loads Content for the game.
    /// </summary>
    public class ContentLoader : IAssetLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of PlayerChanger
        private PlayerChanger PlayerChanger;

        /// <summary>
        /// ContentLoader's Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of PlayerChanger</param>
        public ContentLoader(ModEntry entry, PlayerChanger changer)
        {
            // Set the fields
            Entry = entry;
            PlayerChanger = changer;
        }

        /// <summary>
        /// Whether SMAPI can load the asset.
        /// </summary>
        /// <typeparam name="T">The type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>Wether it can load the asset</returns>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png") || asset.AssetName.StartsWith("GetGlam_"))
                return true;

            return false;
        }

        /// <summary>
        /// Whether SMAPI can load a given asset.
        /// </summary>
        /// <typeparam name="T">The assets type</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>The specific asset to load else default</returns>
        public T Load<T>(IAssetInfo asset)
        {
            // If asset to edit is dresser, load the dresser image.
            if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
                return Entry.Helper.Content.Load<T>(Path.Combine("assets", "dresser.png"));

            // If asset is GetGlam then load the player base
            if (asset.AssetName.StartsWith("GetGlam_"))
                return (T)(object)PlayerChanger.LoadPlayerBase();

            return default;
        }
    }
}
