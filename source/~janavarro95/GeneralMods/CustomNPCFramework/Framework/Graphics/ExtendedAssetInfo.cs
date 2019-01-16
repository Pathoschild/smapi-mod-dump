using System.Collections.Generic;
using CustomNPCFramework.Framework.Enums;
using Microsoft.Xna.Framework;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>An expanded Asset info class that deals with seasons and genders.</summary>
    public class ExtendedAssetInfo : AssetInfo
    {
        /// <summary>The genders this part is associated with.</summary>
        public Genders gender;

        /// <summary>A list of seasons where this part can be displayed.</summary>
        public List<Seasons> seasons = new List<Seasons>();

        /// <summary>The part type to be used for this asset such as hair, eyes, etc.</summary>
        public PartType type;

        /// <summary>Construct an instance.</summary>
        public ExtendedAssetInfo() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the asset. This is the name that will be referenced in any asset manager or asset pool.</param>
        /// <param name="standingAssetPaths">The name of the files to be used for the standing animation.</param>
        /// <param name="movingAssetPaths">The name of the files to be used for the moving animation.</param>
        /// <param name="swimmingAssetPaths">The name of the files to be used for the swimming animation.</param>
        /// <param name="sittingAssetPaths">The name of the files to be used for the sitting animation.</param>
        /// <param name="assetSize">The size of the asset. Width and height of the texture.</param>
        /// <param name="randomizeOnLoad">Legacy, not really used anymore.</param>
        /// <param name="gender">The type of gender this asset will be associated with.</param>
        /// <param name="season">The type of season this asset will be associated with.</param>
        /// <param name="type">The part type to be used for this asset such as hair, eyes, etc.</param>
        public ExtendedAssetInfo(string name, NamePairings standingAssetPaths, NamePairings movingAssetPaths, NamePairings swimmingAssetPaths, NamePairings sittingAssetPaths, Vector2 assetSize, bool randomizeOnLoad, Genders gender, List<Seasons> season, PartType type)
            : base(name, standingAssetPaths, movingAssetPaths, swimmingAssetPaths, sittingAssetPaths, assetSize, randomizeOnLoad)
        {
            this.gender = gender;
            this.seasons = season ?? new List<Seasons>();
            this.type = type;
        }

        /// <summary>Save the json to a certain location.</summary>
        /// <param name="relativePath">The relative path to write.</param>
        public new void writeToJson(string relativePath)
        {
            Class1.ModHelper.Data.WriteJsonFile<ExtendedAssetInfo>(relativePath, this);
        }

        /// <summary>Read the json from a certain location.</summary>
        /// <param name="relativePath">The relative path to read.</param>
        public new static ExtendedAssetInfo readFromJson(string relativePath)
        {
            return Class1.ModHelper.Data.ReadJsonFile<ExtendedAssetInfo>(relativePath);
        }
    }
}
