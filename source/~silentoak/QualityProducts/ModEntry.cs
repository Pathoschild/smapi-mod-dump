/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using QualityProducts.Cooking;
using SilentOak.Patching;
using SilentOak.QualityProducts.API;
using SilentOak.QualityProducts.Patches.BetterMeadIcons;
using SilentOak.QualityProducts.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts
{
    /// <summary>
    /// Mod entry.
    /// </summary>
    internal class ModEntry : Mod
    {
        /*********
         * Fields
         *********/

        /// <summary>The list of processors for each game location.</summary>
        internal readonly IDictionary<GameLocation, IList<Processor>> locationProcessors = new Dictionary<GameLocation, IList<Processor>>(new ObjectReferenceComparer<GameLocation>());


        /*************
         * Properties      
         *************/

        /// <summary>The mod configuration from the player.</summary>
        internal static QualityProductsConfig Config { get; set; }


        /*****************
         * Public methods
         *****************/

        /// <summary>
        /// Entry for this mod.
        /// </summary>
        /// <param name="helper">Helper.</param>
        public override void Entry(IModHelper helper)
        {
            Util.Init(Helper, Monitor);

            Config = Helper.ReadConfig<QualityProductsConfig>();

            if (Config.IsAnythingEnabled())
            {
                if (Config.IsCookingEnabled())
                {
                    Helper.Events.Display.MenuChanged += OnCrafting;
                }

                Helper.Events.GameLoop.Saved += OnSaved;
                Helper.Events.GameLoop.Saving += OnSaving;
                Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
                Helper.Events.World.LocationListChanged += OnLoadLocation;
                Helper.Events.World.ObjectListChanged += OnPlacingProcessor;

                if (Config.EnableMeadTextures && SpriteLoader.Init(Helper, Monitor, Config))
                {
                    PatchManager.ApplyAll(
                        typeof(SObjectDrawPatch),
                        typeof(SObjectDraw2Patch),
                        typeof(SObjectDrawInMenuPatch),
                        typeof(SObjectDrawWhenHeld),
                        typeof(FurnitureDrawPatch)
                    );
                }
            }
        }

        /// <summary>
        /// Gets the API.
        /// </summary>
        /// <returns>The API.</returns>
        public override object GetApi()
        {
            return new QualityProductsAPI(Config);
        }


        /*****************
         * Private methods
         *****************/

        /// <summary>
        /// Places the objects in the specified game location.
        /// </summary>
        /// <param name="gameLocation">Game location.</param>
        /// <param name="objects">Objects.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private void PlaceObjects<T>(GameLocation gameLocation, IList<T> objects) where T : SObject
        {
            foreach (T @object in objects)
            {
                Monitor.VerboseLog($"Placing `{@object.Name}`:{@object.GetType().Name} at {gameLocation.Name}({@object.TileLocation.X},{@object.TileLocation.Y})");
                gameLocation.setObject(@object.TileLocation, @object);
            }
        }

        /// <summary>
        /// Checks if the given object should be replaced with a processor.
        /// </summary>
        /// <returns><c>true</c>, if the object should be replaced, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to check.</param>
        /// <param name="location">Location of the object.</param>
        /// <param name="processor">Processor to replace the object with.</param>
        private bool ShouldReplaceWithProcessor(SObject @object, out Processor processor)
        {
            if (@object != null && (bool)@object.bigCraftable && !(@object is Processor))
            {
                processor = Processor.FromObject(@object);
                if (processor != null && Config.IsEnabled(processor))
                {
                    return true;
                }
            }

            processor = null;
            return false;
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
                    if (ShouldReplaceWithProcessor(@object, out Processor processor))
                    {
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
            foreach (KeyValuePair<GameLocation, IList<Processor>> kv in locationProcessors)
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
            foreach (KeyValuePair<GameLocation, IList<Processor>> kv in locationProcessors)
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
            IList<Processor> processors = new List<Processor>();
            foreach (KeyValuePair<Vector2, SObject> kv in e.Added)
            {
                if (ShouldReplaceWithProcessor(kv.Value, out Processor processor))
                {
                    processors.Add(processor);
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
                Monitor.VerboseLog("Cooking menu opened. Swapping to custom cooking menu...");
                bool cooking = Helper.Reflection.GetField<bool>(menu, "cooking").GetValue();
                Game1.activeClickableMenu = new ModdedCraftingPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height, cooking);
            }
        }
    }
}
