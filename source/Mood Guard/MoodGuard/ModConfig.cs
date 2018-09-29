using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodGuard
{
    [Serializable]
    public class ModConfig
    {
        /// <summary>
        /// Whether animals' happiness should increase when inside after 6pm.
        /// </summary>
        public NightFixConfig NightFix = new NightFixConfig();

        public ProfessionFixConfig ProfessionFix = new ProfessionFixConfig();
    }
}
