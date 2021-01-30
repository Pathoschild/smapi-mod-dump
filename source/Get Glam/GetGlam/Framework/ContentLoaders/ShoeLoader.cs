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
using System.IO;

namespace GetGlam.Framework.ContentLoaders
{
    public class ShoeLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the shoe files are stored
        private DirectoryInfo ShoeDirectory;

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
        public ShoeLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            ShoeDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Shoes"));
            Entry = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads shoes from a Content Pack.
        /// </summary>
        public void LoadShoes()
        {
            if (DoesShoeDirectoryExists())
            {
                try
                {
                    FindShoeFiles();
                }
                catch
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} shoes is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Whether the shoe directory exists.
        /// </summary>
        /// <returns>Whether the directory exists</returns>
        private bool DoesShoeDirectoryExists()
        {
            return ShoeDirectory.Exists;
        }

        /// <summary>
        /// Finds the shoe files.
        /// </summary>
        private void FindShoeFiles()
        {
            foreach (FileInfo file in ShoeDirectory.EnumerateFiles())
            {
                AddFemaleShoes(file);
                AddMaleShoes(file);
            }
        }

        /// <summary>
        /// Adds female shoes to the texture list.
        /// </summary>
        /// <param name="file">Current file</param>
        private void AddFemaleShoes(FileInfo file)
        {
            if (file.Name.Contains("female_shoes"))
                PackHelper.FemaleShoeTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>(Path.Combine("Shoes", file.Name)));
        }

        /// <summary>
        /// Adds male shoes to the texture list.
        /// </summary>
        /// <param name="file">Current file</param>
        private void AddMaleShoes(FileInfo file)
        {
            if (file.Name.Contains("male_shoes") && !file.Name.Contains("fe"))
                PackHelper.MaleShoeTextureList.Add(CurrentContentPack.LoadAsset<Texture2D>(Path.Combine("Shoes", file.Name)));
        }
    }
}
