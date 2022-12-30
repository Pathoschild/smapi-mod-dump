/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using MailFrameworkMod;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using OrnithologistsGuild.Game.Items;
using OrnithologistsGuild.Content;

namespace OrnithologistsGuild
{
    public class Mail
    {
        public static void Initialize()
        {
            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_Introduction",
                // Adds conversation topic "Ivy_OrnithologistGuild_Introduction" for 14 days
                $"{I18n.Mail_Introduction()} %item conversationTopic Ivy_OrnithologistGuild_Introduction 14 %%",
                new List<Item> { new LifeList() },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && (SDate.From(Game1.Date) >= new SDate(5, "spring", 1) || SaveDataManager.SaveData.LifeList.Count > 0),
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList1",
                I18n.Mail_LifeList1(),
                new List<Item> { new StardewValley.Object(770 /* Mixed Seeds */, 5) },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && SaveDataManager.SaveData.LifeList.IdentifiedCount > 0,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList5",
                I18n.Mail_LifeList5(),
                new List<Item> { new StardewValley.Object(270 /* Corn */, 5) },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && SaveDataManager.SaveData.LifeList.IdentifiedCount >= 5,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList10",
                I18n.Mail_LifeList10(),
                new List<Item> { new StardewValley.Object(431 /* Sunflower Seeds */, 5) },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && SaveDataManager.SaveData.LifeList.IdentifiedCount >= 10,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList15",
                I18n.Mail_LifeList15(),
                new List<Item> { new StardewValley.Object(296 /* Salmonberries */, 5) },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && SaveDataManager.SaveData.LifeList.IdentifiedCount >= 15,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailDao.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeListAll",
                // Adds conversation topic "Ivy_OrnithologistGuild_LifeListAll" for 14 days
                $"{I18n.Mail_LifeListAll()} %item conversationTopic Ivy_OrnithologistGuild_LifeListAll 14 %%",
                new List<Item> { new StardewValley.Object(928 /* Golden Egg */, 1) },
                (l) => !Game1.player.mailReceived.Contains(l.Id) && SaveDataManager.SaveData.LifeList.IdentifiedCount >= ContentPackManager.BirdieDefs.Count,
                (l) => Game1.player.mailReceived.Add(l.Id)));
        }
    }
}

