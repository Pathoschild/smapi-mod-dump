/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;

namespace PlatoTK.Patching
{
    internal class TileAction : ITileAction
    {
        public string[] Trigger { get; }

        public Action<ITileActionTrigger> Handler { get; }

        public TileAction(Action<ITileActionTrigger> handler, params string[] trigger)
        {
            Trigger = trigger;
            Handler = handler;
        }
    }
}
