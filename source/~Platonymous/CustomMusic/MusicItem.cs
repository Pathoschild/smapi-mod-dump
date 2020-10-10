/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace CustomMusic
{
    public class MusicItem
    {
        public string Id { get; set; }
        public string File { get; set; }
        public bool Loop { get; set; } = true;
        public bool Ambient { get; set; } = false;
        public bool Preload { get; set; } = false;
        public string Conditions { get; set; } = "";

        public MusicItem()
        {

        }

        public MusicItem(string id, string file, bool ambient, bool loop, bool preload, string conditions)
        {
            this.Id = id;
            this.File = file;
            this.Loop = loop;
            this.Ambient = ambient;
            this.Preload = preload;
            this.Conditions = conditions;
        }
    }
}
