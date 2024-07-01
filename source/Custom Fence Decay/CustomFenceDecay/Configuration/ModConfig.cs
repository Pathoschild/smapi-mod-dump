/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JeanSebGwak/CustomFenceDecay
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomFenceDecay.Configuration
{
    public sealed class ModConfig
    {
        /// <summary>
        /// Flag that indicate if all fences types have the same decaying speed
        /// Default: true
        /// </summary>
        public bool IdenticalValue { get; set; } = true;

        /// <summary>
        /// Fences decaying speed, in percent
        /// Used when <see cref="IdenticalValue"/> is set to true OR in case of unrecognized fence (Custom fence by modder for example)
        /// Default: 50%
        /// </summary>
        public int FenceDecaySpeedInPercent { get; set; } = 50;

        /// <summary>
        /// Wood fences decaying speed, in percent
        /// Only used when <see cref="IdenticalValue"/> is set to false
        /// Default: 50%
        /// </summary>
        public int WoodFenceDecaySpeedInPercent { get; set; } = 50;

        /// <summary>
        /// Stone fences decaying speed, in percent
        /// Only used when <see cref="IdenticalValue"/> is set to false
        /// Default: 50%
        /// </summary>
        public int StoneFenceDecaySpeedInPercent { get; set; } = 50;

        /// <summary>
        /// Iron fences decaying speed, in percent
        /// Only used when <see cref="IdenticalValue"/> is set to false
        /// Default: 50%
        /// </summary>
        public int IronFenceDecaySpeedInPercent { get; set; } = 50;

        /// <summary>
        /// Hardwood fences decaying speed, in percent
        /// Only used when <see cref="IdenticalValue"/> is set to false
        /// Default: 50%
        /// </summary>
        public int HardwoodFenceDecaySpeedInPercent { get; set; } = 50;
    }
}
