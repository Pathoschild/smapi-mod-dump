/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/icepuente/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace HorseWhistle.Models
{
    internal class ModConfigModel
    {
        public bool EnableGrid { get; set; } = false;
        public bool EnableWhistleAudio { get; set; } = true;
        public SButton EnableGridKey { get; set; } = SButton.G;
        public SButton TeleportHorseKey { get; set; } = SButton.V;
    }
}