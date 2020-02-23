using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots.Config
{
    class ConfigItem
    {
        public List<Item> WhatCanBeFound { get; set; }
        public List<Item> WhatTrashCanBeFound { get; set; }

        public ConfigItem(List<Item> whatCanBeFound, List<Item> whatTrashCanBeFound)
        {
            WhatCanBeFound = whatCanBeFound;
            WhatTrashCanBeFound = whatTrashCanBeFound;
        }
    }
}
