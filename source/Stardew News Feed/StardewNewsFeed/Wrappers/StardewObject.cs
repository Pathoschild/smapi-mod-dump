using System;
using Netcode;
using Object = StardewValley.Object;

namespace StardewNewsFeed.Wrappers {
    public class StardewObject : IStardewObject {

        private readonly Object _object;

        public StardewObject(object obj) {
            if(obj is Object) {
                _object = obj as Object;
            } else {
                throw new ArgumentException($"{nameof(obj)} is not a valid StardewValley.Object");
            }
        }

        public bool IsReadyForHarvest() {
            return (_object.readyForHarvest == new NetBool(true))
                || _object.isAnimalProduct(); // animal products laying around are always ready for harvest
        }
    }
}
