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
using StardewModdingAPI;
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
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public Unlockable Unlockable;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            RegisterCommand("ub_applyPatch", delegate { ub_applyPatch(); });
        }
        public UBEvent(Unlockable unlockable, string eventString, Farmer farmerActor = null) : base(eventString, farmerActor)
        {
            Unlockable = unlockable;
        }

        public static void ub_applyPatch()
        {
            if (Game1.CurrentEvent is UBEvent ev)
                UpdateHandler.applyUnlockable(ev.Unlockable);
            else
                Monitor.Log("Event command ub_applyPatch was called outside of the context of Unlockable Bundles.", LogLevel.Warn);

            Game1.CurrentEvent.CurrentCommand++;
        }
    }
}
