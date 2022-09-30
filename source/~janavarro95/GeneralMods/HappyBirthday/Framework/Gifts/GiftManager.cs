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
using System.IO;
using System.Linq;
using Omegasis.HappyBirthday.Framework;
using Omegasis.HappyBirthday.Framework.Configs;
using Omegasis.HappyBirthday.Framework.ContentPack;
using Omegasis.HappyBirthday.Framework.Gifts;
using Omegasis.HappyBirthday.Framework.Utilities;
using StardewValley;
using static System.String;
using static Omegasis.HappyBirthday.Framework.Gifts.GiftIDS;
using SObject = StardewValley.Object;

namespace Omegasis.HappyBirthday
{
    public class GiftManager
    {

        /// <summary>The next birthday gift the player will receive.</summary>
        public Item BirthdayGiftToReceive;

        public Dictionary<string, List<GiftInformation>> npcBirthdayGifts;
        public Dictionary<string, List<GiftInformation>> spouseBirthdayGifts;
        public List<GiftInformation> defaultBirthdayGifts;

        public Dictionary<string, Item> registeredGifts = new Dictionary<string, Item>();

        public event EventHandler<string> OnBirthdayGiftRegistered;
        public event EventHandler PostAllBirthdayGiftsRegistered;

        /// <summary>Construct an instance.</summary>
        public GiftManager()
        {
            //this.BirthdayGifts = new List<Item>();


            this.npcBirthdayGifts = new Dictionary<string, List<GiftInformation>>();
            this.spouseBirthdayGifts = new Dictionary<string, List<GiftInformation>>();
            this.defaultBirthdayGifts = new List<GiftInformation>();
            this.registeredGifts = new Dictionary<string, Item>();

            this.registerGiftIDS();
        }

        /// <summary>
        /// Reloads all of the birthday gifts from content packs.
        /// </summary>
        public virtual void reloadBirthdayGifts()
        {

            this.npcBirthdayGifts.Clear();
            this.spouseBirthdayGifts.Clear();
            this.defaultBirthdayGifts.Clear();

            this.addInGiftsFromLoadedContentPacks();
        }


        protected virtual void registerGiftIDS()
        {
            foreach (var v in this.getSDVObjects())
            {
                Item i = new StardewValley.Object((int)v, 1);
                string uniqueID = "StardewValley.Object." + Enum.GetName(typeof(GiftIDS.SDVObject), (int)v);
                HappyBirthdayModCore.Instance.Monitor.Log("Added gift with id: " + uniqueID);

                if (this.registeredGifts.ContainsKey(uniqueID)) continue;

                this.registeredGifts.Add(uniqueID, i);


                if (OnBirthdayGiftRegistered != null)
                {
                    OnBirthdayGiftRegistered.Invoke(this,uniqueID);
                }

            }
            List<string> registeredGiftKeys = this.registeredGifts.Keys.ToList();
            registeredGiftKeys.Sort();
            HappyBirthdayModCore.Instance.Helper.Data.WriteJsonFile<List<string>>(Path.Combine("ModAssets", "Gifts", "RegisteredGifts" + ".json"), this.registeredGifts.Keys.ToList());
        }


