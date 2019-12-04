using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

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
            Letter letter = new Letter("WaterRetainingFieldLetter"
                , I18N.Get("WaterRetainingFieldResolution.Letter")
                ,(l)=>
                {
                    return !Game1.player.mailReceived.Contains(l.Id) &&
                           (Game1.player.farmingLevel.Value >= 4 || SDate.Now() >= new SDate(15, "spring", 1));
                },(l)=> Game1.player.mailReceived.Add(l.Id)
            )
            {
                Title = I18N.Get("WaterRetainingFieldResolution.Letter.Title")
            };
            MailDao.SaveLetter(letter);
        }
    }
}
