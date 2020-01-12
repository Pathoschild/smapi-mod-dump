using StardewModdingAPI;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>Class that loads Content for the game</summary>
    public class ContentLoader : IAssetLoader
    {
        //Instance of ModEntry
        ModEntry Entry;

        //Instance of ContentPackHelper
        ContentPackHelper PackHelper;

        /// <summary>ContentLoader's Constructor</summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        public ContentLoader(ModEntry entry, ContentPackHelper packHelper)
        {
            //Set the fields
            Entry = entry;
            PackHelper = packHelper;
        }

        /// <summary>Whether SMAPI can load the asset</summary>
        /// <typeparam name="T">The type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>Wether it can load the asset</returns>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
                return true;
            if (asset.AssetName.StartsWith("GetGlam_"))
                return true;

            return false;
        }

        /// <summary>Whether SMAPI can load a given asset</summary>
        /// <typeparam name="T">The assets type</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>The specific asset to load else default</returns>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
                return Entry.Helper.Content.Load<T>(Path.Combine("assets", "dresser.png"));
            if (asset.AssetName.StartsWith("GetGlam_"))
                return (T)(object)PackHelper.LoadPlayerBase();

            return default;
        }
    }
}
