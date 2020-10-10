/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/littleraskol/ReRegeneration
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Tools;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ReRegeneration
{
    class ReRegenerator : Mod
    {
        //Stamina values
        float stamRegenVal;                    //How much stamina to regen
        float lastStamina;                     //Last recorded player stamina value.
        //double lastMaxStamina;                  //Last recorded max stamina value.
        float staminaCooldown;                 //How long to wait before beginning stamina regen.
        int staminaDelay;                       //Time from exertion to regen
        float maxStamRatio;                    //Percent of max stam to regen to.
        float maxStamRegenAmount;               //Actual max stamina value to regen.
        float playerStamNow;                    //Holds current value, for validation.
        float stamRegenMult;                   //Used for scaling regen.
        float stamDelayMult;                   //Used for scaling idle delay.

        //Health values
        float healthRegenVal;                  //How often to add health, needed due to weird math constraints...
        int lastHealth;                         //Last recorded player health value.
        //int lastMaxHealth;                      //Last recorded max health value.
        double healthAccum;                     //Accumulated health regenerated while running.
        float healthCooldown;                  //How long to wait before beginning health regen.
        int healthDelay;                        //Time from injury to regen
        float maxHealthRatio;                  //Percent of max health to regen to.
        int maxHealthRegenAmount;               //Actual max health value to regen.
        int playerHealthNow;                    //Holds current value, for validation.
        float healthRegenMult;                 //Used for scaling regen.
        float healthDelayMult;                 //Used for scaling idle delay.

        //Control values
        double currentTime;                     //The... current time. (In seconds?)
        double timeElapsed;                     //Time since last check.
        double lastLogTime;                     //Time since last log event.
        double lastTickTime;                    //The time at the last tick processed.

        bool percentageMode;                    //Whether we're using this mode of operation.
        float activeRegenMult;                 //Rate limiter while fishing, riding horse, etc.
        float runRegenRate;                    //Running rate.
        float exhaustRegenPenalty;                  //Penalty to regen due to being exhausted.
        float exhaustCooldownPenalty;                  //Penalty to cooldown due to being exhausted.
        float endExhaustion;                   //For the exhuastion status end option.
        float stillnessDelayBonus;             //Staying still = shorter idle delay.
        float runningDelayMalus;               //Running = longer idle delay.

        double intervalMult;                    //Seconds or fractions thereof, defines regen interval.
        uint updateTickCount;                    //How many actual ticks the above translates into.
        bool verbose;                           //Whether to log regular diagnostics
        
        Farmer myPlayer;                        //Our player.
        RegenConfig myConfig;                     //Config data.

        public override void Entry(IModHelper helper)
        {
            myConfig = helper.ReadConfig<RegenConfig>();

            //Starting values at extremes which prevent unexpected behavior.
            lastStamina = 9999;
            //lastMaxStamina = 0.0;
            staminaCooldown = 0;

            lastHealth = 9999;
            //lastMaxHealth = 0;
            healthCooldown = 0;
            healthAccum = 0.0;

            currentTime = 0.0;
            timeElapsed = 0.0;
            lastTickTime = 0.0;
            lastLogTime = 0.0;

            updateTickCount = 1; //Avoids div by 0 errors

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += StartupTasks;
            helper.Events.GameLoop.DayStarted += DailyUpdate;
            helper.Events.GameLoop.UpdateTicked += OnUpdate;    //Set to quarter-second intervals.

            Monitor.Log("ReRegeneration => Initialized", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            TryLoadingGMCM();
        }

        private void TryLoadingGMCM()
        {
            //See if we can find GMCM, quit if not.
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenu.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                Monitor.Log("Unable to load GMCM API.", LogLevel.Info);
                return;
            }

            api.RegisterModConfig(ModManifest, () => myConfig = new RegenConfig(), () => Helper.WriteConfig(myConfig));

            //Basic Values
            api.RegisterLabel(ModManifest, "Basic Configuration", "Core settings for stamina and health regeneration.");
            api.RegisterClampedOption(ModManifest, "Stamina Regen/Sec", "How much stamina to regenerate passively per second. Use config file to enter values greater than 10.", () => myConfig.staminaRegenPerSecond, (float val) => myConfig.staminaRegenPerSecond = val, 0, 10);
            api.RegisterClampedOption(ModManifest, "Stamina Idle Time", "How long to wait, in seconds, after an exertion to regen stamina. Use config file to enter values greater than 60.", () => myConfig.staminaIdleSeconds, (int val) => myConfig.staminaIdleSeconds = val, 1, 60);
            api.RegisterClampedOption(ModManifest, "Health Regen/Sec", "How much health to regenerate passively per second. Use config file to enter values greater than 10.", () => myConfig.healthRegenPerSecond, (float val) => myConfig.healthRegenPerSecond = val, 0, 10);
            api.RegisterClampedOption(ModManifest, "Health Idle Time", "How long to wait, in seconds, after an injury to regen health. Use config file to enter values greater than 60.", () => myConfig.healthIdleSeconds, (int val) => myConfig.healthIdleSeconds = val, 1, 60);

            //Advanced Values
            api.RegisterLabel(ModManifest, "Advanced Configuration", "More complex settings. Consult the readme to understand these.");
            api.RegisterSimpleOption(ModManifest, "Percentage Mode", "Whether to treat the regen values as percentages of max stamina/health. See readme for more details.", () => myConfig.percentageMode, (bool val) => myConfig.percentageMode = val);
            api.RegisterClampedOption(ModManifest, "Max Stamina Ratio", "Percentage of player's maximum stamina that can be passively regenerated.", () => (int)(myConfig.maxStaminaRatioToRegen * 100), (int val) => myConfig.maxStaminaRatioToRegen = (float)val / 100, 1, 100);
            api.RegisterClampedOption(ModManifest, "Stamina Regen Scaling Rate", "See readme for an explanation of this entry.", () => (int)(myConfig.scaleStaminaRegenRateTo * 100), (int val) => myConfig.scaleStaminaRegenRateTo = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Stamina Delay Scaling Rate", "See readme for an explanation of this entry (use config file to enter value higher than 1.0 (i.e., 100%)).", () => (int)(myConfig.scaleStaminaRegenDelayTo * 100), (int val) => myConfig.scaleStaminaRegenDelayTo = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Max Health Ratio", "Percentage of player's maximum health that can be passively regenerated.", () => (int)(myConfig.maxHealthRatioToRegen * 100), (int val) => myConfig.maxHealthRatioToRegen = (float)val / 100, 1, 100);
            api.RegisterClampedOption(ModManifest, "Health Regen Scaling Rate", "See readme for an explanation of this entry.", () => (int)(myConfig.scaleHealthRegenRateTo * 100), (int val) => myConfig.scaleHealthRegenRateTo = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Health Delay Scaling Rate", "See readme for an explanation of this entry (use config file to enter value higher than 1.0 (i.e., 100%)).", () => (int)(myConfig.scaleHealthRegenDelayTo * 100), (int val) => myConfig.scaleHealthRegenDelayTo = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Active Regen Rate", "Rate of regeneration while fishing or riding a horse. See readme for an more info.", () => (int)(myConfig.regenWhileActiveRate * 100), (int val) => myConfig.regenWhileActiveRate = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Running Regen Rate", "Rate of regeneration while running. See readme for an more info.", () => (int)(myConfig.regenWhileRunningRate * 100), (int val) => myConfig.regenWhileRunningRate = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Exhaustion Penalty", "Percentage by which the amount of regeneration is decreased and the duration of the idle delay is increased while the player is exhausted. See readme for an more info.", () => (int)(myConfig.exhuastionPenalty * 100), (int val) => myConfig.exhuastionPenalty = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "End Exhaustion At...", "At what percentage of max stamina should the exaustion penalty be removed. Enter 0 to disable this feature. See readme for an more info.", () => (int)(myConfig.endExhaustionAt * 100), (int val) => myConfig.endExhaustionAt = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Delay Stillness Bonus", "Shortens the regen delay while standing still by the specified percentage. Enter 0 to disable this feature. See readme for an more info.", () => (int)(myConfig.shortenDelayWhenStillBy * 100), (int val) => myConfig.lengthenDelayWhenRunningBy = (float)val / 100, 0, 100);
            api.RegisterClampedOption(ModManifest, "Delay Running Penalty", "Lengthens the regen delay while running by the specified percentage. Enter 0 to disable this feature. See readme for an more info. (Use config file to enter value higher than 1.0 (i.e., 100%).", () => (int)(myConfig.lengthenDelayWhenRunningBy * 100), (int val) => myConfig.lengthenDelayWhenRunningBy = (float)val / 100, 0, 100);
        }

        private void StartupTasks(object sender, SaveLoadedEventArgs e)
        {
            myPlayer = Game1.player;

            //Using percentage mode?
            percentageMode = myConfig.percentageMode;

            //Verbosity?
            verbose = myConfig.verboseMode;

            //Set update interval/ticks
            intervalMult = Math.Max(0.01, myConfig.timeInterval);
            updateTickCount = (uint)Math.Max(1, (60 * intervalMult));

            //Running rates
            runRegenRate = Math.Max(0, Math.Min(1, myConfig.regenWhileRunningRate));

            //Max regen ratios
            maxStamRatio = Math.Max(0.01f, Math.Min(1, myConfig.maxStaminaRatioToRegen));
            maxHealthRatio = Math.Max(0.01f, Math.Min(1, myConfig.maxHealthRatioToRegen));

            //Active regen ratio
            activeRegenMult = Math.Max(0, Math.Min(1, myConfig.regenWhileActiveRate));

            //Exhaustion penalty
            exhaustCooldownPenalty = Math.Max(0, Math.Min(0.99f, myConfig.exhuastionPenalty));
            exhaustRegenPenalty = 1 - exhaustRegenPenalty;

            //End exhaustion at?
            endExhaustion = Math.Max(0, Math.Min(1, myConfig.endExhaustionAt));

            //Delay bonuses and penalties
            stillnessDelayBonus = 1 + Math.Max(0, myConfig.shortenDelayWhenStillBy);
            runningDelayMalus = 1 - Math.Max(0, Math.Min(1, myConfig.lengthenDelayWhenRunningBy));

            //Delay/idle before regen
            staminaDelay = Math.Max(0, myConfig.staminaIdleSeconds);
            healthDelay = Math.Max(0, myConfig.healthIdleSeconds);
        }

        private void DailyUpdate(object sender, DayStartedEventArgs e)
        {
            //Reset these values
            staminaCooldown = 0;
            healthCooldown = 0;
            healthAccum = 0.0;

            //Every day, initialize relevant values. This happens very frequently but I want to get some baseline values.
            SetRegenVals();
        }

        /* Figure out what the regen values are. May be called frequently.
         * 
         * Health regen is expected to be slower than stamina. This will effectively increase the interval time in that case.
         * We have to do this because unlike stamina, health is an int. So, in the default case of 0.1 health/sec, what
         * really happens is that the player gets 1 health every 10 seconds (1 / 0.1 = 10).
         * 
        */
        void SetRegenVals()
        {
            if (!Game1.hasLoadedGame || myPlayer == null) return;   //Not sure this can ever happen but eh

            //Reset update interval/ticks
            intervalMult = Math.Max(0.01, myConfig.timeInterval);
            updateTickCount = (uint)Math.Max(1 ,(60 * intervalMult));

            //Maximum values that passive regen can reach
            maxStamRegenAmount = (float)Math.Round((double)myPlayer.maxStamina * maxStamRatio);
            maxHealthRegenAmount = (int)Math.Round((double)myPlayer.maxHealth * maxHealthRatio);

            //Validated current values
            playerStamNow = myPlayer.stamina < 0 ? 0 : myPlayer.stamina;
            playerHealthNow = myPlayer.health < 0 ? 0 : myPlayer.health;    //Shouldn't be possible but eh...

            float stamRatioToMax = Math.Min(playerStamNow, this.maxStamRegenAmount) / this.maxStamRegenAmount;
            float healthRatioToMax = Math.Min(playerHealthNow, this.maxHealthRegenAmount) / this.maxHealthRegenAmount;

            //Using as a holding value until modified
            stamRegenMult = Math.Max(0, Math.Min(1, myConfig.scaleStaminaRegenRateTo));
            healthRegenMult = Math.Max(0, Math.Min(1, myConfig.scaleHealthRegenRateTo));

            //Scaling regen values
            stamRegenMult = stamRegenMult > 0 ? stamRegenMult + (stamRatioToMax * (1 - stamRegenMult)) : 1;
            healthRegenMult = healthRegenMult > 0 ? healthRegenMult + (healthRatioToMax * (1- healthRegenMult)) : 1;

            //Using as a holding value until modified
            stamDelayMult = Math.Max(0, myConfig.scaleStaminaRegenDelayTo);
            healthDelayMult = Math.Max(0, myConfig.scaleHealthRegenDelayTo);

            //Scaling idle delay
            stamDelayMult = 1 + ((1 - stamRatioToMax) * stamDelayMult);
            healthDelayMult = 1 + ((1 - healthRatioToMax) * healthDelayMult);

            //Initial default values, will be modified for percentage mode if needed.
            stamRegenVal = Math.Max(0, myConfig.staminaRegenPerSecond);
            healthRegenVal = Math.Max(0, myConfig.healthRegenPerSecond);

            //What to do if calculating by percentages.
            if (percentageMode)
            {
                stamRegenVal *= (0.01f * maxStamRegenAmount);
                healthRegenVal *= (0.01f * maxHealthRegenAmount);
            }

        }

        //Timed logging.
        void LogIt(string msg, bool doIt = true, LogLevel lvl = LogLevel.Debug)
        {
            if (doIt && msg != "")
            {
                Monitor.Log(msg, lvl);
                lastLogTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
            }
        }

        //Formatted stat report.
        string StatReport(bool doAll = true, bool doMod = false, bool doStam = false, bool doHealth = false)
        {
            //only if in verbose mode
            if (!verbose) { return ""; }

            string retStr = String.Format("\n\n========= ReRegenerator Status after {0} seconds ==========", Math.Round(Game1.currentGameTime.TotalGameTime.TotalSeconds));

            myPlayer = Game1.player;

            if (doAll || doMod)
            {
                retStr += String.Format("\n\n-- Mod Status --\n*Stam recovery rate: {0}\n*While running: {1}\n*Health recovery rate: {2}\n*While running: {3}\n*Percent mode: {4}",
                    stamRegenVal, runRegenRate, healthRegenVal, runRegenRate, percentageMode);
            }

            if (doAll || doStam)
            {
                retStr += String.Format("\n\n-- Stamina Status --\n*Lost stamina: {0}\n*Cooldown time: {1}", (myPlayer.maxStamina - myPlayer.stamina), Math.Round(staminaCooldown));
            }

            if (doAll || doHealth)
            {
                retStr += String.Format("\n\n-- Health Status --\n*Lost Health: {0}\n*Cooldown time: {1}\n*Accumulator: {2}", (myPlayer.maxHealth - myPlayer.health), Math.Round(healthCooldown), healthAccum);
            }

            //On mon-mod updates, give player stats.
            if (!doMod)
            {
                retStr += String.Format("\n\n-- myPlayer Status --\n*Is running: {0}\n*Moved last tick: {1}\n*On horseback: {2}", myPlayer.running, myPlayer.movedDuringLastTick(), myPlayer.isRidingHorse());
            }

            retStr += "\n\n========== ReRegenerator Status Report Complete ==========\n\n";

            return retStr;
        }

        /* Determine whether movement status will block normal regeneration: If player is... 
         * 1. running, and
         * 2. has moved recently, and
         * 3. is not on horseback, then
         * movement blocks normal regen and the running rate prevails.
        */
        private bool HasMovePenalty(Farmer f)
        {
            return (f.running && f.movedDuringLastTick() && !f.isRidingHorse());
        }

        /* Determine whether "action" penalty applies: If player is... 
         * 1. fishing, or
         * 2. riding a horse, then
         * regen penalty to ongoing action applies.
        */
        private bool HasActPenalty(Farmer f)
        {
            return (f.usingTool && f.CurrentTool is FishingRod rod && (rod.isFishing || rod.isCasting || rod.isReeling || rod.isTimingCast)) || (f.isRidingHorse() && f.movedDuringLastTick());
        }

        private void OnUpdate(object sender, UpdateTickedEventArgs e)
        {
            //All of this requires that the game be loaded, the player is set, and this be a quarter-second tick.
            if (!e.IsMultipleOf(updateTickCount) || !Game1.hasLoadedGame || myPlayer == null) return;

            //Make sure we know exactly how much time has elapsed
            currentTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
            timeElapsed = currentTime - lastTickTime;   //Every amount of regen multiplied by this.
            lastTickTime = currentTime;

            //Catches and attempts to deal with menus, cutscenes, etc. reducing the cooldown.
            //if (!Game1.shouldTimePass()) frozenTime += timeElapsed;
            //If time can pass (i.e., are not in an event/cutscene/menu/festival)...
            //else
            if (Game1.shouldTimePass())
            {
                SetRegenVals();

                //Reduce time we want to "use" by frozen time.
                //if (frozenTime > 0.0)
                //{
                //    timeElapsed = timeElapsed > frozenTime ? timeElapsed - frozenTime : 0.0;
                //    frozenTime = 0.0;
                //}

                //Do this once.
                LogIt(StatReport(false, true, false, false), (currentTime < 30.0) && lastLogTime == 0.0, LogLevel.Trace);

                //Every 30 secs report on all.
                LogIt(StatReport(true, false, false, false), (currentTime - lastLogTime) >= 30, LogLevel.Trace);

                double regenProgress;   //Will be set to timeElapsed w/ modifiers

                bool movePenalty = HasMovePenalty(myPlayer);
                bool actPenalty = HasActPenalty(myPlayer);

                /* Determine how much progress made towards ending cooldown.
                 * 1. If running and there is a penalty, apply it to elapsed time.
                 * 2. Otherwise, if staying still and there is a bonus, apply it to elapsed time.
                 * 3. Otherwise, elapsed time is progress.
                 */
                if (movePenalty && (runningDelayMalus > 0.0)) regenProgress = timeElapsed * runningDelayMalus;
                else if (!myPlayer.movedDuringLastTick() && (stillnessDelayBonus > 0.0)) regenProgress = timeElapsed * stillnessDelayBonus;
                else regenProgress = timeElapsed;

                //LogIt($"\nCooldown 1: Time Elapsed = {timeElapsed}, Run Penalty = {runningDelayMalus}, Still Bonus = {stillnessDelayBonus}, Start Stam Cooldown = {staminaCooldown}, Start Heal Cooldown = {healthCooldown}\nCooldown 2: Time Counted = {regenProgress}, Expected if Running = {timeElapsed*runningDelayMalus}, Expected if Still = {timeElapsed*stillnessDelayBonus}", verbose, LogLevel.Trace);

                //If exhausted, increase delay by the penalty
                if (myPlayer.exhausted)
                {
                    stamDelayMult += exhaustCooldownPenalty;
                    healthDelayMult += exhaustCooldownPenalty;
                }

                //LogIt($"\nCooldown 3: Exhausted? {myPlayer.exhausted}, Stam Delay Mult = {stamDelayMult}, Heal Delay Mult = {healthDelayMult}, Exhaustion Penalty = {exhaustCooldownPenalty}\nCooldown 4: Expected New Stam Cooldown (Stam Changes) = {staminaDelay*stamDelayMult}, Expected New Stam Cooldown (Stam Static) = {staminaCooldown - regenProgress}", verbose, LogLevel.Trace);

                //Check for player exertion. If player has used stamina since last tick, reset the cooldown.
                //Decrement how long we've been on stamina cooldown otherwise.
                if (myPlayer.stamina < lastStamina) { staminaCooldown = staminaDelay * stamDelayMult; }
                else if (staminaCooldown > 0) { staminaCooldown -= (float)regenProgress; }
                else staminaCooldown = 0;

                //LogIt($"\nCooldown 5: Stam Cooldown Time = {staminaCooldown}, Has Stamina Changed? {myPlayer.stamina < lastStamina}\nCooldown 6: Expected New Heal Cooldown (Stam Changes) = {healthDelay*healthDelayMult}, Expected New Heal Cooldown (Stam Static) = {healthCooldown - regenProgress}", verbose, LogLevel.Trace);

                //Check for player injury. If player has been injured since last tick, reset the cooldown.
                //Decrement how long we've been on health cooldown otherwise.
                if (myPlayer.health < lastHealth) { healthCooldown = healthDelay * healthDelayMult; }
                else if (healthCooldown > 0) { healthCooldown -= (float)regenProgress; }
                else healthCooldown = 0;

                //LogIt($"\nCooldown 7: Heal Cooldown Time = {healthCooldown}, Has Health Changed? {myPlayer.health < lastHealth}", verbose, LogLevel.Trace);

                /*
                 * Process stamina regeneration. Here are the criteria:
                 * -Must have some stamina regen value.
                 * -Must have less than max stamina.
                 * -The stamina cooldown must be over.
                */
                if (stamRegenVal > 0 && myPlayer.stamina < maxStamRegenAmount && staminaCooldown <= 0)
                {
                    //LogIt($"\nStam Regen 1: Starting Stamina = {myPlayer.stamina}, Max to Regen = {maxStamRegenAmount}, Starting Regen Val = {stamRegenVal}", verbose, LogLevel.Trace);

                    //Start building the regen modifier.
                    double stamMult = stamRegenMult;

                    //If "active" reduce by specified amount.
                    if (actPenalty) stamMult *= activeRegenMult;

                    //If running, reduce by specified amount.
                    if (movePenalty) stamMult *= runRegenRate;

                    //If exhausted, reduce by the penalty.
                    if (myPlayer.exhausted) stamMult *= exhaustRegenPenalty;

                    //Per-sec val * multiplier * fractions of 1 sec passed
                    myPlayer.stamina += (float)(stamRegenVal * stamMult * intervalMult);

                    //LogIt($"\nStam Regen 2: Initial Mod = {stamRegenMult}, Activity Mod = {activeRegenMult}, Running Mod = {runRegenRate}, Exhaustion Mod = {exhaustRegenPenalty}\nStam Regen 3: Final Mod = {stamMult}, Interval Mult = {intervalMult}, Final Stam Regained = {(float)(stamRegenVal*stamMult*intervalMult)}, Final Stamina = {myPlayer.stamina}", verbose, LogLevel.Trace);

                    //Final sanity check
                    if (myPlayer.stamina > maxStamRegenAmount) { myPlayer.stamina = maxStamRegenAmount; }

                    //staminaCooldown = 1.0;
                }

                /*
                 * Process health regeneration. Here are the criteria:
                 * -Must have some health regen value.
                 * -Must have less than max health.
                 * -The health cooldown must be over.
                */
                if (healthRegenVal > 0 && myPlayer.health < maxHealthRegenAmount && healthCooldown <= 0)
                {
                    //LogIt($"\nHeal Regen 1: Starting Health = {myPlayer.health}, Max to Regen = {maxHealthRegenAmount}, Starting Regen Val = {healthRegenVal}", verbose, LogLevel.Trace);
                    //Start building the regen modifier.
                    double healMult = healthRegenMult;

                    //If "active" reduce by specified amount.
                    if (actPenalty) healMult *= activeRegenMult;

                    //If running, reduce by specified amount.
                    if (movePenalty) healMult *= runRegenRate;

                    //If exhausted, reduce by the penalty.
                    if (myPlayer.exhausted) healMult *= exhaustRegenPenalty;

                    /* 
                     * Basically, we want to try to restore health every interval, but we absolutely need a round number
                     * because player health is an integer, not a float. So we "accumulate" fractional health each interval 
                     * (only actually fractional under some circumstances). In this case, the fraction is not applied to
                     * the regen amount so much as accumulated per interval.
                    */
                    healthAccum += (healthRegenVal * healMult * intervalMult); //Per-sec val * multiplier * fractions of 1 sec passed

                    //LogIt($"\nHeal Regen 2: Initial Mod = {healthRegenMult}, Activity Mod = {activeRegenMult}, Running Mod = {runRegenRate}, Exhaustion Mod = {exhaustRegenPenalty}\nHeal Regen 3: Final Mod = {healMult}, Interval Mult = {intervalMult}\nHeal Regen 4: Health Accumulated Now = {(float)(healthRegenVal*healMult*intervalMult)}, Total Health Accumulated = {healthAccum}, Enough Accumulated? {healthAccum >= 1}", verbose, LogLevel.Trace);

                    //If we've accumulated 1 or more health, apply the whole number value and recalc the accumulation accordingly.
                    if (healthAccum >= 1)
                    {
                        double rmndr = healthAccum % 1;       //Capture fractional value
                        healthAccum -= rmndr;                 //Reduce to whole number
                        myPlayer.health += (int)healthAccum;  //Apply whole number value
                        healthAccum = rmndr;                  //Accumulate remainder
                    }

                    //LogIt($"\nHeal Regen 5: Modified Accumulator = {healthAccum}, Final Health = {myPlayer.health}", verbose, LogLevel.Trace);

                    //If we have achieved a round positive number accumulated, apply it and reset the accumulation.
                    //This probably shouldn't ever really happen because of the above, but it's a fallback just in case...
                    //else if (healthAccum > 0 && healthAccum % 1 == 0)
                    //{
                    //    myPlayer.health += (int)healthAccum;
                    //    healthAccum = 0.0;
                    //}

                    //Final sanity check
                    if (myPlayer.health > maxHealthRegenAmount) { myPlayer.health = maxHealthRegenAmount; }

                    //healthCooldown = 1.0;
                }

                //LogIt($"\nExhaustion 1: Exhausted? {myPlayer.exhausted}, Can End Exhaustion? {(endExhaustion > 0.0)}, Should End Exhaustion? {(myPlayer.stamina / maxStamRegenAmount) > endExhaustion}", verbose, LogLevel.Trace);

                //Determine whether to end exhausted status.
                if (myPlayer.exhausted && endExhaustion > 0.0 && (myPlayer.stamina / maxStamRegenAmount) > endExhaustion) myPlayer.exhausted.Value = false;

                //LogIt($"\nExhaustion 1: Still Exhausted? {myPlayer.exhausted}", verbose, LogLevel.Trace);

                // Updated stored health/stamina values.
                lastHealth = myPlayer.health;
                lastStamina = myPlayer.stamina;

                //Every 10 seconds give health/stam update.
                LogIt(StatReport(false, false, true, true), (currentTime - lastLogTime) >= 10, LogLevel.Trace);
            }
        }
    }

}
