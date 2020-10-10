/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace PelicanTTS
{
    public class ModConfig
    {
        public float Pitch { get; set; } = 0;
        public float Volume { get; set; } = 1;
        public bool MumbleDialogues { get; set; } = false;
        public bool Greeting { get; set; } = true;

        public bool ReadDialogues { get; set; } = true;

        public bool ReadNonCharacterMessages{ get; set; } = true;

        public bool ReadLetters { get; set; } = true;

        public bool ReadHudMessages { get; set; } = true;

       // public bool ReadChatMessages { get; set; } = true;

        public int Rate { get; set; } = 100;

        public Dictionary<string, VoiceSetup> Voices { get; set; } = new Dictionary<string, VoiceSetup>();
    }
}
