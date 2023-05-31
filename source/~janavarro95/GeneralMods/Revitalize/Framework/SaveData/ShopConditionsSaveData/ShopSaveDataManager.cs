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
    /// <summary>
    /// Unified class for storing all of the necessary save data that needs to be persisted for shops.
    /// </summary>
    public class ShopSaveDataManager
    {
        /// <summary>
        /// Deals with necessary save data regarding the animal shop.
        /// </summary>
        public AnimalShopSaveData animalShopSaveData;

        /// <summary>
        /// Deals with necessary save data related to the carpenter's shop (aka Robin's shop).
        /// </summary>
        public CarpenterShopSaveData carpenterShopSaveData;

        public ShopSaveDataManager()
        {
        }

        public virtual void save()
        {
            this.animalShopSaveData.save();
            this.carpenterShopSaveData.save();
        }

        public virtual void load()
        {
            this.animalShopSaveData = RevitalizeModCore.SaveDataManager.initializeSaveData<AnimalShopSaveData>(this.getRelativeSavePath(),AnimalShopSaveData.SaveFileName);
            this.carpenterShopSaveData = RevitalizeModCore.SaveDataManager.initializeSaveData<CarpenterShopSaveData>(this.getRelativeSavePath(),CarpenterShopSaveData.SaveFileName);
        }


        public virtual string getRelativeSavePath()
        {
            return Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "ShopConditionsSaveData");
        }

        public virtual string getFullSavePath()
        {
            return Path.Combine(RevitalizeModCore.SaveDataManager.getFullSaveDataPath(), "ShopConditionsSaveData");
        }

    }
}
