using System;
using System.Linq;
using Netcode;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewNewsFeed.Wrappers {
    interface IAmHarvestable {
        bool readyForHarvest { get; }
    }
    
    public class StardewObject : IStardewObject {
        private readonly object _object;
        private const int AutoGrabberSheetIndex = 165;

        public StardewObject(object obj) {
            _object = obj;
        }

        public bool IsReadyForHarvest() {
            if (_object is HoeDirt hoeDirt) {
                return hoeDirt.readyForHarvest();
            }

            if (!(_object is Object)) {
                throw new ArgumentException($"{nameof(_object)} is not a valid StardewValley.Object");
            }

            var stardewObject = (Object) _object;
            var itemIsReadyForHarvest = (stardewObject.readyForHarvest == new NetBool(true))
                || stardewObject.isAnimalProduct(); // animal products laying around are always ready for harvest
            if (itemIsReadyForHarvest) {
                return true;
            }

            var itemIsAutoGrabber = stardewObject.ParentSheetIndex == AutoGrabberSheetIndex;
            if (!itemIsAutoGrabber) {
                return false;
            }

            var chest = stardewObject.heldObject.Value as Chest;
            return chest?.items?.Any() ?? false;
        }
    }
}
