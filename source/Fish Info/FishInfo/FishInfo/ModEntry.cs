/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace FishInfo
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        internal static Dictionary<int, FishData> FishInfo = new Dictionary<int, FishData>();

        internal static IModHelper helper;
        internal static IMonitor monitor;

        internal static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            ModEntry.monitor = Monitor;
            Config = this.Helper.ReadConfig<ModConfig>();
            this.Helper.WriteConfig(Config);
            Patches.DoPatches();
            
            Helper.Events.GameLoop.SaveLoaded += LoadData;
            Helper.Events.GameLoop.DayStarted += LoadData;
        }

        private static FishData GetOrCreateData(int fishID)
        {
            if (FishInfo.TryGetValue(fishID, out FishData data))
            {
                return data;
            }
            else
            {
                FishInfo.Add(fishID, new FishData());
                return GetOrCreateData(fishID); //lol recursion
            }
        }

        public void LoadData(object sender, EventArgs e)
        {
            FishInfo.Clear();
            Dictionary<string, string> LocationData = helper.Content.Load<Dictionary<string, string>>("Data\\Locations", ContentSource.GameContent);
            foreach (KeyValuePair<string, string> locdata in LocationData)
            {
                string[] data = locdata.Value.Split('/');

                string locationName = locdata.Key;

                if (locationName == "fishingGame" || locationName == "Temp") continue; //don't want these - what the fuck even is temp
                //if (locationName == "BugLand") locationName = "MutantBugLair"; //fucking bugland lmao
                
                string[] seasonData;
                for (int i = 4; i <= 7; i++)
                {
                    if (data[i] == "-1")
                    {
                        continue;
                    }

                    seasonData = data[i].Split(' ');

                    for (int fish = 0; fish < seasonData.Length; fish += 2)
                    {
                        if (seasonData[fish] == "-1") continue;
                        int FishID;
                        int region;
                        try
                        {
                            ParseInts(seasonData[fish], seasonData[fish + 1], out FishID, out region);
                        }
                        catch
                        {
                            ParseInts(seasonData[fish], null, out FishID, out region);
                        }
                        
                        
                        FishData fd = GetOrCreateData(FishID);

                        if (locationName.Equals("forest", StringComparison.OrdinalIgnoreCase))
                        {
                            if (region == 0 || region == -1)
                            {
                                fd.AddLocation("ForestRiver");
                            }
                            if (region == 1 || region == -1)
                            {
                                fd.AddLocation("ForestPond");
                            }
                        }
                        else
                        {

                            fd.AddLocation(locationName);
                        }
                        
                        //locationName = Regex.Replace(locationName, "  ", " ");

                        fd.AddSeason((Season)(1 << (i - 4)));
                    }
                }

            }

            Dictionary<int, string> FishData = helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);
            foreach (KeyValuePair<int, string> fishData in FishData)
            {
                int FishID = fishData.Key;
                string[] fishInfo = fishData.Value.Split('/');

                FishData fd = GetOrCreateData(FishID);

                if(fishInfo.Length == 14)
                {
                    fd.FishName = fishInfo[13];
                }
                else if (fishInfo.Length == 13||fishInfo.Length == 7)
                {
                    string data = helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent)[FishID];
                    fd.FishName = data.Split('/')[4];
                }
                else if(fishInfo.Length == 8)
                {
                    fd.FishName = fishInfo[7];
                }
                else
                {
                    fd.FishName = fishInfo[4];
                }

                if (fishInfo[1] == "trap") //crabpot
                {
                    fd.IsCrabPot = true;
                    fd.AddLocation(fishInfo[4]);
                }
                else
                {
                    string[] times = fishInfo[5].Split(' ');

                    for (int time = 0; time < times.Length; time += 2)
                    {
                        fd.AddTimes(int.Parse(times[time]), int.Parse(times[time + 1]));
                    }

                    if (fishInfo[7] == "sunny" || fishInfo[7] == "both")
                    {
                        fd.AddWeather(Weather.Sun);
                    }
                    if (fishInfo[7] == "rainy" || fishInfo[7] == "both")
                    {
                        fd.AddWeather(Weather.Rain);
                    }
                }
            }
        }

        private void ParseInts(string StepInLoop, string NextStepInLoop, out int FishID, out int region)
        {
            if(StepInLoop == "1069-1") //Terrible hacky fix for issue that only occurs on spirit's eve when More new Fish is installed
            {
                FishID = 1069;
                region = -1;
                return;
            }
            FishID = int.Parse(StepInLoop);
            region = int.Parse(NextStepInLoop);
        }
    }
}
