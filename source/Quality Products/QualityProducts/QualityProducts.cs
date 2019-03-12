using System.Collections.Generic;
using Microsoft.Xna.Framework;
using QualityProducts.Menus;
using QualityProducts.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace QualityProducts
{
    public class QualityProducts : Mod
    {
        internal static QualityProducts Instance { get; private set; }

        internal readonly Dictionary<GameLocation, List<Processor>> locationProcessors = new Dictionary<GameLocation, List<Processor>>(new ObjectReferenceComparer<GameLocation>());

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

        private void PlaceObjects<T>(GameLocation gameLocation, List<T> objects) where T : SObject
        {
            foreach (T @object in objects)
            {
                gameLocation.setObject(@object.TileLocation, @object);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            locationProcessors.Clear();
        }

        private void OnLoadLocation(object sender, LocationListChangedEventArgs e)
        {
            foreach (GameLocation gameLocation in e.Added)
            {
                if (!locationProcessors.ContainsKey(gameLocation))
                {
                    locationProcessors.Add(gameLocation, new List<Processor>());
                }

                List<Processor> processors = new List<Processor>();
                foreach (SObject @object in gameLocation.Objects.Values)
                {
                    if (@object.bigCraftable.Value && Processor.WhichProcessor(@object.ParentSheetIndex) != null && !(@object is Processor))
                    {
                        Processor processor = Processor.FromObject(@object);
                        processors.Add(processor);
                    }
                }

                PlaceObjects(gameLocation, processors);
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
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

        private void OnSaved(object sender, SavedEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
                PlaceObjects(kv.Key, kv.Value);
                kv.Value.Clear();
            }
        }

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
