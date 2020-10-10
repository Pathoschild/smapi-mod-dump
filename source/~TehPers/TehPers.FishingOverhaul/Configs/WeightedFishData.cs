/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;
using TehPers.Core.Api.Weighted;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class WeightedFishData : IWeighted {
        public int Fish { get; }
        public IFishData Data { get; }
        public Farmer Who { get; }

        public WeightedFishData(int fish, IFishData data, Farmer who) {
            this.Fish = fish;
            this.Data = data;
            this.Who = who;
        }

        public double GetWeight() {
            return this.Data.GetWeight(this.Who);
        }

    }
}
