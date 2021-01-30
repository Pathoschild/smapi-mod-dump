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
    public class DresserLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Directory where the dresser files are stored
        private DirectoryInfo DresserDirectory;

        // The model of the dresser
        private DresserModel Dresser;

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
        public DresserLoader(ModEntry entry, IContentPack contentPack, ContentPackHelper packHelper)
        {
            DresserDirectory = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Dresser"));
            Entry = entry;
            CurrentContentPack = contentPack;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads a dresser from a Content Pack.
        /// </summary>
        public void LoadDresser()
        {
            if (DoesDresserDirectoryExists())
            {
                try
                {
                    CreateNewDresserModel();
                    SetDresserModelVariables();
                    AddToDresserList();
                }
                catch 
                {
                    Entry.Monitor.Log($"{CurrentContentPack.Manifest.Name} dressers is empty. This pack was not added.", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// Whether the dresser directory exists.
        /// </summary>
        /// <returns></returns>
        private bool DoesDresserDirectoryExists()
        {
            return DresserDirectory.Exists;
        }

        /// <summary>
        /// Creates a new dresser model.
        /// </summary>
        private void CreateNewDresserModel()
        {
            Dresser = new DresserModel();
            Dresser.Texture = CurrentContentPack.LoadAsset<Texture2D>("Dresser/dresser.png");
        }

        /// <summary>
        /// Sets variables for the dresser model.
        /// </summary>
        private void SetDresserModelVariables()
        {
            Dresser.TextureHeight = Dresser.Texture.Height;
            Dresser.ModName = CurrentContentPack.Manifest.Name;
            PackHelper.DresserTextureHeight += Dresser.TextureHeight;
        }

        /// <summary>
        /// Adds the dresser to the dresser list.
        /// </summary>
        private void AddToDresserList()
        {
            PackHelper.DresserList.Add(Dresser);
        }
    }
}
