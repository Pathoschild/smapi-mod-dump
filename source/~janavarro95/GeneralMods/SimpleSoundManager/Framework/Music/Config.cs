using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager.Framework.Music
{
    /// <summary>
    /// Config class for the mod.
    /// </summary>
    public class Config
    {
        public bool EnableDebugLog;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Config()
        {
            EnableDebugLog = false;
        }

    }
}
