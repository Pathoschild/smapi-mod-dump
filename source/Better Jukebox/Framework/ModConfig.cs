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
    public class ModConfig
    {
        /// <summary>
        /// Show the menu. When disabled, only permanent list settings will apply.
        /// </summary>
        public bool ShowMenu { get; set; } = true;

        // List settings

        /// <summary>
        /// Addresses ambient and other non-musical tracks normally hidden from the player.
        /// `0` performs no removal, `1` uses the default removal, and `2` removes additional non-music items not removed normally,
        /// whether due to bugs or mod interference. Applied after adding unheard tracks. Does not affect vanilla random choice.
        /// </summary>
        public int AmbientTracks { get; set; } = 1;

        /// <summary>
        /// Comma-separated list of music cues to hide from the list in addition to the above.
        /// </summary>
        public string Blacklist { get; set; } = "";
        /// <summary>
        /// Comma-separated list of music cues to unhide from the list in addition to the above. Does not add tracks, only applies if heard or added with `ShowUnheardTracks`.
        /// </summary>
        public string Whitelist { get; set; } = "";

        /// <summary>
        /// Shows locked icons in place of unheard songs from the soundtrack. Ignored if `ShowUnheardTracks` is enabled.
        /// </summary>
        public bool ShowLockedSongs { get; set; } = false;

        /// <summary>
        /// Songs not yet heard on the current save file can be found in the jukebox.
        /// WARNING: Will permanently add songs to the farmer's "heard songs" list when played in the Saloon.
        /// </summary>
        public bool ShowUnheardTracks { get; set; } = false;

        // Changes which unheard tracks will be added.
        public bool UnheardSoundtrack { get; set; } = true;
        public bool UnheardNamed { get; set; } = true;
        public bool UnheardRandom { get; set; } = false;
        public bool UnheardMisc { get; set; } = false;
        public bool UnheardDupes { get; set; } = false;
        public bool UnheardMusical { get; set; } = false;

        /// <summary>
        /// Any selected unheard tracks are added to the farmer's "heard songs" list. This allows the
        /// default jukebox "random" to choose these songs. This will be permanent after the game is saved.
        /// </summary>
        public bool PermanentUnheard { get; set; } = false;

        /// <summary>
        /// Comma-separated list of music cues to remove from the farmer's "heard songs" list. Applied on loading the save. May be undone by hearing the song again.
        /// </summary>
        public string PermanentBlacklist { get; set; } = "";

        // Functional settings

        /// <summary>
        /// Replace random function with menu items instead of only heard songs, but lose automatic shuffle on warp.
        /// </summary>
        public bool TrueRandom { get; set; } = false;

        /// <summary>
        /// Non-default sorting options are enabled.
        /// </summary>
        public bool ShowAlternateSorts { get; set; } = true;

        // Visual settings

        /// <summary>
        /// Internal music identifiers are displayed alongside the regular music name.
        /// </summary>
        public bool ShowInternalId { get; set; } = false;

        /// <summary>
        /// Replaces (sometimes incorrect) track names with the full Bandcamp names.
        /// </summary>
        public bool ShowBandcampNames { get; set; } = true;
    }
}
