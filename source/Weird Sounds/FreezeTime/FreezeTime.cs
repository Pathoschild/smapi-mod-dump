/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/SVMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace FreezeTime;

public partial class FreezeTime: Mod
{
    private const string BroadcastStatusMessageType = "_FREEZE";
    private const string AskStatusMessageType = "_ASK_FREEZE";
    private const string BroadcastConfigMessageType = "_FREEZE_CONFIG";
    private static Texture2D? _frame, _blackBlock, _frozenBlock,_unfrozenBlock,_unloadedButFrozenBlock,_unloadedButUnfrozenBlock;

    private FreezeTimeChecker _checker = null!;
    private static bool _lastFreezeStatus;
    private static bool _forcePassTime;
    private static readonly Vector2 FramePosition = new(44, 240);
    public override void Entry(IModHelper helper)
    {
        _frame = helper.ModContent.Load<Texture2D>("assets/Frame.png");
        _blackBlock = helper.ModContent.Load<Texture2D>("assets/BlackBlock.png");
        _frozenBlock = helper.ModContent.Load<Texture2D>("assets/FrozenBlock.png");
        _unfrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnfrozenBlock.png");        
        _unloadedButFrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnloadedButFrozenBlock.png");
        _unloadedButUnfrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnloadedButUnfrozenBlock.png");
        Helper.Events.Display.RenderingHud += PreRenderHudEvent;
        Helper.Events.GameLoop.UpdateTicked += GameTickEvent;
        Helper.Events.Multiplayer.PeerConnected += PlayerConnectedEvent;
        Helper.Events.Multiplayer.PeerDisconnected += PlayerDisconnectedEvent;
        Helper.Events.Multiplayer.ModMessageReceived += ModMessageReceivedEvent;
        Helper.Events.Input.ButtonReleased += ButtonReleasedEvent;
        Helper.Events.GameLoop.SaveLoaded += SaveLoadedEvent;
        Helper.Events.GameLoop.GameLaunched += GameLaunchedEvent;
        
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), nameof(Game1.shouldTimePass)),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.ShouldTimePass))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), "Update"),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.ForceNetTimePause))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PerformSleepCheck))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PerformSleepChecked))
        );
    }

    private class Game1Patcher
    {
        public static bool ShouldTimePass(ref bool __result)
        {
            if (!Context.IsMultiplayer) {
                return true;
            }
            if (_lastFreezeStatus && _forcePassTime) {
                __result = true;
                return false;
            }
            if (!Context.IsMainPlayer) {
                return true;
            }
            __result = !_lastFreezeStatus;
            return false;
        }
        public static void ForceNetTimePause()
        {
            if (Context.IsMainPlayer && Context.IsMultiplayer) {
                Game1.netWorldState.Value.IsTimePaused = _lastFreezeStatus;
            }
        }
        public static void PerformSleepCheck()
        {
            _forcePassTime = true;
        }        
        public static void PerformSleepChecked()
        {
            _forcePassTime = false;
        }
    }

    private void SaveLoadedEvent(object? sender, SaveLoadedEventArgs e)
    {
        _checker = new FreezeTimeChecker(_config);
        if (Context.IsMainPlayer) {
            _lastFreezeStatus = false;
            if (_checker.HasPlayer(Game1.player)) {
                return;
            }
            _checker.AddPlayer(Game1.player);
            _checker.SetPlayerLoaded(Game1.player, true);
        } else {
            AskFreezeTimeStatus();
        }
    }
    
    private void ModMessageReceivedEvent(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModManifest.UniqueID) {
            return;
        }
        if (Context.IsMainPlayer) {
            if (e.Type != AskStatusMessageType) {
                return;
            }
            BroadcastStatus();
            BroadcastConfig();
        } else {
            switch (e.Type) {
                case BroadcastStatusMessageType: 
                    var status = e.ReadAs<Dictionary<long, Dictionary<string, bool>>>();
                    _checker.LoadFromStatus(status);
                    break;
                case BroadcastConfigMessageType:
                    _config = e.ReadAs<ModConfig>();
                    break;
            }
            _lastFreezeStatus = _checker.IsFrozen();
        }
    }

    private void AskFreezeTimeStatus()
    {
        Helper.Multiplayer.SendMessage(true, AskStatusMessageType);
    }
    
    private void PreRenderHudEvent(object? sender, EventArgs e)
    {
        if (Game1.displayHUD && !Game1.freezeControls) {
            DrawStatusBar();
        }
    }
    
    private void ButtonReleasedEvent(object? sender, ButtonReleasedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft) {
            return;
        }

        if (!new Rectangle((int)((Game1.dayTimeMoneyBox.position.X + FramePosition.X + 24) * Game1.options.uiScale),(int)((Game1.dayTimeMoneyBox.position.Y + FramePosition.Y + 24) * Game1.options.uiScale),(int)(108 * Game1.options.uiScale), (int)(24 * Game1.options.uiScale)).Contains(Game1.getMouseX(), Game1.getMouseY())) { 
            return;
        }
        Game1.chatBox.addMessage(_checker.GetFreezeTimeMessage(), Color.Aqua);
        Helper.Input.Suppress(SButton.MouseLeft);
    }
    
    private static bool PlayerFrozen(Farmer player)
    {
        return player is { CanMove: false, UsingTool: false } || player.hasMenuOpen.Value || player.Sprite.currentFrame == 84;
    }
    
    private void DrawStatusBar()
    {
        const int totalBlockWidth = 108;
        const int separationLineWidth = 4;
        Vector2 offset = new(24, 24);
        var blockWidth = (totalBlockWidth - (Game1.getOnlineFarmers().Count - 1) * separationLineWidth) / Game1.getOnlineFarmers().Count;
        var counter = 0;
        Game1.spriteBatch.Draw(_frame, Game1.dayTimeMoneyBox.position + FramePosition , null, Color.White, 0.0f, Vector2.Zero, 4, SpriteEffects.None, 0.99f);
        foreach (var status in _checker.GetCollection()) {
            Texture2D? bar;
            if (status.Value.Loaded) {
                bar = status.Value.Frozen ? _frozenBlock : _unfrozenBlock;
            } else {
                bar = status.Value.Frozen ? _unloadedButFrozenBlock : _unloadedButUnfrozenBlock;
            }
            Game1.spriteBatch.Draw(bar, Game1.dayTimeMoneyBox.position + FramePosition + offset +  new Vector2(counter * (blockWidth + separationLineWidth), 0), new Rectangle(0, 0, blockWidth, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            if (counter != 0) {
                Game1.spriteBatch.Draw(_blackBlock, Game1.dayTimeMoneyBox.position + FramePosition + offset + new Vector2(counter * (blockWidth + separationLineWidth) - separationLineWidth, 0), new Rectangle(0, 0, separationLineWidth, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            }
            counter++;
        }
    }
    
    //main player
    private void PlayerConnectedEvent(object? sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) {
            return;
        }
        foreach (var farmer in Game1.getOnlineFarmers().Where(farmer => farmer.UniqueMultiplayerID == e.Peer.PlayerID)) {
            _checker.AddPlayer(farmer);
            string message;
            Color color;
            if (e.Peer.GetMod(ModManifest.UniqueID) == null) { 
                message = farmer.Name + " doesn't have " + ModManifest.Name + " mod.";
                color = Color.Red;
            } else {
                _checker.SetPlayerLoaded(farmer, true);
                message = farmer.Name + " has " + ModManifest.Name + " mod.";
                color = Color.Blue;
            }
            if (Context.IsMainPlayer) {
                Game1.chatBox.addMessage(message, color);
            }
        }
    }

    private void PlayerDisconnectedEvent(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) {
            return;
        }
        if (_checker.HasPlayer(e.Peer.PlayerID)) {
            _checker.DelPlayer(e.Peer.PlayerID);
        }
    }
    
    private void GameTickEvent(object? sender, EventArgs e)
    {
        if (!Context.IsWorldReady) {
            return;
        }
        if(Context.IsMainPlayer) {
            UpdateChecker();
            ApplyFreezing();
            ApplyUnFreezing();
        }
        ApplyMonsterFreezing();
    }
    
    private void UpdateChecker()
    {
        foreach (var farmer in Game1.getOnlineFarmers()) {
            if (!_checker.HasPlayer(farmer)) {
                return;
            }
            if (PlayerFrozen(farmer)) {
                if (_checker.GetPlayer(farmer).Frozen) {
                    continue;
                }
                _checker.SetPlayerFrozen(farmer, true);
                BroadcastStatus();
            } else {
                if (!_checker.GetPlayer(farmer).Frozen) {
                    continue;
                }
                _checker.SetPlayerFrozen(farmer, false);
                BroadcastStatus();
            }
        }
    }

    private void ApplyFreezing()
    {
        if (!_checker.IsFrozen()) {
            return;
        }
        if (!_lastFreezeStatus) {
             _lastFreezeStatus = true;
        }
    }

    private static void ApplyMonsterFreezing()
    {
        if (!Game1.netWorldState.Value.IsTimePaused) {
            return;
        }
        foreach (var character in Game1.player.currentLocation.characters) {
            if (character is not Monster monster) {
                continue;
            }
            if (monster.invincibleCountdown > 0) {
                monster.invincibleCountdown = 0;
            }
        }
    }
    
    private void ApplyUnFreezing()
    {
        if (!_lastFreezeStatus || _checker.IsFrozen()) {
            return;
        }
        _lastFreezeStatus = false;
    }
    
    private void BroadcastStatus()
    {   
        Helper.Multiplayer.SendMessage(_checker.FreezeTimeStatus(), BroadcastStatusMessageType, modIDs: [ModManifest.UniqueID]);
    }
    
    private void BroadcastConfig()
    {
        Helper.Multiplayer.SendMessage(_config, BroadcastConfigMessageType, modIDs: [ModManifest.UniqueID]);
    }
}