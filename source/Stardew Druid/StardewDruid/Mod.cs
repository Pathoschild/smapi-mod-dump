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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewDruid.Journal.Druid;


namespace StardewDruid
{

    public class Mod : StardewModdingAPI.Mod
    {

        public ModData Config;

        public bool modReady;

        public bool receivedData;

        internal static Mod instance;

        public Rite rite;

        public IconData iconData;

        public QuestHandle questHandle;

        public HerbalData herbalData;

        public RelicsData relicsData;

        internal StardewDruid.Data.StaticData save;

        public Dictionary<string, Event.EventHandle> eventRegister = new();

        public Dictionary<string, string> activeEvent = new();

        public Dictionary<int, string> clickRegister = new();

        public List<SpellHandle> spellRegister = new();

        public List<ThrowHandle> throwRegister = new();

        public double messageBuffer;

        public double consumeBuffer;

        public StardewValley.Tools.Pickaxe virtualPick;

        public StardewValley.Tools.Axe virtualAxe;

        public StardewValley.Tools.Hoe virtualHoe;

        public StardewValley.Tools.WateringCan virtualCan;

        public int currentTool;

        public Dictionary<CharacterData.characters, StardewDruid.Character.Character> characters = new();

        public Dictionary<CharacterData.characters, StardewDruid.Dialogue.Dialogue> dialogue = new();

        public Dictionary<CharacterData.characters, Character.TrackHandle> trackers = new();

        public Dictionary<CharacterData.characters, StardewValley.Objects.Chest> chests = new();

        public Dictionary<string, Dictionary<Vector2, string>> featureRegister = new();

        public Dictionary<string, GameLocation> locations = new();

        public Random randomIndex = new();

        public int version = 220;

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

            ConfigMenu.MenuConfig(this);

            SoundData.AddSounds();

            iconData = new IconData();

        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            eventRegister = new();

            activeEvent = new();

            clickRegister = new();

            spellRegister = new();

            throwRegister = new();

            characters = new();

            dialogue = new();

            chests = new();

            trackers = new();

            featureRegister = new();

            locations = new();

            if (!Context.IsMainPlayer)
            {

                Helper.Multiplayer.SendMessage(new QueryData(), "SaveRequest", modIDs: new[] { this.ModManifest.UniqueID });

                return;

            }

            save = Helper.Data.ReadSaveData<StardewDruid.Data.StaticData>("saveData_" + version.ToString());

            save ??= VersionData.Reconfigure();

            questHandle = new QuestHandle();

            herbalData = new HerbalData();

            relicsData = new RelicsData();

            if (Config.setMilestone != 0)
            {

                if (save.set != Config.setMilestone)
                {

                    save = new();

                    save.set = Config.setMilestone;

                    questHandle.Promote((QuestHandle.milestones)Config.setMilestone);

                    if (Config.setOnce)
                    {

                        Config.setOnce = false;

                        Config.setMilestone = 0;

                        Helper.Data.WriteJsonFile("config.json",Config);

                    }


                }

            }

            if (save.milestone == QuestHandle.milestones.none)
            {

                questHandle.Promote((QuestHandle.milestones)1);

            }

            Helper.Data.WriteJsonFile("saveData.json", save);

            ReadyState();

        }

