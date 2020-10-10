/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using System;

namespace TheChestDimension
{
    class TCDRules
    {
        public string CanOnlyEnterFrom { get; set; } // if not empty, implies CanEnterFromCave = false
        public bool CanEnterFromCave { get; set; } = true;

        public TCDRules()
        {
        }

        public TCDRules(ModConfig config)
        {
            CanOnlyEnterFrom = config.CanOnlyEnterFrom;
            CanEnterFromCave = config.CanEnterFromCave;
        }
    }
}