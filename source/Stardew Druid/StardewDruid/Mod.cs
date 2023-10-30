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
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Force.DeepCloner;
using StardewValley.Quests;
using static StardewValley.Minigames.MineCart.Whale;
using StardewValley.Menus;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using StardewDruid.Event;
using System.Dynamic;
using StardewDruid.Monster;
using StardewValley.BellsAndWhistles;
using System.Xml.Linq;
using System.Reflection.Emit;
using StardewDruid.Character;

namespace StardewDruid
{
    public class Mod : StardewModdingAPI.Mod
    {
        
        public ModData Config;

        private ActiveData activeData;

        private StaticData staticData;

        public Dictionary<int, string> weaponAttunement;

        private MultiplayerData multiplayerData;

        public Map.Effigy druidEffigy;

        public Dictionary<int, string> TreeTypes;

        public Dictionary<string, List<Vector2>> earthCasts;

        //private Dictionary<Type, List<Cast.CastHandle>> activeCasts;

        //private Event.ChallengeHandle activeChallenge;

        //private Event.ChallengeHandle inactiveChallenge;

        private Dictionary<string,Event.ChallengeHandle> challengeRegister;

        private Dictionary<string, Event.MarkerHandle> markerRegister;

        public Dictionary<string, Vector2> warpPoints;

        public Dictionary<string, Vector2> firePoints;

        public Dictionary<string, int> warpTotems;

        public List<string> warpCasts;

        public List<string> fireCasts;

        private Dictionary<string, Map.Quest> questIndex;

        private Queue<Rite> riteQueue;

        private Queue<int> castBuffer;

        public StardewValley.Tools.Pickaxe virtualPick;

        public StardewValley.Tools.Axe virtualAxe;

        public StardewValley.Tools.Hoe virtualHoe;

        public StardewValley.Tools.WateringCan virtualCan;

        public int currentTool;

        public Dictionary<string, string> locationPoll;

        public Dictionary<string, MonsterHandle> monsterHandles;

        public StardewValley.NPC disembodiedVoice;

        public Character.Effigy realEffigy;

        private bool trainedToday;

        public List<string> triggerList;

        public List<string> triggerActive;

        public bool receivedData;

        public string mineShaftName;

        public Dictionary<string,List<int>> riteWitnesses;

