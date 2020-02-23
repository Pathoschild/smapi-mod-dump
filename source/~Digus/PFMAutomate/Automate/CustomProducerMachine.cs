using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using ProducerFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using SObject = StardewValley.Object;

namespace PFMAutomate.Automate
{
    internal class CustomProducerMachine : IMachine
    {
        public string MachineTypeID { get; }

        private readonly SObject _machine;
        public GameLocation Location { get; }
        public Rectangle TileArea { get; }

        public CustomProducerMachine(SObject machine, GameLocation location, Vector2 tile)
        {
            MachineTypeID = "PFM.Custom." + machine.Name;
            _machine = machine;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            if (_machine.heldObject.Value == null)
                return MachineState.Empty;

            return _machine.readyForHarvest.Value
                ? MachineState.Done
                : MachineState.Processing;
        }

        public ITrackedStack GetOutput()
        {
            return new TrackedItem(_machine.heldObject.Value, onEmpty: item =>
            {
                _machine.heldObject.Value = null;
                _machine.readyForHarvest.Value = false;
            });
        }

        public bool SetInput(IStorage input)
        {
            foreach (ITrackedStack trackedStack in input.GetItems())
            {
                if (trackedStack.Sample is SObject objectInput 
                    && !objectInput.bigCraftable.Value 
                    && ProducerController.GetProducerItem(_machine.Name, objectInput) is ProducerRule producerRule
                    && !ProducerRuleController.IsInputExcluded(producerRule, objectInput))
                {
                    ProducerConfig producerConfig = ProducerController.GetProducerConfig(_machine.Name);

                    if (producerConfig == null || (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition()))
                    {
                        if (input.TryGetIngredient(objectInput.ParentSheetIndex, producerRule.InputStack,
                            out IConsumable inputConsumable))
                        {
                            objectInput = inputConsumable.Sample as SObject;
                            List<IConsumable> requiredFuels = GetRequiredFuels(producerRule, input);
                            if (requiredFuels != null)
                            {
                                try
                                {
                                    OutputConfig outputConfig = ProducerRuleController.ProduceOutput(producerRule, _machine,
                                        (i, q) => input.TryGetIngredient(i, q, out IConsumable fuel), null, Location,
                                        producerConfig, objectInput,noSoundAndAnimation:true);
                                    if (outputConfig != null)
                                    {
                                        inputConsumable.Take();
                                        requiredFuels.ForEach(f => f.Reduce());
                                        List<IConsumable> outputRequiredFuels = GetRequiredFuels(outputConfig, input);
                                        outputRequiredFuels.ForEach(f => f.Reduce()); return true;
                                    }
                                }
                                catch (RestrictionException) {/* No action needed */}
                            }
                        }
                    }
                } 
            }

            return false;
        }

        private List<IConsumable> GetRequiredFuels(ProducerRule producerRule, IStorage storage)
        {
            List<IConsumable> requiredFuels =  new List<IConsumable>();
            foreach (Tuple<int, int> requiredFuel in producerRule.FuelList)
            {
                if (!storage.TryGetIngredient(requiredFuel.Item1, requiredFuel.Item2, out IConsumable fuel))
                {
                    return null;
                }
                requiredFuels.Add(fuel);
            }
            return requiredFuels;
        }

        private List<IConsumable> GetRequiredFuels(OutputConfig outputConfig, IStorage storage)
        {
            List<IConsumable> requiredFuels = new List<IConsumable>();
            foreach (Tuple<int, int> requiredFuel in outputConfig.FuelList)
            {
                if (!storage.TryGetIngredient(requiredFuel.Item1, requiredFuel.Item2, out IConsumable fuel))
                {
                    return null;
                }
                requiredFuels.Add(fuel);
            }
            return requiredFuels;
        }
    }
}
