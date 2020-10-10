/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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
