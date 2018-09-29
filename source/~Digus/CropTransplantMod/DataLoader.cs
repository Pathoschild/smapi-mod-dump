using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace CropTransplantMod
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            MailDao.SaveLetter
            (
                new Letter
                (
                    "CropTransplantLetter"
                    , I18N.Get("CropTransplant.Letter")
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantPotLetter") && Game1.player.craftingRecipes.ContainsKey("Garden Pot") && !ModConfig.GetGradenPotEarlier
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );

            MailDao.SaveLetter
            (
                new Letter
                (
                    "CropTransplantPotLetter"
                    , I18N.Get("CropTransplantPot.Letter")
                    , new List<Item>() { new Object(Vector2.Zero, 62, false)}
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantLetter") && GetNpcFriendship("Evelyn") >= 2 * 250 && ModConfig.GetGradenPotEarlier
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );
        }

        private int GetNpcFriendship(string name)
        {
            if (Game1.player.friendshipData.ContainsKey(name))
            {
                return Game1.player.friendshipData[name].Points;
            }
            else
            {
                return 0;
            }
        }
    }
}
