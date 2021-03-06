/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Json.Serialization;
using TehPers.FishingOverhaul.Api.Enums;

namespace TehPers.FishingOverhaul.Configs {
    [JsonDescribe]
    public class ConfigFishTraits {
        [Description("The traits for each fish.")]
        public Dictionary<int, FishTraits> FishTraits { get; set; }

        public void PopulateData() {
            ModFishing.Instance.Monitor.Log("Automatically populating fishTraits.json with data from Fish.xnb", LogLevel.Info);
            ModFishing.Instance.Monitor.Log("NOTE: If this file is modded, the config will reflect the changes!", LogLevel.Info);

            Dictionary<int, string> fishDict = ModFishing.Instance.Helper.Content.Load<Dictionary<int, string>>(@"Data\Fish.xnb", ContentSource.GameContent);
            this.FishTraits = this.FishTraits ?? new Dictionary<int, FishTraits>();
            IEnumerable<int> possibleFish = (from locationKV in ModFishing.Instance.FishConfig.PossibleFish
                                             from fishKV in locationKV.Value
                                             select fishKV.Key).Distinct();

            // Loop through each possible fish
            foreach (int fish in possibleFish) {
                try {
                    if (!fishDict.TryGetValue(fish, out string rawData))
                        continue;

                    string[] data = rawData.Split('/');

                    // Get difficulty
                    int.TryParse(data[1], out int difficulty);

                    // Get motion type
                    string motionTypeName = data[2].ToLower();
                    FishMotionType motionType;
                    switch (motionTypeName) {
                        case "mixed":
                            motionType = FishMotionType.MIXED;
                            break;
                        case "dart":
                            motionType = FishMotionType.DART;
                            break;
                        case "smooth":
                            motionType = FishMotionType.SMOOTH;
                            break;
                        case "sinker":
                            motionType = FishMotionType.SINKER;
                            break;
                        case "floater":
                            motionType = FishMotionType.FLOATER;
                            break;
                        default:
                            motionType = FishMotionType.MIXED;
                            break;
                    }

                    // Get size
                    int minSize = Convert.ToInt32(data[3]);
                    int maxSize = Convert.ToInt32(data[4]);

                    // Add trait
                    this.FishTraits.Add(fish, new FishTraits {
                        Difficulty = difficulty,
                        MinSize = minSize,
                        MaxSize = maxSize,
                        MotionType = motionType
                    });
                } catch (Exception) {
                    ModFishing.Instance.Monitor.Log($"Failed to generate traits for {fish}, vanilla traits will be used.", LogLevel.Warn);
                }
            }
        }
    }
}
