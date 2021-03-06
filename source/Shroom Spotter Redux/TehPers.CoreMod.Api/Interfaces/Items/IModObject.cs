/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public interface IModObject : IModItem {
        /// <summary>Gets the raw information that should be added to "Data/ObjectInformation".</summary>
        /// <returns>The raw information string.</returns>
        string GetRawObjectInformation();
    }
}