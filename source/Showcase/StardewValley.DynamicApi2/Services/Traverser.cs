using System;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class Traverser
    {
        private static readonly Lazy<Traverser> Lazy = new Lazy<Traverser>(() => new Traverser());
        public static Traverser Instance => Lazy.Value;
        private Traverser() { }

        public void TraverseLocations(Action<GameLocation> processLocation)
        {
            foreach (var location in Game1.locations)
            {
                processLocation(location);
                if (!(location is BuildableGameLocation buildableLocation)) continue;

                foreach (var building in buildableLocation.buildings)
                {
                    var indoorLocation = building.indoors;
                    if (indoorLocation == null) continue;
                    processLocation(indoorLocation);
                }
            }
        }

        public void TraverseLocation(GameLocation location, Func<Object, Object> processObject)
        {
            if (location == null) return;
            var objectInfos = location.Objects.ToList();
            
            foreach (var objectInfo in objectInfos)
            {
                var oldObject = objectInfo.Value;
                var newObject = processObject(oldObject);
                TraverseChest(newObject as Chest, processObject);
                if (oldObject != newObject)
                {
                    location.Objects[objectInfo.Key] = newObject;
                }
            }

            if (!(location is DecoratableLocation decoratableLocation)) return;
            for (var i = 0; i < decoratableLocation.furniture.Count; ++i)
            {
                decoratableLocation.furniture[i] = (Furniture)processObject(decoratableLocation.furniture[i]);
            }

            if (!(decoratableLocation is FarmHouse farmHouse)) return;
            TraverseChest(farmHouse.fridge, processObject);           
        }

        public void TraverseInventory(Farmer farmer, Func<Object, Object> processObject)
        {
            if (farmer == null) return;
            for (var i = 0; i < farmer.Items.Count; ++i)
            {
                if (!(farmer.Items[i] is Object inventoryObject)) continue;
                farmer.Items[i] = processObject(inventoryObject);
            }
        }

        private void TraverseChest(Chest chest, Func<Object, Object> processObject)
        {
            if (chest == null) return;
            for (var i = 0; i < chest.items.Count; ++i)
            {
                if (!(chest.items[i] is Object chestObject)) continue;
                chest.items[i] = processObject(chestObject);
            }
        }
    }
}