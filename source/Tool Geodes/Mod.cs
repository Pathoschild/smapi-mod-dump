using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using ToolGeodes.Overrides;

namespace ToolGeodes
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public static Configuration Config;
        public static SaveData Data;
        private HarmonyInstance harmony;

        public const string MSG_TOOLGEODEDATA = "ToolGeodeData";

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Config = helper.ReadConfig<Configuration>() ?? new Configuration();

            helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
            helper.Events.Display.RenderedWorld += TrueSight.onDrawWorld;
            helper.Events.Input.ButtonPressed += onButtonPressed;
            helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            Helper.Events.Multiplayer.PeerContextReceived += onPeerContextReceived;
            Helper.Events.Multiplayer.ModMessageReceived += onModMessageReceived;

            try
            {
                harmony = HarmonyInstance.Create("spacechase0.ToolGeodes");
                doPrefix(typeof(Pickaxe), nameof(Pickaxe.DoFunction), typeof(PickaxeStaminaHook));
                doPrefix(typeof(Axe), nameof(Axe.DoFunction), typeof(AxeStaminaHook));
                doPrefix(typeof(WateringCan), nameof(WateringCan.DoFunction), typeof(WateringCanStaminaHook));
                doPrefix(typeof(Hoe), nameof(Hoe.DoFunction), typeof(HoeStaminaHook));
                doPrefix(typeof(Tool), "tilesAffected", typeof(ToolTilesHook));
                doPrefix(typeof(GameLocation).GetMethod( nameof(GameLocation.damageMonster), new Type[]{typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer)}), typeof(MonsterDamageHook).GetMethod(nameof(MonsterDamageHook.Prefix)));
                doPrefix(typeof(RockCrab).GetMethod(nameof(RockCrab.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(RockCrabPiercingHook).GetMethod( nameof(RockCrabPiercingHook.Prefix)));
                doPrefix(typeof(Bug).GetMethod(nameof(Bug.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(BugPiercingHook).GetMethod(nameof(BugPiercingHook.Prefix)));
                doPrefix(typeof(Mummy).GetMethod(nameof(Mummy.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(MummyPiercingHook).GetMethod(nameof(MummyPiercingHook.Prefix)));
                doPrefix(typeof(MeleeWeapon), nameof(MeleeWeapon.setFarmerAnimating), typeof(MeleeWeaponSpeedHook));
                doPostfix(typeof(MeleeWeapon), nameof(MeleeWeapon.setFarmerAnimating), typeof(MeleeWeaponSpeedHook));
                doTranspiler(typeof(Game1), nameof(Game1.pressUseToolButton), typeof(Game1ToolRangeHook));
                doPrefix(typeof(Pickaxe), nameof(Pickaxe.DoFunction), typeof(PickaxeRemoteUseHook));
                doPrefix(typeof(Axe), nameof(Axe.DoFunction), typeof(AxeRemoteUseHook));
                doPrefix(typeof(WateringCan), nameof(WateringCan.DoFunction), typeof(WateringCanRemoteUseHook));
                doPrefix(typeof(Hoe), nameof(Hoe.DoFunction), typeof(HoeRemoteUseHook));
            }
            catch ( Exception ex )
            {
                Log.error($"Exception doing harmony: {ex}");
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Game1.player.hasWateringCanEnchantment = Game1.player.HasAdornment(ToolType.WateringCan, Config.GEODE_INFINITE_WATER) > 0;

            if ( Game1.player.UsingTool )
            {
                if (Game1.player.toolPower == 1 &&
                    ( (Game1.player.CurrentTool is WateringCan &&
                       Game1.player.HasAdornment(ToolType.WateringCan, Config.GEODE_INSTANT_CHARGE) > 0) ||
                      (Game1.player.CurrentTool is Hoe &&
                       Game1.player.HasAdornment(ToolType.Hoe, Config.GEODE_INSTANT_CHARGE) > 0) ))
                    Game1.player.toolPower = Game1.player.CurrentTool.UpgradeLevel;
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.AdornKey)
            {
                if ( Game1.activeClickableMenu == null && !Game1.eventUp )
                {
                    Log.trace("Opening adornment menu (1)");
                    Game1.activeClickableMenu = new AdornMenu_PickTool();
                }
            }
        }

        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Data = Helper.Data.ReadSaveData<SaveData>($"spacechase0.ToolGeodes.{Game1.player.UniqueMultiplayerID}") ?? new SaveData();
            }
        }

        /// <summary>Raised after the mod context for a peer is received. This happens before the game approves the connection, so the player doesn't yet exist in the game. This is the earliest point where messages can be sent to the peer via SMAPI.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            Log.debug("Received peer context: " + e.Peer.PlayerID + " " + Game1.IsMasterGame);
            if (!Game1.IsServer)
                return;

            Log.debug($"Sending tool geode data to {e.Peer.PlayerID}");
            var data = Helper.Data.ReadSaveData<SaveData>($"spacechase0.ToolGeodes.{e.Peer.PlayerID}") ?? new SaveData();
            Helper.Multiplayer.SendMessage(data, MSG_TOOLGEODEDATA, playerIDs: new[] { e.Peer.PlayerID });
        }

        /// <summary>Raised after a mod message is received over the network.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID && e.Type == MSG_TOOLGEODEDATA)
            {
                Log.debug($"Got tool geode data from {e.FromPlayerID}");
                var data = e.ReadAs<SaveData>();
                if (Context.IsMainPlayer)
                {
                    Helper.Data.WriteSaveData<SaveData>($"spacechase0.ToolGeodes.{e.FromPlayerID}", data);
                }
                else
                    Data = data;
            }
        }

        private void doPrefix(Type origType, string origMethod, Type newType)
        {
            doPrefix(origType.GetMethod(origMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static), newType.GetMethod("Prefix"));
        }
        private void doPrefix(MethodInfo orig, MethodInfo prefix)
        {
            try
            {
                Log.trace($"Doing prefix patch {orig}:{prefix}...");
                harmony.Patch(orig, new HarmonyMethod(prefix), null);
            }
            catch (Exception e)
            {
                Log.error($"Exception doing prefix patch {orig}:{prefix}: {e}");
            }
        }
        private void doPostfix(Type origType, string origMethod, Type newType)
        {
            doPostfix(origType.GetMethod(origMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static), newType.GetMethod("Postfix"));
        }
        private void doPostfix(MethodInfo orig, MethodInfo postfix)
        {
            try
            {
                Log.trace($"Doing postfix patch {orig}:{postfix}...");
                harmony.Patch(orig, null, new HarmonyMethod(postfix));
            }
            catch (Exception e)
            {
                Log.error($"Exception doing postfix patch {orig}:{postfix}: {e}");
            }
        }
        private void doTranspiler(Type origType, string origMethod, Type newType)
        {
            doTranspiler(origType.GetMethod(origMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static), newType.GetMethod("Transpiler"));
        }
        private void doTranspiler(MethodInfo orig, MethodInfo transpiler)
        {
            try
            {
                Log.trace($"Doing transpiler patch {orig}:{transpiler}...");
                harmony.Patch(orig, null, null, new HarmonyMethod(transpiler));
            }
            catch (Exception e)
            {
                Log.error($"Exception doing transpiler patch {orig}:{transpiler}: {e}");
            }
        }
    }
}
