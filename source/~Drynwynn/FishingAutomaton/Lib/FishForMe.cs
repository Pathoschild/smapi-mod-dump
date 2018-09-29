using DsStardewLib.Utils;
using FishingAutomaton.Lib.HarmonyHacks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FishingAutomaton.Lib
{
  class FishForMe
  {
    /*******************
    ** PUBLIC PROPS/VARS
    *******************/

    /********************
    ** PRIVATE PROPS/VARS
    ********************/
    private static int logSVValuesCount = 0;

    private IModHelper helper;
    private ModConfig config;
    private Logger log;

    private bool inFishingMenu = false;
    private bool catchingFishState = true;

    /// <summary>
    /// Due to a potential bug in the game, or something necessary for its internal code,
    /// the 'caughtTreasure' flag on the fishing pole is never reset after a fish is caught
    /// UNTIL the next cast.  So if the mod has process treasure turned on, it will run
    /// for *all* chests opened after a fish is caught with treasure if another cast is not
    /// attempted that does not yield treasure.  So this variable tracks if we're at the
    /// treasure chest associated with the fishing game.
    /// </summary>
    private bool doneCaughtFish = false;

    // Fishing constants to control algorithm
    private readonly float switchToTreasureThreshold = 0.75F;
    private readonly float switchtoFishThreshold = 0.35F;
    private readonly float fishingBiteAccumulatorGap = 10.0F;

    private readonly float fBarMaxVelocity = 1.0F;
    private readonly float lowerLimitMaxVelocity = 2.0F;
    private readonly float fBarTerminalVelocity = 6.0F;
    private readonly float lowerLimitBrakePoint = 30.0F;

    // Reflection variables so they are not evaluated each update tick
    private IReflectedField<int> svBobberBarHeight;

    private IReflectedField<float> svBobberBarPos;
    private IReflectedField<float> svBobberBarSpeed;
    private IReflectedField<float> svDistanceFromCatching;
    private IReflectedField<float> svBobberPosition;
    private IReflectedField<float> svTreasurePosition;

    private IReflectedField<bool> svPerfect;
    private IReflectedField<bool> svTreasure;
    private IReflectedField<bool> svTreasureCaught;

    /**************
    ** CONSTRUCTORS
    **************/

    /// <summary>
    /// Set up configuration and any variables that don't need modification during Update.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="config"></param>
    public FishForMe(IModHelper helper, ModConfig config)
    {
      this.log = Logger.GetLog();

      log.Silly("Creating FishForMe instance");
      this.helper = helper;
      this.config = config;

      // Set readonly values from config
      fBarMaxVelocity = config.fishingBarMaxVelWithFish;
      fBarTerminalVelocity = config.fishingBarMaxVelWithoutFish;
      lowerLimitMaxVelocity = config.fishingBarMaxVelAtBottom;

      // Since this is a static value, just set it here in the constructor
      if (config.alwaysTreasure) {
        log.Trace("Setting fishing to always spawn a treasure chest");
        FishingRod.baseChanceForTreasure = 1.0F;
      }
    }

    /****************
    ** PUBLIC METHODS
    ****************/

    /// <summary>
    /// Do anything the automaton wants on half update ticks.
    /// </summary>
    public void OnHalfSecondUpdate()
    {
      PrintInternalDebugValues();
    }

    /// <summary>
    /// Update mod state.  Try to keep processing quick here.
    /// </summary>
    public void OnUpdate()
    {
      if (!(Game1.player.CurrentTool is FishingRod))
        return;

      FishingRod currentTool = Game1.player.CurrentTool as FishingRod;

      // Could set up a state machine to track this, but for now just check either in the minigame or not.
      if (Game1.activeClickableMenu is BobberBar && currentTool.isFishing) {
        PlayFishMinigame();
      }
      else {
        // Do everything else the mod does outside of the fishing game.
        // If this is the first time out of the fishing game, clear the data.
        if (this.inFishingMenu) {
          log.Silly("Left Fishing minigame, clearing memory");
          this.UnloadSVReflectionObjects();
          this.inFishingMenu = false;
          IsButtonDownHack.simulateDown = false;
        }

        // This might be where a state machine might help, as an extra check, (e.g can't set power if you're not done casting, etc...)
        // but the SV tool API seems to give us enough data to have a go at it just checking conditionals.

        // Sequential steps to automate fishing
        if (config.autoCast && ShouldDoAutoCast(currentTool)) {
          log.Trace("Attempting to cast for the user");
          currentTool.beginUsing(Game1.currentLocation,
                                  Game1.player.getStandingX(),
                                  Game1.player.getStandingY(),
                                  Game1.player);
        }

        if (config.maxCastPower && ShouldSetCastingPower(currentTool)) {
          log.Trace("Modifying cast to max cast power");
          // Apparently for max to be registered it needs to be > 1.0, but anything over increases the
          // power bar past the UI, so make it a small number.  Also, 1.001F is too small.
          currentTool.castingPower = 1.01F;
        }

        if (config.quickHook && ShouldSetQuickHook(currentTool)) {
          log.Trace("Modifying cast so fish will bite quickly.");
          // Game uses fishingBiteAccumulator as the value that gets modified so we should only modify that one
          // and let the game increase it and handle logic as they become equal.  Change the gap if it doesn't
          // generate a quick hit quick enough.
          currentTool.fishingBiteAccumulator = currentTool.timeUntilFishingBite - fishingBiteAccumulatorGap;
        }

        if (config.autoHook && ShouldDoAutoHook(currentTool)) {
          log.Trace("Executing hook function for FishingRod");
          currentTool.DoFunction(Game1.currentLocation,
                                  Game1.player.getStandingX(),
                                  Game1.player.getStandingY(),
                                  (int)currentTool.castingPower,
                                  Game1.player);
        }

        // Click away the catched fish.  The conditionals are ordered here in a way
        // the check for the popup happens before the config check so the code can
        // always check the treasure chest.  See the variable doneCaughtFish for more
        // info.
        if (ShouldDoDismissCaughtPopup(currentTool)) {
          log.Trace("Tool is sitting at caught fish popup");
          doneCaughtFish = true;

          if (config.autoFinish && config.HarmonyLoad) {
            log.Trace("Closing popup with Harmony");
            ClickAtAllHack.simulateClick = true;
          }
        }
        else {
          if (ClickAtAllHack.simulateClick) {
            log.Trace("Turning off Harmony injection for caught fish popup");
            ClickAtAllHack.simulateClick = false;
          }
        }

        // Deal with any treasure that has popped up
        if (ShouldDoProcessTreasure(currentTool) && doneCaughtFish) {
          log.Trace("Found treasure after the caught fish popup");
          doneCaughtFish = false;
          if (config.autoFinish) {
            log.Trace("Acquiring treasure");
            ItemGrabMenu m = Game1.activeClickableMenu as ItemGrabMenu;
            if (m.ItemsToGrabMenu != null && m.ItemsToGrabMenu.actualInventory != null) {
              if (AcquireTreasure(m.ItemsToGrabMenu.actualInventory)) {
                m.exitThisMenu(true);
              }
              else {
                HandleItemGrabMenuError(m, "might be full inventory");
              }
            }
            else {
              HandleItemGrabMenuError(m, "treasure chest is open, but inventory is null");
            }
          }
        }
      }
    }

    /*****************
    ** PRIVATE METHODS
    *****************/

    private bool AcquireTreasure(IList<Item> inventory)
    {
      log.Trace("Attempting to acquire all items from inventory");
      log.Silly($"There are {inventory.Count} items in this inventory");
      for (int i = 0; i < inventory.Count; ++i) {
        if (inventory[i] != null) {
          Item invItem = inventory[i];
          log.Silly($"item in treasure box is {invItem.Name}");
          if (Game1.player.couldInventoryAcceptThisItem(invItem)) {
            log.Trace($"Inventory can accept: {invItem.Name}");
            if (Game1.player.addItemToInventoryBool(invItem)) {
              log.Trace($"Successfully added {invItem.Name}, removing from chest");
              Utility.removeItemFromInventory(i, inventory);
            }
            else {
              log.Error("Adding item to inventory failed even after call to couldAcceptItem");
              return false;
            }
          }
          else {
            log.Debug("Item cannot accept item, probably full");
            return false;
          }
        }
      }
      return true;
    }

    private float FBarTargetVelocity(float currentDelta, float minVel, float maxVel,
                                     float minDelta, float maxDelta)
    {
      // Incase function below uses logs, stay away from zero
      if (currentDelta == 0.0F || minDelta == 0.0F || maxDelta == 0.0F) {
        log.Silly("Deltas were zero, returning minVel");
        return minVel;
      }

      // Parabolic, higher speed allowed as fish reaches outer portion of bar.
      double oldMinVel = FBarTargetVelocityFn((double)minDelta);
      double oldMaxVel = FBarTargetVelocityFn((double)maxDelta);

      // Normalize the delta between the two values.
      float newVel = (float)((((maxVel - minVel) / (oldMaxVel - oldMinVel)) * (FBarTargetVelocityFn((double)currentDelta) - oldMaxVel)) + maxVel);
      log.Silly($"Setting new max velocity to {newVel}");
      return newVel;
    }

    /// <summary>
    /// Map the FB velocity to a given function.
    /// </summary>
    /// <param name="v">The value to manipulate</param>
    /// <returns>A new value representing the current max velocity the bar should be able to move.</returns>
    private double FBarTargetVelocityFn(double v)
    {
      // Parabolic, higher speed allowed as fish reaches outer portion of bar.
      return Math.Pow(v, 2);
    }

    /// <summary>
    /// Determine whether the menu should be closed or not on an error
    /// </summary>
    /// <param name="menu">the inventory to check</param>
    /// <param name="msg">Added to the error message</param>
    private void HandleItemGrabMenuError(ItemGrabMenu menu, string msg)
    {
      log.Info($"There was an error getting all menu items - {msg}");
      if (config.pauseTreasureOnError) {
        log.Trace("Pausing collection until resolved.");
      }
      else {
        log.Info("Option pauseTreasureOnError is false, continuing with loss of treasure");
        menu.exitThisMenu(true);
      }
    }
    
    /// <summary>
    /// Simple check to see if the desired position is inside the fishing bar.
    /// </summary>
    /// <param name="barSize">The size of the green fishing bar</param>
    /// <param name="barPos">Where the bar is at</param>
    /// <param name="fishPos">The desired position to check</param>
    /// <returns>true if the position is in the bar, inclusive.</returns>
    private bool IsFishInsideBar(int barSize, float barPos, float fishPos)
    {
      return (fishPos >= barPos) && (fishPos <= barPos + barSize);
    }

    /// <summary>
    /// Assign the objects that will pull non-public Stardew data.  The docs say this is an expensive operation
    /// and so should be cached and then the same objects are used to call GetValue and SetValue.
    /// </summary>
    /// <param name="bar">A reference to the Fishing window</param>
    private void LoadSVReflectionObjects(BobberBar bar)
    {
      log.Silly("Assigning reflected fields to class members.");

      svBobberBarHeight = helper.Reflection.GetField<int>(bar, "bobberBarHeight");

      svBobberBarPos = helper.Reflection.GetField<float>(bar, "bobberBarPos");
      svBobberBarSpeed = helper.Reflection.GetField<float>(bar, "bobberBarSpeed");
      svDistanceFromCatching = helper.Reflection.GetField<float>(bar, "distanceFromCatching");
      svBobberPosition = helper.Reflection.GetField<float>(bar, "bobberPosition");
      svTreasurePosition = helper.Reflection.GetField<float>(bar, "treasurePosition");

      svPerfect = helper.Reflection.GetField<bool>(bar, "perfect");
      svTreasure = helper.Reflection.GetField<bool>(bar, "treasure");
      svTreasureCaught = helper.Reflection.GetField<bool>(bar, "treasureCaught");

      log.Silly("Finished processing reflected values");
    }

    /// <summary>
    /// Called when the Fishing menu is up and the game is ready to play.
    /// Do all the steps (that are configured) to catched the fish.
    /// </summary>
    private void PlayFishMinigame()
    {
      log.Silly("Algorithm to play the fishing game started");
      BobberBar fishGame = Game1.activeClickableMenu as BobberBar;

      // Check if this is the first tick we've been in the BobberBar menu
      if (!inFishingMenu) {
        log.Trace("Fishing started, setting up environment");
        LoadSVReflectionObjects(fishGame);
        inFishingMenu = true;
      }

      // Does the user even want the mod to fish?
      if (!config.catchFish) {
        log.Silly("Player does not want this to fish for them. =-(");
        return;
      }

      // Just set perfect first so it gets caught by boring fishing.
      if (config.alwaysPerfect) {
        log.Trace("Setting perfect flag to true");
        svPerfect.SetValue(true);
      }

      // Are we being boring?
      if (config.boringFishing) {
        log.Silly("Boring fishing turned on, just catch the fish");
        svDistanceFromCatching.SetValue(1.0F);
        return;
      }

      // Here, fBar is the green fishing bar, fish is the 'bobber' or eemmm.. fish.  Top of the window
      // is 0 and y+ as you go down, so a negative velocity means bar is going up.  Cache the values
      // so they're not constantly queried from the Reflected fields.
      log.Silly("Acquiring values from reflected fields");
      int fBarHeight = svBobberBarHeight.GetValue();
      float maxFBarPos = BobberBar.bobberBarTrackHeight - fBarHeight;
      float currentFBarPos = svBobberBarPos.GetValue();
      float currentFbarHalfPos = currentFBarPos + (fBarHeight / 2);
      float currentFBarVelocity = svBobberBarSpeed.GetValue();

      float fishPos = svBobberPosition.GetValue();
      float distanceFromCatching = svDistanceFromCatching.GetValue();
      float desiredPos = fishPos;

      bool treasureSpawn = svTreasure.GetValue();
      bool caughtTreasure = svTreasureCaught.GetValue();
      float treasurePos = svTreasurePosition.GetValue();

      // Go for the treasure?
      if (treasureSpawn && treasurePos > 0 && !caughtTreasure) {
        if (distanceFromCatching > switchToTreasureThreshold && catchingFishState) catchingFishState = false;
        else if (distanceFromCatching < switchtoFishThreshold && !catchingFishState) catchingFishState = true;
      }
      else {
        // If we not catching treasure at all, always make sure we're going for the fish.
        catchingFishState = true;
      }
      log.Silly($"Going for the fish? {catchingFishState}");
      desiredPos = catchingFishState ? fishPos : treasurePos;

      // Check velocity limits first
      if ((Math.Abs(currentFBarVelocity) > fBarTerminalVelocity) ||
          (IsFishInsideBar(fBarHeight, currentFBarPos, desiredPos) &&
          Math.Abs(currentFBarVelocity) >
          FBarTargetVelocity(Math.Abs(currentFbarHalfPos - desiredPos), fBarMaxVelocity, fBarTerminalVelocity, 0.01F, (fBarHeight / 2)))) {
        log.Silly("Fishing bar is travelling too fast; limiting rate");
        if (currentFBarVelocity > 0.0) IsButtonDownHack.simulateDown = true;
        else IsButtonDownHack.simulateDown = false;
      }
      else if ((currentFBarPos > maxFBarPos - lowerLimitBrakePoint) &&
               currentFBarVelocity > lowerLimitMaxVelocity) {
        // The bar can bounce on the bottom, but don't stop it too early in case it's trying to
        // catch the fish.  But try to minimize the speed before the bounce.
        log.Silly("Fishing bar is about to bounce on the bottom; limiting rate");
        IsButtonDownHack.simulateDown = true;
      }
      else if (desiredPos >= currentFbarHalfPos) {
        log.Silly($"Pos >= half, turning off.  Des: {desiredPos} half: {currentFbarHalfPos}");
        IsButtonDownHack.simulateDown = false;
      }
      else {
        log.Silly($"Pos < half, turning on.  Des: {desiredPos} half: {currentFbarHalfPos}");
        IsButtonDownHack.simulateDown = true;
      }
    }

    /// <summary>
    /// A purely debug method that prints out various values for research.
    /// </summary>
    /// <remarks>
    /// Marked Conditional DEBUG so that it this method is only included in code when the Debug
    /// build configuration is used.  During Release, it turns any call to it into a noop.
    /// </remarks>
    [Conditional("DEBUG")]
    private void PrintInternalDebugValues()
    {
      if (!(Game1.player.CurrentTool is FishingRod)) return;

      FishingRod r = Game1.player.CurrentTool as FishingRod;

      // I don't use multiline string because I want it all to be aligned in the log output instead of
      // subsequent print lines not having the log header at the front of the line.
      log.Silly();
      log.Silly($"UPDATE {FishForMe.logSVValuesCount}");
      log.Silly("--Game--");
      log.Silly($"activeClickableMenu: {Game1.activeClickableMenu}");
      log.Silly($"currentMinigame: {Game1.currentMinigame}");
      log.Silly();
      log.Silly("--Player--");
      log.Silly($"canMove: {Game1.player.canMove}");
      log.Silly();
      log.Silly("--Rod--");
      log.Silly($"castedButBobberStillInAir: {r.castedButBobberStillInAir}");
      log.Silly($"fishCaught: {r.fishCaught}");
      log.Silly($"hit: {r.hit}");
      log.Silly($"inUse(): {r.inUse()}");
      log.Silly($"isCasting: {r.isCasting}");
      log.Silly($"isFishing: {r.isFishing}");
      log.Silly($"isNibbling: {r.isNibbling}");
      log.Silly($"isReeling: {r.isReeling}");
      log.Silly($"isTimingCast: {r.isTimingCast}");
      log.Silly($"pullingOutOfWater: {r.pullingOutOfWater}");
      log.Silly($"showingTreasure: {r.showingTreasure}");
      log.Silly($"timeUntilFishingBite: {r.timeUntilFishingBite}");
      log.Silly($"fishingBiteAccumulator: {r.fishingBiteAccumulator}");
      log.Silly($"treasureCaught: {r.treasureCaught}");
      if (inFishingMenu) {
        log.Silly();
        log.Silly("--Window--");
        FieldInfo[] fif = typeof(FishForMe).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var prop in fif) {
          if (prop.FieldType.IsGenericType) {
            if (prop.FieldType.GetGenericTypeDefinition() == typeof(IReflectedField<>)) {
              MethodInfo gv = prop.FieldType.GetMethod("GetValue");
              if (gv != null) {
                log.Silly($"{prop.Name}: {gv.Invoke(prop.GetValue(this), null)}");
              }
            }
          }
        }
      }
      log.Silly($"END UPDATE {FishForMe.logSVValuesCount}");
      log.Silly();
      FishForMe.logSVValuesCount++;
    }

    /// <summary>
    /// Test whether the mod can automatically cast the fishing rod for the user.
    /// </summary>
    /// <param name="currentTool">A FishingRod that is being used to fish</param>
    /// <returns>true if the mod should cast for the user</returns>
    private bool ShouldDoAutoCast(FishingRod fr)
    {
      return Context.CanPlayerMove && Game1.activeClickableMenu is null && !fr.castedButBobberStillInAir &&
             !fr.hit && !fr.inUse() && !fr.isCasting && !fr.isFishing && !fr.isNibbling && !fr.isReeling &&
             !fr.isTimingCast && !fr.pullingOutOfWater;
    }

    /// <summary>
    /// Test whether the game is in a state that a fishing rod should run its function for a hit.
    /// </summary>
    /// <param name="currentTool">A FishingRod that is being used to fish</param>
    /// <returns>true if the mod should run the hit function</returns>
    private bool ShouldDoAutoHook(FishingRod fr)
    {
      return (!Context.CanPlayerMove) && Game1.activeClickableMenu is null && fr.isNibbling &&
             fr.isFishing && !fr.isReeling && !fr.hit && !fr.pullingOutOfWater &&
             !fr.fishCaught && !fr.isTimingCast;
    }

    /// <summary>
    /// Check if the player is waiting for the caught fish popup to go away
    /// </summary>
    /// <param name="currentTool">A FishingRod that is being used to fish</param>
    /// <returns>true if the mod should click the popup away</returns>
    private bool ShouldDoDismissCaughtPopup(FishingRod fr)
    {
      return (!Context.CanPlayerMove) && fr.fishCaught && fr.inUse() && !fr.isCasting && !fr.isFishing &&
             !fr.isReeling && !fr.isTimingCast && !fr.pullingOutOfWater && !fr.showingTreasure;
    }

    /// <summary>
    /// Check if the player is waiting to claim any treasure found.
    /// </summary>
    /// <param name="currentTool">A FishingRod that is being used to fish</param>
    /// <returns>true if the mod should get the treasure</returns>
    /// <remarks>
    /// Found a slight oddity - when the ItemGrabMenu is closed after treasure is awarded, the 'showingTreasure'
    /// variable is not reset.  This can lead to a bug with the below function as most of it relies on showingTreasure.
    /// Add code to the handling function as needed to add secondary checks.
    /// </remarks>
    private bool ShouldDoProcessTreasure(FishingRod fr)
    {
      return (!Context.CanPlayerMove) && Game1.activeClickableMenu is ItemGrabMenu && fr.showingTreasure &&
             !fr.inUse() && !fr.isCasting && !fr.isFishing && !fr.isNibbling && !fr.isReeling && !fr.isTimingCast;
    }

    /// <summary>
    /// Test whether the mod should be modifying the casting power of a fishing rod.
    /// </summary>
    /// <param name="currentTool">A FishingRod that is being used to fish.</param>
    /// <returns>true if the mod can set the casting power</returns>
    private bool ShouldSetCastingPower(FishingRod fr)
    {
      // Probably not a big deal, regardless only edit the casting power when the rod is being cast.
      return (!Context.CanPlayerMove) && Game1.activeClickableMenu is null && fr.isTimingCast;
    }

    /// <summary>
    /// Test whether the mod should be modifying when the fish will bite on the rod.
    /// 
    /// The game primarily uses FishingRod.timeUntilFishingBite and FishingRod.fishingBiteAccumulator, but it seems to
    /// also set or use the values during the fishing game when BobberBar window is active, so make sure the mod is
    /// only modifying when it should.
    /// </summary>
    /// <param name="fr">A FishingRod that is being used to fish.</param>
    /// <returns>true if the mod can set values for FastBite configuration</returns>
    private bool ShouldSetQuickHook(FishingRod fr)
    {
      return (!Context.CanPlayerMove) && Game1.activeClickableMenu is null && fr.inUse() && fr.isFishing &&
             (!(fr.hit || fr.isReeling || fr.isNibbling)) && fr.timeUntilFishingBite >= 0 &&
             fr.fishingBiteAccumulator + fishingBiteAccumulatorGap < fr.timeUntilFishingBite;
    }

    /// <summary>
    /// Unloads the SV IReflectedFields from memory to prevent any possible memory leaks
    /// </summary>
    private void UnloadSVReflectionObjects()
    {
      log.Silly("Clearing unused reflected fields");

      svBobberBarHeight = null;
      svBobberBarPos = null;
      svBobberBarSpeed = null;
      svDistanceFromCatching = null;
      svBobberPosition = null;
      svTreasurePosition = null;
      svTreasure = null;
      svTreasureCaught = null;
    }
  }
}
