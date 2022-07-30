/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using static MachineAugmentors.Harmony.GamePatches;
using Object = StardewValley.Object;

namespace MachineAugmentors.Items
{
    [XmlRoot(ElementName = "AugmentorType", Namespace = "")]
    public enum AugmentorType
    {
        /// <summary>Provides a small increase to the machine's outputs</summary>
        [XmlEnum("Output")]
        [Description("Output")]
        Output = 0,
        /// <summary>Provides a small decrease to the machine's processing time</summary>
        [XmlEnum("Speed")]
        [Description("Speed")]
        Speed = 1,
        /// <summary>Provides a small decrease to the machine's inputs</summary>
        [XmlEnum("Efficiency")]
        [Description("Efficiency")]
        Efficiency = 2,
        /// <summary>Provides a small chance that the machine will produce higher quality products</summary>
        [XmlEnum("Quality")]
        [Description("Quality")]
        Quality = 3,
        /// <summary>Provides a large increase to the machine's output, but also to its inputs</summary>
        [XmlEnum("Production")]
        [Description("Production")]
        Production = 4,
        /// <summary>Provides a small chance that the machine will spawn a copy of itself when inputs are placed into it<para/>
        /// (chance is affected by <see cref="Object.MinutesUntilReady"/>, products that take more time have higher chance to create a duplicate machine)</summary>
        [XmlEnum("Duplication")]
        [Description("Duplication")]
        Duplication = 5
    }

    [XmlRoot("OutputAugmentor", Namespace = "")]
    public class OutputAugmentor : Augmentor
    {
        public OutputAugmentor() : base(AugmentorType.Output) { }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(CFAData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Output) || Info.RequiresInput || CFAData.CurrentHeldObject == null)
                    return;

                int PreviousStack = CFAData.CurrentHeldObjectQuantity;
                int NewStack = ComputeNewValue(AugmentorQuantity, PreviousStack, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                CFAData.CurrentHeldObject.Stack = NewStack;

                MachineAugmentorsMod.LogTrace(AugmentorType.Output, AugmentorQuantity, CFAData.Machine, Info.RequiresInput, CFAData.Machine.TileLocation,
                    "HeldObject.Stack", PreviousStack, DesiredNewValue, NewStack, Effect);
            }
#endif
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Output) || !Info.RequiresInput || PODIData.CurrentHeldObject == null)
                    return;

                int PreviousStack = PODIData.CurrentHeldObjectQuantity;
                int NewStack = ComputeNewValue(AugmentorQuantity, PreviousStack, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                PODIData.CurrentHeldObject.Stack = NewStack;

                MachineAugmentorsMod.LogTrace(AugmentorType.Output, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "HeldObject.Stack", PreviousStack, DesiredNewValue, NewStack, Effect);
            }
