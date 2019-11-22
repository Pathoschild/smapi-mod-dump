using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    public class NonFriendNPCRelationship : IRelationship
    {
        public Func<Character, Item, bool> ConditionsMet =>
            (c, i) =>
        c is NPC &&
        !c.IsMonster &&
        !(c is Pet) &&
        ((NPC)c).isVillager() &&
        !Game1.player.friendshipData.ContainsKey(c.Name)
            &&
            NonGiftableNPCs.Contains(c.Name);

        public int Priority => 20000;

        public bool BreakAfter => true;
        public static string[] NonGiftableNPCs = new string[]
        {
            "Bouncer",
            "Gil",
            "Governor",
            "Grandpa",
            "Gunther",
            "Henchman",
            "Marlon",
            "Morris",
            "Mr. Qi"
        };
        public string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            return "";
        }

        public string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
