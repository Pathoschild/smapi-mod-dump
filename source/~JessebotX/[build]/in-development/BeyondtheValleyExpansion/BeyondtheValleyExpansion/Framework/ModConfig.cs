using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondTheValleyExpansion.Framework.Config;

namespace BeyondTheValleyExpansion.Framework
{
    class ModConfig
    {
        /// <summary> If alchemy features are enabled by the player </summary>
        public AlchemyConfig Alchemy { get; set; } = new AlchemyConfig();
    }
}
