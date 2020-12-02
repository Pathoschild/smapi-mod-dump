/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using ProducerFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction.Automate
{
    /// <summary>
    /// For Automate integration.
    /// </summary>
    public class MPMAutomated : IMachine
    {
        protected SObject Machine;

        public string MachineTypeID
        {
            get
            {
                string mpKey = Machine.GetMassProducerKey();
                string machineID = "MassProduction." + Machine.Name;
                if (!string.IsNullOrEmpty(mpKey))
                {
                    machineID += "." + mpKey;
                }
                return machineID;
            }
        }
        public GameLocation Location { get; protected set; }
        public Rectangle TileArea { get; protected set; }

        public bool IsMassProducer
        {
            get
            {
                return !string.IsNullOrEmpty(Machine.GetMassProducerKey()) && ModEntry.GetMPMMachine(Machine.name, Machine.GetMassProducerKey()) != null;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="location"></param>
        /// <param name="tile"></param>
        public MPMAutomated(SObject machine, GameLocation location, Vector2 tile)
        {
            Machine = machine;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        /// <summary>
        /// Gets the output.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/PFMAutomate/Automate/CustomProducerMachine.cs
        /// </summary>
        /// <returns></returns>
        public ITrackedStack GetOutput()
        {
            PFMCompatability.PrepareOutput(Machine, Location, Game1.getFarmer(Machine.owner));
            return new TrackedItem(Machine.heldObject.Value, onEmpty: Reset);
        }

        /// <summary>
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/PFMAutomate/Automate/CustomProducerMachine.cs
        /// </summary>
        /// <param name="item"></param>
        internal void Reset(Item item)
        {
            PFMCompatability.ClearProduction(Machine, Location);
            MassProductionMachineDefinition mpm = null;
            ProducerConfig producerConfig;

            if (IsMassProducer)
            {
                mpm = ModEntry.GetMPMMachine(Machine.name, Machine.GetMassProducerKey());
                producerConfig = ProducerController.GetProducerConfig(mpm.BaseProducerName);
            }
            else
            {
                producerConfig = ProducerController.GetProducerConfig(Machine.name);
            }

            if (producerConfig == null)
            {
                return;
            }
            else if (producerConfig.NoInputStartMode != null || producerConfig.IncrementStatsOnOutput.Count > 0)
            {
                producerConfig.IncrementStats(item);
                if (producerConfig.NoInputStartMode == NoInputStartMode.Placement)
                {
                    if (ProducerController.GetProducerItem(Machine.Name, null) is ProducerRule producerRule)
                    {
                        try
                        {
                            if (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition())
                            {
                                if (mpm != null)
                                {
                                    PFMCompatability.ProduceOutput(producerRule, mpm.Settings, Machine, (i, q) => true, null, Location, producerConfig);
                                }
                                else
                                {
                                    ProducerRuleController.ProduceOutput(producerRule, Machine, (i, q) => true, null, Location, producerConfig);
                                }
                            }
                        }
                        catch (RestrictionException) {/*No action needed*/}
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current operation of the machine.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/PFMAutomate/Automate/CustomProducerMachine.cs
        /// </summary>
        /// <returns></returns>
        public MachineState GetState()
        {
            if (Machine.heldObject.Value != null && Machine.minutesUntilReady <= 0 && Machine.readyForHarvest.Value)
            {
                return MachineState.Done;
            }

            MassProductionMachineDefinition mpm;
            ProducerConfig producerConfig = GetCurrentConfig(out mpm);

            if (producerConfig != null)
            {
                if (!producerConfig.CheckWeatherCondition() || !producerConfig.CheckSeasonCondition() || !producerConfig.CheckLocationCondition(Location) ||
                            !producerConfig.CheckCurrentTimeCondition())
                {
                    return MachineState.Disabled;
                }
                if (producerConfig.NoInputStartMode != null)
                {
                    //A no input machine is considered processing even while empty.
                    return MachineState.Processing;
                }
            }
            else if (StaticValues.SUPPORTED_VANILLA_MACHINES.ContainsKey(Machine.name) &&
                StaticValues.SUPPORTED_VANILLA_MACHINES[Machine.name] == InputRequirement.NoInputsOnly)
            {
                //A no input machine is considered processing even while empty.
                return MachineState.Processing;
            }

            if (Machine.heldObject.Value == null)
            {
                return MachineState.Empty;
            }

            return MachineState.Processing;
        }

        /// <summary>
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/PFMAutomate/Automate/CustomProducerMachine.cs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool SetInput(IStorage input)
        {
            if (IsMassProducer)
            {
                MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(Machine.name, Machine.GetMassProducerKey());

                foreach (ITrackedStack trackedStack in input.GetItems())
                {
                    if (trackedStack.Sample is SObject objectInput && !objectInput.bigCraftable.Value &&
                        ProducerController.GetProducerItem(Machine.Name, objectInput) is ProducerRule producerRule &&
                        !PFMCompatability.IsInputExcluded(producerRule, mpm, objectInput))
                    {
                        ProducerConfig producerConfig = mpm.GetBaseProducerConfig();

                        if (producerConfig == null || (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition()))
                        {
                            List<InputInfo> inputsRequired = InputInfo.ConvertPFMInputs(producerRule, objectInput);

                            if (inputsRequired.Count > 0 &&
                                input.TryGetIngredient(objectInput.ParentSheetIndex, mpm.Settings.CalculateInputRequired(inputsRequired.First()), out IConsumable inputConsumable))
                            {
                                objectInput = inputConsumable.Sample as SObject;
                                List<IConsumable> requiredFuels = GetRequiredFuels(inputsRequired, mpm.Settings, input);

                                if (requiredFuels != null)
                                {
                                    try
                                    {
                                        Dictionary<int, int> fuelQuantities = new Dictionary<int, int>();

                                        foreach (InputInfo inputInfo in inputsRequired)
                                        {
                                            if (inputInfo.IsFuel)
                                            {
                                                fuelQuantities.Add(inputInfo.ID, mpm.Settings.CalculateInputRequired(inputInfo));
                                            }
                                        }

                                        Func<int, int, bool> fuelSearch = (i, q) => input.TryGetIngredient(i, fuelQuantities[i], out IConsumable fuel);
                                        OutputConfig outputConfig = PFMCompatability.ProduceOutput(producerRule, mpm.Settings, Machine, fuelSearch, null, Location, producerConfig,
                                            objectInput, inputQuantity: mpm.Settings.CalculateInputRequired(inputsRequired.First()), noSoundAndAnimation: true,
                                            inputInfo: inputsRequired);

                                        if (outputConfig != null)
                                        {
                                            inputConsumable.Take();
                                            requiredFuels.ForEach(f => f.Reduce());
                                            List<IConsumable> outputRequiredFuels = GetRequiredFuels(InputInfo.ConvertPFMInputs(outputConfig), mpm.Settings, input);
                                            outputRequiredFuels.ForEach(f => f.Reduce());
                                            return true;
                                        }
                                    }
                                    catch (RestrictionException) {/* No action needed */}
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (ITrackedStack trackedStack in input.GetItems())
                {
                    if (trackedStack.Sample is SObject objectInput && !objectInput.bigCraftable.Value &&
                        ProducerController.GetProducerItem(Machine.Name, objectInput) is ProducerRule producerRule &&
                        !ProducerRuleController.IsInputExcluded(producerRule, objectInput))
                    {
                        ProducerConfig producerConfig = ProducerController.GetProducerConfig(Machine.name);

                        if (producerConfig == null || (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition()))
                        {
                            if (input.TryGetIngredient(objectInput.ParentSheetIndex, producerRule.InputStack, out IConsumable inputConsumable))
                            {
                                objectInput = inputConsumable.Sample as SObject;
                                List<IConsumable> requiredFuels = GetRequiredFuels(InputInfo.ConvertPFMInputs(producerRule, objectInput), null, input);

                                if (requiredFuels != null)
                                {
                                    try
                                    {
                                        Func<int, int, bool> fuelSearch = (i, q) => input.TryGetIngredient(i, q, out IConsumable fuel);
                                        OutputConfig outputConfig = ProducerRuleController.ProduceOutput(producerRule, Machine, fuelSearch, null, Location, producerConfig,
                                            objectInput, noSoundAndAnimation: true);

                                        if (outputConfig != null)
                                        {
                                            inputConsumable.Take();
                                            requiredFuels.ForEach(f => f.Reduce());
                                            List<IConsumable> outputRequiredFuels = GetRequiredFuels(InputInfo.ConvertPFMInputs(outputConfig), null, input);
                                            outputRequiredFuels.ForEach(f => f.Reduce());
                                            return true;
                                        }
                                    }
                                    catch (RestrictionException) {/* No action needed */}
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/PFMAutomate/Automate/CustomProducerMachine.cs
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="settings"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        private List<IConsumable> GetRequiredFuels(List<InputInfo> inputs, MPMSettings settings, IStorage storage)
        {
            List<IConsumable> requiredFuels = new List<IConsumable>();

            foreach (InputInfo input in inputs)
            {
                if (!input.IsFuel) { continue; }

                int quantity = (settings != null) ? settings.CalculateInputRequired(input) : input.BaseQuantity;

                if (quantity == 0) { continue; }

                if (!storage.TryGetIngredient(input.ID, quantity, out IConsumable fuel))
                {
                    return null;
                }
                requiredFuels.Add(fuel);
            }

            return requiredFuels;
        }

        /// <summary>
        /// Gets the current configuration of the machine.
        /// </summary>
        /// <param name="mpm"></param>
        /// <returns></returns>
        private ProducerConfig GetCurrentConfig(out MassProductionMachineDefinition mpm)
        {
            mpm = null;
            ProducerConfig producerConfig;

            if (IsMassProducer)
            {
                mpm = ModEntry.GetMPMMachine(Machine.name, Machine.GetMassProducerKey());
                producerConfig = ProducerController.GetProducerConfig(mpm.BaseProducerName);
            }
            else
            {
                producerConfig = ProducerController.GetProducerConfig(Machine.name);
            }

            return producerConfig;
        }
    }
}