        /// <summary>
        /// Called after all content packs have been loaded. It then combines the gifts from all of the content packs into this singular gift pool.
        /// </summary>
        public virtual void addInGiftsFromLoadedContentPacks()
        {

            //Loads in all gifts across all content packs across all translations.
            foreach (HappyBirthdayContentPack contentPack in HappyBirthdayModCore.Instance.happyBirthdayContentPackManager.contentPacks.Values.SelectMany(contentPackList=>contentPackList))
            {
                HappyBirthdayModCore.Instance.Monitor.Log("Adding default gifts for content pack: " + contentPack.baseContentPack.Manifest.UniqueID);
                foreach (GiftInformation giftInfo in contentPack.getDefaultBirthdayGifts())
                {
                    this.defaultBirthdayGifts.Add(giftInfo);
                }
                foreach (KeyValuePair<string,List<GiftInformation>> giftInfo in contentPack.npcBirthdayGifts)
                {
                    if (this.npcBirthdayGifts.ContainsKey(giftInfo.Key))
                    {
                        HappyBirthdayModCore.Instance.Monitor.Log(string.Format("Adding npc {1} gifts for content pack: {0}", contentPack.baseContentPack.Manifest.UniqueID, giftInfo.Key));
                        this.npcBirthdayGifts[giftInfo.Key].AddRange(giftInfo.Value);
                    }
                    else
                    {
                        HappyBirthdayModCore.Instance.Monitor.Log(string.Format("Adding npc {1} gifts for content pack: {0}", contentPack.baseContentPack.Manifest.UniqueID, giftInfo.Key));
                        this.npcBirthdayGifts.Add(giftInfo.Key, giftInfo.Value);
                    }
                }
                foreach (KeyValuePair<string, List<GiftInformation>> giftInfo in contentPack.spouseBirthdayGifts)
                {
                    if (this.spouseBirthdayGifts.ContainsKey(giftInfo.Key))
                    {
                        HappyBirthdayModCore.Instance.Monitor.Log(string.Format("Adding spouse {1} gifts for content pack: {0}", contentPack.baseContentPack.Manifest.UniqueID, giftInfo.Key));
                        this.spouseBirthdayGifts[giftInfo.Key].AddRange(giftInfo.Value);
                    }
                    else
                    {
                        HappyBirthdayModCore.Instance.Monitor.Log(string.Format("Adding spouse {1} gifts for content pack: {0}", contentPack.baseContentPack.Manifest.UniqueID, giftInfo.Key));
                        this.spouseBirthdayGifts.Add(giftInfo.Key, giftInfo.Value);
                    }
                }
            }

            List<string> registeredGiftKeys = this.registeredGifts.Keys.ToList();
            registeredGiftKeys.Sort();
            HappyBirthdayModCore.Instance.Helper.Data.WriteJsonFile<List<string>>(Path.Combine("ModAssets", "Gifts", "RegisteredGifts" + ".json"),registeredGiftKeys );
            if (PostAllBirthdayGiftsRegistered != null)
            {
                PostAllBirthdayGiftsRegistered.Invoke(this, new EventArgs());
            }
        }

        public virtual bool registerDefaultBirthdayGift(string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {

            return this.registerDefaultBirthdayGift(new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));

        }

        public virtual bool registerDefaultBirthdayGift(GiftInformation giftInformation)
        {
            this.defaultBirthdayGifts.Add(giftInformation);
            return true;
        }

        public virtual bool unregisterDefaultBirthdayGift(string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return this.unregisterDefaultBirthdayGift(new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));
        }

        public virtual bool unregisterDefaultBirthdayGift(GiftInformation giftInformation)
        {
            List<GiftInformation> removalList = new();
            for(int i = 0; i < this.defaultBirthdayGifts.Count; i++)
            {
                if (giftInformation.Equals(this.defaultBirthdayGifts[i]))
                {
                    removalList.Add(this.defaultBirthdayGifts[i]);
                }
            }
            foreach(GiftInformation info in removalList)
            {
                this.defaultBirthdayGifts.Remove(info);
            }
            return removalList.Count > 0;
        }

