using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility.Config
{
    public class ModConfig
    {
        /// <summary>
        /// Sets whether this mod is enabled, default true
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Object containing which sounds should be disabled
        /// </summary>
        public Sounds SoundsToDisable { get; set; } = new Sounds();
    }
}
