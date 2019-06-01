using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondTheValleyExpansion.Framework.Config
{
    class AlchemyConfig
    {
        /// <summary> If alchemy features are enabled</summary>
        public bool Enabled { get; set; } = true;
        public Dictionary<string, int> PropertyMultiplier { get; set; } = new Dictionary<string, int>()
        {
            ["Potency"] = 1,
            ["Density"] = 1,
            ["Growth"] = 1,
            ["Fire"] = 1,
            ["Water"] = 1,
            ["Purity"] = 1
        };
    }
}
