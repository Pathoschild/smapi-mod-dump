/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace ToolSmartSwitch
{
    public class ToolSmartSwitchAPI : IToolSmartSwitchAPI
    {
        public void SmartSwitch(Farmer f)
        {
            ModEntry.SmartSwitch(f);
        }

    }

    public interface IToolSmartSwitchAPI
    {
        public void SmartSwitch(Farmer f);

    }
}