/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace BNWCore
{
    abstract class MapGrabber
    {
        protected ModEntry Mod { get; set; }
        protected GameLocation Location { get; set; }
        protected List<KeyValuePair<Vector2, SObject>> GrabberPairs { get; set; }
        protected IEnumerable<SObject> Grabbers
        {
            get
            {
                return GrabberPairs.Select(pair => pair.Value);
            }
        }
        protected Farmer Player
        {
            get
            {
                return Game1.MasterPlayer;
            }
        }
        public MapGrabber(ModEntry mod, GameLocation location)
        {
            Mod = mod;
            Location = location;
            GrabberPairs = location.Objects.Pairs.Where(pair => IsValidGrabber(pair.Value, pair.Key)).ToList();
        }
        protected bool TryAddItem(Item item, IEnumerable<KeyValuePair<Vector2, SObject>> grabbers)
        {
            if (item == null || item.Stack < 1) return false;
            var prevStack = item.Stack;
            foreach (var pair in grabbers)
            {
                if (IsValidGrabber(pair.Value, pair.Key))
                {
                    item = AddItemToGrabberChest(pair.Value, item);
                    if (item == null) return true;
                }
            }
            return item.Stack < prevStack;
        }
        protected bool TryAddItem(Item item)
        {
            return TryAddItem(item, GrabberPairs);
        }
        protected bool TryAddItems(IEnumerable<Item> items, IEnumerable<KeyValuePair<Vector2, SObject>> grabbers)
        {
            var successful = false;
            foreach (var item in items)
            {
                successful = TryAddItem(item, grabbers) || successful;
            }
            return successful;
        }
        protected bool TryAddItems(IEnumerable<Item> items)
        {
            return TryAddItems(items, GrabberPairs);
        }
        protected void GainExperience(int skill, int exp)
        {
            
            Player.gainExperience(skill, 1);

        }
        public bool CanGrab()
        {
            return GrabberPairs.Select(pair => IsValidGrabber(pair.Value, pair.Key)).Any(x => x);
        }
        public Dictionary<InventoryEntry, int> GetInventory()
        {
            var inventory = new Dictionary<InventoryEntry, int>();
            foreach (var pair in GrabberPairs)
            {
                if (!IsValidGrabber(pair.Value, pair.Key)) continue;
                var grabber = pair.Value;
                var chest = grabber.heldObject.Value as Chest;
                foreach (var item in chest.items.Where(i => i != null))
                {
                    var entry = new InventoryEntry(item);
                    if (inventory.ContainsKey(entry))
                    {
                        inventory[entry] += item.Stack;
                    }
                    else
                    {
                        inventory.Add(entry, item.Stack);
                    }
                }
            }
            return inventory;
        }
        public abstract bool GrabItems();
        private Item AddItemToGrabberChest(SObject grabber, Item item)
        {
            var chest = grabber.heldObject.Value as Chest;
            var leftoverItem = chest.addItem(item);
            if (chest.items.Where(i => i != null).Any())
            {
                grabber.showNextIndex.Value = true;
            }
            return leftoverItem;
        }
        private bool IsValidGrabber(SObject obj, Vector2 tile)
        {
            if (Location.Objects.ContainsKey(tile))
            {
                return obj.ParentSheetIndex == ItemIds.Autograbber &&
                obj.heldObject.Value != null &&
                obj.heldObject.Value is Chest;
            }
            else
            {
                return false;
            }
        }
    }
    abstract class ObjectsMapGrabber : MapGrabber
    {
        protected List<KeyValuePair<Vector2, SObject>> Objects { get; set; }
        public ObjectsMapGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
            Objects = location.Objects.Pairs.ToList();
        }
        public abstract bool GrabObject(Vector2 tile, SObject obj);
        public override bool GrabItems()
        {
            return Objects.Select(pair => GrabObject(pair.Key, pair.Value)).Aggregate(false, (grabbed, next) => grabbed || next);
        }
    }
    abstract class TerrainFeaturesMapGrabber : MapGrabber
    {
        protected List<KeyValuePair<Vector2, TerrainFeature>> Features { get; set; }
        public TerrainFeaturesMapGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
            Features = location.terrainFeatures.Pairs
                .Concat(location.largeTerrainFeatures
                    .Select(ft => new KeyValuePair<Vector2, TerrainFeature>(ft.tilePosition.Value, ft))
                ).ToList();
        }
        public abstract bool GrabFeature(Vector2 tile, TerrainFeature feature);
        public override bool GrabItems()
        {
            return Features.Select(pair => GrabFeature(pair.Key, pair.Value)).Aggregate(false, (grabbed, next) => grabbed || next);

        }
    }
}
