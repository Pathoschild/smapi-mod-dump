/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

namespace Bookcase.Registration {

    /// <summary>
    /// This class serves as a default implementation of IIdentifiable. It provides common ID logic.
    /// </summary>
    public class Identifiable : IIdentifiable {

        /// <summary>
        /// A property that holds the identifier for the object. Please don't abuse public set.
        /// </summary>
        public Identifier Identifier { get; set; }
    }
}
