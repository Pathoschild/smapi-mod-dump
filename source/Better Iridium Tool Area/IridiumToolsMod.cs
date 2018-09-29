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
