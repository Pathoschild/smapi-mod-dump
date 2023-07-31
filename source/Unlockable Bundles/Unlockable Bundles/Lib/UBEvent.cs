/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib
{
    public class UBEvent : Event
    {
        public Unlockable Unlockable;
        public UBEvent(Unlockable unlockable, string eventString, int eventID = -1, Farmer farmerActor = null) : base(eventString, eventID, farmerActor)
        {
            Unlockable = unlockable;

            if (!_commandLookup.ContainsKey("ub_applyPatch"))
                _commandLookup.Add("ub_applyPatch", (from method_info in typeof(UBEvent).GetMethods()
                                                           where method_info.Name == "command_ub_applyPatch"
                                                           select method_info).First());
        }

        public virtual void command_ub_applyPatch(GameLocation location, GameTime time, string[] split)
        {
            UpdateHandler.applyUnlockable(Unlockable);
            CurrentCommand++;
        }
    }
}
