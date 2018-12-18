using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.StardewSymphony.Framework.SongsProcessor
{
    public enum SongState
    {
        /// <summary>The song is currently playing.</summary>
        Playing,

        /// <summary>The song is currently paused.</summary>
        Paused,

        /// <summary>The song is currently stopped.</summary>
        Stopped
    }
}
