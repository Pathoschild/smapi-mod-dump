/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGlam.Framework.ContentLoaders
{
    public class BaseLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the base files are stored
        private DirectoryInfo BaseDirectory;

        // Current content pack being looked at
        private IContentPack CurrentContentPack;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="contentPack"></param>
        /// <param name="packHelper"></param>
        public BaseLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            BaseDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Base"));
            Entry = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads a base from a Content Pack.
        /// </summary>
        public void LoadBase()
        {
            if (DoesBaseDirectoryExists())
            {
                try
                {
                    CheckForBaseFiles();
                }
                catch
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} bases is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Checks the directory for base files.
        /// </summary>
        private void CheckForBaseFiles()
        {
            foreach (FileInfo file in BaseDirectory.EnumerateFiles())
            {
                AddFemaleBaseToList(file);
                AddMaleBaseToList(file);
                AddFemaleBaldBaseToList(file);
                AddMaleBaldBaseToList(file);
            }
        }

        /// <summary>
        /// Checks if the base directory exists.
        /// </summary>
        /// <returns>Whether the base directory exists.</returns>
        private bool DoesBaseDirectoryExists()
        {
            return BaseDirectory.Exists;
        }

        /// <summary>
        /// Adds the female bases to the list.
        /// </summary>
        /// <param name="file">The current file.</param>
        private void AddFemaleBaseToList(FileInfo file)
        {
            if (file.Name.Contains("farmer_girl_base.png"))
                PackHelper.FemaleBaseTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>("Base/farmer_girl_base.png"));
        }

        /// <summary>
        /// Adds the male bases to the list.
        /// </summary>
        /// <param name="file">The current file.</param>
        private void AddMaleBaseToList(FileInfo file)
        {
            if (file.Name.Contains("farmer_base.png"))
                PackHelper.MaleBaseTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>("Base/farmer_base.png"));
        }

        /// <summary>
        /// Adds the female bald bases to the list.
        /// </summary>
        /// <param name="file">The current file.</param>
        private void AddFemaleBaldBaseToList(FileInfo file)
        {
            if (file.Name.Contains("girl_base_bald"))
                PackHelper.FemaleBaseBaldTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>("Base/farmer_girl_base_bald.png"));
        }

        /// <summary>
        /// Adds the male bald bases to the list.
        /// </summary>
        /// <param name="file">The current file.</param>
        private void AddMaleBaldBaseToList(FileInfo file)
        {
            if (file.Name.Contains("farmer_base_bald"))
                PackHelper.MaleBaseBaldTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>("Base/farmer_base_bald.png"));
        }
    }
}
