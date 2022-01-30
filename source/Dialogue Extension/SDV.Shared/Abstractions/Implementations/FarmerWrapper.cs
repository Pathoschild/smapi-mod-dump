/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Menus;

namespace SDV.Shared.Abstractions
{
  public class FarmerWrapper : CharacterWrapper, IFarmerWrapper
  {
    public FarmerWrapper(Character character) : base(character)
    {
    }

    public IEnumerable<IItemWrapper> Items { get; set; }
    public IEnumerable<int> DialogueQuestionsAnswered { get; }
    public long UniqueMultiplayerID { get; set; }
    public IFarmerRendererWrapper FarmerRenderer { get; set; }
    public int CurrentToolIndex { get; set; }
    public IItemWrapper TemporaryItem { get; set; }
    public IItemWrapper CursorSlotItem { get; set; }
    public int WoodPieces { get; set; }
    public int StonePieces { get; set; }
    public int CopperPieces { get; set; }
    public int IronPieces { get; set; }
    public int CoalPieces { get; set; }
    public int GoldPieces { get; set; }
    public int IridiumPieces { get; set; }
    public int QuartzPieces { get; set; }
    public int Feed { get; set; }
    public int FarmingLevel { get; set; }
    public int MiningLevel { get; set; }
    public int CombatLevel { get; set; }
    public int ForagingLevel { get; set; }
    public int FishingLevel { get; set; }
    public int LuckLevel { get; set; }
    public int NewSkillPointsToSpend { get; set; }
    public int MaxStamina { get; set; }
    public int MaxItems { get; set; }
    public int HouseUpgradeLevel { get; set; }
    public int CoopUpgradeLevel { get; set; }
    public int BarnUpgradeLevel { get; set; }
    public bool HasTownKey { get; set; }
    public int MagneticRadius { get; set; }
    public int CraftingTime { get; set; }
    public bool IsMale { get; set; }
    public bool CanMove { get; set; }
    public bool UsingTool { get; set; }
    public IBoundingBoxGroupWrapper TemporaryPassableTiles { get; set; }
    public int visibleQuestCount { get; }
    public IItemWrapper recoveredItem { get; set; }
    public long theaterBuildDate { get; set; }
    public int festivalScore { get; set; }
    public int deepestMineLevel { get; set; }
    public float stamina { get; set; }
    public float Stamina { get; set; }
    public IFarmerTeamWrapper team { get; }
    public uint totalMoneyEarned { get; set; }
    public ulong millisecondsPlayed { get; set; }
    public bool hasRustyKey { get; set; }
    public bool hasSkullKey { get; set; }
    public bool canUnderstandDwarves { get; set; }
    public bool hasPendingCompletedQuests { get; }
    public bool useSeparateWallets { get; set; }
    public int timesReachedMineBottom { get; set; }
    public string spouse { get; set; }
    public bool isUnclaimedFarmhand { get; }
    public IHorseWrapper mount { get; set; }
    public int Level { get; }
    public double DailyLuck { get; }
    public IObjectWrapper ActiveObject { get; set; }
    public IToolWrapper CurrentTool { get; set; }
    public IItemWrapper CurrentItem { get; }
    public bool IsLocalPlayer { get; }
    public bool IsMainPlayer { get; }
    public IFarmerSpriteWrapper FarmerSprite { get; set; }
    public int _money { get; set; }
    public int QiGems { get; set; }
    public int Money { get; set; }
    public bool IsSitting() => false;

    public void LerpPosition(Vector2 start_position, Vector2 end_position, float duration)
    {
    }

    public void addUnearnedMoney(int money)
    {
    }

    public NetStringList GetEmoteFavorites() => null;

    public bool CanEmote() => false;

    public void performRenovation(string location_name)
    {
    }

    public void performPlayerEmote(string emote_string)
    {
    }

    public bool ShouldHandleAnimationSound() => false;

    public Point getMailboxPosition() => default;

    public void ClearBuffs()
    {
    }

    public void addBuffAttributes(int[] buffAttributes)
    {
    }

    public void removeBuffAttributes(int[] buffAttributes)
    {
    }

    public void removeBuffAttributes()
    {
    }

    public bool isActive() => false;

    public string getTexture() => null;

