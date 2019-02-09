using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using System.Reflection;

namespace AnimalBirthEveryNight
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool AttemptAnimalBirth = false;
        private HarmonyInstance Harmony;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log($"This mod has not been fully tested and may corrupt your save file. Only used for debugging purposes. Use at your own discretion.", LogLevel.Warn);

            // Harmony
            this.Harmony = HarmonyInstance.Create("paritee.animalbirtheverynight");

            // Console commands
            helper.ConsoleCommands.Add("event_animalbirth", "Toggles force attempt to birth an animal every night.\n\nUsage: event_animalbirth", this.ToggleAttemptAnimalBirth);
        }
        
        private void ToggleAttemptAnimalBirth(string command, string[] args)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log($"Cannot toggle attempt animal birth - world not ready", LogLevel.Trace);

                return;
            }

            if (!Game1.IsMasterGame)
            {
                this.Monitor.Log($"Cannot toggle attempt animal birth - not master game", LogLevel.Trace);

                return;
            }
            
            this.AttemptAnimalBirth = !this.AttemptAnimalBirth;

            if (this.AttemptAnimalBirth)
            {
                MethodInfo targetMethod;
                HarmonyMethod prefixMethod;

                targetMethod = AccessTools.Method(typeof(Utility), "pickPersonalFarmEvent");
                prefixMethod = new HarmonyMethod(typeof(Patch).GetMethod("PickPersonalFarmEventPrefix"));

                this.Harmony.Patch(targetMethod, prefixMethod, null);

                targetMethod = AccessTools.Method(typeof(QuestionEvent), "setUp");
                prefixMethod = new HarmonyMethod(typeof(Patch).GetMethod("SetUpPrefix"));
                this.Harmony.Patch(targetMethod, prefixMethod, null);
            }
            else
            {
                this.Harmony.UnpatchAll("paritee.animalbirtheverynight");
            }

            this.Monitor.Log($"AttemptAnimalBirth is {this.AttemptAnimalBirth}", LogLevel.Trace);
        }
    }
}
