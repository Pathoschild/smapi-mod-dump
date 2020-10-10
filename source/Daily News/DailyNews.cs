/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/DailyNews
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using CustomTV;
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace DailyNews
{

    public class ModEntry : Mod
    {
        private int dailyNews;
        private ModConfig config;
        private ModData contentFiles;
        private string[] contentFolderFiles;
        private string contentFolderExtension;
        private List<Headline> NewsItems = new List<Headline>();
        public string customContentFolder;

        public override void Entry(IModHelper helper)
        {
            Load();
            TimeEvents.AfterDayStarted += (x, y) => checkIfNews();
        }

        private void Load()
        {
            //Read the config file.
            this.config = this.Helper.ReadConfig<ModConfig>();
            customContentFolder = Path.Combine(this.Helper.DirectoryPath, this.config.contentFolder);
            if (!Directory.Exists(customContentFolder))
            {
                Monitor.Log("The '" + this.config.contentFolder + "' folder is empty. Mod broken.", LogLevel.Error);
                Directory.CreateDirectory(customContentFolder);
                this.Monitor.Log("Created the '" + this.config.contentFolder + "' folder", LogLevel.Warn);
            }

            this.contentFolderExtension = config.extension;

            //Read the news files.
            contentFolderFiles = ParseDir(customContentFolder, contentFolderExtension);
            foreach (string file in contentFolderFiles)
            {
                this.Monitor.Log("Loading: " + file, LogLevel.Trace);
                contentFiles = this.Helper.ReadJsonFile<ModData>(file) ?? new ModData();
                string newscasterFileName = string.IsNullOrWhiteSpace(contentFiles.newscaster)
                    ? config.defaultNewscaster
                    : contentFiles.newscaster;

                foreach (string headlineItem in contentFiles.newsItems)
                    NewsItems.Add(new Headline(headlineItem, newscasterFileName, file));
            }
        }

        private void checkIfNews()
        {
            CustomTVMod.removeChannel("News");

            Random randomNews = new Random();
            dailyNews = randomNews.Next(0, NewsItems.Count);

            string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);

            if (str.Equals("Tue") || str.Equals("Fri") || str.Equals("Sat"))
            {
                string season = Game1.currentSeason;
                CustomTVMod.addChannel("News", "News Report", deliverNews);

                if (config.showMessages)
                    showMessage("Breaking News for " + char.ToUpper(season[0]) + season.Substring(1) + " " + Game1.dayOfMonth, 2);
            }
        }

        private void deliverNews(TV tv, TemporaryAnimatedSprite sprite, Farmer who, string answer)
        {
            Texture2D newsScreen = null;

            try  //Try to load news screen from .json file
            {
                newsScreen = Helper.Content.Load<Texture2D>(NewsItems[dailyNews].Texture);
            }
            catch  //If the load failed, just load the default newscaster.
            {
                Monitor.Log("Unable to load newscaster(" + NewsItems[dailyNews].Texture + ") defined in: " + NewsItems[dailyNews].Source, LogLevel.Error);
            }

            TemporaryAnimatedSprite newsSprite = new TemporaryAnimatedSprite(newsScreen, new Rectangle(0, 0, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            string text = NewsItems[dailyNews].HeadlineText;
            CustomTVMod.showProgram(newsSprite, text, CustomTVMod.endProgram);
        }

        private static void showMessage(string msg, int type)
        {
            var hudmsg = new HUDMessage(msg, Color.SeaGreen, 5250f, true);
            hudmsg.whatType = type;
            Game1.addHUDMessage(hudmsg);
        }

        private string[] ParseDir(string path, string extension)
        {
            return Directory.GetFiles(path, extension, SearchOption.AllDirectories);
        }
    }

}
