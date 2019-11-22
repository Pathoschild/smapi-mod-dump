using StardewValley;
using StardewValley.Events;

namespace Bookcase.Events {

    /// <summary>
    /// This event is fired when a new nightime farm event is selected. This event can be used to change the event that is chozen, or add new events.
    /// 
    /// This event can be canceled. Canceling will result in no farm event taking place.
    /// </summary>
    public class SelectFarmEvent : Event {

        /// <summary>
        /// The original farm event that would have happened. This can be null.
        /// </summary>
        public FarmEvent OriginalEvent { get; private set; }

        /// <summary>
        /// The actual farm event that will be selected.
        /// </summary>
        public FarmEvent SelectedEvent { get; set; }

        public SelectFarmEvent(FarmEvent original) {

            this.OriginalEvent = original;
            this.SelectedEvent = original;
        }

        public override bool CanCancel() {

            return true;
        }
    }
}
