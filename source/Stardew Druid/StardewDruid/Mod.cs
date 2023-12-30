/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewDruid.Event.World;
using StardewDruid.Journal;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid
{

    public class Mod : StardewModdingAPI.Mod
    {

        public ModData Config;

        public ActiveData activeData;

        private StaticData staticData;

        public Dictionary<int, string> weaponAttunement;

        private MultiplayerData multiplayerData;

        public Dictionary<string, Event.EventHandle> eventRegister;

        public List<string> eventSync;

        public Dictionary<string, TriggerHandle> markerRegister;

        public Dictionary<string, Character.TrackHandle> trackRegister;

        public List<string> warpCasts;

        public List<string> fireCasts;

        public Dictionary<string, int> rockCasts;

        public Dictionary<string, Dictionary<Vector2, string>> targetCasts;

        public Dictionary<string, Dictionary<Vector2, string>> terrainCasts;

        public Dictionary<string, Dictionary<Vector2, int>> featureCasts;

        private Dictionary<string, Map.Quest> questIndex;

        public int updateRite;

        public int updateEvent;

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

        public List<string> lessons;

        public List<string> triggerList;

        public List<string> blessingList;

        public bool receivedData;

        public Dictionary<string, List<int>> riteWitnesses;

        internal static Mod instance;

        override public void Entry(IModHelper helper)
        {

            instance = this;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += SaveLoaded;

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
            else if (!receivedData)
            {

                StaticData loadData = new();

                Helper.Multiplayer.SendMessage(loadData, "FarmhandRequest", modIDs: new[] { this.ModManifest.UniqueID });

            }

            StaticChecks();

            Helper.Data.WriteJsonFile("staticData.json", staticData);

            questIndex = Map.QuestData.QuestList();

            lessons = new();

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

            if (staticData == null)
            {

                staticData = new StaticData() { staticVersion = QuestData.StaticVersion() };

            }

            if (staticData.staticVersion != QuestData.StaticVersion())
            {

                staticData = QuestData.ReconfigureData(staticData);

            }

            if (Config.newProgress != -1 && staticData.setProgress != Config.newProgress)
            {

                staticData = QuestData.ConfigureProgress(staticData, Config.newProgress);

            }

            staticData.setProgress = Config.newProgress;


        }

        private void SaveImminent(object sender, SavingEventArgs e)
        {

            foreach (string lesson in this.lessons)
            {

                switch (lesson)
                {
                    case "sync":
                    case "farmhand":
                    case "daily":
                    case "dailytwo":

                        break;

                    default:

                        staticData.activeProgress = QuestData.AchieveProgress(lesson);

                        break;


                }

            }

            if (Context.IsMainPlayer)
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

            eventSync.Clear();

            foreach (KeyValuePair<string, Event.TriggerHandle> markerEntry in markerRegister)
            {

                markerEntry.Value.EventRemove();

            }

            markerRegister.Clear();

            trackRegister.Clear();

            foreach (KeyValuePair<string, MonsterHandle> monsterEntry in monsterHandles)
            {

                monsterEntry.Value.ShutDown();

            }

            activeData.castInterrupt = true;

            dialogue.Clear();

            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {

                if (location.characters.Count > 0)
                {
                    for (int index = location.characters.Count - 1; index >= 0; --index)
                    {
                        NPC character = location.characters[index];

                        if (character is StardewDruid.Character.Character)
                        {
                            location.characters.RemoveAt(index);
                        }

                    }
                }

            }
            foreach (KeyValuePair<string, StardewDruid.Character.Character> character in this.characters)
            {
                if (character.Value.currentLocation != null)
                {
                    character.Value.currentLocation.characters.Remove(character.Value);

                }

            }

            characters.Clear();

            Game1.currentSpeaker = null;

            Game1.objectDialoguePortraitPerson = null;


            if (Game1.buffsDisplay.otherBuffs.Count > 0)
            {

                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {

                    buff.removeBuff();

                }

            }

            Game1.buffsDisplay.otherBuffs.Clear();

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
            if (e.FromModID != ModManifest.UniqueID)
            {

                return;

            }

            if (e.Type == "EventRegister")
            {
                if (e.FromPlayerID != Game1.player.UniqueMultiplayerID)
                {
                    this.eventSync.Add(e.ReadAs<StaticData>().activeBlessing);
                    Console.WriteLine(string.Format("Event activated by {0}", e.FromPlayerID));
                }
            }
            else if (e.Type == "EventRemove" && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
            {
                this.eventSync.Remove(e.ReadAs<StaticData>().activeBlessing);
                Console.WriteLine(string.Format("Event activated by {0}", e.FromPlayerID));
            }

            if (Context.IsMainPlayer)
            {

                if (e.Type == "FarmhandSave")
                {
                    StaticData farmhandData = e.ReadAs<StaticData>();

                    multiplayerData ??= Helper.Data.ReadSaveData<MultiplayerData>("multiplayerData");

                    multiplayerData ??= new MultiplayerData();

                    multiplayerData.farmhandData[e.FromPlayerID] = farmhandData;

                    Helper.Data.WriteSaveData("multiplayerData", multiplayerData);

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
                        if (lessons.Contains("farmhand")) { return; }

                        lessons.Add("farmhand");

                    }

                    staticData = farmhandData;

                    StaticChecks();

                    Console.WriteLine($"Received Stardew Druid data for Farmer ID {e.FromPlayerID}");

                    ReadyState();

                    receivedData = true;

                }

            }


        }

        public void ReadyState()
        {

            triggerList = new();

            activeData = new ActiveData() { activeBlessing = staticData.activeBlessing };

            eventRegister = new();

            eventSync = new();

            markerRegister = new();

            trackRegister = new();

            monsterHandles = new();

            rockCasts = new();

            targetCasts = new();

            featureCasts = new();

            terrainCasts = new();

            warpCasts = new();

            fireCasts = new();

            locationPoll = new();

            riteWitnesses = new();

            // ---------------------- trigger assignment

            weaponAttunement = SpawnData.WeaponAttunement();

            foreach (KeyValuePair<int, string> keyValuePair in staticData.weaponAttunement)
            {

                if (!weaponAttunement.ContainsKey(keyValuePair.Key))
                {

                    weaponAttunement[keyValuePair.Key] = keyValuePair.Value;
                }

            }

            RiteTool();

            blessingList = QuestData.RitesProgress();

            if (Config.autoProgress)
            {
                QuestData.NextProgress();

            }

            SynchroniseQuest();

            return;

        }

        public void CreateCharacters()
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            CharacterData.CharacterCheck(staticData.activeProgress);

            foreach (KeyValuePair<string, string> characterInfo in staticData.characterList)
            {

                CharacterData.CharacterLoad(characterInfo.Key, characterInfo.Value);

            }

        }

        public void SynchroniseQuest()
        {

            if (staticData.activeProgress == 0)
            {
                QuestData.NextProgress();

            }

            if (!lessons.Contains("sync"))
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

                lessons.Add("sync");

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

        public bool CasterBusy()
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

            /*if(triggerList.Count > 0)
            {

                Map.Quest questData = questIndex[triggerList.First()];

                QuestData.MarkerInstance(Game1.player.currentLocation, questData);

            }*/

            //List<string> locationsDone = new();

            foreach (string castString in triggerList)
            {

                //if (locationsDone.Contains(Game1.player.currentLocation.Name))
                //{

                 //   continue;

                //}

                Map.Quest questData = questIndex[castString];

                QuestData.MarkerInstance(Game1.player.currentLocation, questData);

                if(markerRegister.Count > 0)
                {

                    break;

                }

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

            if (eventRegister.Count == 0 && markerRegister.Count == 0)
            {
                return;
            }

            bool exitAll = false;

            bool extendAll = false;

            List<string> removeList = new();

            if (Game1.eventUp || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
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

            foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
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
                if (Game1.IsMultiplayer)
                {

                    foreach (KeyValuePair<string, EventHandle> keyValuePair in eventRegister)
                    {

                        Helper.Multiplayer.SendMessage<StaticData>(new StaticData() { activeBlessing = keyValuePair.Key }, "EventRemove", new string[1] { ModManifest.UniqueID }, null);

                    }

                }

                eventRegister.Clear();

                return;
            }

            foreach (string removeChallenge in removeList)
            {

                if (Game1.IsMultiplayer)
                {

                    Helper.Multiplayer.SendMessage<StaticData>(new StaticData() { activeBlessing = removeChallenge }, "EventRemove", new string[1] { ModManifest.UniqueID }, null);

                }

                eventRegister.Remove(removeChallenge);

            }

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // Game is not ready
            if (!Context.IsWorldReady)
            {

                return;

            }

            bool casterBusy = CasterBusy();

            // action press
            if (eventRegister.Count > 0 && !casterBusy)
            {

                if (Game1.didPlayerJustLeftClick() || Game1.didPlayerJustRightClick() || Config.actionButtons.GetState() == SButtonState.Pressed)
                {

                    List<Type> typeList = new List<Type>();

                    foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
                    {

                        if (!typeList.Contains(eventEntry.GetType()) && eventEntry.Value.EventPerformAction(e.Button))
                        {

                            typeList.Add(eventEntry.GetType());

                        }

                    }

                }

            }

            bool ritePressed = RiteButtonPressed();

            bool journalPressed = JournalButtonPressed();

            if (casterBusy)
            {
                
                activeData.castInterrupt = true;

                if (Game1.activeClickableMenu != null)
                {

                    if (Game1.activeClickableMenu is QuestLog && ritePressed)
                    {

                        Game1.activeClickableMenu = new Druid();

                        return;

                    }

                }

            }

            if (journalPressed)
            {

                activeData.castInterrupt = true;

                Game1.activeClickableMenu = new Druid();

                return;

            }

            if (ritePressed)
            {

                ResetCast();

            }

        }

        public bool RiteButtonPressed()
        {

            return Config.riteButtons.GetState() == SButtonState.Pressed;

        }

        public bool JournalButtonPressed()
        {

            return Config.journalButtons.GetState() == SButtonState.Pressed;

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
            updateRite = Math.Max(0, updateRite - 1);

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

                if (staticData.activeProgress <= 1)
                {

                    CastMessage(this.Config.journalButtons.ToString() + " to open Druid Journal and get started");
                }
                else
                {

                    CastMessage("Nothing happened... ");
                }

                activeData.castInterrupt = true;

                return;

            }


            // unable to cast if game location has no spawn profile
            if (activeData.spawnIndex.Count == 0 && !eventRegister.ContainsKey("active") && !eventSync.Contains("active"))
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

            if (eventRegister.ContainsKey("active") || eventSync.Contains("active"))
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

            foreach (KeyValuePair<string, TriggerHandle> marker in markerRegister)
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

            if (staticData.activeProgress <= 1)
            {

                activeBlessing = "none";

            }
            else if (Config.slotAttune)
            {

                // check if valid tool and valid location are still selected
                activeBlessing = AttuneableSlot();

                if (activeBlessing == "none")
                {

                    CastMessage("No rite attuned to slot " + (Game1.player.CurrentToolIndex + 1));

                    activeData.castInterrupt = true;

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
                    if (!blessingList.Contains(activeBlessing))
                    {

                        CastMessage("I'm not attuned to this artifact... perhaps the Effigy can help");

                        activeData.castInterrupt = true;

                        return false;

                    }

                }

            }

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

            string slotBlessing = Rite.GetSlotBlessing();

            return blessingList.Contains(slotBlessing) ? slotBlessing : "none";

        }

        public int AttuneableWeapon()
        {

            if (Game1.player.CurrentTool is null)
            {

                if (Game1.player.CurrentToolIndex == 999 && eventRegister.ContainsKey("transform"))
                {

                    return (eventRegister["transform"] as Transform).attuneableIndex;

                }

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

                //if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }

            }
            else if (Game1.player.CurrentTool is Axe)
            {
                toolIndex = 992;

                //if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
            }
            else if (Game1.player.CurrentTool is Hoe)
            {
                toolIndex = 993;

                //if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
            }
            else if (Game1.player.CurrentTool is WateringCan)
            {
                toolIndex = 994;

                //if (currentTool != toolIndex) { RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel); }
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

                castTask = new Dictionary<string, int>(staticData.taskList),

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

                damageLevel += 2 * Game1.player.MiningLevel;

                damageLevel += 2 * Game1.player.ForagingLevel;

                damageLevel += 1 * Game1.player.FarmingLevel;

                damageLevel += 1 * Game1.player.FishingLevel;

                damageLevel += staticData.activeProgress * 2;

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

        public bool EffectDisabled(string effect)
        {

            if (Config.disableSeeds && effect == "Seeds") { return true; }


            if (Config.disableTrees && effect == "Trees") { return true; }


            if (Config.disableWildspawn && effect == "Wildspawn") { return true; }


            if (Config.disableFish && effect == "Fish") { return true; }

            return false;
        }

        public int CurrentProgress()
        {

            return staticData.activeProgress;

        }

        public string CurrentBlessing()
        {

            return staticData.activeBlessing;

        }

        public void ChangeBlessing(string blessing)
        {

            staticData.activeBlessing = blessing;

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

            Helper.Multiplayer.SendMessage<StaticData>(new StaticData() { activeBlessing = placeHolder }, "EventRegister", new string[1] { ModManifest.UniqueID }, null);

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

        public string QuestDiscuss(string quest)
        {

            return questIndex[quest].questDiscuss;

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

            if (questData.questProgress == 2)
            {

                lessons.Add(quest);

            }

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

                if (!triggerList.Contains(quest))
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

            if (questData.taskFinish != null)
            {
                
                staticData.taskList[questData.taskFinish] = 1;

            }

            if (questData.questProgress == 1)
            {

                staticData.activeProgress = QuestData.AchieveProgress(quest);

            }

            blessingList = QuestData.RitesProgress();

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

        public Dictionary<string, int> TaskList()
        {
            return staticData.taskList;
        }

        public int UpdateTask(string quest, int update)
        {
            if (!questIndex.ContainsKey(quest))
            {
                return -1;
            }

            Map.Quest questData = questIndex[quest];

            if (questData.taskCounter == 0)
            {
                return -1;
            }

            if (!staticData.questList.ContainsKey(quest))
            {
                NewQuest(quest);
            }

            if (!staticData.taskList.ContainsKey(quest))
            {
                ReassignQuest(quest);
            }

            if (staticData.questList[quest])
            {
                return -1;
            }

            staticData.taskList[quest] += update;

            if (staticData.taskList[quest] >= questData.taskCounter)
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

        public bool LocationPoll(string entryKey)
        {

            string location = Game1.player.currentLocation.Name;

            if (locationPoll.ContainsKey(entryKey))
            {

                if (locationPoll[entryKey] == location)
                {

                    return true;

                }

            }

            locationPoll[entryKey] = location;

            return false;

        }

        public void RiteTool()
        {

            int level = Math.Min(5, Math.Max(1,staticData.activeProgress / 5));

            if (Config.maxDamage)
            {

                level = 5;

            }

            virtualPick = new Pickaxe();
            virtualPick.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualPick.UpgradeLevel = level;

            virtualAxe = new Axe();
            virtualAxe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualAxe.UpgradeLevel = level;

            virtualHoe = new Hoe();
            virtualHoe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualHoe.UpgradeLevel = level;

            virtualCan = new WateringCan();
            virtualCan.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualCan.UpgradeLevel = level;

            Game1.player.Stamina += Math.Min(8, Game1.player.MaxStamina - Game1.player.Stamina);

        }

        public void TrainFarmhands()
        {

            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {

                if (Helper.Multiplayer.GetConnectedPlayer(farmer.UniqueMultiplayerID) != null)
                {

                    StaticData farmhandData = staticData.DeepClone();

                    farmhandData.staticId = farmer.UniqueMultiplayerID;

                    Helper.Multiplayer.SendMessage(farmhandData, "FarmhandTrain", modIDs: new[] { this.ModManifest.UniqueID });

                }

            }

        }

        public string AttunedWeapon(int toolIndex)
        {

            if (SpawnData.WeaponAttunement().ContainsKey(toolIndex))
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

            if (terrain == "ground")
            {

                return monsterHandle.SpawnGround(vector);

            }
            else
            {

                monsterHandle.TargetToPlayer(vector);

                Vector2 spawnVector = monsterHandle.SpawnVector();

                if (spawnVector != new Vector2(-1))
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

                            grizzleConsume = Math.Min(checkItem.Stack, 30 / grizzlePower);

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

            if (characters.ContainsKey(character))
            {

                characters[character].DefaultMap = map;

                characters[character].DefaultPosition = CharacterData.CharacterPosition(map);

            }

        }

        public string CharacterMap(string character)
        {

            if (staticData.characterList.ContainsKey(character))
            {

                return staticData.characterList[character];

            }

            return null;

        }

        public string ColourPreference()
        {

            return Config.colourPreference;

        }


    }

}
