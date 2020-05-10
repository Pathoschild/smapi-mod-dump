using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using MisophoniaAccessibility.Config;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace MisophoniaAccessibility
{
    public class MisophoniaAccessibilityMod : Mod
    {
        public static Dictionary<string, bool> DisabledSounds { get; set; } = new Dictionary<string, bool>();

        public override void Entry(IModHelper helper)
        {
            ModConfig modConfig = helper.ReadConfig<ModConfig>();

            if (!modConfig.Enabled)
                return;

            if (modConfig.SoundsToDisable != null)
            {
                SetDisabledSounds(modConfig.SoundsToDisable);
                PerformHarmonyPatches();
            }
        }

        /// <summary>
        /// Patches the Game1.playSound method to not play the eat sound
        /// </summary>
        private void PerformHarmonyPatches()
        {
            try
            {
                Monitor.Log("Doing harmony patches...");

                HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

                MethodInfo originalMethod = AccessTools.Method(typeof(Game1), "playSound");
                MethodInfo prefixMethod = AccessTools.Method(typeof(EatSoundPatch), nameof(EatSoundPatch.Prefix));

                // Have Harmony take the original property get method, and replace it with the new one we defined in DailyLuckPatch
                harmony.Patch(
                    original: originalMethod,
                    prefix: new HarmonyMethod(prefixMethod));
            }
            catch (Exception e)
            {
                Monitor.Log("Failed to patch MisophoniaAccessibility with Harmony" + e.ToString(), LogLevel.Error);
                throw;
            }
        }

        private void SetDisabledSounds(Sounds soundConfig)
        {
            PropertyInfo[] configProperties = soundConfig.GetType().GetProperties();

            foreach (PropertyInfo property in configProperties)
            {
                GameSound gameSound = (GameSound)property.GetCustomAttribute(typeof(GameSound));

                string soundName = gameSound.Name;
                bool isDisabled = (bool) property.GetValue(soundConfig);

                MisophoniaAccessibilityMod.DisabledSounds.Add(soundName, isDisabled);
            }
        }
    }
}
