/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkbenchAnywhere.Utils
{
    public static class InputUtils
    {
        public static SButton ParseButton(string buttonString, IMonitor monitor = null)
        {
            if (string.IsNullOrWhiteSpace(buttonString) || !Enum.TryParse(buttonString, out SButton button))
            {
                monitor?.Log($"Could not parse {buttonString} as a valid button", LogLevel.Warn);
                return SButton.None;
            }

            return button;
        }
    }
}
