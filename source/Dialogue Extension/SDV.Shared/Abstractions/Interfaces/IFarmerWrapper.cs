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
using StardewValley.Menus;

namespace SDV.Shared.Abstractions
{
  public interface IFarmerWrapper : ICharacterWrapper
  {
    IEnumerable<IItemWrapper> Items { get; set; }
    IEnumerable<int> DialogueQuestionsAnswered { get; }
    long UniqueMultiplayerID { get; set; }
    IFarmerRendererWrapper FarmerRenderer { get; set; }
    int CurrentToolIndex { get; set; }
    IItemWrapper TemporaryItem { get; set; }
    IItemWrapper CursorSlotItem { get; set; }
    int WoodPieces { get; set; }
    int StonePieces { get; set; }
    int CopperPieces { get; set; }
    int IronPieces { get; set; }
    int CoalPieces { get; set; }
    int GoldPieces { get; set; }
    int IridiumPieces { get; set; }
    int QuartzPieces { get; set; }
    int Feed { get; set; }
    int FarmingLevel { get; set; }
    int MiningLevel { get; set; }
    int CombatLevel { get; set; }
    int ForagingLevel { get; set; }
    int FishingLevel { get; set; }
    int LuckLevel { get; set; }
    int NewSkillPointsToSpend { get; set; }
    int MaxStamina { get; set; }
    int MaxItems { get; set; }
    int HouseUpgradeLevel { get; set; }
    int CoopUpgradeLevel { get; set; }
    int BarnUpgradeLevel { get; set; }
    bool HasTownKey { get; set; }
    int MagneticRadius { get; set; }
    int CraftingTime { get; set; }
    bool IsMale { get; set; }
    bool CanMove { get; set; }
    bool UsingTool { get; set; }
    IBoundingBoxGroupWrapper TemporaryPassableTiles { get; set; }
    int visibleQuestCount { get; }
    IItemWrapper recoveredItem { get; set; }
    long theaterBuildDate { get; set; }
    int festivalScore { get; set; }
    int deepestMineLevel { get; set; }
    float stamina { get; set; }
    float Stamina { get; set; }
    IFarmerTeamWrapper team { get; }
    uint totalMoneyEarned { get; set; }
    ulong millisecondsPlayed { get; set; }
    bool hasRustyKey { get; set; }
    bool hasSkullKey { get; set; }
    bool canUnderstandDwarves { get; set; }
    bool hasPendingCompletedQuests { get; }
    bool useSeparateWallets { get; set; }
    int timesReachedMineBottom { get; set; }
    string spouse { get; set; }
    bool isUnclaimedFarmhand { get; }
    IHorseWrapper mount { get; set; }
    int Level { get; }
    double DailyLuck { get; }
    IObjectWrapper ActiveObject { get; set; }
    IToolWrapper CurrentTool { get; set; }
    IItemWrapper CurrentItem { get; }
    bool IsLocalPlayer { get; }
    bool IsMainPlayer { get; }
    IFarmerSpriteWrapper FarmerSprite { get; set; }
    int _money { get; set; }
    int QiGems { get; set; }
    int Money { get; set; }
    bool IsSitting();
    void LerpPosition(Vector2 start_position, Vector2 end_position, float duration);
    void addUnearnedMoney(int money);
    NetStringList GetEmoteFavorites();
    bool CanEmote();
    void performRenovation(string location_name);
    void performPlayerEmote(string emote_string);
    bool ShouldHandleAnimationSound();
    Point getMailboxPosition();
    void ClearBuffs();
    void addBuffAttributes(int[] buffAttributes);
    void removeBuffAttributes(int[] buffAttributes);
    void removeBuffAttributes();
    bool isActive();
    string getTexture();
    void checkForLevelTenStatus();
    void unload();
    void setInventory(IEnumerable<IItemWrapper> newInventory);
    void makeThisTheActiveObject(IObjectWrapper o);
    int getNumberOfChildren();
    bool isRidingHorse();
    IEnumerable<IChildWrapper> getChildren();
    int getChildrenCount();
    IToolWrapper getToolFromName(string name);
    int? tryGetFriendshipLevelForNPC(string name);
    int getFriendshipLevelForNPC(string name);
    int getFriendshipHeartLevelForNPC(string name);
    bool isRoommate(string name);
    bool hasCurrentOrPendingRoommate();
    bool hasRoommate();
    bool hasAFriendWithHeartLevel(int heartLevel, bool datablesOnly);
    int getTallyOfObject(int index, bool bigCraftable);
    bool areAllItemsNull();
    void shippedBasic(int index, int number);
    void shiftToolbar(bool right);
    void foundWalnut(int stack = 1);
    void RemoveMail(string mail_key, bool from_broadcast_list = false);
    void showNutPickup();
    void foundArtifact(int index, int number);
    void cookedRecipe(int index);
    bool caughtFish(int index, int size, bool from_fish_pond = false, int numberCaught = 1);
    void gainExperience(int which, int howMuch);
    int getEffectiveSkillLevel(int whichSkill);
    void revealGiftTaste(INPCWrapper npc, int parent_sheet_index);
    void revealGiftTaste(INPCWrapper npc, IObjectWrapper item);
    void onGiftGiven(INPCWrapper npc, IObjectWrapper item);
    bool hasGiftTasteBeenRevealed(INPCWrapper npc, int item_index);
    bool hasItemBeenGifted(INPCWrapper npc, int item_index);
    void MarkItemAsTailored(IItemWrapper item);
    bool HasTailoredThisItem(IItemWrapper item);
    void foundMineral(int index);
    void increaseBackpackSize(int howMuch);
    void consumeObject(int index, int quantity);
    int getItemCount(int item_index, int min_price = 0);
    bool hasItemInInventory(int itemIndex, int quantity, int minPrice = 0);
    bool hasItemInInventoryNamed(string name);
    int getItemCountInList(IEnumerable<IItemWrapper> list, int item_index, int min_price = 0);
    bool hasItemInList(IEnumerable<IItemWrapper> list, int itemIndex, int quantity, int minPrice = 0);

