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
using System.Collections.Generic;

namespace TreatYourAnimals
{
    class CharacterTreatedData
    {
        public List<string> Characters = new List<string>();

        public string FormatEntry(string type, string id)
        {
            return String.Join("_", new string[] { type, id });
        }
    }
}
