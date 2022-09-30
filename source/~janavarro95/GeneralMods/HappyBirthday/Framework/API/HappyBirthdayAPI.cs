/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.HappyBirthday.Framework.Gifts;

namespace Omegasis.HappyBirthday.Framework.API
{
    public class HappyBirthdayAPI
    {

        /// <summary>
        /// Event hooks for when a birthday gift is registered.
        /// </summary>
        public event EventHandler<string> OnBirthdayGiftRegistered
        {
            add
            {
                HappyBirthdayModCore.Instance.giftManager.OnBirthdayGiftRegistered += value;
            }
            remove
            {
                HappyBirthdayModCore.Instance.giftManager.OnBirthdayGiftRegistered -= value;
            }
        }

        public event EventHandler PostAllBirthdayGiftsRegistered
        {
            add
            {
                HappyBirthdayModCore.Instance.giftManager.PostAllBirthdayGiftsRegistered += value;
            }
            remove
            {
                HappyBirthdayModCore.Instance.giftManager.PostAllBirthdayGiftsRegistered -= value;
            }
        }

        public HappyBirthdayAPI()
        {
        }

        /// <summary>
        /// Registers a specific item under a unique gift id to be used when adding items to npc gift pools.
        /// </summary>
        /// <param name="UniqueGiftId">The unique gift id for a given item.</param>
        /// <param name="item">The actual item to be referenced when given as a gift.</param>
        /// <returns>A boolean representing if registration was successful or not.</returns>
        public bool RegisterBirthdayGift(string UniqueGiftId, StardewValley.Item item)
        {
           return HappyBirthdayModCore.Instance.giftManager.registerGift(UniqueGiftId, item);
        }


        /// <summary>
        /// Checks to see if a given gift was registered or not.
        /// </summary>
        /// <param name="UniqueGiftId">The unique gift id for a given item.</param>
        /// <returns>A boolean representing if a gift was registered under a given id or not.</returns>
        public bool IsGiftRegistered(string UniqueGiftId)
        {
            return HappyBirthdayModCore.Instance.giftManager.isGiftRegistered(UniqueGiftId);
        }

        /// <summary>
        /// Unregisters a gift with a given id.
        /// </summary>
        /// <param name="UnqiueGiftId">The unique gift id for a given item.</param>
        /// <returns>A boolean representing if a gift under a given id was removed or not.</returns>
        public bool UnRegisterGift(string UnqiueGiftId)
        {
            return HappyBirthdayModCore.Instance.giftManager.removeGift(UnqiueGiftId);
        }

        /// <summary>
        /// Replaces a gift with the given unique gift id with a different one.
        /// </summary>
        /// <param name="UnqiueGiftId">The unique gift id to be replaced.</param>
        /// <param name="ReplacementGift">The replacement gift.</param>
        /// <returns></returns>
        public bool ModifyGift(string UnqiueGiftId, StardewValley.Item ReplacementGift)
        {
            return HappyBirthdayModCore.Instance.giftManager.modifyGift(UnqiueGiftId, ReplacementGift);
        }

