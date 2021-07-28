/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace PlatoTK.Events
{
    public interface ICalledEventCommandEventArgs
    {
        Event Event { get; }
        GameLocation Location { get; }

        GameTime Time { get; }

        string Trigger { get; }

        string[] Parameter { get; }
    }

    public interface ICallingEventCommandEventArgs : ICalledEventCommandEventArgs
    {
        void PreventDefault(bool gotoNext = false);
    }
}
