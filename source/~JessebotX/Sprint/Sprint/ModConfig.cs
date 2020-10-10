/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using Sprint.Config;

namespace Sprint
{
    class ModConfig
    {
        public SButton SprintKey { get; set; } = SButton.LeftShift;
        public SprintConfig Sprint { get; set; } = new SprintConfig();
        public StaminaDrainConfig StaminaDrain { get; set; } = new StaminaDrainConfig();
    }
}
