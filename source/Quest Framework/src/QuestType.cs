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
        Custom = Quest.type_basic,
        Basic = Quest.type_basic,
        Building = Quest.type_building,
        Crafting = Quest.type_crafting,
        ItemDelivery = Quest.type_itemDelivery,
        ItemHarvest = Quest.type_harvest,
        Location = Quest.type_location,
        LostItem = Quest.type_harvest,
        Monster = Quest.type_monster,
        SecretLostItem = Quest.type_harvest,
        Social = Quest.type_socialize,
    }
}
