/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QuestEssentials.Framework;
using QuestEssentials.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    public class GiftTask : QuestTask<GiftTask.GiftData>
    {
        public enum LikeLevels
        {
            None,
            Hated,
            Disliked,
            Neutral,
            Liked,
            Loved
        }

        public struct GiftData
        {
            public string AcceptedContextTags { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public LikeLevels MinimumLikeLevel { get; set; }
            public string NpcName { get; set; }
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is GiftMessage giftMessage && this.IsWhenMatched())
            {
                if (giftMessage.Item == null || giftMessage.Npc == null)
                    return false;

                if (this.Data.NpcName != null && this.Data.NpcName != giftMessage.Npc.Name)
                    return false;

                if (this.Data.AcceptedContextTags != null && !Helper.CheckItemContextTags(giftMessage.Item, this.Data.AcceptedContextTags))
                    return false;

                if (this.Data.MinimumLikeLevel > LikeLevels.None)
                {
                    int like_level = giftMessage.Npc.getGiftTasteForThisItem(giftMessage.Item);
                    LikeLevels gift_like_level = MapGiftLikeLevel(like_level);

                    if (gift_like_level < this.Data.MinimumLikeLevel)
                    {
                        return false;
                    }
                }

                this.IncrementCount(1);

                return true;
            }

            return false;
        }

        private static LikeLevels MapGiftLikeLevel(int like_level)
        {
            LikeLevels gift_like_level = LikeLevels.None;
            switch (like_level)
            {
                case 6:
                    gift_like_level = LikeLevels.Hated;
                    break;
                case 4:
                    gift_like_level = LikeLevels.Disliked;
                    break;
                case 8:
                    gift_like_level = LikeLevels.Neutral;
                    break;
                case 2:
                    gift_like_level = LikeLevels.Liked;
                    break;
                case 0:
                    gift_like_level = LikeLevels.Loved;
                    break;
            }

            return gift_like_level;
        }
    }
}