        /// <summary>
        /// Adds a default birthday gift with the given paramaters.
        /// </summary>
        /// <param name="UniqueGiftId">The unique gift id as it is registered in <see cref="GiftIDS.RegisteredGifts"/></param>
        /// <param name="MinHeartsRequiredForGift">The minimum hearts required for an npc for this gift to be given.</param>
        /// <param name="MaxHeartsRequiredForGift">The maximum hearts required for an npc for this gift to be given. After this maximum threashold, the gift wil no longer be given to the player as a way to filter out "low quality" gifts.</param>
        /// <param name="MinStackAmount">The minimum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <param name="MaxStackAmount">The maximum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <returns></returns>
        public bool AddDefaultBirthdayGift(string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.registerDefaultBirthdayGift(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }

        /// <summary>
        /// Adds a gift for an ncp to give the player.
        /// </summary>
        /// <param name="NpcName">The name of the npc.</param>
        /// <param name="UniqueGiftId">The unique gift id as it is registered in <see cref="GiftIDS.RegisteredGifts"/></param>
        /// <param name="MinHeartsRequiredForGift">The minimum hearts required for an npc for this gift to be given.</param>
        /// <param name="MaxHeartsRequiredForGift">The maximum hearts required for an npc for this gift to be given. After this maximum threashold, the gift wil no longer be given to the player as a way to filter out "low quality" gifts.</param>
        /// <param name="MinStackAmount">The minimum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <param name="MaxStackAmount">The maximum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <returns>A boolean representing if the gift was registered or not.</returns>
        public bool AddNPCBirthdayGift(string NpcName,string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.registerNpcBirthdayGift(NpcName,UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }

        /*
        /// <summary>
        /// Adds a gift for a spouse to give the player. Unused since the Spouse will always give the player their favorite birthday gift, but I may change this in a future update.
        /// </summary>
        /// <param name="SpouseName">The name of the spouse.</param>
        /// <param name="UniqueGiftId">The unique gift id as it is registered in <see cref="GiftIDS.RegisteredGifts"/></param>
        /// <param name="MinHeartsRequiredForGift">The minimum hearts required for an npc for this gift to be given.</param>
        /// <param name="MaxHeartsRequiredForGift">The maximum hearts required for an npc for this gift to be given. After this maximum threashold, the gift wil no longer be given to the player as a way to filter out "low quality" gifts.</param>
        /// <param name="MinStackAmount">The minimum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <param name="MaxStackAmount">The maximum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <returns>A boolean representing if the gift was registered or not.</returns>
        public bool AddSpouseBirthdayGift(string SpouseName,string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.registerSpouseBirthdayGift(SpouseName, UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }
        */

        /// <summary>
        /// Removes a default (not associated with any specific npc) birthday gift that matches the given criteria.
        /// </summary>
        /// <param name="UniqueGiftId"></param>
        /// <param name="MinHeartsRequiredForGift"></param>
        /// <param name="MaxHeartsRequiredForGift"></param>
        /// <param name="MinStackAmount"></param>
        /// <param name="MaxStackAmount"></param>
        /// <returns></returns>
        public bool RemoveDefaultBirthdayGift(string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.unregisterDefaultBirthdayGift(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }

        /// <summary>
        /// Removes a gift from a given spouse pool that matches all of the given criteria.
        /// </summary>
        /// <param name="NpcName">The name of the spouse.</param>
        /// <param name="UniqueGiftId">The unique gift id as it is registered in <see cref="GiftIDS.RegisteredGifts"/></param>
        /// <param name="MinHeartsRequiredForGift">The minimum hearts required for an npc for this gift to be given.</param>
        /// <param name="MaxHeartsRequiredForGift">The maximum hearts required for an npc for this gift to be given. After this maximum threashold, the gift wil no longer be given to the player as a way to filter out "low quality" gifts.</param>
        /// <param name="MinStackAmount">The minimum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <param name="MaxStackAmount">The maximum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <returns>A boolean representing if the gift was removed or not.</returns>
        public bool RemoveNPCBirthdayGift(string NpcName, string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.unregisterNPCBirthdayGift(NpcName, UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }

        /*
        /// <summary>
        /// Adds a registered birthday gift from a spouse's pool of possible gifts. Not really used due to spouses giving the player's favorite gift instead, but this is hear incase I change it later.
        /// </summary>
        /// <param name="SpouseName">The name of the spouse.</param>
        /// <param name="UniqueGiftId">The unique gift id as it is registered in <see cref="GiftIDS.RegisteredGifts"/></param>
        /// <param name="MinHeartsRequiredForGift">The minimum hearts required for an npc for this gift to be given.</param>
        /// <param name="MaxHeartsRequiredForGift">The maximum hearts required for an npc for this gift to be given. After this maximum threashold, the gift wil no longer be given to the player as a way to filter out "low quality" gifts.</param>
        /// <param name="MinStackAmount">The minimum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <param name="MaxStackAmount">The maximum <see cref="StardewValley.Item.Stack"/> value.</param>
        /// <returns>A boolean representing if the gift was removed or not.</returns>
        public bool RemoveSpouseBirthdayGift(string SpouseName, string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return HappyBirthdayModCore.Instance.giftManager.unregisterSpouseBirthdayGift(SpouseName, UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount);
        }
        */


    }
}
