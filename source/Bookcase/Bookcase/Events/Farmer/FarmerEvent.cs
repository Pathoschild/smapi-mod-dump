using StardewValley;

namespace Bookcase.Events {

    /// <summary>
    /// This is a parent class that should be used for all bookcase farmer events.
    /// </summary>
    public abstract class FarmerEvent : Event {

        /// <summary>
        /// The farmer that the event is for.
        /// </summary>
        public Farmer Farmer { get; private set; }

        public FarmerEvent(Farmer farmer) {

            this.Farmer = farmer;
        }
    }
}