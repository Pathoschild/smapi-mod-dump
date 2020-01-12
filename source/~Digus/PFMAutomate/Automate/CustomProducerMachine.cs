using System;
using System.Collections.Generic;
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
                    if (input.TryGetIngredient(objectInput.ParentSheetIndex, producerRule.InputStack,out IConsumable inputConsumable))
                    {
                        objectInput = inputConsumable.Sample as SObject;
                        List<IConsumable> requiredFuels = GetRequiredFuels(producerRule, input);
                        if (requiredFuels != null)
                        {
                            Random random = ProducerRuleController.GetRandomForProducing(_machine.TileLocation);
                            OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random);
                            SObject output = OutputConfigController.CreateOutput(outputConfig, objectInput, random);
                            _machine.heldObject.Value = output;
                            OutputConfigController.LoadOutputName(outputConfig, output, objectInput);

                            _machine.MinutesUntilReady = producerRule.MinutesUntilReady;

                            if (ProducerController.GetProducerConfig(_machine.Name) is ProducerConfig producerConfig)
                            {
                                _machine.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                            }

                            _machine.initializeLightSource(_machine.TileLocation, false);

                            producerRule.IncrementStatsOnInput.ForEach(s => StatsController.IncrementStardewStats(s, producerRule.InputStack));

                            inputConsumable.Take();
                            requiredFuels.ForEach(f => f.Reduce());
                            return true;
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
    }
}