        override public void Entry(IModHelper helper)
        {

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += SaveLoaded;

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            helper.Events.GameLoop.OneSecondUpdateTicked += EverySecond;

            helper.Events.GameLoop.Saving += SaveUpdated;

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {

            Config = Helper.ReadConfig<ModData>();

            ConfigMenu.MenuConfig(this);
            
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            
            if (Context.IsMainPlayer) {

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

            warpPoints = Map.WarpData.WarpPoints();

            warpTotems = Map.WarpData.WarpTotems();

            firePoints = Map.FireData.FirePoints();

            Map.TileData.LoadSheets();

            AnimatedSprite effigySprite = CharacterData.CharacterSprite("Effigy");

            Texture2D effigyPortrait = CharacterData.CharacterPortrait("Effigy");

            Dictionary<int, int[]> effigySchedule = CharacterData.CharacterSchedule("Effigy");

            Vector2 effigyPosition = CharacterData.CharacterPosition("Effigy");

            string effigyDefaultMap = CharacterData.CharacterDefaultMap("Effigy");

            realEffigy = new(this, effigySprite, effigyPosition, effigyDefaultMap, 2, "Effigy", effigySchedule, effigyPortrait);

            GameLocation farmCave = Game1.getLocationFromName(effigyDefaultMap);

            farmCave.characters.Add(realEffigy);

            realEffigy.update(Game1.currentGameTime, farmCave );

            druidEffigy = new(
                this,
                Config.farmCaveStatueX,
                Config.farmCaveStatueY,
                Config.farmCaveHideStatue,
                Config.farmCaveMakeSpace
            );

            druidEffigy.ModifyCave();

            ReadyState();

        }

        private void StaticChecks()
        {

            int stableVersion = Map.QuestData.StableVersion();

            /*if (Config.masterStart && !staticData.blessingList.ContainsKey("masterStart"))
            {

                staticData = new StaticData() { staticVersion = stableVersion };

                staticData = Map.QuestData.ConfigureProgress(staticData,14);

                staticData.blessingList["masterStart"] = 1;

                return;

            }
            else if (staticData.blessingList.ContainsKey("masterStart"))
            {

                staticData = new StaticData() { staticVersion = stableVersion };

            }*/

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

        }

        private void SaveUpdated(object sender, SavingEventArgs e)
        {
            if(Context.IsMainPlayer)
            {
                
                Helper.Data.WriteSaveData("staticData", staticData);

            }
            else
            {
                
                Helper.Multiplayer.SendMessage(staticData, "FarmhandSave", modIDs: new[] { this.ModManifest.UniqueID });

            }

            druidEffigy.lessonGiven = false;

            /*foreach (KeyValuePair<Type, List<Cast.CastHandle>> castEntry in activeCasts)
            {
                
                if (castEntry.Value.Count > 0)
                {
                    
                    foreach (Cast.CastHandle castInstance in castEntry.Value)
                    {
                        
                        castInstance.CastRemove();
                    
                    }
                
                }

            }*/

            foreach (KeyValuePair<string, Event.ChallengeHandle> challengeEntry in challengeRegister)
            {

                challengeEntry.Value.EventRemove();

            }

            challengeRegister.Clear();

            foreach (KeyValuePair<string, MonsterHandle> monsterEntry in monsterHandles)
            {

                monsterEntry.Value.ShutDown();

            }
                
            activeData.castInterrupt = true;

            if (disembodiedVoice != null)
            {

                if (disembodiedVoice.currentLocation != null)
                {
                    disembodiedVoice.currentLocation.characters.Remove(disembodiedVoice);
                }
                
                disembodiedVoice = null;

            }

            ReadyState();

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

                        StaticData farmhandData = multiplayerData.farmhandData[e.FromPlayerID];

                        farmhandData.staticId = e.FromPlayerID;
                        
                        this.Helper.Multiplayer.SendMessage(farmhandData, "FarmhandLoad", modIDs: new[] { this.ModManifest.UniqueID });

                        //Game1.addHUDMessage(new HUDMessage($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}", ""));
                        Console.WriteLine($"Sent Stardew Druid data to Farmer ID {e.FromPlayerID}");
                    }

                }
                else if (e.Type == "FarmhandLoad" || e.Type == "FarmhandTrain")
                {

                    StaticData farmhandData = e.ReadAs<StaticData>();
                    
                    if(farmhandData.staticId == Game1.player.UniqueMultiplayerID)
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

            foreach(KeyValuePair<int,string> kvp in staticData.weaponAttunement)
            {

                weaponAttunement[kvp.Key] = kvp.Value;

            }

            triggerList = new();

            triggerActive = new();

            activeData = new ActiveData() { activeBlessing = staticData.activeBlessing };

            earthCasts = new();

            challengeRegister = new();

            markerRegister = new();

            monsterHandles = new();

            riteQueue = new();

            castBuffer = new();

            druidEffigy.DecorateCave();

            warpCasts = new();

            fireCasts = new();

            RiteTool(991);

            RiteTool(992);

            RiteTool(993);

            RiteTool(994);

            locationPoll = new();

            disembodiedVoice = new();

            riteWitnesses = new();

            // ---------------------- trigger assignment

            SynchroniseQuest();

            return;

        }

        private void EverySecond(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {

                return;

            }

            if (!LocationPoll("current"))
            {

                SetTriggers();

            }

            if (challengeRegister.Count == 0 && markerRegister.Count == 0)
            {
                return;
            }

            bool exitAll = false;

            bool extendAll = false;

            List<string> removeList = new();

            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {

                exitAll = true;

            }

            if (!Game1.shouldTimePass() || !Game1.game1.IsActive)
            {

                extendAll = true;

            }

            if (markerRegister.Count > 0 && !extendAll)
            {

                foreach (KeyValuePair<string, MarkerHandle> markerHandle in markerRegister)
                {

                    markerHandle.Value.EventInterval();

                }

            }

            if (challengeRegister.Count == 0)
            {
                
                return;
            
            }

            foreach (KeyValuePair<string, Event.ChallengeHandle> challengeEntry in challengeRegister)
            {

                if (exitAll)
                {

                    challengeEntry.Value.EventAbort();

                    challengeEntry.Value.EventRemove();

                    continue;

                }

                if (extendAll)
                {

                    challengeEntry.Value.EventExtend();

                    continue;

                }

                if (!challengeEntry.Value.EventActive())
                {

                    removeList.Add(challengeEntry.Key);

                    challengeEntry.Value.EventRemove();

                    continue;

                }

                challengeEntry.Value.EventInterval();

            }

            if (exitAll)
            {

                challengeRegister.Clear();

                return;
            }

            foreach (string removeChallenge in  removeList) 
            { 
            
                challengeRegister.Remove(removeChallenge);
            
            }

            /*if (activeCasts.Count == 0)
            {

                activeLockout = "none";

                return;

            }
            
            List<Cast.CastHandle> activeCast = new();

            List<Cast.CastHandle> removeCast = new();

            foreach (KeyValuePair<Type, List<Cast.CastHandle>> castEntry in activeCasts)
            {

                int entryCount = castEntry.Value.Count;

                if (castEntry.Value.Count > 0)
                {

                    int castIndex = 0;

                    foreach (Cast.CastHandle castInstance in castEntry.Value)
                    {

                        castIndex++;

                        if (castInstance.CastActive(castIndex, entryCount))
                        {

                            activeCast.Add(castInstance);

                        }
                        else
                        {

                            removeCast.Add(castInstance);

                        }


                    }


                }

            }

            foreach (Cast.CastHandle castInstance in removeCast)
            {
                
                castInstance.CastRemove();

                Type castType = castInstance.GetType();

                activeCasts[castType].Remove(castInstance);

                if (activeCasts[castType].Count == 0)
                {

                    activeCasts.Remove(castType);

                }

            }

            removeCast.Clear();

            bool exitAll = false;

            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {
                exitAll = true;
            
            }

            foreach (Cast.CastHandle castInstance in activeCast)
            {
                if (exitAll)
                {

                    castInstance.CastRemove();

                    Type castType = castInstance.GetType();

                    activeCasts[castType].Remove(castInstance);

                    if (activeCasts[castType].Count == 0)
                    {

                        activeCasts.Remove(castType);

                    }

                }
                if (!Game1.shouldTimePass() || !Game1.game1.IsActive)
                {

                    castInstance.CastExtend();
                    
                }
                else
                {

                    castInstance.CastTrigger();

                }

            }

            activeCast.Clear();

            return;*/

        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {

                return;

            }

            // ignore if player is busy with something else
            if (CasterBusy())
            {

                activeData.castInterrupt = true;

                riteQueue.Clear();

                castBuffer.Clear();

                return;

            }

            // simulates interactions with the farm cave effigy
            if (Game1.currentLocation.Name == "FarmCave")
            {

                Vector2 playerLocation = Game1.player.getTileLocation();

                Vector2 cursorLocation = Game1.currentCursorTile;

                if (playerLocation.X >= Config.farmCaveStatueX - 1 && playerLocation.X <= Config.farmCaveActionX + 1 && playerLocation.Y == Config.farmCaveActionY)
                {

                    foreach (SButton buttonPressed in e.Pressed)
                    {

                        if (buttonPressed.IsUseToolButton() || buttonPressed.IsActionButton())
                        {

                            Helper.Input.Suppress(buttonPressed);

                            druidEffigy.DialogueApproach();

                            return;

                        }

                    }

                }

                if (Config.riteButtons.GetState() == SButtonState.Pressed)
                {
                    druidEffigy.DialogueApproach();

                    return;
                }

            }

            int toolIndex = AttuneableWeapon();

            // ignore if player is busy with something else
            if(toolIndex == -1)
            {

                if (Config.riteButtons.GetState() == SButtonState.Pressed)
                {
                    Game1.addHUDMessage(new HUDMessage("Rite requires a melee weapon or tool", 2));

                }

                activeData.castInterrupt = true;

                riteQueue.Clear();

                castBuffer.Clear();

                return;

            }

            if(activeData.toolIndex != toolIndex && currentTool != toolIndex)
            {

                if(toolIndex >= 990)
                {

                    RiteTool(toolIndex, Game1.player.CurrentTool.UpgradeLevel);

                }

                currentTool = toolIndex;

                activeData.castInterrupt = true;

            }

            if (Config.riteButtons.GetState() == SButtonState.Pressed)
            {

                // new cast configuration
                if ((riteQueue.Count != 0 || castBuffer.Count != 0) && !(Config.unrestrictedStars && activeData.activeBlessing == "stars"))
                {

                    return;

                }

                if(!ResetCast(toolIndex))
                {
                    
                    return;

                }

            }
            else if (Config.riteButtons.GetState() != SButtonState.Held)
            {

                return;

            }

            activeData.chargeAmount--;

            if (activeData.chargeAmount > 0) {

                return;

            }

            if( activeData.castInterrupt)
            {

                if (!ResetCast(toolIndex))
                {

                    return;

                }

            }

            activeData.castLevel++;

            if(activeData.castLevel == 3)
            {
                
                if(activeData.activeBlessing == "earth")
                {

                    Vector2 currentVector = Game1.player.getTileLocation();

                    if(Vector2.Distance(currentVector,activeData.originVector) >= 4)
                    {

                        activeData.castLevel = 1;

                        activeData.originVector = currentVector;

                        if (activeData.castDoppler)
                        {

                            activeData.cycleLevel++;

                            activeData.castDoppler = false;

                        }
                        else
                        {

                            activeData.castDoppler = true;

                        }

                    }

                }

            }


            if (activeData.castLevel == 5)
            {

                if (activeData.activeBlessing == "water")
                {

                    activeData.activeDirection = -1;

                }

                activeData.castLevel = 1;

                activeData.cycleLevel++;

            }

            activeData.chargeAmount = 40;

            if (activeData.activeBlessing == "stars")
            {

                activeData.chargeAmount = 80;

            }

            if (activeData.activeBlessing != "water")
            {

                activeData.activeDirection = -1;

            }

            if (activeData.activeDirection == -1)
            {

                activeData.activeDirection = Game1.player.FacingDirection;

                activeData.activeVector = Game1.player.getTileLocation();

                if (activeData.activeBlessing == "water")
                {

                    List<int> targetList = ModUtility.CastWaterAdjust(activeData.activeVector, activeData.activeDirection);

                    activeData.activeDirection = targetList[0];

                    activeData.activeVector = new(targetList[1], targetList[2]);

                }

            }

            if (activeData.castLevel == 1)
            {

                if (EventHandler(activeData.activeVector))
                {
                    activeData.castInterrupt = true;

                    return;
                }

            }

            //int staminaRequired = 48;
            int staminaRequired = 32;

            // check player has enough energy for eventual costs
            if (Game1.player.Stamina <= staminaRequired)
            {
                AutoConsume();

                if (Game1.player.Stamina <= staminaRequired)
                {

                    if (activeData.castLevel >= 1)
                    {
                        Game1.addHUDMessage(new HUDMessage("Not enough energy to continue rite", 3));

                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage("Not enough energy to perform rite", 3));

                    }
                    activeData.castInterrupt = true;

                    return;

                }

            }

            Rite castRite = NewRite();
            
            castRite.castLevel = activeData.castLevel.ShallowClone();

            castRite.direction = activeData.activeDirection.ShallowClone();

            castRite.castType = activeData.activeBlessing.ShallowClone();

            castRite.castCycle = activeData.cycleLevel.ShallowClone();

            switch (activeData.activeBlessing)
            {

                case "stars":

                    riteQueue.Enqueue(castRite);

                    CastRite();

                    castBuffer.Enqueue(1);

                    DelayedAction.functionAfterDelay(ClearBuffer, 1333);

                    break;

                case "water":

                    castRite.castVector = activeData.activeVector.DeepClone();

                    ModUtility.AnimateWaterCast(activeData.activeVector, activeData.castLevel, activeData.cycleLevel);

                    riteQueue.Enqueue(castRite);

                    //DelayedAction.functionAfterDelay(CastRite, 666);
                    DelayedAction.functionAfterDelay(CastRite, 333);

                    break;

                default: //"earth"

                    int castLevel = (activeData.castDoppler) ? activeData.castLevel + 2 : activeData.castLevel;

                    ModUtility.AnimateEarthCast(activeData.activeVector,castLevel,activeData.cycleLevel);

                    riteQueue.Enqueue(castRite);

                    CastRite();

                    //DelayedAction.functionAfterDelay(CastRite, 666);

                    break;

            }


        }

