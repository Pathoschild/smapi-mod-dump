using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace PartOfTheCommunity.Framework
{
    /// <summary>Tracked data for an NPC.</summary>
    internal class CharacterInfo
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The character type.</summary>
        public CharacterType Type { get; }

        /// <summary>The NPC name.</summary>
        public string Name { get; }

        /// <summary>Whether the NPC is male.</summary>
        public bool IsMale { get; }

        /// <summary>Whether the NPC owns a shop.</summary>
        public bool IsShopOwner { get; set; }

        /// <summary>Whether the player talked to this NPC today.</summary>
        public bool HasTalked { get; set; }

        /// <summary>Whether the player gifted this NPC today.</summary>
        public bool ReceivedGift { get; set; }

        /// <summary>Whether the player shopped at the NPC's store today.</summary>
        public bool HasShopped { get; set; }

        /// <summary>The number of NPCs this character saw the player talk to nearby.</summary>
        public int NearbyTalksSeen { get; set; }

        /// <summary>The NPC's relationships with other NPCs.</summary>
        public IList<CharacterRelationship> Relationships { get; } = new List<CharacterRelationship>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="isMale">Whether the NPC is male.</param>
        /// <param name="name">The NPC name.</param>
        /// <param name="type">The character type.</param>
        public CharacterInfo(string name, bool isMale, CharacterType type = CharacterType.Villager)
        {
            this.Name = name;
            this.IsMale = isMale;
            this.Type = type;
        }

        /// <summary>Add a relationship to another NPC.</summary>
        /// <param name="relationship">The target character's relationship to the original character (like 'Mother').</param>
        /// <param name="character">The target character.</param>
        public void AddRelationship(Relationship relationship, CharacterInfo character)
        {
            this.Relationships.Add(new CharacterRelationship(relationship, character));
        }

        /// <summary>Get the in-game instance for this character.</summary>
        /// <param name="npc">The in-game instance for this character.</param>
        /// <returns>Returns whether the NPC was found.</returns>
        public bool TryGetInstance(out Character npc)
        {
            switch (this.Type)
            {
                case CharacterType.Villager:
                    npc = Game1.getCharacterFromName(this.Name, mustBeVillager: true);
                    return npc != null;

                case CharacterType.Player:
                    npc = this.Name == Game1.player.Name ? Game1.player : null;
                    return npc != null;

                case CharacterType.Child:
                    npc = Game1.player.getChildren().FirstOrDefault(p => p.Name == this.Name);
                    return npc != null;

                default:
                    throw new NotSupportedException($"Unknown character type {this.Type} for NPC {this.Name}.");
            }
        }

        /// <summary>Get the NPC for this character.</summary>
        /// <param name="npc">The NPC for this character.</param>
        /// <returns>Returns whether the NPC was found.</returns>
        public bool TryGetNpc(out NPC npc)
        {
            if (this.TryGetInstance(out Character character) && character is NPC instance)
            {
                npc = instance;
                return true;
            }

            npc = null;
            return false;
        }
    }
}
