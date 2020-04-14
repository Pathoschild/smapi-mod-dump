using BetterQualityMoreSeeds.Framework;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using SObject = StardewValley.Object;

namespace BetterQualityMoreSeeds
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool hasAutomate;
        private HarmonyInstance harmony;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Harmony instance for patching main game code
            harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            helper.Events.GameLoop.GameLaunched += PatchSeedMaker;
            helper.Events.GameLoop.GameLaunched += CheckAutomate;
        }

        private void PatchSeedMaker(object sender, GameLaunchedEventArgs e)
        {
            TryToCheckAtPatch.Initialize(this.Monitor);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.tryToCheckAt)),
                prefix: new HarmonyMethod(typeof(TryToCheckAtPatch), nameof(TryToCheckAtPatch.TryToCheckAt_PreFix)),
                postfix: new HarmonyMethod(typeof(TryToCheckAtPatch), nameof(TryToCheckAtPatch.TryToCheckAt_PostFix))
                );

        }

        private void CheckAutomate(object sender, GameLaunchedEventArgs e)
        {
            hasAutomate = this.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
            if (hasAutomate)
            {
                Monitor.Log("Getting Assembly", LogLevel.Debug);
                object api = this.Helper.ModRegistry.GetApi("Pathoschild.Automate");
                Assembly assembly = api.GetType().Assembly;
                Monitor.Log("Getting a Seedmaker Type", LogLevel.Debug);
                Type SeedMakerMachine = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine");
                SeedMakerMachinePatch.Initialize(this.Monitor, SeedMakerMachine, this.Helper.Reflection);

                harmony.Patch(
                    original: AccessTools.Method(SeedMakerMachine, SeedMakerMachine.GetMethod("SetInput", BindingFlags.Public | BindingFlags.Instance ).Name),
                    prefix: new HarmonyMethod(typeof(SeedMakerMachinePatch), nameof(SeedMakerMachinePatch.SetInputPrefix)),
                    postfix: new HarmonyMethod(typeof(SeedMakerMachinePatch), nameof(SeedMakerMachinePatch.SetInputPostfix))
                    );
            }
        }


        /*********
        ** Private methods
        *********/


    }
}
