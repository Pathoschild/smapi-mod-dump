/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace GiftTasteHelper.Framework
{
    internal class GiftInfo
    {
        public static readonly SVector2 IconSize = new (16, 16);
        public ItemData Item;
        public GiftTaste Taste;
        public bool Universal;
    }

    internal class GiftDrawData
    {
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

            return new GiftDrawData(npcName)
            {
                Gifts = GetGiftsOfTaste(npcName, tastesToDisplay, includeUniversal).ToArray()
            };
        }

        private IEnumerable<GiftInfo> GetGiftsOfTaste(string npcName, GiftTaste[] tastes, bool includeUniversal)
        {
            NpcGiftInfo infoForNpc = NpcGiftInfo[npcName];
            if (includeUniversal)
            {
                foreach (var taste in tastes)
                {
                    foreach (var gift in infoForNpc.UniversalGifts[taste].OrderBy(x => x.Name))
                    {
                        // Allow npc gift tastes override universal gift tastes
                        if (!infoForNpc.Gifts.Any(pair => pair.Key != GiftTaste.Neutral && pair.Value.Contains(gift)))
                        {
                            yield return new GiftInfo() { Item = gift, Taste = taste, Universal = true };
                        }
                    }
                }
            }

            foreach (var taste in tastes)
            {
                foreach (var gift in infoForNpc.Gifts[taste].OrderBy(x => x.Name))
                {
                    yield return new GiftInfo() { Item = gift, Taste = taste, Universal = false };
                }
            }
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
