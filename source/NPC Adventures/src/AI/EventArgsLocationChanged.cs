/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewValley;
using System;

namespace NpcAdventure.AI
{
    public class EventArgsLocationChanged : EventArgs
    {
        public GameLocation PreviousLocation { get; set; }
        public GameLocation CurrentLocation { get; set; }
    }
}