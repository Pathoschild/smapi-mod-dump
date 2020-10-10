/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/


namespace PelicanTTS
{
    class SpeechConfig
    {
        public string name { get; set; }
        public string voicename { get; set; }

        public SpeechConfig(string name, string voicename)
        {
            this.name = name;
            this.voicename = voicename;
        }

    }
}
