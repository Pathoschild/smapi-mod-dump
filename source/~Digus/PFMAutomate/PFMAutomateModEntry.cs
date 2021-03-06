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
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using PFMAutomate.Automate;
using ProducerFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace PFMAutomate
{
    public class PFMAutomateModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IAutomateAPI automate = Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            automate.AddFactory(new ProducerFrameworkAutomationFactory());

            var harmony = HarmonyInstance.Create("Digus.PFMAutomate");

            Assembly automateAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith("Automate,"));
            MethodInfo automateMethodInfo = AccessTools.GetDeclaredMethods(automateAssembly.GetType("Pathoschild.Stardew.Automate.Framework.AutomationFactory")).Find(m=> m.GetParameters().Any(p=>p.ParameterType == typeof(SObject)));
            harmony.Patch(
                original: automateMethodInfo,
                postfix: new HarmonyMethod(typeof(AutomateOverrides), nameof(AutomateOverrides.GetFor))
            );

            Assembly ccrmAutomateAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("CCRMAutomate,"));
            if (ccrmAutomateAssembly != null)
            {
                MethodInfo ccrmAutomateMethodInfo = AccessTools.GetDeclaredMethods(ccrmAutomateAssembly.GetType("CCRMAutomate.Automate.CustomCrystalariumAutomationFactory")).Find(m => m.GetParameters().Any(p => p.ParameterType == typeof(SObject)));
                harmony.Patch(
                    original: ccrmAutomateMethodInfo,
                    postfix: new HarmonyMethod(typeof(CCRMAutomateOverrides), nameof(CCRMAutomateOverrides.GetFor))
                );
            }
            
        }
    }
}
