/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework.Graphics;
using GenericModConfigMenu;
using StardewValley.Objects;

namespace JojaExpress
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig? config;
        public static List<Dictionary<string, int>> tobeReceived = new() { new()};
        public static Dictionary<long, List<Dictionary<string, int>>> globalReceived = new();
        public static Dictionary<string, int> localReceived = new();
        public static bool needMail = false;
        public static int fee_state = 0;
        public static Translation postfix;
        public static ModEntry Instance;
        public static IModHelper _Helper;
        public static IMonitor _Monitor;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += PlayerInteractionHandler.OnButtonPressed;
            helper.Events.Content.AssetReady += LoadingManager.fillShops;
            helper.Events.Content.AssetRequested += LoadingManager.loadAsset;
            helper.Events.GameLoop.SaveLoaded += load;
            helper.Events.GameLoop.DayStarted += initNewDay;
            helper.Events.GameLoop.DayEnding += PlayerInteractionHandler.sendMail;
            helper.Events.Content.LocaleChanged += (a, b) => { postfix = Helper.Translation.Get("postfix"); };
            helper.Events.GameLoop.Saving += save;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Player.InventoryChanged += PlayerInteractionHandler.checkInv;
            helper.Events.Multiplayer.ModMessageReceived += receiveMultiplayerMessage;
            helper.Events.Display.MenuChanged += GUI.checkUI;
            helper.Events.Display.RenderedWorld += GUI.drawBird;
            config = this.Helper.ReadConfig<ModConfig>();
            Instance = this;
            postfix = Helper.Translation.Get("postfix");
            GUI.birdTexture = Helper.GameContent.Load<Texture2D>("LooseSprites\\parrots");
            _Helper = helper;
            _Monitor = Monitor;
            Phone.PhoneHandlers.Add(new JojaPhoneHandler());
        }

        public override object? GetApi()
        {
            return new JojaExpressAPI();
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => config.Open,
                setValue: value => config.Open = value,
                name: () => Helper.Translation.Get("open_name"),
                tooltip: () => Helper.Translation.Get("open_tip")
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.CarriageFee,
                setValue: value => config.CarriageFee = value,
                name: () => Helper.Translation.Get("fee_name"),
                tooltip: () => Helper.Translation.Get("fee_tip"), 
                min: 0,
                formatValue: (value) => (value - 1).ToString("P1")
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.CarriageFee_NoJoja,
                setValue: value => config.CarriageFee_NoJoja = value,
                name: () => Helper.Translation.Get("fee_NoJoja_name"),
                tooltip: () => Helper.Translation.Get("fee_NoJoja_tip"),
                min: 0,
                formatValue: (value) => (value - 1).ToString("P1")
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.CarriageFee_Member,
                setValue: value => config.CarriageFee_Member = value,
                name: () => Helper.Translation.Get("fee_Member_name"),
                tooltip: () => Helper.Translation.Get("fee_Member_tip"),
                min: 0,
                formatValue: (value) => (value - 1).ToString("P1")
            );

            configMenu.AddBoolOption(
                mod:ModManifest,
                getValue: () => config.OpenByKey,
                setValue: value => config.OpenByKey = value,
                name: () => Helper.Translation.Get("openByKey"),
                tooltip: () => Helper.Translation.Get("openByKey_tooltip")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.OpenByPad,
                setValue: value => config.OpenByPad = value,
                name: () => Helper.Translation.Get("openByPad"),
                tooltip: () => Helper.Translation.Get("openByPad_tooltip")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.OpenByPhone,
                setValue: value => config.OpenByPhone = value,
                name: () => Helper.Translation.Get("openByPhone"),
                tooltip: () => Helper.Translation.Get("openByPhone_tooltip")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.CloseWhenCCComplete,
                setValue: value => config.CloseWhenCCComplete = value,
                name: () => Helper.Translation.Get("CloseWhenCCComplete"),
                tooltip: () => Helper.Translation.Get("CloseWhenCCComplete")
            );
        }

        public void receiveMultiplayerMessage(object? sender, ModMessageReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                if(e.Type == "ofts.jojaExp.tobeReceivedDispatch")
                {
                    var value = e.ReadAs<KeyValuePair<long, List<Dictionary<string, int>>>>();
                    if(value.Key == Game1.player.UniqueMultiplayerID)
                    {
                        tobeReceived = value.Value;
                    }
                }
                return;
            }

            if (e.Type == "ofts.jojaExp.tobeReceivedPoped")
            {
                if(globalReceived.TryGetValue(e.FromPlayerID, out var value))
                {
                    value.RemoveAt(0);
                }
                return;
            }

            if (e.Type == "ofts.jojaExp.tobeReceivedAdded")
            {
                var kvpair = e.ReadAs<KeyValuePair<string, int>>();
                if (globalReceived.TryGetValue(e.FromPlayerID, out var value))
                {
                    if (value.Last().ContainsKey(kvpair.Key)) value.Last()[kvpair.Key] += kvpair.Value;
                    else value.Last().Add(kvpair.Key, kvpair.Value);
                }
                return;
            }
        }

        public void checkFeeState()
        {
            int old_state = fee_state;
            if (Game1.player.hasOrWillReceiveMail("JojaMember")) fee_state = 0;
            else if (GameStateQuery.CheckConditions("IS_COMMUNITY_CENTER_COMPLETE")) fee_state = 1;
            else fee_state = 2;
            if (old_state != fee_state) Helper.GameContent.InvalidateCache("Data/shops");
        }

        public static float getPriceModifier()
        {
            if (config == null) return 1;
            switch (fee_state)
            {
                case 0: return config.CarriageFee_Member;
                case 1: return config.CarriageFee_NoJoja; 
                case 2: return config.CarriageFee;
                default: return 1;
            }
        }

        public void load(object? sender,EventArgs e)
        {
            checkFeeState();

            if(!Context.IsMainPlayer)
            {
                tobeReceived = new() { new() };
                return;
            }

            if (Context.IsSplitScreen && Context.ScreenId != 0) return;

            tobeReceived = Helper.Data.ReadSaveData<List<Dictionary<string, int>>>("jojaExp.tobeReceived");
            if (tobeReceived == null) tobeReceived = new() { new()};

            globalReceived = Helper.Data.ReadSaveData<Dictionary<long, List<Dictionary<string, int>>>>("jojaExp.globalReceived");
            if (globalReceived == null) globalReceived = new();

            foreach(var player in globalReceived)
            {
                Helper.Multiplayer.SendMessage(player, "ofts.jojaExp.tobeReceivedDispatch");
            }

            needMail = false;
            GUI.showAnimation.Value = false;
        }

        

        public void save(object? sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer || Context.IsSplitScreen && Context.ScreenId != 0) return;
            Helper.Data.WriteSaveData("jojaExp.tobeReceived", tobeReceived);
            Helper.Data.WriteSaveData("jojaExp.globalReceived", globalReceived);
        }

        public void initNewDay(object? sender, EventArgs e)
        {
            checkFeeState();
            GUI.showAnimation.Value = false;
            if (Context.IsSplitScreen && Context.ScreenId != 0) return;
            if (tobeReceived == null) tobeReceived = new() { new()};
            if (tobeReceived.Count == 0 || tobeReceived.Last().Count != 0) tobeReceived.Add(new());
            if (Context.IsMainPlayer)
            {
                foreach(var player in globalReceived)
                {
                    if(player.Value.Count == 0 || player.Value.Last().Count != 0) player.Value.Add(new());
                }
            }
        }
    }

    public class ModConfig
    {
        public KeybindList Open { get; set; } = KeybindList.Parse("J");
        public float CarriageFee { get; set; } = 1.3f;
        public float CarriageFee_NoJoja { get; set; } = 1.5f;
        public float CarriageFee_Member { get; set; } = 1.1f;
        public bool OpenByPhone { get; set; } = true;
        public bool OpenByKey { get; set; } = false;
        public bool OpenByPad { get; set; } = true;
        public bool CloseWhenCCComplete { get; set; } = true;
    }
}