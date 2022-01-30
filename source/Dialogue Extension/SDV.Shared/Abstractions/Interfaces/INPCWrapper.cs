/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace SDV.Shared.Abstractions
{
  public interface INPCWrapper : ICharacterWrapper
  {
    IDictionary<int, ISchedulePathDescriptionWrapper> Schedule { get; set; }
    IDictionary<string, string> Dialogue { get; }
    ISchedulePathDescriptionWrapper DirectionsToNewLocation { get; set; }
    int DirectionIndex { get; set; }
    Texture2D Portrait { get; set; }
    bool IsWalkingInSquare { get; set; }
    bool IsWalkingTowardPlayer { get; set; }
    string Birthday_Season { get; set; }
    int Birthday_Day { get; set; }
    int Age { get; set; }
    int Manners { get; set; }
    int SocialAnxiety { get; set; }
    int Optimism { get; set; }
    int Gender { get; set; }
    bool Breather { get; set; }
    bool HideShadow { get; set; }
    bool IsInvisible { get; set; }
    int DefaultFacingDirection { get; set; }
    Vector2 DefaultPosition { get; set; }
    string DefaultMap { get; set; }
    Stack<IDialogueWrapper> CurrentDialogue { get; set; }
    bool HasPartnerForDance { get; }
    bool CanSocialize { get; }
    void reloadData();
    void reloadDefaultLocation();
    bool canTalk();
    string getName();
    string getTextureName();

    bool PathToOnFarm(
      Point destination,
      PathFindControllerWrapper.endBehavior on_path_success = null);

    void OnFinishPathForActivity(ICharacterWrapper c, IGameLocationWrapper location);
    void resetPortrait();
    void resetSeasonalDialogue();
    void reloadSprite();

    void showTextAboveHead(
      string Text,
      int spriteTextColor = -1,
      int style = 2,
      int duration = 3000,
      int preTimer = 0);

    void moveToNewPlaceForEvent(int xTile, int yTile, string oldMap);
    bool hitWithTool(IToolWrapper t);
    bool canReceiveThisItemAsGift(IItemWrapper i);
    int getGiftTasteForThisItem(IItemWrapper item);
    bool CheckTasteContextTags(IItemWrapper item, string[] list);
    void tryToReceiveActiveObject(IFarmerWrapper who);
    string GetDispositionModifiedString(string path, params object[] substitutions);
    void haltMe(IFarmerWrapper who);
    bool checkAction(IFarmerWrapper who, IGameLocationWrapper l);
    void grantConversationFriendship(IFarmerWrapper who, int amount = 20);
    void AskLeoMemoryPrompt();
    bool CanRevisitLeoMemory(KeyValuePair<string, int>? event_data);
    KeyValuePair<string, int>? GetUnseenLeoEvent();
    void OnLeoMemoryResponse(IFarmerWrapper who, string whichAnswer);
    bool isDivorcedFrom(IFarmerWrapper who);


    IGameLocationWrapper getHome();
    void behaviorOnFarmerPushing();
    void behaviorOnFarmerLocationEntry(IGameLocationWrapper location, IFarmerWrapper who);
    void behaviorOnLocalFarmerLocationEntry(IGameLocationWrapper location);
    void facePlayer(IFarmerWrapper who);
    void doneFacingPlayer(IFarmerWrapper who);
    void UpdateFarmExploration(GameTime time, IGameLocationWrapper location);
    void InitializeFarmActivities();
    bool FindFarmActivity();
    void wearIslandAttire();
    void wearNormalClothes();
    void performTenMinuteUpdate(int timeOfDay, IGameLocationWrapper l);
    void sayHiTo(ICharacterWrapper c);
    string getHi(string nameToGreet);
    bool isFacingToward(Vector2 tileLocation);
    void arriveAt(IGameLocationWrapper l);
    void addExtraDialogues(string dialogues);
    void PerformDivorce();

    string tryToGetMarriageSpecificDialogueElseReturnDefault(
      string dialogueKey,
      string defaultMessage = "");

    void resetCurrentDialogue();
    bool checkForNewCurrentDialogue(int heartLevel, bool noPreface = false);

    IDialogueWrapper tryToRetrieveDialogue(
      string preface,
      int heartLevel,
      string appendToEnd = "");

    void clearSchedule();
    void checkSchedule(int timeOfDay);
    void checkForMarriageDialogue(int timeOfDay, IGameLocationWrapper location);
    bool isOnSilentTemporaryMessage();
    bool hasTemporaryMessageAvailable();
    bool setTemporaryMessages(IFarmerWrapper who);
    void playSleepingAnimation();
    bool IsReturningToEndPoint();
    void StartActivityWalkInSquare(int square_width, int square_height, int pause_offset);
    void EndActivityRouteEndBehavior();
    void StartActivityRouteEndBehavior(string behavior_name, string end_message);
    void warp(bool wasOutdoors);
    void shake(int duration);
    void setNewDialogue(string s, bool add = false, bool clearOnMovement = false);

    void setNewDialogue(
      string dialogueSheetName,
      string dialogueSheetKey,
      int numberToAppend = -1,
      bool add = false,
      bool clearOnMovement = false);

    string GetDialogueSheetName();
    void setSpouseRoomMarriageDialogue();

    void setRandomAfternoonMarriageDialogue(
      int time,
      IGameLocationWrapper location,
      bool countAsDailyAfternoon = false);

    bool isBirthday(string season, int day);
    IObjectWrapper getFavoriteItem();

    void receiveGift(
      IObjectWrapper o,
      IFarmerWrapper giver,
      bool updateGiftLimitInfo = true,
      float friendshipChangeMultiplier = 1f,
      bool showResponse = true);

    bool NeedsBirdieEmoteHack();
    void warpToPathControllerDestination();
    Rectangle getMugShotSourceRect();
    void getHitByPlayer(IFarmerWrapper who, IGameLocationWrapper location);
    void walkInSquare(int squareWidth, int squareHeight, int squarePauseOffset);
    void moveTowardPlayer(int threshold);
    bool withinPlayerThreshold();
    bool withinPlayerThreshold(int threshold);

    IDictionary<int, ISchedulePathDescriptionWrapper> parseMasterSchedule(
      string rawData);

    IDictionary<int, ISchedulePathDescriptionWrapper> getSchedule(
      int dayOfMonth);

    void handleMasterScheduleFileLoadError(Exception e);
    void InvalidateMasterSchedule();
    IDictionary<string, string> getMasterScheduleRawData();
    string getMasterScheduleEntry(string schedule_key);
    bool hasMasterScheduleEntry(string key);
    bool isRoommate();
    bool isMarried();
    bool isMarriedOrEngaged();
    void dayUpdate(int dayOfMonth);
    void resetForNewDay(int dayOfMonth);
    void returnHomeFromFarmPosition(IFarmWrapper farm);
    Vector2 GetSpousePatioPosition();
    void setUpForOutdoorPatioActivity();
    bool isGaySpouse();
    bool canGetPregnant();
    void marriageDuties();
    void popOffAnyNonEssentialItems();

    void addMarriageDialogue(
      string dialogue_file,
      string dialogue_key,
      bool gendered = false,
      params string[] substitutions);

    void clearTextAboveHead();
    bool isVillager();
    void arriveAtFarmHouse(IFarmHouseWrapper farmHouse);
    IFarmerWrapper getSpouse();
    string getTermOfSpousalEndearment(bool happy = true);

    bool spouseObstacleCheck(
      MarriageDialogueReference backToBedMessage,
      IGameLocationWrapper currentLocation,
      bool force = false);

    void setTilePosition(Point p);
    void setTilePosition(int x, int y);
    void ReachedEndPoint();
    void changeSchedulePathDirection();
    void moveCharacterOnSchedulePath();
    void randomSquareMovement(GameTime time);
    void returnToEndPoint();
    void SetMovingOnlyUp();
    void SetMovingOnlyRight();
    void SetMovingOnlyDown();
    void SetMovingOnlyLeft();
    int CompareTo(object obj);
    void Removed();
  }
}