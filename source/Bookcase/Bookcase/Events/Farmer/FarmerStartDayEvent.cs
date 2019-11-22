using StardewValley;

namespace Bookcase.Events {

    /// <summary>
    /// This event is fired when a player starts their day. There is a pre and post version of this event.
    /// </summary>
    public class FarmerStartDayEvent : FarmerEvent {

        public FarmerStartDayEvent(Farmer farmer) : base(farmer) {

        }
    }
}