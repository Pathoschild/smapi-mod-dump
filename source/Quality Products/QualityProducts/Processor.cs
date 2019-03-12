using System;
using Microsoft.Xna.Framework;
using QualityProducts.Processors;
using QualityProducts.Util;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts
{
    public abstract class Processor : SObject
    {
        public enum ProcessorType
        {
            KEG = 12,
            PRESERVE_JAR = 15,
            CHEESE_PRESS = 16,
            LOOM = 17,
            OIL_MAKER = 19,
            MAYONNAISE_MACHINE = 24
        }

        public static ProcessorType? WhichProcessor(int parentSheetIndex)
        {
            if (Enum.IsDefined(typeof(ProcessorType), parentSheetIndex))
            {
                return (ProcessorType)Enum.ToObject(typeof(ProcessorType), parentSheetIndex);
            }

            return null;
        }

        public static Processor Create(ProcessorType processorType)
        {
            switch (processorType)
            {
                case ProcessorType.KEG:
                    return new Keg();
                case ProcessorType.PRESERVE_JAR:
                    return new PreserveJar();
                case ProcessorType.CHEESE_PRESS:
                    return new CheesePress();
                case ProcessorType.LOOM:
                    return new Loom();
                case ProcessorType.OIL_MAKER:
                    return new OilMaker();
                case ProcessorType.MAYONNAISE_MACHINE:
                    return new MayonnaiseMachine();
                default:
                    throw new UnimplementedCaseException($"Enum value {Enum.GetName(typeof(ProcessorType), processorType)} of Processor.ValidType has no corresponding case");
            }
        }

        public static Processor Create(ProcessorType processorType, Action<Processor> initializer)
        {
            Processor newObj;
            switch (processorType)
            {
                case ProcessorType.KEG:
                    newObj = new Keg();
                    break;
                case ProcessorType.PRESERVE_JAR:
                    newObj = new PreserveJar();
                    break;
                case ProcessorType.CHEESE_PRESS:
                    newObj = new CheesePress();
                    break;
                case ProcessorType.LOOM:
                    newObj = new Loom();
                    break;
                case ProcessorType.OIL_MAKER:
                    newObj = new OilMaker();
                    break;
                case ProcessorType.MAYONNAISE_MACHINE:
                    newObj = new MayonnaiseMachine();
                    break;
                default:
                    throw new UnimplementedCaseException($"Enum value {Enum.GetName(typeof(ProcessorType), processorType)} of Processor.ValidType has no corresponding case");
            }
            initializer(newObj);
            return newObj;
        }

        public static Processor FromObject(SObject @object)
        {
            if (!@object.bigCraftable.Value)
            {
                return null;
            }

            ProcessorType? processorType = WhichProcessor(@object.ParentSheetIndex);
            if (processorType != null) {
                Processor processor = Create(processorType.Value,
                p => 
                {
                    p.TileLocation = @object.TileLocation;
                    p.IsRecipe = (bool)@object.isRecipe;
                    p.name = @object.name;
                    p.DisplayName = @object.DisplayName;
                    p.Scale = @object.Scale;
                    p.MinutesUntilReady = @object.MinutesUntilReady;
                });

                processor.owner.Value = @object.owner.Value;
                processor.heldObject.Value = @object.heldObject.Value;
                processor.readyForHarvest.Value = @object.readyForHarvest.Value;

                return processor;
            }
            return null;
        }

        protected Processor(ProcessorType processorType) : base(Vector2.Zero, (int)processorType, false)
        {
        }

        public SObject ToObject()
        {
            SObject @object = new SObject(tileLocation, parentSheetIndex, false)
            {
                IsRecipe = (bool)isRecipe,
                name = name,
                DisplayName = DisplayName,
                Scale = Scale,
                MinutesUntilReady = MinutesUntilReady
            };

            @object.owner.Value = owner.Value;
            @object.heldObject.Value = heldObject.Value;
            @object.readyForHarvest.Value = readyForHarvest.Value;

            return @object;
        }

        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (dropInItem is SObject)
            {
                SObject @object = dropInItem as SObject;
                if (heldObject.Value != null)
                {
                    return false;
                }
                if (@object != null && (bool)@object.bigCraftable)
                {
                    return false;
                }
                if (!probe && @object != null && heldObject.Value == null)
                {
                    scale.X = 5f;
                }

                if (PerformProcessing(@object, probe, who))
                {
                    heldObject.Value.Quality = @object.Quality;
                    return true;
                }
            }
            return false;
        }

        protected abstract bool PerformProcessing(SObject @object, bool probe, Farmer who);
    }
}
