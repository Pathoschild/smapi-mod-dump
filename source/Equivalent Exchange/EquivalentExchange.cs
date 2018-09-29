using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Input;
using EquivalentExchange.Models;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using StardewValley.Network;
using SpaceCore.Events;
using SpaceCore;

namespace EquivalentExchange
{

    /// <summary>The mod entry point.</summary>
    public class EquivalentExchange : Mod
    {
        // instantiate config
        private ConfigurationModel Config;

        // the mod's "static" instance, initialized by Entry. There caN ONly bE ONe
        public static EquivalentExchange instance;

        // holds the player data for all active players, then uses statics to expose this player's data.
        public SaveDataModel currentPlayerData = new SaveDataModel();

        // config for if the mod is allowed to play sounds
        public static bool canPlaySounds;        

        // new stuff for space's skills api
        public static AlchemySkill skill;

        public const string MSG_DATA = "EquivalentExchange.AlchemySkill.Data";
        public const string MSG_EXPERIENCE = "EquivalentExchange.AlchemySkill.Experience";
        public const string MSG_LEVEL = "EquivalentExchange.AlchemySkill.Level";
        public const string MSG_CURRENT_ENERGY = "EquivalentExchange.AlchemySkill.CurrentEnergy";
        public const string MSG_MAX_ENERGY = "EquivalentExchange.AlchemySkill.MaxEnergy";
        public const string MSG_TOTAL_VALUE_TRANSMUTED = "EquivalentExchange.AlchemySkill.TotalValueTransumted";
        public const string MSG_REGEN_TICK = "EquivalentExchange.AlchemySkill.RegenTick";
        public const string MSG_IS_SLIME_GIVEN_TO_WIZARD = "EquivalentExchange.AlchemySkill.GaveWizardSlime";

        //handles all the things.
        public override void Entry(IModHelper helper)
        {
            //set the static instance variable. is this an oxymoron?
            instance = this;            

            //read the config file, poached from horse whistles, get the configured keys and settings
            Config = helper.ReadConfig<ConfigurationModel>();

            //add handler for the "transmute/copy" button.
            ControlEvents.KeyPressed += ControlEvents_KeyPressed;

            //exclusively to figure out if ctrl or shift have been let go of.
            ControlEvents.KeyReleased += ControlEvents_KeyReleased;

            //wire up the library scraping function to occur on save-loading to defer recipe scraping until all mods are loaded, optimistically.
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;

            //we need this to save our alchemists['] data
            SaveEvents.BeforeSave += SaveEvents_BeforeSave;

            //set texture files in memory, they're tiny things.
            DrawingUtil.HandleTextureCaching();

            //handles high resolution update ticks, like regeneration and held keys.
            GameEvents.UpdateTick += GameEvents_UpdateTick;            

            //wire up the PreRenderHUD event so I can display info bubbles when needed
            GraphicsEvents.OnPreRenderHudEvent += GraphicsEvents_OnPreRenderHudEvent;
            
            // handles end of night event requirements like alchemy energy being restored and level ups.
            SpaceEvents.ShowNightEndMenus += SpaceEvents_ShowNightEndMenus;

            // stuff we have to do for multiplayer now, handles client join events to cascade data to the non-hosts.
            SpaceEvents.ServerGotClient += SpaceEvents_ServerGotClient;

            // handle looking out for the slime gift to the wizard that gates the alchemy content
            SpaceEvents.AfterGiftGiven += SpaceEvents_AfterGiftGiven;

            Networking.RegisterMessageHandler(MSG_DATA, OnDataMessage);
            Networking.RegisterMessageHandler(MSG_EXPERIENCE, OnExpMessage);
            Networking.RegisterMessageHandler(MSG_LEVEL, OnLevelMessage);
            Networking.RegisterMessageHandler(MSG_CURRENT_ENERGY, OnCurrentEnergyMessage);
            Networking.RegisterMessageHandler(MSG_MAX_ENERGY, OnMaxEnergyMessage);
            Networking.RegisterMessageHandler(MSG_TOTAL_VALUE_TRANSMUTED, OnTransmutedValueMessage);
            Networking.RegisterMessageHandler(MSG_REGEN_TICK, OnRegenTick);
            Networking.RegisterMessageHandler(MSG_IS_SLIME_GIVEN_TO_WIZARD, OnSlimeGivenToWizardMessage);

            Skills.RegisterSkill(skill = new AlchemySkill());
        }

        // bools for menuing through the wizard slime-gift dialog
        public static bool isSlimeGiftDialogComplete = false;
        public static bool needsToShowSlimeEventDialog = false;
        public static bool needsToShowAlchemyUnlockedDialog = false;

        private void SpaceEvents_AfterGiftGiven(object sender, EventArgsGiftGiven e)
        {
            // this can only fire the first time.
            if (IsSlimeGivenToWizard)
                return;

            if (e.Npc.Name.Equals("Wizard") && e.Gift.parentSheetIndex.Equals(Reference.Items.Slime))
            {
                IsSlimeGivenToWizard = true;
                needsToShowSlimeEventDialog = true;
            }
        }

