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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using xTile.Dimensions;

namespace SDV.Shared.Abstractions
{
  public interface IEventWrapper : IWrappedType<Event>
  {
    int CurrentCommand { get; set; }
    bool playerControlSequence { get; set; }
    IFarmerWrapper farmer { get; }
    Texture2D festivalTexture { get; }
    string FestivalName { get; }
    void setupEventCommands();
    void tryEventCommand(IGameLocationWrapper location, GameTime time, string[] split);

    void command_ignoreEventTileOffset(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_move(IGameLocationWrapper location, GameTime time, string[] split);
    void command_speak(IGameLocationWrapper location, GameTime time, string[] split);

    void command_beginSimultaneousCommand(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_endSimultaneousCommand(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_minedeath(IGameLocationWrapper location, GameTime time, string[] split);
    void command_hospitaldeath(IGameLocationWrapper location, GameTime time, string[] split);
    void command_showItemsLost(IGameLocationWrapper location, GameTime time, string[] split);
    void command_end(IGameLocationWrapper location, GameTime time, string[] split);

    void command_locationSpecificCommand(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_unskippable(IGameLocationWrapper location, GameTime time, string[] split);
    void command_skippable(IGameLocationWrapper location, GameTime time, string[] split);
    void command_emote(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopMusic(IGameLocationWrapper location, GameTime time, string[] split);
    void command_playSound(IGameLocationWrapper location, GameTime time, string[] split);

    void command_tossConcession(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_pause(IGameLocationWrapper location, GameTime time, string[] split);
    void command_resetVariable(IGameLocationWrapper location, GameTime time, string[] split);
    void command_faceDirection(IGameLocationWrapper location, GameTime time, string[] split);
    void command_warp(IGameLocationWrapper location, GameTime time, string[] split);
    void command_speed(IGameLocationWrapper location, GameTime time, string[] split);

    void command_stopAdvancedMoves(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_doAction(IGameLocationWrapper location, GameTime time, string[] split);
    void command_removeTile(IGameLocationWrapper location, GameTime time, string[] split);
    void command_textAboveHead(IGameLocationWrapper location, GameTime time, string[] split);
    void command_showFrame(IGameLocationWrapper location, GameTime time, string[] split);

    void command_farmerAnimation(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_ignoreMovementAnimation(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_animate(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopAnimation(IGameLocationWrapper location, GameTime time, string[] split);

    void command_showRivalFrame(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_weddingSprite(IGameLocationWrapper location, GameTime time, string[] split);

    void command_changeLocation(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_halt(IGameLocationWrapper location, GameTime time, string[] split);
    void command_message(IGameLocationWrapper location, GameTime time, string[] split);

    void command_addCookingRecipe(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_itemAboveHead(IGameLocationWrapper location, GameTime time, string[] split);

    void command_addCraftingRecipe(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_hostMail(IGameLocationWrapper location, GameTime time, string[] split);
    void command_mail(IGameLocationWrapper location, GameTime time, string[] split);
    void command_shake(IGameLocationWrapper location, GameTime time, string[] split);

    void command_temporarySprite(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_removeTemporarySprites(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_null(IGameLocationWrapper location, GameTime time, string[] split);

    void command_specificTemporarySprite(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_playMusic(IGameLocationWrapper location, GameTime time, string[] split);
    void command_nameSelect(IGameLocationWrapper location, GameTime time, string[] split);
    void command_makeInvisible(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addObject(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addBigProp(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addFloorProp(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addProp(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addToTable(IGameLocationWrapper location, GameTime time, string[] split);
    void command_removeObject(IGameLocationWrapper location, GameTime time, string[] split);
    void command_glow(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopGlowing(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addQuest(IGameLocationWrapper location, GameTime time, string[] split);
    void command_removeQuest(IGameLocationWrapper location, GameTime time, string[] split);

    void command_awardFestivalPrize(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_attachCharacterToTempSprite(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_fork(IGameLocationWrapper location, GameTime time, string[] split);
    void command_switchEvent(IGameLocationWrapper location, GameTime time, string[] split);
    void command_globalFade(IGameLocationWrapper location, GameTime time, string[] split);

    void command_globalFadeToClear(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_cutscene(IGameLocationWrapper location, GameTime time, string[] split);
    void command_grabObject(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addTool(IGameLocationWrapper location, GameTime time, string[] split);

    void command_waitForTempSprite(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_waitForKey(IGameLocationWrapper location, GameTime time, string[] split);
    void command_cave(IGameLocationWrapper location, GameTime time, string[] split);

    void command_updateMinigame(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_startJittering(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_money(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopJittering(IGameLocationWrapper location, GameTime time, string[] split);
    void command_addLantern(IGameLocationWrapper location, GameTime time, string[] split);
    void command_rustyKey(IGameLocationWrapper location, GameTime time, string[] split);
    void command_swimming(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopSwimming(IGameLocationWrapper location, GameTime time, string[] split);
    void command_tutorialMenu(IGameLocationWrapper location, GameTime time, string[] split);
    void command_animalNaming(IGameLocationWrapper location, GameTime time, string[] split);
    void command_splitSpeak(IGameLocationWrapper location, GameTime time, string[] split);
    void command_catQuestion(IGameLocationWrapper location, GameTime time, string[] split);
    void command_ambientLight(IGameLocationWrapper location, GameTime time, string[] split);
    void command_bgColor(IGameLocationWrapper location, GameTime time, string[] split);
    void command_bloom(IGameLocationWrapper location, GameTime time, string[] split);

    void command_elliottbooktalk(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_removeItem(IGameLocationWrapper location, GameTime time, string[] split);
    void command_friendship(IGameLocationWrapper location, GameTime time, string[] split);
    void command_setRunning(IGameLocationWrapper location, GameTime time, string[] split);

    void command_extendSourceRect(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_waitForOtherPlayers(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_requestMovieEnd(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_restoreStashedItem(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_advancedMove(IGameLocationWrapper location, GameTime time, string[] split);
    void command_stopRunning(IGameLocationWrapper location, GameTime time, string[] split);
    void command_eyes(IGameLocationWrapper location, GameTime time, string[] split);

    void command_addMailReceived(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_addWorldState(IGameLocationWrapper location, GameTime time, string[] split);
    void command_fade(IGameLocationWrapper location, GameTime time, string[] split);
    void command_changeMapTile(IGameLocationWrapper location, GameTime time, string[] split);
    void command_changeSprite(IGameLocationWrapper location, GameTime time, string[] split);

    void command_waitForAllStationary(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_proceedPosition(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_changePortrait(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_changeYSourceRectOffset(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_changeName(IGameLocationWrapper location, GameTime time, string[] split);

    void command_playFramesAhead(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_showKissFrame(IGameLocationWrapper location, GameTime time, string[] split);

    void command_addTemporaryActor(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_changeToTemporaryMap(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_positionOffset(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_question(IGameLocationWrapper location, GameTime time, string[] split);
    void command_quickQuestion(IGameLocationWrapper location, GameTime time, string[] split);
    void command_drawOffset(IGameLocationWrapper location, GameTime time, string[] split);
    void command_hideShadow(IGameLocationWrapper location, GameTime time, string[] split);
    void command_animateHeight(IGameLocationWrapper location, GameTime time, string[] split);
    void command_jump(IGameLocationWrapper location, GameTime time, string[] split);
    void command_farmerEat(IGameLocationWrapper location, GameTime time, string[] split);
    void command_spriteText(IGameLocationWrapper location, GameTime time, string[] split);

    void command_ignoreCollisions(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_screenFlash(IGameLocationWrapper location, GameTime time, string[] split);

    void command_grandpaCandles(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_grandpaEvaluation2(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_grandpaEvaluation(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_loadActors(IGameLocationWrapper location, GameTime time, string[] split);
    void command_playerControl(IGameLocationWrapper location, GameTime time, string[] split);
    void command_removeSprite(IGameLocationWrapper location, GameTime time, string[] split);
    void command_viewport(IGameLocationWrapper location, GameTime time, string[] split);

    void command_broadcastEvent(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_addConversationTopic(
      IGameLocationWrapper location,
      GameTime time,
      string[] split);

    void command_dump(IGameLocationWrapper location, GameTime time, string[] split);
    bool tryToLoadFestival(string festival);
    string GetFestivalDataForYear(string key);
    void setExitLocation(string location, int x, int y);
    void endBehaviors(string[] split, IGameLocationWrapper location);
    void exitEvent();
    void resetDialogueIfNecessary(INPCWrapper n);
    void incrementCommandAfterFade();
    void cleanup();
    void LogErrorAndHalt(Exception e);
    void checkForNextCommand(IGameLocationWrapper location, GameTime time);
    bool isTileWalkedOn(int x, int y);
    INPCWrapper getActorByName(string name);
    void applyToAllFarmersByFarmerString(string farmer_string, Action<IFarmerWrapper> function);
    IFarmerWrapper getFarmerFromFarmerNumberString(string name, IFarmerWrapper defaultFarmer);
    ICharacterWrapper getCharacterByName(string name);

    Vector3 getPositionAfterMove(
      ICharacterWrapper c,
      int xMove,
      int yMove,
      int facingDirection);

    Vector2 OffsetPosition(Vector2 original);
    Vector2 OffsetTile(Vector2 original);
    float OffsetPositionX(float original);
    float OffsetPositionY(float original);
    int OffsetTileX(int original);
    int OffsetTileY(int original);
    void receiveMouseClick(int x, int y);
    void skipEvent();
    void receiveKeyPress(Keys k);
    void receiveKeyRelease(Keys k);
    void receiveActionPress(int xTile, int yTile);
    void startSecretSantaEvent();
    void festivalUpdate(GameTime time);
    void drawFarmers(SpriteBatch b);
    bool ShouldHideCharacter(INPCWrapper n);
    void draw(SpriteBatch b);
    void drawUnderWater(SpriteBatch b);
    void drawAfterMap(SpriteBatch b);
    void EndPlayerControlSequence();
    void OnPlayerControlSequenceEnd(string id);
    void setUpPlayerControlSequence(string id);
    bool canMoveAfterDialogue();
    void forceFestivalContinue();
    bool isSpecificFestival(string festivalID);
    void setUpFestivalMainEvent();
    void interpretGrangeResults();
    void answerDialogueQuestion(INPCWrapper who, string answerKey);
    void addItemToGrangeDisplay(IItemWrapper i, int position, bool force);
    bool canPlayerUseTool();
    bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who);
    void removeFestivalProps(Microsoft.Xna.Framework.Rectangle rect);
    void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation);
    void forceEndFestival(IFarmerWrapper who);
    bool checkForCollision(Microsoft.Xna.Framework.Rectangle position, IFarmerWrapper who);
    void answerDialogue(string questionKey, int answerChoice);
    void chooseSecretSantaGift(IItemWrapper i, IFarmerWrapper who);
    void perfectFishing();
    void caughtFish(int whichFish, int size, IFarmerWrapper who);
    void readFortune();
    void fadeClearAndviewportUnfreeze();
    void betStarTokens(int value, int price, IFarmerWrapper who);
    void buyStarTokens(int value, int price, IFarmerWrapper who);
    void clickToAddItemToLuauSoup(IItemWrapper i, IFarmerWrapper who);
    void setUpAdvancedMove(string[] split, NPCControllerWrapper.endBehavior endBehavior = null);
    void addItemToLuauSoup(IItemWrapper i, IFarmerWrapper who);
  }
}