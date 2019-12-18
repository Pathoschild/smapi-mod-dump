using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace Better10Hearts
{
    /// <summary>Mod entry point.</summary>
    class ModEntry : Mod, IAssetEditor
    {

        public IDictionary<string, bool> npcEnergyGeneration = new Dictionary<string, bool>();

        /// <summary>The mod configuration.</summary>
        public ModConfig Config;

        /// <summary></summary>
        public static bool HasPassoutBeenHandled = false;

        /// <summary>Mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.DayStarted += Events_DayStarted;
            this.Helper.Events.GameLoop.TimeChanged += Events_TimeChanged;
            this.Helper.Events.GameLoop.UpdateTicked += Events_UpdateTicked;
            
            Config = this.Helper.ReadConfig<ModConfig>();

            ApplyHarmonyPatches(this.ModManifest.UniqueID);
        }

        /// <summary>The method that applies the harmony patches for replacing game code.</summary>
        /// <param name="uniqueId">The mod unique id.</param>
        private void ApplyHarmonyPatches(string uniqueId)
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix)))
            );
        }

        /// <summary>This is code that will replace some game code, this is ran whenever the player is about to place some mixed seeds. Used for calculating the result from the seed list.</summary>
        /// <returns>This will always return false as this contains the game code as well as the patch.</returns>
        private static bool Prefix(Farmer who)
        {
            if (!who.IsLocalPlayer)
                return false;
            if (who.isRidingHorse())
                who.mount.dismount();
            if (Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
            }
            who.completelyStopAnimatingOrDoingAction();
            if (who.bathingClothes.Value)
                who.changeOutOfSwimSuit();
            who.swimming.Value = false;
            who.CanMove = false;
            GameLocation passOutLocation = Game1.currentLocation;
            Vector2 bed = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
            bed.X -= 64f;
            LocationRequest.Callback callback = (LocationRequest.Callback)(() =>
            {
                who.Position = bed;
                who.currentLocation.lastTouchActionLocation = bed;
                if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
                    Game1.PassOutNewDay();
                Game1.changeMusicTrack("none");
                if (passOutLocation is FarmHouse || passOutLocation is Cellar)
                    return;

                // Check if an NPC has piked up the player or if the game should handle the passout
                if (!ModEntry.HasPassoutBeenHandled)
                {
                    int num = Math.Min(1000, who.Money / 10);
                    who.Money -= num;
                    who.mailForTomorrow.Add("passedOut " + (object)num);
                }
            });
            if (!who.isInBed.Value)
            {
                LocationRequest locationRequest = Game1.getLocationRequest(who.homeLocation.Value, false);
                Game1.warpFarmer(locationRequest, (int)bed.X / 64, (int)bed.Y / 64, 2);
                locationRequest.OnWarp += callback;
                who.FarmerSprite.setCurrentSingleFrame(5, (short)3000, false, false);
                who.FarmerSprite.PauseForSingleAnimation = true;
            }
            else
                callback();

            return false;
        }

        /// <summary>Check every second if the player has spoken to any NPC since the previous second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Events_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Make sure a save is loaded and to only run code once every second
            if (!Context.IsWorldReady || !e.IsOneSecond)
            {
                return;
            }

            List<string> npcNamesToChange = new List<string>();

            foreach (var key in npcEnergyGeneration.Keys)
            {
                NPC npc = Game1.getCharacterFromName(key);

                if (npc == null)
                {
                    continue;
                }

                bool previousHasSpokenToday = npcEnergyGeneration[npc.Name];
                bool newHasSpokenToday = Game1.player.hasTalkedToFriendToday(npc.Name);

                // Check if there the player has spoken to NPC
                if (previousHasSpokenToday != newHasSpokenToday)
                {
                    var npcSpouse = npc.getSpouse();

                    // This is to prevent a NullException error if the NPC doesn't have a spouse
                    if (npcSpouse != null)
                    {
                        // Check if NPC is player's spouse or not
                        if (npcSpouse.Name == Game1.player.Name)
                        {
                            // Increase the player's stamina by the config value for the spouse. Make sure not to go above the max and it is increased and not decreased
                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.SpouseStaminaIncrease));
                        }
                        else
                        {
                            // Increase the player's stamina by the config value for the NPC. Make sure not to go above the max and it is increased and not decreased
                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.NPCStaminaIncrease));
                        }
                    }
                    else
                    {
                        // Increase the player's stamina by the config value for the NPC. Make sure not to go above the max and it is increased and not decreased
                        Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.NPCStaminaIncrease));
                    }

                    // Add the names to a list so later on we can set them to true. We can't change it now as the list that needs changing is being iterated
                    npcNamesToChange.Add(npc.Name);
                }
            }

            // Iternate through that list that the names were added to, make the changes
            if (npcNamesToChange.Count > 0)
            {
                foreach (var name in npcNamesToChange)
                {
                    npcEnergyGeneration[name] = true;
                }
            }
        }

        /// <summary>The method invoked once the game clock has updated, this is used for checking if the player has passed out.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Events_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Get to current location to check the NPCs against
            var location = Game1.currentLocation;

            // Make sure an NPC is in the area passed out in and that the time is 2am +
            if (location.characters.Count == 0 || Game1.timeOfDay < 2600)
            {
                return;
            }

            // Make sure to set the passout as being handled so the game doesn't handle it
            HasPassoutBeenHandled = true;

            // Pick a random NPC that is in that location and set as the mail sender
            NPC letterSender = location.characters[new Random().Next(location.characters.Count)];

            // Check if the player is in the NPC's home or not
            if (letterSender.DefaultMap == location.Name)
            {
                // Send the player mail from that NPC as if they are at home
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Home");
            }
            else
            {
                // Send the player mail from that NPC as if they are away from home
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Away");
            }
        }

        /// <summary>The method invoked once the player starts a new day, this is used for checking NPC birthdays and preparing players to get stamina when talking to NPCs.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Events_DayStarted(object sender, DayStartedEventArgs e)
        {
            // Mark passout handled as false for the new day
            HasPassoutBeenHandled = false;

            // Remove all the mail added by the mod from the mailbox so it can be resent each day
            foreach (var mail in Game1.player.mailReceived)
            {
                if (mail.Contains("better10Hearts"))
                {
                    Game1.player.mailReceived.Remove(mail);
                }
            }

            // Empty the list, this means it's regenerated every day. Meaning if an NPC goes below 10, they no longer will be on the list
            npcEnergyGeneration.Clear();

            foreach (NPC npc in Utility.getAllCharacters())
            {
                // Check the player has friendship data with the NPC
                if (Game1.player.friendshipData.ContainsKey(npc.Name))
                {
                    // Retrieve that frienship data and calculate how many hearts they have
                    var friendship = Game1.player.friendshipData[npc.Name];

                    float hearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;

                    if (Config.OnlyGetStaminaAt10Hearts)
                    {
                        // Check if friendship is at 10 hearts
                        if (hearts >= 10 && !npcEnergyGeneration.ContainsKey(npc.Name))
                        {
                            // Add the npc to a list to check if they have spoken today
                            npcEnergyGeneration.Add(npc.Name, false);

                            // Check if it's the NPC's birthday
                            if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                            {
                                // Set max luck 
                                Game1.player.team.sharedDailyLuck.Value = 0.12;
                            }
                        }
                    }
                    else
                    {
                        // Check if friendship is at 10 hearts
                        if (!npcEnergyGeneration.ContainsKey(npc.Name))
                        {
                            // Add the npc to a list to check if they have spoken today
                            npcEnergyGeneration.Add(npc.Name, false);

                            // Check if it's the NPC's birthday
                            if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                            {
                                // Set max luck 
                                Game1.player.team.sharedDailyLuck.Value = 0.12;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>This will call when loading each asset, if the mail asset is being loaded, return true as we want to edit this.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset info being loaded.</param>
        /// <returns>True if the assets being loaded needs to be edited.</returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // If the mail asset is passed in, return true so we can edit it
            if (asset.AssetNameEquals("Data/mail"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit the mail asset to add the new mail for when the player passes out.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset data being loaded.</param>
        public void Edit<T>(IAssetData asset)
        {
            // Add new mail items
            if (asset.AssetNameEquals("Data/mail"))
            {
                IDictionary<string, string> newMailList = this.Helper.Content.Load<Dictionary<string, string>>("Assets/NewMail.json", ContentSource.ModFolder);

                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                // Add each new mail item to the mail list
                foreach (var newMail in newMailList)
                {
                    data[newMail.Key] = newMail.Value;
                }
            }
        }
    }
}
