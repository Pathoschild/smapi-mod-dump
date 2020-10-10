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
    }
}
