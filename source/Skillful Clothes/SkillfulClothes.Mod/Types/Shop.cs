/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    public enum Shop
    {
        // NPC shops have the NPC id as value
        None = -1,        
        Clint = 2,
        Willy = 4,
        Pierre = 24,
        Robin = 25,        
        Dwarf = 31,
        Krobus = 33,     
        Marnie = 20,
        JojaMarket = 900,
        AdventureGuild = 901        
    }

    [Flags]
    public enum SellingCondition
    {
        None = 0,
        /// <summary>
        /// Corresponding Skill Level must be >= 2
        /// </summary>
        SkillLevel_2 = 1,
        /// <summary>
        /// Corresponding Skill Level must be >= 4
        /// </summary>
        SkillLevel_4 = 2,
        /// <summary>
        /// Corresponding Skill Level must be >= 6
        /// </summary>
        SkillLevel_6 = 4,
        /// <summary>
        /// Corresponding Skill Level must be >= 8
        /// </summary>
        SkillLevel_8 = 8,
        /// <summary>
        /// Corresponding Skill Level must be >= 10
        /// </summary>
        SkillLevel_10 = 16,
        /// <summary>
        /// Must have 2 or more hearts with the selling NPC
        /// </summary>
        FriendshipHearts_2 = 32,
        /// <summary>
        /// Must have 4 or more hearts with the selling NPC
        /// </summary>
        FriendshipHearts_4 = 64,
        /// <summary>
        /// Must have 6 or more hearts with the selling NPC
        /// </summary>
        FriendshipHearts_6 = 128,
        /// <summary>
        /// Must have 8 or more hearts with the selling NPC
        /// </summary>
        FriendshipHearts_8 = 256,
        /// <summary>
        /// Must have 10 or more hearts with the selling NPC
        /// </summary>
        FriendshipHearts_10 = 512
    }

    public static class ShopExtensions
    {
        public static string GetShopReferral(this Shop shop)
        {
            switch (shop)
            {
                case Shop.Willy: return "Willy";
                case Shop.Clint: return "Clint";
                case Shop.Pierre: return "Pierre";
                case Shop.Marnie: return "Marnie";                
                case Shop.Dwarf: return "the dwarf";
                case Shop.Krobus: return "Krobus";
                case Shop.Robin: return "Robin";
                case Shop.JojaMarket: return "the Joja market";
                case Shop.AdventureGuild: return "the Adventure Guild";
                default: return "someone";
            }
        }

        public static string GetNpcName(this Shop shop)
        {
            switch (shop)
            {
                case Shop.AdventureGuild:
                case Shop.JojaMarket:
                    return null;
                default: return shop.ToString();                
            }
        }

        public static Shop GetShop(this ShopMenu shopMenu)
        { 
            switch (shopMenu.ShopId.ToLower())
            {
                case "seedshop": return Shop.Pierre;                        
                case "joja": return Shop.JojaMarket;
                case "adventureshop": return Shop.AdventureGuild;
                case "carpenter": return Shop.Robin;
                case "animalshop": return Shop.Marnie;
                case "blacksmith":
                case "clintupgrade":
                    return Shop.Clint;
                case "dwarf": return Shop.Dwarf;
                case "fishshop": return Shop.Willy;
                    // hatmouse
                    // qigemshop
                    // saloon
                    // sandy
                    // bookseller
                    // shadowshop
            }
            return Shop.None;
        }

        public static Skill GetCorrespondingSkill(this Shop atShop)
        {
            switch (atShop)
            {
                case Shop.AdventureGuild: return Skill.Combat;
                case Shop.Willy: return Skill.Fishing;
                case Shop.Pierre: return Skill.Farming;
                case Shop.Clint: return Skill.Mining;
                case Shop.Dwarf: return Skill.Combat;
                case Shop.Marnie: return Skill.Foraging;
                default: return 0;
            }
        }

        /// <summary>
        /// Indicate if the player can (already) access the shop
        /// in their savegame
        /// </summary>        
        public static bool CanBeAccessedByPlayer(this Shop shop)
        {
            return true;
        }

        public static bool IsFulfilled(this SellingCondition condition, Shop atShop)
        {
            if (condition == SellingCondition.None) return true;

            foreach (SellingCondition sc in Enum.GetValues(typeof(SellingCondition)))
            {
                if (sc != SellingCondition.None && condition.HasFlag(sc))
                {
                    if (!CheckSingleCondition(sc, atShop)) return false;
                }
            }

            return true;
        }

        static bool CheckSingleCondition(SellingCondition condition, Shop atShop)
        {
            switch (condition)
            {
                case SellingCondition.SkillLevel_2: return GetCorrespondingSkillLevel(atShop) >= 2;
                case SellingCondition.SkillLevel_4: return GetCorrespondingSkillLevel(atShop) >= 4;
                case SellingCondition.SkillLevel_6: return GetCorrespondingSkillLevel(atShop) >= 6;
                case SellingCondition.SkillLevel_8: return GetCorrespondingSkillLevel(atShop) >= 8;
                case SellingCondition.SkillLevel_10: return GetCorrespondingSkillLevel(atShop) >= 10;

                case SellingCondition.FriendshipHearts_2: return GetCorrespondingFriendshipLevel(atShop) >= 2;
                case SellingCondition.FriendshipHearts_4: return GetCorrespondingFriendshipLevel(atShop) >= 4;
                case SellingCondition.FriendshipHearts_6: return GetCorrespondingFriendshipLevel(atShop) >= 6;
                case SellingCondition.FriendshipHearts_8: return GetCorrespondingFriendshipLevel(atShop) >= 8;
                case SellingCondition.FriendshipHearts_10: return GetCorrespondingFriendshipLevel(atShop) >= 10;
                default:
                    return false;
            }
        }

        static int GetCorrespondingSkillLevel(Shop atShop)
        {
            return atShop.GetCorrespondingSkill().GetCurrentLevel();
        }

        static int GetCorrespondingFriendshipLevel(Shop atShop)
        {
            string npcName = atShop.GetNpcName();
            if (String.IsNullOrEmpty(npcName)) return 0;

            return Game1.player.getFriendshipHeartLevelForNPC(npcName);
        }
    }
}
