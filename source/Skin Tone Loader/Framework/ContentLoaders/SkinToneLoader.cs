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
using SkinToneLoader.Framework.DataModels;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinToneLoader.Framework.ContentLoaders
{
    public class SkinToneLoader
    {
        // Instance of ModEntry
        private ModEntry modEntryInstance;

        // Directory where the skin tone files are stored
        private DirectoryInfo SkinToneDirectory;

        // The model of the skin tone
        private SkinToneModel SkinTone;

        // Current content pack being looked at
        private IContentPack CurrentContentPack;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="contentPack">Current Content Pack</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        public SkinToneLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            SkinToneDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, entry.contentPackSkinToneFolder));
            modEntryInstance = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        public SkinToneLoader(ModEntry entry, ContentPackHelper packHelper)
        {
            modEntryInstance = entry;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads Skin Tone from a Content Pack.
        /// </summary>
        public void LoadSkinTone(IContentPack contentPack)
        {
            CurrentContentPack = contentPack;
            SkinToneDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, modEntryInstance.contentPackSkinToneFolder));

            if (DoesSkinToneDirectoryExists())
            {
                try
                {
                    CreateNewSkinModel();
                    SetSkinModelVariables();
                    AddSkinToneToList();
                }
                catch
                {
                    modEntryInstance.Monitor.Log($"{CurrentContentPack.Manifest.Name} skin tones is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Whether the Skin Tone Directory Exists.
        /// </summary>
        /// <returns></returns>
        private bool DoesSkinToneDirectoryExists()
        {
            return SkinToneDirectory.Exists;
        }

        /// <summary>
        /// Creates a new SkinToneModel and sets the Texture.
        /// </summary>
        private void CreateNewSkinModel()
        {
            SkinTone = new SkinToneModel();
            SkinTone.Texture = CurrentContentPack.ModContent.Load<Texture2D>(Path.Combine(modEntryInstance.contentPackSkinToneFolder, modEntryInstance.contentPackSkinTonePNGName));
        }

        /// <summary>
        /// Sets Texture Height and Mod Name for the Skin Tone Model.
        /// </summary>
        private void SetSkinModelVariables()
        {
            //SkinColor.TextureHeight = SkinColor.Texture.Height;
            SkinTone.ModName = CurrentContentPack.Manifest.Name;
        }

        /// <summary>
        /// Adds Skin Tone to list of added Skin Tones.
        /// </summary>
        private void AddSkinToneToList()
        {
            PackHelper.SkinToneList.Add(SkinTone);
        }
    }
}
