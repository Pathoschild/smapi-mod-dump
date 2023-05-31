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
using Omegasis.Revitalize.Framework.World.Buildings;

namespace Omegasis.Revitalize.Framework.SaveData.World
{
    public class BuildingSaveData:SaveDataInfo
    {

        public const string SaveFileName = "BuildingSaveData.json";
        /// <summary>
        /// The maximum items that can be stored in the dimensional storage unit.
        /// </summary>
        public long DimensionalStorageUnitMaxItems;
        public BuildingSaveData()
        {

        }

        public override void save()
        {
            RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(),"World", "BuildingSaveData.json"), this);
        }
    
    }
}
