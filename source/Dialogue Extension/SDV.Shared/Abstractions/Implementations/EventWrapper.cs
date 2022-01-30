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
using Microsoft.Xna.Framework.Input;using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace SDV.Shared.Abstractions
{
  public class EventWrapper : IEventWrapper
  {
    public EventWrapper(Event item) => GetBaseType = item;
    public Event GetBaseType { get; }
    public int CurrentCommand { get; set; }
    public bool playerControlSequence { get; set; }
    public IFarmerWrapper farmer { get; }
    public Texture2D festivalTexture { get; }
    public string FestivalName { get; }
    public void setupEventCommands()
    {
    }

    public void tryEventCommand(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_ignoreEventTileOffset(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_move(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_speak(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_beginSimultaneousCommand(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_endSimultaneousCommand(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_minedeath(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_hospitaldeath(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_showItemsLost(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_end(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_locationSpecificCommand(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_unskippable(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_skippable(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_emote(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopMusic(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_playSound(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_tossConcession(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_pause(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_resetVariable(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_faceDirection(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_warp(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_speed(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopAdvancedMoves(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_doAction(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeTile(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_textAboveHead(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_showFrame(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_farmerAnimation(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_ignoreMovementAnimation(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_animate(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopAnimation(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_showRivalFrame(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_weddingSprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeLocation(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_halt(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_message(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addCookingRecipe(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_itemAboveHead(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addCraftingRecipe(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_hostMail(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_mail(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_shake(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_temporarySprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeTemporarySprites(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_null(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_specificTemporarySprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_playMusic(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_nameSelect(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_makeInvisible(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addObject(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addBigProp(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addFloorProp(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addProp(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addToTable(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeObject(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_glow(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopGlowing(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addQuest(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeQuest(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_awardFestivalPrize(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_attachCharacterToTempSprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_fork(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_switchEvent(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_globalFade(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_globalFadeToClear(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_cutscene(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_grabObject(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addTool(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_waitForTempSprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_waitForKey(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_cave(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_updateMinigame(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_startJittering(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_money(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopJittering(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addLantern(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_rustyKey(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_swimming(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopSwimming(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_tutorialMenu(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_animalNaming(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_splitSpeak(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_catQuestion(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_ambientLight(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_bgColor(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_bloom(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_elliottbooktalk(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeItem(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_friendship(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_setRunning(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_extendSourceRect(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_waitForOtherPlayers(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_requestMovieEnd(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_restoreStashedItem(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_advancedMove(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_stopRunning(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_eyes(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addMailReceived(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addWorldState(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_fade(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeMapTile(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeSprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_waitForAllStationary(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_proceedPosition(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changePortrait(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeYSourceRectOffset(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeName(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_playFramesAhead(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_showKissFrame(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addTemporaryActor(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_changeToTemporaryMap(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_positionOffset(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_question(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_quickQuestion(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_drawOffset(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_hideShadow(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_animateHeight(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_jump(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_farmerEat(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_spriteText(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_ignoreCollisions(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_screenFlash(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_grandpaCandles(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_grandpaEvaluation2(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_grandpaEvaluation(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_loadActors(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_playerControl(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_removeSprite(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_viewport(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_broadcastEvent(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_addConversationTopic(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public void command_dump(IGameLocationWrapper location, GameTime time, string[] split)
    {
    }

    public bool tryToLoadFestival(string festival) => false;

    public string GetFestivalDataForYear(string key) => null;

    public void setExitLocation(string location, int x, int y)
    {
    }

    public void endBehaviors(string[] split, IGameLocationWrapper location)
    {
    }

    public void exitEvent()
    {
    }

    public void resetDialogueIfNecessary(INPCWrapper n)
    {
    }

    public void incrementCommandAfterFade()
    {
    }

    public void cleanup()
    {
    }

    public void LogErrorAndHalt(Exception e)
    {
    }

    public void checkForNextCommand(IGameLocationWrapper location, GameTime time)
    {
    }

    public bool isTileWalkedOn(int x, int y) => false;

    public INPCWrapper getActorByName(string name) => null;

    public void applyToAllFarmersByFarmerString(string farmer_string, Action<IFarmerWrapper> function)
    {
    }

    public IFarmerWrapper getFarmerFromFarmerNumberString(string name, IFarmerWrapper defaultFarmer) => null;

    public ICharacterWrapper getCharacterByName(string name) => null;

    public Vector3 getPositionAfterMove(ICharacterWrapper c, int xMove, int yMove, int facingDirection) => default;

    public Vector2 OffsetPosition(Vector2 original) => default;

    public Vector2 OffsetTile(Vector2 original) => default;

    public float OffsetPositionX(float original) => 0;

    public float OffsetPositionY(float original) => 0;

    public int OffsetTileX(int original) => 0;

    public int OffsetTileY(int original) => 0;

    public void receiveMouseClick(int x, int y)
    {
    }

    public void skipEvent()
    {
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void receiveActionPress(int xTile, int yTile)
    {
    }

    public void startSecretSantaEvent()
    {
    }

    public void festivalUpdate(GameTime time)
    {
    }

    public void drawFarmers(SpriteBatch b)
    {
    }

    public bool ShouldHideCharacter(INPCWrapper n) => false;

    public void draw(SpriteBatch b)
    {
    }

    public void drawUnderWater(SpriteBatch b)
    {
    }

    public void drawAfterMap(SpriteBatch b)
    {
    }

    public void EndPlayerControlSequence()
    {
    }

    public void OnPlayerControlSequenceEnd(string id)
    {
    }

    public void setUpPlayerControlSequence(string id)
    {
    }

    public bool canMoveAfterDialogue() => false;

    public void forceFestivalContinue()
    {
    }

    public bool isSpecificFestival(string festivalID) => false;

    public void setUpFestivalMainEvent()
    {
    }

    public void interpretGrangeResults()
    {
    }

    public void answerDialogueQuestion(INPCWrapper who, string answerKey)
    {
    }

    public void addItemToGrangeDisplay(IItemWrapper i, int position, bool force)
    {
    }

    public bool canPlayerUseTool() => false;

    public bool checkAction(Location tileLocation, Rectangle viewport, IFarmerWrapper who) => false;

    public void removeFestivalProps(Microsoft.Xna.Framework.Rectangle rect)
    {
    }

    public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
    {
    }

    public void forceEndFestival(IFarmerWrapper who)
    {
    }

    public bool checkForCollision(Microsoft.Xna.Framework.Rectangle position, IFarmerWrapper who) => false;

    public void answerDialogue(string questionKey, int answerChoice)
    {
    }

    public void chooseSecretSantaGift(IItemWrapper i, IFarmerWrapper who)
    {
    }

    public void perfectFishing()
    {
    }

    public void caughtFish(int whichFish, int size, IFarmerWrapper who)
    {
    }

    public void readFortune()
    {
    }

    public void fadeClearAndviewportUnfreeze()
    {
    }

    public void betStarTokens(int value, int price, IFarmerWrapper who)
    {
    }

    public void buyStarTokens(int value, int price, IFarmerWrapper who)
    {
    }

    public void clickToAddItemToLuauSoup(IItemWrapper i, IFarmerWrapper who)
    {
    }

    public void setUpAdvancedMove(string[] split, NPCControllerWrapper.endBehavior endBehavior = null)
    {
    }

    public void addItemToLuauSoup(IItemWrapper i, IFarmerWrapper who)
    {
    }
  }
}
