using System;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

namespace AutoQualityPatch
{
    /***
     * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/GenericObjectMachine.cs
     * and  https://github.com/Pathoschild/StardewMods/tree/38b2fe127f2fdb2ad669841b15b755940512abf8/Automate#extensibility-for-modders
     ***/
    /// <summary>A generic automatable processor instance.</summary>
    public abstract class AutomatableBase : IMachine
    {
        /*********
         * Fields 
         *********/

        /// <summary>Underlying Stardew Valley object.</summary>
        private readonly SObject Entity;


        /*************
         * Properties 
         *************/

        /// <summary>Get the GameLocation of the entity.</summary>
        /// <value>The location.</value>
        public GameLocation Location { get; }

        /// <summary>Get the total tile area occupied by the entity.</summary>
        /// <value>The tile area.</value>
        public Rectangle TileArea { get; }

        /// <summary>Recipe list for the entity</summary>
        /// <value>An array with all possible recipes.</value>
        protected abstract IRecipe[] Recipes { get; }


        /*****************
         * Public methods
         *****************/
        
        /// <summary>Get the machine's processing state.</summary>
        public MachineState GetState()
        {
            if (Entity.heldObject.Value == null)
                return MachineState.Empty;

            return Entity.readyForHarvest.Value
                ? MachineState.Done : MachineState.Processing;
        }

        /// <summary>Get the output item.</summary>
        public ITrackedStack GetOutput()
        {
            return new TrackedItem(Entity.heldObject.Value, onEmpty: item =>
            {
                Entity.heldObject.Value = null;
                Entity.readyForHarvest.Value = false;
            });
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public bool SetInput(IStorage input)
        {
            if (input.TryGetIngredient(Recipes, out IConsumable consumable, out IRecipe recipe))
            {
                Entity.heldObject.Value = recipe.Output(consumable.Take());
                Entity.heldObject.Value.Quality = (consumable.Consumables.Sample as SObject).Quality;
                Entity.MinutesUntilReady = recipe.Minutes;
                return true;
            }
            return false;
        }


        /********************
         * Protected methods 
         ********************/
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="location">Location.</param>
        /// <param name="tile">Tile.</param>
        protected AutomatableBase(SObject entity, GameLocation location, Vector2 tile)
        {
            Entity = entity;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }


        /********************
         * Nested classes 
         ********************/

        /***
         * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Recipe.cs
         ***/
        /// <summary>Describes a generic recipe based on item input and output.</summary>
        protected class Recipe : IRecipe
        {
            /*********
            ** Accessors
            *********/
            /// <summary>The input item or category ID.</summary>
            public int InputID { get; }

            /// <summary>The number of inputs needed.</summary>
            public int InputCount { get; }

            /// <summary>The output to generate (given an input).</summary>
            public Func<Item, SObject> Output { get; }

            /// <summary>The time needed to prepare an output.</summary>
            public int Minutes { get; }


            /*********
            ** Public methods
            *********/
            /// <summary>Construct an instance.</summary>
            /// <param name="input">The input item or category ID.</param>
            /// <param name="inputCount">The number of inputs needed.</param>
            /// <param name="output">The output to generate (given an input).</param>
            /// <param name="minutes">The time needed to prepare an output.</param>
            public Recipe(int input, int inputCount, Func<Item, SObject> output, int minutes)
            {
                InputID = input;
                InputCount = inputCount;
                Output = output;
                Minutes = minutes;
            }

            /// <summary>Get whether the recipe can accept a given item as input (regardless of stack size).</summary>
            /// <param name="stack">The item to check.</param>
            public bool AcceptsInput(ITrackedStack stack)
            {
                return stack.Sample.ParentSheetIndex == InputID || stack.Sample.Category == InputID;
            }
        }
    }
}
