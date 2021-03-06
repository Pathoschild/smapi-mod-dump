/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.ComponentModel;
using TehPers.Core.Json.Serialization;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Enums;

namespace TehPers.FishingOverhaul.Configs {
    [JsonDescribe]
    public class FishTraits : IFishTraits {
        [Description("The difficulty of this fish.")]
        public float Difficulty { get; set; }

        [Description("The minimum size of the fish.")]
        public int MinSize { get; set; }

        [Description("The maximum size of the fish.")]
        public int MaxSize { get; set; }

        [Description("The fish's motion type. Mixed = 0, dart = 1, smooth = 2, sinker = 3, and floater = 4.")]
        public FishMotionType MotionType { get; set; }
    }
}