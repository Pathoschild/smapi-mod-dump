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
    public class CarpenterShopSaveData : ShopSaveDataInfo
    {

        public const string SaveFileName = "CarpenterShopSaveData.json";

        public CarpenterShopSaveData()
        {

        }

        public override void save()
        {
            this.save(SaveFileName);
        }

    }
}
