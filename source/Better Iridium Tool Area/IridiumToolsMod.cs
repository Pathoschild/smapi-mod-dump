/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dphile/SDVBetterIridiumTools
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using PyTK.Extensions;
using PyTK.Types;
using StardewValley.Tools;


namespace BetterIridiumFarmTools
{
    public class IridiumToolsMod : Mod
    {
        internal static IModHelper _helper;

        internal static EventHandler<EventArgsClickableMenuChanged> addtoshop;

        internal static EventHandler<EventArgsClickableMenuChanged> addtoshop2;

     

        public override void Entry(IModHelper helper)
        {
            IridiumToolsMod._helper = helper;
            IridiumToolsMod.addtoshop = PyEvents.addToNPCShop(new InventoryItem((Item)new IridiumHoe(), 25000, 3), "Pierre");
            IridiumToolsMod.addtoshop = PyEvents.addToNPCShop(new InventoryItem((Item)new IridiumWaterCan(), 25000, 3), "Pierre");

        }
        
    }
}
