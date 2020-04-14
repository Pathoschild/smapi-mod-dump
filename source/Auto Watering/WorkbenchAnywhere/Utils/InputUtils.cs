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
