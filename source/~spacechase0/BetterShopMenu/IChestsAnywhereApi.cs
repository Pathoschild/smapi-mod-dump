/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.ChestsAnywhere
{
    /// <summary>The Chests Anywhere API which other mods can access.</summary>
    public interface IChestsAnywhereApi
    {
        /// <summary>Get whether the chest overlay is currently visible on top of the current menu. In split-screen mode, this is for the current screen being updated/rendered.</summary>
        bool IsOverlayActive();

        /// <summary>Get whether the chest overlay is currently blocking input to the underlying menu (e.g. a dropdown or the options form is open). In split-screen mode, this is for the current screen being updated/rendered.</summary>
        bool IsOverlayModal();
    }
}
