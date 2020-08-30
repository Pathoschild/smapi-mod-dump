using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SvFishingMod
{
    public sealed partial class FishingMod : Mod
    {
        private static int defaultMaxFishingBiteTime = -1;
        private static int defaultMinFishingBiteTime = -1;
        public const string MultiplayerSettingsMessageType = "EnforcedSessionSettings";

        private CircularBuffer<int> _circularFishList { get; set; } = null;
        private SortedList<int, string> _fishList { get; set; } = null;
        private BobberBar _fishMenu { get; set; }
        private bool EnableDebugOutput { get; set; } = false;
        private int FirstFishId { get; set; } = -1;
        private int LastFishId { get; set; } = -1;
        private ISemanticVersion CurrentVersion { get; set; } = null;
        private string CurrentModId { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            Settings.HelperInstance = helper;
            Settings.MonitorInstance = Monitor;
            Settings.ConfigFilePath = "svfishmod.json";
            Settings.LoadFromFile();

            CurrentVersion = helper.ModRegistry.Get(helper.ModRegistry.ModID).Manifest.Version;
            CurrentModId = helper.ModRegistry.ModID;

            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;

            helper.ConsoleCommands.Add("sv_fishing_debug", "Enables or disables Debug mode on the SvFishingMod.\nUsage: sv_fishing_debug 0|1", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_enabled", "Enables or disables SvFishingMod.\nUsage: sv_fishing_enabled 0|1", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_autoreel", "Enables or disables the Auto reel functionality of the SvFishingMod.\nUsage: sv_fishing_autoreel 0|1.", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_reload", "Reloads from disk the configuration file used by the SvFishingMod.\nUsage: sv_fishing_reload", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_search", "Searches the fish list for a fish that contains the specified string on its name.\nUsage: sv_fishing_search <keyword>", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_setfish", "Forces the next fishing event to give a fish with the specified id.\nUsage: sv_fishing_setfish <fish_id>\nUse sv_fishing_search to get the id of a given fish by name.\nUse -1 as the fish id to restore original game functionality.", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_fishcycling", "Enables or disables the reeled fish cycling feature.\nUsage: sv_fishing_fishcycling 0|1\nWhen enabled, this feature will allow you to automatically reel all possibles fishes one after another each time you throw your fishrod.", HandleCommand);
            helper.ConsoleCommands.Add("sv_fishing_bitedelay", "Enables or disables the bite delay for fishes once the rod has been casted into the water.\nUsage: sv_fishing_bitedelay 0|1\nIf the value is 1, the original game mechanics will be used and the fish will bite after a random amount of time.", HandleCommand);

            defaultMaxFishingBiteTime = maxFishingBiteTime;
            defaultMinFishingBiteTime = minFishingBiteTime;
        }

        private void Multiplayer_PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
        {
            if (!e.Peer.IsHost)
            {
                // Send Settings update package
                if (e.Peer.HasSmapi)
                {
                    Monitor.Log(string.Format("Sending mod settings to newly connected peer with ID: {0}, GameVersion {1} and SMAPIVersion {2}.", e.Peer.PlayerID, e.Peer.GameVersion, e.Peer.ApiVersion));
                    Helper.Multiplayer.SendMessage(Settings.Local, MultiplayerSettingsMessageType, new string[] { CurrentModId }, new long[] { e.Peer.PlayerID });
                }
                else
                {
                    Monitor.Log(string.Format("Connected peer {0} doesnt have SMAPI installed, hence the mod settings could not be synced.", e.Peer.PlayerID));
                }
            }
        }

        private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (!string.Equals(e.FromModID, CurrentModId, StringComparison.OrdinalIgnoreCase))
                return;

            ISemanticVersion ver = Helper.Multiplayer.GetConnectedPlayer(e.FromPlayerID)?.GetMod(CurrentModId)?.Version ?? null;
            bool compatible = ver?.IsNewerThan(CurrentVersion) ?? false;

            if (!compatible)
            {
                Monitor.Log(string.Format("Cannot use this mod during this online session because the host is currently using a more recent version of the mod. Update this mod to enable it on this server. RemoteVer: {0}. LocalVer: {1}", ver?.ToString() ?? "Null", CurrentVersion));
                Settings.Remote = Settings.DefaultDisabled;
                return;
            }

            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.UniqueMultiplayerID == e.FromPlayerID)
                {
                    if (farmer.IsMainPlayer)
                    {
                        // Message received from server
                        if (string.Equals(e.Type, MultiplayerSettingsMessageType, StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                Settings data = e.ReadAs<Settings>();

                                if (data != null)
                                {
                                    Settings.Remote = data;
                                    Monitor.Log(string.Format("Remote settings updated from server. FarmerId: {0}.", e.FromPlayerID));
                                }
                            }
                            catch (Exception ex)
                            {
                                Monitor.Log(string.Format("Remote settings cannot be updated from server. FarmerId: {0}. Ex: {1}. Msg: {2}. InnerEx: {3}", e.FromPlayerID, ex.GetType().Name, ex.Message, ex.InnerException?.GetType().Name ?? "None"));
                            }
                        }
                        else
                        {
                            Monitor.Log(string.Format("Unknown mod message received from server. FarmerId: {0}. Type: {1}.", e.FromPlayerID, e.Type));
                        }
                    }
                    else
                    {
                        Monitor.Log(string.Format("Remote mod message received from a non-host farmer. FarmerId: {0}. Type: {1}.", e.FromPlayerID, e.Type));
                    }

                    return;
                }
            }

            Monitor.Log(string.Format("Remote mod message received from an unknown farmer. PlayerId: {0}. Type: {1}", e.FromPlayerID, e.Type));
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Settings.Remote = null;
            if (Game1.multiplayerMode == 0)
            {
                Settings.IsMultiplayerSession = false;
                Settings.IsServer = true;
            }
            else
            {
                Settings.IsMultiplayerSession = true;
                Settings.IsServer = Game1.multiplayerMode == Game1.multiplayerServer;
            }
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            Settings.Remote = null;
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            FishingRod rod = Game1.player.CurrentTool as FishingRod;

            if (rod == null)
                return;

            if (Settings.Active.RemoveBiteDelay)
            {
                minFishingBiteTime = -20000;
                maxFishingBiteTime = 1;
            }    
            else
            {
                minFishingBiteTime = defaultMinFishingBiteTime;
                maxFishingBiteTime = defaultMaxFishingBiteTime;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
            base.Dispose(disposing);
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            BobberBar fishBarMenu = e.NewMenu as BobberBar;
            FishingRod fishTool = Game1.player.CurrentTool as FishingRod;

            if (fishBarMenu == null || fishTool == null || Settings.Active.DisableMod)
                return;

            _fishMenu = fishBarMenu;

            int attachmentValue = fishTool.attachments[0] == null ? -1 : fishTool.attachments[0].parentSheetIndex;
            bool caughtDouble = Settings.Active.AlwaysCatchDoubleFish || (bossFish && attachmentValue == 774 && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0);

            if (Settings.Active.OverrideFishType >= 0) whichFish = Settings.Active.OverrideFishType;
            if (Settings.Active.OverrideFishQuality >= 0) fishQuality = Settings.Active.OverrideFishQuality;
            if (Settings.Active.AlwaysPerfectCatch) perfect = true;
            if (Settings.Active.AlwaysCatchTreasure)
            {
                treasure = true;
                treasureCaught = true;
            }
            if (Settings.Active.DistanceFromCatchingOverride >= 0) distanceFromCatching = Settings.Active.DistanceFromCatchingOverride;
            if (Settings.Active.OverrideBarHeight >= 0) bobberBarHeight = Settings.Active.OverrideBarHeight;
            if (Settings.Active.ReelFishCycling)
            {
                if (_circularFishList == null) LoadFishList();
                whichFish = _circularFishList.ElementAt(0);
                _circularFishList.Rotate(1);
            }

            if (Settings.Active.AutoReelFish)
            {
                // Emulate BobberBar.update() when fadeOut = true
                if (EnableDebugOutput) Monitor.Log(string.Format("Auto-reeling fish with id: {0}. : {1},", whichFish, GetFishNameFromId(whichFish)));
                fadeOut = true;
                handledFishResult = true;
                distanceFromCatching = 1;
                fishTool.pullFishFromWater(whichFish, fishSize, fishQuality, (int)difficulty, treasure, perfect, fromFishPond, caughtDouble);
                Game1.exitActiveMenu();
            }

            Game1.setRichPresence("location", (object)Game1.currentLocation.Name);
        }

        private string GetFishNameFromId(int id)
        {
            if (_fishList == null) LoadFishList();

            if (_fishList.TryGetValue(id, out string name))
                return name;

            return "";
        }

        private void HandleCommand(string command, string[] args)
        {
            if (string.Equals(command, "sv_fishing_debug", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (string.Equals(args[0].Trim(), "1", StringComparison.Ordinal))
                        EnableDebugOutput = true;
                    else if (string.Equals(args[0].Trim(), "0", StringComparison.Ordinal))
                        EnableDebugOutput = false;
                }
                return;
            }

            if (string.Equals(command, "sv_fishing_enabled", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (string.Equals(args[0].Trim(), "1", StringComparison.Ordinal))
                        Settings.Local.DisableMod = false;
                    else if (string.Equals(args[0].Trim(), "0", StringComparison.Ordinal))
                        Settings.Local.DisableMod = true;
                }
                return;
            }

            if (string.Equals(command, "sv_fishing_autoreel", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (string.Equals(args[0].Trim(), "1", StringComparison.Ordinal))
                        Settings.Local.AutoReelFish = true;
                    else if (string.Equals(args[0].Trim(), "0", StringComparison.Ordinal))
                        Settings.Local.AutoReelFish = false;
                }
                return;
            }

            if (string.Equals(command, "sv_fishing_reload", StringComparison.OrdinalIgnoreCase))
            {
                Game1.playSound("jingle1");
                Settings.Local = null;
                Monitor.Log("Successfully reloaded SvFishingMod settings from disk.", LogLevel.Info);
                return;
            }

            if (string.Equals(command, "sv_fishing_search", StringComparison.OrdinalIgnoreCase))
            {
                if (_fishList == null) LoadFishList();
                int matchCount = 0;
                foreach (var fish in _fishList)
                {
                    foreach (string word in args)
                    {
                        if (fish.Value.ToLowerInvariant().Contains(word.ToLowerInvariant().Trim()))
                        {
                            Monitor.Log(string.Format("   {0}: {1}", fish.Key, fish.Value), LogLevel.Info);
                            matchCount++;
                        }
                    }
                }
                Monitor.Log(string.Format("Found a total of {0:N0} fishes.", matchCount), LogLevel.Info);
                return;
            }

            if (string.Equals(command, "sv_fishing_setfish", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (int.TryParse(args[0].Trim(), out int fishId))
                    {
                        Settings.Local.OverrideFishType = fishId;
                        Monitor.Log(string.Format("Done. The next reeled fish will be {0}: {1}.", fishId, GetFishNameFromId(fishId)), LogLevel.Info);
                    }
                    else
                        Monitor.Log("Invalid fish id specified.", LogLevel.Info);
                }
                return;
            }

            if (string.Equals(command, "sv_fishing_fishcycling", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (string.Equals(args[0].Trim(), "1", StringComparison.Ordinal))
                        Settings.Local.ReelFishCycling = true;
                    else if (string.Equals(args[0].Trim(), "0", StringComparison.Ordinal))
                        Settings.Local.ReelFishCycling = false;
                }
                return;
            }

            if (string.Equals(command, "sv_fishing_bitedelay", StringComparison.OrdinalIgnoreCase))
            {
                if (args != null && args.Length > 0)
                {
                    if (string.Equals(args[0].Trim(), "1", StringComparison.Ordinal))
                        Settings.Local.RemoveBiteDelay = false;
                    else if (string.Equals(args[0].Trim(), "0", StringComparison.Ordinal))
                        Settings.Local.RemoveBiteDelay = true;
                }
                return;
            }
        }

        private void LoadFishList()
        {
            if (_fishList != null)
                _fishList.Clear();
            else
                _fishList = new SortedList<int, string>();

            var fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            _circularFishList = new CircularBuffer<int>(fishData.Count);
            foreach (var fish in fishData)
            {
                string[] segments = fish.Value.Split('/');
                string name = segments[0] + '/' + segments[segments.Length - 1];
                _fishList.Add(fish.Key, name);

                if (FirstFishId == -1 || fish.Key < FirstFishId)
                    FirstFishId = fish.Key;
                if (LastFishId == -1 || fish.Key > LastFishId)
                    LastFishId = fish.Key;

                _circularFishList.InsertBackwards(fish.Key);
            }

            if (EnableDebugOutput) Monitor.Log(string.Format("Loaded {0} fishes from internal content database.", _fishList.Count));
        }
    }
}