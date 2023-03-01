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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using WaterRetainingFieldMod.Integrations;

namespace WaterRetainingFieldMod
{
    public class DataLoader
    {
        internal static IModHelper Helper;
        internal static ITranslationHelper I18N;
        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            IMailFrameworkModApi mailFrameworkModApi = helper.ModRegistry.GetApi<IMailFrameworkModApi>("DIGUS.MailFrameworkMod");
            if (mailFrameworkModApi != null)
            {
                ApiLetter letter = new ApiLetter
                {
                    Id = "WaterRetainingFieldLetter",
                    Text = "WaterRetainingFieldResolution.Letter",
                    Title = "WaterRetainingFieldResolution.Letter.Title",
                    I18N = helper.Translation
                };
                mailFrameworkModApi.RegisterLetter(
                    letter
                    ,(l) => !Game1.player.mailReceived.Contains(l.Id) && (Game1.player.farmingLevel.Value >= 4 || SDate.Now() >= new SDate(15, "spring", 1))
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                );
            }
        }
    }
}
