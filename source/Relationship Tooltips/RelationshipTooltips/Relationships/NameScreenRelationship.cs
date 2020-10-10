/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/RelationshipTooltips
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    public class NameScreenRelationship : IRelationship, IInputListener
    {
        public NameScreenRelationship(ModConfig config)
        {
            Display = false;
            Config = config;
        }
        private ModConfig Config;
        public bool Display { get; set; }
        private SButton DisplayKey { get { return Config.DisplayAllNPCNamesKey; } }
        public Func<Character, Item, bool> ConditionsMet => (c, i) => { return Display && (c is NPC && !((NPC)c).IsMonster || c is FarmAnimal || c is Horse); };

        public int Priority => 30000;

        public bool BreakAfter => false;

        public Action<Character, Item, EventArgsInput> ButtonPressed => (c, i, e) => { if (e.Button == DisplayKey) { Display = true; } };

        public Action<Character, Item, EventArgsInput> ButtonReleased => (c, i, e) => { if (e.Button == DisplayKey) { Display = false; } };

        public string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            return "";
        }

        public string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            NPC n = character as NPC;
            if (n == null || currentHeader != "")
            {
                if (character is FarmAnimal || character is Horse)
                    return character.displayName;
                return "";
            }
            if(character.GetType() == typeof(NPC))
            {
                if (Game1.player.friendshipData.ContainsKey(character.Name))
                {
                    return character.displayName;
                }
                else if (NonFriendNPCRelationship.NonGiftableNPCs.Contains(character.Name))
                {
                    return character.displayName;
                }
                else
                    return "???";
            }
            return character.displayName;
        }
    }
}
