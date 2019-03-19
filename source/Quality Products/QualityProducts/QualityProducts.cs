using System.Collections.Generic;
using Microsoft.Xna.Framework;
using QualityProducts.Cooking;
using QualityProducts.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace QualityProducts
{
    /// <summary>
    /// Mod entry.
    /// </summary>
    internal class QualityProducts : Mod
    {
        /****************
         * Public methods
         ****************/

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            Helper.Events.Display.MenuChanged += OnCrafting;
            Helper.Events.GameLoop.Saved += OnSaved;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.World.LocationListChanged += OnLoadLocation;
            Helper.Events.World.ObjectListChanged += OnPlacingProcessor;
        }


        /*****************
         * Internal fields
         ******************/

        internal static QualityProducts Instance { get; private set; }

        internal readonly Dictionary<GameLocation, List<Processor>> locationProcessors = new Dictionary<GameLocation, List<Processor>>(new ObjectReferenceComparer<GameLocation>());


        /*****************
         * Private methods
         *****************/

        /// <summary>
        /// Places the objects in the specified game location.
        /// </summary>
        /// <param name="gameLocation">Game location.</param>
        /// <param name="objects">Objects.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private void PlaceObjects<T>(GameLocation gameLocation, List<T> objects) where T : SObject
        {
            foreach (T @object in objects)
            {
                Monitor.VerboseLog($"Placing {@object.Name} at {gameLocation.Name}({@object.TileLocation.X},{@object.TileLocation.Y})");
                gameLocation.setObject(@object.TileLocation, @object);
            }
        }

        /// <summary>
        /// Save loaded event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            locationProcessors.Clear();
        }

        /// <summary>
        /// Location list changed event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnLoadLocation(object sender, LocationListChangedEventArgs e)
        {
            foreach (GameLocation gameLocation in e.Added)
            {
                Monitor.VerboseLog($"Loading {gameLocation.Name}");

                if (!locationProcessors.ContainsKey(gameLocation))
                {
                    locationProcessors.Add(gameLocation, new List<Processor>());
                }

                List<Processor> processors = new List<Processor>();
                foreach (SObject @object in gameLocation.Objects.Values)
                {
                    if (@object.bigCraftable.Value && Processor.GetProcessorType(@object.ParentSheetIndex) != null && !(@object is Processor))
                    {
                        Processor processor = Processor.FromObject(@object);
                        processors.Add(processor);
                    }
                }

                PlaceObjects(gameLocation, processors);
            }
        }

        /// <summary>
        /// Saving event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
                Monitor.VerboseLog($"Unloading {kv.Key.Name}");

                List<SObject> objects = new List<SObject>();
                foreach (SObject @object in kv.Key.Objects.Values)
                {
                    if (@object is Processor processor)
                    {
                        kv.Value.Add(processor);
                        SObject objectClone = processor.ToObject();
                        objects.Add(objectClone);
                    }
                }

                PlaceObjects(kv.Key, objects);
            }
        }

        /// <summary>
        /// Saved event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
                PlaceObjects(kv.Key, kv.Value);
                kv.Value.Clear();
            }
        }

        /// <summary>
        /// Object list changed event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnPlacingProcessor(object sender, ObjectListChangedEventArgs e)
        {
            List<Processor> processors = new List<Processor>();
            foreach (KeyValuePair<Vector2, SObject> kv in e.Added)
            {
                if (!(kv.Value is Processor))
                {
                    Processor processor = Processor.FromObject(kv.Value);
                    if (processor != null)
                    {
                        processors.Add(processor);
                    }
                }
            }

            PlaceObjects(e.Location, processors);
        }

        /***
         * From https://github.com/spacechase0/CookingSkill/blob/162be2dd01f2fb728f2e375f83152fe67f4da811/Mod.cs
         ***/
        /// <summary>
        /// Menu changed event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnCrafting(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CraftingPage menu)
            {
                bool cooking = Helper.Reflection.GetField<bool>(menu, "cooking").GetValue();
                Game1.activeClickableMenu = new ModdedCraftingPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height, cooking);
            }
        }
    }
}
