using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace StarDustCore
{
    public class Base : Mod
    {
        public override void Entry(IModHelper helper)
        {

            //GameEvents.GameLoaded += new EventHandler(SerializerUtility.Event_GameLoaded);
            //Command.RegisterCommand("include_types", "Includes types to serialize", (string[])null).CommandFired += new EventHandler<EventArgsCommand>(SerializerUtility.Command_IncludeTypes);
        }



    }
}