#endif
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(MS.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Output) || AugmentorQuantity <= 0 || MS.Machine.readyForHarvest.Value || MS.CurrentHeldObject == null)
                    return;

                int PreviousStack = MS.CurrentHeldObjectQuantity;
                int NewStack = ComputeNewValue(AugmentorQuantity, PreviousStack, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                MS.CurrentHeldObject.Stack = NewStack;

                MachineAugmentorsMod.LogTrace(AugmentorType.Output, AugmentorQuantity, MS.Machine, Info.RequiresInput, MS.Machine.TileLocation,
                    "HeldObject.Stack", PreviousStack, DesiredNewValue, NewStack, Effect);
            }
        }

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput)
        {
            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Output);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            if (Config.UseLinearFormula)
            {
                return 1.0 + (ActualQuantity * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;
                return 1.0 + (UpperLimit * (1.0 - Math.Exp(ActualQuantity * -1 * RateOfDecay)));
            }
        }

        public static int ComputeNewValue(int AugmentorQuantity, int PreviousValue, bool RequiresInput, out double Effect, out double ValueBeforeRandomization)
        {            
            //Formula: Effect = 1.0 + MaxEffect * (1.0 - e ^ (-x * DecayRate))
            //CurrentHeldObject.Stack *= Effect
            Effect = ComputeEffect(AugmentorQuantity, RequiresInput);
            double DesiredNewValue = PreviousValue * Effect;
            ValueBeforeRandomization = DesiredNewValue;
            return WeightedRound(DesiredNewValue);
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Output); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("OutputAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("OutputAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("OutputAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new OutputAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is OutputAugmentor; }
    }

    [XmlRoot("SpeedAugmentor", Namespace = "")]
    public class SpeedAugmentor : Augmentor
    {
        public SpeedAugmentor() : base(AugmentorType.Speed) { }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(CFAData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Speed) || AugmentorQuantity <= 0 || Info.RequiresInput || CFAData.Machine.readyForHarvest.Value)
                    return;

                int PreviousMinutes = CFAData.CurrentMinutesUntilReady;
                int NewMinutes = ComputeNewValue(AugmentorQuantity, PreviousMinutes, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                CFAData.Machine.MinutesUntilReady = NewMinutes;
                if (NewMinutes <= 0)
                    CFAData.Machine.readyForHarvest.Value = true;

                MachineAugmentorsMod.LogTrace(AugmentorType.Speed, AugmentorQuantity, CFAData.Machine, Info.RequiresInput, CFAData.Machine.TileLocation,
                    "HeldObject.MinutesUntilReady", PreviousMinutes, DesiredNewValue, NewMinutes, Effect);
            }
#endif
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Speed) || AugmentorQuantity <= 0 || !Info.RequiresInput || PODIData.Machine.readyForHarvest.Value)
                    return;

                int PreviousMinutes = PODIData.CurrentMinutesUntilReady;
                int NewMinutes = ComputeNewValue(AugmentorQuantity, PreviousMinutes, Info.RequiresInput, out double Effect, out double DesiredNewValue);

                if (PreviousMinutes != NewMinutes)
                {
                    //  Find the GameLocation of the Machine
                    //  (Possible TODO: If using Automate mod, may need to iterate all GameLocations until finding the one where GameLocation.Objects[Machine Tile Location] is the Machine)
                    GameLocation MachineLocation = null;
                    if (Game1.player.currentLocation.Objects.TryGetValue(PODIData.Machine.TileLocation, out Object PlacedMachine) && PlacedMachine == PODIData.Machine)
                        MachineLocation = Game1.player.currentLocation;

                    //  There seems to be a bug where there is no product if the machine is instantly done processing.
                    //NewMinutes = Math.Max(10, NewMinutes); // temporary fix - require at least one 10-minute processing cycle
                    //  It looks like Object.checkForAction happens right AFTER Object.performObjectDropIn, and checkForAction is setting Object.heldObject=null if Object.readyForHarvest=true
                    //  So set a flag that tells GamePatches.CheckForAction_Prefix to skip execution
                    if (NewMinutes <= 0)
                        GamePatches.SkipNextCheckForAction = true;

                    if (MachineLocation != null)
                    {
                        int Elapsed = PreviousMinutes - NewMinutes;
                        PODIData.Machine.minutesElapsed(Elapsed, MachineLocation);
                    }
                    else
                    {
                        PODIData.Machine.MinutesUntilReady = NewMinutes;
                        if (NewMinutes <= 0)
                            PODIData.Machine.readyForHarvest.Value = true;
                    }
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Speed, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "HeldObject.MinutesUntilReady", PreviousMinutes, DesiredNewValue, NewMinutes, Effect);
            }
