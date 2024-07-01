/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Avas-Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using GenericModConfigMenu;

namespace FriendshipStreaks
{
    /// <summary>The mod entry point.</summary>
    public sealed class ModEntry : Mod
    {
        public static Mod instance;

        public static Texture2D gameCursors;

        public static Dictionary<string, FriendshipStreak> streaks = new Dictionary<string, FriendshipStreak>();

        public static NetworkDataManager networkDataManager;

        public static ModConfig Config;

        public bool skipResettingStreaks = true; //Only for farmhands
        public override void Entry(IModHelper helper)
        {
            I18n.Init(Helper.Translation);
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Player.Warped += OnWarped;

            Config = Helper.ReadConfig<ModConfig>();

            Helper.ConsoleCommands.Add("set_streak", "Sets the streak for an NPC to the given value.\nUsage: set_streak <type> <NPC name> <value>\n- type - must be either 'gift' or 'talking'\n- NPC name - the name of your target\n- value - amount of the desired streak.", this.SetStreak);

            instance = this;
            gameCursors = Helper.GameContent.Load<Texture2D>("LooseSprites/Cursors");
            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawNPCSlot)),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(Patches.Transpiler))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_receiveGift))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(ProfileMenu), nameof(ProfileMenu.draw), new Type[] { typeof(SpriteBatch)}),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawProfileMenu))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.grantConversationFriendship)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_grantConversationFriendship))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_changeFriendship))
                );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Config_EnableBonus(),
                tooltip: () => I18n.Config_EnableBonusDescription(),
                getValue: () => Config.enableBonus,
                setValue: value => Config.enableBonus = value
            );
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (skipResettingStreaks && !Context.IsMainPlayer)
            {
                skipResettingStreaks = false;
                return;
            }

            foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
            {
                kvp.Value.ResetStreaksIfMissed();
            }
            if (!Context.IsMainPlayer)
            {
                networkDataManager.SendDataToHost();
            }
        }
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                networkDataManager.SendDataToHost();
                Monitor.Log("Sent data to host");
            }
            else
            {
                Helper.Data.WriteSaveData<NetworkDataManager>("NetworkDataManager", networkDataManager);
                foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
                {
                    Helper.Data.WriteSaveData(kvp.Key, kvp.Value);
                    Monitor.Log($"Saved Streak data for {kvp.Key}");
                }
            }
        }
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            foreach (NPC npc in Utility.getAllVillagers())
            {
                if (!streaks.ContainsKey(npc.Name) && npc.CanSocialize)
                {
                    Monitor.Log($"New villager {npc.Name}found. Initialising streak for them...");
                    FriendshipStreak streak = new FriendshipStreak(npc.Name, 0, 0, 0, 0);
                    streaks.Add(npc.Name, streak);
                }
            }
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (networkDataManager == null)
                networkDataManager = new NetworkDataManager();

            networkDataManager.OnMessageReceived(sender, e);
        }
        private void InitialiseStreaks(bool readFromSave = false)
        {
            List<string> npcNames = new List<string>();
            foreach (NPC npc in Utility.getAllVillagers())
            {
                if (!npcNames.Contains(npc.Name))
                {
                    if (!npc.CanSocialize)
                        continue;

                    npcNames.Add(npc.Name);
                    if (readFromSave)
                    {
                        FriendshipStreak streak = Helper.Data.ReadSaveData<FriendshipStreak>(npc.Name);
                        if (streak == null)
                        {
                            Monitor.Log($"No streak found for {npc.Name}. Initialising new one...");
                            streak = new FriendshipStreak(npc.Name, 0, 0, 0, 0);
                        }
                        streaks.Add(npc.Name, streak);
                    }
                    else
                    {
                        FriendshipStreak streak = new FriendshipStreak(npc.Name, 0, 0, 0, 0);
                        streaks.Add(npc.Name, streak);
                    }
                }
            }
        }
        private void SetStreak(string command, string[] args)
        {

            if (args.Length < 3)
                return;

            string type = args[0].ToLower();
            string npcName = args[1];
            int value = int.Parse(args[2]);
            
            switch(type)
            {
                case "gift":
                    streaks[npcName].CurrentGiftStreak = value;
                    Monitor.Log("Success", LogLevel.Info);
                    break;

                case "talking":
                    streaks[npcName].CurrentTalkingStreak = value;
                    Monitor.Log("Success", LogLevel.Info);
                    break;

                default:
                    Monitor.Log("First argument must be 'gift' or 'talking'", LogLevel.Error);
                    break;
            }
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            networkDataManager.SendDataToFarmhand(e.Peer.PlayerID);
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            streaks.Clear();
            skipResettingStreaks = true; //Only relevant for farmhands in multiplayer
            if (networkDataManager == null)
                networkDataManager = new();
            if (Context.IsMainPlayer && Context.IsMultiplayer)
            {
                NetworkDataManager _manager = Helper.Data.ReadSaveData<NetworkDataManager>("NetworkDataManager");
                if (_manager != null)
                    networkDataManager = _manager;

                InitialiseStreaks(true);
                return;
            }

            else if (Context.IsMultiplayer)
            {
                networkDataManager.RequestData();
                if (streaks.Count == 0)
                    InitialiseStreaks();
            }

            else
            {
                InitialiseStreaks(true);
            }
        }
    }
}