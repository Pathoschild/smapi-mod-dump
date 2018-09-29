using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>
    /// A class to be used to hold information regarding assets such as the name of the assets and the paths to the images.
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// The name of the asset to be used in the main asset pool.
        /// </summary>
        public string assetName;
        /// <summary>
        /// The list of files to be used for the standing animation.
        /// </summary>
        public NamePairings standingAssetPaths;
        /// <summary>
        /// The list of files to be used for the swimming animation.
        /// </summary>
        public NamePairings swimmingAssetPaths;
        /// <summary>
        /// The list of files to be used with the moving animation.
        /// </summary>
        public NamePairings movingAssetPaths;
        /// <summary>
        /// The list of files to be used with the sitting animation.
        /// </summary>
        public NamePairings sittingAssetPaths;
        /// <summary>
        /// The size of the asset texture. Width and height.
        /// </summary>
        public Vector2 assetSize;
        /// <summary>
        /// Not really used anymore. More of a legacy feature.
        /// </summary>
        public bool randomizeUponLoad;
        
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public AssetInfo()
        {

        }

        /// <summary>
        /// Constructor that assigns values to the class.
        /// </summary>
        /// <param name="assetName">The name of the asset. This is the name that will be referenced in any asset manager or asset pool.</param>
        /// <param name="StandingAssetPaths">The name of the files to be used for the standing animation.</param>
        /// <param name="MovingAssetPaths">The name of the files to be used for the moving animation.</param>
        /// <param name="SwimmingAssetPaths">The name of the files to be used for the swimming animation.</param>
        /// <param name="SittingAssetPaths">The name of the files to be used for the sitting animation.</param>
        /// <param name="assetSize">The size of the asset. Width and height of the texture.</param>
        /// <param name="randomizeUponLoad">Legacy, not really used anymore.</param>
        public AssetInfo(string assetName,NamePairings StandingAssetPaths, NamePairings MovingAssetPaths, NamePairings SwimmingAssetPaths, NamePairings SittingAssetPaths, Vector2 assetSize, bool randomizeUponLoad)
        {
            this.assetName = assetName;
            this.sittingAssetPaths = SittingAssetPaths;
            this.standingAssetPaths = StandingAssetPaths;
            this.movingAssetPaths = MovingAssetPaths;
            this.swimmingAssetPaths = SwimmingAssetPaths;
            this.assetSize = assetSize;
            this.randomizeUponLoad = randomizeUponLoad;
        }

        /// <summary>
        /// Save the json to a certain location.
        /// </summary>
        /// <param name="path"></param>
        public void writeToJson(string path)
        {
            Class1.ModHelper.WriteJsonFile<AssetInfo>(path, this);
        }

        /// <summary>
        /// Read the json from a certain location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetInfo readFromJson(string path)
        {
           return Class1.ModHelper.ReadJsonFile<AssetInfo>(path);
        }
    }
}
