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
using SpaceShared;
using StardewValley;

namespace RushOrders
{
    public interface IApi
    {
        event EventHandler<Tool> ToolRushed;
        event EventHandler BuildingRushed;
    }

    public class Api : IApi
    {
        public event EventHandler<Tool> ToolRushed;
        internal void InvokeToolRushed(Tool tool)
        {
            Log.Trace("Event: ToolRushed");
            if (this.ToolRushed == null)
                return;
            Util.InvokeEvent("RushOrders.Api.ToolRushed", this.ToolRushed.GetInvocationList(), null, tool);
        }

        public event EventHandler BuildingRushed;
        internal void InvokeBuildingRushed()
        {
            Log.Trace("Event: BuildingRushed");
            if (this.BuildingRushed == null)
                return;
            Util.InvokeEvent("RushOrders.Api.BuildingRushed", this.BuildingRushed.GetInvocationList(), null);
        }
    }
}
