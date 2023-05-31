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

namespace Omegasis.Revitalize.Framework.SaveData.World
{
    public class WorldSaveDataManager
    {

        public BuildingSaveData buildingSaveData = new BuildingSaveData();

        public WorldSaveDataManager()
        {

        }

        public virtual void save()
        {
            this.buildingSaveData.save();
        }

        public virtual void load()
        {
            this.buildingSaveData = RevitalizeModCore.SaveDataManager.initializeSaveData<BuildingSaveData>(this.getRelativeSavePath(), BuildingSaveData.SaveFileName);
        }


        public virtual string getRelativeSavePath()
        {
            return Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "World");
        }

        public virtual string getFullSavePath()
        {
            return Path.Combine(RevitalizeModCore.SaveDataManager.getFullSaveDataPath(), "World");
        }
    }
}
