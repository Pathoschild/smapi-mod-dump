/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using StardewValley.Tools;
using System;
using System.Collections.Generic;

using Force.DeepCloner;

using StardewDruid.Monster;

using StardewDruid.Event;

namespace StardewDruid
{

    public class Mod : StardewModdingAPI.Mod
    {

        public ModData Config;

        public ActiveData activeData;

        private StaticData staticData;

        public bool questSync;

        public Dictionary<int, string> weaponAttunement;

        private MultiplayerData multiplayerData;

        public Dictionary<int, string> TreeTypes;

        public Dictionary<string, Event.EventHandle> eventRegister;

        public Dictionary<string, TriggerHandle> markerRegister;

        public Dictionary<string, Character.TrackHandle> trackRegister;

        public List<string> warpCasts;

        public List<string> fireCasts;

        public Dictionary<string, int> rockCasts;

        public Dictionary<string, Dictionary<Vector2,string>> targetCasts;

        public Dictionary<string, Dictionary<Vector2,string>> terrainCasts;

        public Dictionary<string, Dictionary<Vector2,int>> featureCasts;

        private Dictionary<string, Map.Quest> questIndex;

        public int updateRite;

        public int updateEvent;

        public int performAction;

        public double messageBuffer;

        public StardewValley.Tools.Pickaxe virtualPick;

        public StardewValley.Tools.Axe virtualAxe;

        public StardewValley.Tools.Hoe virtualHoe;

        public StardewValley.Tools.WateringCan virtualCan;

        public int currentTool;

        public Dictionary<string, string> locationPoll;

        public Dictionary<string, MonsterHandle> monsterHandles;

        public Dictionary<string, StardewDruid.Character.Character> characters;

        public Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue;

        private bool trainedToday;

        public List<string> triggerList;

        public bool receivedData;

        public string mineShaftName;

        public Dictionary<string,List<int>> riteWitnesses;

        internal static Mod instance;

