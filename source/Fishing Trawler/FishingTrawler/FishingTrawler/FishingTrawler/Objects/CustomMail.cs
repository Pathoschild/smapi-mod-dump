/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTrawler.Objects
{
    internal class CustomMail : IAssetEditor
    {
        internal CustomMail()
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;

            // Intro letter
            data["PeacefulEnd.FishingTrawler_WillyIntroducesMurphy"] = String.Format(ModEntry.i18n.Get("letter.meet_murphy"), Game1.MasterPlayer.modData[ModEntry.MURPHY_DAY_TO_APPEAR]);

            // Ginger Island letter
            data["PeacefulEnd.FishingTrawler_MurphyGingerIsland"] = String.Format(ModEntry.i18n.Get("letter.island_murphy"), Game1.MasterPlayer.modData[ModEntry.MURPHY_DAY_TO_APPEAR]);
        }
    }
}
