/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

namespace MassProduction.VanillaOverrides
{
    public class SeedMakerOverride : IVanillaOverride
    {
        public static Dictionary<int, int> SEED_LOOKUP;

        /// <summary>
        /// Initializes the map of produce -> seed.
        /// </summary>
        public static void Initialize()
        {
            SEED_LOOKUP = new Dictionary<int, int>();

            var crops = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");

            foreach (KeyValuePair<int, string> entry in crops)
            {
                int seedId = entry.Key;
                int produceId = Convert.ToInt32(entry.Value.Split('/')[3]);

                //Take only the first that appears
                if (!SEED_LOOKUP.ContainsKey(produceId))
                {
                    SEED_LOOKUP.Add(produceId, seedId);
                }
            }
        }

        /// <inheritdoc/>
        public bool Automate_SetInput(IStorage input, MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject)
        {
            if (mpm != null)
            {
                try
                {
                    ITrackedStack crop = null;
                    InputInfo cropInfo = null;
                    int inputQuantity = 0;

                    foreach (ITrackedStack item in input.GetItems())
                    {
                        if (IsValidCrop(item))
                        {
                            InputInfo inputInfo = new InputInfo()
                            {
                                ID = item.Sample.ParentSheetIndex,
                                Name = item.Sample.Name,
                                Quality = 0,
                                IsFuel = false,
                                BaseQuantity = 1
                            };

                            if (item.Sample is SObject obj)
                            {
                                inputInfo.Quality = obj.Quality;
                            }
                            
                            inputQuantity = mpm.Settings.CalculateInputRequired(inputInfo);

                            if (item.Count >= inputQuantity)
                            {
                                crop = item;
                                cropInfo = inputInfo;
                                break;
                            }
                        }
                    }

                    if (crop != null)
                    {
                        crop.Reduce(inputQuantity);
                        int seedID = SEED_LOOKUP[crop.Sample.ParentSheetIndex];

                        Random random = new Random((int)Game1.stats.DaysPlayed +
                            (int)Game1.uniqueIDForThisGame / 2 + (int)originalMachineObject.TileLocation.X + (int)originalMachineObject.TileLocation.Y * 77 + Game1.timeOfDay);
                        int outputID = seedID;
                        int outputQuantity = mpm.Settings.CalculateOutputProduced(random.Next(1, 4), cropInfo);

                        if (random.NextDouble() < 0.005)
                        {
                            outputID = 499;
                            outputQuantity = mpm.Settings.CalculateOutputProduced(1, cropInfo);
                        }
                        else if (random.NextDouble() < 0.02)
                        {
                            outputID = 770;
                            outputQuantity = mpm.Settings.CalculateOutputProduced(random.Next(1, 5), cropInfo);
                        }

                        originalMachineObject.heldObject.Value = new SObject(outputID, outputQuantity);
                        originalMachineObject.MinutesUntilReady = mpm.Settings.CalculateTimeRequired(20);

                        return true;
                    }
                }
                catch (Exception e)
                {
                    ModEntry.Instance.Monitor.Log($"{e}", StardewModdingAPI.LogLevel.Error);
                }
            }
            else
            {
                return originalMachine.SetInput(input);
            }

            return false;
        }

        /// <summary>
        /// Adapted from https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/Machines/Objects/SeedMakerMachine.cs
        /// Get whether a given item is a crop compatible with the seed marker.
        /// </summary>
        /// <param name="item">The item to check.</param>
        private bool IsValidCrop(ITrackedStack item)
        {
            return
                item.Type == ItemType.Object
                && item.Sample.ParentSheetIndex != 433 // seed maker doesn't allow coffee beans
                && SEED_LOOKUP.ContainsKey(item.Sample.ParentSheetIndex);
        }

        /// <summary>
        /// Get whether a given item is a crop compatible with the seed marker.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsValidCrop(SObject input)
        {
            return input.ParentSheetIndex != 433 && SEED_LOOKUP.ContainsKey(input.ParentSheetIndex);
        }

        /// <inheritdoc/>
        public bool Manual_PerformObjectDropInAction(SObject machine, SObject input, bool probe, Farmer who, MassProductionMachineDefinition mpm)
        {
            if (!probe && mpm != null)
            {
                try
                {
                    InputInfo inputInfo = InputInfo.ConvertInput(input, 1);
                    int inputQuantity = mpm.Settings.CalculateInputRequired(inputInfo);

                    if (IsValidCrop(input) && input.Stack >= inputQuantity)
                    {
                        input.Stack -= inputQuantity;

                        int seedID = SEED_LOOKUP[input.ParentSheetIndex];

                        Random random = new Random((int)Game1.stats.DaysPlayed +
                            (int)Game1.uniqueIDForThisGame / 2 + (int)machine.TileLocation.X + (int)machine.TileLocation.Y * 77 + Game1.timeOfDay);
                        int outputID = seedID;
                        int outputQuantity = mpm.Settings.CalculateOutputProduced(random.Next(1, 4), inputInfo);

                        if (random.NextDouble() < 0.005)
                        {
                            outputID = 499;
                            outputQuantity = mpm.Settings.CalculateOutputProduced(1, inputInfo);
                        }
                        else if (random.NextDouble() < 0.02)
                        {
                            outputID = 770;
                            outputQuantity = mpm.Settings.CalculateOutputProduced(random.Next(1, 5), inputInfo);
                        }

                        machine.heldObject.Value = new SObject(outputID, outputQuantity);
                        machine.MinutesUntilReady = mpm.Settings.CalculateTimeRequired(20);

                        return true;
                    }
                }
                catch (Exception e)
                {
                    ModEntry.Instance.Monitor.Log($"{e}", StardewModdingAPI.LogLevel.Error);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public ITrackedStack Automate_GetOutput(MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool Manual_PerformDropDownAction(SObject machine, MassProductionMachineDefinition mpm)
        {
            return false;
        }
    }
}
