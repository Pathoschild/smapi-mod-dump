using System;
using StardewValley;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using Harmony;
using Netcode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ExhaustionTweaks
{
    public class ModEntry : Mod
    {
        private static IModHelper mHelper;
        private static ModEntry modEntry;
        private static ModConfig config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            mHelper = helper;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            modEntry = this;

            config = mHelper.ReadConfig<ModConfig>();
            
            // example patch, you'll need to edit this for your patch
            modEntry.Monitor.Log($"Loading Harmony Patch for passOutFromTired method.", LogLevel.Info);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.passOutFromTired)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.passOutFromTired_Prefix))
            );

            if(config.enableConstantEnergyLossAfterTime)
            {
                modEntry.Monitor.Log($"Register Energy Management Event", LogLevel.Info);
                mHelper.Events.GameLoop.TimeChanged += EnergyReductionHandler;
            }
            

            modEntry.Monitor.Log($"Patch Loaded.", LogLevel.Trace);

            modEntry = this;
        }

        private void EnergyReductionHandler(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            Farmer localFarmer = Game1.player;
            modEntry.Monitor.Log($"Time Update: {e.NewTime}.", LogLevel.Trace);
            modEntry.Monitor.Log($"In Bed: {localFarmer.isInBed.Value}.", LogLevel.Trace);
            if (config.constantEnergyLossStartTime <= e.NewTime && !localFarmer.isInBed.Value)
            {
                
                modEntry.Monitor.Log($"Reduce energy for: {localFarmer.displayName}.", LogLevel.Trace);
                bool reduceEnergy = true;
                if (config.disableLossAtMinimumEnergy && config.minimumEnergyToDisableLoss > localFarmer.Stamina)
                {
                    modEntry.Monitor.Log($"Don't take energy. {config.minimumEnergyToDisableLoss} > {localFarmer.Stamina}", LogLevel.Trace);
                    reduceEnergy = false;
                }

                if (reduceEnergy)
                {
                    float percentEnergy = (config.percentEnergyLostEvery10Minutes / 100.0f);
                    modEntry.Monitor.Log($"Take {percentEnergy} of {localFarmer.MaxStamina}.", LogLevel.Trace);
                    float energyToTake = localFarmer.MaxStamina * percentEnergy;

                    if ((localFarmer.Stamina - energyToTake) < config.minimumEnergyToDisableLoss)
                    {
                        modEntry.Monitor.Log($"Would normally take {energyToTake} energy, but that is too much, so altering the amount.", LogLevel.Trace);
                        energyToTake = (localFarmer.Stamina - config.minimumEnergyToDisableLoss);
                    }

                    modEntry.Monitor.Log($"Take {energyToTake} energy.", LogLevel.Trace);

                    localFarmer.Stamina -= energyToTake;
                }
            }

            return;
        }

        public static bool letModHandlePassout(Farmer who, GameLocation passOutLocation)
        {
            //at the moment this is simply based on the location, but we can layer in more complex conditions here.
            if (ModEntry.config.enableSafeSleeping)
            {
                bool processPassout = true;
                if (ModEntry.config.requireSpouseForSafeSleeping)
                {
                    modEntry.Monitor.Log($"Spouse required for safe sleep handling. Spouse is {who.spouse}", LogLevel.Trace);

                    if (who.spouse == null || who.spouse == "")
                    {
                        modEntry.Monitor.Log($"{who.displayName} does not have a spouse. Let normal processing proceed.", LogLevel.Trace);
                        processPassout = false;
                    }
                }

                if (processPassout)
                {
                    foreach (String locationName in ModEntry.config.safeSleepingLocations)
                    {
                        if (passOutLocation.Name.Equals(locationName))
                        {
                            return true;
                        }
                    }

                    if (ModEntry.config.enableSafeSleepingInFarmBuildings)
                    {
                        if (passOutLocation.isFarmBuildingInterior() ||
                            passOutLocation.Name == "Greenhouse")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool isFarmLocation(GameLocation location)
        {
            return (location is Farm || location is StardewValley.Locations.FarmCave || location.isFarmBuildingInterior());
        }

        public static bool passOutFromTired_Prefix(SObject __instance, Farmer who)
        {
            try
            {
                if (ModEntry.config.reduceEnergyWhenPassingOut)
                {
                    if (!who.IsLocalPlayer)
                    {
                        modEntry.Monitor.Log($"{who.displayName} is not local player. Stop.", LogLevel.Trace);
                        return true;
                    }

                    modEntry.Monitor.Log($"Energy loss on passout is enabled.", LogLevel.Trace);

                    int percentToTake = ModEntry.config.percentEnergyLostWhenPassingOut;

                    float currentStamina = who.Stamina;

                    int staminaToTake = (int)Math.Floor(who.Stamina * (100 / percentToTake));

                    if (currentStamina - staminaToTake < 2.0)
                    {
                        staminaToTake = (int) currentStamina - 2;
                    }

                    modEntry.Monitor.Log($"Remove {staminaToTake} energy.", LogLevel.Trace);

                    who.Stamina -= staminaToTake;
                }

                GameLocation passOutLocation = Game1.currentLocation;
                modEntry.Monitor.Log($"Game Location for {who.displayName} is {passOutLocation.Name}", LogLevel.Trace);
                //turn this into a configurable list of locations.
                if (letModHandlePassout(who, passOutLocation))
                {
                    modEntry.Monitor.Log($"{who.displayName} in mod controlled location. Game Location is {passOutLocation.Name}", LogLevel.Trace);
                    //we need to handle most of the passing out logic again here so we can deal with the money and messaging
                    //stuff in the middle. Sad sad panda.
                    if (!who.IsLocalPlayer)
                    {
                        modEntry.Monitor.Log($"{who.displayName} is not local player. Stop.", LogLevel.Trace);
                        return true;
                    }

                    modEntry.Monitor.Log($"{who.displayName} is mounted. Dismount.", LogLevel.Trace);

                    if (who.isRidingHorse())
                    {   
                        who.mount.dismount(false);
                    }
                        
                    if (Game1.activeClickableMenu != null)
                    {
                        Game1.activeClickableMenu.emergencyShutDown();
                        Game1.exitActiveMenu();
                    }

                    who.completelyStopAnimatingOrDoingAction();
                    if ((bool)((NetFieldBase<bool, NetBool>)who.bathingClothes))
                    {
                        who.changeOutOfSwimSuit();
                    }
                        
                    who.swimming.Value = false;
                    who.CanMove = false;
                    if (who == Game1.player && (FarmerTeam.SleepAnnounceModes)((NetFieldBase<FarmerTeam.SleepAnnounceModes, NetEnum<FarmerTeam.SleepAnnounceModes>>)Game1.player.team.sleepAnnounceMode) != FarmerTeam.SleepAnnounceModes.Off)
                    {
                        string str = "PassedOut";
                        int num = 0;
                        for (int index = 0; index < 2; ++index)
                        {
                            if (Game1.random.NextDouble() < 0.25)
                                ++num;
                        }

                        if (mHelper != null)
                        {
                            String message = who.displayName + " has passed out.";
                            if (Game1.IsMultiplayer)
                            {
                                mHelper.Multiplayer.SendMessage<String>(message, "PassoutMessage");
                            }
                        }
                    }
                    
                    Vector2 bed = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
                    bed.X -= 64f;

                    LocationRequest.Callback callback = (LocationRequest.Callback)(() =>
                    {
                        who.Position = bed;
                        who.currentLocation.lastTouchActionLocation = bed;
                        if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
                            Game1.PassOutNewDay();
                        Game1.changeMusicTrack("none", false, Game1.MusicContext.Default);

                        if (ModEntry.config.enableRobberyOnFarm || ModEntry.config.enableRobberyOnNonFarmSafeLocations)
                        {
                            modEntry.Monitor.Log($"Robbery enabled. Check for robbery.", LogLevel.Trace);
                            if ((ModEntry.config.enableRobberyOnFarm && isFarmLocation(passOutLocation)) ||
                                (ModEntry.config.enableRobberyOnNonFarmSafeLocations && !isFarmLocation(passOutLocation)))
                            {
                                //eligible to be robbed
                                Random random1 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)Game1.player.UniqueMultiplayerID);

                                int robbedRoll = random1.Next(100);

                                modEntry.Monitor.Log($"Robbery roll:. {robbedRoll}", LogLevel.Trace);

                                if (robbedRoll < ModEntry.config.percentRobberyChance)
                                {
                                    //we got robbed, sad sad panda.
                                    int percentTaken = random1.Next(ModEntry.config.percentFundsLostMinimumInRobbery, ModEntry.config.percentFundsLostMaximumInRobbery);
                                    int amountTaken = who.Money * (percentTaken / 100);

                                    List<int> list = new List<int>();
                                    list.Add(2); //the willy letter.
                                    list.Add(4); //the marlon letter.

                                    int letterNumber = Utility.GetRandom<int>(list, random1);

                                    string letter = "passedOut" + (object)letterNumber + " " + (object)amountTaken;
                                    modEntry.Monitor.Log($"Got robbed send letter: {letter}", LogLevel.Trace);
                                    if (amountTaken > 0)
                                    {
                                        who.Money -= amountTaken;
                                        who.mailForTomorrow.Add(letter);
                                    }
                                }
                            }
                        }
                    });
                    
                    if (!(bool)((NetFieldBase<bool, NetBool>)who.isInBed) || who.currentLocation != null && !who.currentLocation.Equals((object)who.homeLocation.Value))
                    {
                        modEntry.Monitor.Log($"{who.displayName} is not in bed. Send home.", LogLevel.Trace);
                        LocationRequest locationRequest = Game1.getLocationRequest(who.homeLocation.Value, false);
                        Game1.warpFarmer(locationRequest, (int)bed.X / 64, (int)bed.Y / 64, 2);
                        locationRequest.OnWarp += callback;
                        who.FarmerSprite.setCurrentSingleFrame(5, (short)3000, false, false);
                        who.FarmerSprite.PauseForSingleAnimation = true;
                    }
                    else
                    {
                        callback();
                    }

                    modEntry.Monitor.Log($"Skip regular passout behavior.", LogLevel.Trace);
                    return false;
                }

                modEntry.Monitor.Log($"Execute standard passout behavior.", LogLevel.Trace);
                return true;
            }
            catch (Exception ex)
            {
                modEntry.Monitor.Log($"Failed in {nameof(passOutFromTired_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
