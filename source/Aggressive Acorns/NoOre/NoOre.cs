using Harmony;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace NoOre
{
    [UsedImplicitly]
    public class NoOre : Mod
    {
        private static IModConfig _config;
        private static IMonitor _monitor;

        private bool _handleNewLocations;

        private bool DoHandleNewLocations
        {
            get => _handleNewLocations;
            set
            {
                if (value == _handleNewLocations) return;
                if (value)
                {
                    Helper.Events.World.LocationListChanged += WorldOnLocationListChanged;
                }
                else
                {
                    Helper.Events.World.LocationListChanged -= WorldOnLocationListChanged;
                }

                _handleNewLocations = value;
            }
        }


        public override void Entry([NotNull] IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            var target = typeof(GameLocation).GetMethod(nameof(GameLocation.breakStone));

            // Monitor.Log(target != null ? $"got method info for {target.DeclaringType}::{target.Name}" : "couldn't reflect method",LogLevel.Trace);
            var postfix = new HarmonyMethod(GetType(), nameof(Postfix));
            harmony.Patch(target, null, postfix);

            helper.Events.GameLoop.SaveLoaded += (sender, args) => DoHandleNewLocations = Context.IsMainPlayer;
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => DoHandleNewLocations = false;
        }


        private void WorldOnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }


        private static void Postfix(
            GameLocation __instance,
            bool __result,
            int indexOfStone,
            int x,
            int y,
            Farmer who)
        {
            // _monitor.Log(
            //     $"breakStone called in {__instance.Name} at ({x},{y}) by {who.Name} on {indexOfStone}",
            //     LogLevel.Trace);
            if (__result) return; // if the original method returned true, it wasn't normal stone

            if (_config.ReplaceOres)
            {
                // chance to drop ores
                /*
                 * Base chance options: flat chance, native range, by deepest level, by skill level
                 * Copper
                 * Iron
                 * Gold
                 * Iridium
                 */
            }

            if (_config.ReplaceGemNodes)
            {
                // chance to drop gems
            }

            if (_config.ReplaceMysticStone)
            {
                // chance to drop mystic stone drops
            }

            if (_config.ReplaceGeodeNodes)
            {
                // chance to drop geodes
            }
        }
    }


    internal static class Constants
    {
        public const int CopperNode = 378;
        public const int IronNode = 380;
        public const int CoalNode = 382;
        public const int GoldNode = 384;
        public const int IridiumNode = 386;
    }
}
