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

namespace Omegasis.Revitalize.Framework.SaveData.ShopConditionsSaveData
{
    /// <summary>
    /// Unified class for storing all of the necessary save data that needs to be persisted for shops.
    /// </summary>
    public class ShopSaveData : SaveDataBase
    {
        /// <summary>
        /// Deals with necessary save data regarding the animal shop.
        /// </summary>
        public AnimalShopSaveData animalShopSaveData;
        public ShopSaveData()
        {
            this.animalShopSaveData = AnimalShopSaveData.LoadOrCreate();
        }

        public override void save()
        {
            this.animalShopSaveData.save();
        }

        public override void load()
        {
            this.animalShopSaveData.load();
        }


    }
}
