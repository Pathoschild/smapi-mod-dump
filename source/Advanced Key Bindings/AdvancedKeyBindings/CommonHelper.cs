using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace AdvancedKeyBindings
{
    public static class CommonHelper
    {
        /****
        ** Input handling
        ****/
        /// <summary>Parse a button configuration string into a buttons array.</summary>
        /// <param name="raw">The raw config string.</param>
        /// <param name="onInvalidButton">A callback invoked when a button value can't be parsed.</param>
        public static SButton[] ParseButtons(string raw, Action<string> onInvalidButton)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new SButton[0];

            IList<SButton> buttons = new List<SButton>();
            foreach (string value in raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(value, true, out SButton button))
                    buttons.Add(button);
                else
                    onInvalidButton(value);
            }
            return buttons.ToArray();
        }
        
        /// <summary>Parse a button configuration string into a buttons array.</summary>
        /// <param name="raw">The raw config string.</param>
        /// <param name="monitor">The monitor through which to log an error if a button value is invalid.</param>
        /// <param name="field">The field name to report in logged errors.</param>
        public static SButton[] ParseButtons(string raw, IMonitor monitor, string field)
        {
            return CommonHelper.ParseButtons(raw, onInvalidButton: value => monitor.Log($"Ignored invalid button '{value}' for {field} in config.json; delete the file to regenerate it, or see http://stardewvalleywiki.com/Modding:Key_bindings for valid keys.", LogLevel.Error));
        }
    }
}