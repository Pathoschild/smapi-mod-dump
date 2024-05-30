/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using StardewModdingAPI;

namespace MoonShared.Patching
{
    internal interface IPatcher
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Apply the Harmony patches for this instance.</summary>
        /// <param name="harmony">The Harmony instance.</param>
        /// <param name="monitor">The monitor with which to log any errors.</param>
        public void Apply(Harmony harmony, IMonitor monitor);
    }
}
