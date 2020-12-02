/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace ContentPatcher.Framework.Conditions
{
    /// <summary>Indicates how an image should be patched.</summary>
    public enum PatchImageMode
    {
        /// <summary>Erase the original content within the area before drawing the new content.</summary>
        Replace = PatchMode.Replace,

        /// <summary>Draw the new content over the original content, so the original content shows through any transparent pixels.</summary>
        Overlay = PatchMode.Overlay
    }
}