    void addItemByMenuIfNecessaryElseHoldUp(
      IItemWrapper item,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null);

    void addItemByMenuIfNecessary(
      IItemWrapper item,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null);

    void addItemsByMenuIfNecessary(
      IEnumerable<IItemWrapper> itemsToAdd,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null);

    void ShowSitting();
    void showRiding();
    void showCarrying();
    void showNotCarrying();
    int GetDaysMarried();
    IFriendshipWrapper GetSpouseFriendship();
    bool hasDailyQuest();
    void showToolUpgradeAvailability();
    void dayupdate();
    void doDivorce();
    bool hasBuff(int whichBuff);
    bool hasOrWillReceiveMail(string id);
    void holdUpItemThenMessage(IItemWrapper item, bool showMessage = true);
    void resetState();
    void resetItemStates();
    void clearBackpack();
    int numberOfItemsInInventory();
    void resetFriendshipsForNewDay();
    int GetAppliedMagneticRadius();
    void updateFriendshipGifts(IWorldDateWrapper date);
    bool hasPlayerTalkedToNPC(string name);
    void fuelLantern(int units);

    bool tryToCraftItem(
      IEnumerable<int[]> ingredients,
      double successRate,
      int itemToCraft,
      bool bigCraftable,
      string craftingOrCooking);

