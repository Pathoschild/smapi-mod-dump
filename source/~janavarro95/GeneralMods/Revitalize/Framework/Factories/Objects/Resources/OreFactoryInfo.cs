/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Revitalize.Framework.Factories.Objects.Furniture;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Objects.Resources.OreVeins;

namespace Revitalize.Framework.Factories.Objects.Resources
{
    /// <summary>
    /// Handles serialization of ore veins.
    /// </summary>
    public class OreFactoryInfo:FactoryInfo
    {
        /// <summary>
        /// The resource info for ore spawning.
        /// </summary>

        public OreResourceInformation OreSpawnInfo;
        /// <summary>
        /// Extra information that holds drop chances on extra drops.
        /// </summary>
        public List<ResourceInformation> ExtraDrops;
        /// <summary>
        /// The health of the ore vein.
        /// </summary>
        public int Health;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public OreFactoryInfo():base()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="obj"></param>
        public OreFactoryInfo(OreVeinObj obj):base(obj)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tile"></param>
        public OreFactoryInfo(OreVeinTile tile) : base(tile)
        {
            this.OreSpawnInfo = tile.resourceInfo;
            this.ExtraDrops = tile.extraDrops;
            this.Health = tile.healthValue;
        }

    }
}
