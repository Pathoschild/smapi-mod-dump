/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;
using ModConfig = CustomCrystalariumMod.ModConfig;

namespace CustomCrystalariumMod
{
    internal class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static Dictionary<int,int> CrystalariumData;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CrystalariumData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<int, int>>("data\\CrystalariumData.json") ?? new Dictionary<int, int>(){{74, 20160}};
            DataLoader.Helper.Data.WriteJsonFile("data\\CrystalariumData.json", CrystalariumData);

            if (!ModConfig.DisableLetter)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCrystalarium"
                        , I18N.Get("CustomCrystalarium.Letter")
                        , (l) => !Game1.player.mailReceived.Contains(l.Id)
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                    {
                        Title = I18N.Get("CustomCrystalarium.Letter.Title")
                    }
                );
            }
        }
    }
}