#endif
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(MS.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Speed) || AugmentorQuantity <= 0 || MS.Machine.readyForHarvest.Value || MS.CurrentHeldObject == null)
                    return;

                int PreviousMinutes = MS.CurrentMinutesUntilReady;
                int NewMinutes = ComputeNewValue(AugmentorQuantity, PreviousMinutes, Info.RequiresInput, out double Effect, out double DesiredNewValue);

                if (PreviousMinutes != NewMinutes)
                {
                    MS.Machine.MinutesUntilReady = NewMinutes;
                    if (NewMinutes <= 0)
                        MS.Machine.readyForHarvest.Value = true;
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Speed, AugmentorQuantity, MS.Machine, Info.RequiresInput, MS.Machine.TileLocation,
                    "HeldObject.MinutesUntilReady", PreviousMinutes, DesiredNewValue, NewMinutes, Effect);
            }
        }

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput)
        {
            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Speed);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            if (Config.UseLinearFormula)
            {
                return 1.0 - (ActualQuantity * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;
                return 1.0 - (UpperLimit * (1.0 - Math.Exp(ActualQuantity * -1 * RateOfDecay)));
            }
        }

        public static int ComputeNewValue(int AugmentorQuantity, int PreviousValue, bool RequiresInput, out double Effect, out double ValueBeforeRandomization)
        {
            //Formula: Effect = 1.0 - (MaxEffect * (1.0 - e ^ (-x * DecayRate)))
            //CurrentHeldObject.MinutesUntilReady *= Effect
            Effect = ComputeEffect(AugmentorQuantity, RequiresInput);
            double DesiredNewValue = PreviousValue * Effect;
            ValueBeforeRandomization = DesiredNewValue;
            int NewValue = WeightedRound(DesiredNewValue);

            //  Round to nearest 10 since the game processes machine outputs every 10 game minutes
            //  EX: If NewValue = 38, then there is a 20% chance of rounding down to 30, 80% chance of rounding up to 40
            int SmallestDigit = NewValue % 10;
            NewValue = NewValue - SmallestDigit; // Round down to nearest 10
            if (RollDice(SmallestDigit / 10.0))
                NewValue += 10; // Round up

            return NewValue;
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Speed); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("SpeedAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("SpeedAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("SpeedAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new SpeedAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is SpeedAugmentor; }
    }

    [XmlRoot("EfficiencyAugmentor", Namespace = "")]
    public class EfficiencyAugmentor : Augmentor
    {
        public EfficiencyAugmentor() : base(AugmentorType.Efficiency) { }

        private static readonly ReadOnlyCollection<int> OreIds = new List<int>() {
            378, 380, 384, 386 // Coppre Ore, Iron Ore, Gold Ore, Iridium Ore
        }.AsReadOnly();
        public static bool IsOre(Item Item)
        {
            return Item != null && OreIds.Contains(Item.ParentSheetIndex) && Item is Object Obj && !Obj.GetType().IsSubclassOf(typeof(Object)) && !Obj.IsRecipe && !Obj.bigCraftable.Value;
        }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
            // Intentionally left blank since there are no inputs to refund during an OnProductsCollected.
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Efficiency) || AugmentorQuantity <= 0 || !Info.RequiresInput || PODIData.Input == null)
                    return;

                int PreviousAmountInserted = PODIData.PreviousInputQuantity - PODIData.CurrentInputQuantity;

                int NewAmountInserted;
                double Effect;
                double DesiredNewValue;
                if (PreviousAmountInserted <= 0)
                {
                    //  No clue why, but for some machines the game hasn't actually taken the input yet by the time Object.performObjectDropIn finishes.
                    //  so assume the input amount was = to 1 when computing the refund.
                    NewAmountInserted = ComputeNewValue(AugmentorQuantity, 1, Info.RequiresInput, out Effect, out DesiredNewValue)
                        - 1 - Math.Abs(PreviousAmountInserted); //  -1 because we assume it required at least 1 input, -PreviousInputQuantityUsed because another augmentor whose effect could have been applied first may have set the quantity to a negative value to allow saving a material
                }
                else
                {
                    NewAmountInserted = ComputeNewValue(AugmentorQuantity, PreviousAmountInserted, Info.RequiresInput, out Effect, out DesiredNewValue);
                }

                int RefundAmt = PreviousAmountInserted - NewAmountInserted;
                if (RefundAmt > 0)
                {
                    bool WasStackDepleted = PODIData.Input.Stack <= 0;
                    PODIData.Input.Stack += RefundAmt;

                    //  If Stack was set to 0 by the game, then the game would have removed it from their inventory, and so they wouldn't be able to receive the refunded quantity
                    if (WasStackDepleted && PODIData.WasInputInInventory && Game1.player.Items[PODIData.InputInventoryIndex.Value] == null)
                    {
                        Game1.player.addItemToInventory(PODIData.Input, PODIData.InputInventoryIndex.Value);
                    }
                }

                //  Refund coal when processing ores
                if (IsOre(PODIData.Input))
                {
                    double Chance = 1.0 - Effect;
                    int SpawnedQuantity = WeightedRound(Chance);
                    if (SpawnedQuantity > 0)
                    {
                        Object Coal = new Object(382, SpawnedQuantity, false, -1, 0);
                        int SpawnDirection = Randomizer.Next(4);
                        Game1.createItemDebris(Coal, PODIData.Machine.TileLocation * Game1.tileSize, SpawnDirection, null, -1);
                    }
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Efficiency, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "Input.Stack", PreviousAmountInserted, DesiredNewValue, NewAmountInserted, Effect);
            }
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {

        }

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput)
        {
            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Efficiency);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            if (Config.UseLinearFormula)
            {
                return 1.0 - (ActualQuantity * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;
                return 1.0 - (UpperLimit * (1.0 - Math.Exp(ActualQuantity * -1 * RateOfDecay)));
            }
        }

        public static int ComputeNewValue(int AugmentorQuantity, int PreviousValue, bool RequiresInput, out double Effect, out double ValueBeforeRandomization)
        {
            //Formula: Effect = MaxEffect * (1.0 - e ^ (-x * DecayRate))
            //Refunded Quantity = OriginalQuantity * Effect
            Effect = ComputeEffect(AugmentorQuantity, RequiresInput);
            double DesiredNewValue = PreviousValue * Effect;
            ValueBeforeRandomization = DesiredNewValue;
            return WeightedRound(DesiredNewValue);
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Efficiency); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("EfficiencyAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("EfficiencyAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("EfficiencyAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new EfficiencyAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is EfficiencyAugmentor; }
    }

    [XmlRoot("QualityAugmentor", Namespace = "")]
    public class QualityAugmentor : Augmentor
    {
        public QualityAugmentor() : base(AugmentorType.Quality) { }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(CFAData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Quality) || AugmentorQuantity <= 0 || Info.RequiresInput || !Info.HasQualityProducts)
                    return;

                int PreviousQuality = CFAData.CurrentHeldObject.Quality;
                int NewQuality = ComputeNewValue(AugmentorQuantity, PreviousQuality, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                CFAData.CurrentHeldObject.Quality = NewQuality;

                MachineAugmentorsMod.LogTrace(AugmentorType.Quality, AugmentorQuantity, CFAData.Machine, Info.RequiresInput, CFAData.Machine.TileLocation,
                    "HeldObject.Quality", PreviousQuality, DesiredNewValue, NewQuality, Effect);
            }
#endif
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Quality) || AugmentorQuantity <= 0 || !Info.RequiresInput)
                    return;

                if (Info.IsFurnace() && Info.TryGetUpgradedQuality(PODIData.CurrentHeldObject, out Object UpgradedObject))
                {
                    double Effect = ComputeEffect(AugmentorQuantity, Info.RequiresInput);
                    bool Success = WeightedRound(Effect) == 1;
                    if (Success)
                    {
                        PODIData.Machine.heldObject.Value = UpgradedObject;
                    }

                    MachineAugmentorsMod.LogTrace(AugmentorType.Quality, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                        "HeldObject.Quality", 0, Effect, Success ? 1 : 0, Effect);
                }
                else if (Info.HasQualityProducts)
                {
                    int PreviousQuality = PODIData.CurrentHeldObject.Quality;
                    int NewQuality = ComputeNewValue(AugmentorQuantity, PreviousQuality, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                    PODIData.CurrentHeldObject.Quality = NewQuality;

                    MachineAugmentorsMod.LogTrace(AugmentorType.Quality, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                        "HeldObject.Quality", PreviousQuality, DesiredNewValue, NewQuality, Effect);
                }
            }