    public void checkForLevelTenStatus()
    {
    }

    public void unload()
    {
    }

    public void setInventory(IEnumerable<IItemWrapper> newInventory)
    {
    }

    public void makeThisTheActiveObject(IObjectWrapper o)
    {
    }

    public int getNumberOfChildren() => 0;

    public bool isRidingHorse() => false;

    public IEnumerable<IChildWrapper> getChildren()
    {
      yield break;
    }

    public int getChildrenCount() => 0;

    public IToolWrapper getToolFromName(string name) => null;

    public int? tryGetFriendshipLevelForNPC(string name) => null;

    public int getFriendshipLevelForNPC(string name) => 0;

    public int getFriendshipHeartLevelForNPC(string name) => 0;

    public bool isRoommate(string name) => false;

    public bool hasCurrentOrPendingRoommate() => false;

    public bool hasRoommate() => false;

    public bool hasAFriendWithHeartLevel(int heartLevel, bool datablesOnly) => false;

    public int getTallyOfObject(int index, bool bigCraftable) => 0;

    public bool areAllItemsNull() => false;

    public void shippedBasic(int index, int number)
    {
    }

    public void shiftToolbar(bool right)
    {
    }

    public void foundWalnut(int stack = 1)
    {
    }

    public void RemoveMail(string mail_key, bool from_broadcast_list = false)
    {
    }

    public void showNutPickup()
    {
    }

    public void foundArtifact(int index, int number)
    {
    }

    public void cookedRecipe(int index)
    {
    }

    public bool caughtFish(int index, int size, bool from_fish_pond = false, int numberCaught = 1) => false;

    public void gainExperience(int which, int howMuch)
    {
    }

    public int getEffectiveSkillLevel(int whichSkill) => 0;

    public void revealGiftTaste(INPCWrapper npc, int parent_sheet_index)
    {
    }

    public void revealGiftTaste(INPCWrapper npc, IObjectWrapper item)
    {
    }

    public void onGiftGiven(INPCWrapper npc, IObjectWrapper item)
    {
    }

    public bool hasGiftTasteBeenRevealed(INPCWrapper npc, int item_index) => false;

    public bool hasItemBeenGifted(INPCWrapper npc, int item_index) => false;

    public void MarkItemAsTailored(IItemWrapper item)
    {
    }

    public bool HasTailoredThisItem(IItemWrapper item) => false;

    public void foundMineral(int index)
    {
    }

    public void increaseBackpackSize(int howMuch)
    {
    }

    public void consumeObject(int index, int quantity)
    {
    }

    public int getItemCount(int item_index, int min_price = 0) => 0;

    public bool hasItemInInventory(int itemIndex, int quantity, int minPrice = 0) => false;

    public bool hasItemInInventoryNamed(string name) => false;

    public int getItemCountInList(IEnumerable<IItemWrapper> list, int item_index, int min_price = 0) => 0;

    public bool hasItemInList(IEnumerable<IItemWrapper> list, int itemIndex, int quantity, int minPrice = 0) => false;

    public void addItemByMenuIfNecessaryElseHoldUp(IItemWrapper item, ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
    }

    public void addItemByMenuIfNecessary(IItemWrapper item, ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
    }

    public void addItemsByMenuIfNecessary(IEnumerable<IItemWrapper> itemsToAdd, ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
    }

    public void ShowSitting()
    {
    }

    public void showRiding()
    {
    }

    public void showCarrying()
    {
    }

    public void showNotCarrying()
    {
    }

    public int GetDaysMarried() => 0;

    public IFriendshipWrapper GetSpouseFriendship() => null;

    public bool hasDailyQuest() => false;

    public void showToolUpgradeAvailability()
    {
    }

    public void dayupdate()
    {
    }

    public void doDivorce()
    {
    }

    public bool hasBuff(int whichBuff) => false;

    public bool hasOrWillReceiveMail(string id) => false;

    public void holdUpItemThenMessage(IItemWrapper item, bool showMessage = true)
    {
    }

    public void resetState()
    {
    }

    public void resetItemStates()
    {
    }

    public void clearBackpack()
    {
    }

    public int numberOfItemsInInventory() => 0;

    public void resetFriendshipsForNewDay()
    {
    }

    public int GetAppliedMagneticRadius() => 0;

