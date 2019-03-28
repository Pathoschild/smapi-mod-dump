using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Extensions;
using SilentOak.QualityProducts.Processors;
using SilentOak.QualityProducts.Utils;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts
{
    /// <summary>
    /// An entity that is capable of processing items into products.
    /// </summary>
    public abstract class Processor : SObject
    {
        /*********
         * Types
         ********/

        /// <summary>
        /// The available processor types.
        /// </summary>
        public enum ProcessorTypes
        {
            Keg = 12,
            PreservesJar = 15,
            CheesePress = 16,
            Loom = 17,
            OilMaker = 19,
            MayonnaiseMachine = 24
        }


        /*************
         * Properties
         *************/

        /// <summary>
        /// Gets the type of the processor.
        /// </summary>
        /// <value>The type of the processor.</value>
        public ProcessorTypes ProcessorType => (ProcessorTypes)ParentSheetIndex;

        /// <summary>
        /// Gets the available recipes for this entity.
        /// </summary>
        /// <value>The recipes.</value>
        public abstract IEnumerable<Recipe> Recipes { get; }

        /// <summary>
        /// Gets or sets the current recipe.
        /// </summary>
        /// <value>The current recipe.</value>
        private Recipe CurrentRecipe { get; set; }


        /****************
         * Public methods
         ****************/

        /// <summary>
        /// Gets the type of the processor for the corresponding index.
        /// </summary>
        /// <returns>The processor type.</returns>
        /// <param name="parentSheetIndex">Parent sheet index.</param>
        public static ProcessorTypes? GetProcessorType(int parentSheetIndex)
        {
            if (Enum.IsDefined(typeof(ProcessorTypes), parentSheetIndex))
            {
                return (ProcessorTypes)Enum.ToObject(typeof(ProcessorTypes), parentSheetIndex);
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of the specified processor type.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="processorType">Processor type to be instantiated.</param>
        public static Processor Create(ProcessorTypes processorType)
        {
            switch (processorType)
            {
                case ProcessorTypes.Keg:
                    return new Keg();
                case ProcessorTypes.PreservesJar:
                    return new PreservesJar();
                case ProcessorTypes.CheesePress:
                    return new CheesePress();
                case ProcessorTypes.Loom:
                    return new Loom();
                case ProcessorTypes.OilMaker:
                    return new OilMaker();
                case ProcessorTypes.MayonnaiseMachine:
                    return new MayonnaiseMachine();
                default:
                    throw new UnimplementedCaseException($"Enum value {Enum.GetName(typeof(ProcessorTypes), processorType)} of {typeof(ProcessorTypes)} has no corresponding case");
            }
        }

        /// <summary>
        /// Creates a new instance of the specified processor type, initializing it with the specified initializer.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="processorType">Processor type to be instantiated.</param>
        /// <param name="initializer">Initializer.</param>
        public static Processor Create(ProcessorTypes processorType, Action<Processor> initializer)
        {
            Processor newObj = Create(processorType);
            initializer(newObj);
            return newObj;
        }

        /// <summary>
        /// Creates a processor instance based on the specified Stardew Valley object.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="object">Reference object.</param>
        public static Processor FromObject(SObject @object)
        {
            if (!@object.bigCraftable.Value)
            {
                return null;
            }

            ProcessorTypes? processorType = GetProcessorType(@object.ParentSheetIndex);
            if (processorType != null) {
                Processor processor = Create(processorType.Value,
                p => 
                {
                    p.TileLocation = @object.TileLocation;
                    p.IsRecipe = (bool)@object.isRecipe;
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

        /// <summary>
        /// Creates a new regular Stardew Valley object with the same attributes as this one.  
        /// </summary>
        /// <returns>The new object.</returns>
        public SObject ToObject()
        {
            SObject @object = new SObject(TileLocation, ParentSheetIndex)
            {
                IsRecipe = IsRecipe,
                Name = Name,
                DisplayName = DisplayName,
                Scale = Scale,
                MinutesUntilReady = MinutesUntilReady
            };

            @object.owner.Value = owner.Value;
            @object.heldObject.Value = heldObject.Value;
            @object.readyForHarvest.Value = readyForHarvest.Value;

            return @object;
        }

        /***
         * Modified from StardewValley.Object.performObjectDropInAction
         **/
        /// <summary>
        /// Performs the object drop in action.
        /// </summary>
        /// <returns><c>true</c>, if object drop in action was performed, <c>false</c> otherwise.</returns>
        /// <param name="dropInItem">Drop in item.</param>
        /// <param name="probe">If set to <c>true</c> probe.</param>
        /// <param name="who">Who.</param>
        public sealed override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (dropInItem is SObject @object)
            {
                if (heldObject.Value != null)
                {
                    return false;
                }

                if ((bool)@object.bigCraftable)
                {
                    return false;
                }

                if (!probe && @object != null && heldObject.Value == null)
                {
                    scale.X = 5f;
                }

                /* 
                 * The base method will be called only if the object to be
                 * inserted is not an SObject, or a recipe for it was found but
                 * it was disabled in the config.
                 */
                if (!TryForRecipe(@object, out Recipe recipe))
                {
                    return false;
                }

                if (!ModEntry.Config.IsEnabled(recipe, this))
                {
                    Util.Monitor.VerboseLog($"{recipe.Name} is disabled; fallback to default behaviour.");
                    return base.performObjectDropInAction(dropInItem, probe, who);
                }

                if (probe)
                {
                    // awful, but it's what vanilla SDV does, so must be done for compatibility with other mods.
                    heldObject.Value = recipe.Process(@object); 
                    return true;
                }

                if (PerformProcessing(@object, who, recipe))
                {
                    Util.Monitor.VerboseLog($"Inserted {@object.DisplayName} (quality {@object.Quality}) into {Name} @({TileLocation.X},{TileLocation.Y})");
                    Util.Monitor.VerboseLog($"{Name} @({TileLocation.X},{TileLocation.Y}) is producing {heldObject.Value.DisplayName} (quality {heldObject.Value.Quality})");
                    return true;
                }

                return false;
            }

            return base.performObjectDropInAction(dropInItem, probe, who);
        }

        /***
         * Modified from StardewValley.Object.checkForAction
         ***/
        /// <summary>
        /// Checks for action, executing if available when <paramref name="justCheckingForActivity"/> is false.
        /// </summary>
        /// <returns><c>true</c>, if action was performed, <c>false</c> otherwise.</returns>
        /// <param name="who">Farmer that requested for action.</param>
        /// <param name="justCheckingForActivity">If set to <c>true</c>, doesn't execute any available action.</param>
        public sealed override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (!justCheckingForActivity && who != null
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1)
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1)
                && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY())
                && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY())
                && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
            {
                performToolAction(null, who.currentLocation);
            }

            if ((bool)readyForHarvest)
            {
                if (justCheckingForActivity)
                {
                    return true;
                }

                if (who.IsLocalPlayer)
                {
                    SObject value2 = heldObject.Value;
                    heldObject.Value = null;
                    if (!who.addItemToInventoryBool(value2, false))
                    {
                        heldObject.Value = value2;
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                        return false;
                    }
                    Game1.playSound("coin");
                    UpdateStats(value2);
                }

                heldObject.Value = null;
                readyForHarvest.Value = false;
                showNextIndex.Value = false;
                return true;
            }
            return false;
        }

        /***
         * Modified from StardewValley.Object.addWorkingAnimation
         **/
        /// <summary>
        /// Adds this entity's working animation to its location.
        /// </summary>
        /// <param name="environment">Game location.</param>
        public sealed override void addWorkingAnimation(GameLocation environment)
        {
            /* 
             * If not doing anything, then the recipe was disabled
             * and it should fall back on the game logic
             */
            if (CurrentRecipe == null)
            {
                base.addWorkingAnimation(environment);
                return;
            }

            if (environment != null && environment.farmers.Count != 0)
            {
                AddWorkingEffects(environment);
            }
        }


        /*******************
         * Protected methods
         *******************/

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processor"/> of the given type.
        /// </summary>
        /// <param name="processorType">Processor type.</param>
        /// <param name="location">Where the entity is.</param>
        protected Processor(ProcessorTypes processorType) : base(Vector2.Zero, (int)processorType, false)
        {
        }

        /// <summary>
        /// Updates the game stats.
        /// </summary>
        /// <param name="object">Previously held object.</param>
        protected virtual void UpdateStats(SObject @object)
        {
            return; 
        }

        /// <summary>
        /// Executes if recipe doesn't specify any input effects
        /// </summary>
        protected virtual void DefaultInputEffects(GameLocation location)
        {
            location.playSound("Ship");
        }

        /// <summary>
        /// Executes if recipe doesn't specify any working effects
        /// </summary>
        protected virtual void DefaultWorkingEffects(GameLocation location)
        {
            return;
        }


        /******************
         * Private methods
         ******************/

        /// <summary>
        /// Checks if item is ingredient of an available recipe, setting <see cref="CurrentRecipe"/> accordingly.
        /// </summary>
        /// <returns><c>true</c>, if found a recipe with the given ingredient, <c>false</c> otherwise.</returns>
        /// <param name="object">Object.</param>
        private bool TryForRecipe(SObject @object, out Recipe foundRecipe)
        {
            foundRecipe = Recipes.FirstOrDefault(recipe => recipe.AcceptsInput(@object));
            return foundRecipe != null;
        }


        /// <summary>
        /// Performs item processing.
        /// </summary>
        /// <returns><c>true</c> if started processing, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to be processed.</param>
        /// <param name="who">Farmer that initiated processing.</param>
        private bool PerformProcessing(SObject @object, Farmer who, Recipe recipe)
        {
            int amount = recipe.GetAmount(@object);
            if (amount > @object.Stack)
            {
                recipe.FailAmount();
                return false;
            }

            if (amount > 1)
            {
                @object.Stack -= amount - 1;
                if (@object.Stack <= 0)
                {
                    who.removeItemFromInventory(@object);
                }
            }

            CurrentRecipe = recipe;
            heldObject.Value = CurrentRecipe.Process(@object);
            minutesUntilReady.Value = CurrentRecipe.Minutes;

            /* Both of these need to be below the CurrentRecipe assignment above. */
            AddInputEffects(who.currentLocation);
            AddWorkingEffects(who.currentLocation);

            return true;
        }

        /// <summary>
        /// Adds this entity's input animation to its location.
        /// Assumes <see cref="CurrentRecipe"/> is not null.
        /// </summary>
        private void AddInputEffects(GameLocation location)
        {
            if (CurrentRecipe.InputEffects != null)
            {
                CurrentRecipe.InputEffects(location, TileLocation);
            }
            else
            {
                DefaultInputEffects(location);
            }
        }

        /// <summary>
        /// Adds this entity's working animation to its location.
        /// Assumes <see cref="CurrentRecipe"/> is not null.
        /// </summary>
        private void AddWorkingEffects(GameLocation location)
        {
            if (CurrentRecipe.WorkingEffects != null)
            {
                CurrentRecipe.WorkingEffects(location, TileLocation);
            }
            else
            {
                DefaultWorkingEffects(location);
            }
        }

    }
}