    bool IsEquippedItem(IItemWrapper item);
    bool collideWith(IObjectWrapper o);
    void changeIntoSwimsuit();
    void changeOutOfSwimSuit();
    bool ownsFurniture(string name);
    void showFrame(int frame, bool flip = false);
    void stopShowingFrame();
    IItemWrapper addItemToInventory(IItemWrapper item, IEnumerable<IItemWrapper> affected_items_list);
    IItemWrapper addItemToInventory(IItemWrapper item);
    IItemWrapper addItemToInventory(IItemWrapper item, int position);
    void BeginSitting(ISittableWrapper furniture);
    void StopSitting(bool animate = true);
    void SortSeatExitPositions(IEnumerable<Vector2> list, Vector2 a, Vector2 b, Vector2 c);
    bool isInventoryFull();
    bool couldInventoryAcceptThisItem(IItemWrapper item);
    bool couldInventoryAcceptThisObject(int index, int stack, int quality = 0);
    bool hasItemOfType(string type);
    INPCWrapper getSpouse();
    int freeSpotsInInventory();
    IItemWrapper hasItemWithNameThatContains(string name);
    bool addItemToInventoryBool(IItemWrapper item, bool makeActiveObject = false);
    int getIndexOfInventoryItem(IItemWrapper item);
    void reduceActiveItemByOne();
    bool removeItemsFromInventory(int index, int stack);
    void ReequipEnchantments();
    void removeItemFromInventory(IItemWrapper which);
    IItemWrapper removeItemFromInventory(int whichItemIndex);
    bool isMarried();
    bool isEngaged();
    void removeFirstOfThisItemFromInventory(int parentSheetIndexOfItem);
    void changeShirt(int whichShirt, bool is_customization_screen = false);
    void changePantStyle(int whichPants, bool is_customization_screen = false);
    void ConvertClothingOverrideToClothesItems();
    void changeHairStyle(int whichHair);
    bool IsBaldHairStyle(int style);
    void changeShoeColor(int which);
    void changeHairColor(Color c);
    void changePants(Color color);
    void changeHat(int newHat);
    void changeAccessory(int which);
    void changeSkinColor(int which, bool force = false);
    bool hasDarkSkin();
    void changeEyeColor(Color c);
    int getHair(bool ignore_hat = false);
    void changeGender(bool male);
    void changeFriendship(int amount, INPCWrapper n);
    bool knowsRecipe(string name);
    Vector2 getUniformPositionAwayFromBox(int direction, int distance);
    bool hasTalkedToFriendToday(string npcName);
    void talkToFriend(INPCWrapper n, int friendshipPointChange = 20);
    void moveRaft(IGameLocationWrapper currentLocation, GameTime time);
    void warpFarmer(IWarpWrapper w, int warp_collide_direction);
    void warpFarmer(IWarpWrapper w);
    void startToPassOut();
    string getPetName();
    IPetWrapper getPet();
    string getPetDisplayName();
    bool hasPet();
    void UpdateClothing();
    int GetPantsIndex();
    int GetShirtIndex();
    IEnumerable<string> GetShirtExtraData();
    Color GetShirtColor();
    Color GetPantsColor();
    bool movedDuringLastTick();
    int CompareTo(object obj);
    void SetOnBridge(bool val);
    float getDrawLayer();
    void DrawUsername(SpriteBatch b);
    void handleDisconnect();
    bool isDivorced();
    void wipeExMemories();
    void getRidOfChildren();
    void animateOnce(int whichAnimation);
    void FireTool();
    void synchronizedJump(float velocity);
    void BeginUsingTool();
    void EndUsingTool();
    void checkForExhaustion(float oldStamina);
    void setMoving(byte command);
    void toolPowerIncrease();
    void UpdateIfOtherPlayer(GameTime time);
    void forceCanMove();
    void dropItem(IItemWrapper i);
    bool addEvent(string eventName, int daysActive);
    void dropObjectFromInventory(int parentSheetIndex, int quantity);
    Vector2 getMostRecentMovementVector();
    void dropActiveItem();
    int GetSkillLevel(int index);
    int GetUnmodifiedSkillLevel(int index);
    bool hasCompletedCommunityCenter();
    bool CanBeDamaged();
    void takeDamage(int damage, bool overrideParry, IMonsterWrapper damager);
    int GetEffectsOfRingMultiplier(int ring_index);
    bool checkAction(IFarmerWrapper who, IGameLocationWrapper location);
    void Update(GameTime time, IGameLocationWrapper location);
    bool IsBusyDoingSomething();
    void UpdateItemStow();
    void addQuest(int questID);
    void removeQuest(int questID);
    void completeQuest(int questID);
    bool hasQuest(int id);
    bool hasNewQuestActivity();
    float getMovementSpeed();
    bool isWearingRing(int ringIndex);
    void stopJittering();
    Rectangle nextPositionHalf(int direction);
    int getProfessionForSkill(int skillType, int skillLevel);
    void behaviorOnMovement(int direction);
    void OnEmoteAnimationEnd(IFarmerWrapper farmer);
    void EndEmoteAnimation();
    void performKissFarmer(long otherPlayerID);
    void PerformKiss(int facingDirection);

    void updateMovementAnimation(GameTime time);
    bool IsCarrying();
    void doneEating();

    bool checkForQuestComplete(
      INPCWrapper n,
      int number1,
      int number2,
      IItemWrapper item,
      string str,
      int questType = -1,
      int questTypeToIgnore = -1);

    void completelyStopAnimatingOrDoingAction();
    void doEmote(int whichEmote);
    void performTenMinuteUpdate();
    void setRunning(bool isRunning, bool force = false);
    void addSeenResponse(int id);
    void eatObject(IObjectWrapper o, bool overrideFullness = false);
    IFarmerWrapper CreateFakeEventFarmer();
    void netDoEmote(string emote_type);
    void eatHeldObject();
    void grabObject(IObjectWrapper obj);
    void PlayFishBiteChime();
    string getTitle();
    void queueMessage(byte messageType, IFarmerWrapper sourceFarmer, params object[] data);
    void queueMessage(IOutgoingMessageWrapper message);
  }
}