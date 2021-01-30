/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Netcode;
using StardewValley;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class ToolsFactory
    {
        public static Tool GetMeatCleaver()
        {
            Tool meatCleaver = new Axe()
            {
                Name = "Meat Cleaver",
                InitialParentTileIndex = MeatCleaverOverrides.InitialParentTileIndex,
                IndexOfMenuItemView = MeatCleaverOverrides.IndexOfMenuItemView
            };
            meatCleaver.modData.Add(MeatCleaverOverrides.MeatCleaverKey, Game1.random.Next().ToString());
            return meatCleaver;
        }

        public static Item GetInseminationSyringe()
        {
            Tool inseminationSyringe = new MilkPail()
            {
                Name = "Insemination Syringe",
                InitialParentTileIndex = InseminationSyringeOverrides.InitialParentTileIndex,
                IndexOfMenuItemView = InseminationSyringeOverrides.IndexOfMenuItemView,
                CurrentParentTileIndex = InseminationSyringeOverrides.InitialParentTileIndex
            };
            AnimalHusbandryModEntry.ModHelper.Reflection.GetField<NetInt>(inseminationSyringe, "numAttachmentSlots").GetValue().Value = 1;
            inseminationSyringe.attachments.SetCount(1);
            string inseminationSyringeId = Game1.random.Next().ToString();
            inseminationSyringe.modData.Add(InseminationSyringeOverrides.InseminationSyringeKey, inseminationSyringeId);
            return inseminationSyringe;
        }

        public static Item GetFeedingBasket()
        {
            Tool feedingBasket = new MilkPail()
            {
                Name = "Feeding Basket",
                InitialParentTileIndex = FeedingBasketOverrides.InitialParentTileIndex,
                IndexOfMenuItemView = FeedingBasketOverrides.IndexOfMenuItemView,
                CurrentParentTileIndex = FeedingBasketOverrides.InitialParentTileIndex
            };
            AnimalHusbandryModEntry.ModHelper.Reflection.GetField<NetInt>(feedingBasket, "numAttachmentSlots").GetValue().Value = 1;
            feedingBasket.attachments.SetCount(1);
            string feedingBasketId = Game1.random.Next().ToString();
            feedingBasket.modData.Add(FeedingBasketOverrides.FeedingBasketKey, feedingBasketId);
            return feedingBasket;
        }
        
        public static Item GetParticipantRibbon()
        {
            Tool feedingBasket = new MilkPail()
            {
                Name = "Participant Ribbon",
                InitialParentTileIndex = ParticipantRibbonOverrides.InitialParentTileIndex,
                IndexOfMenuItemView = ParticipantRibbonOverrides.IndexOfMenuItemView,
                CurrentParentTileIndex = ParticipantRibbonOverrides.InitialParentTileIndex
            };
            string participantRibbonId = Game1.random.Next().ToString();
            feedingBasket.modData.Add(ParticipantRibbonOverrides.ParticipantRibbonKey, participantRibbonId);
            return feedingBasket;
        }
    }
}
