using System;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services.Internal
{
    public sealed class Traverser
    {
        #region Singleton Access

        private Traverser() { }

        private static Traverser _instance;

        public static Traverser Instance => _instance ?? (_instance = new Traverser());

        #endregion

        #region	Public Methods

        public void TraverseLocations(Action<GameLocation> processLocation)
        {
            foreach (var location in Game1.locations)
            {
                processLocation(location);

                var buildableLocation = location as BuildableGameLocation;
                if (buildableLocation == null) continue;

                foreach (var building in buildableLocation.buildings)
                {
                    var indoorLocation = building.indoors;
                    if (indoorLocation == null) continue;
                    processLocation(indoorLocation);
                }
            }
        }

        public void TraverseLocationLight(GameLocation location, Func<Object, Object> processObject)
        {
            if (location == null) return;
            var objectInfos = location.Objects.ToList();
            foreach (var objectInfo in objectInfos)
            {
                var oldObject = objectInfo.Value;
                var newObject = processObject(oldObject);
                if (oldObject != newObject)
                {
                    location.Objects[objectInfo.Key] = newObject;
                }
            }
        }

        public void TraverseLocationDeep(GameLocation location, Func<Object, Object> processObject)
        {
            if (location == null) return;
            var objectInfos = location.Objects.ToList();
            if (location is FarmHouse) TraverseChest(((FarmHouse)location).fridge, processObject);
            foreach (var objectInfo in objectInfos)
            {
                var oldObject = objectInfo.Value;
                var newObject = processObject(oldObject);
                if (newObject.heldObject != null)
                {
                    newObject.heldObject = processObject(newObject.heldObject);
                }
                TraverseChest(newObject as Chest, processObject);
                if (oldObject != newObject)
                {
                    location.Objects[objectInfo.Key] = newObject;
                }
            }
        }

        public void TraverseInventory(Farmer farmer, Func<Object, Object> processObject)
        {
            if (farmer == null) return;
            for (var i = 0; i < farmer.Items.Count; ++i)
            {
                var inventoryObject = farmer.Items[i] as Object;
                if (inventoryObject == null) continue;
                farmer.Items[i] = processObject(inventoryObject);
            }
            if (farmer.ActiveObject != null)
            {
                farmer.ActiveObject = processObject(farmer.ActiveObject);
            }
        }

        #endregion

        #region	Auxiliary Methods

        private void TraverseChest(Chest chest, Func<Object, Object> processObject)
        {
            if (chest == null) return;
            for (var i = 0; i < chest.items.Count; ++i)
            {
                var chestObject = chest.items[i] as Object;
                if (chestObject == null) continue;
                chest.items[i] = processObject(chestObject);
            }
        }

        #endregion
    }
}