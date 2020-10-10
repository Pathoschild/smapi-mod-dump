/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

using System;
using StardewModdingAPI;

namespace EideeEasyFishing
{
    internal class ModConfigRawKeys
    {
        public string ReloadConfig { get; set; } = SButton.F5.ToString();

        private static SButton ParseButton(string button)
        {
            foreach (SButton value in Enum.GetValues(typeof(SButton)))
            {
                string name = Enum.GetName(typeof(SButton), value);
                if (name.Equals(button, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
            return SButton.None;
        }

        public ModConfigKeys ParseControls()
        {
            return new ModConfigKeys(reloadConfig: ParseButton(this.ReloadConfig));
        }
    }
}
