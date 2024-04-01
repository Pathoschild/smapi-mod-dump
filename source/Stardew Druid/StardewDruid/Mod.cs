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
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Cast.Ether;
using StardewDruid.Character;
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewDruid.Event.Challenge;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Events;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewDruid.Event.SpellHandle;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid
{

    public class Mod : StardewModdingAPI.Mod
    {

        public ModData Config;

        public CustomData Customisation;

        public bool modReady;

        //public ActiveData activeData;

        public Rite rite;

        private StaticData staticData;

        public Dictionary<int, string> weaponAttunement;

        private MultiplayerData multiplayerData;

        public Dictionary<string, Event.EventHandle> eventRegister;

        public Dictionary<int, string> clickRegister;

        public Dictionary<string, List<TriggerHandle>> markerRegister;

        public Dictionary<string, Character.TrackHandle> trackRegister;

        public Dictionary<string, int> rockCasts;

        public Dictionary<string, List<string>> specialCasts;

        public Dictionary<string, Dictionary<Vector2, string>> targetCasts;

        public Dictionary<string, Dictionary<Vector2, string>> terrainCasts;

        public Dictionary<string, Dictionary<Vector2, int>> featureCasts;

        public List<SpellHandle> spellRegister;

        public List<string> triggerCasts;

        public int updateRite;

        public int updateEvent;

        public double messageBuffer;

        public StardewValley.Tools.Pickaxe virtualPick;

        public StardewValley.Tools.Axe virtualAxe;

        public StardewValley.Tools.Hoe virtualHoe;

        public StardewValley.Tools.WateringCan virtualCan;

        public int currentTool;

        public Dictionary<string, StardewDruid.Character.Character> characters;

        public Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue;

        public List<string> limits;

        public List<string> lessons;

        public List<string> blessingList;

        public List<string> locationList;

        public bool receivedData;

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
            Customisation = Helper.Data.ReadJsonFile<CustomData>("customData.json");
            if(Customisation == null) { Customisation = new(); }
            //Helper.Data.WriteJsonFile("customData.json", Customisation);

            ConfigMenu.MenuConfig(this);

            /*DragonRoar: Dinosaur Roar by Mike Koenig Attribute 3.0 SoundBank.com */
            CueDefinition myCueDefinition = new CueDefinition();
            myCueDefinition.name = "DragonRoar";
            myCueDefinition.instanceLimit = 1;
            myCueDefinition.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
            FileStream soundstream = new(Path.Combine(Mod.instance.Helper.DirectoryPath, "Sounds", "Roar.wav"), FileMode.Open);
            SoundEffect roarSound = SoundEffect.FromStream(soundstream);
            myCueDefinition.SetSound(roarSound, Game1.audioEngine.GetCategoryIndex("Sound"), false);
            Game1.soundBank.AddCue(myCueDefinition);

        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            if (Context.IsMainPlayer)
            {

                staticData = Helper.Data.ReadSaveData<StaticData>("staticData");

                //Game1.MasterPlayer.mailReceived.Add("ccCraftsRoom");

                //(Game1.getLocationFromName("Mountain") as Mountain).MakeMapModifications(true);

            }
            else if (!receivedData)
            {

                StaticData loadData = new();

                Helper.Multiplayer.SendMessage(loadData, "FarmhandRequest", modIDs: new[] { this.ModManifest.UniqueID });

                return;

            }

            StaticChecks();

            ReadyState();

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

            Helper.Data.WriteJsonFile("staticData.json", staticData);

        }

        private void SaveImminent(object sender, SavingEventArgs e)
        {

            foreach (string lesson in lessons)
            {

                staticData.activeProgress = QuestData.AchieveProgress(lesson);

            }

            lessons.Clear();

            limits.Clear();

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

            ClearTriggers();

            trackRegister.Clear();

            //activeData.castInterrupt = true;

            rite.shutdown();

            dialogue.Clear();

            if (Context.IsMainPlayer)
            {

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

                            if (character is StardewDruid.Character.Dragon)
                            {
                                location.characters.RemoveAt(index);
                            }

                        }
                    }

                }
                foreach (KeyValuePair<string, StardewDruid.Character.Character> character in characters)
                {
                    if (character.Value.currentLocation != null)
                    {
                        character.Value.currentLocation.characters.Remove(character.Value);

                    }

                }

            }

            characters.Clear();

            Game1.currentSpeaker = null;

            Game1.objectDialoguePortraitPerson = null;

            foreach(string locationName in locationList)
            {

                GameLocation customLocation = Game1.getLocationFromName(locationName);

                if (customLocation != null)
                {

                    Game1.locations.Remove(customLocation);

                    Game1.removeLocationFromLocationLookup(customLocation);

                }

            }

            locationList.Clear();

            /*if (Game1.buffsDisplay.otherBuffs.Count > 0)
            {

                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {

                    buff.removeBuff();

                }

            }

            Game1.buffsDisplay.otherBuffs.Clear();*/

        }

        private void SaveUpdated(object sender, SavedEventArgs e)
        {

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

            StaticData farmhandData;

            QueryData queryData;

            switch (e.Type)
            {

                case "FarmhandSave":

                    if (!Context.IsMainPlayer) { return; }

                    farmhandData = e.ReadAs<StaticData>();

                    multiplayerData ??= Helper.Data.ReadSaveData<MultiplayerData>("multiplayerData");

                    multiplayerData ??= new MultiplayerData();

                    multiplayerData.farmhandData[e.FromPlayerID] = farmhandData;

                    Helper.Data.WriteSaveData("multiplayerData", multiplayerData);

                    Console.WriteLine($"Saved Stardew Druid data for Farmer ID {e.FromPlayerID}");

                    break;

                case "FarmhandRequest":

                    if (!Context.IsMainPlayer) { return; }

                    multiplayerData ??= Helper.Data.ReadSaveData<MultiplayerData>("multiplayerData");

                    multiplayerData ??= new MultiplayerData();

                    if (multiplayerData.farmhandData.ContainsKey(e.FromPlayerID))
                    {

                        farmhandData = multiplayerData.farmhandData[e.FromPlayerID];

                    }
                    else
                    {

                        farmhandData = new StaticData();

                    }

                    farmhandData.staticId = e.FromPlayerID;

                    Helper.Multiplayer.SendMessage(farmhandData, "FarmhandLoad", modIDs: new[] { this.ModManifest.UniqueID });

                    Console.WriteLine($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}");

                    break;

                case "FarmhandLoad":
                case "FarmhandTrain":

                    farmhandData = e.ReadAs<StaticData>();

                    if (farmhandData.staticId != Game1.player.UniqueMultiplayerID)
                    {
                    
                        return;
                    
                    }

                    staticData = farmhandData;

                    StaticChecks();

                    ReadyState();

                    receivedData = true;

                    Console.WriteLine($"Received Stardew Druid data for Farmer ID {e.FromPlayerID}");

                    break;

                case "LocationEdit":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    Location.LocationData.LocationEdit(queryData);

                    break;


                case "LocationReset":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    Location.LocationData.LocationReset(queryData);

                    break;


                case "LocationPortal":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    Location.LocationData.LocationPortal(queryData);

                    break;


                case "LocationReturn":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    Location.LocationData.LocationReturn(queryData);

                    break;

                case "EventComplete":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    Location.LocationData.QuestComplete(queryData);

                    if (!QuestGiven(queryData.value))
                    {

                        RegisterQuest(queryData.value);

                    }

                    if (!QuestComplete(queryData.value))
                    {

                        CompleteQuest(queryData.value);

                    }

                    Console.WriteLine(string.Format("Event completion triggered by {0}", e.FromPlayerID));

                    SetTriggers();

                    break;

                case "EventAbort":

                    if (!Context.IsMainPlayer) { return; }

                    if (!eventRegister.ContainsKey("active")) { return; }

                    queryData = e.ReadAs<QueryData>();

                    if(queryData.location == Game1.player.currentLocation.Name)
                    {

                        AbortAllEvents();

                        if(queryData.value == "damage")
                        {
                            
                            CastMessage(queryData.name + " is in critical condition", 3, true);

                        }

                    }

                    break;

                case "CharacterCommand":

                    if (!Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    CharacterData.QueryCommand(queryData);

                    break;

                case "DialogueDisplay":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    DialogueData.QueryDisplay(queryData);

                    break;

                case "DialogueSpecial":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    dialogue[queryData.name].AddSpecial(queryData.name,queryData.value);

                    break;

                case "SpellHandle":

                    queryData = e.ReadAs<QueryData>();

                    if (queryData.longId == Game1.player.UniqueMultiplayerID)
                    {

                        break;

                    }

                    if(Game1.player.currentLocation.Name != queryData.location)
                    {

                        break;

                    }

                    List<int> spellData = System.Text.Json.JsonSerializer.Deserialize<List<int>>(queryData.value);

                    SpellHandle spellEffect = new(
                        Game1.player.currentLocation,
                        new Vector2(spellData[0], spellData[1]),
                        new Vector2(spellData[2], spellData[3]),
                        spellData[4]
                    );

                    spellEffect.external = true;
                    spellEffect.type = (barrages)Enum.Parse(typeof(barrages),spellData[5].ToString());
                    spellEffect.scheme = (schemes)Enum.Parse(typeof(schemes), spellData[6].ToString());
                    spellEffect.indicator = (indicators)Enum.Parse(typeof(indicators), spellData[7].ToString());

                    spellRegister.Add(spellEffect);

                    break;

            }

        }

        public void ReadyState()
        {

            modReady = true;

            limits = new();

            lessons = new();

            characters = new();

            dialogue = new();

            modReady = true;

            eventRegister = new();

            clickRegister = new();

            markerRegister = new();

            trackRegister = new();

            spellRegister = new();

            rockCasts = new();

            targetCasts = new();

            featureCasts = new();

            terrainCasts = new();

            specialCasts = new();

            locationList = new();

            triggerCasts = new();

            rite = new();

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

            CreateCharacters();

            return;

        }

        public void CreateCharacters()
        {

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

            Dictionary<string, string> questIds = new();

            foreach (KeyValuePair<string, StardewDruid.Map.Quest> questData in Map.QuestData.QuestList())
            {

                if (questData.Value.questId != 0)
                {

                    questIds.Add(questData.Value.questId.ToString(), questData.Key);

                }

            }

            List<string> logged = new();

            for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
            {

                string gameId = Game1.player.questLog[num].id.Value;

                if(gameId == null)
                {

                    if (Game1.player.questLog[num].questTitle == null)
                    {

                        Game1.player.questLog.RemoveAt(num);

                    }

                    continue;

                }

                if (questIds.ContainsKey(gameId)) // valid
                {

                    string questName = questIds[gameId];

                    if (staticData.questList.ContainsKey(questName))
                    {

                        // player has a duplicate quest
                        if (logged.Contains(questName))
                        {

                            Game1.player.questLog.RemoveAt(num);

                            continue;

                        }

                        logged.Add(questName);

                        // player can see unfinished quest but mod has already turned over
                        if (staticData.questList[questName] && !Game1.player.questLog[num].completed.Value)
                        {

                            staticData.questList[questName] = false;

                            continue;

                        }

                        // player has finished quest according to game but mod has not turned over
                        if (!staticData.questList[questName] && Game1.player.questLog[num].completed.Value)
                        {

                            staticData.questList[questName] = true;

                            continue;

                        }

                    }
                    else
                    {

                        Game1.player.questLog.RemoveAt(num);

                    }

                }

            }

            Dictionary<string, Quest> questDatas = Map.QuestData.QuestList();

            foreach (KeyValuePair<string, bool> questPair in staticData.questList)
            {

                if (questPair.Value)
                {
                    continue;
                }

                if(!logged.Contains(questPair.Key))
                {

                    RegisterQuest(questPair.Key, questDatas[questPair.Key]);

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

        private void EverySecond(object sender, OneSecondUpdateTickedEventArgs e)
        {

            if (!Context.IsWorldReady || !modReady)
            {

                return;

            }

            rite.RiteBuff();

            if (!markerRegister.ContainsKey(Game1.player.currentLocation.Name))
            {

                SetTriggers();

            }

            if (eventRegister.Count == 0 && markerRegister[Game1.player.currentLocation.Name].Count == 0)
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

            if (Game1.paused || Game1.freezeControls || Game1.overlayMenu != null || Game1.isTimePaused || Game1.activeClickableMenu != null || !Game1.game1.IsActive)
            {

                extendAll = true;

                if (Context.IsMultiplayer)
                {

                    if (!Game1.netWorldState.Value.IsTimePaused)
                    {

                        extendAll = false;

                    }

                }

            }

            if (markerRegister[Game1.player.currentLocation.Name].Count > 0 && !extendAll)
            {

                foreach (TriggerHandle markerHandle in markerRegister[Game1.player.currentLocation.Name])
                {

                    markerHandle.EventInterval();

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

                    if (eventEntry.AttemptAbort())
                    {
                        
                        eventEntry.EventAbort();

                        eventEntry.EventRemove();

                        removeList.Add(eventKey);
                        
                        continue;
                    
                    }

                }

                if (extendAll)
                {

                    if (eventEntry.EventExtend())
                    {

                        continue;

                    }

                }

                if (!eventEntry.EventActive())
                {

                    removeList.Add(eventKey);

                    eventEntry.EventRemove();

                    continue;

                }

                eventEntry.EventInterval();

            }

            foreach (string removeChallenge in removeList)
            {

                eventRegister.Remove(removeChallenge);

            }

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // Game is not ready
            if (!Context.IsWorldReady || !modReady)
            {

                return;

            }

            bool casterBusy = CasterBusy();

            string actionPressed = ActionButtonPressed();

            // action press
            if (clickRegister.Count > 0 && !casterBusy && actionPressed != "none")
            {
                int cursor = 0;

                for (int i = 0; i < 20; i++)
                {

                    if (clickRegister.ContainsKey(i))
                    {
                            
                        string click = clickRegister[i];

                        if (eventRegister.ContainsKey(click))
                        {

                            if (eventRegister[click].EventPerformAction(e.Button, actionPressed))
                            {

                                break;

                            }

                        }
                        else
                        {

                            clickRegister.Remove(i);

                        }

                        cursor++;

                    }

                    if(cursor == clickRegister.Count)
                    {
                            
                        break;
                        
                    }

                }

            }

            rite.charge(actionPressed);

            bool ritePressed = RiteButtonPressed();

            bool journalPressed = JournalButtonPressed();

            if (casterBusy)
            {

                //activeData.castInterrupt = true;

                rite.shutdown();

                if (Game1.activeClickableMenu != null)
                {

                    if (Game1.activeClickableMenu is QuestLog && ritePressed)
                    {

                        Game1.activeClickableMenu = new Druid();

                    }

                }

                return;

            }

            if (journalPressed)
            {

                //activeData.castInterrupt = true;
                rite.shutdown();

                if(Game1.currentLocation is Farm)
                {

                    foreach(KeyValuePair<string,StardewDruid.Character.Character> farmHelp in characters)
                    {

                        farmHelp.Value.SummonToPlayer(Game1.player.Position);

                    }

                }

                Game1.activeClickableMenu = new Druid();

                return;

            }

            if (ritePressed)
            {

                /*for(int i = 0; i < 3; i++)
                {

                    List<Vector2> tilevectors = ModUtility.GetTilesWithinRadius(Game1.player.currentLocation, Game1.player.Tile, i);

                    foreach(Vector2 tilevector in tilevectors)
                    {
                        
                        Monitor.Log("tile |" + tilevector.ToString(), LogLevel.Debug);

                        Layer backLayer = Game1.player.currentLocation.Map.GetLayer("Back");
                        Tile backTile = backLayer.Tiles[(int)tilevector.X, (int)tilevector.Y];

                        if (backTile != null)
                        {

                            foreach (KeyValuePair<string, PropertyValue> property in backTile.TileIndexProperties)
                            {
                                Monitor.Log(property.Key + "|" + property.Value.Type.ToString() + "|" + property.Value.ToString(), LogLevel.Debug);
                            }

                            Monitor.Log(backTile.TileIndex.ToString() + "|" + backTile.TileSheet.ToString(), LogLevel.Debug);

                        }

                        Layer frontLayer = Game1.player.currentLocation.Map.GetLayer("Front");
                        Tile frontTile = frontLayer.Tiles[(int)tilevector.X, (int)tilevector.Y];

                        if (frontTile != null)
                        {

                            foreach (KeyValuePair<string, PropertyValue> property in frontTile.TileIndexProperties)
                            {
                                Monitor.Log(property.Key + "|" + property.Value.Type.ToString() + "|" + property.Value.ToString(), LogLevel.Debug);
                            }

                            Monitor.Log(frontTile.TileIndex.ToString() + "|" + frontTile.TileSheet.ToString(), LogLevel.Debug);

                        }

                        Layer buildingLayer = Game1.player.currentLocation.Map.GetLayer("Buildings");
                        Tile buildingTile = buildingLayer.Tiles[(int)tilevector.X, (int)tilevector.Y];

                        if (buildingTile != null)
                        {
                    
                            foreach (KeyValuePair<string, PropertyValue> property in buildingTile.TileIndexProperties)
                            {
                                Monitor.Log(property.Key + "|"+ property.Value.Type.ToString()+ "|" + property.Value.ToString(), LogLevel.Debug);
                            }

                            Monitor.Log(buildingTile.TileIndex.ToString(), LogLevel.Debug);


                        }

                    }


                }*/

                rite.start();

            }

        }

        public string ActionButtonPressed()
        {

            if (Config.actionButtons.GetState() == SButtonState.Pressed)
            {

                return "Action";

            }

            if(Config.specialButtons.GetState() == SButtonState.Pressed)
            {

                return "Special";

            }

            return "none";

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
            if (!Context.IsWorldReady || !modReady)
            {

                return;

            }

            for (int j = spellRegister.Count - 1; j >= 0; j--)
            {

                SpellHandle barrage = spellRegister[j];

                if (!barrage.Update())
                {

                    spellRegister.RemoveAt(j);

                }

            }

            // caster is busy
            if (CasterBusy())
            {

                return;

            }

            rite.update();

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
                    
                    for(int t = trackRegister.Count-1; t >= 0; t--)
                    {
                        KeyValuePair<string, Character.TrackHandle> trackEntry = trackRegister.ElementAt(t);

                        if (trackEntry.Value.followPlayer == null)
                        {

                            trackRegister.Remove(trackEntry.Key);

                            continue;

                        }

                        trackEntry.Value.TrackPlayer();

                    }

                }

                updateEvent = 6;

            }

        }

        public void ClearTriggers()
        {

            foreach (KeyValuePair<string, List<Event.TriggerHandle>> markers in markerRegister)
            {

                foreach (TriggerHandle marker in markers.Value)
                {

                    marker.EventRemove();

                }

            }

            markerRegister.Clear();

        }

        public void SetTriggers()
        {

            ClearTriggers();

            markerRegister.Add(Game1.player.currentLocation.Name, new());

            Dictionary<string,Quest> questIndex = Map.QuestData.QuestList();

            foreach (KeyValuePair<string, bool> questEntry in staticData.questList)
            {

                if (questEntry.Value) { continue; }

                if (triggerCasts.Contains(questEntry.Key)) { continue; }

                Map.Quest questData = questIndex[questEntry.Key];

                if (questData.type == null) { continue; }

                QuestData.MarkerInstance(Game1.player.currentLocation, questData);

                if (markerRegister[Game1.player.currentLocation.Name].Count > 0)
                {

                    break;

                }

            }

        }

        public bool CheckTrigger()
        {
            
            if (Game1.eventUp || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {

                return false;

            }

            if (eventRegister.ContainsKey("active"))
            {

                return false;
            
            }

            if (!markerRegister.ContainsKey(Game1.player.currentLocation.Name))
            {

                SetTriggers();

            }

            if (markerRegister.Count == 0)
            {
                
                return false;
            
            }

            List<TriggerHandle> triggered = new();

            foreach (TriggerHandle marker in markerRegister[Game1.player.currentLocation.Name])
            {
                
                if (marker.CheckMarker())
                {

                    triggered.Add(marker);

                    break;

                }

            }

            if(triggered.Count > 0)
            {

                ClearTriggers();

                TriggerHandle trigger = triggered.First();

                triggerCasts.Add(trigger.questData.name);

                trigger.TriggerQuest();

                return true;

            }

            return false;

        }

        public void CastMessage(string message, int type = 0, bool ignore = false)
        {

            if (messageBuffer < Game1.currentGameTime.TotalGameTime.TotalSeconds || ignore)
            {

                HUDMessage hudmessage = new(message, type);

                if(type == 0)
                {
                    hudmessage.noIcon = true;
                }

                Game1.addHUDMessage(hudmessage);

                messageBuffer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 6;

            }

        }

        public int AttuneableWeapon()
        {

            if (Game1.player.CurrentTool is null)
            {

                if (Config.slotAttune && Config.slotFreedom)
                {

                    // valid tool not required
                    return 999;

                }

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

            if (Config.disableGrass && effect == "Grass") { return true; }

            if (Config.disableFish && effect == "Fish") { return true; }

            return false;
        
        }

        public int CurrentProgress()
        {

            return staticData.activeProgress;

        }

        public int PowerLevel()
        {

            return Math.Max(1,Math.Min(5,(int)staticData.activeProgress / 5));

        }

        public int CombatModifier()
        {

            int difficulty = 1;

            switch (Config.combatDifficulty)
            {
                case "hard":

                    difficulty = 3;
                    break;

                case "medium":

                    difficulty = 2;
                    break;

            }

            if (Context.IsMultiplayer)
            {

                difficulty += 1;

            }

            int progress = Math.Max(1,(int)staticData.activeProgress / 5);

            return progress * difficulty;

        }

        public string CurrentBlessing()
        {

            return staticData.activeBlessing;

        }

        public void ChangeBlessing(string blessing)
        {

            staticData.activeBlessing = blessing;

            CastMessage("Active rite is now " + blessing + ", consult the Effigy to change");

        }

        public Dictionary<string, bool> QuestList()
        {

            return new Dictionary<string, bool>(staticData.questList);

        }

        public void RegisterEvent(Event.EventHandle eventHandle, string placeHolder)
        {

            if (eventRegister.ContainsKey(placeHolder))
            {

                eventRegister[placeHolder].EventAbort();

                eventRegister[placeHolder].EventRemove();

            }

            eventRegister[placeHolder] = eventHandle;

        }

        public void EventQuery(QueryData querydata,string query = "EventComplete")
        {

            Helper.Multiplayer.SendMessage<QueryData>(
                querydata,
                query,
                new string[1] { ModManifest.UniqueID },
                null
            );

        }

        public void CriticalCondition()
        {

            if (Context.IsMainPlayer)
            {

                if (!eventRegister.ContainsKey("active")) { return; }

                AbortAllEvents();
                    
                CastMessage("Challenge aborted due to critical condition", 3, true);

            }
            else if (Context.IsMultiplayer)
            {
                QueryData queryData = new()
                {
                    name = Game1.player.Name,
                    value = "damage",
                    time = Game1.currentGameTime.TotalGameTime.TotalMilliseconds,
                    location = Game1.player.currentLocation.Name,
                };

                EventQuery(queryData, "EventAbort");

            }

        }

        public void AbortAllEvents()
        {

            foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
            {

                eventRegister[eventEntry.Key].eventAbort = true;

            }

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

        public void NewQuest(string quest, Map.Quest questData = null)
        {

            if (!staticData.questList.ContainsKey(quest))
            {

                staticData.questList[quest] = false;

            }
            if (questData == null)
            {
                questData = Map.QuestData.QuestList()[quest];

            }

            if (questData.questId != 0)
            {

                for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
                {
                    if (Game1.player.questLog[num].id.Value == questData.questId.ToString())
                    {

                        if (Game1.player.questLog[num].completed.Value)
                        {
                            
                            Game1.player.questLog.RemoveAt(num);

                        }

                    }

                }

            }

            RegisterQuest(quest, questData);

            ReassignQuest(quest, questData);

            ClearTriggers();

            if (questData.questProgress == 2)
            {

                lessons.Add(quest);

            }

        }

        public void RegisterQuest(string quest, Map.Quest questData = null)
        {
            
            if(questData == null)
            {
                questData = Map.QuestData.QuestList()[quest];

            }

            for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
            {

                string gameId = Game1.player.questLog[num].id.Value;

                if (gameId == questData.questId.ToString()) // valid
                {

                    Game1.player.questLog.RemoveAt(num);

                }

            }

            StardewValley.Quests.Quest newQuest = new();

            newQuest.questType.Value = questData.questValue;

            newQuest.id.Value = questData.questId.ToString();

            newQuest.questTitle = questData.questTitle;

            newQuest.questDescription = questData.questDescription;

            newQuest.currentObjective = questData.questObjective;

            newQuest.showNew.Value = true;

            newQuest.moneyReward.Value = questData.questReward;

            Game1.player.questLog.Add(newQuest);

        }

        public void ReassignQuest(string quest, Map.Quest questData = null)
        {

            if (questData == null)
            {
                questData = Map.QuestData.QuestList()[quest];

            }

            if (triggerCasts.Contains(quest))
            {

                triggerCasts.Remove(quest);

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

            Map.Quest questData = Map.QuestData.QuestList()[quest];

            staticData.questList[quest] = true;

            if (questData.questId != 0)
            {

                Game1.player.completeQuest(questData.questId.ToString());

            }

            if (questData.taskFinish != null)
            {
                
                staticData.taskList[questData.taskFinish] = 1;

            }

            if (questData.questProgress <= 1)
            {

                List<string> ritesList = QuestData.RitesProgress();

                staticData.activeProgress = QuestData.AchieveProgress(quest);

                blessingList = QuestData.RitesProgress();

                if (blessingList.Count > ritesList.Count)
                {

                    ChangeBlessing(blessingList.Last());

                }

            }

            blessingList = QuestData.RitesProgress();

        }

        public void RemoveQuest(string quest)
        {

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

            Dictionary<string, Quest> questDatas = Map.QuestData.QuestList();

            if (!questDatas.ContainsKey(quest))
            {

                return -1;

            }

            Map.Quest questData = questDatas[quest];

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
                ReassignQuest(quest, questData);
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

            rite.castTask = staticData.taskList;

            return staticData.taskList[quest];

        }

        public void TaskSet(string task, int set)
        {

            staticData.taskList[task] = set;

            rite.castTask = staticData.taskList;

        }

        public string CastControl()
        {

            return Config.riteButtons.ToString();

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

            //activeData.castInterrupt = true;

            rite.shutdown();

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

            //activeData.castInterrupt = true;

            rite.shutdown();

            return true;

        }

        public void AutoConsume()
        {

            if (Config.consumeRoughage || Config.consumeQuicksnack)
            {

                int grizzleConsume;

                int grizzlePower;

                float staminaUp;

                bool snack = false;

                Dictionary<int, int> coffeeList = Map.SpawnData.CoffeeList();

                List<string> consumeList = new();

                int checkIndex;

                for (int i = 0; i < Game1.player.Items.Count; i++)
                {

                    if (Game1.player.Stamina == Game1.player.MaxStamina)
                    {

                        break;

                    }

                    Item checkSlot = Game1.player.Items[i];

                    // ignore empty slots
                    if (checkSlot == null)
                    {

                        continue;
                    }

                    Item checkItem = checkSlot.getOne();

                    if (Config.slotConsume && i < 12)
                    {

                        if (@checkItem.Category == -7 && !snack)
                        {

                            if (@checkItem.HasContextTag("ginger_item"))
                            {
                                Game1.player.buffs.Remove("25");
                            }

                            foreach (Buff foodOrDrinkBuff in @checkItem.GetFoodOrDrinkBuffs())
                            {
                                Game1.player.applyBuff(foodOrDrinkBuff);
                            }

                            if (@checkItem.QualifiedItemId == "(O)773")
                            {
                                Game1.player.health = Game1.player.maxHealth;
                            }
                            else if (@checkItem.QualifiedItemId == "(O)351")
                            {
                                Game1.player.exhausted.Value = false;
                            }

                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + (@checkItem.staminaRecoveredOnConsumption() * 2));

                            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + @checkItem.healthRecoveredOnConsumption());

                            consumeList.Add(checkItem.DisplayName);

                            Game1.player.Items[i].Stack -= 1;

                            if (Game1.player.Items[i].Stack <= 0)
                            {
                                Game1.player.Items[i] = null;

                            }

                            snack = true;

                        }

                    }

                    int itemIndex = checkItem.ParentSheetIndex;

                    if (Config.consumeRoughage)
                    {

                        checkIndex = Customisation.roughageItems.IndexOf(itemIndex);

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
                        //checkIndex = sashimiList.IndexOf(itemIndex);
                        checkIndex = Customisation.lunchItems.IndexOf(itemIndex);

                        if (checkIndex != -1 && !snack)
                        {

                            if (@checkItem.HasContextTag("ginger_item"))
                            {
                                Game1.player.buffs.Remove("25");
                            }

                            foreach (Buff foodOrDrinkBuff in @checkItem.GetFoodOrDrinkBuffs())
                            {
                                Game1.player.applyBuff(foodOrDrinkBuff);
                            }

                            if (@checkItem.QualifiedItemId == "(O)773")
                            {
                                Game1.player.health = Game1.player.maxHealth;
                            }
                            else if (@checkItem.QualifiedItemId == "(O)351")
                            {
                                Game1.player.exhausted.Value = false;
                            }

                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + @checkItem.staminaRecoveredOnConsumption());

                            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + @checkItem.healthRecoveredOnConsumption());

                            consumeList.Add(checkItem.DisplayName);

                            Game1.player.Items[i].Stack -= 1;

                            if (Game1.player.Items[i].Stack <= 0)
                            {
                                Game1.player.Items[i] = null;

                            }

                            snack = true;

                        }

                    }

                    if (Config.consumeCaffeine)
                    {

                        if (coffeeList.ContainsKey(itemIndex))
                        {

                            if (!Game1.player.buffs.IsApplied("184653"))
                            {

                                int coffeeConsume = 1;

                                int getSpeed = coffeeList[itemIndex];

                                if (getSpeed < 90000)
                                {

                                    coffeeConsume = Math.Min(5, Game1.player.Items[i].Stack);

                                    getSpeed *= coffeeConsume;

                                }

                                BuffEffects buffEffect = new();

                                buffEffect.Speed.Set(1);

                                Buff speedBuff = new("184653", source: checkItem.DisplayName, displaySource: checkItem.DisplayName, duration: getSpeed, displayName: "Druidic Roastmaster", description: "Increases Speed", effects: buffEffect);

                                Game1.player.buffs.Apply(speedBuff);

                                if (itemIndex == 349)
                                {

                                    Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + @checkItem.staminaRecoveredOnConsumption());

                                    Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + @checkItem.healthRecoveredOnConsumption());

                                }
                                else
                                {

                                    Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + 25);

                                }

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

        public bool DisableHands()
        {

            return Config.disableHands;

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

        public string ColourPreference()
        {

            string colour = Config.colourPreference;

            if(colour == "Green")
            {

                colour = "Red";

            }

            return colour;

        }

        public bool ReverseJournal()
        {

            return Config.reverseJournal;

        }

        public int DifficultyLevel()
        {
            string difficulty = Config.combatDifficulty;

            int level = 1;

            switch (difficulty)
            {

                case "medium":

                    level = 2;

                    break;

                case "hard":

                    level = 3;

                    break;

            }

            return level;

        }

    }

}
