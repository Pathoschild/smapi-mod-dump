using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Helpers;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;
using Igorious.StardewValley.DynamicAPI.Extensions;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartBigCrafrableBase
    {
        protected MachineBase(int id) : base(id) { }

        protected abstract MachineOutputInformation Output { get; }
        private static readonly List<Sound> DefaultSound = new List<Sound> { Sound.Ship };

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return (heldObject == null) && (Output.Items.ContainsKey(item.ParentSheetIndex) || Output.Items.ContainsKey(item.Category));
        }

        protected override bool PerformDropIn(Object dropInItem, Farmer farmer)
        {
            if (!ProcessRequiredItems(dropInItem, farmer)) return false;

            var outputItem = GetOutputItem(dropInItem);
            PutItem(
                GetOutputID(outputItem),
                GetOutputCount(outputItem, dropInItem),
                GetOutputQuality(outputItem, dropInItem),
                GetOutputName(outputItem, dropInItem),
                GetOutputPrice(outputItem, dropInItem),
                GetColor(outputItem, dropInItem));

            (Output.Sounds ?? DefaultSound).ForEach(PlaySound);
            if (Output.Animation != null) PlayAnimation(farmer, Output.Animation.Value);
            minutesUntilReady = GetMinutesUntilReady(outputItem);
            return true;
        }

        protected bool ProcessRequiredItems(Object dropInItem, Farmer farmer)
        {
            var outputItem = GetOutputItem(dropInItem);

            if (dropInItem.Stack < outputItem.InputCount)
            {
                ShowRedMessage(farmer, $"Requires {outputItem.InputCount} {dropInItem.Name}");
                return false;
            }

            var and = outputItem.And;
            if (and == null)
            {
                dropInItem.Stack -= (outputItem.InputCount - 1);
                return true;
            }

            var andName = new Object(and.ID, 1).Name;
            var actualCount = farmer.getTallyOfObject(and.ID, false);
            if (actualCount < and.Count)
            {
                ShowRedMessage(farmer, $"Requires {and.Count} {andName}");
                return false;
            }
            else
            {
                dropInItem.Stack -= (outputItem.InputCount - 1);
                InventoryHelper.RemoveItem(and.ID, and.Count, farmer);
                return true;
            }
        }

        #region Config Getters

        protected OutputItem GetOutputItem(Object dropInItem)
        {
            var outputInfo = TryGetOutputItemByID(dropInItem) ?? TryGetOutputItemByCategory(dropInItem);
            return ResolveVariants(outputInfo);
        }

        protected virtual string GetOutputName(OutputItem outputItem, Object dropInItem)
        {
            var itemNameFormat = outputItem?.Name;
            if (!string.IsNullOrWhiteSpace(itemNameFormat)) return string.Format(itemNameFormat, "{0}", dropInItem.Name);
            if (itemNameFormat != null && string.IsNullOrWhiteSpace(itemNameFormat)) return null;
            return (Output.Name != null) ? string.Format(Output.Name, "{0}", dropInItem.Name) : null;
        }

        protected virtual int GetOutputQuality(OutputItem outputItem, Object dropInItem)
        {
            var qualityExpression = outputItem?.Quality ?? Output.Quality;
            var calculateQuality = ExpressionCompiler.CompileExpression<QualityExpression>(qualityExpression);
            if (calculateQuality == null) return 0;

            var random = GetRandom();
            return calculateQuality(dropInItem.Price, dropInItem.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int GetOutputCount(OutputItem outputItem, Object dropInItem)
        {
            var countExpression = outputItem?.Count ?? Output.Count;
            var calculateCount = ExpressionCompiler.CompileExpression<CountExpression>(countExpression);
            if (calculateCount == null) return 1;

            var random = GetRandom();
            return calculateCount(dropInItem.Price, dropInItem.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int? GetOutputPrice(OutputItem outputItem, Object dropInItem)
        {
            var priceExpression = outputItem?.Price ?? Output.Price;
            var calculatePrice = ExpressionCompiler.CompileExpression<PriceExpression>(priceExpression);
            return calculatePrice?.Invoke(dropInItem.price, dropInItem.quality);
        }

        protected virtual int GetOutputID(OutputItem outputItem)
        {
            return outputItem?.ID ?? Output.ID ?? 0;
        }

        protected virtual int GetMinutesUntilReady(OutputItem outputItem)
        {
            return outputItem?.MinutesUntilReady ?? Output.MinutesUntilReady ?? 0;
        }

        private Color? GetColor(OutputItem outputItem, Object dropInItem)
        {
            var color = outputItem?.Color;
            if (string.IsNullOrWhiteSpace(color)) return null;

            return (color != "@")
                ? RawColor.FromHex(color).ToXnaColor()
                : DominantColorFinder.GetDominantColor(dropInItem.ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
        }
      
        private OutputItem TryGetOutputItemByID(Object dropInItem)
        {
            return Output.Items.TryGetValue(dropInItem.ParentSheetIndex);
        }

        private OutputItem TryGetOutputItemByCategory(Object dropInItem)
        {
            return Output.Items.TryGetValue(dropInItem.Category);
        }

        private OutputItem ResolveVariants(OutputItem outputItem)
        {
            var variants = outputItem?.Switch;
            if (variants == null) return outputItem;

            var random = Convert.ToDecimal(GetRandom().NextDouble());
            var current = 0m;
            foreach (var variant in variants.Where(o => o.Chance != null))
            {
                current += variant.Chance.Value;
                if (current >= random) return variant;
            }
            return variants.FirstOrDefault(o => o.Chance == null) ?? variants.Last();
        }

        #endregion
    }
}