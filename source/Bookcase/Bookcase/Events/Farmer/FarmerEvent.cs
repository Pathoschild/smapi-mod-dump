/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

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