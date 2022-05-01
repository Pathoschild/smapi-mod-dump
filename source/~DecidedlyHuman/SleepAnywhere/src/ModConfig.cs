/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.SDKs;

namespace SleepAnywhere
{
    public class ModConfig
    {
        public KeybindList PlaceSleepingBag = KeybindList.Parse("OemTilde");
        public KeybindList GrabSleepingBagRemotely = KeybindList.Parse("OemPeriod");
        
        // The cheese zone.
        public bool RequireSleepingBagItem = true;
    }
}