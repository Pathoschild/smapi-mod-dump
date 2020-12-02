/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework
{
    /// <summary>
    /// Quest type (vanilla SDV types + one special custom)
    /// </summary>
    public enum QuestType
    {
        Custom,
        Basic,
        Building,
        Crafting,
        ItemDelivery,
        ItemHarvest,
        Location,
        LostItem,
        Monster,
        SecretLostItem,
        Social,
    }

    public static class QuestTypeId
    {
        public static int Basic => Quest.type_basic;
        public static int Building => Quest.type_building;
        public static int Crafting => Quest.type_crafting;
        public static int ItemDelivery => Quest.type_itemDelivery;
        public static int ItemHarvest => Quest.type_harvest;
        public static int Location => Quest.type_location;
        public static int LostItem => Quest.type_harvest;
        public static int Monster => Quest.type_monster;
        public static int SecretLostItem => Quest.type_harvest;
        public static int Social => Quest.type_socialize;

        /// <summary>
        /// Returns quest type id in vanilla SDV quest coresponding for a quest class type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int ToVanillaTypeId(this QuestType type)
        {
            switch (type)
            {
                case QuestType.Basic:
                    return Basic;
                case QuestType.Building:
                    return Building;
                case QuestType.Crafting:
                    return Crafting;
                case QuestType.ItemDelivery:
                    return ItemDelivery;
                case QuestType.ItemHarvest:
                    return ItemHarvest;
                case QuestType.Location:
                    return Location;
                case QuestType.LostItem:
                    return LostItem;
                case QuestType.Monster:
                    return Monster;
                case QuestType.SecretLostItem:
                    return SecretLostItem;
                case QuestType.Social:
                    return Social;
                default:
                    return 0;
            }
        }
    }
}
