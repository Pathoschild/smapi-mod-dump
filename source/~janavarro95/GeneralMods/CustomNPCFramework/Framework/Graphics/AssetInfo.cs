using Microsoft.Xna.Framework;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>A class to be used to hold information regarding assets such as the name of the assets and the paths to the images.</summary>
    public class AssetInfo
    {
        /// <summary>The name of the asset to be used in the main asset pool.</summary>
        public string assetName;

        /// <summary>The list of files to be used for the standing animation.</summary>
        public NamePairings standingAssetPaths;

        /// <summary>The list of files to be used for the swimming animation.</summary>
        public NamePairings swimmingAssetPaths;

        /// <summary>The list of files to be used with the moving animation.</summary>
        public NamePairings movingAssetPaths;

        /// <summary>The list of files to be used with the sitting animation.</summary>
        public NamePairings sittingAssetPaths;

        /// <summary>The size of the asset texture. Width and height.</summary>
        public Vector2 assetSize;

        /// <summary>Not really used anymore. More of a legacy feature.</summary>
        public bool randomizeUponLoad;

        /// <summary>Construct an instance.</summary>
        public AssetInfo() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="assetName">The name of the asset. This is the name that will be referenced in any asset manager or asset pool.</param>
        /// <param name="standingAssetPaths">The name of the files to be used for the standing animation.</param>
        /// <param name="movingAssetPaths">The name of the files to be used for the moving animation.</param>
        /// <param name="swimmingAssetPaths">The name of the files to be used for the swimming animation.</param>
        /// <param name="sittingAssetPaths">The name of the files to be used for the sitting animation.</param>
        /// <param name="assetSize">The size of the asset. Width and height of the texture.</param>
        /// <param name="randomizeUponLoad">Legacy, not really used anymore.</param>
        public AssetInfo(string assetName, NamePairings standingAssetPaths, NamePairings movingAssetPaths, NamePairings swimmingAssetPaths, NamePairings sittingAssetPaths, Vector2 assetSize, bool randomizeUponLoad)
        {
            this.assetName = assetName;
            this.sittingAssetPaths = sittingAssetPaths;
            this.standingAssetPaths = standingAssetPaths;
            this.movingAssetPaths = movingAssetPaths;
            this.swimmingAssetPaths = swimmingAssetPaths;
            this.assetSize = assetSize;
            this.randomizeUponLoad = randomizeUponLoad;
        }

        /// <summary>Save the json to a certain location.</summary>
        /// <param name="relativeFilePath">The relative path to save.</param>
        public void writeToJson(string relativeFilePath)
        {
            Class1.ModHelper.Data.WriteJsonFile(relativeFilePath, this);
        }

        /// <summary>Read the json from a certain location.</summary>
        /// <param name="relativePath">The relative path to save.</param>
        public static AssetInfo readFromJson(string relativePath)
        {
            return Class1.ModHelper.Data.ReadJsonFile<AssetInfo>(relativePath);
        }
    }
}
