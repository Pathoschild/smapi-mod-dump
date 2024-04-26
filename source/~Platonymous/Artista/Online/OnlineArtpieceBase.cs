/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace Artista.Online
{
    public class OnlineArtpieceBase
    {
        public string id { get; set; }

        public string artwork { get; set; }

        public bool active { get; set; } = true;

        public string collection { get; set; }

        public int downloads { get; set; }

        public string author { get; set; }

        public bool won { get; set; } = false;
    }
}
