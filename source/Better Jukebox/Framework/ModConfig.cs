/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

namespace Gaphodil.BetterJukebox.Framework
{
    class ModConfig
    {
        /// <summary>
        /// Whether internal music identifiers are displayed alongside the regular music name.
        /// </summary>
        public bool ShowInternalID { get; set; } = false;

        /// <summary>
        /// Whether ambience, sound effects, and other permanently disabled tracks show up in the jukebox.
        /// </summary>
        public bool ShowAmbientTracks { get; set; } = false;

        /// <summary>
        /// Whether only songs heard on the current save file can be found in the jukebox.
        /// WARNING: may add songs to your "heard songs" list.
        /// </summary>
        //public bool ShowUnheardTracks { get; set; } = false;
    }
}