#endif
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Quality) || AugmentorQuantity <= 0 || !Info.RequiresInput)
                    return;

                if (Info.IsFurnace() && Info.TryGetUpgradedQuality(PODIData.CurrentHeldObject, out Object UpgradedObject))
                {
                    double Effect = ComputeEffect(AugmentorQuantity, Info.RequiresInput);
                    bool Success = WeightedRound(Effect) == 1;
                    if (Success)
                    {
                        PODIData.Machine.heldObject.Value = UpgradedObject;
                    }

                    MachineAugmentorsMod.LogTrace(AugmentorType.Quality, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                        "HeldObject.Quality", 0, Effect, Success ? 1 : 0, Effect);
                }
            }
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(MS.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Quality) || AugmentorQuantity <= 0 || MS.Machine.readyForHarvest.Value || MS.CurrentHeldObject == null || !Info.HasQualityProducts)
                    return;

                int PreviousQuality = MS.CurrentHeldObject.Quality;
                int NewQuality = ComputeNewValue(AugmentorQuantity, PreviousQuality, Info.RequiresInput, out double Effect, out double DesiredNewValue);
                MS.CurrentHeldObject.Quality = NewQuality;

                MachineAugmentorsMod.LogTrace(AugmentorType.Quality, AugmentorQuantity, MS.Machine, Info.RequiresInput, MS.Machine.TileLocation,
                    "HeldObject.Quality", PreviousQuality, DesiredNewValue, NewQuality, Effect);
            }
        }

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput)
        {
            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Quality);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            if (Config.UseLinearFormula)
            {
                return Math.Min(0.999999, ActualQuantity * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;
                return Math.Min(0.999999, UpperLimit * (1.0 - Math.Exp(ActualQuantity * -1 * RateOfDecay)));
            }
        }

        private const int QualityRegular = 0, QualitySilver = 1, QualityGold = 2, QualityIridium = 4;
        private static readonly ReadOnlyCollection<int> ValidQualities = new List<int>() { QualityRegular, QualitySilver, QualityGold, QualityIridium }.AsReadOnly();

        public static int ComputeNewValue(int AugmentorQuantity, int PreviousValue, bool RequiresInput, out double Effect, out double ValueBeforeRandomization)
        {
            //Formula: Chance = MaxEffect * (1.0 - e ^ (-x * DecayRate))
            //If random number <= Chance, Quality incremented
            Effect = ComputeEffect(AugmentorQuantity, RequiresInput);
            double DesiredNewValue = PreviousValue + Effect;
            ValueBeforeRandomization = DesiredNewValue;
            if (PreviousValue < QualityIridium && WeightedRound(DesiredNewValue) != PreviousValue)
                return ValidQualities[ValidQualities.IndexOf(PreviousValue) + 1];
            else
                return PreviousValue;
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Quality); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("QualityAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("QualityAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("QualityAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new QualityAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is QualityAugmentor; }
    }

    [XmlRoot("ProductionAugmentor", Namespace = "")]
    public class ProductionAugmentor : Augmentor
    {
        public ProductionAugmentor() : base(AugmentorType.Production) { }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(CFAData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Production) || AugmentorQuantity <= 0 || Info.RequiresInput || CFAData.CurrentHeldObject == null)
                    return;

                //  Modify the output
                int PreviousOutputStack = CFAData.CurrentHeldObjectQuantity;
                int NewOutputStack = ComputeNewValue(AugmentorQuantity, double.MaxValue, PreviousOutputStack, Info.RequiresInput, out double OutputEffect, out double DesiredNewOutputValue);
                CFAData.CurrentHeldObject.Stack = NewOutputStack;
                MachineAugmentorsMod.LogTrace(AugmentorType.Production, AugmentorQuantity, CFAData.Machine, Info.RequiresInput, CFAData.Machine.TileLocation,
                    "HeldObject.Stack", PreviousOutputStack, DesiredNewOutputValue, NewOutputStack, OutputEffect);
            }
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Production) || AugmentorQuantity <= 0 || !Info.RequiresInput || PODIData.CurrentHeldObject == null || PODIData.Input == null)
                    return;

                //  Compute the maximum multiplier we can apply to the input and output based on how many more of the inputs the player has
                int PreviousInputQuantityUsed = PODIData.PreviousInputQuantity - PODIData.CurrentInputQuantity;
                //double MaxMultiplier = PreviousInputQuantityUsed == 0 ? int.MaxValue : Math.Abs(PODIData.PreviousInputQuantity * 1.0 / PreviousInputQuantityUsed);
                double MaxMultiplier = PreviousInputQuantityUsed == 0 ? PODIData.CurrentInputQuantity : Math.Abs(PODIData.PreviousInputQuantity * 1.0 / PreviousInputQuantityUsed);

                //  Modify the output
                int PreviousOutputStack = PODIData.CurrentHeldObjectQuantity;
                int NewOutputStack = ComputeNewValue(AugmentorQuantity, MaxMultiplier, PreviousOutputStack, Info.RequiresInput, out double OutputEffect, out double DesiredNewOutputValue);
                PODIData.CurrentHeldObject.Stack = NewOutputStack;
                MachineAugmentorsMod.LogTrace(AugmentorType.Production, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "HeldObject.Stack", PreviousOutputStack, DesiredNewOutputValue, NewOutputStack, OutputEffect);

                //  Modify the input
                int CurrentInputQuantityUsed;
                double InputEffect;
                double DesiredNewInputValue;
                if (PreviousInputQuantityUsed <= 0)
                {
                    //  No clue why, but for some machines the game hasn't actually taken the input yet by the time Object.performObjectDropIn finishes.
                    //  so assume the input amount was = to 1.
                    CurrentInputQuantityUsed = ComputeNewValue(AugmentorQuantity, MaxMultiplier, 1, Info.RequiresInput, out InputEffect, out DesiredNewInputValue)
                        - 1 - Math.Abs(PreviousInputQuantityUsed); //  -1 because we assume it required at least 1 input, -PreviousInputQuantityUsed because EfficiencyAugmentor may have set the quantity to a negative value to allow saving a material
                }
                else
                {
                    CurrentInputQuantityUsed = ComputeNewValue(AugmentorQuantity, MaxMultiplier, PreviousInputQuantityUsed, Info.RequiresInput, out InputEffect, out DesiredNewInputValue);
                }
                int NewInputStack = PODIData.PreviousInputQuantity - CurrentInputQuantityUsed;
                PODIData.Input.Stack = NewInputStack;
                if (NewInputStack <= 0)
                {
                    if (PODIData.WasInputInInventory)
                        Game1.player.removeItemFromInventory(PODIData.Input);
                    else
                    {
                        PODIData.Input.Stack = 1; // Just a failsafe to avoid glitched out Items with zero quantity, such as if the input came from a chest due to the Automate mod
                    }
                }

                ////  Modify the input
                //int CurrentInputQuantityUsed = ComputeNewValue(AugmentorQuantity, MaxMultiplier, PreviousInputQuantityUsed, Info.RequiresInput, out double InputEffect, out double DesiredNewInputValue);
                //int NewInputStack = PODIData.PreviousInputQuantity - CurrentInputQuantityUsed;
                //PODIData.Input.Stack = NewInputStack;
                //if (NewInputStack <= 0)
                //{
                //    if (PODIData.WasInputInInventory)
                //        Game1.player.removeItemFromInventory(PODIData.Input);
                //    else
                //    {
                //        PODIData.Input.Stack = 1; // Just a failsafe to avoid glitched out Items with zero quantity, such as if the input came from a chest due to the Automate mod
                //    }
                //}

                MachineAugmentorsMod.LogTrace(AugmentorType.Production, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "Input-UsedAmount", PreviousInputQuantityUsed, DesiredNewInputValue, CurrentInputQuantityUsed, InputEffect);
            }
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {

        }

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput)
        {
            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Production);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            if (Config.UseLinearFormula)
            {
                return 1.0 + (ActualQuantity * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;
                return 1.0 + (UpperLimit * (1.0 - Math.Exp(ActualQuantity * -1 * RateOfDecay)));
            }
        }

        public static int ComputeNewValue(int AugmentorQuantity, double MaxEffect, int PreviousValue, bool RequiresInput, out double Effect, out double ValueBeforeRandomization)
        {
            //Formula: Effect = 1.0 + MaxEffect * (1.0 - e ^ (-x * DecayRate))
            //CurrentHeldObject.Stack *= Effect
            //RequiredInputs *= Effect

            Effect = Math.Min(MaxEffect, ComputeEffect(AugmentorQuantity, RequiresInput));
            double DesiredNewValue = PreviousValue * Effect;
            ValueBeforeRandomization = DesiredNewValue;
            return WeightedRound(DesiredNewValue);
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Production); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("ProductionAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("ProductionAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("ProductionAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new ProductionAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is ProductionAugmentor; }
    }

    [XmlRoot("DuplicationAugmentor", Namespace = "")]
    public class DuplicationAugmentor : Augmentor
    {
        public DuplicationAugmentor() : base(AugmentorType.Duplication) { }

        public static void OnProductsCollected(CheckForActionData CFAData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(CFAData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Duplication) || AugmentorQuantity <= 0 || Info.RequiresInput || CFAData.Machine.readyForHarvest)
                    return;

                bool Success = SpawnDuplicate(AugmentorQuantity, CFAData.CurrentMinutesUntilReady, Info.RequiresInput, out double Chance);
                if (Success)
                {
                    if (CFAData.Machine.getOne() is Object Duplicate)
                    {
                        Duplicate.Stack = 1;
                        int SpawnDirection = Randomizer.Next(4);
                        Game1.createItemDebris(Duplicate, Game1.player.getStandingPosition(), SpawnDirection, null, -1);
                    }
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Duplication, AugmentorQuantity, CFAData.Machine, Info.RequiresInput, CFAData.Machine.TileLocation,
                    "CreateDuplicate", 0, Chance, Convert.ToInt32(Success), Chance);
            }
#endif
        }

        public static void OnInputsInserted(PerformObjectDropInData PODIData, int AugmentorQuantity)
        {
#if LEGACYCODE
            if (MachineInfo.TryGetMachineInfo(PODIData.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Duplication) || AugmentorQuantity <= 0 || !Info.RequiresInput || PODIData.Machine.readyForHarvest)
                    return;

                bool Success = SpawnDuplicate(AugmentorQuantity, PODIData.CurrentMinutesUntilReady, Info.RequiresInput, out double Chance);
                if (Success)
                {
                    if (PODIData.Machine.getOne() is Object Duplicate)
                    {
                        Duplicate.Stack = 1;
                        int SpawnDirection = Randomizer.Next(4);
                        Game1.createItemDebris(Duplicate, Game1.player.getStandingPosition(), SpawnDirection, null, -1);
                    }
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Duplication, AugmentorQuantity, PODIData.Machine, Info.RequiresInput, PODIData.Machine.TileLocation,
                    "CreateDuplicate", 0, Chance, Convert.ToInt32(Success), Chance);
            }
