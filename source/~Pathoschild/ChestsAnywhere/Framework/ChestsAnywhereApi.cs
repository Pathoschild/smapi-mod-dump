/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays;

namespace Pathoschild.Stardew.ChestsAnywhere.Framework
{
    /// <inheritdoc />
    public class ChestsAnywhereApi : IChestsAnywhereApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get the current chest UI overlay, if any.</summary>
        private readonly Func<IStorageOverlay?> GetOverlay;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="getOverlay">Get the current chest UI overlay, if any.</param>
        internal ChestsAnywhereApi(Func<IStorageOverlay?> getOverlay)
        {
            this.GetOverlay = getOverlay;
        }

        /// <inheritdoc />
        public bool IsOverlayActive()
        {
            return this.GetOverlay() != null;
        }

        /// <inheritdoc />
        public bool IsOverlayModal()
        {
            return this.GetOverlay()?.ActiveElement is not (null or Element.Menu);
        }
    }
}