        private void SaveImminent(object sender, SavingEventArgs e)
        {

            if (Context.IsMainPlayer)
            {

                foreach (KeyValuePair<CharacterData.characters, StardewDruid.Character.Character> character in characters)
                {

                    switch (character.Value.modeActive)
                    {

                        case Character.Character.mode.scene:
                        case Character.Character.mode.random:

                            save.characters[character.Key] = Character.Character.mode.home;

                            break;

                        default:

                            save.characters[character.Key] = character.Value.modeActive;

                            break;

                    }

                }

                foreach (KeyValuePair<CharacterData.characters, StardewValley.Objects.Chest> chest in chests)
                {

                    //save.chests[chest.Key].Clear();

                    if (save.chests.ContainsKey(chest.Key))
                    {

                        save.chests[chest.Key].Clear();

                    }
                    else
                    {

                        save.chests[chest.Key] = new();

                    }

                    foreach (Item item in chest.Value.Items)
                    {

                        save.chests[chest.Key].Add(new() { id = item.itemId.Value, quality = item.quality.Value, stack = item.stack.Value, });

                    }

                }

                Helper.Data.WriteSaveData("saveData_" + version.ToString(), save);

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

            Game1.currentSpeaker = null;

            Game1.objectDialoguePortraitPerson = null;

            foreach(KeyValuePair<string,GameLocation> location in locations)
            {

                Game1.locations.Remove(location.Value);

                Game1.removeLocationFromLocationLookup(location.Value);

            }

        }

        private void SaveUpdated(object sender, SavedEventArgs e)
        {

            foreach (KeyValuePair<string, GameLocation> location in locations)
            {

                Game1.locations.Add(location.Value);

                location.Value.updateWarps();

            }

            foreach (KeyValuePair<CharacterData.characters, StardewDruid.Character.Character> character in characters)
            {

                character.Value.SwitchToMode(save.characters[character.Key], Game1.player);

            }

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

                    if (eventEntry.TriggerAbort())
                    {

                        removal.Add(eventHandle.Key);

                    }
                    else
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

            Druid.journalTypes journalPressed = Journal.Druid.JournalButtonPressed();

            if (casterBusy)
            {

                rite.shutdown();

                if (Game1.activeClickableMenu != null)
                {

                    if (ritePressed)
                    {
                        
                        if (Game1.activeClickableMenu is QuestLog)
                        {

                            Game1.activeClickableMenu = new Druid();

                        }
                        else if(Game1.activeClickableMenu is Druid)
                        {

                            Game1.activeClickableMenu.exitThisMenu(true);

                        }

                    }

                    if(journalPressed != journalTypes.none)
                    {
                        
                        if (Game1.activeClickableMenu is Druid druidJournal)
                        {

                            if (druidJournal.type == journalPressed)
                            {

                                druidJournal.exitThisMenu(true);

                            }
                            else
                            {

                                druidJournal.switchTo(journalPressed);

                            }

                        }

                    }

                }

                return;

            }

            if (journalPressed != journalTypes.none)
            {

                rite.shutdown();

                Game1.activeClickableMenu = new Druid(journalPressed);

                return;

            }


            if (ritePressed)
            {

                /*foreach(Layer layer in Game1.player.currentLocation.map.Layers)
                {

                    Monitor.Log(layer.Id.ToString(), LogLevel.Debug);

                }

                for (int i = 0; i < 1; i++)
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

            for (int j = throwRegister.Count - 1; j >= 0; j--)
            {

                ThrowHandle throwing = throwRegister[j];

                if (!throwing.update())
                {

                    throwRegister.RemoveAt(j);

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

                if(type == 0 || type == -1)
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

            if (herbalData.applied.ContainsKey(HerbalData.herbals.ligna))
            {

                damageLevel += (int)(damageLevel * 0.125f * herbalData.applied[HerbalData.herbals.ligna].level);

            }

            return damageLevel;

        }

        public List<float> CombatCritical()
        {

            float critChance = 0.05f;

            if (Game1.player.professions.Contains(25))
            {

                critChance += 0.1f;

            }

            if (Game1.player.hasBuff("statue_of_blessings_5"))
            {
                
                critChance += 0.05f;
            
            }

            if (herbalData.applied.ContainsKey(HerbalData.herbals.impes))
            {

                critChance += (0.05f * herbalData.applied[HerbalData.herbals.impes].level);

            }

            float critModifier = 0.5f;

            if (Game1.player.professions.Contains(29))
            {
                
                critModifier += 0.5f;

            }

            return new() { critChance, critModifier };

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

            List<int> herbalLines = new();

            for(int i = 5; i > 0; i--)
            {
                
                if (Game1.player.Stamina >= (Game1.player.MaxStamina * 0.5) && Game1.player.health >= (Game1.player.maxHealth * 0.5))
                {

                    break;

                }

                for (int j = 0; j < 3; j++)
                {

                    int h = (j * 5) + i;

                    if (herbalLines.Contains(j))
                    {

                        continue;

                    }

                    if (save.herbalism.ContainsKey((HerbalData.herbals)h))
                    {

                        if (save.herbalism[(HerbalData.herbals)h] > 0)
                        {

                            string herbId = ((HerbalData.herbals)h).ToString();

                            herbalData.ConsumeHerbal(herbId);

                            herbalLines.Add(j);

                        }

                    }

                }

            }

            if (!Config.slotConsume)
            {
                return;
            }

            Dictionary<int, string> slots = new()
            {
                [0] = Config.slotOne,
                [1] = Config.slotTwo,
                [2] = Config.slotThree,
                [3] = Config.slotFour,
                [4] = Config.slotFive,
                [5] = Config.slotSix,
                [6] = Config.slotSeven,
                [7] = Config.slotEight,
                [8] = Config.slotNine,
                [9] = Config.slotTen,
                [10] = Config.slotEleven,
                [11] = Config.slotTwelve,

            };

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {

                if (i >= 12)
                {

                    break;

                }

                if (slots[i] != "lunch")
                {

                    continue;

                }

                if (Game1.player.Stamina >= Game1.player.MaxStamina / 2 && Game1.player.health >= (Game1.player.maxHealth / 2))
                {

                    break;

                }

                Item checkSlot = Game1.player.Items[i];

                // ignore empty slots
                if (checkSlot == null || checkSlot is not StardewValley.Object checkItem)
                {

                    continue;
                
                }

                //Item checkItem = checkSlot.getOne();

                if (@checkItem.Edibility > 0)
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

                    Microsoft.Xna.Framework.Color messageColour = Microsoft.Xna.Framework.Color.White;

                    if (Game1.player.MaxStamina > Game1.player.Stamina)
                    {

                        Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + (@checkItem.staminaRecoveredOnConsumption()));

                    }

                    if (Game1.player.maxHealth > Game1.player.health)
                    {

                        Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + @checkItem.healthRecoveredOnConsumption());

                    }

                    Game1.player.Items[i].Stack -= 1;

                    if (Game1.player.Items[i].Stack <= 0)
                    {
                        Game1.player.Items[i] = null;

                    }

                    //Microsoft.Xna.Framework.Rectangle healthBox = Game1.player.GetBoundingBox();

                    //Game1.player.currentLocation.debris.Add(
                    //    new Debris(checkItem.DisplayName, 1, new Vector2(healthBox.Center.X + 16, healthBox.Center.Y), Microsoft.Xna.Framework.Color.Goldenrod, 0.75f,0f)
                    //);

                    if (consumeBuffer < Game1.currentGameTime.TotalGameTime.TotalSeconds)
                    {

                        ConsumeEdible hudmessage = new("Consumed " + checkItem.DisplayName + " for " + @checkItem.staminaRecoveredOnConsumption().ToString() + " stamina", checkItem);

                        Game1.addHUDMessage(hudmessage);

                        consumeBuffer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

                    }

                    break;

                }

            }

        }

    }

}

