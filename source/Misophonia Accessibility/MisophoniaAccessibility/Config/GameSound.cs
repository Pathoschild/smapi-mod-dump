/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility.Config
{
    public class GameSound : Attribute
    {
        public string CodeName { get; private set; }
        public string DisplayName { get; private set; }

        public GameSound(string codeName, string displayName)
        {
            CodeName = codeName;
            DisplayName = displayName;
        }
    }
}
