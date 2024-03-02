/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.LittleHelpersCore.Framework.Commands;
using NeverToxic.StardewMods.LittleHelpersCore.Framework.Validators;
using System.Collections.Generic;

namespace NeverToxic.StardewMods.LittleHelpersCore.Framework.Buildings
{
    internal interface IBuilding
    {
        List<BaseCommand> Commands { get; set; }

        List<int> Tiles { get; }

        int? Radius { get; set; }

        int HelperCapacity { get; set; }

        int? Location { get; set; }

        ILocationValidator LocationValidator { get; set; }

        void ExecuteCommands();
    }
}
