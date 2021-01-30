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
    /// Interface used to define an identifiable object. Intended for use in Bookcase registries.
    /// </summary>
    public interface IIdentifiable {

        /// <summary>
        /// Property for the id. Implementation is up to implementer.
        /// </summary>
        Identifier Identifier { get; set; }
    }
}