using CustomNPCFramework.Framework.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>
    /// Used to hold assets from specified directories.
    /// </summary>
    public class AssetManager
    {
        /// <summary>
        /// A list of all of the assets held by this asset manager.
        /// </summary>
        public List<AssetSheet> assets;
        /// <summary>
        /// A list of all of the directories managed by this asset manager.
        /// </summary>
        public Dictionary<string,string> paths;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public AssetManager()
        {
            this.assets = new List<AssetSheet>();
            this.paths = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assetsPathsToLoadFrom">A list of all directories to be managed by the asset manager. Name, path is the key pair value.</param>
        public AssetManager(Dictionary<string,string> assetsPathsToLoadFrom)
        {
            this.assets = new List<AssetSheet>();
            this.paths = assetsPathsToLoadFrom;
        }

        /// <summary>
        /// Default loading function from hard coded paths.
        /// </summary>
        public void loadAssets()
        {
            foreach(var path in this.paths)
            {
                ProcessDirectory(path.Value);
            }
        }

        /// <summary>
        /// Taken from Microsoft c# documented webpages.
        /// Process all .json files in the given directory. If there are more nested directories, keep digging to find more .json files. Also allows us to specify a broader directory like Content/Grahphics/ModularNPC/Hair to have multiple hair styles.
        /// </summary>
        /// <param name="targetDirectory"></param>
        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] files = Directory.GetFiles(targetDirectory, "*.json");
            foreach (var file in files)
            {
                ProcessFile(file,targetDirectory);
            }
            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        /// <summary>
        /// Actually load in the asset information.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        private void ProcessFile(string file,string path)
        {
            try
            {
                ExtendedAssetInfo info = ExtendedAssetInfo.readFromJson(file);
                AssetSheet sheet = new AssetSheet(info, path);
                addAsset(sheet);
                Class1.ModMonitor.Log("Loaded in new modular asset: " + info.assetName + " asset type: " + info.type);
            }
            catch (Exception err)
            {
                AssetInfo info = AssetInfo.readFromJson(file);
                AssetSheet sheet = new AssetSheet(info, path);
                addAsset(sheet);
            }
        }

        /// <summary>
        /// Add an asset to be handled from the asset manager.
        /// </summary>
        /// <param name="asset"></param>
        public void addAsset(AssetSheet asset)
        {
            this.assets.Add(asset);
        }

        /// <summary>
        /// Get an individual asset by its name.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public AssetSheet getAssetByName(string s)
        {
            foreach(var v in assets)
            {
                if (v.assetInfo.assetName == s) return v;
            }
            return null;
        }
        
        /// <summary>
        /// Add a new path to the asset manager and create the directory for it.
        /// </summary>
        /// <param name="path"></param>
        public void addPathCreateDirectory(KeyValuePair<string,string> path)
        {
            this.addPath(path);
            string dir = Path.Combine(Class1.ModHelper.DirectoryPath, path.Value);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(Path.Combine(Class1.ModHelper.DirectoryPath, path.Value));
            }
        }

        /// <summary>
        /// Add a path to the dictionary.
        /// </summary>
        /// <param name="path"></param>
        private void addPath(KeyValuePair<string,string> path)
        {
            this.paths.Add(path.Key, path.Value);
        }

        /// <summary>
        /// Create appropriate directories for the path.
        /// </summary>
        private void createDirectoriesFromPaths()
        {
            foreach(var v in paths)
            {
                Directory.CreateDirectory(Path.Combine(Class1.ModHelper.DirectoryPath,v.Value));
            }
        }

        /// <summary>
        /// Returns a list of assets from this manager that match the given critera.
        /// </summary>
        /// <param name="gender">The criteria we are searching for this time is gender.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach(var v in this.assets)
            {
                if(v.assetInfo is ExtendedAssetInfo)
                {
                    if ((v.assetInfo as ExtendedAssetInfo).gender == gender) aSheet.Add(v);
                }
            }
            return aSheet;
        }

        /// <summary>
        /// Get a list of all the assets of this kind of part.
        /// </summary>
        /// <param name="type">The type of part to return a list of from this asset manager.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(PartType type)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo)
                {
                    if ((v.assetInfo as ExtendedAssetInfo).type == type) aSheet.Add(v);
                }
            }
            return aSheet;
        }

        /// <summary>
        /// Get a list of assets that match the critera.
        /// </summary>
        /// <param name="gender">The gender criteria for this asset, such as if this part belongs to either a female or male character.</param>
        /// <param name="type">The type of asset to return. Hair eyes, shoes, etc.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender,PartType type)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo)
                {
                    if ((v.assetInfo as ExtendedAssetInfo).type == type && (v.assetInfo as ExtendedAssetInfo).gender == gender) aSheet.Add(v);
                }
            }
            return aSheet;
        }

        /// <summary>
        /// Returns a list of assets from this manager that match the given critera.
        /// </summary>
        /// <param name="gender">The criteria we are searching for this time is gender.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Seasons season)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo)
                {
                    foreach (var sea in (v.assetInfo as ExtendedAssetInfo).seasons) {
                        if (sea == season) aSheet.Add(v);
                        break; //Only need to find first validation that this is a valid asset.
                    }
                }
            }
            return aSheet;
        }

        /// <summary>
        /// Get a list of assets that match this criteria of gender and seasons.
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="season"></param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender,Seasons season)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo)
                {
                    foreach (var sea in (v.assetInfo as ExtendedAssetInfo).seasons)
                    {
                        if (sea == season && (v.assetInfo as ExtendedAssetInfo).gender==gender) aSheet.Add(v);
                        break; //Only need to find first validation that this is a valid asset.
                    }
                }
            }
            return aSheet;
        }

        /// <summary>
        /// Get a list of asssets that match certain critera.
        /// </summary>
        /// <param name="gender">The gengder certain assets belong to, such as male or female.</param>
        /// <param name="season">The season that an asset can be displayed in. Good for seasonal assets.</param>
        /// <param name="type">The type of part to return a list of such as hair, eyes, or pants.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfAssetsThatMatchThisCriteria(Genders gender, Seasons season, PartType type)
        {
            List<AssetSheet> aSheet = new List<AssetSheet>();
            foreach (var v in this.assets)
            {
                if (v.assetInfo is ExtendedAssetInfo)
                {
                    foreach (var sea in (v.assetInfo as ExtendedAssetInfo).seasons)
                    {
                        //Class1.ModMonitor.Log("Searching: seasons");
                        if (sea == season && (v.assetInfo as ExtendedAssetInfo).gender == gender && (v.assetInfo as ExtendedAssetInfo).type == type)
                        {
                            aSheet.Add(v);
                        }
                        else
                        {
                            //Class1.ModMonitor.Log("Not what I was looking for.");
                        }
                    }
                }
            }
            //Class1.ModMonitor.Log("ok it's over: "+aSheet.Count.ToString());
            return aSheet;
        }


    }
}
