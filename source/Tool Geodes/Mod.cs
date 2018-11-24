using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Config = helper.ReadConfig<Configuration>() ?? new Configuration();

            GameEvents.UpdateTick += onUpdate;
            Helper.Events.Display.RenderedWorld += TrueSight.onDrawWorld;
            InputEvents.ButtonPressed += onButtonPressed;
            SaveEvents.AfterLoad += afterLoad;
            Helper.Events.Multiplayer.PeerContextReceived += onClientReceived;
            Helper.Events.Multiplayer.ModMessageReceived += msgReceived;

            try
            {
                harmony = HarmonyInstance.Create("spacechase0.ToolGeodes");
                doPrefix(typeof(Pickaxe), "DoFunction", typeof(PickaxeStaminaHook));
                doPrefix(typeof(Axe), "DoFunction", typeof(AxeStaminaHook));
                doPrefix(typeof(WateringCan), "DoFunction", typeof(WateringCanStaminaHook));
                doPrefix(typeof(Hoe), "DoFunction", typeof(HoeStaminaHook));
                doPrefix(typeof(Tool), "tilesAffected", typeof(ToolTilesHook));
                doPrefix(typeof(GameLocation).GetMethod("damageMonster", new Type[]{typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer)}), typeof(MonsterDamageHook).GetMethod("Prefix"));
                doPrefix(typeof(RockCrab).GetMethod("takeDamage", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(RockCrabPiercingHook).GetMethod("Prefix"));
                doPrefix(typeof(Bug).GetMethod("takeDamage", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(BugPiercingHook).GetMethod("Prefix"));
                doPrefix(typeof(Mummy).GetMethod("takeDamage", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }), typeof(MummyPiercingHook).GetMethod("Prefix"));
                doPrefix(typeof(MeleeWeapon), "setFarmerAnimating", typeof(MeleeWeaponSpeedHook));
                doPostfix(typeof(MeleeWeapon), "setFarmerAnimating", typeof(MeleeWeaponSpeedHook));
                doTranspiler(typeof(Game1), "pressUseToolButton", typeof(Game1ToolRangeHook));
                doPrefix(typeof(Pickaxe), "DoFunction", typeof(PickaxeRemoteUseHook));
                doPrefix(typeof(Axe), "DoFunction", typeof(AxeRemoteUseHook));
                doPrefix(typeof(WateringCan), "DoFunction", typeof(WateringCanRemoteUseHook));
                doPrefix(typeof(Hoe), "DoFunction", typeof(HoeRemoteUseHook));
            }
            catch ( Exception e )
            {
                Log.error("Exception doing harmony: " + e);
            }
        }

        private void onUpdate(object sender, EventArgs args)
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

        private void onButtonPressed(object sender, EventArgsInput args)
        {
            if (args.Button == Config.AdornKey)
            {
                if ( Game1.activeClickableMenu == null && !Game1.eventUp )
                {
                    Log.trace("Opening adornment menu (1)");
                    Game1.activeClickableMenu = new AdornMenu_PickTool();
                }
            }
        }

        private void afterLoad(object sender, EventArgs args)
        {
            if (Game1.IsMasterGame)
            {
                Data = Helper.Data.ReadSaveData<SaveData>("spacechase0.ToolGeodes." + Game1.player.UniqueMultiplayerID) ?? new SaveData();
            }

        }

        private void onClientReceived(object sender, PeerContextReceivedEventArgs args)
        {
            Log.debug("Sending tool geode data to " + args.Peer.PlayerID);
            var data = Helper.Data.ReadSaveData<SaveData>("spacechase0.ToolGeodes." + args.Peer.PlayerID) ?? new SaveData();
            Helper.Multiplayer.SendMessage(data, MSG_TOOLGEODEDATA);
        }

        private void msgReceived(object sender, ModMessageReceivedEventArgs args)
        {
            if (args.FromModID == ModManifest.UniqueID && args.Type == MSG_TOOLGEODEDATA)
            {
                Log.debug("Got tool geode data from " + args.FromPlayerID);
                var data = args.ReadAs<SaveData>();
                if (Game1.IsMasterGame)
                {
                    Helper.Data.WriteSaveData<SaveData>("spacechase0.ToolGeodes." + args.FromPlayerID, data);
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