        override public void Entry(IModHelper helper)
        {

            instance = this;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += SaveLoaded;

            //helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.GameLoop.OneSecondUpdateTicked += EverySecond;

            helper.Events.GameLoop.UpdateTicked += EveryTicked;

            helper.Events.GameLoop.Saving += SaveImminent;

            helper.Events.GameLoop.Saved += SaveUpdated;

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {

            Config = Helper.ReadConfig<ModData>();

            ConfigMenu.MenuConfig(this);
            
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            if (Context.IsMainPlayer) 
            {

                staticData = Helper.Data.ReadSaveData<StaticData>("staticData");

            }
            else if(!receivedData)
            {

                StaticData loadData = new();

                Helper.Multiplayer.SendMessage(loadData, "FarmhandRequest", modIDs: new[] { this.ModManifest.UniqueID });

            }

            staticData ??= new StaticData() {  staticVersion = Map.QuestData.StableVersion() };
            
            StaticChecks();

            Helper.Data.WriteJsonFile("staticData.json", staticData);

            questIndex = Map.QuestData.QuestList();

            characters = new();

            dialogue = new();

            ReadyState();

            if (Context.IsMainPlayer)
            {

                CreateCharacters();

            }

        }

        private void StaticChecks()
        {

            int stableVersion = Map.QuestData.StableVersion();

            if (Config.setProgress != -1)
            {
                
                if (!staticData.blessingList.ContainsKey("setProgress"))
                {

                    staticData = new StaticData() { staticVersion = stableVersion };

                    staticData = Map.QuestData.ConfigureProgress(staticData, Config.setProgress);

                    return;

                } 
                
                if(staticData.blessingList["setProgress"] != Config.setProgress)
                {

                    staticData = new StaticData() { staticVersion = stableVersion };

                    staticData = Map.QuestData.ConfigureProgress(staticData, Config.setProgress);

                    return;
                
                }

            }

            if (staticData.staticVersion != stableVersion)
            {

                staticData = Map.QuestData.QuestCheck(staticData);

                staticData.staticVersion = stableVersion;

            }

            List<string> disabledEffects = DisabledEffects();

            foreach(string effect in disabledEffects)
            {

                string effectString = "forget" + effect;

                if (!staticData.toggleList.ContainsKey(effectString))
                {
                    
                    staticData.toggleList[effectString] = 1;

                }

            }

        }

        private void SaveImminent(object sender, SavingEventArgs e)
        {
            if(Context.IsMainPlayer)
            {
                
                Helper.Data.WriteSaveData("staticData", staticData);

            }
            else
            {
                
                Helper.Multiplayer.SendMessage(staticData, "FarmhandSave", modIDs: new[] { this.ModManifest.UniqueID });

            }

            foreach (KeyValuePair<string, Event.EventHandle> challengeEntry in eventRegister)
            {

                challengeEntry.Value.EventRemove();

            }

            eventRegister.Clear();

            foreach (KeyValuePair<string, Event.TriggerHandle> markerEntry in markerRegister)
            {

                markerEntry.Value.EventRemove();

            }

            markerRegister.Clear();

            //lockoutRegister.Clear();

            trackRegister.Clear();

            foreach (KeyValuePair<string, MonsterHandle> monsterEntry in monsterHandles)
            {

                monsterEntry.Value.ShutDown();

            }
                
            activeData.castInterrupt = true;

            dialogue.Clear();

            if(characters.Count > 0)
            {

                foreach(KeyValuePair<string, StardewDruid.Character.Character> characterPair in characters)
                {

                    if (characterPair.Value.currentLocation != null)
                    {
                        characterPair.Value.currentLocation.characters.Remove(characterPair.Value);
                    
                    }

                    //Monitor.Log(characterPair.Value.currentLocation.Name, LogLevel.Debug);

                }

                characters.Clear();

            }

            if (Game1.buffsDisplay.otherBuffs.Count > 0)
            {

                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {

                    buff.removeBuff();

                }

            }

            Game1.buffsDisplay.otherBuffs.Clear();

            Game1.player.MagneticRadius = 128;

        }

        private void SaveUpdated(object sender, SavedEventArgs e)
        {

            CreateCharacters();

            ReadyState();

        }

        public void MultiplayerMessage(string requestString, long multiplayerId)
        {

            StaticData messageData = new();

            messageData.staticId = multiplayerId;

            Helper.Multiplayer.SendMessage(messageData, requestString, modIDs: new[] { this.ModManifest.UniqueID });

        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {

            if (e.FromModID == ModManifest.UniqueID)
            {

                if (Context.IsMainPlayer)
                {

                    if (e.Type == "FarmhandSave")
                    {
                        StaticData farmhandData = e.ReadAs<StaticData>();

                        multiplayerData ??= Helper.Data.ReadSaveData<MultiplayerData>("multiplayerData");

                        multiplayerData ??= new MultiplayerData();

                        multiplayerData.farmhandData[e.FromPlayerID] = farmhandData;

                        Helper.Data.WriteSaveData("multiplayerData", multiplayerData);

                        //Game1.addHUDMessage(new HUDMessage($"Saved Stardew Druid data for Farmer ID {e.FromPlayerID}", ""));

                        Console.WriteLine($"Saved Stardew Druid data for Farmer ID {e.FromPlayerID}");

                    }

                    if (e.Type == "FarmhandRequest")
                    {
                        multiplayerData ??= Helper.Data.ReadSaveData<MultiplayerData>("multiplayerData");

                        multiplayerData ??= new MultiplayerData();

                        StaticData farmhandData;

                        if (multiplayerData.farmhandData.ContainsKey(e.FromPlayerID))
                        {

                            farmhandData = multiplayerData.farmhandData[e.FromPlayerID];

                        }
                        else
                        {

                            farmhandData = new StaticData();

                        }

                        farmhandData.staticId = e.FromPlayerID;

                        this.Helper.Multiplayer.SendMessage(farmhandData, "FarmhandLoad", modIDs: new[] { this.ModManifest.UniqueID });

                        //Game1.addHUDMessage(new HUDMessage($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}", ""));
                        Console.WriteLine($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}");
                    }

                }
                else if (e.Type == "FarmhandLoad" || e.Type == "FarmhandTrain")
                {

                    StaticData farmhandData = e.ReadAs<StaticData>();

                    if (farmhandData.staticId == Game1.player.UniqueMultiplayerID)
                    {

                        if (e.Type == "FarmhandTrain")
                        {

                            if (trainedToday) { return; }

                            trainedToday = true;

                        }

                        staticData = farmhandData;

                        StaticChecks();

                        //Game1.addHUDMessage(new HUDMessage($"Received Stardew Druid data for Farmer ID {e.FromPlayerID}", ""));
                        Console.WriteLine($"Received Stardew Druid data for Farmer ID {e.FromPlayerID}");

                        ReadyState();

                        receivedData = true;
                        
                    }

                }

            }
        
        }

        public void ReadyState()
        {

            weaponAttunement = Config.weaponAttunement.DeepClone();

            foreach (KeyValuePair<int, string> kvp in staticData.weaponAttunement)
            {

                weaponAttunement[kvp.Key] = kvp.Value;

            }

            triggerList = new();

            activeData = new ActiveData() { activeBlessing = staticData.activeBlessing };

            eventRegister = new();

            //lockoutRegister = new();

            markerRegister = new();

            trackRegister = new();

            monsterHandles = new();

            rockCasts = new();

            targetCasts = new();

            featureCasts = new();

            terrainCasts = new();

            warpCasts = new();

            fireCasts = new();

            RiteTool(991);

            RiteTool(992);

            RiteTool(993);

            RiteTool(994);

            locationPoll = new();

            riteWitnesses = new();

            // ---------------------- trigger assignment

            SynchroniseQuest();

            return;

        }

        public void CreateCharacters()
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            foreach (KeyValuePair<string, string> characterInfo in staticData.characterList)
            {

                CharacterData.CharacterLoad(characterInfo.Key, characterInfo.Value);

            }

        }

        public void SynchroniseQuest()
        {

            // load milestone quests

            if (Context.IsMainPlayer)
            {

                Map.QuestData.IntroductionQuests();

            }

            // once per game play quest check
            if (!questSync)
            {
                Dictionary<int, string> questIds = new();

                foreach (KeyValuePair<string, StardewDruid.Map.Quest> questData in questIndex)
                {

                    if (questData.Value.questId != 0)
                    {

                        questIds.Add(questData.Value.questId, questData.Key);

                    }

                }

                List<string> duplicateIds = new();

                for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
                {

                    int gameId = Game1.player.questLog[num].id.Value;

                    if (questIds.ContainsKey(gameId)) // valid
                    {

                        string questName = questIds[gameId];

                        if (staticData.questList.ContainsKey(questName))
                        {

                            // player has a duplicate quest
                            if (duplicateIds.Contains(questName))
                            {

                                Game1.player.questLog.RemoveAt(num);

                                continue;

                            }

                            duplicateIds.Add(questName);

                            // player can see unfinished quest but mod has already turned over
                            if (staticData.questList[questName] && !Game1.player.questLog[num].completed.Value)
                            {
                                staticData.questList[questName] = false;

                                continue;

                            }

                            // player has finished quest according to game but mod has not turned over
                            if (!staticData.questList[questName] && Game1.player.questLog[num].completed.Value)
                            {

                                Game1.player.questLog.RemoveAt(num);

                                RegisterQuest(questName);

                            }

                        }
                        else
                        {
                            // mod has not initiated this quest yet - possible duplicate or reset progress
                            Game1.player.questLog.RemoveAt(num);

                        }

                    }

                }

                questSync = true;

            }

            // populate trigger and task lists
            foreach (KeyValuePair<string, bool> questPair in staticData.questList)
            {

                if (questPair.Value)
                {
                    continue;
                }

                ReassignQuest(questPair.Key);

            }


        }

        private static bool CasterBusy()
        {
            if (Game1.eventUp)
            {
                return true;
            }

            if (Game1.fadeToBlack)
            {
                return true;
            }

            if (Game1.currentMinigame != null)
            {
                return true;
            }

            if (Game1.activeClickableMenu != null)
            {
                return true;
            }

            if (Game1.isWarping)
            {
                return true;
            }

            if (Game1.killScreen)
            {
                return true;
            }

            if (Game1.player.freezePause > 0)
            {
                return true;
            }

            return false;

        }

        public void SetTriggers()
        {

            foreach (KeyValuePair<string, TriggerHandle> markerHandle in markerRegister)
            {

                markerHandle.Value.EventRemove();

            }

            markerRegister.Clear();

            foreach (string castString in triggerList)
            {

                Map.Quest questData = questIndex[castString];

                QuestData.MarkerInstance(Game1.player.currentLocation, questData);

            }

        }

        private void EverySecond(object sender, OneSecondUpdateTickedEventArgs e)
        {

            if (!Context.IsWorldReady)
            {

                return;

            }

            updateEvent = 6;

            if (!LocationPoll("trigger"))
            {

                SetTriggers();

            }
            //Monitor.Log(Game1.player.currentLocation.characters.First().Name, LogLevel.Debug);
            if (eventRegister.Count == 0 && markerRegister.Count == 0)
            {
                return;
            }

            bool exitAll = false;

            bool extendAll = false;

            List<string> removeList = new();

            if (Game1.eventUp || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen) // Game1.fadeToBlack
            {

                exitAll = true;

            }

            if (!Game1.shouldTimePass() || !Game1.game1.IsActive)
            {

                extendAll = true;

            }

            if (markerRegister.Count > 0 && !extendAll)
            {

                foreach (KeyValuePair<string, TriggerHandle> markerHandle in markerRegister)
                {

                    markerHandle.Value.EventInterval();

                }

            }

            if (eventRegister.Count == 0)
            {
                
                return;
            
            }

            List<string> iterateList = new();

            foreach(KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
            { 
            
                iterateList.Add(eventEntry.Key);
            
            }

            foreach (string eventKey in iterateList)
            {

                Event.EventHandle eventEntry = eventRegister[eventKey];

                if (exitAll)
                {

                    eventEntry.EventAbort();

                    eventEntry.EventRemove();

                    continue;

                }

                if (extendAll)
                {

                    eventEntry.EventExtend();

                    continue;

                }

                if (!eventEntry.EventActive())
                {

                    removeList.Add(eventKey);

                    eventEntry.EventRemove();

                    continue;

                }

                eventEntry.EventInterval();

            }

            if (exitAll)
            {

                eventRegister.Clear();

                //lockoutRegister.Clear();

                return;
            }

            foreach (string removeChallenge in  removeList) 
            { 
            
                eventRegister.Remove(removeChallenge);

            }

            /*if (!eventRegister.ContainsKey("active"))
            {

                lockoutRegister.Clear();

            }*/

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // Game is not ready
            if (!Context.IsWorldReady)
            {

                return;

            }

            // action press
            if(eventRegister.Count > 0 && performAction == 0)
            {

                if (Game1.didPlayerJustLeftClick() || Config.actionButtons.GetState() == SButtonState.Pressed) //&& Config.riteButtons.GetState() == SButtonState.Held)
                {

                    foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
                    {

                        if (eventEntry.Value.EventPerformAction())
                        {

                            performAction = 60;

                            return;

                        }

                    }

                }

            }

            // track wrong key
            if (Config.riteButtons.GetState() != SButtonState.Pressed)
            {

                return;

            }

            /*if (Config.overrideKeypress)
            {

                suppressedButtons.Clear();

                Helper.Input.Suppress(e.Button);

                suppressedButtons.Add(e.Button);

            }*/

            // unable to cast rite at this time
            if (CasterBusy())
            {
                activeData.castInterrupt = true;

                return;

            }

            ResetCast();

        }

        private void EveryTicked(object sender, UpdateTickedEventArgs e)
        {
            
            // Game is not ready
            if (!Context.IsWorldReady)
            {

                return;

            }

            // caster is busy
            if (CasterBusy())
            {

                return;

            }

            // rite timer
            updateRite = Math.Max(0,updateRite-1);

            // check if able to cast
            if (!activeData.castInterrupt)
            {

                if (updateRite <= 0)
                {

                    UpdateRite();

                    updateRite = 40;

                }

            }

            updateEvent = Math.Max(0, updateEvent - 1);

            if (updateEvent <= 0)
            {

                if (eventRegister.Count > 0)
                {
                    foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
                    {

                        eventEntry.Value.EventDecimal();

                    }

                }

                if (trackRegister.Count > 0)
                {
                    foreach (KeyValuePair<string, Character.TrackHandle> trackEntry in trackRegister)
                    {

                        trackEntry.Value.TrackPlayer();

                    }

                }

                updateEvent = 6;

            }

            if(performAction > 0)
            {

                performAction--;

            }

        }

        private void UpdateRite()
        {

            /*bool suppressHeld = false;

            if (Config.overrideKeypress)
            {

                if (suppressedButtons.Count > 0)
                {
                    
                    if (Helper.Input.IsSuppressed(suppressedButtons.First()))
                    {

                        suppressHeld = true;

                    }
                    else
                    {

                        suppressedButtons.Clear();

                    }
                    
                }

            }*/

            // check if initial cast or sustained cast
            //if (!(activeData.castLevel == 0 || Config.riteButtons.GetState() == SButtonState.Held) || suppressHeld)
            if (!(activeData.castLevel == 0 || Config.riteButtons.GetState() == SButtonState.Held))
            {

                return;

            }

            if (Config.slotAttune)
            {

                // check if valid tool and valid location are still selected
                string slot = AttuneableSlot();

                if (activeData.activeBlessing != slot || Game1.player.currentLocation.Name != activeData.activeLocation)
                {

                    if (!ResetCast())
                    {

                        return;

                    }

                }

            }
            else
            {

                // check if valid tool and valid location are still selected
                int toolIndex = AttuneableWeapon();

                if (activeData.toolIndex != toolIndex || Game1.player.currentLocation.Name != activeData.activeLocation)
                {

                    if (!ResetCast())
                    {

                        return;

                    }

                }

            }

            // create rite to invoke
            Rite castRite = NewRite();

            castRite.CastVector();

            // trigger check 
            if (activeData.castLevel == 0)
            {

                if (CheckTrigger(castRite))
                {
                    
                    activeData.castInterrupt = true;

                    return;

                }

            }

            // unable to cast if no rite selected
            if (activeData.activeBlessing == "none")
            {

                CastMessage("Nothing happened... ");

                activeData.castInterrupt = true;

                return;

            }


            // unable to cast if game location has no spawn profile
            if (activeData.spawnIndex.Count == 0)
            {

                CastMessage("Unable to reach the otherworldly plane from this location");

                activeData.castInterrupt = true;

                return;

            }

            // check player has enough energy for eventual costs
            if (Game1.player.Stamina <= 32 || Game1.player.health <= 50)
            {
                
                AutoConsume();

                if (Game1.player.Stamina <= 32)
                {

                    if (activeData.castLevel > 0)
                    {
                        CastMessage("Not enough energy to continue rite", 3);

                    }
                    else
                    {
                        CastMessage("Not enough energy to perform rite", 3);

                    }

                    activeData.castInterrupt = true;

                    return;

                }

            }

            // invoke rite
            castRite.CastRite();

            // increment level
            activeData.castLevel++;

            return;

        }

        private bool CheckTrigger(Rite rite)
        {

            if (eventRegister.ContainsKey("active"))
            {
                return false;
            }

            if (!LocationPoll("trigger"))
            {

                SetTriggers();

            }

            if (markerRegister.Count == 0)
            {
                return false;
            }

            foreach (KeyValuePair<string,TriggerHandle> marker in markerRegister)
            {

                if (marker.Value.CheckMarker(rite))
                {

                    markerRegister.Clear();

                    triggerList.Remove(marker.Key);

                    locationPoll["trigger"] = null;

                    return true;

                }
            
            }

            return false;

        }

        private bool ResetCast()
        {

            int toolIndex = AttuneableWeapon();

            // unable to cast if player does not have a valid tool
            if (toolIndex == -1)
            {
                CastMessage("Rite requires a melee weapon or tool");

                activeData.castInterrupt = true;

                return false;

            }

            // default rite choice
            string activeBlessing = staticData.activeBlessing;

            if (Config.slotAttune)
            {

                // check if valid tool and valid location are still selected
                activeBlessing = AttuneableSlot();

                if(activeBlessing == "none")
                {

                    CastMessage("No rite attuned to slot "+(Game1.player.CurrentToolIndex + 1));

                    return false;

                }

            }
            else
            {

                // check if rite override
                if (weaponAttunement.ContainsKey(toolIndex))
                {

                    activeBlessing = weaponAttunement[toolIndex];

                    // player must have rite unlocked
                    if (!staticData.blessingList.ContainsKey(activeBlessing))
                    {

                        CastMessage("I'm not attuned to this artifact... perhaps the Effigy can help");

                        activeData.castInterrupt = true;

                        return false;

                    }

                }

            }

            // unable to cast if lockout
            /*if (lockoutRegister.ContainsKey(activeBlessing))
            {

                if (eventRegister.ContainsKey("active"))
                {

                    CastMessage("Something is interfering with the rite!");

                    activeData.castInterrupt = true;

                    return false;

                }
                else
                {

                    lockoutRegister.Remove(activeBlessing);

                }

            }*/

            // create fresh cast sheet
            activeData = new ActiveData()
            {
                
                activeBlessing = activeBlessing,

                toolIndex = toolIndex,

                spawnIndex = Map.SpawnData.SpawnIndex(Game1.player.currentLocation),

                activeLocation = Game1.player.currentLocation.Name,

                activeVector = Game1.player.getTileLocation(),

                activeDirection = Game1.player.facingDirection,

                castInterrupt = false,
    
            };

            return true;

        }

        public void CastMessage(string message, int type = 2)
        {

            if (messageBuffer < Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {
                string stringType = type.ToString();

                if (type == -1)
                {

                    stringType = "";

                }

                Game1.addHUDMessage(new HUDMessage(message, stringType));

                messageBuffer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            }

        }

        public string AttuneableSlot()
        {

            string blessing = "none";

            switch (Game1.player.CurrentToolIndex % 12)
            {

                case 0:

                    blessing = "earth";

                    break;

                case 1:

                    blessing = "water";

                    break;

                case 2:

                    blessing = "stars";

                    break;

                case 3:

                    blessing = "fates";

                    break;
            }

            if (staticData.blessingList.ContainsKey(blessing))
            {

                return blessing;

            }

            return "none";

        }

        public int AttuneableWeapon()
        {

            if (Game1.player.CurrentTool is null)
            {
                return -1;
            }

            if (Game1.player.CurrentTool is not Tool)
            {
                return -1;
            }

            int toolIndex = -1;

            if (Game1.player.CurrentTool is Pickaxe)
            {
                toolIndex = 991;

                if(currentTool != toolIndex){RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel);}

            }
            else if (Game1.player.CurrentTool is Axe)
            {
                toolIndex = 992;

                if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
            }
            else if (Game1.player.CurrentTool is Hoe)
            {
                toolIndex = 993;

                if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
            }
            else if (Game1.player.CurrentTool is WateringCan)
            {
                toolIndex = 994;

                if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
            }
            else if (Game1.player.CurrentTool is MeleeWeapon)
            {
                toolIndex = Game1.player.CurrentTool.InitialParentTileIndex;
            }

            currentTool = toolIndex;

            return toolIndex;

        }

        public Rite NewRite(bool update = true)
        {

            Rite newRite = new()
            {

                blessingList = new Dictionary<string,int>(staticData.blessingList),

                castTask = new Dictionary<string, int>(staticData.taskList),

                castToggle = new Dictionary<string, int>(staticData.toggleList),

                castLevel = activeData.castLevel.ShallowClone(),

                castType = activeData.activeBlessing.ShallowClone(),

                castVector = activeData.activeVector.ShallowClone(),

                castBuffs = Config.castBuffs,

            };

            newRite.CastDamage(Config.combatDifficulty);

            if (update)
            {

                newRite.CastVector();

                activeData.activeVector = newRite.castVector.ShallowClone();

            }

            return newRite;

        }

        public int DamageLevel()
        {

            int damageLevel = 175;

            if (!Config.maxDamage)
            {

                damageLevel = 5 * Game1.player.CombatLevel;

                damageLevel += 1 * Game1.player.MiningLevel;

                damageLevel += 1 * Game1.player.ForagingLevel;

                damageLevel += 5 * Mod.instance.virtualAxe.UpgradeLevel;

                damageLevel += 5 * Mod.instance.virtualPick.UpgradeLevel;

                if (Game1.player.CurrentTool is Tool currentTool)
                {
                    if (currentTool.enchantments.Count > 0)
                    {
                        damageLevel += 25;
                    }
                }

                if (Game1.player.professions.Contains(24))
                {
                    damageLevel += 15;
                }

                if (Game1.player.professions.Contains(26))
                {
                    damageLevel += 15;
                }

            }

            return damageLevel;

        }

        public List<string> DisabledEffects()
        {
            List<string> disabled = new();

            if (Config.disableSeeds) { disabled.Add("Seeds"); }


            if (Config.disableTrees) { disabled.Add("Trees"); }


            if (Config.disableWildspawn) { disabled.Add("Wildspawn"); }


            if (Config.disableFish) { disabled.Add("Fish"); }

            return disabled;
        }

        public string ActiveBlessing()
        {

            return staticData.activeBlessing;

        }

        public Dictionary<string, int> BlessingList()
        {

            return new Dictionary<string, int>(staticData.blessingList);

        }

        public bool HasBlessing(string blessing)
        {

            return staticData.blessingList.ContainsKey(blessing);

        }

        public void ChangeBlessing(string blessing)
        {
            
            staticData.activeBlessing = blessing;

            activeData = new() { activeBlessing = blessing, castInterrupt = true, };

        }

        public void UpdateBlessing(string blessing)
        {

            //staticData.activeBlessing = blessing;

            //activeData = new() { activeBlessing = blessing, castInterrupt = true, };

            if (!staticData.blessingList.ContainsKey(blessing))
            {

                staticData.blessingList[blessing] = 0;

            }

        }

        public void LevelBlessing(string blessing)
        {

            staticData.blessingList[blessing] += 1;

        }

        public Dictionary<string, bool> QuestList()
        {

            return new Dictionary<string, bool>(staticData.questList);

        }

        public void RegisterEvent(Event.EventHandle eventHandle, string placeHolder)
        {

            if (eventRegister.ContainsKey(placeHolder))
            {

                eventRegister[placeHolder].EventRemove();

            }

            eventRegister[placeHolder] = eventHandle;

        }

        public bool QuestComplete(string quest)
        {

            if (staticData.questList.ContainsKey(quest))
            {

                return staticData.questList[quest];

            }

            return false;

        }

        public bool QuestOpen(string quest)
        {
            
            if (staticData.questList.ContainsKey(quest))
            {
                
                return staticData.questList[quest] == false;

            
            }

            return false;

        }

        public bool QuestGiven(string quest)
        {

            return staticData.questList.ContainsKey(quest);

        }

        public bool QuestCompletion()
        {
            
            return staticData.questList.ContainsValue(false);

        }

        public void NewQuest(string quest)
        {

            if (!staticData.questList.ContainsKey(quest))
            {

                staticData.questList[quest] = false;

            }

            Map.Quest questData = questIndex[quest];

            if (questData.questId != 0)
            {

                for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
                {
                    if (Game1.player.questLog[num].id.Value == questData.questId)
                    {

                        if (Game1.player.questLog[num].completed.Value)
                        {
                            Game1.player.questLog.RemoveAt(num);

                        }

                    }

                }

            }

            RegisterQuest(quest);

            ReassignQuest(quest);

        }

        public void RegisterQuest(string quest)
        {
            Map.Quest questData = questIndex[quest];

            StardewValley.Quests.Quest newQuest = new();

            newQuest.questType.Value = questData.questValue;
            newQuest.id.Value = questData.questId;
            newQuest.questTitle = questData.questTitle;
            newQuest.questDescription = questData.questDescription;
            newQuest.currentObjective = questData.questObjective;
            newQuest.showNew.Value = true;
            newQuest.moneyReward.Value = questData.questReward;

            Game1.player.questLog.Add(newQuest);

        }

        public void ReassignQuest(string quest)
        {

            Map.Quest questData = questIndex[quest];

            if (questData.type != null)
            {

                if(!triggerList.Contains(quest))
                {
                    
                    triggerList.Add(quest);

                    locationPoll["trigger"] = null;

                }

            }

            if (questData.taskCounter != 0)
            {

                if (!staticData.taskList.ContainsKey(quest))
                {

                    staticData.taskList[quest] = 0;

                }

                if (staticData.taskList.ContainsKey(questData.taskFinish))
                {

                    staticData.taskList.Remove(questData.taskFinish);

                }

            }

        }

        public void CompleteQuest(string quest)
        {

            Map.Quest questData = questIndex[quest];

            staticData.questList[quest] = true;

            if (questData.questId != 0)
            {

                Game1.player.completeQuest(questData.questId);

            }

            if (questData.type != null)
            {

                if (triggerList.Contains(quest))
                {

                    triggerList.Remove(quest);

                    locationPoll["trigger"] = null;

                }

            }

            if(questData.taskFinish != null)
            {

                staticData.taskList[questData.taskFinish] = 1;

            }

        }

        public void RemoveQuest(string quest)
        {
            
            Map.Quest questData = questIndex[quest];
            
            if (questData.type != null)
            {

                if (triggerList.Contains(quest))
                {

                    triggerList.Remove(quest);

                    locationPoll["trigger"] = null;

                }

            }

            if (staticData.questList.ContainsKey(quest))
            {

                staticData.questList.Remove(quest);

            }

            SynchroniseQuest();

        }

        public int UpdateTask(string quest, int update)
        {

            Map.Quest questData = questIndex[quest];

            if (questData.taskCounter == 0)
            {
                return -1;
            }

            if (!staticData.questList.ContainsKey(quest))
            {
                NewQuest(quest);    
            }

            if(!staticData.taskList.ContainsKey(quest))
            {
                ReassignQuest(quest);
            }

            if (staticData.questList[quest])
            {
                return -1;
            }

            staticData.taskList[quest] += update;

            if(staticData.taskList[quest] >= questData.taskCounter)
            {
                CompleteQuest(quest);
            }

            return staticData.taskList[quest];

        }

        public void TaskSet(string task, int set)
        {

            staticData.taskList[task] = set;

        }

        public string CastControl()
        {

            return Config.riteButtons.ToString();

        }

        public void UnlockAll()
        {

             staticData = new StaticData() { staticVersion = Map.QuestData.StableVersion() };

             staticData = Map.QuestData.ConfigureProgress(staticData, Map.QuestData.MaxProgress());

        }

        public bool LocationPoll(string entryKey)
        {

            string location = Game1.player.currentLocation.Name;

            if (locationPoll.ContainsKey(entryKey))
            {

               if(locationPoll[entryKey] == location)
               {

                    return true;

                }

            }

            locationPoll[entryKey] = location;

            return false;

        }

        public void RiteTool(int toolIndex,int setLevel = -1)
        {

            int level = 1;

            string toolBlessing = "tool" + toolIndex.ToString();

            if (staticData.blessingList.ContainsKey(toolBlessing))
            {

                level = staticData.blessingList[toolBlessing];

            }

            if (setLevel != -1)
            {

                level = setLevel;

            }

            if (Config.maxDamage)
            {

                level = 5;

            }

            staticData.blessingList[toolBlessing] = level;

            switch (toolIndex)
            {

                case 991:

                    if(virtualPick == null)
                    {

                        virtualPick = new Pickaxe();
                        virtualPick.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
                        Game1.player.Stamina += Math.Min(2, Game1.player.MaxStamina - Game1.player.Stamina);
                    }

                    virtualPick.UpgradeLevel = level;

                    return;

                case 992:

                    if (virtualAxe == null)
                    {

                        virtualAxe = new Axe();
                        virtualAxe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
                        Game1.player.Stamina += Math.Min(2, Game1.player.MaxStamina - Game1.player.Stamina);

                    }

                    virtualAxe.UpgradeLevel = level;

                    return;

                case 993:

                    if (virtualHoe == null)
                    {

                        virtualHoe = new Hoe();
                        virtualHoe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
                        Game1.player.Stamina += Math.Min(2, Game1.player.MaxStamina - Game1.player.Stamina);
                    }

                    virtualHoe.UpgradeLevel = level;

                    return;

                default:

                    if (virtualCan == null)
                    {

                        virtualCan = new WateringCan();
                        virtualCan.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
                        Game1.player.Stamina += Math.Min(2, Game1.player.MaxStamina - Game1.player.Stamina);
                    }

                    virtualCan.UpgradeLevel = level;

                    return;

            }

        }

        public Dictionary<string, int> ToggleList()
        {

            return new Dictionary<string, int>(staticData.toggleList);

        }

        public void ToggleEffect(string effect)
        {

            if (staticData.toggleList.ContainsKey(effect))
            {
                
                staticData.toggleList.Remove(effect);

            }
            else
            {

                staticData.toggleList[effect] = 1;

            }

        }

        /*public NPC RetrieveVoice(GameLocation location, Vector2 position)
        {

            if (characters.ContainsKey("Disembodo"))
            {

                GameLocation previous = characters["Disembodo"].currentLocation;

                if (previous != null)
                {

                    if(previous != location)
                    {

                        previous.characters.Remove(characters["Disembodo"]);

                        location.characters.Add(characters["Disembodo"]);

                        characters["Disembodo"].update(Game1.currentGameTime, location);

                    }

                }
                else
                {
                    location.characters.Add(characters["Disembodo"]);

                    characters["Disembodo"].update(Game1.currentGameTime, location);

                }

            }
            else
            {

                characters["Disembodo"] = new(position, location.Name, "Disembodo");

                characters["Disembodo"].IsInvisible = true;

                characters["Disembodo"].eventActor = true;

                characters["Disembodo"].forceUpdateTimer = 9999;

                characters["Disembodo"].collidesWithOtherCharacters.Value = true;

                characters["Disembodo"].farmerPassesThrough = true;

                location.characters.Add(characters["Disembodo"]);

                characters["Disembodo"].update(Game1.currentGameTime, location);

            }

            return characters["Disembodo"];

        }*/

        public void TrainFarmhands()
        {

            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {

                if(Helper.Multiplayer.GetConnectedPlayer(farmer.UniqueMultiplayerID) != null)
                {

                    StaticData farmhandData = staticData.DeepClone();

                    farmhandData.staticId = farmer.UniqueMultiplayerID;

                    Helper.Multiplayer.SendMessage(farmhandData, "FarmhandTrain", modIDs: new[] { this.ModManifest.UniqueID });

                }

            }

        }

        public string AttunedWeapon(int toolIndex)
        {

            if (Config.weaponAttunement.ContainsKey(toolIndex))
            {
                return "reserved";
            }

            if (weaponAttunement.ContainsKey(toolIndex))
            {
                return weaponAttunement[toolIndex];

            }

            return "none";

        }

        public bool AttuneWeapon(string blessing)
        {

            int toolIndex = AttuneableWeapon();

            if (toolIndex == -1) { return false; };

            staticData.weaponAttunement[toolIndex] = blessing;

            weaponAttunement[toolIndex] = blessing;

            activeData.castInterrupt = true;

            return true;

        }

        public bool DetuneWeapon()
        {
            
            int toolIndex = AttuneableWeapon();

            if (toolIndex == -1) { return false; };

            if (staticData.weaponAttunement.ContainsKey(toolIndex))
            {

                staticData.weaponAttunement.Remove(toolIndex);

            }

            if (weaponAttunement.ContainsKey(toolIndex))
            {

                weaponAttunement.Remove(toolIndex);

            }

            activeData.castInterrupt = true;

            return true;

        }

        public StardewValley.Monsters.Monster SpawnMonster(GameLocation location, Vector2 vector, List<int> spawnIndex, string terrain = "ground")
        {
            
            MonsterHandle monsterHandle;

            if (monsterHandles.ContainsKey(location.Name))
            {

                monsterHandle = monsterHandles[location.Name];

                monsterHandle.SpawnCheck();

            }
            else
            {

                Rite rite = NewRite();

                rite.castLocation = location;

                rite.castVector = vector;

                monsterHandle = new(vector, rite);

                monsterHandles[location.Name] = monsterHandle;

            }

            monsterHandle.spawnIndex = spawnIndex;

            if(terrain == "ground")
            {

                return monsterHandle.SpawnGround(vector);

            } 
            else 
            {

                monsterHandle.TargetToPlayer(vector);

                Vector2 spawnVector = monsterHandle.SpawnVector();

                if(spawnVector != new Vector2(-1))
                {

                    return monsterHandle.SpawnTerrain(spawnVector, vector, (terrain == "water"));

                }

            }

            return null;

        }

        public List<StardewValley.Monsters.Monster> SpawnList(GameLocation location)
        {
            
            List<StardewValley.Monsters.Monster> spawnList = new();

            if (monsterHandles.ContainsKey(location.Name))
            {

                MonsterHandle monsterHandle = monsterHandles[location.Name];

                monsterHandle.SpawnCheck();

                spawnList = monsterHandle.monsterSpawns;

            }

            return spawnList;

        }

        public void AutoConsume()
        {

            if (Config.consumeRoughage || Config.consumeQuicksnack)
            {

                int grizzleConsume;

                int grizzlePower;

                float staminaUp;

                bool sashimiPower = false;

                List<int> grizzleList = Map.SpawnData.GrizzleList();

                List<int> sashimiList = Map.SpawnData.SashimiList();

                Dictionary<int, int> coffeeList = Map.SpawnData.CoffeeList();

                List<string> consumeList = new();

                int checkIndex;

                for (int i = 0; i < Game1.player.Items.Count; i++)
                {

                    if (Game1.player.Stamina == Game1.player.MaxStamina)
                    {

                        break;

                    }

                    Item checkItem = Game1.player.Items[i];

                    // ignore empty slots
                    if (checkItem == null)
                    {

                        continue;

                    }

                    int itemIndex = checkItem.ParentSheetIndex;

                    if (Config.consumeRoughage)
                    {
                        checkIndex = grizzleList.IndexOf(itemIndex);

                        if (checkIndex != -1)
                        {

                            grizzlePower = Math.Max(2, checkIndex);

                            grizzleConsume = Math.Min(checkItem.Stack, (int)(30 / grizzlePower));

                            staminaUp = grizzleConsume * grizzlePower;

                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + (float)staminaUp);

                            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + (int)(staminaUp * 0.35));

                            consumeList.Add(checkItem.DisplayName);

                            Game1.player.Items[i].Stack -= grizzleConsume;

                            if (Game1.player.Items[i].Stack <= 0)
                            {
                                Game1.player.Items[i] = null;

                            }

                            continue;

                        }
                    }

                    if (Config.consumeQuicksnack)
                    {
                        checkIndex = sashimiList.IndexOf(itemIndex);

                        if (checkIndex != -1 && !sashimiPower)
                        {

                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + @checkItem.staminaRecoveredOnConsumption());

                            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + @checkItem.healthRecoveredOnConsumption());

                            consumeList.Add(checkItem.DisplayName);

                            Game1.player.Items[i].Stack -= 1;

                            if (Game1.player.Items[i].Stack <= 0)
                            {
                                Game1.player.Items[i] = null;

                            }

                            sashimiPower = true;

                        }

                    }

                    if (Config.consumeCaffeine)
                    {

                        if (coffeeList.ContainsKey(itemIndex))
                        {

                            if (Game1.buffsDisplay.drink == null)
                            {

                                int coffeeConsume = 1;

                                int getSpeed = coffeeList[itemIndex];

                                if (getSpeed < 90000)
                                {

                                    coffeeConsume = Math.Min(5, Game1.player.Items[i].Stack);

                                    getSpeed *= coffeeConsume;

                                }

                                Buff speedBuff = new("Druidic Roastmaster", getSpeed, checkItem.DisplayName, 9);

                                speedBuff.buffAttributes[9] = 1;

                                speedBuff.total = 1;

                                speedBuff.which = 184653;

                                Game1.buffsDisplay.tryToAddDrinkBuff(speedBuff);

                                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + 25);

                                consumeList.Add(checkItem.DisplayName);

                                Game1.player.Items[i].Stack -= coffeeConsume;

                                if (Game1.player.Items[i].Stack <= 0)
                                {
                                    Game1.player.Items[i] = null;

                                }

                            }

                        }

                    }

                }

                if (consumeList.Count > 0)
                {

                    string consumeString = String.Join(", ", consumeList.ToArray());

                    Game1.addHUDMessage(new HUDMessage(consumeString, 4));

                }

            }


        }

        public bool PartyHats()
        {

            return Config.partyHats;

        }

        public bool CastAnywhere()
        {

            return Config.castAnywhere;
        }

        public bool WitnessedRite(string type, NPC witness)
        {

            if (!riteWitnesses.ContainsKey(type))
            {

                riteWitnesses[type] = new()
                {
                    witness.id,

                };

                return false;

            }

            if (!riteWitnesses[type].Contains(witness.id))
            {

                riteWitnesses[type].Add(witness.id);

                return false;

            }

            return true;

        }

        public void CharacterRegister(string character, string map)
        {

            staticData.characterList[character] = map;

        }

        public string CharacterMap(string character)
        {

            if (staticData.characterList.ContainsKey(character))
            { 
            
                return staticData.characterList[character];
    
            }

            return null;

        }

        public Vector2 CharacterPosition(string character)
        {

            if (staticData.characterList.ContainsKey(character))
            {

                return CharacterData.CharacterPosition(staticData.characterList[character]);

            }

            return Vector2.Zero;

        }



    }

}
