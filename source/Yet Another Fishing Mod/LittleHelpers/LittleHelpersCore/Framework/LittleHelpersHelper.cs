/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.LittleHelpersCore.Framework.Buildings;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.LittleHelpersCore.Framework
{
    internal class LittleHelpersHelper
    {
        public List<BaseBuilding> Buildings { get; set; } = [];

        public void OnDayChanged()
        {
            foreach (BaseBuilding building in this.Buildings)
                building.ExecuteCommands();
        }
    }
}
