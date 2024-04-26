/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using StardewValley;
using MailFrameworkMod;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using OrnithologistsGuild.Content;

namespace OrnithologistsGuild
{
    public class Mail
    {
        public static void Initialize()
        {
            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_Introduction",
                // Adds conversation topic "Ivy_OrnithologistGuild_Introduction" for 14 days
                $"{I18n.Mail_Introduction()} %item conversationTopic Ivy_OrnithologistGuild_Introduction 14 %%",
                new List<Item> { ItemRegistry.Create("(O)Ivy_OrnithologistsGuild_LifeList", 1) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    (SDate.From(Game1.Date) >= new SDate(5, "spring", 1) ||
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.Count > 0),
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList1",
                I18n.Mail_LifeList1(),
                new List<Item> { ItemRegistry.Create("(O)770" /* Mixed Seeds */, 5) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.IdentifiedCount > 0,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList3",
                I18n.Mail_LifeList3(),
                new List<Item> { ItemRegistry.Create("(O)270" /* Corn */, 5) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.IdentifiedCount >= 3,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList5",
                I18n.Mail_LifeList5(),
                new List<Item> { ItemRegistry.Create("(O)431" /* Sunflower Seeds */, 5) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.IdentifiedCount >= 5,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeList7",
                I18n.Mail_LifeList7(),
                new List<Item> { ItemRegistry.Create("(O)296" /* Salmonberries */, 5) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.IdentifiedCount >= 7,
                (l) => Game1.player.mailReceived.Add(l.Id)));

            MailRepository.SaveLetter(new Letter(
                "Mods_Ivy_OrnithologistsGuild_LifeListAll",
                // Adds conversation topic "Ivy_OrnithologistGuild_LifeListAll" for 14 days
                $"{I18n.Mail_LifeListAll()} %item conversationTopic Ivy_OrnithologistGuild_LifeListAll 14 %%",
                new List<Item> { ItemRegistry.Create("(O)928" /* Golden Egg */, 1) },
                (l) =>
                    !Game1.player.mailReceived.Contains(l.Id) &&
                    SaveDataManager.SaveData.ForPlayer(Game1.player.UniqueMultiplayerID).LifeList.IdentifiedCount >= ContentPackManager.BirdieDefs.Count,
                (l) => Game1.player.mailReceived.Add(l.Id)));
        }
    }
}