    public void updateFriendshipGifts(IWorldDateWrapper date)
    {
    }

    public bool hasPlayerTalkedToNPC(string name) => false;

    public void fuelLantern(int units)
    {
    }

    public bool tryToCraftItem(IEnumerable<int[]> ingredients, double successRate, int itemToCraft, bool bigCraftable,
      string craftingOrCooking) =>
      false;

    public bool IsEquippedItem(IItemWrapper item) => false;

    public bool collideWith(IObjectWrapper o) => false;

    public void changeIntoSwimsuit()
    {
    }

    public void changeOutOfSwimSuit()
    {
    }

    public bool ownsFurniture(string name) => false;

    public void showFrame(int frame, bool flip = false)
    {
    }

    public void stopShowingFrame()
    {
    }

    public IItemWrapper addItemToInventory(IItemWrapper item, IEnumerable<IItemWrapper> affected_items_list) => null;

    public IItemWrapper addItemToInventory(IItemWrapper item) => null;

    public IItemWrapper addItemToInventory(IItemWrapper item, int position) => null;

    public void BeginSitting(ISittableWrapper furniture)
    {
    }

    public void StopSitting(bool animate = true)
    {
    }

    public void SortSeatExitPositions(IEnumerable<Vector2> list, Vector2 a, Vector2 b, Vector2 c)
    {
    }

    public bool isInventoryFull() => false;

    public bool couldInventoryAcceptThisItem(IItemWrapper item) => false;

    public bool couldInventoryAcceptThisObject(int index, int stack, int quality = 0) => false;

    public bool hasItemOfType(string type) => false;

    public INPCWrapper getSpouse() => null;

    public int freeSpotsInInventory() => 0;

    public IItemWrapper hasItemWithNameThatContains(string name) => null;

    public bool addItemToInventoryBool(IItemWrapper item, bool makeActiveObject = false) => false;

    public int getIndexOfInventoryItem(IItemWrapper item) => 0;

    public void reduceActiveItemByOne()
    {
    }

    public bool removeItemsFromInventory(int index, int stack) => false;

    public void ReequipEnchantments()
    {
    }

    public void removeItemFromInventory(IItemWrapper which)
    {
    }

    public IItemWrapper removeItemFromInventory(int whichItemIndex) => null;

    public bool isMarried() => false;

    public bool isEngaged() => false;

    public void removeFirstOfThisItemFromInventory(int parentSheetIndexOfItem)
    {
    }

    public void changeShirt(int whichShirt, bool is_customization_screen = false)
    {
    }

    public void changePantStyle(int whichPants, bool is_customization_screen = false)
    {
    }

    public void ConvertClothingOverrideToClothesItems()
    {
    }

    public void changeHairStyle(int whichHair)
    {
    }

    public bool IsBaldHairStyle(int style) => false;

    public void changeShoeColor(int which)
    {
    }

    public void changeHairColor(Color c)
    {
    }

    public void changePants(Color color)
    {
    }

    public void changeHat(int newHat)
    {
    }

    public void changeAccessory(int which)
    {
    }

    public void changeSkinColor(int which, bool force = false)
    {
    }

    public bool hasDarkSkin() => false;

    public void changeEyeColor(Color c)
    {
    }

    public int getHair(bool ignore_hat = false) => 0;

    public void changeGender(bool male)
    {
    }

    public void changeFriendship(int amount, INPCWrapper n)
    {
    }

    public bool knowsRecipe(string name) => false;

    public Vector2 getUniformPositionAwayFromBox(int direction, int distance) => default;

    public bool hasTalkedToFriendToday(string npcName) => false;

    public void talkToFriend(INPCWrapper n, int friendshipPointChange = 20)
    {
    }

    public void moveRaft(IGameLocationWrapper currentLocation, GameTime time)
    {
    }

    public void warpFarmer(IWarpWrapper w, int warp_collide_direction)
    {
    }

    public void warpFarmer(IWarpWrapper w)
    {
    }

    public void startToPassOut()
    {
    }

    public string getPetName() => null;

    public IPetWrapper getPet() => null;

    public string getPetDisplayName() => null;

    public bool hasPet() => false;

    public void UpdateClothing()
    {
    }

    public int GetPantsIndex() => 0;

