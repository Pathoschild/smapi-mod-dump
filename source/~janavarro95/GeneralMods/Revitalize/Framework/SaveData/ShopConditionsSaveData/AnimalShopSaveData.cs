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
using Newtonsoft.Json;

namespace Omegasis.Revitalize.Framework.SaveData.ShopConditionsSaveData
{
    public class AnimalShopSaveData : ShopSaveDataInfo
    {
        public const string SaveFileName = "AnimalShopSaveData.json";

        /// <summary>
        /// Used to determine if the player has a tier 2 better barn or coop built to unlock the hay maker.
        /// </summary>
        [JsonProperty]
        public bool hasBuiltTier2BarnOrCoop;

        public AnimalShopSaveData()
        {

        }

        public override void save()
        {
            this.save(SaveFileName);
        }

        public virtual void setHasBuiltTier2OrHigherBarnOrCoop()
        {
            this.hasBuiltTier2BarnOrCoop = true;
        }

        public virtual bool getHasBuiltTier2OrHigherBarnOrCoop()
        {
            return this.hasBuiltTier2BarnOrCoop;
        }

    }
}
