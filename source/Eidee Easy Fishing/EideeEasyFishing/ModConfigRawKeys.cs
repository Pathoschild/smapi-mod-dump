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
using System.Linq;
using StardewModdingAPI;

namespace EideeEasyFishing
{
    internal class ModConfigRawKeys
    {
        public string ReloadConfig { get; set; } = SButton.F5.ToString();

        private static SButton ParseButton(string button, SButton defaultButton)
        {
            return (from SButton value in Enum.GetValues(typeof(SButton))
                    let name = Enum.GetName(typeof(SButton), value)
                    where name is not null && name.Equals(button, StringComparison.OrdinalIgnoreCase)
                    select value)
                .DefaultIfEmpty(defaultButton)
                .FirstOrDefault();
        }

        public ModConfigKeys ParseControls()
        {
            return new ModConfigKeys(reloadConfig: ParseButton(ReloadConfig, SButton.F5));
        }
    }
}