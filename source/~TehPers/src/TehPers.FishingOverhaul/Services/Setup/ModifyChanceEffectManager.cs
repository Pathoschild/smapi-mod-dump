/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Effects;
using TehPers.FishingOverhaul.Api.Events;
using TehPers.FishingOverhaul.Effects;
using TehPers.FishingOverhaul.Parsing;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class ModifyChanceEffectManager : ISetup, IDisposable
    {
        private readonly IMonitor monitor;
        private readonly IFishingApi fishingApi;
        private readonly Dictionary<Operation, List<Func<double, double>>> appliedFarmers;
        private readonly HashSet<IDisposable> disposables;

        public ModifyChanceEffectManager(IMonitor monitor, IFishingApi fishingApi)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.appliedFarmers = new();
            this.disposables = new();
        }

        public IFishingEffect CreateEffect(ModifyChanceEffectEntry options)
        {
            // Parse the expression
            if (!ExpressionParser.TryParse(options.Expression, out var parsed, out var error))
            {
                this.monitor.Log(
                    $"Invalid expression '{options.Expression}': {error}",
                    LogLevel.Error
                );
                return new EmptyEffect();
            }

            // Compile the expression
            if (!parsed.TryCompile("x", out var result, out var missingVariables))
            {
                this.monitor.Log(
                    $"Error compiling expression '{options.Expression}':",
                    LogLevel.Error
                );
                foreach (var missingVariable in missingVariables)
                {
                    this.monitor.Log($"- Unknown variable {missingVariable}", LogLevel.Error);
                }

                return new EmptyEffect();
            }

            // Create the effect
            return new Effect(options.Type, result.Compile(), this.appliedFarmers);
        }

        public void Setup()
        {
            this.disposables.Add(
                new EffectManager<CalculatedFishChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.Fish,
                    handler => this.fishingApi.CalculatedFishChance += handler,
                    handler => this.fishingApi.CalculatedFishChance -= handler
                )
            );
            this.disposables.Add(
                new EffectManager<CalculatedFishChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.MinFish,
                    handler => this.fishingApi.CalculatedMinFishChance += handler,
                    handler => this.fishingApi.CalculatedMinFishChance -= handler
                )
            );
            this.disposables.Add(
                new EffectManager<CalculatedFishChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.MaxFish,
                    handler => this.fishingApi.CalculatedMaxFishChance += handler,
                    handler => this.fishingApi.CalculatedMaxFishChance -= handler
                )
            );
            this.disposables.Add(
                new EffectManager<CalculatedTreasureChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.Treasure,
                    handler => this.fishingApi.CalculatedTreasureChance += handler,
                    handler => this.fishingApi.CalculatedTreasureChance -= handler
                )
            );
            this.disposables.Add(
                new EffectManager<CalculatedTreasureChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.MinTreasure,
                    handler => this.fishingApi.CalculatedMinTreasureChance += handler,
                    handler => this.fishingApi.CalculatedMinTreasureChance -= handler
                )
            );
            this.disposables.Add(
                new EffectManager<CalculatedTreasureChanceEventArgs>(
                    this.appliedFarmers,
                    ModifyChanceType.MaxTreasure,
                    handler => this.fishingApi.CalculatedMaxTreasureChance += handler,
                    handler => this.fishingApi.CalculatedMaxTreasureChance -= handler
                )
            );
        }

        public void Dispose()
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }
        }

        private sealed class EffectManager<TEventArgs> : IDisposable
            where TEventArgs : ChanceCalculatedEventArgs
        {
            private readonly Dictionary<Operation, List<Func<double, double>>> appliedFarmers;
            private readonly ModifyChanceType chanceType;
            private readonly Action<EventHandler<TEventArgs>> unregister;

            public EffectManager(
                Dictionary<Operation, List<Func<double, double>>> appliedFarmers,
                ModifyChanceType chanceType,
                Action<EventHandler<TEventArgs>> register,
                Action<EventHandler<TEventArgs>> unregister
            )
            {
                this.appliedFarmers = appliedFarmers
                    ?? throw new ArgumentNullException(nameof(appliedFarmers));
                this.chanceType = chanceType;
                this.unregister = unregister ?? throw new ArgumentNullException(nameof(unregister));

                register(this.HandleEvent);
            }

            public void Dispose()
            {
                this.unregister(this.HandleEvent);
            }

            private void HandleEvent(object? sender, TEventArgs args)
            {
                if (!this.appliedFarmers.TryGetValue(
                        new(this.chanceType, args.FishingInfo.User),
                        out var calculators
                    ))
                {
                    return;
                }

                args.Chance = calculators.Aggregate(
                    args.Chance,
                    (chance, calculator) => calculator(chance)
                );
            }
        }

        public record Operation(ModifyChanceType Type, Farmer Target);

        public class Effect : IFishingEffect
        {
            private readonly ModifyChanceType chanceType;
            private readonly Func<double, double> calculate;
            private readonly Dictionary<Operation, List<Func<double, double>>> appliedFarmers;

            public Effect(
                ModifyChanceType chanceType,
                Func<double, double> calculate,
                Dictionary<Operation, List<Func<double, double>>> appliedFarmers
            )
            {
                this.chanceType = chanceType;
                this.calculate = calculate;
                this.appliedFarmers = appliedFarmers
                    ?? throw new ArgumentNullException(nameof(appliedFarmers));
            }

            public void Apply(FishingInfo fishingInfo)
            {
                var operation = new Operation(this.chanceType, fishingInfo.User);
                var calculators = this.appliedFarmers.GetOrAdd(operation, () => new());
                calculators.Add(this.calculate);
            }

            public void Unapply(FishingInfo fishingInfo)
            {
                if (this.appliedFarmers.TryGetValue(
                        new(this.chanceType, fishingInfo.User),
                        out var calculators
                    ))
                {
                    calculators.Remove(this.calculate);
                }
            }

            public void UnapplyAll()
            {
                this.appliedFarmers.Clear();
            }
        }
    }
}
