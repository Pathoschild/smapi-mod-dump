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
    /// This event is fired when a player starts their day. There is a pre and post version of this event.
    /// </summary>
    public class FarmerStartDayEvent : FarmerEvent {

        public FarmerStartDayEvent(Farmer farmer) : base(farmer) {

        }
    }
}