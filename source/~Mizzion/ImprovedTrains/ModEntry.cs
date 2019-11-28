using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using ImprovedTrains.Framework.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;

namespace ImprovedTrains
{
    public class ModEntry : Mod
    {
        private TrainCarDrawPatch tcdp;
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            //Harmony Original Code credit goes to Cat from the SDV Modding Discord, I modified his Harmony code.
            try
            {
                HarmonyInstance Harmony = HarmonyInstance.Create("mizzion.improvedtrains");

                //Now we set up the patches, will use a dictionary, just in case I need to expand later. Idea of using Harmony this way came from Cat#2506's mod  from the SDV discord
                IDictionary<string, Type> replacements = new Dictionary<string, Type>
                {
                    [nameof(TrainCar.draw)] = typeof(TrainCarDrawPatch)
                };

                IList<Type> typesToPatch = new List<Type>();
                typesToPatch.Add(typeof(TrainCar));
                
                //Go through and set up the patching
                foreach (Type t in typesToPatch)
                    foreach (KeyValuePair<string, Type> replacement in replacements)
                    {
                        MethodInfo original = t.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(m => m.Name == replacement.Key);

                        MethodInfo prefix = replacement.Value
                            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Prefix");
                        MethodInfo postfix = replacement.Value
                            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Postfix");

                        //this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);
                        this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);

                        Harmony.Patch(original, prefix == null ? null : new HarmonyMethod(prefix),
                            postfix == null ? null : new HarmonyMethod(postfix));
                    }

            }
            catch (Exception ex)
            {
                Monitor.Log($"There was an error setting up harmony.\n {ex}", LogLevel.Trace);
            }
            tcdp = new TrainCarDrawPatch(Monitor);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(SButton.NumPad6))
            {

            }
        }
    }
}
