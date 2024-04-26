/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ProducerFrameworkMod.Api;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class ProducerFrameworkModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicking += OnGameLoopOnUpdateTicking ;
            helper.Events.GameLoop.SaveLoaded += DataLoader.LoadContentPacks;
        }

        private void OnGameLoopOnUpdateTicking(object sender, UpdateTickingEventArgs args)
        {
            if (args.Ticks == 120)
            {
                DataLoader.LoadContentPacks(sender, args);
                Helper.Events.GameLoop.UpdateTicking -= OnGameLoopOnUpdateTicking;
            }
        }

        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, EventArgs e)
        {
            new DataLoader(Helper);

            var harmony = new Harmony("Digus.ProducerFrameworkMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "loadDisplayName"),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.LoadDisplayName))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.checkForActionPrefix)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.checkForActionPostfix))
            ); 
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.minutesElapsed)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.minutesElapsedPrefix)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.minutesElapsedPostfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performDropDownAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.performDropDownAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.DayUpdate)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.DayUpdate))
            ); 
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.draw), new Type[]{typeof(SpriteBatch),typeof(int), typeof(int), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.draw_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.initializeLightSource)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.initializeLightSource))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.TryApplyFairyDust)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.TryApplyFairyDust))
            );
        }

        public override object GetApi()
        {
            return new ProducerFrameworkModApi();
        }
    }
}
