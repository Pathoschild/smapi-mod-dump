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
using Microsoft.Xna.Framework;

namespace Omegasis.Revitalize.Framework.Configs.ObjectsConfigs
{
    public class HayMakerConfig
    {
        public int NumberOfCornRequired = 1;
        public int NumberOfWheatRequired = 1;
        public int NumberOfFiberRequired = 5;
        public int NumberOfAmaranthRequired = 1;

        public int CornToHayOutput = 3;
        public int WheatToHayOutput = 5;
        public int FiberToHayOutput = 1;
        public int AmaranthToHayOutput = 10;

        public int MinutesToProcess = 60;

        public HayMakerConfig()
        {

        }

        public static HayMakerConfig InitializeConfig()
        {
            if (File.Exists(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, "Configs", "ObjectConfigs", "HayMakerConfig.json")))
                return RevitalizeModCore.ModHelper.Data.ReadJsonFile<HayMakerConfig>(Path.Combine("Configs", "ObjectConfigs", "HayMakerConfig.json"));
            else
            {
                HayMakerConfig Config = new HayMakerConfig();
                RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine("Configs", "ObjectConfigs", "HayMakerConfig.json"), Config);
                return Config;
            }
        }


    }
}
