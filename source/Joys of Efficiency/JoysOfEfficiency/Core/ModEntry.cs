using System;
using JoysOfEfficiency.EventHandler;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.Menus;
using JoysOfEfficiency.ModCheckers;
using JoysOfEfficiency.Patches;
using JoysOfEfficiency.Utils;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;

namespace JoysOfEfficiency.Core
{
    using Player = Farmer;


    /// <summary>
    /// This class is a representation of the mod itself.
    /// </summary>
    internal class ModEntry : Mod
    {
        public static bool IsCoGOn { get; private set; }
        public static bool IsCcOn { get; private set; }
        public static bool IsCaOn { get; private set; }

        private static Config Conf => InstanceHolder.Config;

        public static bool HarmonyPatched { get; private set; }

        public static bool DebugMode { get; private set; }

        /// <summary>
        /// Called firstly when SMAPI finished loading of the mod.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            // Loads configuration from file.
            Config conf = Helper.ReadConfig<Config>();

            // Initialize InstanceHolder.
            InstanceHolder.Init(this, conf);

            // Register events.
            EventHolder.RegisterEvents(Helper.Events);
            

            // Registration commands.
            Helper.ConsoleCommands.Add("joedebug", "Debug command for JoE", OnDebugCommand);

            // Limit config values.
            Conf.CpuThresholdFishing = Util.Cap(Conf.CpuThresholdFishing, 0, 0.5f);
            Conf.HealthToEatRatio = Util.Cap(Conf.HealthToEatRatio, 0.1f, 0.8f);
            Conf.StaminaToEatRatio = Util.Cap(Conf.StaminaToEatRatio, 0.1f, 0.8f);
            Conf.AutoCollectRadius = (int)Util.Cap(Conf.AutoCollectRadius, 1, 3);
            Conf.AutoHarvestRadius = (int)Util.Cap(Conf.AutoHarvestRadius, 1, 3);
            Conf.AutoPetRadius = (int)Util.Cap(Conf.AutoPetRadius, 1, 3);
            Conf.AutoWaterRadius = (int)Util.Cap(Conf.AutoWaterRadius, 1, 3);
            Conf.AutoDigRadius = (int)Util.Cap(Conf.AutoDigRadius, 1, 3);
            Conf.AutoShakeRadius = (int)Util.Cap(Conf.AutoShakeRadius, 1, 3);
            Conf.MachineRadius = (int)Util.Cap(Conf.MachineRadius, 1, 3);
            Conf.RadiusCraftingFromChests = (int) Util.Cap(Conf.RadiusCraftingFromChests, 1, 5);
            Conf.IdleTimeout = (int) Util.Cap(Conf.IdleTimeout, 1, 300);
            Conf.ScavengingRadius = (int) Util.Cap(Conf.ScavengingRadius, 1, 3);
            Conf.AnimalHarvestRadius = (int) Util.Cap(Conf.AnimalHarvestRadius, 1, 3);
            Conf.TrialOfExamine = (int) Util.Cap(Conf.TrialOfExamine, 1, 10);

            // Check mod compatibilities.
            if(ModChecker.IsCoGLoaded(helper))
            {
                Monitor.Log("CasksOnGround detected.");
                IsCoGOn = true;
            }

            if (ModChecker.IsCaLoaded(helper))
            {
                Monitor.Log("CasksAnywhere detected.");
                IsCaOn = true;
            }

            if (ModChecker.IsCcLoaded(helper))
            {
                Monitor.Log("Convenient Chests detected. JoE's CraftingFromChests feature will be disabled and won't patch the game.");
                Conf.CraftingFromChests = false;
                IsCcOn = true;
            }
            else if(!Conf.SafeMode)
            {
                Monitor.Log("Start patching using Harmony...");
                HarmonyPatched = HarmonyPatcher.Init();
            }
            else
            {
                Monitor.Log("SafeMode enabled, and won't patch the game.");
            }
            helper.WriteConfig(Conf);
            MineIcons.Init(helper);
        }

        private static void OnDebugCommand(string name, string[] args)
        {
            DebugMode = !DebugMode;
            Game1.activeClickableMenu = new RegisterFlowerMenu(800, 640, Color.White, 376);
        }
    }
}
