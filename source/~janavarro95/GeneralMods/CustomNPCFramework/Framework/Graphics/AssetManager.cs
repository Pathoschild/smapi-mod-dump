using System.Collections.Generic;
using System.IO;
using CustomNPCFramework.Framework.Enums;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>Used to hold assets from specified directories.</summary>
    public class AssetManager
    {
        /// <summary>A list of all of the assets held by this asset manager.</summary>
        public List<AssetSheet> assets { get; } = new List<AssetSheet>();

        /// <summary>A list of directories managed by this asset manager, relative to the mod folder.</summary>
        public Dictionary<string, string> relativePaths { get; } = new Dictionary<string, string>();

        /// <summary>Default loading function from hardcoded paths.</summary>
        public void loadAssets()
        {
            foreach (var relativePath in this.relativePaths)
                this.ProcessDirectory(relativePath.Value);
        }

        /// <summary>Process all .json files in the given directory. If there are more nested directories, keep digging to find more .json files. Also allows us to specify a broader directory like Content/Grahphics/ModularNPC/Hair to have multiple hair styles.</summary>
        /// <param name="relativeDirPath">The relative directory path to process.</param>
        /// <remarks>Taken from Microsoft c# documented webpages.</remarks>
        private void ProcessDirectory(string relativeDirPath)
        {
            DirectoryInfo root = new DirectoryInfo(Path.Combine(Class1.ModHelper.DirectoryPath, relativeDirPath));
            foreach (FileInfo file in root.GetFiles("*.json"))
                this.ProcessFile(Path.Combine(relativeDirPath, file.Name), relativeDirPath);

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo subdir in root.GetDirectories())
                this.ProcessDirectory(Path.Combine(relativeDirPath, subdir.Name));
        }

        /// <summary>Actually load in the asset information.</summary>
        /// <param name="relativeFilePath">The relative path to the file to process.</param>
        /// <param name="relativeDirPath">The relative path containing the file.</param>
        private void ProcessFile(string relativeFilePath, string relativeDirPath)
        {
            try
            {
                ExtendedAssetInfo info = ExtendedAssetInfo.readFromJson(relativeFilePath);
                AssetSheet sheet = new AssetSheet(info, relativeDirPath);
                this.addAsset(sheet);
                Class1.ModMonitor.Log("Loaded in new modular asset: " + info.assetName + " asset type: " + info.type);
            }
            catch
            {
                AssetInfo info = AssetInfo.readFromJson(relativeFilePath);
                AssetSheet sheet = new AssetSheet(info, relativeDirPath);
                this.addAsset(sheet);
            }
        }

        /// <summary>Add an asset to be handled from the asset manager.</summary>
        /// <param name="asset">The asset sheet.</param>
        public void addAsset(AssetSheet asset)
        {
            this.assets.Add(asset);
        }

        /// <summary>Get an individual asset by its name.</summary>
        /// <param name="s">The asset name.</param>
        public AssetSheet getAssetByName(string s)
        {
            foreach (var v in this.assets)
            {
                if (v.assetInfo.assetName == s)
                    return v;
            }
            return null;
        }

        /// <summary>Add a new path to the asset manager and create the directory for it.</summary>
        /// <param name="path">The absolute path to add.</param>
        public void addPathCreateDirectory(KeyValuePair<string, string> path)
        {
            this.addPath(path);
            string dir = Path.Combine(Class1.ModHelper.DirectoryPath, path.Value);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(Path.Combine(Class1.ModHelper.DirectoryPath, path.Value));
        }

        /// <summary>Add a path to the dictionary.</summary>
        /// <param name="path">The relative path to add.</param>
        private void addPath(KeyValuePair<string, string> path)
        {
            this.relativePaths.Add(path.Key, path.Value);
        }

        /// <summary>Create appropriate directories for the path.</summary>
        private void createDirectoriesFromPaths()
        {
            foreach (var v in this.relativePaths)
                Directory.CreateDirectory(Path.Combine(Class1.ModHelper.DirectoryPath, v.Value));
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="gender">The gender to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    if (info.gender == gender)
                        sheets.Add(v);
                }
            }
            return sheets;
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="type">The part type to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(PartType type)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    if (info.type == type)
                        sheets.Add(v);
                }
            }
            return sheets;
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="gender">The gender to match.</param>
        /// <param name="type">The part type to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender, PartType type)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    if (info.type == type && info.gender == gender)
                        sheets.Add(v);
                }
            }
            return sheets;
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="season">The season to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Seasons season)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    foreach (var sea in info.seasons)
                    {
                        if (sea == season)
                            sheets.Add(v);
                        break; //Only need to find first validation that this is a valid asset.
                    }
                }
            }
            return sheets;
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="gender">The gender to match.</param>
        /// <param name="season">The season to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender, Seasons season)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    foreach (var sea in info.seasons)
                    {
                        if (sea == season && info.gender == gender)
                            sheets.Add(v);
                        break; //Only need to find first validation that this is a valid asset.
                    }
                }
            }
            return sheets;
        }

        /// <summary>Get a list of assets which match the given critera.</summary>
        /// <param name="gender">The gender to match.</param>
        /// <param name="season">The season to match.</param>
        /// <param name="type">The part type to match.</param>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender, Seasons season, PartType type)
        {
            List<AssetSheet> sheets = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo info)
                {
                    foreach (var sea in info.seasons)
                    {
                        if (sea == season && info.gender == gender && info.type == type)
                            sheets.Add(v);
                    }
                }
            }
            return sheets;
        }
    }
}
