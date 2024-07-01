/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;


namespace Unlockable_Bundles.Lib
{
    public class UBEvent : Event
    {
        public Unlockable Unlockable;

        public const string APPLYPATCH = "UB_ApplyPatch";

        public static void Initialize()
        {
            RegisterCommand(APPLYPATCH, delegate { UB_ApplyPatch(); });
            //RegisterCommandAlias("ub_applyPatch", APPLYPATCH); //Old command
        }
        public UBEvent(Unlockable unlockable, string eventString, Farmer farmerActor = null) : base(eventString, farmerActor)
        {
            Unlockable = unlockable;
        }

        public static void UB_ApplyPatch()
        {
            if (Game1.CurrentEvent is UBEvent ev)
                MapPatches.applyUnlockable(ev.Unlockable);
            else
                Monitor.Log("Event command UB_ApplyPatch was called outside of the context of Unlockable Bundles.", LogLevel.Warn);

            Game1.CurrentEvent.CurrentCommand++;
        }
    }
}
