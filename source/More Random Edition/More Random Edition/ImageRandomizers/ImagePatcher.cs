/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    public abstract class ImagePatcher
    {
        /// <summary>
        /// The image width in px - used when determining whether to crop and when drawing the image itself
        /// </summary>
        protected int ImageWidthInPx = 16;

        /// <summary>
        /// The image height in px - used when determining whether to crop and when drawing the image itself
        /// </summary>
        protected int ImageHeightInPx = 16;

        /// <summary>
        /// The name of the output file, should the setting save them for debugging purposes
        /// </summary>
        protected const string OutputFileName = "randomizedImage.png";

        /// <summary>
        /// The path to the output file
        /// </summary>
        public string OutputFileFullPath
        {
            get
            {
                return Globals.GetFilePath(Path.Combine(PatcherImageFolder, OutputFileName));
            }
        }

        /// <summary>
        /// The assets folder name
        /// </summary>
        protected const string AssetsFolder = "assets";

        /// <summary>
        /// The sub folder to use as the root for this patcher - located after Assets
        /// </summary>
        protected string SubFolder { get; set; }

        /// <summary>
        /// The folder to use for this patcher - equivalent to
        /// assets/<SubFolder>
        /// </summary>
        protected string PatcherImageFolder => Path.Combine(AssetsFolder, SubFolder);

        /// <summary>
        /// Called when the asset is requested
        /// This is where the work should be done to modify the image
        /// </summary>
        /// <param name="asset">Stardew's asset to be replaced</param>
        abstract public void OnAssetRequested(IAssetData asset);

        /// <summary>
        /// Wrties the randomized image to the filesystem if the setting permits
        /// </summary>
        protected void TryWriteRandomizedImage(Texture2D image)
        {
            if (Globals.Config.SaveRandomizedImages)
            {
                using FileStream fileStream = File.OpenWrite(OutputFileFullPath);
                image.SaveAsPng(fileStream, image.Width, image.Height);
            }
        }

        /// <summary>
        /// Gets a list of file names in the patcher's folder
        /// Igmores the output file name
        /// </summary>
        /// <param name="subfolder">Any subfolder you with to search through instead - set to nothing by default</param>
        /// <param name="getFullPath">Whether to get the full path of the filename - false by default</param>
        /// <returns>The list of file names</returns>
        protected List<string> GetAllFileNamesInFolder(string subfolder = "", bool getFullPath = false)
        {
            var pathToSearch = subfolder == ""
                ? PatcherImageFolder
                : Path.Combine(PatcherImageFolder, subfolder);

            return Directory.GetFiles(Globals.GetFilePath(pathToSearch))
                .Where(x => x.EndsWith(".png") && !x.EndsWith(OutputFileName))
                .Select(x => getFullPath ? x : Path.GetFileName(x))
                .OrderBy(x => x)
                .ToList();
        }
    }
}