using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GiftTasteHelper.Framework
{
    internal class GiftInfo
    {
        public ItemData Item;
        public GiftTaste Taste;
        public bool Universal;
    }

    internal class GiftDrawData
    {
        public Rectangle IconSize => this.Gifts.First().Item.TileSheetSourceRect;
        public string NpcName { get; }
        public GiftInfo[] Gifts;

        public GiftDrawData(string npcName)
        {
            this.NpcName = npcName;
        }
    }

    internal class GiftDrawDataProvider : IGiftDrawDataProvider
    {
        private IGiftDataProvider GiftDataProvider;

        // Contains all the gift info for all npcs (or just what's known if we're in prog mode).
        private static Dictionary<string, NpcGiftInfo> NpcGiftInfo;

        public GiftDrawDataProvider(IGiftDataProvider GiftDataProvider, bool rebuildGiftData = true)
        {
            this.GiftDataProvider = GiftDataProvider;
            this.GiftDataProvider.DataSourceChanged += () => BuildGiftData();

            // Only build once the first time, after that it only needs to be rebuilt
            // if the datasource changes (in progression mode).
            // TODO: this rebuild flag is causing it to be built twice (once for calendar, once for social).
            // It only happens once so it's not a big deal, but if there's a nice way to avoid it that would be better.
            if (NpcGiftInfo == null || rebuildGiftData)
            {
                BuildGiftData();
            }
        }

        public bool HasDataForNpc(string npcName)
        {
            return NpcGiftInfo.ContainsKey(npcName);
        }

        public GiftDrawData GetDrawData(string npcName, GiftTaste[] tastesToDisplay, bool includeUniversal)
        {
            if (tastesToDisplay.Length == 0 || !NpcGiftInfo.ContainsKey(npcName))
            {
                return null;
            }

            IEnumerable<GiftInfo> MakeGifts(Dictionary<GiftTaste, ItemData[]> allGifts, bool universal = false)
            {
                return tastesToDisplay
                .SelectMany(taste => allGifts[taste]
                .Select(item => new GiftInfo() { Item = item, Taste = taste, Universal = universal }))
                .OrderBy(gift => gift.Item.Name);
            }

            NpcGiftInfo infoForNpc = NpcGiftInfo[npcName];
            return new GiftDrawData(npcName)
            {
                Gifts = (includeUniversal
                ? MakeGifts(infoForNpc.UniversalGifts, true).Concat(MakeGifts(infoForNpc.Gifts))
                : MakeGifts(infoForNpc.Gifts))
                .ToArray()
            };
        }

        private void BuildGiftData()
        {
            Utils.DebugLog("[GiftDrawDataProvider] - Building Gift Data");

            NpcGiftInfo = new Dictionary<string, NpcGiftInfo>();
            foreach (var giftTaste in Game1.NPCGiftTastes)
            {
                string npcName = giftTaste.Key;

                // We only want NPC's
                if (Utils.UniversalTastes.Keys.Contains(npcName))
                {
                    continue;
                }

                NpcGiftInfo npcInfo = new NpcGiftInfo();
                foreach (GiftTaste taste in Enum.GetValues(typeof(GiftTaste)))
                {
                    if (taste == GiftTaste.MAX)
                        continue;

                    var itemIds = this.GiftDataProvider.GetGifts(npcName, taste, false);
                    var universalItemIds = this.GiftDataProvider.GetUniversalGifts(npcName, taste);

                    npcInfo.Gifts.Add(taste, ItemData.MakeItemsFromIds(itemIds));
                    npcInfo.UniversalGifts.Add(taste, ItemData.MakeItemsFromIds(universalItemIds));
                }

                NpcGiftInfo.Add(npcName, npcInfo);
            }
        }
    }
}