        // there are two states we need to preserve. 
        // The first is whether the player gave the wizard a slime.        
        // The second is whether the player has unlocked their alchemy ability.
        // In between those, we can infer that the player hasn't seen the dialog.
        private void HandleWizardSlimeListener()
        {
            if (Game1.dialogueUp)
                return;

            if (IsSlimeGivenToWizard && needsToShowAlchemyUnlockedDialog)
            {
                // Game1.drawDialogueBox(Helper.Translation.Get(Reference.Localizations.AlchemyUnlocked, new { transmuteKey = instance.Config.TransmuteKey.ToString() }));
                Game1.drawObjectDialogue(Helper.Translation.Get(Reference.Localizations.AlchemyUnlocked, new { transmuteKey = instance.Config.TransmuteKey.ToString() }));
                needsToShowAlchemyUnlockedDialog = false;
            }

            if (IsSlimeGivenToWizard && needsToShowSlimeEventDialog)
            {
                // show the npc dialog with localizations
                Game1.drawDialogue(Game1.getCharacterFromName(Reference.Characters.WizardName, true), Helper.Translation.Get(Reference.Localizations.WizardSpeech));
                needsToShowSlimeEventDialog = false;
                needsToShowAlchemyUnlockedDialog = true;
            }
        }

        private void SpaceEvents_ShowNightEndMenus(object sender, EventArgsShowNightEndMenus e)
        {
            //the new day hook seems to be inconsistent, so this is a full restore at the end of the night.
            Alchemy.RestoreAlkahestryEnergyForNewDay();
        }

        private void SpaceEvents_ServerGotClient(object sender, EventArgsServerGotClient e)
        {
            // first thing we need to do is check to see if this player exists in player data. If they don't, let's make them a profile.
            var farmerId = e.FarmerID;
            if (!PlayerData.AlchemyLevel.ContainsKey(farmerId))
                PlayerData.AlchemyLevel[farmerId] = 0;
            if (!PlayerData.AlchemyExperience.ContainsKey(farmerId))
                PlayerData.AlchemyExperience[farmerId] = 0;
            if (!PlayerData.AlkahestryCurrentEnergy.ContainsKey(farmerId))
                PlayerData.AlkahestryCurrentEnergy[farmerId] = 0F;
            if (!PlayerData.AlkahestryMaxEnergy.ContainsKey(farmerId))
                PlayerData.AlkahestryMaxEnergy[farmerId] = 0F;
            if (!PlayerData.TotalValueTransmuted.ContainsKey(farmerId))
                PlayerData.TotalValueTransmuted[farmerId] = 0;
            if (!PlayerData.IsSlimeGivenToWizard.ContainsKey(farmerId))
                PlayerData.IsSlimeGivenToWizard[farmerId] = false;

            // Log.debug($"Adding player {farmerId.ToString()} to registry. Keys currently: { PlayerData.AlchemyLevel.Count }");

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                // arbitrarily using the first property as the index master, it should be irrelevant, as each should have the same # of keys.
                writer.Write(PlayerData.AlchemyLevel.Count);
                foreach (var lvl in PlayerData.AlchemyLevel)
                {
                    writer.Write(lvl.Key);
                    writer.Write(lvl.Value);
                }
                // we don't need the key beyond this point.
                foreach (var exp in PlayerData.AlchemyExperience)
                {
                    writer.Write(exp.Key);
                    writer.Write(exp.Value);
                }
                foreach (var maxEnergy in PlayerData.AlkahestryMaxEnergy)
                {
                    writer.Write(maxEnergy.Key);
                    writer.Write(maxEnergy.Value);
                }
                foreach (var currentEnergy in PlayerData.AlkahestryCurrentEnergy)
                {
                    writer.Write(currentEnergy.Key);
                    writer.Write(currentEnergy.Value);
                }
                foreach (var totalValue in PlayerData.TotalValueTransmuted)
                {
                    writer.Write(totalValue.Key);
                    writer.Write(totalValue.Value);
                }
                foreach (var totalValue in PlayerData.IsSlimeGivenToWizard)
                {
                    writer.Write(totalValue.Key);
                    writer.Write(totalValue.Value);
                }

                Networking.ServerSendTo(e.FarmerID, MSG_DATA, stream.ToArray());
            }
        }

        private void OnTransmutedValueMessage(IncomingMessage msg)
        {
            PlayerData.TotalValueTransmuted[msg.FarmerID] = msg.Reader.ReadInt32();
        }

        private void OnMaxEnergyMessage(IncomingMessage msg)
        {
            PlayerData.AlkahestryMaxEnergy[msg.FarmerID] = msg.Reader.ReadSingle();
        }

        private void OnCurrentEnergyMessage(IncomingMessage msg)
        {
            PlayerData.AlkahestryCurrentEnergy[msg.FarmerID] = msg.Reader.ReadSingle();
        }

        private void OnLevelMessage(IncomingMessage msg)
        {
            PlayerData.AlchemyLevel[msg.FarmerID] = msg.Reader.ReadInt32();
        }

        private void OnSlimeGivenToWizardMessage(IncomingMessage msg)
        {
            PlayerData.IsSlimeGivenToWizard[msg.FarmerID] = msg.Reader.ReadBoolean();
        }

