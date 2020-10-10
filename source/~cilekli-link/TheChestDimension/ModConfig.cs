/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChestDimension
{
    class ModConfig
    {
        public SButton TCDKey { get; set; } = SButton.H;
        public SButton SetSpawnKey { get; set; } = SButton.J;

        public bool ShowEntryMessage { get; set; } = false;
        public string EntryMessage { get; set; } = "Welcome to the Chest Dimension. Press {TCDkey} to exit.";

        public string CanOnlyEnterFrom { get; set; } // if not empty, implies CanEnterFromCave = false
        public bool CanEnterFromCave { get; set; } = true;
        public bool ShowCannotEnterMessage { get; set; } = true;
        public string CannotEnterMessage { get; set; } = "You cannot enter the Chest Dimension from this location.";
    }
}
