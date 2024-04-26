/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SkinToneLoader.Framework.ContentLoaders;
using SkinToneLoader.Framework.DataModels;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinToneLoader.Framework
{
    /// <summary>
    /// Class that handles loading Content Packs.
    /// </summary>
    public class ContentPackHelper
    {
        // Instance of ModEntry
        private ModEntry modEntryInstance;

        ContentLoaders.SkinToneLoader skinToneLoader;

        // List of Skin Color Models
        public List<SkinToneModel> SkinToneList = new List<SkinToneModel>();

        /// <summary>
        /// ContentPackHelpers Contructor.
        /// </summary>
        /// <param name="entry">An instance of <see cref="ModEntry"/></param>
        public ContentPackHelper(ModEntry entry)
        {
            // Set the var to the instance
            modEntryInstance = entry;
            skinToneLoader = new ContentLoaders.SkinToneLoader(entry, this);
        }

        /// <summary>
        /// Reads all the content packs for the mod.
        /// </summary>
        public void ReadContentPacks()
        {
            if(modEntryInstance.DoesModdedSkinToneDirectoryExists())
                File.Delete(Path.Combine(modEntryInstance.Helper.DirectoryPath, modEntryInstance.moddedSkinTonesFileName));

            // Loop through each content pack
            foreach (IContentPack contentPack in modEntryInstance.Helper.ContentPacks.GetOwned())
            {
                modEntryInstance.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Trace);
                LoadSkinColor(contentPack);
            }

            // Add ImageInjector to the Asset Editor to start patching the images
            //Entry.Helper.Content.AssetEditors.Add(new ImageInjector(Entry, this));
        }

       

        /// <summary>
        /// Load Skin Color from a Content Pack.
        /// </summary>
        /// <param name="contentPack">The Current Content Pack.</param>
        private void LoadSkinColor(IContentPack contentPack)
        {
            //SkinColorLoader skinColorLoader = new SkinColorLoader(Entry, contentPack, this);
            skinToneLoader.LoadSkinTone(contentPack);
        }
    }
}