        private void OnExpMessage(IncomingMessage msg)
        {
            PlayerData.AlchemyExperience[msg.FarmerID] = msg.Reader.ReadInt32();
            if (msg.FarmerID != Game1.player.UniqueMultiplayerID)
                return;
            if (Game1.player.GetCustomSkillExperience(skill) < PlayerData.AlchemyExperience[msg.FarmerID])
            {
                AddAlchemyExperience(PlayerData.AlchemyExperience[msg.FarmerID] - Game1.player.GetCustomSkillExperience(skill));
            }
        }

        // unabashedly stolen from spacechase, like all things.
        private void OnDataMessage(IncomingMessage msg)
        {
            Log.info("Receiving player data from server.");
            // Log.debug("Receiving updated data from server.");
            int count = msg.Reader.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                int level = msg.Reader.ReadInt32();
                // PlayerData.AlchemyLevel[id] = level;
            }

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                int experience = msg.Reader.ReadInt32();
                // PlayerData.AlchemyExperience[id] = experience;
                if (id == Game1.player.UniqueMultiplayerID)
                {
                    if (Game1.player.GetCustomSkillExperience(skill) < experience)
                    {
                        AddAlchemyExperience(experience - Game1.player.GetCustomSkillExperience(skill));
                    }
                }
            }

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                float maxEnergy = msg.Reader.ReadSingle();
                PlayerData.AlkahestryMaxEnergy[id] = maxEnergy;
            }

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                float currentEnergy = msg.Reader.ReadSingle();
                PlayerData.AlkahestryCurrentEnergy[id] = currentEnergy;
            }

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                int totalValueTransmuted = msg.Reader.ReadInt32();
                PlayerData.TotalValueTransmuted[id] = totalValueTransmuted;
            }

            for (int i = 0; i < count; ++i)
            {
                long id = msg.Reader.ReadInt64();
                bool isSlimeGivenToWizard = msg.Reader.ReadBoolean();
                PlayerData.IsSlimeGivenToWizard[id] = isSlimeGivenToWizard;
            }
        }

        static int lastTickTime = 0;  // The time at the last tick processed.
        public static int CurrentDefaultTickInterval => 7000 + (Game1.currentLocation?.getExtraMillisecondsPerInGameMinuteForThisLocation() ?? 0);
        public static int CurrentRegenResolution => CurrentDefaultTickInterval / 100;
        private static void RegenerateAlchemyBar()
        {
            //Log.debug("Regen debug out:");
            //Log.debug($"Game1.menuUp || Game1.paused || Game1.dialogueUp || Game1.activeClickableMenu != null || !Game1.shouldTimePass()");
            //Log.debug($"{Game1.menuUp}    {Game1.paused}    {Game1.dialogueUp}    {Game1.activeClickableMenu != null}    {!Game1.shouldTimePass() }");
            //checking for paused or menuUp doesn't return true for some reason, but this is
            //a reliable way to check to see if the player is in a menu to prevent regen.
            if (!Game1.shouldTimePass() || Game1.HostPaused)
                return;
            // Log.debug($"Game tick interval: {Game1.gameTimeInterval}");

            // it's important to point out that only the master game will ever have a gameTimeInterval > 0
            // this *never fires* for clients, which is why the server has to cascade a broadcast message to clients to DoRegenTick();
            int currentTime = Game1.gameTimeInterval;

            if (currentTime - lastTickTime < 0)
                lastTickTime = 0;
            int timeElapsed = currentTime - lastTickTime;
            if (timeElapsed > CurrentRegenResolution)
            {
                DoRegenTick();
                BroadcastRegenTick();
                lastTickTime = currentTime;
            }
        }

        private static void BroadcastRegenTick()
        {
            foreach (var farmer in Game1.otherFarmers)
            {
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    // arbitrary bool
                    writer.Write(true);
                    Networking.ServerSendTo(farmer.Key, MSG_REGEN_TICK, stream.ToArray());
                }
            }
        }

        private static void OnRegenTick(IncomingMessage msg)
        {
            var arbitraryBool = msg.Reader.ReadBoolean();
            DoRegenTick();
        }

        private static void DoRegenTick()
        {
            // handles this player's regen, super nerfed.
            double regenAlchemyBar = Math.Sqrt(AlchemyLevel + 1) / 10D;
            regenAlchemyBar *= MaxEnergy / 600D;
            CurrentEnergy = (float)Math.Min(CurrentEnergy + Math.Max(0.01D, regenAlchemyBar), MaxEnergy);
        }

        public static SaveDataModel PlayerData
        {
            get { return EquivalentExchange.instance.currentPlayerData; }
            set { EquivalentExchange.instance.currentPlayerData = value; }
        }        

        public static float CurrentEnergy
        {
            get
            {
                if (!PlayerData.AlkahestryCurrentEnergy.ContainsKey(PlayerId))
                    return 0F;
                // Log.debug($"Current energy is {PlayerData.AlkahestryCurrentEnergy[PlayerId]}");
                return PlayerData.AlkahestryCurrentEnergy[PlayerId];
            }
            set
            {
                if (!PlayerData.AlkahestryCurrentEnergy.ContainsKey(PlayerId) || PlayerData.AlkahestryCurrentEnergy[PlayerId] != value)
                {
                    PlayerData.AlkahestryCurrentEnergy[PlayerId] = value;
                    using (var stream = new MemoryStream())
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(value);
                        Networking.BroadcastMessage(MSG_CURRENT_ENERGY, stream.ToArray());
                    }
                }
            }
        }

        public static float MaxEnergy
        {
            get
            {
                if (!PlayerData.AlkahestryMaxEnergy.ContainsKey(PlayerId))
                    return 0F;
                // Log.debug($"Current alchemy max energy is {PlayerData.AlkahestryMaxEnergy[PlayerId]}");
                return PlayerData.AlkahestryMaxEnergy[PlayerId];
            }
            set
            {
                if (!PlayerData.AlkahestryMaxEnergy.ContainsKey(PlayerId) || PlayerData.AlkahestryMaxEnergy[PlayerId] != value)
                {
                    PlayerData.AlkahestryMaxEnergy[PlayerId] = value;
                    using (var stream = new MemoryStream())
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(value);
                        Networking.BroadcastMessage(MSG_MAX_ENERGY, stream.ToArray());
                    }
                }
            }
        }

        public static int TotalValueTransmuted
        {
            get
            {
                if (!PlayerData.TotalValueTransmuted.ContainsKey(PlayerId))
                    return 0;
                // Log.debug($"Current value transmuted is {PlayerData.TotalValueTransmuted[PlayerId]}");
                return PlayerData.TotalValueTransmuted[PlayerId];
            }
            set
            {
                if (!PlayerData.TotalValueTransmuted.ContainsKey(PlayerId) || PlayerData.TotalValueTransmuted[PlayerId] != value)
                {
                    PlayerData.TotalValueTransmuted[PlayerId] = value;
                    using (var stream = new MemoryStream())
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(value);
                        Networking.BroadcastMessage(MSG_TOTAL_VALUE_TRANSMUTED, stream.ToArray());
                    }
                }
            }
        }

        public static bool IsSlimeGivenToWizard
        {
            get
            {
                if (!PlayerData.IsSlimeGivenToWizard.ContainsKey(PlayerId))
                    return false;
                // Log.debug($"Current value transmuted is {PlayerData.TotalValueTransmuted[PlayerId]}");
                return PlayerData.IsSlimeGivenToWizard[PlayerId];
            }
            set
            {
                if (!PlayerData.IsSlimeGivenToWizard.ContainsKey(PlayerId) || PlayerData.IsSlimeGivenToWizard[PlayerId] != value)
                {
                    PlayerData.IsSlimeGivenToWizard[PlayerId] = value;
                    using (var stream = new MemoryStream())
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(value);
                        Networking.BroadcastMessage(MSG_IS_SLIME_GIVEN_TO_WIZARD, stream.ToArray());
                    }
                }
            }
        }

        public static long PlayerId
        {
            get { return Game1.player.uniqueMultiplayerID; }
        }

        public static void AddTotalValueTransmuted(int value)
        {
            TotalValueTransmuted += value;
            // Buffed formula for alchemy energy training. You start with 100 at a minimum.
            var updatedMaxEnergy = (int)Math.Floor(Math.Pow(TotalValueTransmuted / 5, 0.6)) + (AlchemyLevel * 25) + 100;
            MaxEnergy = updatedMaxEnergy;
        }

        // public static int AlchemyExperience => Game1.player.GetCustomSkillExperience(EquivalentExchange.skill);

        public static int AlchemyLevel => Game1.player.GetCustomSkillLevel(EquivalentExchange.skill);

        public static void AddAlchemyExperience(int exp)
        {
            Game1.player.AddCustomSkillExperience(EquivalentExchange.skill, exp);
        }
        
        //internal default value for the repeat rate starting point of the auto-fire functionality of transmute/liquidate when the buttons are held.
        private const int AUTO_REPEAT_UPDATE_RATE_REFRESH = 20;

        int heldCounter = 1;
        int updateTickCount = AUTO_REPEAT_UPDATE_RATE_REFRESH;

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            // Log.debug($"Update tick firing. Context isWorldReady returns { Context.IsWorldReady.ToString() }");
            if (!Context.IsWorldReady)
                return;
            RegenerateAlchemyBar();
            HandleHeldTransmuteKeysUpdateTick();

            // Detect (heuristically) whether the player gave the wizard a slime ball. This is the gateway for the rest of the mod.
            HandleWizardSlimeListener();
        }

        private void HandleHeldTransmuteKeysUpdateTick()
        {
            if (transmuteKeyHeld)
            {
                heldCounter++;
                if (heldCounter % updateTickCount == 0)
                {
                    HandleEitherTransmuteEvent(Config.TransmuteKey.ToString());
                    updateTickCount = (int)Math.Floor(Math.Max(1, updateTickCount * 0.9F));
                }
            }
        }        

        //fires when loading a save, initializes the item blacklist and loads player save data.
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            InitializePlayerData();
        }

        // the order of this list is not arbitrary - it starts at one end of the the transmutation map
        // and works its way to the other. Certain professions allow alternative inputs/outputs
        // or increase the number of "steps" away from a target you can get inputs.
        public static List<int> transmutationSteps = new List<int>
        {
            Reference.Items.IridiumOre,
            Reference.Items.GoldOre,
            Reference.Items.IronOre,
            Reference.Items.CopperOre,
            Reference.Items.Stone,
            Reference.Items.Clay,
            Reference.Items.Coal,
            Reference.Items.Sap,
            Reference.Items.Fiber,
            Reference.Items.Wood,
            Reference.Items.Hardwood
        };

        public static List<AlchemyTransmutationRecipe> GetTransmutationFormulas()
        {
            var recipes = new List<AlchemyTransmutationRecipe>();

            // iterate over each step in the transmutation "map"; by default, you can transmute into any object from 1 step away.
            foreach(var step in transmutationSteps)
            {
                var index = transmutationSteps.IndexOf(step);
                if (index > 0)
                {
                    recipes.AddRecipeLink(transmutationSteps[index - 1], step);
                }
                if (index < transmutationSteps.Count - 1)
                {
                    recipes.AddRecipeLink(transmutationSteps[index + 1], step);
                }

                // if the player has the "Sage" profession, they can traverse up to 2 steps away.
                if (index > 1 && HasProfession(AlchemySkill.ProfessionSage.GetVanillaId()))
                {
                    recipes.AddRecipeLink(transmutationSteps[index - 2], step);
                }
                if (index < transmutationSteps.Count - 2 && HasProfession(AlchemySkill.ProfessionSage.GetVanillaId()))
                {
                    recipes.AddRecipeLink(transmutationSteps[index + 2], step);
                }
                
                // if the player has the adept profession, you can transmute this thing into slimes no matter what, but transmutations cost double.
                if (HasProfession(AlchemySkill.ProfessionAdept.GetVanillaId()))
                {
                    recipes.AddRecipeLink(step, Reference.Items.Slime, 2);
                }

                // if the player has the conduit profession, you can create this thing from slimes no matter what, but transmutations cost double.
                if (HasProfession(AlchemySkill.ProfessionConduit.GetVanillaId()))
                {
                    recipes.AddRecipeLink(Reference.Items.Slime, step, 2);
                }

                // if the player has the shaper profession you can create stone and clay from slime and vice versa.
                if (HasProfession(AlchemySkill.ProfessionShaper.GetVanillaId()) && (step == Reference.Items.Stone || step == Reference.Items.Clay))
                {
                    recipes.AddRecipeLink(Reference.Items.Slime, step);
                    recipes.AddRecipeLink(step, Reference.Items.Slime);
                }

                // if the player has the transmuter profession you can create wood from slime and vice versa.
                if (HasProfession(AlchemySkill.ProfessionTransmuter.GetVanillaId()) && step == Reference.Items.Wood)
                {
                    recipes.AddRecipeLink(Reference.Items.Slime, step);
                    recipes.AddRecipeLink(step, Reference.Items.Slime);
                }

                // if the player has the aurumancer profession, you can create gold from slime and vice versa.
                if (HasProfession(AlchemySkill.ProfessionAurumancer.GetVanillaId()) && step == Reference.Items.GoldOre)
                {
                    recipes.AddRecipeLink(Reference.Items.Slime, step);
                    recipes.AddRecipeLink(step, Reference.Items.Slime);
                }
            }

            // a special handler for hay to and from fiber - unfortunately hay has a value of 0 so it breaks.
            //recipes.AddRecipeLink(Reference.Items.Hay, Reference.Items.Fiber);
            //recipes.AddRecipeLink(Reference.Items.Fiber, Reference.Items.Hay);

            foreach (var recipe in recipes)
            {
                var inputName = Util.GetItemName(recipe.InputId);
                var inputValue = Util.GetItemValue(recipe.InputId);
                var outputName = Util.GetItemName(recipe.OutputId);
                var outputValue = Util.GetItemValue(recipe.OutputId);
                //Log.debug($"Transmute: {recipe.GetInputCost()} {inputName} ({inputValue}) into {recipe.GetOutputQuantity()} {outputName} ({outputValue}), costs {recipe.Cost}");
            }
            return recipes;
        }

        public static bool HasProfession(int profession)
        {
            return Game1.player.professions.Contains(profession);
        }

        //handles reading current player json file and loading them into memory
        private void InitializePlayerData()
        {
            // save is loaded
            if (Context.IsWorldReady)
            {
                //fetch the alchemy save for this game file.
                if (!Game1.IsMultiplayer || Game1.IsMasterGame)
                    PlayerData = Helper.ReadJsonFile<SaveDataModel>(Path.Combine(Constants.CurrentSavePath, $"{Game1.uniqueIDForThisGame.ToString()}.json")) ?? new SaveDataModel();

                // if we are the player/host and we don't have a profile, let's make one for ourselves.
                var farmerId = Game1.player.uniqueMultiplayerID;
                if (!PlayerData.AlchemyLevel.ContainsKey(farmerId))
                    PlayerData.AlchemyLevel[farmerId] = 0;
                if (!PlayerData.AlchemyExperience.ContainsKey(farmerId))
                    PlayerData.AlchemyExperience[farmerId] = 0;
                // sync old experience values into the new system
                if (Game1.player.GetCustomSkillExperience(skill) < PlayerData.AlchemyExperience[farmerId])
                {
                    AddAlchemyExperience(PlayerData.AlchemyExperience[farmerId] - Game1.player.GetCustomSkillExperience(skill));
                }
                if (!PlayerData.AlkahestryCurrentEnergy.ContainsKey(farmerId))
                    PlayerData.AlkahestryCurrentEnergy[farmerId] = 0F;
                if (!PlayerData.AlkahestryMaxEnergy.ContainsKey(farmerId))
                    PlayerData.AlkahestryMaxEnergy[farmerId] = 0F;
                if (!PlayerData.TotalValueTransmuted.ContainsKey(farmerId))
                    PlayerData.TotalValueTransmuted[farmerId] = 0;
                if (!PlayerData.IsSlimeGivenToWizard.ContainsKey(farmerId))
                    PlayerData.IsSlimeGivenToWizard[farmerId] = false;

                // clean up deprecated profession data and use the new profession hooks
                ProfessionHelper.CleanDeprecatedProfessions();
            }
            Log.info("Player data loaded.");
        }

        //handles writing "each" player's json save to the appropriate file.
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            SavePlayerData();
        }

        private void SavePlayerData()
        {
            Log.info("Saving player data.");
            if (!Game1.IsMultiplayer || Game1.IsMasterGame)
                Helper.WriteJsonFile<SaveDataModel>(Path.Combine(Constants.CurrentSavePath, $"{ Game1.uniqueIDForThisGame.ToString()}.json"), PlayerData);
        }

        /// <summary>Update the mod's config.json file from the current <see cref="Config"/>.</summary>
        internal void SaveConfig()
        {
            Helper.WriteConfig(Config);
        }
        
        private static void GraphicsEvents_OnPreRenderHudEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // one of these counts as true whenever the player is in the bus stop...
            if (Game1.eventUp)
                return;

            // per the advice of Ento, abort if the player is in an event
            if (Game1.CurrentEvent != null)
                return;

            // if the player hasn't unlocked alchemy, skip
            if (!IsSlimeGivenToWizard)
                return;

            RenderAlchemyBarToHUD();
        }
        
        public static int GetSlimeValue()
        {
            return Util.GetItemValue(Reference.Items.Slime);
        }

        public static void RenderAlchemyBarToHUD()
        {
            int scale = 4;
            int alchemyBarWidth = DrawingUtil.alchemyBarSprite.Width * scale;
            int alchemyBarHeight = DrawingUtil.alchemyBarSprite.Height * scale;

            //special consideration for maps that are smaller than your display viewport (horizontally, this happens at the bus stop)
            int tileSizeWidth = Game1.player.currentLocation.Map.DisplayWidth;

            //apply special constraints in the event of small-ish maps here
            bool isPlayerOutdoors = Game1.player.currentLocation.IsOutdoors;

            //borders from the screen are viewport width / 2 - tileSizeWidth / 2
            int viewportBorderWidth = Math.Max(0, Game1.viewport.Width / 2 - tileSizeWidth / 2);

            int alchemyBarPositionX = Game1.viewport.Width - (isPlayerOutdoors ? viewportBorderWidth : 0) - alchemyBarWidth - 120;
            int alchemyBarPositionY = Game1.viewport.Height - alchemyBarHeight - 16;

            Vector2 alchemyBarPosition = new Vector2(alchemyBarPositionX, alchemyBarPositionY);

            Game1.spriteBatch.Draw(DrawingUtil.alchemyBarSprite, alchemyBarPosition, new Rectangle(0, 0, DrawingUtil.alchemyBarSprite.Width, DrawingUtil.alchemyBarSprite.Height), Color.White, 0, new Vector2(), scale, SpriteEffects.None, 1);
            if (CurrentEnergy > 0)
            {
                Rectangle targetArea = new Rectangle(3, 13, 6, 41);
                float perc = CurrentEnergy / MaxEnergy;
                int h = (int)(targetArea.Height * perc);
                targetArea.Y += targetArea.Height - h;
                targetArea.Height = h;

                targetArea.X *= 4;
                targetArea.Y *= 4;
                targetArea.Width *= 4;
                targetArea.Height *= 4;
                targetArea.X += (int)alchemyBarPosition.X;
                targetArea.Y += (int)alchemyBarPosition.Y;
                Game1.spriteBatch.Draw(DrawingUtil.alchemyBarFillSprite, targetArea, new Rectangle(0, 0, 1, 1), Color.White);

                int alchemyBarMaxX = alchemyBarPositionX + alchemyBarWidth;
                int alchemyBarMaxY = alchemyBarPositionY + alchemyBarHeight;
                //perform hover over manually
                if (Game1.getMouseX() >= alchemyBarPositionX && Game1.getMouseX() <= alchemyBarMaxX && Game1.getMouseY() >= alchemyBarPositionY && Game1.getMouseY() <= alchemyBarMaxY)
                {
                    string alkahestryEnergyString = $"{ ((int)Math.Floor(CurrentEnergy)).ToString()}/{ MaxEnergy.ToString()}";
                    float stringWidth = Game1.dialogueFont.MeasureString(alkahestryEnergyString).X;
                    Vector2 alkahestryEnergyStringPosition = new Vector2(alchemyBarPosition.X - stringWidth - 32, alchemyBarPosition.Y + 64);
                    Game1.spriteBatch.DrawString(Game1.dialogueFont, alkahestryEnergyString, alkahestryEnergyStringPosition, Color.White);
                }
            }

            // if the player is holding a transmutable item, let's show the current recipe, if it's available.

            //get the player's current item
            Item heldItem = Game1.player.CurrentItem;

            //player is holding item
            if (heldItem != null)
            {
                //get the item's ID
                int heldItemID = heldItem.parentSheetIndex;
                //abort any transmutation event for blacklisted items or items that for whatever reason can't exist in world.
                if (!GetTransmutationFormulas().HasItem(heldItemID))
                {
                    return;
                }                

                // get a list of recipes, valid or not, for the item we're holding
                var recipes = GetTransmutationFormulas().GetRecipesForOutput(heldItemID);

                // get an initial sprite position for the recipe list origin (top left) point
                var startingRecipeSpritePosition = new Vector2(alchemyBarPositionX - (136), alchemyBarPositionY);

                // loop over recipes, for displaying them in sequence
                // we "throttle" the max number of recipes we can iterate over (this list is sorted by validity!)
                // to avoid drawing off screen. At most it can draw as many as fits "in the alchemy bar's height"
                for (int i = 0; i < Math.Min(recipes.Count, (int)Math.Floor(alchemyBarHeight / 62D)); i++)
                {
                    // originally column index was intended to let recipes display two by two (or more)
                    // it made a mess so I scrapped it.
                    var columnIndex = 0;

                    // row index is just the index of the recipe, this one is still used.
                    var rowIndex = i;

                    // get the draw position offsets for the first item
                    var xOffset = (int)Math.Floor(columnIndex * -98D);
                    var yOffset = (int)Math.Floor(rowIndex * 62D);

                    // turn them into a vector
                    var bestRecipeSpritePosition = new Vector2(startingRecipeSpritePosition.X + xOffset, startingRecipeSpritePosition.Y + yOffset);                    

                    // fetch the recipe
                    var bestRecipe = recipes[i];                    

                    // display the current recipe beside the alchemy bar

                    // fetch the input item as an object representing the input and quantity desired by this recipe
                    Item inputItem = new StardewValley.Object(bestRecipe.InputId, bestRecipe.GetInputCost(), false);

                    // figure out if the player has the necessary inputs for this recipe
                    var hasInputs = Game1.player.hasItemInInventory(inputItem.parentSheetIndex, inputItem.Stack + 1);

                    // set the transparency of the renderer based on whether inputs are present, this is just visual flare.
                    var transparencyBasedOnValidity = hasInputs ? 1.0F : 0.5F;

                    // calls the vanilla menu item display code, it's perfect for this
                    inputItem.drawInMenu(Game1.spriteBatch, bestRecipeSpritePosition, 0.92F, transparencyBasedOnValidity, 0.875F, true, Color.White, false);

                    // create a font vector for the tiny arrow we draw pointing from left to right between the items. This is just visual flare.
                    // this hard-coded vector offset puts the arrow directly to the right of the quantity of the input, pointing right toward the output #
                    var fontVector = new Vector2(bestRecipeSpritePosition.X + 71, bestRecipeSpritePosition.Y + 44);

                    // draw the arrow at 0.7 scale of a normal dialogue font, we want it about the same size as the numbers... for visual flare.
                    Game1.spriteBatch.DrawString(Game1.dialogueFont, ">", fontVector, Color.White, 0F, new Vector2(0, 0), 0.70F, SpriteEffects.None, 0.89F);

                    // now get the vector for the output item
                    var outputRecipeSpritePosition = new Vector2(bestRecipeSpritePosition.X + 65, bestRecipeSpritePosition.Y);

                    // fetch the output item as an object representing the output and quantity produced by this recipe
                    Item outputItem = new StardewValley.Object(heldItemID, bestRecipe.GetOutputQuantity() + 1, false);

                    // finally, draw the output item
                    outputItem.drawInMenu(Game1.spriteBatch, outputRecipeSpritePosition, 0.92F, transparencyBasedOnValidity, 0.875F, true, Color.White, false);
                }                
            }
        }

        private static bool brokeRepeaterDueToNoEnergy = false;

        //handles the release key event for figuring out if control or shift is let go of
        public static void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
        {
            //let the app know the shift key is released
            if (e.KeyPressed == leftShiftKey || e.KeyPressed == rightShiftKey)
                SetModifyingControlKeyState(e.KeyPressed, false);

            //the key for transmuting is pressed, fire once and then initiate the callback routine to auto-fire.
            if (instance.Config.TransmuteKey.Equals(e.KeyPressed.ToString()))
            {
                brokeRepeaterDueToNoEnergy = false;
                transmuteKeyHeld = false;
                instance.heldCounter = 1;
                instance.updateTickCount = AUTO_REPEAT_UPDATE_RATE_REFRESH;
            }
        }

        //remembers the state of the mod control keys so we can do some fancy stuff.
        public static bool transmuteKeyHeld = false;

        //handles the key press event for figuring out if control or shift is held down, or either of the mod's major transmutation actions is being attempted.
        public static void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            // if the player hasn't unlocked the secrets yet, abort
            if (!IsSlimeGivenToWizard)
                return;

            //let the app know the shift key is held
            if (e.KeyPressed == leftShiftKey || e.KeyPressed == rightShiftKey)
                SetModifyingControlKeyState(e.KeyPressed, true);

            //the key for transmuting is pressed, fire once and then initiate the callback routine to auto-fire.
            if (instance.Config.TransmuteKey.Equals(e.KeyPressed.ToString()))
            {
                transmuteKeyHeld = true;
                HandleEitherTransmuteEvent(e.KeyPressed.ToString());
            }

            //the key pressed is one of the mods keys.. I'm doing this so I don't fire logic for anything unless either of the mod's keys were pressed.            
            if (instance.Config.NormalizeKey.Equals(e.KeyPressed.ToString()))
            {
                HandleEitherTransmuteEvent(e.KeyPressed.ToString());
            }
        }

        //sets up the basic structure of either transmute event, since they have some common ground
        private static void HandleEitherTransmuteEvent(string keyPressed)
        {
            // save is loaded
            if (Context.IsWorldReady)
            {
                //per the advice of Ento, abort if the player is in an event
                if (Game1.CurrentEvent != null)
                    return;

                //something may have gone wrong if this is null, maybe there's no save data?
                if (Game1.player != null)
                {
                    //get the player's current item
                    Item heldItem = Game1.player.CurrentItem;

                    //player is holding item
                    if (heldItem != null)
                    {
                        //get the item's ID
                        int heldItemID = heldItem.parentSheetIndex;

                        //alchemy energy can be used to execute a complex tool action if a tool is in hand.
                        if (heldItem is StardewValley.Tool && keyPressed.ToString() == instance.Config.TransmuteKey)
                        {
                            Tool itemTool = heldItem as Tool;

                            bool isScythe = itemTool is MeleeWeapon && itemTool.Name.ToLower().Contains("scythe");
                            bool isAxe = itemTool is Axe;
                            bool isPickaxe = itemTool is Pickaxe;
                            bool isHoe = itemTool is Hoe;
                            bool isWateringCan = itemTool is WateringCan;

                            bool canDoToolAlchemy = isScythe || isAxe || isPickaxe || isHoe || isWateringCan;

                            if (canDoToolAlchemy)
                            {
                                Alchemy.HandleToolTransmute(itemTool);
                            }
                        }

                        //try to normalize the item [make all items of a different quality one quality and exchange any remainder for gold]
                        if (keyPressed.ToString() == instance.Config.NormalizeKey)
                        {
                            Alchemy.HandleNormalizeEvent(heldItem);
                            return;
                        }

                        //abort any transmutation event for blacklisted items or items that for whatever reason can't exist in world.
                        if (!GetTransmutationFormulas().HasItem(heldItemID) || !heldItem.canBeDropped())
                        {
                            return;
                        }

                        //get the transmutation value, it's based on what it's worth to the player, including profession bonuses. This affects both cost and value.
                        int actualValue = ((StardewValley.Object)heldItem).sellToStorePrice();

                        //try to transmute [copy] the item
                        if (keyPressed.ToString() == instance.Config.TransmuteKey)
                        {
                            var shouldBreakOutOfRepeater = Alchemy.HandleTransmuteEvent(heldItem, actualValue);
                            if (shouldBreakOutOfRepeater && !brokeRepeaterDueToNoEnergy)
                            {
                                brokeRepeaterDueToNoEnergy = true;
                                instance.heldCounter = 1;
                                instance.updateTickCount = AUTO_REPEAT_UPDATE_RATE_REFRESH * 2;
                            }
                        }
                    }
                }
            }
        }

        //control key modifiers [shift and ctrl], I include both for a more robust "is either pressed" mechanic.
        public static bool leftShiftKeyPressed = false;
        public static bool rightShiftKeyPressed = false;

        //simple consts to keep code clean, both shift keys, both control keys.
        public const Keys leftShiftKey = Keys.LeftShift;
        public const Keys rightShiftKey = Keys.RightShift;

        //convenience methods for detecting when either keys are pressed to modify amount desired from liquidation/transmutes.
        public static bool IsShiftKeyPressed()
        {
            return leftShiftKeyPressed || rightShiftKeyPressed;
        }

        //handler for which flag to set when X key is pressed/released
        public static void SetModifyingControlKeyState(Keys keyChanged, bool isPressed)
        {
            switch (keyChanged)
            {
                case leftShiftKey:
                    leftShiftKeyPressed = isPressed;
                    break;
                case rightShiftKey:
                    rightShiftKeyPressed = isPressed;
                    break;
                default:
                    break;
            }
        }
    }
}