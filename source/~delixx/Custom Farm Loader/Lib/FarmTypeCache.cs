/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using System.Xml;

namespace Custom_Farm_Loader.Lib
{
    public sealed class FarmTypeCache
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static Dictionary<string, string> Cache = new();
        const string CacheFileName = "FarmTypeCache.json";

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            Helper.Events.GameLoop.Saved += Saved;

            LoadInitialCache();
        }

        private static void Saved(object sender, StardewModdingAPI.Events.SavedEventArgs e)
        {
            string friendlyName = SaveGame.FilterFileName(Game1.GetSaveGameName(false));
            string savefile = friendlyName + "_" + Game1.uniqueIDForThisGame;

            if (Cache.ContainsKey(savefile) && Cache[savefile] == Game1.GetFarmTypeID())
                return;

            Monitor.Log("Updating FarmTypeCache for " + Game1.GetSaveGameName(false));

            if (Cache.ContainsKey(savefile))
                Cache[savefile] = Game1.GetFarmTypeID();

            else Cache.Add(savefile, Game1.GetFarmTypeID());

            Helper.Data.WriteJsonFile(CacheFileName, Cache);
        }

        private static void LoadInitialCache()
        {
            int i = 0;
            bool isInitial = false;
            Cache = Helper.Data.ReadJsonFile<Dictionary<string, string>>(CacheFileName);

            if (Cache is null) {
                isInitial = true;
                Cache = new();
            }

            var saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves");
            if (!Directory.Exists(saveFilePath))
                return;

            var saveFiles = Directory.GetDirectories(saveFilePath);

            Monitor.Log("Generating FarmTypeCache, this might take a while initially", isInitial ? LogLevel.Info : LogLevel.Trace);
            foreach (var fullFilePath in saveFiles) {
                var saveFile = Path.GetFileName(fullFilePath);

                if (Cache.ContainsKey(saveFile))
                    continue;

                if (!File.Exists(Path.Combine(fullFilePath, saveFile)))
                    continue;

                Monitor.Log("Reading Farmtype for: " + saveFile);

                string whichFarm = readFarmTypeQuickly(saveFile);

                if (whichFarm == "")
                    whichFarm = readFarmType(saveFile);

                i++;
                Monitor.Log("Read: " + whichFarm);
                Cache.Add(saveFile, whichFarm);
            }

            Helper.Data.WriteJsonFile(CacheFileName, Cache);
            Monitor.Log($"Read {i} Savefiles", isInitial ? LogLevel.Info : LogLevel.Trace);
        }

        public static string getFarmType(string saveFile)
        {
            if (Cache.TryGetValue(saveFile, out var farmType))
                return farmType;

            return readFarmTypeQuickly(saveFile);
        }

        private static string readFarmTypeQuickly(string saveFile)
        {
            //Explanation: The FarmType is not part of the small SaveGameInfo, but instead the massive general purpose save file
            //In order to display the farm type icon it is required to read the whichFarm xml node.
            //Parsing those 2.5-5 mb large save data files to read that node is very draining on the performance
            //I noticed that the whichFarm node is somewhere in the last ~7k characters.
            //I leave 4x leeway and gamble that I can skip the first 3-5 million characters

            string fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", saveFile, saveFile);
            string whichFarm = "";

            var fileInfo = new FileInfo(fullFilePath);
            var offset = fileInfo.Length - 25000 < 0 ? 0 : fileInfo.Length - 25000;

            var target = Array.ConvertAll("<whichFarm>".ToCharArray(), chr => (int)chr);
            byte k = 0;

            using (var stream = File.OpenRead(fullFilePath)) {
                stream.Seek(offset, SeekOrigin.Begin);
                int chr;

                using (StreamReader sr = new StreamReader(stream))
                    while ((chr = sr.Read()) >= 0) {

                        if (k != 11) //k is the target length
                            if (chr == target[k])
                                k++;
                            else
                                k = 0;

                        else {
                            if (chr == '<')
                                break;
                            whichFarm += (char)chr;
                        }
                    }
            }

            return whichFarm;
        }

        //Failsave
        private static string readFarmType(string saveFile)
        {
            string fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", saveFile, saveFile);
            string whichFarm = "";

            XmlDocument doc = new XmlDocument();

            try {
                doc.Load(fullFilePath);

                var node = doc.DocumentElement.SelectSingleNode("/SaveGame/whichFarm");
                if (node != null)
                    whichFarm = node.InnerText;

                return whichFarm;
            } catch (Exception e) {
                Monitor.Log("Encountered error trying to parse savedata: " + e.Message);
                return "";
            }

        }
    }
}
