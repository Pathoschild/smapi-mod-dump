using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PEMAutomate
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;
        public static Pathoschild.Stardew.Automate.IAutomateAPI automate;
        public static ModEntry instance;

        /*********
        ** Public methods
        *********/
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            instance = this;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create("GZhynko.PufferEggsToMayonnaise.Automate");

            if (Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                harmony.Patch((MethodBase)AccessTools.GetDeclaredMethods(((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()).First<Assembly>((Func<Assembly, bool>)(a => a.FullName.StartsWith("Automate,"))).GetType("Pathoschild.Stardew.Automate.Framework.AutomationFactory")).Find((Predicate<MethodInfo>)(m => ((IEnumerable<ParameterInfo>)m.GetParameters()).Any<ParameterInfo>((Func<ParameterInfo, bool>)(p => p.ParameterType == typeof(StardewValley.Object))))), (HarmonyMethod)null, new HarmonyMethod(typeof(AutomatePatchOverrides), "GetFor", (System.Type[])null), (HarmonyMethod)null);

                automate = this.Helper.ModRegistry.GetApi<Pathoschild.Stardew.Automate.IAutomateAPI>("Pathoschild.Automate");
                automate.AddFactory(new MayonnaiseAutomationFactory());

                Monitor.Log("This mod patches Automate. If you encounter any issues using Automate with this mod, try removing it first. If this does help, please report the issue to Pufferfish Chickens mod's page. Thanks!", LogLevel.Debug);
            }
        }
    }
}
