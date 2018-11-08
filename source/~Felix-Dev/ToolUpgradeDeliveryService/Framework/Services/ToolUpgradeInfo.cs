using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ToolUpgradeDeliveryService.Framework
{
    internal class ToolUpgradeInfo
    {
        public Type ToolType { get; }

        public int Level { get; }

        public ToolUpgradeInfo(Type toolType, int level)
        {
            ToolType = toolType ?? throw new ArgumentNullException(nameof(toolType));
            Level = level;
        }
    }
}
