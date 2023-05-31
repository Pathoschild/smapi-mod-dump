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

namespace Omegasis.Revitalize.Framework.SaveData.ShopConditionsSaveData
{
    public class ShopSaveDataInfo:SaveDataInfo
    {

        public override void save()
        {

        }

        public override void save(string SaveFileName)
        {
            RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(this.getRelativeSavePath(), SaveFileName), this);
        }

        public override void load()
        {

        }


        public virtual string getRelativeSavePath()
        {
            return RevitalizeModCore.SaveDataManager.shopSaveData.getRelativeSavePath();
        }

        public virtual string getFullSavePath()
        {
            return RevitalizeModCore.SaveDataManager.shopSaveData.getFullSavePath();
        }
    }
}
