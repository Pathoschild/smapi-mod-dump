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
    public class SoundItem : MusicItem
    {
          public SoundItem()
        {
            Loop = false;
        }

        public SoundItem(string id, string file, bool ambient, bool loop, bool preload, string conditions)
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