        public virtual bool registerNpcBirthdayGift(string NPC,string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return this.registerNpcBirthdayGift(NPC,new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));
        }

        public virtual bool registerNpcBirthdayGift(string NPC,GiftInformation giftInformation)
        {
            if (this.npcBirthdayGifts.ContainsKey(giftInformation.objectID))
            {
                this.npcBirthdayGifts[NPC].Add(giftInformation);
            }
            else
            {
                this.npcBirthdayGifts.Add(NPC, new List<GiftInformation>() { giftInformation });
            }
            return true;
        }

        public virtual bool unregisterNPCBirthdayGift(string NPC,string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return this.unregisterNPCBirthdayGift(NPC,new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));
        }

        public virtual bool unregisterNPCBirthdayGift(string NPC, GiftInformation giftInformation)
        {
            if (!this.npcBirthdayGifts.ContainsKey(NPC)) return false;
            List<GiftInformation> removalList = new();
            for (int i = 0; i < this.npcBirthdayGifts[NPC].Count; i++)
            {
                if (giftInformation.Equals(this.npcBirthdayGifts[NPC][i]))
                {
                    removalList.Add(this.npcBirthdayGifts[NPC][i]);
                }
            }
            foreach (GiftInformation info in removalList)
            {
                this.npcBirthdayGifts[NPC].Remove(info);
            }
            return removalList.Count > 0;
        }

        public virtual bool registerSpouseBirthdayGift(string SpouseName,string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return this.registerSpouseBirthdayGift(SpouseName, new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));
        }

        public virtual bool registerSpouseBirthdayGift(string SpouseName, GiftInformation giftInformation)
        {
            if (this.spouseBirthdayGifts.ContainsKey(giftInformation.objectID))
            {
                this.spouseBirthdayGifts[SpouseName].Add(giftInformation);
            }
            else
            {
                this.spouseBirthdayGifts.Add(SpouseName, new List<GiftInformation>() { giftInformation });
            }
            return true;
        }


        public virtual bool unregisterSpouseBirthdayGift(string Spouse, string UniqueGiftId, int MinHeartsRequiredForGift, int MaxHeartsRequiredForGift, int MinStackAmount, int MaxStackAmount)
        {
            return this.unregisterSpouseBirthdayGift(Spouse, new GiftInformation(UniqueGiftId, MinHeartsRequiredForGift, MaxHeartsRequiredForGift, MinStackAmount, MaxStackAmount));
        }

        public virtual bool unregisterSpouseBirthdayGift(string Spouse, GiftInformation giftInformation)
        {
            if (!this.spouseBirthdayGifts.ContainsKey(Spouse)) return false;
            List<GiftInformation> removalList = new();
            for (int i = 0; i < this.spouseBirthdayGifts[Spouse].Count; i++)
            {
                if (giftInformation.Equals(this.spouseBirthdayGifts[Spouse][i]))
                {
                    removalList.Add(this.spouseBirthdayGifts[Spouse][i]);
                }
            }
            foreach (GiftInformation info in removalList)
            {
                this.spouseBirthdayGifts[Spouse].Remove(info);
            }
            return removalList.Count > 0;
        }



        /// <summary>
        /// Gets the next birthday gift that would be received by the given npc.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Item getNextBirthdayGift(string name)
        {
            if (Game1.player.friendshipData.ContainsKey(name))
            {
                if (Game1.player.getSpouse() != null)
                {
                    if (Game1.player.getSpouse().Name.Equals(name))
                    {
                        //Get spouse gift here
                        Item gift = this.getSpouseBirthdayGift(name);
                        return gift;
                    }
                    else
                    {
                        Item gift = this.getNonSpouseBirthdayGift(name);
                        return gift;
                    }
                }
                else
                {
                    if (this.npcBirthdayGifts.ContainsKey(name))
                    {
                        Item gift = this.getNonSpouseBirthdayGift(name);
                        return gift;
                    }
                    else
                    {
                        Item gift = this.getDefaultBirthdayGift(name);
                        return gift;

                    }
                }
            }
            else
            {
                if (this.npcBirthdayGifts.ContainsKey(name))
                {

                    Item gift = this.getNonSpouseBirthdayGift(name);
                    return gift;
                }
                else
                {
                    Item gift = this.getDefaultBirthdayGift(name);
                    return gift;
                }
            }
        }

        /// <summary>
        /// Tries to get a default spouse birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public Item getNonSpouseBirthdayGift(string name)
        {
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);

            List<GiftInformation> possibleItems = new List<GiftInformation>();
            if (this.npcBirthdayGifts.ContainsKey(name))
            {
                List<GiftInformation> npcPossibleGifts = this.npcBirthdayGifts[name];

                foreach (GiftInformation info in npcPossibleGifts)
                {

                    if (info == null)
                    {
                        continue;
                    }

                    if (info.minRequiredHearts <= heartLevel && heartLevel <= info.maxRequiredHearts)
                    {
                        possibleItems.Add(info);
                    }
                }

                Item gift;
                int index = StardewValley.Game1.random.Next(possibleItems.Count);
                gift = possibleItems[index].getOne();
                return gift;

            }
            else
            {
                Item gift = this.getDefaultBirthdayGift(name);
                return gift;
            }

        }


        /// <summary>
        /// Tries to get a spouse birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public Item getSpouseBirthdayGift(string name)
        {
            if (string.IsNullOrEmpty(HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.favoriteBirthdayGift) == false)
            {
                if (this.registeredGifts.ContainsKey(HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.favoriteBirthdayGift))
                {
                    GiftInformation info = new GiftInformation(HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.favoriteBirthdayGift, 0, 1, 1);
                    return info.getOne();
                }
            }

            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);


            List<GiftInformation> possibleItems = new List<GiftInformation>();
            if (this.spouseBirthdayGifts.ContainsKey(name))
            {
                List<GiftInformation> npcPossibleGifts = this.spouseBirthdayGifts[name];
                foreach (GiftInformation info in npcPossibleGifts)
                {
                    if (info.minRequiredHearts <= heartLevel && heartLevel <= info.maxRequiredHearts)
                    {
                        possibleItems.Add(info);
                    }
                }

                Item gift;
                int index = StardewValley.Game1.random.Next(possibleItems.Count);
                gift = possibleItems[index].getOne();
                return gift;
            }
            else
            {
                return this.getNonSpouseBirthdayGift(name);
            }

        }

        /// <summary>
        /// Tries to get a default birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public Item getDefaultBirthdayGift(string name)
        {
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);

            List<GiftInformation> possibleItems = new List<GiftInformation>();

            List<GiftInformation> npcPossibleGifts = this.defaultBirthdayGifts;

            //If no gifts are registered and there are 0 default gifts, we should throw an error.
            if (this.registeredGifts.Count == 0 && this.defaultBirthdayGifts.Count==0)
            {
                throw new InvalidDataException("There are zero registered gifts and zero default birthday gifts for the game's gift manager as well as  {0} installed HappyBirthdayContentPacks." + HappyBirthdayModCore.Instance.Helper.ContentPacks.GetOwned().Count());
            }
            //Just give the player a random item if there are no default registered gifts.
            if (this.defaultBirthdayGifts.Count == 0)
            {
                int randomItemIndex = Game1.random.Next(this.registeredGifts.Count);
                Item randomItem = this.registeredGifts.ElementAt(randomItemIndex).Value.getOne();
                return randomItem;
            }

            //If for some reason this given npc doesn't have a gift just keep picking random npcs until something happens.
            if (npcPossibleGifts.Count == 0)
            {
                List<NPC> npcs = NPCUtilities.GetAllNonSpecialHumanNpcs();
                int npcRandomDefault = Game1.random.Next(npcs.Count);
                NPC npc = npcs[npcRandomDefault];
                return this.getDefaultBirthdayGift(npc.Name);
            }

            foreach (GiftInformation info in npcPossibleGifts)
            {
                if (info.minRequiredHearts <= heartLevel && heartLevel <= info.maxRequiredHearts)
                {
                    possibleItems.Add(info);
                }
            }



            Item gift;
            try
            {
                int index = StardewValley.Game1.random.Next(possibleItems.Count);
                gift = possibleItems[index].getOne();
                return gift;
            }
            catch(Exception err)
            {
                throw new ArgumentOutOfRangeException(string.Format("There were no possible items in the list of possible items for this npc's birthday gift list. The Npcs name is {0} and the original error message is this {1}",name,err.ToString()));
            }


        }

        /// <summary>
        /// Actually sets the next birthday gift to receieve or drops it on the ground for the player to pick up afterwards.
        /// </summary>
        /// <param name="gift"></param>
        public virtual void setNextBirthdayGift(Item gift)
        {
            if (Game1.player.isInventoryFull())
                Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
            else
                this.BirthdayGiftToReceive = gift;
        }

        /// <summary>Set the next birthday gift the player will receive.</summary>
        /// <param name="name">The villager's name who's giving the gift.</param>
        /// <remarks>This returns gifts based on the speaker's heart level towards the player: neutral for 3-4, good for 5-6, and best for 7-10.</remarks>
        public void setNextBirthdayGift(string name)
        {
            Item gift = this.getNextBirthdayGift(name);
            this.setNextBirthdayGift(gift);
        }


        public List<SDVObject> getSDVObjects()
        {
            SDVObject[] objIDS = (SDVObject[])Enum.GetValues(typeof(SDVObject));
            return objIDS.ToList();
        }
        public bool registerGift(string UnqiueGiftId, Item item)
        {
            if (this.isGiftRegistered(UnqiueGiftId))
            {
                return false;
            }
            this.registeredGifts.Add(UnqiueGiftId, item);

            if (OnBirthdayGiftRegistered != null)
            {
                OnBirthdayGiftRegistered.Invoke(HappyBirthdayModCore.Instance, UnqiueGiftId);
            }

            return true;
        }

        public bool removeGift(string UniqueGiftId)
        {
            return this.registeredGifts.Remove(UniqueGiftId);
        }

        public bool modifyGift(string UniqueGiftId, Item ReplacementGift)
        {
            if (this.isGiftRegistered(UniqueGiftId))
            {
                this.registeredGifts[UniqueGiftId] = ReplacementGift;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is there a gift registered with the given unique id?
        /// </summary>
        /// <param name="UnqiueGiftId"></param>
        /// <returns></returns>
        public bool isGiftRegistered(string UnqiueGiftId)
        {
            return this.registeredGifts.ContainsKey(UnqiueGiftId);
        }
    }

}
