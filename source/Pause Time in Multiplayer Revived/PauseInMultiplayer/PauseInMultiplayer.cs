/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mishagp/PauseInMultiplayer
**
*************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace PauseInMultiplayer
{
    public class PauseInMultiplayer : Mod
    {

        Dictionary<long, bool> eventStatus;
        public bool lastEventCheck = false;

        int timeInterval = -100;

        string pauseTime = "false";
        IDictionary<long, string> pauseTimeAll;

        string inSkull = "false";
        IDictionary<long, string> inSkullAll;
        int lastSkullLevel = 121;

        string pauseCommand = "false";

        bool shouldPauseLast = false;
        string inSkullLast = "false";

        bool extraTimeAdded = false;

        int healthLock = -100;
        Dictionary<StardewValley.Monsters.Monster, Vector2> monsterLocks = new Dictionary<StardewValley.Monsters.Monster, Vector2>();
        bool lockMonsters = false;

        bool votePause = false;
        IDictionary<long, bool> votePauseAll;

        bool pauseOverride = false;

        ModConfig config;

        Dictionary<string, int> buffDurations = new Dictionary<string, int>();

        public override void Entry(IModHelper helper)
        {
            this.config = Helper.ReadConfig<ModConfig>();

            bool skullElevatorMod = Helper.ModRegistry.Get("SkullCavernElevator") != null;
            if (skullElevatorMod)
            {
                this.Monitor.Log("DisableSkullShaftFix set to true due to SkullCavernElevator mod.", LogLevel.Debug);
                this.config.DisableSkullShaftFix = true;
            }

            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            Helper.Events.GameLoop.Saving += GameLoop_Saving;
            Helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            Helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            Helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;

            Helper.Events.Display.Rendered += Display_Rendered;

            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            //GMCM support
            var configMenu = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null) return;

            configMenu.Register(mod: this.ModManifest, reset: () => this.config = new ModConfig(), save: () => this.Helper.WriteConfig(this.config));

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Local Settings"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show X while paused",
                tooltip: () => "Toggles whether or not to display an X over the clock while paused.",
                getValue: () => this.config.ShowPauseX,
                setValue: value => this.config.ShowPauseX = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show vote messages",
                tooltip: () => "Toggles whether or not to update the chat with vote pause messages.",
                getValue: () => this.config.DisplayVotePauseMessages,
                setValue: value => this.config.DisplayVotePauseMessages = value
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Vote to pause keybind",
                tooltip: () => "Set as a key that you won't use for other purposes.",
                getValue: () => this.config.VotePauseHotkey,
                setValue: value => this.config.VotePauseHotkey = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Disable Skull Cavern shaft fix",
                tooltip: () => "Only set this to true if you have a specific reason to, such as using the Skull Cavern elevator mod.",
                getValue: () => this.config.DisableSkullShaftFix,
                setValue: value => this.config.DisableSkullShaftFix = value
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Host-Only Settings"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fix Skull Cavern time",
                tooltip: () => "(host only)\nToggles whether or not to slow down time like in single-player if all players are in the Skull Cavern.",
                getValue: () => this.config.FixSkullTime,
                setValue: value => this.config.FixSkullTime = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Monster/HP pause lock",
                tooltip: () => "(host only)\nToggles whether or not monsters will freeze and health will lock while paused.",
                getValue: () => this.config.LockMonsters,
                setValue: value => this.config.LockMonsters = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable vote to pause",
                tooltip: () => "(host only)\nToggles vote to pause functionality.\nThis is in addition to normal pause functionality.",
                getValue: () => this.config.EnableVotePause,
                setValue: value => this.config.EnableVotePause = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Match joining player vote to host",
                tooltip: () => "(host only)\nToggles whether or not joining players will automatically have their vote to pause set to the host's.",
                getValue: () => this.config.JoinVoteMatchesHost,
                setValue: value => this.config.JoinVoteMatchesHost = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable pause override key",
                tooltip: () => "(host only)\nToggles whether or not pressing the pause override key will toggle pausing the game.",
                getValue: () => this.config.EnablePauseOverride,
                setValue: value => this.config.EnablePauseOverride = value
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Override pause hotkey",
                tooltip: () => "(host only)\nSet as a key that you won't use for other purposes.",
                getValue: () => this.config.PauseOverrideHotkey,
                setValue: value => this.config.PauseOverrideHotkey = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Any cutscene pauses",
                tooltip: () => "(host only)\nWhen enabled, time will pause if any player is in a cutscene.",
                getValue: () => this.config.AnyCutscenePauses,
                setValue: value => this.config.AnyCutscenePauses = value
            );


        }

        private void Input_ButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == config.VotePauseHotkey)
            {
                votePauseToggle();
            }

            else if (e.Button == config.PauseOverrideHotkey && Context.IsMainPlayer && config.EnablePauseOverride)
            {
                pauseOverride = !pauseOverride;

                if (!config.DisplayVotePauseMessages)
                    return;

                if (pauseOverride)
                {
                    this.Helper.Multiplayer.SendMessage<string>("The host has paused via override.", "info", new[] { this.ModManifest.UniqueID });
                    Game1.chatBox.addInfoMessage("The host has paused via override.");
                }
                else
                {
                    this.Helper.Multiplayer.SendMessage<string>("The host has unpaused their override.", "info", new[] { this.ModManifest.UniqueID });
                    Game1.chatBox.addInfoMessage("The host has unpaused their override.");
                }
            }
        }

        private void GameLoop_DayEnding(object? sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            //reset invincibility settings while saving to help prevent future potential errors if the mod is disabled later
            //redundant with Saving to handle farmhand inconsistency
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.player.currentTemporaryInvincibilityDuration = 0;
            Game1.player.temporarilyInvincible = false;
        }

        private void GameLoop_Saving(object? sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            //reset invincibility settings while saving to help prevent future potential errors if the mod is disabled later
            //redundant with DayEnding to handle farmhand inconsistency
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.player.currentTemporaryInvincibilityDuration = 0;
            Game1.player.temporarilyInvincible = false;
        }

        private void Display_Rendered(object? sender, StardewModdingAPI.Events.RenderedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            //draw X over time indicator
            if (shouldPause() && this.config.ShowPauseX && Game1.displayHUD)
                Game1.spriteBatch.Draw(Game1.mouseCursors, updatePosition(), new Rectangle(269, 471, 15, 15), new Color(0, 0, 0, 64), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.91f);


        }

        private void Multiplayer_PeerDisconnected(object? sender, StardewModdingAPI.Events.PeerDisconnectedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                pauseTimeAll.Remove(e.Peer.PlayerID);
                inSkullAll.Remove(e.Peer.PlayerID);
                votePauseAll.Remove(e.Peer.PlayerID);
                eventStatus.Remove(e.Peer.PlayerID);
            }

        }

        private void Multiplayer_PeerConnected(object? sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                pauseTimeAll[e.Peer.PlayerID] = "false";
                inSkullAll[e.Peer.PlayerID] = "false";
                votePauseAll[e.Peer.PlayerID] = false;
                eventStatus[e.Peer.PlayerID] = false;

                //send current pause state
                this.Helper.Multiplayer.SendMessage(shouldPauseLast ? "true" : "false", "pauseCommand", new[] { this.ModManifest.UniqueID }, new[] { e.Peer.PlayerID });

                //send message denoting whether or not monsters will be locked
                this.Helper.Multiplayer.SendMessage(lockMonsters ? "true" : "false", "lockMonsters", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });


                //check for version match
                IMultiplayerPeerMod pauseMod = null;
                pauseMod = e.Peer.GetMod(this.ModManifest.UniqueID);
                if (pauseMod == null)
                    Game1.chatBox.addErrorMessage("Farmhand " + Game1.getFarmer(e.Peer.PlayerID).Name + " does not have Pause in Multiplayer mod.");
                else if (!pauseMod.Version.Equals(this.ModManifest.Version))
                {
                    Game1.chatBox.addErrorMessage("Farmhand " + Game1.getFarmer(e.Peer.PlayerID).Name + " has mismatched Pause in Multiplayer version.");
                    Game1.chatBox.addErrorMessage($"Host Version: {this.ModManifest.Version} | {Game1.getFarmer(e.Peer.PlayerID).Name} Version: {pauseMod.Version}");
                }

                //sets the joined player's vote to pause as the host's if enabled
                if (this.config.JoinVoteMatchesHost && this.config.EnableVotePause)
                {
                    votePauseAll[e.Peer.PlayerID] = votePause;

                    //sync votePause to local client for joining player
                    this.Helper.Multiplayer.SendMessage<bool>(votePause, "setVotePause", new[] { this.ModManifest.UniqueID }, new[] { e.Peer.PlayerID });

                    int votedYes = 0;
                    foreach (bool vote in votePauseAll.Values)
                        if (vote) votedYes++;

                    if (votePause)
                    {
                        this.Helper.Multiplayer.SendMessage<string>($"{Game1.getFarmer(e.Peer.PlayerID).Name} joined with a vote to pause. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                        Game1.chatBox.addInfoMessage($"{Game1.getFarmer(e.Peer.PlayerID).Name} joined with a vote to pause. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                    }
                    else
                    {
                        //seems unnecessary to display message for joining without a votepause via host
                        //this.Helper.Multiplayer.SendMessage<string>($"{Game1.getFarmer(e.Peer.PlayerID).Name} joined with a vote to unpause. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                        //Game1.chatBox.addInfoMessage($"{Game1.getFarmer(e.Peer.PlayerID).Name} joined with a vote to unpause. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                    }

                }

            }

        }

        private void Multiplayer_ModMessageReceived(object? sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "info")
            {
                if (config.DisplayVotePauseMessages)
                    Game1.chatBox.addInfoMessage(e.ReadAs<string>());

                return;
            }

            if (Context.IsMainPlayer)
            {
                if (e.FromModID == this.ModManifest.UniqueID)
                {
                    if (e.Type == "pauseTime")
                        pauseTimeAll[e.FromPlayerID] = e.ReadAs<string>();
                    else if (e.Type == "inSkull")
                        inSkullAll[e.FromPlayerID] = e.ReadAs<string>();
                    else if (e.Type == "votePause")
                    {
                        if (!config.EnableVotePause) return;

                        votePauseAll[e.FromPlayerID] = e.ReadAs<bool>();
                        int votedYes = 0;
                        foreach (bool vote in votePauseAll.Values)
                            if (vote) votedYes++;

                        if (e.ReadAs<bool>())
                        {
                            this.Helper.Multiplayer.SendMessage<string>($"{Game1.getFarmer(e.FromPlayerID).Name} voted to pause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                            Game1.chatBox.addInfoMessage($"{Game1.getFarmer(e.FromPlayerID).Name} voted to pause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                        }
                        else
                        {
                            this.Helper.Multiplayer.SendMessage<string>($"{Game1.getFarmer(e.FromPlayerID).Name} voted to unpause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                            Game1.chatBox.addInfoMessage($"{Game1.getFarmer(e.FromPlayerID).Name} voted to unpause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                        }
                    }
                    else if (e.Type == "eventUp")
                        eventStatus[e.FromPlayerID] = e.ReadAs<bool>();

                }
            }
            else
            {
                if (e.FromModID == this.ModManifest.UniqueID && e.Type == "pauseCommand")
                {
                    pauseCommand = e.ReadAs<string>();
                }
                else if (e.FromModID == this.ModManifest.UniqueID && e.Type == "lockMonsters")
                {
                    this.lockMonsters = e.ReadAs<string>() == "true" ? true : false;
                }
                else if (e.FromModID == this.ModManifest.UniqueID && e.Type == "setVotePause")
                {
                    this.votePause = e.ReadAs<bool>();
                }
            }

        }

        private void GameLoop_SaveLoaded(object? sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            //only the main player will use this dictionary
            if (Context.IsMainPlayer)
            {
                pauseTimeAll = new Dictionary<long, string>();
                pauseTimeAll[Game1.player.UniqueMultiplayerID] = "false";

                inSkullAll = new Dictionary<long, string>();
                inSkullAll[Game1.player.UniqueMultiplayerID] = "false";

                //setup lockMonsters for main player
                lockMonsters = this.config.LockMonsters;

                votePauseAll = new Dictionary<long, bool>();
                votePauseAll[Game1.player.UniqueMultiplayerID] = false;

                eventStatus = new Dictionary<long, bool>();
                eventStatus[Game1.player.UniqueMultiplayerID] = false;
            }

        }

        private void GameLoop_UpdateTicking(object? sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            //this mod does nothing if a game isn't running
            if (!Context.IsWorldReady) return;

            //start with checking for events
            if (lastEventCheck != Game1.eventUp)
            {
                //host
                if (Context.IsMainPlayer)
                {
                    eventStatus[Game1.player.UniqueMultiplayerID] = Game1.eventUp;
                }
                //client
                else
                {
                    this.Helper.Multiplayer.SendMessage<bool>(Game1.eventUp, "eventUp", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });
                }
                lastEventCheck = Game1.eventUp;
            }

            //set the pause time data to whether or not time should be paused for this player
            var pauseTime2 = (!Context.IsPlayerFree) ? "true" : "false";

            if (!Context.CanPlayerMove)
                pauseTime2 = "true";

            //time should not be paused when using a tool
            if (Game1.player.UsingTool)
                pauseTime2 = "false";

            //checks to see if the fishing rod has been cast. If this is true but the player is in the fishing minigame, the next if statement will pause - otherwise it won't
            if (Game1.player.CurrentItem != null && Game1.player.CurrentItem is StardewValley.Tools.FishingRod && (Game1.player.CurrentItem as StardewValley.Tools.FishingRod).isFishing)
                pauseTime2 = "false";

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is StardewValley.Menus.BobberBar)
                pauseTime2 = "true";

            if (Game1.currentMinigame != null)
                pauseTime2 = "true";

            //skip skull cavern fix logic if the main player has it disabled, or if is not multiplayer
            if (!Game1.IsMultiplayer)
                goto endSkullLogic;
            if (Context.IsMainPlayer && !this.config.FixSkullTime)
                goto endSkullLogic;


            //check status to see if player is in Skull Cavern
            if (Game1.player.currentLocation is StardewValley.Locations.MineShaft && (Game1.player.currentLocation as StardewValley.Locations.MineShaft).getMineArea() > 120)
                inSkull = "true";
            else
                inSkull = "false";

            if (inSkull != inSkullLast)
            {
                if (Context.IsMainPlayer)
                    inSkullAll[Game1.player.UniqueMultiplayerID] = inSkull;
                else
                    this.Helper.Multiplayer.SendMessage(inSkull, "inSkull", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });

                inSkullLast = inSkull;
            }

            //apply the logic to remove 2000 from the time interval if everyone is in the skull cavern and this hasn't been done yet per this 10 minute day period
            if (Context.IsMainPlayer && Game1.gameTimeInterval > 6000 && allInSkull())
            {
                if (!extraTimeAdded)
                {
                    extraTimeAdded = true;
                    Game1.gameTimeInterval -= 2000;
                }
            }

            if (Context.IsMainPlayer && Game1.gameTimeInterval < 10)
                extraTimeAdded = false;

            endSkullLogic:

            //handle pause time data
            if (Context.IsMainPlayer)
            {
                //host
                if (pauseTime != pauseTime2)
                {
                    pauseTime = pauseTime2;
                    pauseTimeAll[Game1.player.UniqueMultiplayerID] = pauseTime;
                }

            }

            else
            {
                //client
                if (pauseTime != pauseTime2)
                {
                    pauseTime = pauseTime2;
                    this.Helper.Multiplayer.SendMessage(pauseTime, "pauseTime", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });
                }
            }

            var shouldPauseNow = shouldPause();

            //this logic only applies for the main player to control the state of the world
            if (Context.IsMainPlayer)
            {
                if (shouldPauseNow)
                {

                    //save the last time interval, if it's not already saved
                    if (Game1.gameTimeInterval >= 0) timeInterval = Game1.gameTimeInterval;

                    Game1.gameTimeInterval = -100;

                    //pause all Characters
                    foreach (GameLocation location in Game1.locations)
                    {
                        //I don't know if the game stores null locations, and at this point I'm too afraid to ask
                        if (location == null) continue;

                        //pause all NPCs, doesn't seem to work for animals or monsters
                        foreach (Character character in location.characters)
                        {
                            character.movementPause = 1;
                        }

                        //pause all farm animals
                        if (location is Farm)
                            foreach (FarmAnimal animal in (location as Farm).getAllFarmAnimals())
                                animal.pauseTimer = 100;
                        else if (location is AnimalHouse)
                            foreach (FarmAnimal animal in (location as AnimalHouse).animals.Values)
                                animal.pauseTimer = 100;
                    }

                    if (!lockMonsters) goto endMonsterLogic;
                    //pause all Monsters
                    List<GameLocation> farmerLocations = new List<GameLocation>();
                    foreach (Farmer f in Game1.getOnlineFarmers())
                        farmerLocations.Add(f.currentLocation);
                    foreach (GameLocation l in farmerLocations)
                    {
                        foreach (Character c in l.characters)
                        {
                            if (c is StardewValley.Monsters.Monster)
                            {
                                if (!monsterLocks.ContainsKey(c as StardewValley.Monsters.Monster))
                                    monsterLocks.Add(c as StardewValley.Monsters.Monster, c.Position);
                                c.Position = monsterLocks[c as StardewValley.Monsters.Monster];
                            }
                        }
                    }
                endMonsterLogic:;

                }
                else
                {

                    //reset time interval if it hasn't been fixed from the last pause
                    if (Game1.gameTimeInterval < 0)
                    {

                        Game1.gameTimeInterval = timeInterval;
                        timeInterval = -100;
                    }

                    //reset monsterLocks
                    monsterLocks.Clear();

                }

                if (shouldPauseNow != shouldPauseLast)
                    this.Helper.Multiplayer.SendMessage(shouldPauseNow ? "true" : "false", "pauseCommand", new[] { this.ModManifest.UniqueID });

                shouldPauseLast = shouldPauseNow;
            }

            //check if the player has jumped down a Skull Cavern Shaft
            if (!this.config.DisableSkullShaftFix && inSkull == "true")
            {
                int num = (Game1.player.currentLocation as StardewValley.Locations.MineShaft).mineLevel - lastSkullLevel;

                if (num > 1)
                {

                    if (healthLock != -100)
                    {
                        Game1.player.health = Math.Max(1, Game1.player.health - num * 3);
                        healthLock = Game1.player.health;
                    }

                }

                lastSkullLevel = (Game1.player.currentLocation as StardewValley.Locations.MineShaft).mineLevel;
            }

            //pause food and drink buff durations must be run for each player independently
            //handle health locks on a per player basis
            if (shouldPauseNow)
            {
                //set temporary duration locks if it has just become paused and/or update Duration if new food is consumed during pause
                foreach (KeyValuePair<string, Buff> buff in Game1.player.buffs.AppliedBuffs)
                {
                    if (!buffDurations.ContainsKey(buff.Key))
                        buffDurations.Add(buff.Key, buff.Value.millisecondsDuration);
                    else
                    {
                        if (buff.Value.millisecondsDuration < buffDurations[buff.Key])
                        {
                            buff.Value.millisecondsDuration = buffDurations[buff.Key];
                        }
                    }
                }

                if (!lockMonsters) goto endHealthLogic;
                //health lock
                if (healthLock == -100)
                    healthLock = Game1.player.health;
                //catch edge cases where health has increased but asynchronously will not be applied before locking
                if (Game1.player.health > healthLock)
                    healthLock = Game1.player.health;

                Game1.player.health = healthLock;

                Game1.player.temporarilyInvincible = true;
                Game1.player.temporaryInvincibilityTimer = -1000000000;


            endHealthLogic:;

            }
            else
            {
                buffDurations.Clear();

                healthLock = -100;

                if (Game1.player.temporaryInvincibilityTimer < -100000000)
                {
                    Game1.player.temporaryInvincibilityTimer = 0;
                    Game1.player.currentTemporaryInvincibilityDuration = 0;
                    Game1.player.temporarilyInvincible = false;
                }
            }


        }

        public void votePauseToggle()
        {
            votePause = !votePause;
            if (Context.IsMainPlayer)
            {
                if (!config.EnableVotePause) return;

                votePauseAll[Game1.player.UniqueMultiplayerID] = votePause;
                int votedYes = 0;
                foreach (bool vote in votePauseAll.Values)
                    if (vote) votedYes++;

                if (votePause)
                {
                    this.Helper.Multiplayer.SendMessage<string>($"{Game1.player.Name} voted to pause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                    Game1.chatBox.addInfoMessage($"{Game1.player.Name} voted to pause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                }
                else
                {
                    this.Helper.Multiplayer.SendMessage<string>($"{Game1.player.Name} voted to unpause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})", "info", new[] { this.ModManifest.UniqueID });
                    Game1.chatBox.addInfoMessage($"{Game1.player.Name} voted to unpause the game. ({votedYes}/{Game1.getOnlineFarmers().Count})");
                }
            }
            else
            {
                this.Helper.Multiplayer.SendMessage<bool>(votePause, "votePause", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            }
        }

        private bool shouldPause()
        {
            try
            {
                if (Context.IsMainPlayer)
                {
                    //override
                    if (config.EnablePauseOverride && pauseOverride)
                        return true;

                    //votes
                    if (config.EnableVotePause)
                    {
                        bool votedPause = true;
                        foreach (bool vote in votePauseAll.Values)
                            if (!vote) votedPause = false;

                        if (votedPause)
                            return true;
                    }

                    //events
                    if (config.AnyCutscenePauses)
                    {
                        foreach (bool up in eventStatus.Values)
                            if (up)
                                return true;
                    }

                    //normal pause logic (terminates via false)
                    foreach (string pauseTime in pauseTimeAll.Values)
                        if (pauseTime == "false") return false;

                    return true;
                }
                else
                {
                    return pauseCommand == "true" ? true : false;
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log("Reinitializing pauseCommand.", LogLevel.Debug);
                pauseCommand = "false";
                return false;
            }


        }

        private bool allInSkull()
        {
            if (Context.IsMainPlayer)
            {
                foreach (string inSkull in inSkullAll.Values)
                    if (inSkull == "false") return false;

                return true;
            }

            return false;
        }

        private Vector2 updatePosition()
        {
            var position = new Vector2(Game1.uiViewport.Width - 300, 8f);
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                position = new Vector2(Math.Min(position.X, -Game1.uiViewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 300), 8f);
            }

            Utility.makeSafe(ref position, 300, 284);

            position.X += 23;
            position.Y += 55;

            return position;
        }


    }

    class ModConfig
    {
        public bool ShowPauseX { get; set; } = true;

        public bool FixSkullTime { get; set; } = true;

        public bool DisableSkullShaftFix { get; set; } = false;
        public bool LockMonsters { get; set; } = true;

        public bool AnyCutscenePauses { get; set; } = false;

        public bool EnableVotePause { get; set; } = true;

        public bool JoinVoteMatchesHost { get; set; } = true;

        public SButton VotePauseHotkey { get; set; } = SButton.Pause;

        public bool EnablePauseOverride { get; set; } = true;

        public SButton PauseOverrideHotkey { get; set; } = SButton.Scroll;

        public bool DisplayVotePauseMessages { get; set; } = true;
    }
}
