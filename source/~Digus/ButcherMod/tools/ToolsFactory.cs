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
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class ToolsFactory
    {
        public static Tool GetMeatCleaver()
        {
            Tool meatCleaver = ItemRegistry.Create<Tool>(MeatCleaverOverrides.MeatCleaverItemId);
            meatCleaver.modData[MeatCleaverOverrides.MeatCleaverKey] = Game1.random.Next().ToString();
            return meatCleaver;
        }

        public static Item GetInseminationSyringe()
        {
            Tool inseminationSyringe = ItemRegistry.Create<Tool>(InseminationSyringeOverrides.InseminationSyringeItemId);
            inseminationSyringe.modData[InseminationSyringeOverrides.InseminationSyringeKey] = Game1.random.Next().ToString();
            return inseminationSyringe;
        }

        public static Item GetFeedingBasket()
        {
            Tool feedingBasket = ItemRegistry.Create<Tool>(FeedingBasketOverrides.FeedingBasketItemId);
            feedingBasket.modData[FeedingBasketOverrides.FeedingBasketKey] = Game1.random.Next().ToString();
            return feedingBasket;
        }
        
        public static Item GetParticipantRibbon()
        {
            Tool participantRibbon = ItemRegistry.Create<Tool>(ParticipantRibbonOverrides.ParticipantRibbonItemId);
            participantRibbon.modData[ParticipantRibbonOverrides.ParticipantRibbonKey] = Game1.random.Next().ToString();
            return participantRibbon;
        }
    }
}
