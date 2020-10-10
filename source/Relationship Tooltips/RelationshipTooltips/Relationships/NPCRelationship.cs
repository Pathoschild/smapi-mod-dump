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
using StardewValley;
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    /// <summary>
    /// Represents the information and functionality needed to express an NPC's relationship with the player.
    /// </summary>
    public class NPCRelationship : IRelationship
    {
        protected IMonitor Monitor { get; set; }
        protected ModConfig Config { get; private set; }
        public virtual int Priority => 0;

        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return !c.IsMonster && (c.GetType() == typeof(NPC)) && (i == null || !i.canBeGivenAsGift()); };

        public bool BreakAfter => false;
        
        #region Tooltip
        public virtual string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            if (!Game1.player.friendshipData.ContainsKey(character.Name) && character.GetType() == typeof(NPC) && currentHeader == "")
                return "???";
            return character.displayName;
        }

        public virtual string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            NPC selectedNPC = character as NPC ?? throw new ArgumentNullException("character", "Cannot create display text for null Character.");
            if (Game1.player.friendshipData.TryGetValue(selectedNPC.Name, out Friendship friendship))
            {
                string flavourText = "";
                int level = friendship.Points/NPC.friendshipPointsPerHeartLevel;
                int maxLevel = friendship.IsMarried() ? 12 : 10;
                switch (friendship.Status)
                {
                    case FriendshipStatus.Dating:
                        {
                            flavourText = Config.GetDatingString(selectedNPC.Gender);
                            break;
                        }
                    case FriendshipStatus.Married:
                        {
                            flavourText = Config.GetMarriageString(selectedNPC.Gender);
                            break;
                        }
                    case FriendshipStatus.Engaged:
                        {
                            flavourText = Config.GetEngagedString(selectedNPC.Gender);
                            break;
                        }
                    case FriendshipStatus.Divorced:
                        {
                            flavourText = Config.GetDivorcedString(selectedNPC.Gender);
                            break;
                        }
                    case FriendshipStatus.Friendly:
                        {
                            switch (level)
                            {
                                case 0:
                                case 1:
                                    {
                                        flavourText = Config.friendshipAcquaintance;
                                        break;
                                    }
                                case 2:
                                case 3:
                                case 4:
                                    {
                                        flavourText = Config.friendshipFriend;
                                        break;
                                    }
                                case 5:
                                case 6:
                                case 7:
                                    {
                                        flavourText = Config.friendshipCloseFriend;
                                        break;
                                    }
                                case 8:
                                case 9:
                                case 10:
                                    {
                                        flavourText = Config.friendshipBestFriend;
                                        break;
                                    }
                            }
                            break;
                        }
                }
                return $"{flavourText}: {level}/{maxLevel} ({friendship.Points})";
            }
            return "";//blank display if unknown npc
        }
        #endregion

        public NPCRelationship(ModConfig config, IMonitor monitor)
        {
            Config = config;
            Monitor = monitor;
        }
    }
}
