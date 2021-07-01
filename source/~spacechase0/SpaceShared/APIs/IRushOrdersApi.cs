/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;

namespace SpaceShared.APIs
{
    public interface IApi
    {
        event EventHandler<Tool> ToolRushed;
        event EventHandler BuildingRushed;
    }
}
