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
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StardewDruid
{

    public class Mod : StardewModdingAPI.Mod
    {

        public ModData Config;

        public CustomData Customisation;

        public bool modReady;

        public bool receivedData;

        internal static Mod instance;

        public Rite rite;

        public IconData iconData;

        public QuestHandle questHandle;

        internal StardewDruid.Data.StaticData save;

        public Dictionary<string, Event.EventHandle> eventRegister = new();

        public Dictionary<string, string> activeEvent = new();

        public Dictionary<int, string> clickRegister = new();

        public Dictionary<string, List<string>> specialCasts = new();

        public Dictionary<string, Dictionary<Vector2, string>> targetCasts = new();

        public Dictionary<string, Dictionary<Vector2, string>> terrainCasts = new();

        public Dictionary<string, Dictionary<Vector2, int>> featureCasts = new();

        public List<SpellHandle> spellRegister = new();

        public double messageBuffer;

        public StardewValley.Tools.Pickaxe virtualPick;

        public StardewValley.Tools.Axe virtualAxe;

        public StardewValley.Tools.Hoe virtualHoe;

        public StardewValley.Tools.WateringCan virtualCan;

        public int currentTool;

        public Dictionary<CharacterData.characters, StardewDruid.Character.Character> characters = new();

        public Dictionary<CharacterData.characters, StardewDruid.Dialogue.Dialogue> dialogue = new();

        public Dictionary<CharacterData.characters, Character.TrackHandle> trackers = new();

        public List<string> locations = new();

        public Random randomIndex = new();

        public int PowerLevel
        {
            get
            {
                if(save.milestone > QuestHandle.milestones.fates_challenge)
                {
                    return 5;
                }
                if (save.milestone > QuestHandle.milestones.stars_challenge)
                {
                    return 4;
                }
                if (save.milestone > QuestHandle.milestones.mists_challenge)
                {
                    return 3;
                }
                if (save.milestone > QuestHandle.milestones.weald_challenge)
                {
                    return 2;
                }

                return 1;

            }

        }

        public int CurrentProgress
        {

            get
            {
                return save.progress.Count();
            }


        }

        public Dictionary<int,Rite.rites> Attunement
        {

            get
            {

                return SpawnData.WeaponAttunement();

            }

        }

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

            SoundData.AddSounds();

            iconData = new IconData();

        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            if (!Context.IsMainPlayer)
            {

                Helper.Multiplayer.SendMessage(new QueryData(), "SaveRequest", modIDs: new[] { this.ModManifest.UniqueID });

                return;

            }

            save = Helper.Data.ReadSaveData<StardewDruid.Data.StaticData>("saveData_" +StaticData.version);

            if (save == null)
            {

                save = VersionData.Reconfigure();

            }

            Helper.Data.WriteJsonFile("saveData.json", save);

            questHandle = new QuestHandle();

            if (save.milestone == QuestHandle.milestones.none)
            {

                questHandle.NewStart();

            }

            ReadyState();

        }

        private void SaveImminent(object sender, SavingEventArgs e)
        {

            if (Context.IsMainPlayer)
            {

                Helper.Data.WriteSaveData("saveData_" + StaticData.version, save);

            }

            foreach (KeyValuePair<string, Event.EventHandle> eventEntry in eventRegister)
            {

                eventEntry.Value.EventRemove();

            }

            eventRegister.Clear();

            trackers.Clear();

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
                            NPC npc = location.characters[index];

                            if (npc is StardewDruid.Character.Character)
                            {
                                location.characters.RemoveAt(index);
                            }

                        }

                    }

                }
                foreach (KeyValuePair<CharacterData.characters, StardewDruid.Character.Character> character in characters)
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

            foreach(string locationName in locations)
            {

                GameLocation customLocation = Game1.getLocationFromName(locationName);

                if (customLocation != null)
                {

                    Game1.locations.Remove(customLocation);

                    Game1.removeLocationFromLocationLookup(customLocation);

                }

            }

            locations.Clear();

            LocationData.WarpResets();

        }

        private void SaveUpdated(object sender, SavedEventArgs e)
        {

            ReadyState();

        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID)
            {

                return;

            }

            QueryData queryData;

            switch (e.Type)
            {

                case "SaveRequest":

                    if (!Context.IsMainPlayer) { return; }

                    Helper.Multiplayer.SendMessage(save, "SaveSynchronise", modIDs: new[] { this.ModManifest.UniqueID });

                    Console.WriteLine($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}");

                    break;

                case "SaveSynchronise":

                    if (Context.IsMainPlayer) { return; }

                    save = e.ReadAs<StaticData>();

                    ReadyState();

                    receivedData = true;

                    Console.WriteLine($"Received Stardew Druid data for Farmer ID {e.FromPlayerID}");

                    break;

                case "QuestUpdate":

                    if (!Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    questHandle.UpdateTask(queryData.name, Convert.ToInt32(queryData.value));

                    break;

                case "QuestComplete":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    questHandle.CompleteQuest(queryData.name);

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

                case "EventAbort":

                    if (!Context.IsMainPlayer) { return; }

                    if (activeEvent.Count == 0) { return; }

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

                /*case "DialogueEvent":

                    if (Context.IsMainPlayer) { return; }

                    queryData = e.ReadAs<QueryData>();

                    CharacterData.characters character = (CharacterData.characters)Enum.Parse(typeof(CharacterData.characters), queryData.location);

                    if (dialogue.ContainsKey(character))
                    {

                        dialogue[character].AddEventDialogue(queryData.name,Convert.ToInt32(queryData.value));

                    }

                    break;*/

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

                    SpellHandle spellEffect = new(Game1.player.currentLocation, spellData);
                        
                    spellRegister.Add(spellEffect);

                    break;

            }

        }

        public void ReadyState()
        {

            modReady = true;

            rite = new();

            virtualPick = new Pickaxe();
            virtualPick.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualPick.UpgradeLevel = 5;

            virtualAxe = new Axe();
            virtualAxe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualAxe.UpgradeLevel = 5;

            virtualHoe = new Hoe();
            virtualHoe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualHoe.UpgradeLevel = 5;

            virtualCan = new WateringCan();
            virtualCan.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            virtualCan.UpgradeLevel = 5;

            Game1.player.Stamina += Math.Min(8, Game1.player.MaxStamina - Game1.player.Stamina);

            questHandle.Ready();

            return;

        }

        public bool CasterBusy()
        {

            if (Game1.isWarping)
            {

                /*if (eventRegister.ContainsKey("transform"))
                {

                    (eventRegister["transform"] as Cast.Ether.Transform).EventWarp();

                }*/

                return true;

            }

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

            if (eventRegister.Count == 0)
            {

                return;
            
            }

            bool exitAll = false;

            if (Game1.eventUp || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {

                exitAll = true;

            }

            bool extendAll = false;

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

            List<string> abort = new();

            List<string> removal = new();

            for(int er = eventRegister.Count - 1; er >= 0; er--)
            {

                KeyValuePair<string, Event.EventHandle> eventHandle = eventRegister.ElementAt(er);
                
                Event.EventHandle eventEntry = eventHandle.Value;

                if (eventEntry.eventActive)
                {

                    if (exitAll)
                    {

                        if (eventEntry.AttemptAbort())
                        {

                            eventEntry.EventAbort();

                            eventEntry.EventRemove();

                            removal.Add(eventHandle.Key);

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

                        eventEntry.EventRemove();

                        removal.Add(eventHandle.Key);

                        continue;

                    }

                    eventEntry.EventInterval();

                }
                else if (eventEntry.triggerEvent)
                {

                    if (!eventEntry.TriggerActive() || exitAll)
                    {
                        
                        if (eventEntry.triggerActive)
                        {

                            eventEntry.TriggerRemove();

                        }

                    }
                    else
                    {

                        if (!eventEntry.triggerActive)
                        {
                            
                            eventEntry.TriggerMarker();

                        }

                        eventEntry.TriggerInterval();

                    }

                }

            }

            foreach (string remove in removal)
            {

                eventRegister.Remove(remove);

                if (activeEvent.ContainsKey(remove))
                {

                    activeEvent.Remove(remove);

                }

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

            int journalPressed = JournalButtonPressed();

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

            if (journalPressed != -1)
            {

                //activeData.castInterrupt = true;
                rite.shutdown();

                if(Game1.currentLocation is Farm && Context.IsMainPlayer)
                {

                    foreach(KeyValuePair<CharacterData.characters,StardewDruid.Character.Character> farmHelp in characters)
                    {

                        farmHelp.Value.SummonToPlayer(Game1.player.Position);

                    }

                }

                Druid journal = new();

                if(journalPressed == 1)
                {
                    journal.quests = false;
                    journal.effects = true;

                }

                Game1.activeClickableMenu = journal;

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

        public int JournalButtonPressed()
        {

            if(Config.journalButtons.GetState() == SButtonState.Pressed)
            {
                return 0;

            }
            else
            if (Config.effectsButtons.GetState() == SButtonState.Pressed)
            {
                return 1;

            }

            return -1;

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

            //===========================================
            // Every tenth of a second

            if (e.IsMultipleOf(6))
            {

                if (eventRegister.Count > 0)
                {
                    
                    for (int ev = eventRegister.Count - 1; ev >= 0; ev--)
                    {

                        KeyValuePair<string, Event.EventHandle> eventEntry = eventRegister.ElementAt(ev);
                        
                        if (eventEntry.Value.eventActive)
                        {
                            
                            eventEntry.Value.EventDecimal();
                        
                        }

                    }

                }

                if (trackers.Count > 0)
                {
                    
                    for(int tr = trackers.Count-1; tr >= 0; tr--)
                    {
                        KeyValuePair<CharacterData.characters, Character.TrackHandle> trackEntry = trackers.ElementAt(tr);

                        if (trackEntry.Value.followPlayer == null)
                        {

                            trackers.Remove(trackEntry.Key);

                            continue;

                        }

                        trackEntry.Value.TrackPlayer();

                    }

                }

            }

        }

        public bool CheckTrigger()
        {
            
            if (Game1.eventUp || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {

                return false;

            }

            if (activeEvent.Count > 0)
            {

                return false;
            
            }

            if (eventRegister.Count == 0)
            {
                
                return false;
            
            }

            foreach(KeyValuePair<string, EventHandle> trigger in eventRegister)
            {

                if (trigger.Value.triggerActive)
                {

                    if (trigger.Value.TriggerCheck())
                    {

                        return true;

                    }

                }

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

                /*if (Game1.player.CurrentToolIndex == 999 && eventRegister.ContainsKey("transform"))
                {

                    return (eventRegister["transform"] as Transform).attuneableIndex;

                }*/

                return -1;

            }

            if (Game1.player.CurrentTool is not Tool)
            {
                return -1;
            }

            int toolIndex = -1;

            if (Game1.player.CurrentTool is MeleeWeapon)
            {
                toolIndex = Game1.player.CurrentTool.InitialParentTileIndex;
            }
            else if (Game1.player.CurrentTool is Pickaxe)
            {
                toolIndex = 991;
            }
            else if (Game1.player.CurrentTool is Axe)
            {
                toolIndex = 992;
            }
            else if (Game1.player.CurrentTool is Hoe)
            {
                toolIndex = 993;
            }
            else if (Game1.player.CurrentTool is WateringCan)
            {
                toolIndex = 994;
            }
            else if (Game1.player.CurrentTool is FishingRod)
            {
                toolIndex = 995;
            }
            else if (Game1.player.CurrentTool is Pan)
            {
                toolIndex = 996;
            }

            currentTool = toolIndex;

            return toolIndex;

        }

        public int CombatDamage()
        {

            int damageLevel = 250;

            if (!Config.maxDamage)
            {

                damageLevel = 5 * Game1.player.CombatLevel; // 50

                damageLevel += 2 * Game1.player.MiningLevel; // 20

                damageLevel += 2 * Game1.player.ForagingLevel; // 20

                damageLevel += 1 * Game1.player.FarmingLevel; // 10

                damageLevel += 1 * Game1.player.FishingLevel; // 10

                damageLevel += save.progress.Count * 3; // 120

                if (Game1.player.CurrentTool is Tool currentTool) // 25
                {
                    if (currentTool.enchantments.Count > 0)
                    {
                        damageLevel += 25;
                    }
                }

                if (Game1.player.professions.Contains(24)) // 25
                {
                    damageLevel += 25;
                }

                if (Game1.player.professions.Contains(26)) // 25
                {
                    damageLevel += 25;
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

        public int CombatDifficulty()
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

            return PowerLevel * difficulty;

        }

        public void ChangeBlessing(Rite.rites type)
        {

            save.rite = type;

            CastMessage(rite.displayNames[type] + " is now active");

        }

        public void RegisterEvent(Event.EventHandle eventHandle, string placeHolder, bool active = false)
        {

            if (active)
            {

                activeEvent[placeHolder] = "active";

            }

            if (eventRegister.ContainsKey(placeHolder))
            {

                if (eventRegister[placeHolder] == eventHandle) 
                { 
                    
                    return; 
                
                }

                eventRegister[placeHolder].EventAbort();

                eventRegister[placeHolder].EventRemove();

            }

            eventRegister[placeHolder] = eventHandle;

            Monitor.Log(placeHolder, LogLevel.Debug);

        }

        public void RegisterClick(string eventName, int place)
        {

            while(clickRegister.ContainsKey(place))
            {

                if(clickRegister[place] == eventName)
                {

                    return;

                }

                place++;

            }

            clickRegister[place] = eventName;

        }

        public void EventQuery(QueryData querydata,string query = "QuestComplete")
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

                //if (!eventRegister.ContainsKey("active")) { return; }
                if(activeEvent.Count == 0) { return; }

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

        public void SyncMultiplayer()
        {

            if (!Context.IsMultiplayer)
            {

                return;

            }

            if (!Context.IsMainPlayer)
            {

                return;

            }

            Helper.Multiplayer.SendMessage(save, "SaveSynchronise", modIDs: new[] { this.ModManifest.UniqueID });

        }

        public bool AttuneWeapon(Rite.rites blessing)
        {

            int toolIndex = AttuneableWeapon();

            if (toolIndex == -1) { return false; };

            save.attunement[toolIndex] = blessing;

            rite.shutdown();

            return true;

        }

        public bool DetuneWeapon()
        {

            int toolIndex = AttuneableWeapon();

            if (toolIndex == -1) { return false; };

            if (save.attunement.ContainsKey(toolIndex))
            {

                save.attunement.Remove(toolIndex);

            }

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

                Dictionary<int, int> coffeeList = SpawnData.CoffeeList();

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

    }

}
