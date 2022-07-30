/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Utilities
{
    public class Config
    {
        public string OpenButton { get; set; } = "F1,LeftStick";

        [JsonIgnore]
        public IEnumerable<SButton> Open => ParseButtons(OpenButton);

        public bool LockOnTransfer { get; set; } = false;

        public Config() { }

        [JsonConstructor]
        public Config(string open, bool lockOnTransfer)
        {
            OpenButton = open;
            LockOnTransfer = lockOnTransfer;
        }

        private IEnumerable<SButton> ParseButtons(string btn)
        {
            List<SButton> open = new List<SButton>();
            string[] buttons = btn.Split(',');
            for (int i = 0; i < buttons.Length; i++)
                if (Enum.TryParse(buttons[i].Trim(), out SButton sButton))
                    open.Add(sButton);

            return open;
        }
    }
}
