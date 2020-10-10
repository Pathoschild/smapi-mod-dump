/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using System;

namespace BetterFarmAnimalVariety.Models
{
    class Command
    {
        public string Name;
        public string Documentation;
        public Action<string, string[]> Callback;

        public Command(string name, string documentation, Action<string, string[]> callback)
        {
            this.Name = name;
            this.Documentation = documentation;
            this.Callback = callback;
        }
    }
}
