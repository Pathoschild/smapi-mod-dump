/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace ChestDisplays.Utility
{
    public class Config
    {
        public string ChangeItemKey { get; set; } = "OemQuotes, LeftStick";

        public float ItemScale { get; set; } = 0.42f;

        public float Transparency { get; set; } = 1f;

        public bool DisplayQuality { get; set; } = true;

        public bool ShowFirstIfNoneSelected { get; set; } = true;

        public bool ShowFridgeIcon { get; set; } = false;

        [JsonIgnore]
        public IEnumerable<SButton> ChangeItemButtons => Utils.ParseSButton(ChangeItemKey);
    }
}
