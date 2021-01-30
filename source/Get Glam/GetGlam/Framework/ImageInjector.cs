/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.ContentEditors;
using StardewModdingAPI;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class used to inject new textures into the games content.
    /// </summary>
    public class ImageInjector : IAssetEditor
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        /// <summary>
        /// Image Injectors Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        public ImageInjector(ModEntry entry, ContentPackHelper packHelper)
        {
            Entry = entry;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Whether SMAPI's Asset Editor can edit a specific asset.
        /// </summary>
        /// <typeparam name="T">The Type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns>Whether it can load the specific asset</returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters\\Farmer\\hairstyles"))
                return true;
            else if (asset.AssetNameEquals("Characters\\Farmer\\accessories"))
                return true;
            else if (asset.AssetNameEquals("Characters\\Farmer\\skinColors"))
                return true;
            else if (asset.AssetNameEquals($"Mods/{Entry.ModManifest.UniqueID}/dresser.png"))
                return true;

            return false;
        }

        /// <summary>
        /// SMAPI's Asset Editor tries to edit a specific sset.
        /// </summary>
        /// <typeparam name="T">The Type of Asset</typeparam>
        /// <param name="asset">The Asset in question</param>
        public void Edit<T>(IAssetData asset)
        {
            EditHair(asset);
            EditAccessory(asset);
            EditSkin(asset);
            EditDresser(asset);
        }

        /// <summary>
        /// Edits hair texture.
        /// </summary>
        /// <param name="asset">Current Asset</param>
        private void EditHair(IAssetData asset)
        {
            HairEditor hairEditor = new HairEditor(Entry, PackHelper, asset);
            hairEditor.EditHairTexture();
        }

        /// <summary>
        /// Edits accessory texture.
        /// </summary>
        /// <param name="asset">Current Asset</param>
        private void EditAccessory(IAssetData asset)
        {
            AccessoryEditor accessoryEditor = new AccessoryEditor(Entry, PackHelper, asset);
            accessoryEditor.EditAccessoryTexture();
        }

        /// <summary>
        /// Edits skin texture.
        /// </summary>
        /// <param name="asset">Current Asset</param>
        private void EditSkin(IAssetData asset)
        {
            SkinEditor skinEditor = new SkinEditor(Entry, PackHelper, asset);
            skinEditor.EditSkinTexture();
        }

        /// <summary>
        /// Edits dresser texture.
        /// </summary>
        /// <param name="asset">Current Asset</param>
        private void EditDresser(IAssetData asset)
        {
            DresserEditor dresserEditor = new DresserEditor(Entry, PackHelper, asset);
            dresserEditor.EditDresserTexture();
        }

        /// <summary>
        /// Get the number of accessories, used in AccessoryPatch.
        /// </summary>
        /// <returns>The number of Accessories</returns>
        public static int GetNumberOfAccessories()
        {
            return ContentPackHelper.NumberOfAccessoriesAdded;
        }

        /// <summary>
        /// Get the number of accessories minus one, used in AccessoryPatch.
        /// </summary>
        /// <returns>The number of Accessories minus one</returns>
        public static int GetNumberOfAccessoriesMinusOne()
        {
            return ContentPackHelper.NumberOfAccessoriesAdded - 1;
        }   
    }
}
