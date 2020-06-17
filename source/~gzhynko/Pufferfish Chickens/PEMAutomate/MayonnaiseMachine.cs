using Harmony;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using SObject = StardewValley.Object;

namespace PEMAutomate
{
    public class AutomatePatchOverrides
    {
        [HarmonyPriority(800)]
        public static void GetFor(ref object __result, SObject obj)
        {
            string fullName = __result?.GetType().FullName;
            if (fullName != "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine")
                return;
            __result = (object)new VanillaProducerMachine((IMachine)__result);
        }
    }

    public class VanillaProducerMachine : IMachine, IAutomatable
    {
        public IMachine OriginalMachine;
        public IMachine CustomMachine;

        public string MachineTypeID { get; }

        public GameLocation Location { get; }

        public Rectangle TileArea { get; }

        public VanillaProducerMachine(IMachine originalMachine)
        {
            this.Location = ((IAutomatable)originalMachine).Location;
            this.TileArea = ((IAutomatable)originalMachine).TileArea;
            this.OriginalMachine = originalMachine;
            SObject machine = ((IReflectedProperty<SObject>)ModEntry.instance.Helper.Reflection.GetProperty<SObject>((object)originalMachine, "Machine", true)).GetValue();
            this.CustomMachine = (IMachine)new CustomProducerMachine(machine, ((IAutomatable)originalMachine).Location, machine.TileLocation);
            this.MachineTypeID = "Vanilla." + machine.Name;
        }

        public MachineState GetState()
        {
            return this.CustomMachine.GetState();
        }

        public ITrackedStack GetOutput()
        {
            return this.CustomMachine.GetOutput();
        }

        public bool SetInput(IStorage input)
        {
            return this.CustomMachine.SetInput(input) || this.OriginalMachine.SetInput(input);
        }
    }

    internal class CustomProducerMachine : IMachine
    {
        private readonly SObject _machine;

        public string MachineTypeID { get; }

        public GameLocation Location { get; }

        private readonly IRecipe[] Recipes =
        {
            // pufferfish or large pufferfish egg => pufferfish mayonnaise (either gold or normal quality) 
            new Recipe(
                input: PufferEggsToMayonnaise.ModEntry.instance.eggID,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, PufferEggsToMayonnaise.ModEntry.instance.mayoID, null, false, true, false, false),
                minutes: 180
            ),
            new Recipe(
                input: PufferEggsToMayonnaise.ModEntry.instance.lEggID,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, PufferEggsToMayonnaise.ModEntry.instance.mayoID, null, false, true, false, false) { Quality = SObject.highQuality },
                minutes: 180
            )
        };

        public Rectangle TileArea { get; }

        public CustomProducerMachine(SObject machine, GameLocation location, Vector2 tile)
        {
            this.MachineTypeID = "Custom." + machine.Name;
            this._machine = machine;
            this.Location = location;
            this.TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            if (this._machine.heldObject.Value == null)
                return MachineState.Empty;

            return this._machine.readyForHarvest.Value
                ? MachineState.Done
                : MachineState.Processing;
        }

        public ITrackedStack GetOutput()
        {
            return new TrackedItem(this._machine.heldObject.Value, onEmpty: item =>
            {
                this._machine.heldObject.Value = null;
                this._machine.readyForHarvest.Value = false;
            });
        }

        public bool SetInput(IStorage input)
        {
            return GenericPullRecipe(input, this.Recipes, out _);
        }

        protected bool GenericPullRecipe(IStorage storage, IRecipe[] recipes, out Item input)
        {
            if (storage.TryGetIngredient(recipes, out IConsumable consumable, out IRecipe recipe))
            {
                input = consumable.Take();
                this._machine.heldObject.Value = recipe.Output(input);
                this._machine.MinutesUntilReady = recipe.Minutes(input);
                return true;
            }

            input = null;
            return false;
        }
    }

    internal class Recipe : IRecipe
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item type to accept, or <c>null</c> to accept any.</summary>
        public ItemType? Type { get; } = ItemType.Object;

        /// <summary>The input item or category ID.</summary>
        public int InputID { get; }

        /// <summary>The number of inputs needed.</summary>
        public int InputCount { get; }

        /// <summary>The output to generate (given an input).</summary>
        public Func<Item, SObject> Output { get; }

        /// <summary>The time needed to prepare an output (given an input).</summary>
        public Func<Item, int> Minutes { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="input">The input item or category ID.</param>
        /// <param name="inputCount">The number of inputs needed.</param>
        /// <param name="output">The output to generate (given an input).</param>
        /// <param name="minutes">The time needed to prepare an output.</param>
        public Recipe(int input, int inputCount, Func<Item, SObject> output, int minutes)
            : this(input, inputCount, output, _ => minutes) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="input">The input item or category ID.</param>
        /// <param name="inputCount">The number of inputs needed.</param>
        /// <param name="output">The output to generate (given an input).</param>
        /// <param name="minutes">The time needed to prepare an output (given an input).</param>
        public Recipe(int input, int inputCount, Func<Item, SObject> output, Func<Item, int> minutes)
        {
            this.InputID = input;
            this.InputCount = inputCount;
            this.Output = output;
            this.Minutes = minutes;
        }

        /// <summary>Get whether the recipe can accept a given item as input (regardless of stack size).</summary>
        /// <param name="stack">The item to check.</param>
        public bool AcceptsInput(ITrackedStack stack)
        {
            return
                (this.Type == null || stack.Type == this.Type)
                && (stack.Sample.ParentSheetIndex == this.InputID || stack.Sample.Category == this.InputID);
        }
    }


    internal class MayonnaiseAutomationFactory : IAutomationFactory
    {
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            return obj.Name.Contains("Pufferfish Egg") ? (IAutomatable)new CustomProducerMachine(obj, location, tile) : (IAutomatable)null;
        }

        public IAutomatable GetFor(
          TerrainFeature feature,
          GameLocation location,
          in Vector2 tile)
        {
            return (IAutomatable)null;
        }

        public IAutomatable GetFor(
          Building building,
          BuildableGameLocation location,
          in Vector2 tile)
        {
            return (IAutomatable)null;
        }

        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            return (IAutomatable)null;
        }

        IAutomatable IAutomationFactory.GetFor(
          StardewValley.Object obj,
          GameLocation location,
          in Vector2 tile)
        {
            return this.GetFor(obj, location, in tile);
        }

        IAutomatable IAutomationFactory.GetFor(
          TerrainFeature feature,
          GameLocation location,
          in Vector2 tile)
        {
            return this.GetFor(feature, location, in tile);
        }

        IAutomatable IAutomationFactory.GetFor(
          Building building,
          BuildableGameLocation location,
          in Vector2 tile)
        {
            return this.GetFor(building, location, in tile);
        }

        IAutomatable IAutomationFactory.GetForTile(
          GameLocation location,
          in Vector2 tile)
        {
            return this.GetForTile(location, in tile);
        }
    }
}
