/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FishingAdjustMod
{
    public class Main : Mod
    {
        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);

            helper.ConsoleCommands.Add("dump_fishingData", "Dump loaded fishing data. You can specify item numbers to dump specified fishes.", (_, args) => DumpFishingData(args));
            Global.Monitor = Monitor;
            Global.Config = helper.ReadConfig<Config>();
            helper.WriteConfig(Global.Config);
            // TODO: Verify Config values

            ContentEvents.AfterLocaleChanged += this.ContentEvents_AfterLocaleChanged;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
        }

        private void DumpFishingData(string[] args)
        {
            try
            {
                var fishingData = Game1.content.Load<Dictionary<int, string>>(@"Data\Fish");
                if (args.Length == 0)
                {
                    foreach (var p in fishingData)
                    {
                        Monitor.Log($"{p.Key} = {p.Value}");
                    }
                }
                else
                {
                    var keys = new HashSet<int>(args.Select(int.Parse));
                    foreach (var p in fishingData)
                    {
                        if (keys.Contains(p.Key))
                        {
                            Monitor.Log($"{p.Key} = {p.Value}");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Monitor.Log(e.ToString(), LogLevel.Error);
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            Monitor.Log("SaveEvents_AfterLoad");

            InjectFishingDifficultyAdjustance();
        }

        private void ContentEvents_AfterLocaleChanged(object sender, EventArgsValueChanged<string> e)
        {
            Monitor.Log("ContentEvents_AfterLocaleChanged");

            InjectFishingDifficultyAdjustance();
        }

        private void InjectFishingDifficultyAdjustance()
        {
            var fishingData = Game1.content.Load<Dictionary<int, string>>(@"Data\Fish");

            if (fishingData[163].Split('/')[1] != "110")
            {
                this.Monitor.Log("Fishing difficulty already adjusted!", LogLevel.Error);
                return;
            }

            foreach (var k in fishingData.Keys.ToArray())
            {
                var dataGroup = fishingData[k].Split('/');
                if (int.TryParse(dataGroup[1], out int level))
                {
                    dataGroup[1] = ((int)(level * Global.Config.AdjustRatio)).ToString();
                    fishingData[k] = string.Join("/", dataGroup);
                }
            }

            var harmony = HarmonyInstance.Create("io.github.hcoona.StardrewValleyMods.FishingAdjustMod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Monitor.Log("Fishing difficulty adjusted.", LogLevel.Info);
        }
    }
}
