/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Enums;

namespace ExtremeFishingOverhaul {
    public class TehFishingHelper {
        public IFishingApi Api { get; private set; }
        private readonly Dictionary<string, Dictionary<int, FishData>> _addedLocs = new Dictionary<string, Dictionary<int, FishData>>();
        public Dictionary<int, FishTraits> AddedTraits { get; } = new Dictionary<int, FishTraits>();
        public Dictionary<int, FishData> AddedData { get; } = new Dictionary<int, FishData>();
        public ModEntry Mod { get; }

        public TehFishingHelper(ModEntry mod) {
            this.Mod = mod;
        }

        public void TryRegisterFish() {
            this.Api = this.Mod.Helper.ModRegistry.GetApi<IFishingApi>("TehPers.FishingOverhaul");
            if (this.Api == null)
                return;

            // Add traits
            foreach (KeyValuePair<int, FishTraits> trait in this.AddedTraits) {
                this.Api.SetFishTraits(trait.Key, trait.Value);
                this.Api.SetFishName(trait.Key, trait.Value.Name);
            }

            // Add data
            foreach (KeyValuePair<string, Dictionary<int, FishData>> locData in this._addedLocs) {
                foreach (KeyValuePair<int, FishData> data in locData.Value) {
                    this.Api.SetFishData(locData.Key, data.Key, data.Value);
                }
            }
        }

        public void AddFish(int id, FishTraits traits, FishData dataTemplate) {
            this.AddedTraits.Add(id, traits);
            this.AddedData.Add(id, dataTemplate);

            // Register fish if the API has already been loaded
            this.Api?.SetFishTraits(id, traits);
            this.Api?.SetFishName(id, traits.Name);
        }

        public void AddLocation(int id, string location) {
            if (!this.AddedData.TryGetValue(id, out FishData data))
                return;

            // Add location dictionary if it isn't already there
            if (!this._addedLocs.TryGetValue(location, out Dictionary<int, FishData> locData)) {
                locData = new Dictionary<int, FishData>();
                this._addedLocs[location] = locData;
            }

            // Add the fish to this location if it isn't already there
            locData[id] = data;

            // Register fish location if API has already been loaded
            this.Api?.SetFishData(location, id, data);
        }

        public class FishData : IFishData {
            public float Weight { get; set; }
            public List<string> Seasons { get; set; }
            public int MinLevel { get; set; }

            public FishData(int minLevel) {
                this.Weight = 1F;
                this.Seasons = new List<string>();
                this.MinLevel = minLevel;
            }

            public float GetWeight(Farmer who) {
                return this.Weight;
            }

            public bool MeetsCriteria(int fish, WaterType waterType, SDate date, Weather weather, int time, int level, int? mineLevel) {
                return this.Seasons.Contains(date.Season, StringComparer.OrdinalIgnoreCase)
                       && this.MinLevel <= level;
            }
        }

        public class FishTraits : IFishTraits {
            public string Name { get; }
            public float Difficulty { get; set; }
            public FishMotionType MotionType { get; set; }
            public int MinSize { get; set; }
            public int MaxSize { get; set; }

            public FishTraits(float difficulty, FishMotionType motionType, int minSize, int maxSize, string name) {
                this.Difficulty = difficulty;
                this.MaxSize = maxSize;
                this.Name = name;
                this.MinSize = minSize;
                this.MotionType = motionType;
            }
        }
    }
}
