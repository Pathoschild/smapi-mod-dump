/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace SimpleSoundManager.Framework
{
    /// <summary>Interface used for common sound functionality;</summary>
    public interface Sound
    {
        /// <summary>Handles playing a sound.</summary>
        void play();

        void play(float volume);

        /// <summary>Handles pausing a song.</summary>
        void pause();

        /// <summary>Handles stopping a song.</summary>
        void stop();

        /// <summary>Handles restarting a song.</summary>
        void restart();

        /// <summary>Handles getting a clone of the song.</summary>
        Sound clone();

        string getSoundName();

        bool isStopped();
    }
}
