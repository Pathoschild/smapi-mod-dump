/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Better10Hearts.Config;
using Better10Hearts.Patches;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Better10Hearts
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields 
        *********/
        /// <summary>A collection of NPCs that meet the critea to give the player stamina when spoken to.</summary>
        private IDictionary<string, bool> npcEnergyGeneration = new Dictionary<string, bool>();


        /*********
        ** Accessors 
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>Whether this mod has handled the farmer passout yet.</summary>
        public static bool HasPassoutBeenHandled { get; private set; } = false;


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;
            this.Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            this.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
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
            // add new mail items
            if (asset.AssetNameEquals("Data/mail"))
            {
                IDictionary<string, string> newMailList = this.Helper.Content.Load<Dictionary<string, string>>("assets/NewMail.json", ContentSource.ModFolder);
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                foreach (var newMail in newMailList)
                {
                    data[newMail.Key] = newMail.Value;
                }
            }
        }


        /*********
        ** Private Methods 
        *********/
        /****
        ** Methods
        ****/
        /// <summary>The method that applies the harmony patches for replacing game code.</summary>
        /// <param name="uniqueId">The mod unique id.</param>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.PassOutFromTiredPrefix)))
            );
        }

        /****
        ** Event Handlers
        ****/
        /// <summary>Check every second if the player has spoken to any NPC since the previous second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ensure a save is loaded and to only run code once every second
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

                // check if there the player has spoken to NPC
                if (previousHasSpokenToday == newHasSpokenToday)
                {
                    continue;
                }

                // add stamina based on if they are the player's spouse
                var npcSpouse = npc.getSpouse();
                if (npcSpouse?.Name == Game1.player.Name)
                {
                    Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.SpouseStaminaIncrease));
                }
                else
                {
                    Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.NPCStaminaIncrease));
                }

                // add to a separate list as it the iterating collection can't be changed in a loop
                npcNamesToChange.Add(npc.Name);
            }

            // iternate through that list that the names were added to, make the changes
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
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            var location = Game1.currentLocation;

            // ensure an NPC is in the area passed out in and that the time is passed 2am
            if (location.characters.Count == 0 || Game1.timeOfDay < 2600)
            {
                return;
            }

            HasPassoutBeenHandled = true;

            // pick a random NPC that is in that location and set as the mail sender
            NPC letterSender = location.characters[Game1.random.Next(location.characters.Count)];

            // check if the player is in the NPC's home or not, this will determine what letter to send
            if (letterSender.DefaultMap == location.Name)
            {
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Home");
            }
            else
            {
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Away");
            }
        }

        /// <summary>The method invoked once the player starts a new day, this is used for checking NPC birthdays and preparing players to get stamina when talking to NPCs.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            HasPassoutBeenHandled = false;

            // remove all mail this mod adds from received mail, this is because mail that's in the 'recieved mail' can't be send again
            foreach (var mail in Game1.player.mailReceived)
            {
                if (mail.Contains("better10Hearts"))
                {
                    Game1.player.mailReceived.Remove(mail);
                }
            }

            // empty the list so it can be regenerated every day, this is so if an NPC goes below 10 they no longer will be on the list
            npcEnergyGeneration.Clear();

            foreach (NPC npc in Utility.getAllCharacters())
            {
                // ensure the player has friendship data with the NPC and that the NPC hasn't already been added (this is becuase monsters are iterated and they have the same name)
                if (!Game1.player.friendshipData.ContainsKey(npc.Name) || npcEnergyGeneration.ContainsKey(npc.Name))
                {
                    continue;
                }

                var friendship = Game1.player.friendshipData[npc.Name];
                float hearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;

                if (Config.OnlyGetStaminaAt10Hearts ? hearts >= 10 : true)
                {
                    // add the npc to a list to check if they have spoken today
                    npcEnergyGeneration.Add(npc.Name, false);

                    if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth) && Config.MaxLuckOnNPCBirthdays)
                    {
                        // set max luck 
                        Game1.player.team.sharedDailyLuck.Value = 0.12;
                    }
                }
            }
        }
    }
}
