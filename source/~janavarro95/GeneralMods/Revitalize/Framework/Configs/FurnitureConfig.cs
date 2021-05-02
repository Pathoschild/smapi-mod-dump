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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Framework.Configs
{
    /// <summary>
    /// Deals with config settings for furniture.
    /// </summary>
    public class FurnitureConfig
    {
        /// <summary>
        /// How many draw frames should happen between rotating a furniture piece.
        /// </summary>
        public int furnitureFrameRotationDelay;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FurnitureConfig()
        {
            this.furnitureFrameRotationDelay = 20;
        }

        /// <summary>
        /// Initializes the config for furniture.
        /// </summary>
        /// <returns></returns>
        public static FurnitureConfig InitializeConfig()
        {
            if (File.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Configs", "FurnitureConfig.json")))
                return ModCore.ModHelper.Data.ReadJsonFile<FurnitureConfig>(Path.Combine("Configs", "FurnitureConfig.json"));
            else
            {
                FurnitureConfig Config = new FurnitureConfig();
                ModCore.ModHelper.Data.WriteJsonFile<FurnitureConfig>(Path.Combine("Configs", "FurnitureConfig.json"), Config);
                return Config;
            }
        }

    }
}
