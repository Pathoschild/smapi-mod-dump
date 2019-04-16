using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    class ConfigItem
    {
        public List<Item> WhatCanBeFound { get; set; } = new List<Item>();
        public List<Item> WhatTrashCanBeFound { get; set; } = new List<Item>();

        public ConfigItem(List<Item> whatCanBeFound, List<Item> whatTrashCanBeFound)
        {
            WhatCanBeFound = whatCanBeFound;
            WhatTrashCanBeFound = whatTrashCanBeFound;
        }
    }
}
