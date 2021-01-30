/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.DataModels;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace GetGlam.Framework.ContentLoaders
{
    public class SkinColorLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the skin color files are stored
        private DirectoryInfo SkinColorDirectory;

        // The model of the skin color
        private SkinColorModel SkinColor;

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
        public SkinColorLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            SkinColorDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "SkinColor"));
            Entry = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads Skin Color from a Content Pack.
        /// </summary>
        public void LoadSkinColor() 
        {
            if (DoesSkinColorDirectoryExists())
            {
                try
                {
                    CreateNewSkinModel();
                    SetSkinModelVariables();
                    AddSkinColorToList();
                }
                catch 
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} skin colors is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Whether the Skin Color Directory Exists.
        /// </summary>
        /// <returns></returns>
        private bool DoesSkinColorDirectoryExists()
        {
            return SkinColorDirectory.Exists;
        }

        /// <summary>
        /// Creates a new SkinColorModel and sets the Texture.
        /// </summary>
        private void CreateNewSkinModel()
        {
            SkinColor = new SkinColorModel();
            SkinColor.Texture = CurrentContentPack.LoadAsset<Texture2D>(Path.Combine("SkinColor", "skinColors.png"));
        }

        /// <summary>
        /// Sets Texture Height and Mod Name for the Skin Color Model.
        /// </summary>
        private void SetSkinModelVariables()
        {
            SkinColor.TextureHeight = SkinColor.Texture.Height;
            SkinColor.ModName = CurrentContentPack.Manifest.Name;
        }

        /// <summary>
        /// Adds Skin Color to list of added Skin Colors.
        /// </summary>
        private void AddSkinColorToList()
        {
            PackHelper.SkinColorList.Add(SkinColor);
        }
    }
}