    public int GetShirtIndex() => 0;

    public IEnumerable<string> GetShirtExtraData()
    {
      yield break;
    }

    public Color GetShirtColor() => default;

    public Color GetPantsColor() => default;

    public bool movedDuringLastTick() => false;

    public int CompareTo(object obj) => 0;

    public void SetOnBridge(bool val)
    {
    }

    public float getDrawLayer() => 0;

    public void DrawUsername(SpriteBatch b)
    {
    }

    public void handleDisconnect()
    {
    }

    public bool isDivorced() => false;

    public void wipeExMemories()
    {
    }

    public void getRidOfChildren()
    {
    }

    public void animateOnce(int whichAnimation)
    {
    }

    public void FireTool()
    {
    }

    public void synchronizedJump(float velocity)
    {
    }

    public void BeginUsingTool()
    {
    }

    public void EndUsingTool()
    {
    }

    public void checkForExhaustion(float oldStamina)
    {
    }

    public void setMoving(byte command)
    {
    }

    public void toolPowerIncrease()
    {
    }

    public void UpdateIfOtherPlayer(GameTime time)
    {
    }

    public void forceCanMove()
    {
    }

    public void dropItem(IItemWrapper i)
    {
    }

    public bool addEvent(string eventName, int daysActive) => false;

    public void dropObjectFromInventory(int parentSheetIndex, int quantity)
    {
    }

    public Vector2 getMostRecentMovementVector() => default;

    public void dropActiveItem()
    {
    }

    public int GetSkillLevel(int index) => 0;

    public int GetUnmodifiedSkillLevel(int index) => 0;

    public bool hasCompletedCommunityCenter() => false;

    public bool CanBeDamaged() => false;

    public void takeDamage(int damage, bool overrideParry, IMonsterWrapper damager)
    {
    }

    public int GetEffectsOfRingMultiplier(int ring_index) => 0;

    public bool checkAction(IFarmerWrapper who, IGameLocationWrapper location) => false;

    public void Update(GameTime time, IGameLocationWrapper location)
    {
    }

    public bool IsBusyDoingSomething() => false;

    public void UpdateItemStow()
    {
    }

    public void addQuest(int questID)
    {
    }

    public void removeQuest(int questID)
    {
    }

    public void completeQuest(int questID)
    {
    }

    public bool hasQuest(int id) => false;

    public bool hasNewQuestActivity() => false;

    public float getMovementSpeed() => 0;

    public bool isWearingRing(int ringIndex) => false;

    public void stopJittering()
    {
    }

    public Rectangle nextPositionHalf(int direction) => default;

    public int getProfessionForSkill(int skillType, int skillLevel) => 0;

    public void behaviorOnMovement(int direction)
    {
    }

    public void OnEmoteAnimationEnd(IFarmerWrapper farmer)
    {
    }

    public void EndEmoteAnimation()
    {
    }

    public void performKissFarmer(long otherPlayerID)
    {
    }

    public void PerformKiss(int facingDirection)
    {
    }

    public void updateMovementAnimation(GameTime time)
    {
    }

    public bool IsCarrying() => false;

    public void doneEating()
    {
    }

    public bool checkForQuestComplete(INPCWrapper n, int number1, int number2, IItemWrapper item, string str, int questType = -1,
      int questTypeToIgnore = -1) =>
      false;

    public void completelyStopAnimatingOrDoingAction()
    {
    }

    public void doEmote(int whichEmote)
    {
    }

    public void performTenMinuteUpdate()
    {
    }

    public void setRunning(bool isRunning, bool force = false)
    {
    }

    public void addSeenResponse(int id)
    {
    }

    public void eatObject(IObjectWrapper o, bool overrideFullness = false)
    {
    }

    public IFarmerWrapper CreateFakeEventFarmer() => null;

    public void netDoEmote(string emote_type)
    {
    }

    public void eatHeldObject()
    {
    }

    public void grabObject(IObjectWrapper obj)
    {
    }

    public void PlayFishBiteChime()
    {
    }

    public string getTitle() => null;

    public void queueMessage(byte messageType, IFarmerWrapper sourceFarmer, params object[] data)
    {
    }

    public void queueMessage(IOutgoingMessageWrapper message)
    {
    }
  }
}
