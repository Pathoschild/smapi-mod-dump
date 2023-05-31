/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StarterPack.Framework.Managers;
using StarterPack.Framework.Patches.Locations;
using StarterPack.Framework.Utilities.SpecialAttacks;
using System;

namespace StarterPack
{
    public class StarterPack : Mod
    {

        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static IManifest manifest;
        internal static Multiplayer multiplayer;

        // Managers
        internal static ApiManager apiManager;
        internal static AssetManager assetManager;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and manifest
            monitor = Monitor;
            modHelper = helper;
            manifest = ModManifest;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Load managers
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(modHelper);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(ModManifest.UniqueID);

                // Apply Location patches
                new GameLocationPatch(monitor, modHelper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Hook into the game events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.Archery") && apiManager.HookIntoArchery(Helper))
            {
                // Register the native special attacks from our framework
                apiManager.GetArcheryApi().RegisterSpecialAttack(this.ModManifest, "Buff:Speed", Framework.Interfaces.WeaponType.Bow, (arguments) => "Graceful Winds", SpeedBuff.GetDescription, SpeedBuff.GetCooldown, SpeedBuff.HandleSpecialAttack);
            }
        }
    }
}