        private bool ResetCast(int toolIndex)
        {
            
            if (activeData.activeBlessing == "none")
            {

                if (castBuffer.Count == 0)
                {

                    if (!EventHandler(Game1.player.getTileLocation()))
                    {

                        Game1.player.currentLocation.playSound("ghost");

                        castBuffer.Enqueue(1);

                        DelayedAction.functionAfterDelay(ClearBuffer, 1333);

                        Game1.addHUDMessage(new HUDMessage("Nothing happened... I should consult the Effigy in the Farmcave.", 2));
                    }
                    
                }

                activeData.castInterrupt = true;

                return false;

            }

            /*if(Game1.player.CurrentTool is not MeleeWeapon)
            {

                castBuffer.Enqueue(1);

                DelayedAction.functionAfterDelay(ClearBuffer, 1333);

                activeData.castInterrupt = true;

                Game1.addHUDMessage(new HUDMessage("Rite requires a Melee Weapon or Scythe to activate", 2));

                return false;


            }*/

            string activeBlessing = staticData.activeBlessing;

            //int toolIndex = AttuneableWeapon();

            //if(toolIndex == -1)
            //{
            //    return false;
            //}

            if (weaponAttunement.ContainsKey(toolIndex))
            {
                
                activeBlessing = weaponAttunement[toolIndex];

                if (!staticData.blessingList.ContainsKey(activeBlessing))
                {
                    
                    Game1.addHUDMessage(new HUDMessage("I'm not attuned to this artifact... perhaps the Effigy can help", 2));

                    return false;

                }

            }

            Dictionary<string, bool> spawnIndex = Map.SpawnData.SpawnIndex(Game1.player.currentLocation);

            // ignore if game location has no spawn profile
            if (spawnIndex.Count == 0)
            {

                if (castBuffer.Count == 0)
                {
                    castBuffer.Enqueue(1);

                    DelayedAction.functionAfterDelay(ClearBuffer, 1333);

                    Game1.addHUDMessage(new HUDMessage("Unable to reach the otherworldly plane from this location", 2));

                }

                activeData.castInterrupt = true;

                return false;

            }

            //if (activeLockout == "trigger" && activeBlessing == "earth")
            if (challengeRegister.ContainsKey("active") && activeBlessing == "earth")
            {
                if (castBuffer.Count == 0)
                {
                    castBuffer.Enqueue(1);

                    DelayedAction.functionAfterDelay(ClearBuffer, 1333);

                    Game1.addHUDMessage(new HUDMessage("Something is interfering with the rite!", 2));

                }

                return false;

            }

            activeData = new ActiveData
            {
                
                activeBlessing = activeBlessing,

                toolIndex = toolIndex,

                spawnIndex = spawnIndex,

                originVector = Game1.player.getTileLocation(),
    
            };

            return true;

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
            else if (Game1.player.CurrentTool is MeleeWeapon)
            {
                toolIndex = Game1.player.CurrentTool.InitialParentTileIndex;
            }

            return toolIndex;

        }

        private bool CasterBusy()
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

            string locationName = Game1.player.currentLocation.Name;

            Type locationType = Game1.player.currentLocation.GetType();

            triggerActive.Clear();

            foreach(KeyValuePair<string,MarkerHandle> markerHandle in markerRegister)
            {

                markerHandle.Value.EventRemove();

            }

            markerRegister.Clear();

            int markerCounter = 0;

            foreach (string castString in triggerList)
            {
                
                markerCounter++;

                Map.Quest questData = questIndex[castString];

                if (questData.triggerLocation != null)
                {
                    if (!questData.triggerLocation.Contains(locationName))
                    {
                        continue;
                    }
                }

                if (questData.triggerLocale != null)
                {
                    if (!questData.triggerLocale.Contains(locationType))
                    {
                        continue;
                    }
                }

                triggerActive.Add(castString);

                if(questData.markerType != null)
                {

                    MarkerHandle markerHandle = MarkerData.MarkerInstance(this, Game1.player.currentLocation, questData);

                    markerHandle.EventTrigger();

                    markerRegister[castString] = markerHandle;

                }

            }

        }

        private bool EventHandler(Vector2 targetVector)
        {

            if (challengeRegister.ContainsKey("active"))
            {

                return false; 
            
            }

            if(triggerActive.Count == 0)
            {
                
                return false;

            }

            // check if player has activated a triggered event
            Vector2 playerVector = Game1.player.getTileLocation();

            List<string> triggeredQuests = new();

            GameLocation playerLocation = Game1.player.currentLocation;

            foreach (string castString in triggerActive)
            {

                Map.Quest questData = questIndex[castString];

                if (questData.triggerBlessing != null)
                {
                    if (activeData.activeBlessing != questData.triggerBlessing)
                    {
                        continue;
                    }
                }

                if (questData.startTime != 0)
                {
                    if (Game1.timeOfDay < questData.startTime)
                    {
                        continue;
                    }
                }

                if (questData.triggerAnywhere)
                {

                    triggeredQuests.Add(castString);

                    continue;

                }

                if (questData.useTarget)
                {

                    playerVector = targetVector;

                }


                bool runTrigger = false;

                if (questData.triggerSpecial && !questData.vectorList.ContainsKey("triggerVector"))
                {

                    Vector2 specialTrigger = Map.QuestData.SpecialVector(playerLocation, castString);
                    
                    if (specialTrigger != new Vector2(-1))
                    {

                        questData.vectorList["targetVector"] = specialTrigger;

                        questData.vectorList["triggerVector"] = specialTrigger - new Vector2(1,1);

                        questData.vectorList["triggerLimit"] = new Vector2(3, 3);

                        // ----------------------------------

                        questIndex[castString].vectorList["triggerVector"] = questData.vectorList["triggerVector"];

                        questIndex[castString].vectorList["triggerLimit"] = questData.vectorList["triggerLimit"];

                    }
                
                }

                if (questData.vectorList.ContainsKey("triggerVector"))
                {

                    Vector2 TriggerVector = questData.vectorList["triggerVector"];

                    Vector2 TriggerLimit = TriggerVector + questData.vectorList["triggerLimit"];

                    if (
                        playerVector.X >= TriggerVector.X &&
                        playerVector.Y >= TriggerVector.Y &&
                        playerVector.X < TriggerLimit.X &&
                        playerVector.Y < TriggerLimit.Y
                        )
                    {
                        runTrigger = true;
                    }

                }

                if(questData.triggerTile != 0 || questData.triggerAction != null)
                {

                    Layer buildingLayer = playerLocation.Map.GetLayer("Buildings");

                    for (int i = 0; i < questData.triggerRadius; i++)
                    {

                        List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(playerLocation, playerVector, i);

                        foreach (Vector2 tileVector in tileVectors)
                        {

                            Tile buildingTile = buildingLayer.PickTile(new Location((int)tileVector.X * 64, (int)tileVector.Y * 64), Game1.viewport.Size);

                            if (buildingTile != null)
                            {

                                if (questData.triggerTile != 0)
                                {

                                    int tileIndex = buildingTile.TileIndex;

                                    if ((uint)(tileIndex - questData.triggerTile) <= 1u)
                                    {

                                        runTrigger = true; //1140

                                        questData.vectorList["targetVector"] = tileVector;

                                    }

                                }
                                else
                                {

                                    buildingTile.Properties.TryGetValue("Action", out var value3);
                                    
                                    if (value3 != null)
                                    {
                                        if(value3.ToString() == questData.triggerAction)
                                        {

                                            questData.vectorList["targetVector"] = tileVector;

                                            runTrigger = true;
                                        
                                        }

                                    }

                                }

                            }

                            if (runTrigger)
                            {
                                break;

                            }

                        }

                        if (runTrigger)
                        {
                            break;

                        }

                    }

                }

                if(runTrigger)
                {

                    triggeredQuests.Add(castString);

                }

            }

            if (triggeredQuests.Count == 0)
            {
                return false;

            }
                
            Cast.Rite rite = NewRite();

            rite.direction = Game1.player.getFacingDirection();

            foreach (string quest in triggeredQuests)
            {

                triggerList.Remove(quest);

                SetTriggers();

                Map.Quest questData = questIndex[quest];

                questData.name = quest;

                ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 500);

                Game1.player.currentLocation.playSound("yoba");

                switch (questData.triggerType)
                {
                    case "sword":

                        SwordHandle swordHandle = new(this, playerVector, rite, questData);

                        swordHandle.EventTrigger();

                        break;

                    default: // challenge

                        ChallengeHandle challengeHandle = ChallengeData.ChallengeInstance(this, playerVector, rite, questData);

                        challengeHandle.EventTrigger();

                        break;

                }

            }

