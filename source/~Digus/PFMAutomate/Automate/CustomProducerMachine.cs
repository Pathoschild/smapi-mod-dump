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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using ProducerFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Controllers;
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
            if (_machine.heldObject.Value != null && _machine.MinutesUntilReady <= 0 && _machine.readyForHarvest.Value)
            {
                return MachineState.Done;
            }
            if (ProducerController.GetProducerConfig(_machine.QualifiedItemId) is ProducerConfig producerConfig)
            {
                if (!producerConfig.CheckWeatherCondition() || !producerConfig.CheckSeasonCondition(Location) || !producerConfig.CheckLocationCondition(Location) || !producerConfig.CheckCurrentTimeCondition())
                {
                    return MachineState.Disabled;
                }
                if (producerConfig.NoInputStartMode != null)
                {
                    //A no input machine is considered processing even while empty.
                    return MachineState.Processing;
                }
            }
            if (_machine.heldObject.Value == null)
            {
                return MachineState.Empty;
            }
            return MachineState.Processing;
        }

        public ITrackedStack GetOutput()
        {
            ProducerRuleController.PrepareOutput(_machine, this.Location, Game1.getFarmer((long)_machine.owner.Value));

            return new TrackedItem(_machine.heldObject.Value, onEmpty: Reset);
        }

        public void Reset(Item item)
        {
            ProducerRuleController.ClearProduction(_machine, Location);
            if (ProducerController.GetProducerConfig(_machine.QualifiedItemId) is ProducerConfig producerConfig)
            {
                if (producerConfig.NoInputStartMode != null || producerConfig.IncrementStatsOnOutput.Count > 0 || producerConfig.IncrementStatsLabelOnOutput.Count > 0)
                {
                    producerConfig.IncrementStats(item);
                    if (producerConfig.NoInputStartMode == NoInputStartMode.Placement)
                    {
                        if (ProducerController.GetProducerItem(_machine.QualifiedItemId, null) is ProducerRule producerRule)
                        {
                            try
                            {
                                if (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition(Location))
                                {
                                    ProducerRuleController.ProduceOutput(producerRule, _machine, (i, q) => true, null, Location, producerConfig);
                                }
                            }
                            catch (RestrictionException) {/*No action needed*/}
                        }
                    }
                }
            }
        }

        public bool SetInput(IStorage input)
        {
            foreach (ITrackedStack trackedStack in input.GetItems())
            {
                if (trackedStack.Sample is SObject objectInput 
                    && !objectInput.bigCraftable.Value 
                    && ProducerController.GetProducerItem(_machine.QualifiedItemId, objectInput) is ProducerRule producerRule
                    && !ProducerRuleController.IsInputExcluded(producerRule, objectInput))
                {
                    ProducerConfig producerConfig = ProducerController.GetProducerConfig(_machine.QualifiedItemId);

                    if (producerConfig == null || (producerConfig.CheckLocationCondition(Location) && producerConfig.CheckSeasonCondition(Location)))
                    {
                        if (input.TryGetIngredient(i=> objectInput.ItemId == i.Sample.ItemId, producerRule.InputStack,
                            out IConsumable inputConsumable))
                        {
                            objectInput = inputConsumable.Sample as SObject;
                            List<IConsumable> requiredFuels = GetRequiredFuels(producerRule, input);
                            if (requiredFuels != null)
                            {
                                try
                                {
                                    if (objectInput != null) objectInput.Stack = inputConsumable.Consumables.Count;
                                    OutputConfig outputConfig = ProducerRuleController.ProduceOutput(producerRule, _machine,
                                        (i, q) => input.TryGetIngredient((j) => j.Sample.QualifiedItemId == i || j.Sample.Category.ToString() == i, q, out IConsumable fuel), null, Location,
                                        producerConfig, objectInput,noSoundAndAnimation:true);
                                    if (outputConfig != null)
                                    {
                                        if (outputConfig.RequiredInputStack.HasValue)
                                        {
                                            inputConsumable.Consumables.Reduce(outputConfig.RequiredInputStack.Value);
                                        }
                                        else
                                        {
                                            inputConsumable.Reduce();
                                        }
                                        requiredFuels.ForEach(f => f.Reduce());
                                        List<IConsumable> outputRequiredFuels = GetRequiredFuels(outputConfig, input);
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
            return false;
        }

        private List<IConsumable> GetRequiredFuels(ProducerRule producerRule, IStorage storage)
        {
            List<IConsumable> requiredFuels =  new List<IConsumable>();
            foreach (Tuple<string, int> requiredFuel in producerRule.FuelList)
            {
                if (!storage.TryGetIngredient((i) => i.Sample.QualifiedItemId == requiredFuel.Item1 || i.Sample.Category.ToString() == requiredFuel.Item1, requiredFuel.Item2, out IConsumable fuel))
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
            foreach (Tuple<string, int> requiredFuel in outputConfig.FuelList)
            {
                if (!storage.TryGetIngredient((i) => i.Sample.QualifiedItemId == requiredFuel.Item1 || i.Sample.Category.ToString() == requiredFuel.Item1, requiredFuel.Item2, out IConsumable fuel))
                {
                    return null;
                }
                requiredFuels.Add(fuel);
            }
            return requiredFuels;
        }
    }
}
