using CustomNPCFramework.Framework.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Graphics
{
    /// <summary>
    /// An expanded Asset info class that deals with seasons and genders.
    /// </summary>
    public class ExtendedAssetInfo :AssetInfo
    {
        /// <summary>
        /// The genders this part is associated with. 0=Male, 1=female, 2=other.
        /// </summary>
        public Genders gender;
        /// <summary>
        /// A list of seasons where this part can be displayed
        /// </summary>
        public List<Seasons> seasons=new List<Seasons>();
        /// <summary>
        /// The part type to be used for this asset such as hair, eyes, etc.
        /// </summary>
        public PartType type;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtendedAssetInfo()
        {
            this.seasons = new List<Seasons>();
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assetSize"></param>
        /// <param name="randomizeOnLoad"></param>
        /// <param name="Gender">The type of gender this asset will be associated with.</param>
        /// <param name="Season">The type of season this asset will be associated with.</param>
        public ExtendedAssetInfo(string name, NamePairings StandingAssetPaths, NamePairings MovingAssetPaths, NamePairings SwimmingAssetPaths, NamePairings SittingAssetPaths, Vector2 assetSize, bool randomizeOnLoad, Genders Gender, List<Seasons> Season, PartType Type): base(name,StandingAssetPaths,MovingAssetPaths,SwimmingAssetPaths,SittingAssetPaths, assetSize, randomizeOnLoad)
        {
            this.gender = Gender;
            this.seasons = Season;
            if (this.seasons == null) this.seasons = new List<Seasons>();
            this.type = Type;
        }

        /// <summary>
        /// Save the json to a certain location.
        /// </summary>
        /// <param name="path"></param>
        public new void writeToJson(string path)
        {
            Class1.ModHelper.WriteJsonFile<ExtendedAssetInfo>(path, this);
        }

        /// <summary>
        /// Read the json from a certain location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public new static ExtendedAssetInfo readFromJson(string path)
        {
            return Class1.ModHelper.ReadJsonFile<ExtendedAssetInfo>(path);
        }

    }
}