            return true;

        }

        private Rite NewRite()
        {

            int damageLevel = 175;

            if (!Config.maxDamage)
            {

                damageLevel = 5 * Game1.player.CombatLevel;

                damageLevel += 1 * Game1.player.MiningLevel;

                damageLevel += 1 * Game1.player.ForagingLevel;

                damageLevel += 5 * virtualAxe.UpgradeLevel;

                damageLevel += 5 * virtualPick.UpgradeLevel;

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

            int monsterDifficulty = 1;

            switch (Config.combatDifficulty)
            {
                case "hard":

                    monsterDifficulty = 3;
                    break;

                case "medium":

                    monsterDifficulty = 2;
                    break;

            }

            int blessingLevel = 1;

            if (staticData.blessingList.ContainsKey("water"))
            {
                blessingLevel = 2;
            }

            if(staticData.blessingList.ContainsKey("stars"))
            {
                blessingLevel = 3;
            }

            int combatDifficulty = monsterDifficulty + blessingLevel;

            int combatModifier = Math.Min(Game1.player.CombatLevel + 5, 15) * (int)Math.Pow(combatDifficulty,2);

            Rite newRite = new()
            {

                castTask = staticData.taskList.DeepClone(),

                castToggle = staticData.toggleList.DeepClone(),

                castDamage = damageLevel,

                combatModifier = combatModifier,

            };

            return newRite;

        }

        private void CastRite()
        { 
            
            if(riteQueue.Count != 0)
            {

                Rite rite = riteQueue.Dequeue();

                Vector2 castPosition = rite.castVector * 64;

                float castLimit = (rite.castLevel * 128) + 32f;

                /*foreach (SerializableDictionary<Vector2, StardewValley.Object> mobject in rite.castLocation.objects)
                {
                    
                    Monitor.Log($"{mobject.GetType()}", LogLevel.Debug);
                    foreach (KeyValuePair<Vector2, StardewValley.Object> mobject2 in mobject)
                    {
                        Monitor.Log($"{mobject2.Value.Name}", LogLevel.Debug);
                        Monitor.Log($"{mobject2.Key}", LogLevel.Debug);
                    }
                }*/

                if (rite.castLocation.characters.Count > 0)
                {

                    if (rite.castType == "stars")
                    {
                        castLimit = 1000;

                    }
                    
                    foreach (NPC riteWitness in rite.castLocation.characters)
                    {
                        
                        if (riteWitness is StardewValley.Monsters.Monster)
                        {
                            continue;
                        }

                        if (Vector2.Distance(riteWitness.Position, castPosition) < castLimit)
                        {

                            if (WitnessedRite(rite, riteWitness))
                            {

                                continue;

                            }

                            if (riteWitness is Pet petPet)
                            {

                                petPet.checkAction(rite.caster, rite.castLocation);

                            }

                            if (riteWitness.isVillager() && riteWitness.Name != "Disembodied Voice")
                            {

                                if (rite.castType == "stars")
                                {

                                    riteWitness.doEmote(8);

                                    Game1.addHUDMessage(new HUDMessage($"{riteWitness.Name} could get hurt by the impacts!", 3));

                                    return;

                                }
                                else if (rite.castType == "water")
                                {

                                    riteWitness.doEmote(16);

                                    Game1.addHUDMessage(new HUDMessage($"{riteWitness.Name} is disturbed by the storm cloud", 2));

                                }
                                else
                                {

                                    Cast.GreetVillager greetVillager = new(this, rite.castVector, rite, riteWitness);

                                    greetVillager.GentlyCaress();

                                }

                            }

                        }

                    }

                }

                if (rite.castLocation is Farm farmLocation)
                {

                    foreach (KeyValuePair<long, FarmAnimal> pair in farmLocation.animals.Pairs)
                    {

                        if (Vector2.Distance(pair.Value.Position, castPosition) >= castLimit)
                        {

                            continue;

                        }

                        Cast.PetAnimal petAnimal = new(this, rite.castVector, rite, pair.Value);

                        petAnimal.GentlyCaress();

                    }

                }

                if (rite.castLocation is AnimalHouse animalLocation)
                {

                    foreach (KeyValuePair<long, FarmAnimal> pair in animalLocation.animals.Pairs)
                    {

                        if (Vector2.Distance(pair.Value.Position, castPosition) >= castLimit)
                        {

                            continue;

                        }

                        Cast.PetAnimal petAnimal = new(this, rite.castVector, rite, pair.Value);

                        petAnimal.GentlyCaress();

                    }

                }

                // Add cast buff if enabled
                if (Config.castBuffs)
                {

                    Buff magnetBuff = new("Druidic Magnetism", 6000, "Rite of the " + rite.castDisplay, 8);

                    magnetBuff.buffAttributes[8] = 128;

                    magnetBuff.which = 184651;

                    if (!Game1.buffsDisplay.hasBuff(184651))
                    {

                        Game1.buffsDisplay.addOtherBuff(magnetBuff);

                    }

                    Vector2 casterVector = rite.caster.getTileLocation();

                    if (rite.castLocation.terrainFeatures.ContainsKey(casterVector))
                    {
                       
                        if (rite.castLocation.terrainFeatures[casterVector] is StardewValley.TerrainFeatures.Grass)
                        {
                            Buff speedBuff = new("Druidic Freneticism", 6000, "Rite of the " + rite.castType, 9);

                            speedBuff.buffAttributes[9] = 2;

                            speedBuff.which = 184652;

                            if (!Game1.buffsDisplay.hasBuff(184652))
                            {

                                Game1.buffsDisplay.addOtherBuff(speedBuff);

                            }

                        }

                    }

                }

                switch (rite.castType)
                {

                    case "stars":

                        CastStars(rite); break;

                    case "water":

                        CastWater(rite); break;

                    default: //CastEarth

                        CastEarth(rite);

                        break;
                }

            }
        
        }

        private void ClearBuffer()
        {

            castBuffer.Clear();

        }

        private void CastEarth(Rite riteData = null)
        {

            int castCost = 0;

            riteData ??= new();

            List<Vector2> removeVectors = new();

            List<Vector2> terrainVectors = new();

            Dictionary<Vector2, Cast.CastHandle> effectCasts = new();

            Layer backLayer = riteData.castLocation.Map.GetLayer("Back");

            Layer buildingLayer = riteData.castLocation.Map.GetLayer("Buildings");

            int blessingLevel = staticData.blessingList["earth"];

            List<Vector2> clumpIndex;

            int rockfallChance = 10 - Math.Max(5, virtualPick.UpgradeLevel);

            string locationName = riteData.castLocation.Name;

            if (riteData.castLocation is MineShaft)
            {

                if (locationName != mineShaftName && earthCasts.ContainsKey(locationName))
                {

                    earthCasts.Remove(locationName);

                    mineShaftName = locationName;

                }

            }

            if (!earthCasts.ContainsKey(locationName))
            {
                earthCasts[locationName] = new();
                
            };

            int castRange;

            float castLimit = (riteData.castLevel * 2) + 0.5f;
            
            if (riteData.castLocation.largeTerrainFeatures.Count > 0)
            {

                foreach (LargeTerrainFeature largeTerrainFeature in riteData.castLocation.largeTerrainFeatures)
                {

                    if(largeTerrainFeature is not StardewValley.TerrainFeatures.Bush bushFeature)
                    {

                        continue;

                    }

                    Vector2 terrainVector = bushFeature.tilePosition.Value;
                    
                    if (earthCasts[locationName].Contains(terrainVector)) // already served
                    {

                        continue;

                    }
                    
                    if (Vector2.Distance(terrainVector, riteData.castVector) < castLimit)
                    {
                        
                        if (blessingLevel >= 2)
                        {
                            effectCasts[terrainVector] = new Cast.Bush(this, terrainVector, riteData, bushFeature);

                            earthCasts[locationName].Add(terrainVector);
                        }
                        
                    }

                    if (!terrainVectors.Contains(terrainVector)) // already served
                    {

                        terrainVectors.Add(terrainVector);

                    }

                }

            }

            if (riteData.castLocation.objects.Count() > 0 && blessingLevel >= 1 && riteData.spawnIndex["weeds"])
            //if (riteData.castLocation.objects.Count() > 0)
            {

                for (int i = 0; i < ((riteData.castLevel * 2) + 1); i++)
                {

                    List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(riteData.castLocation, riteData.castVector, i);

                    foreach (Vector2 tileVector in tileVectors)
                    {
                        
                        if (riteData.castLocation.objects.ContainsKey(tileVector))
                        {
                            //Monitor.Log($"{riteData.castLocation.objects[tileVector].Name}", LogLevel.Debug);
                            StardewValley.Object tileObject = riteData.castLocation.objects[tileVector];

                            if (tileObject.name.Contains("Stone"))
                            {

                                if (Map.SpawnData.StoneIndex().Contains(tileObject.ParentSheetIndex))
                                {

                                    effectCasts[tileVector] = new Cast.Weed(this, tileVector, riteData);

                                }

                            }
                            else if (tileObject.name.Contains("Weeds") || tileObject.name.Contains("Twig"))
                            {

                                effectCasts[tileVector] = new Cast.Weed(this, tileVector, riteData);

                            }
                            else if (riteData.castLocation is MineShaft && tileObject is BreakableContainer)
                            {

                                effectCasts[tileVector] = new Cast.Weed(this, tileVector, riteData);

                            }

                        }

                        continue;
                    
                    }
                
                }

            }

            for (int i = 0; i < 2; i++)
            {

                castRange = (riteData.castLevel * 2) - 1 + i;

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(riteData.castLocation, riteData.castVector, castRange);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (earthCasts[locationName].Contains(tileVector) || removeVectors.Contains(tileVector) || terrainVectors.Contains(tileVector)) // already served
                    {

                        continue;

                    }
                    else
                    {

                        earthCasts[locationName].Add(tileVector);

                    }

                    int tileX = (int)tileVector.X;

                    int tileY = (int)tileVector.Y;

                    Tile buildingTile = buildingLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                    if (buildingTile != null)
                    {

                        if (riteData.castLocation is Beach && !riteData.castToggle.ContainsKey("forgetFish") && riteData.randomIndex.Next((25 - riteData.caster.FishingLevel)) == 0)
                        {

                            List<int> tidalList = new() { 60, 61, 62, 63, 77, 78, 79, 80, 94, 95, 96, 97, 104, 287, 288, 304, 305, 321, 362, 363 };

                            if (tidalList.Contains(buildingTile.TileIndex))
                            {

                                effectCasts[tileVector] = new Cast.Pool(this, tileVector, riteData);

                            }

                        }
                        
                        if (buildingTile.TileIndexProperties.TryGetValue("Passable", out _) == false)
                        {

                            continue;

                        }

                    }

                    if (riteData.castLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (blessingLevel >= 2) {

                            TerrainFeature terrainFeature = riteData.castLocation.terrainFeatures[tileVector];

                            switch (terrainFeature.GetType().Name.ToString())
                            {

                                case "FruitTree":

                                    StardewValley.TerrainFeatures.FruitTree fruitFeature = terrainFeature as StardewValley.TerrainFeatures.FruitTree;

                                    if (fruitFeature.growthStage.Value >= 4)
                                    {

                                        effectCasts[tileVector] = new Cast.FruitTree(this, tileVector, riteData);

                                    }
                                    else if (staticData.blessingList["earth"] >= 4)
                                    {

                                        effectCasts[tileVector] = new Cast.FruitSapling(this, tileVector, riteData);

                                    }
                                    
                                    break;

                                case "Tree":

                                    StardewValley.TerrainFeatures.Tree treeFeature = terrainFeature as StardewValley.TerrainFeatures.Tree;

                                    if (treeFeature.growthStage.Value >= 5)
                                    {
                                        //Monitor.Log($"{riteData.castAxe.UpgradeLevel}", LogLevel.Debug);
                                        effectCasts[tileVector] = new Cast.Tree(this, tileVector, riteData);

                                    }
                                    else if(staticData.blessingList["earth"] >= 4 && treeFeature.fertilized.Value == false)
                                    {

                                        effectCasts[tileVector] = new Cast.Sapling(this, tileVector, riteData);

                                    }

                                    break;

                                case "Grass":

                                    effectCasts[tileVector] = new Cast.Grass(this, tileVector, riteData);

                                    break;

                                case "HoeDirt":

                                    if (staticData.blessingList["earth"] >= 4)
                                    {

                                        if (riteData.spawnIndex["cropseed"])
                                        {

                                            effectCasts[tileVector] = new Cast.Crop(this, tileVector, riteData);

                                        }

                                    }

                                    break;

                                default:

                                    break;

                            }

                        }

                        continue;

                    }

                    if (riteData.castLocation.resourceClumps.Count > 0)
                    {

                        bool targetClump = false;

                        clumpIndex = new()
                        {
                            tileVector,
                            tileVector + new Vector2(0,-1),
                            tileVector + new Vector2(-1,0),
                            tileVector + new Vector2(-1,-1)

                        };

                        foreach (ResourceClump resourceClump in riteData.castLocation.resourceClumps)
                        {

                            foreach (Vector2 originVector in clumpIndex)
                            {

                                if (resourceClump.tile.Value == originVector)
                                {

                                    if (blessingLevel >= 2)
                                    {

                                        switch (resourceClump.parentSheetIndex.Value)
                                        {

                                            case 600:
                                            case 602:

                                                effectCasts[tileVector] = new Cast.Stump(this, tileVector, riteData, resourceClump, "Farm");

                                                break;

                                            default:

                                                effectCasts[tileVector] = new Cast.Boulder(this, tileVector, riteData, resourceClump);

                                                break;

                                        }

                                    }

                                    Vector2 clumpVector = resourceClump.tile.Value;

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y));

                                    removeVectors.Add(new Vector2(clumpVector.X, clumpVector.Y + 1));

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y + 1));

                                    targetClump = true;

                                    continue;

                                }

                            }

                        }

                        if (targetClump)
                        {

                            continue;

                        }

                    }

                    if (riteData.castLocation is Woods)
                    {
                        if (blessingLevel >= 2)
                        {
                            Woods woodsLocation = riteData.castLocation as Woods;

                            foreach (ResourceClump resourceClump in woodsLocation.stumps)
                            {

                                if (resourceClump.tile.Value == tileVector)
                                {

                                    effectCasts[tileVector] = new Cast.Stump(this, tileVector, riteData, resourceClump, "Woods");

                                    Vector2 clumpVector = tileVector;

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y));

                                    removeVectors.Add(new Vector2(clumpVector.X, clumpVector.Y + 1));

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y + 1));

                                }

                            }

                        }

                    }

                    foreach (Furniture item in riteData.castLocation.furniture)
                    {

                        if (item.boundingBox.Value.Contains(tileX * 64, tileY * 64))
                        {

                            continue;

                        }

                    }

                    if (riteData.castLocation.objects.ContainsKey(tileVector))
                    {
                        
                        continue;

                    }

                    Tile backTile = backLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                    if (backTile != null)
                    {

                        if (riteData.spawnIndex["rockfall"])
                        {

                            if (blessingLevel >= 5)
                            {

                                if (riteData.castLocation is MineShaft || riteData.castLocation is VolcanoDungeon)
                                {


                                    bool generateRock = false;

                                    bool generateHoed = false;

                                    if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
                                    {

                                        if (typeValue == "Stone")
                                        {

                                            generateRock = true;

                                        }

                                        if (typeValue == "Dirt")
                                        {

                                            generateHoed = true;

                                        }

                                    }

                                    int probability = Game1.random.Next(rockfallChance);

                                    if (probability == 0)
                                    {

                                        effectCasts[tileVector] = new Cast.Rockfall(this, tileVector, riteData,generateRock,generateHoed);

                                    }

                                    continue;

                                }

                            }

                        }

                        if (backTile.TileIndexProperties.TryGetValue("Water", out _))
                        {

                            if (blessingLevel >= 2)
                            {

                                if (riteData.spawnIndex["fishup"] && !riteData.castToggle.ContainsKey("forgetFish") && riteData.randomIndex.Next((35-riteData.caster.FishingLevel)) == 0)
                                {

                                    if(riteData.castLocation.Name.Contains("Farm"))
                                    {

                                        effectCasts[tileVector] = new Cast.Pool(this, tileVector, riteData);

                                    }
                                    else
                                    {

                                        effectCasts[tileVector] = new Cast.Water(this, tileVector, riteData);

                                    }

                                }

                            }

                            continue;

                        }

                        if(riteData.castLocation is AnimalHouse)
                        {

                            if (backTile.TileIndexProperties.TryGetValue("Trough", out _))
                            {

                                effectCasts[tileVector] = new Cast.Trough(this, tileVector, riteData);

                                continue;

                            }

                        }

                        if (blessingLevel >= 3)
                        {
                            
                            if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
                            {

                                if (typeValue == "Dirt" || backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                                {

                                    effectCasts[tileVector] = new Cast.Dirt(this, tileVector, riteData);

                                    continue;

                                }

                                if (typeValue == "Grass" && backTile.TileIndexProperties.TryGetValue("NoSpawn", out _) == false)
                                {
                                    
                                    effectCasts[tileVector] = new Cast.Lawn(this, tileVector, riteData);

                                    continue;

                                }

                            }

                        }

                    }

                }

                //float colorIncrement = 1f - (0.1f * riteData.castLevel);

                //Color castColor = new(colorIncrement, 1, colorIncrement, 1f);

                Color castColor = new(0.6f, 1, 0.6f, 1f);

                foreach (Vector2 tileVector in tileVectors)
                {

                    ModUtility.AnimateCastRadius(riteData.castLocation, tileVector, castColor, i);

                }

            }

            //-------------------------- fire effects

            if (effectCasts.Count != 0)
            {
                foreach (KeyValuePair<Vector2, Cast.CastHandle> effectEntry in effectCasts)
                {

                    if (removeVectors.Contains(effectEntry.Key)) // ignore tiles covered by clumps
                    {

                        continue;

                    }

                    
                    Cast.CastHandle effectHandle = effectEntry.Value;

                    Type effectType = effectHandle.GetType();

                    if (activeData.castLimits.Contains(effectType))
                    {

                        continue;

                    }

                    effectHandle.CastEarth();

                    if (effectHandle.castFire)
                    {

                        castCost += effectHandle.castCost;

                    }

                    if (effectHandle.castLimit)
                    {

                        activeData.castLimits.Add(effectEntry.Value.GetType());

                    }

                }

            }

            //-------------------------- effect on player

            if (castCost > 0)
            {

                float oldStamina = Game1.player.Stamina;

                float staminaCost = Math.Min(castCost, oldStamina - 1);

                Game1.player.Stamina -= staminaCost;

                Game1.player.checkForExhaustion(oldStamina);

            }

            return;

        }

        private void CastWater(Rite riteData = null)
        {

            //-------------------------- tile effects

            int castCost = 0;

            riteData ??= new();

            Vector2 castVector = riteData.castVector;

            List <Vector2> tileVectors;

            List<Vector2> removeVectors = new();

            Dictionary<Vector2, Cast.CastHandle> effectCasts = new();

            Layer backLayer = riteData.castLocation.Map.GetLayer("Back");

            Layer buildingLayer = riteData.castLocation.Map.GetLayer("Buildings");

            int blessingLevel = staticData.blessingList["water"];

            string locationName = riteData.castLocation.Name.ToString();

            int castRange; 

            for (int i = 0; i < 2; i++) {

                castRange = (riteData.castLevel * 2) - 2 + i;

                tileVectors = ModUtility.GetTilesWithinRadius(riteData.castLocation, castVector, castRange);

                foreach (Vector2 tileVector in tileVectors)
                {

                    int tileX = (int)tileVector.X;

                    int tileY = (int)tileVector.Y;

                    if (blessingLevel >= 1)
                    {
                        if (warpPoints.ContainsKey(locationName))
                        {

                            if (warpPoints[locationName] == tileVector)
                            {

                                if (warpCasts.Contains(locationName))
                                {

                                    Game1.addHUDMessage(new HUDMessage($"Already extracted {locationName} warp power today", 3));

                                    continue;

                                }

                                int targetIndex = warpTotems[locationName];

                                effectCasts[tileVector] = new Cast.Totem(this, tileVector, riteData, targetIndex);

                                warpCasts.Add(locationName);

                                continue;

                            }

                        }

                    }

                    if (blessingLevel >= 1)
                    {
                        if (firePoints.ContainsKey(locationName))
                        {

                            if (firePoints[locationName] == tileVector)
                            {

                                if (fireCasts.Contains(locationName))
                                {

                                    Game1.addHUDMessage(new HUDMessage($"Already ignited {locationName} camp fire today", 3));

                                    continue;

                                }

                                effectCasts[tileVector] = new Cast.Campfire(this, tileVector, riteData);

                                fireCasts.Add(locationName);
                                
                                continue;

                            }

                        }

                    }

                    if (riteData.castLocation.objects.Count() > 0)
                    {

                        if (riteData.castLocation.objects.ContainsKey(tileVector))
                        {
                            
                            StardewValley.Object targetObject = riteData.castLocation.objects[tileVector];

                            if (riteData.castLocation.IsFarm && targetObject.bigCraftable.Value && targetObject.ParentSheetIndex == 9)
                            {

                                if (warpCasts.Contains("rod"))
                                {

                                    Game1.addHUDMessage(new HUDMessage("Already powered a lightning rod today", 3));

                                }
                                else if (blessingLevel >= 2)
                                {

                                    effectCasts[tileVector] = new Cast.Rod(this, tileVector, riteData);

                                    warpCasts.Add("rod");

                                }

                            }
                            else if (targetObject.Name.Contains("Campfire"))
                            {

                                string fireLocation = riteData.castLocation.Name;

                                if (fireCasts.Contains(fireLocation))
                                {

                                    Game1.addHUDMessage(new HUDMessage($"Already ignited {fireLocation} camp fire today", 3));

                                }
                                else if (blessingLevel >= 2)
                                {

                                    effectCasts[tileVector] = new Cast.Campfire(this, tileVector, riteData);

                                    fireCasts.Add(fireLocation);

                                }
                            }
                            else if (targetObject is Torch && targetObject.ParentSheetIndex == 93) // crafted candle torch
                            {

                                if (blessingLevel >= 5 && !challengeRegister.ContainsKey("active"))
                                {
                                    if (riteData.spawnIndex["portal"])
                                    {

                                        effectCasts[tileVector] = new Cast.Portal(this, tileVector, riteData);

                                    }

                                }

                            }
                            else if (targetObject.IsScarecrow())
                            {
                                
                                string scid = "scarecrow_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                                if (blessingLevel >= 2 && !Game1.isRaining && !warpCasts.Contains(scid))
                                {
                                    
                                    effectCasts[tileVector] = new Cast.Scarecrow(this, tileVector, riteData);

                                    warpCasts.Add(scid);

                                }
                            
                            }
                            else if (targetObject.Name.Contains("Artifact Spot") && virtualHoe.UpgradeLevel >= 3)
                            {

                                effectCasts[tileVector] = new Cast.Artifact(this, tileVector, riteData);

                            }

                            continue;

                        }

                    }

                    if (riteData.castLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (blessingLevel >= 1)
                        {

                            if(riteData.castLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree treeFeature)
                            {
                                
                                if (treeFeature.stump.Value)
                                {

                                    effectCasts[tileVector] = new Cast.Tree(this, tileVector, riteData);

                                }

                            }

                        }

                        continue;

                    }

                    Tile buildingTile = buildingLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                    if (buildingTile != null)
                    {

                        if (riteData.castLocation is Farm farmLocation)
                        {
                            int tileIndex = buildingTile.TileIndex;

                            if (tileIndex == 1938)
                            {
                                effectCasts[tileVector] = new Cast.PetBowl(this, tileVector, riteData);
                            }

                        }

                        continue;

                    }
                   
                    if (riteData.castLocation.terrainFeatures.ContainsKey(tileVector))
                    {
                        
                        continue;

                    }

                    if(riteData.castLocation is Forest forestLocation) 
                    {

                        if (blessingLevel >= 1)
                        {

                            if (forestLocation.log != null && forestLocation.log.tile.Value == tileVector)
                            {

                                effectCasts[tileVector] = new Cast.Stump(this, tileVector, riteData, forestLocation.log, "Log");

                                Vector2 clumpVector = tileVector;

                                removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y));

                                removeVectors.Add(new Vector2(clumpVector.X, clumpVector.Y + 1));

                                removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y + 1));

                            }

                        }

                    }
                    
                    if(riteData.castLocation is Woods)
                    {
                        if (blessingLevel >= 1)
                        {
                            Woods woodsLocation = riteData.castLocation as Woods;

                            foreach (ResourceClump resourceClump in woodsLocation.stumps)
                            {

                                if (resourceClump.tile.Value == tileVector)
                                {

                                    effectCasts[tileVector] = new Cast.Stump(this, tileVector, riteData, resourceClump, "Woods");

                                    Vector2 clumpVector = tileVector;

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y));

                                    removeVectors.Add(new Vector2(clumpVector.X, clumpVector.Y + 1));

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y + 1));

                                }

                            }

                        }

                    }

                    if (riteData.castLocation.resourceClumps.Count > 0)
                    {

                        bool targetClump = false;

                        foreach (ResourceClump resourceClump in riteData.castLocation.resourceClumps)
                        {

                            if (resourceClump.tile.Value == tileVector)
                            {

                                if (blessingLevel >= 1)
                                {

                                    switch (resourceClump.parentSheetIndex.Value)
                                    {

                                        case 600:
                                        case 602:

                                            effectCasts[tileVector] = new Cast.Stump(this, tileVector, riteData, resourceClump, "Farm");

                                            break;

                                        default:

                                            effectCasts[tileVector] = new Cast.Boulder(this, tileVector, riteData, resourceClump);

                                            break;

                                    }

                                    Vector2 clumpVector = tileVector;

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y));

                                    removeVectors.Add(new Vector2(clumpVector.X, clumpVector.Y + 1));

                                    removeVectors.Add(new Vector2(clumpVector.X + 1, clumpVector.Y + 1));

                                }

                                targetClump = true;

                                continue;

                            }

                        }

                        if (targetClump)
                        {

                            continue;

                        }

                    }

                }

                //float colorIncrement = 1f - (0.1f * riteData.castLevel);

                //Color castColor = new(colorIncrement, colorIncrement, 1, 1);

                Color castColor = new(0.6f, 0.6f, 1, 1);

                foreach (Vector2 tileVector in tileVectors) {

                    ModUtility.AnimateCastRadius(riteData.castLocation, tileVector, castColor,i);

                }

            }

            if (blessingLevel >= 3 && riteData.castLevel == 1)
            {

                if (riteData.spawnIndex["fishspot"])
                {

                    /*Dictionary<int, Vector2> portalOffsets = new()
                    {

                        [0] = new Vector2(0, -1),// up
                        [1] = new Vector2(1, 0), // right
                        [2] = new Vector2(0, 1),// down
                        [3] = new Vector2(-1, 0), // left

                    };

                    Vector2 fishVector = castVector + portalOffsets[riteData.direction];*/

                    Vector2 fishVector = castVector;

                    if (ModUtility.WaterCheck(riteData.castLocation, fishVector))
                    {

                        effectCasts[fishVector] = new Cast.Water(this, fishVector, riteData);

                    }


                }

                if(riteData.castLocation is VolcanoDungeon volcanoLocation)
                {
                    int tileX = (int)castVector.X;
                    int tileY = (int)castVector.Y;

                    if (volcanoLocation.waterTiles[tileX, tileY] && !volcanoLocation.cooledLavaTiles.ContainsKey(castVector))
                    {

                        effectCasts[castVector] = new Cast.Lava(this, castVector, riteData);

                    }

                }

            }

            if (blessingLevel >= 4)
            {

                int smiteCount = 0;

                Vector2 castPosition = riteData.castVector * 64;

                float castLimit = (riteData.castLevel * 128) + 32f;

                float castThreshold = Math.Max(0, castLimit - 320f);

                foreach (NPC nonPlayableCharacter in riteData.castLocation.characters)
                {

                    if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                    {

                        float monsterDifference = Vector2.Distance(monsterCharacter.Position, castPosition);

                        if (monsterDifference < castLimit && monsterDifference > castThreshold)
                        {

                            Vector2 monsterVector = monsterCharacter.getTileLocation();

                            effectCasts[monsterVector] = new Cast.Smite(this, monsterVector, riteData, monsterCharacter);

                            smiteCount++;

                            break;

                        } 

                    }

                    if (smiteCount == riteData.castLevel)
                    {
                       break;
                    }

                }

            }
            
            //-------------------------- fire effects

            if (effectCasts.Count != 0)
            {
                foreach (KeyValuePair<Vector2, Cast.CastHandle> effectEntry in effectCasts)
                {

                    if (removeVectors.Contains(effectEntry.Key)) // ignore tiles covered by clumps
                    {

                        continue;

                    }

                    Cast.CastHandle effectHandle = effectEntry.Value;

                    Type effectType = effectHandle.GetType();

                    if (activeData.castLimits.Contains(effectType))
                    {

                        continue;

                    }

                    effectHandle.CastWater();

                    if (effectHandle.castFire)
                    {

                        castCost += effectEntry.Value.castCost;

                    }

                    if (effectHandle.castLimit)
                    {

                        activeData.castLimits.Add(effectEntry.Value.GetType());

                    }

                }

            }

            //-------------------------- effect on player

            if (castCost > 0)
            {

                float oldStamina = Game1.player.Stamina;

                float staminaCost = Math.Min(castCost, oldStamina - 1);

                //staminaCost = 18;

                Game1.player.Stamina -= staminaCost;

                Game1.player.checkForExhaustion(oldStamina);

            }

        }

        private void CastStars(Rite riteData = null)
        {

            if (staticData.blessingList["stars"] < 1)
            {

                return;

            }

            //-------------------------- tile effects

            int castCost = 0;

            riteData ??= new();

            //-------------------------- cast sound

            Random randomIndex = new();

            Dictionary<Vector2, Cast.Meteor> effectCasts = new();

            int castAttempt = riteData.castLevel + 1;

            List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(riteData.castLocation, riteData.castVector, 2 + castAttempt);

            int castSelect = castSelection.Count;

            if(castSelect != 0)
            {
                
                int castIndex = 0;

                Vector2 newVector;

                int castSegment = castSelect / castAttempt;

                if(castSelect % castAttempt >= 2)
                {

                    castAttempt++;

                }

                int lastUpper = castSelect;


                int addedRange = 0;

                if (virtualAxe.UpgradeLevel >= 3)
                {

                    addedRange++;

                }

                if (virtualPick.UpgradeLevel >= 3)
                {

                    addedRange++;

                }

                int damageRadius = 2 + addedRange;

                for (int k = 0; k < castAttempt; k++)
                {

                    int castLower = castSegment * k;

                    int castHigher = castLower + castSegment;

                    bool priorityCast = false;

                    if (riteData.castLocation.objects.Count() > 0 && riteData.castTask.ContainsKey("masterMeteor"))
                    {

                        for (int j = castLower; j < Math.Min(castHigher, castSelection.Count); j++)
                        {

                            newVector = castSelection[j];

                            if (riteData.castLocation.objects.ContainsKey(newVector))
                            {

                                StardewValley.Object tileObject = riteData.castLocation.objects[newVector];

                                if (tileObject.name.Contains("Stone"))
                                {

                                    effectCasts[newVector] = new Cast.Meteor(this, newVector, riteData, damageRadius);

                                    priorityCast = true;

                                    break;

                                }

                            }

                        }

                        if (priorityCast)
                        {

                            continue;

                        }

                    }

                    if (!riteData.castTask.ContainsKey("masterMeteor"))
                    {

                        UpdateTask("lessonMeteor", 1);

                    }

                    castIndex = randomIndex.Next(castLower, Math.Min(castHigher,castSelection.Count));

                    newVector = castSelection[castIndex];

                    effectCasts[newVector] = new Cast.Meteor(this, newVector, riteData, damageRadius);

                    if(k == 0)
                    {

                        lastUpper = (castIndex + castSelect - 3);

                    }

                    if(castIndex >= lastUpper)
                    {

                        break;

                    }


                }

            }
            //-------------------------- fire effects

            if (effectCasts.Count != 0)
            {
                foreach (KeyValuePair<Vector2, Cast.Meteor> effectEntry in effectCasts)
                {

                    Cast.Meteor effectHandle = effectEntry.Value;

                    effectHandle.CastStars();

                    if (effectHandle.castFire)
                    {

                        castCost += effectEntry.Value.castCost;

                    }

                }

            }

            //-------------------------- effect on player

            if (castCost > 0)
            {

                float oldStamina = Game1.player.Stamina;

                float staminaCost = Math.Min(castCost, oldStamina - 1);

                Game1.player.Stamina -= staminaCost;

                Game1.player.checkForExhaustion(oldStamina);

            }

        }

        public string ActiveBlessing()
        {

            return staticData.activeBlessing;

        }

        public Dictionary<string, int> BlessingList()
        {

            return staticData.blessingList;

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

        public void RegisterChallenge(Event.ChallengeHandle challengeHandle, string placeHolder)
        {

            if (challengeRegister.ContainsKey(placeHolder))
            {

                challengeRegister[placeHolder].EventRemove();

            }

            challengeRegister[placeHolder] = challengeHandle;

        }

        public bool QuestComplete(string quest)
        {

            if (staticData.questList.ContainsKey(quest))
            {

                return staticData.questList[quest];

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

        public void SynchroniseQuest()
        {
            
            Dictionary<int, string> questIds = new();

            foreach (KeyValuePair<string,StardewDruid.Map.Quest> questData in questIndex)
            {

                if(questData.Value.questId != 0)
                {

                    questIds.Add(questData.Value.questId, questData.Key);

                }

                if (staticData.questList.Count == 0)
                {

                    NewQuest(questData.Key);

                }

            }

            for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
            {

                int gameId = Game1.player.questLog[num].id.Value;

                if (questIds.ContainsKey(gameId))
                {

                    string questName = questIds[gameId];

                    if (staticData.questList.ContainsKey(questName))
                    {
                        // player can see unfinished quest but mod has already turned over
                        if (staticData.questList[questName] && !Game1.player.questLog[num].completed.Value)
                        {
                            staticData.questList[questName] = false;

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

            foreach(KeyValuePair<string,bool> questPair in staticData.questList)
            {

                if (questPair.Value)
                {
                    continue;
                }

                ReassignQuest(questPair.Key);

            }


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
            //Monitor.Log($"{quest}", LogLevel.Debug);
            if (questData.triggerType != null)
            {

                if(!triggerList.Contains(quest))
                {
                    
                    triggerList.Add(quest);

                    SetTriggers();

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

            if (questData.triggerType != null)
            {

                if (triggerList.Contains(quest))
                {

                    triggerList.Remove(quest);

                    SetTriggers();

                }

            }

            if(questData.taskFinish != null)
            {

                staticData.taskList[questData.taskFinish] = 1;

            }

        }

        public void UpdateEffigy(string quest)
        {
            druidEffigy.questCompleted = quest;

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

             staticData = Map.QuestData.ConfigureProgress(staticData, 14);

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

            return staticData.toggleList;

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

        public NPC RetrieveVoice(GameLocation location, Vector2 position)
        {

            if (disembodiedVoice != null)
            {

                GameLocation previous = disembodiedVoice.currentLocation;

                if(previous == null)
                {

                    disembodiedVoice = null;

                } 
                else if (previous != location && disembodiedVoice.position != position)
                {
                    previous.characters.Remove(disembodiedVoice);

                    disembodiedVoice = null;

                }

            }

            if (disembodiedVoice == null)
            {

                disembodiedVoice = new StardewValley.NPC(new AnimatedSprite("Characters\\Junimo", 0, 16, 16), position, 2,"Disembodied Voice");

                disembodiedVoice.IsInvisible = true;

                disembodiedVoice.eventActor = true;

                disembodiedVoice.forceUpdateTimer = 9999;

                disembodiedVoice.collidesWithOtherCharacters.Value = true;

                disembodiedVoice.farmerPassesThrough = true;

                location.characters.Add(disembodiedVoice);

                disembodiedVoice.update(Game1.currentGameTime, location);

            }

            return disembodiedVoice;

        }

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

        public int SpawnMonster(GameLocation location, Vector2 vector, List<int> spawnIndex, string terrain = "ground")
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

                monsterHandle = new(this, vector, rite);

                monsterHandles[location.Name] = monsterHandle;

            }

            monsterHandle.spawnIndex = spawnIndex;

            if(terrain == "ground")
            {

                monsterHandle.SpawnGround(vector);

                return 1;

            } 
            else 
            {

                monsterHandle.TargetToPlayer(vector);

                Vector2 spawnVector = monsterHandle.SpawnVector();

                if(spawnVector != new Vector2(-1))
                {

                    monsterHandle.SpawnTerrain(spawnVector, vector, (terrain == "water"));

                    return 1;

                }

            }

            return 0;

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

        public bool WitnessedRite(Rite rite, NPC witness)
        {

            if (!riteWitnesses.ContainsKey(rite.castType))
            {

                riteWitnesses[rite.castType] = new()
                {
                    witness.id,

                };

                return false;

            }

            if (!riteWitnesses[rite.castType].Contains(witness.id))
            {

                riteWitnesses[rite.castType].Add(witness.id);

                return false;

            }

            return true;

        }

        public void Log(string message)
        {

            Monitor.Log(message, LogLevel.Debug);

        }


    }

}