#endif
        }

        public static void OnMinutesUntilReadySet(MachineState MS, int AugmentorQuantity)
        {
            if (MachineInfo.TryGetMachineInfo(MS.Machine, out MachineInfo Info))
            {
                if (!Info.AttachableAugmentors.Contains(AugmentorType.Duplication) || AugmentorQuantity <= 0 || MS.Machine.readyForHarvest.Value || MS.CurrentHeldObject == null)
                    return;

                bool Success = SpawnDuplicate(AugmentorQuantity, MS.CurrentMinutesUntilReady, Info.RequiresInput, out double Chance);
                if (Success)
                {
                    if (MS.Machine.getOne() is Object Duplicate)
                    {
                        Duplicate.Stack = 1;
                        int SpawnDirection = Randomizer.Next(4);
                        Game1.createItemDebris(Duplicate, Game1.player.getStandingPosition(), SpawnDirection, null, -1);
                    }
                }

                MachineAugmentorsMod.LogTrace(AugmentorType.Duplication, AugmentorQuantity, MS.Machine, Info.RequiresInput, MS.Machine.TileLocation,
                    "CreateDuplicate", 0, Chance, Convert.ToInt32(Success), Chance);
            }
        }

        public const int MinutesPerDay = 60 * 24;

        public static double ComputeEffect(int AugmentorQuantity, bool RequiresInput, int ProcessingTime)
        {
            double ProcessingDays = ProcessingTime * 1.0 / MinutesPerDay;
            double BaseDaysPerDuplicate = RequiresInput ? MachineAugmentorsMod.UserConfig.DaysPerStandardDuplicate : MachineAugmentorsMod.UserConfig.DaysPerInputlessDuplicate;

            AugmentorConfig Config = MachineAugmentorsMod.UserConfig.GetConfig(AugmentorType.Duplication);
            int ActualQuantity = Math.Max(0, Math.Min(AugmentorQuantity, Config.MaxAttachmentsPerMachine));
            double UpperLimit = RequiresInput ? Config.MaxEffectPerStandardMachine : Config.MaxEffectPerInputlessMachine;
            double RateOfDecay = RequiresInput ? Config.StandardDecayRate : Config.InputlessDecayRate;

            double Multiplier;
            if (Config.UseLinearFormula)
            {
                Multiplier = ((ActualQuantity - 1) * 1.0 / Config.MaxAttachmentsPerMachine * UpperLimit);
            }
            else
            {
                Multiplier = UpperLimit * (1.0 - Math.Exp((ActualQuantity - 1) * -1 * RateOfDecay));
            }

            double DuplicateChance = Math.Min(1.0, ProcessingDays / (BaseDaysPerDuplicate * (1.0 - Multiplier)));
            return DuplicateChance;
        }

        public static bool SpawnDuplicate(int AugmentorQuantity, int ProcessingTime, bool RequiresInput, out double Chance)
        {
            //Formula: Effect = MaxEffect * (1.0 - e ^ (-(x - 1) * DecayRate))
            //DuplicateChance = (MinutesUntilReady / MinutesPerDay) / (DaysPerDuplicate * (1.0 - Effect))
            Chance = ComputeEffect(AugmentorQuantity, RequiresInput, ProcessingTime);
            return RollDice(Chance);
        }

        public override bool IsAugmentable(Object Item) { return MachineInfo.TryGetMachineInfo(Item, out MachineInfo Info) && Info.AttachableAugmentors.Contains(AugmentorType.Duplication); }
        public override Color GetPrimaryIconColor() { return new Color(1f, 1f, 1f, 1f); }
        public override int GetPurchasePrice() { return AugmentorConfig.BasePrice; }
        public override int GetSellPrice() { return GetPurchasePrice() / 2; }
        public override string GetDisplayName() { return MachineAugmentorsMod.Translate("DuplicationAugmentorName"); }
        public override string GetDescription() { return MachineAugmentorsMod.Translate("DuplicationAugmentorDescription"); }
        public override string GetEffectDescription() { return MachineAugmentorsMod.Translate("DuplicationAugmentorEffectDescription"); }
        public override Augmentor CreateSingle() { return new DuplicationAugmentor(); }
        public override bool CanStackWith(ISalable Other) { return Other is DuplicationAugmentor; }
    }
}
