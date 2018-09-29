using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager
{
    /// <summary>
    /// Interface used for common sound functionality;
    /// </summary>
    public interface Sound
    {
        /// <summary>
        /// Handles playing a sound.
        /// </summary>
        void play();
        /// <summary>
        /// Handles pausing a song.
        /// </summary>
        void pause();
        /// <summary>
        /// Handles stopping a song.
        /// </summary>
        void stop();
        /// <summary>
        /// Handles restarting a song.
        /// </summary>
        void restart();
        /// <summary>
        /// Handles getting a clone of the song.
        /// </summary>
        /// <returns></returns>
        Sound clone();
    }
}
