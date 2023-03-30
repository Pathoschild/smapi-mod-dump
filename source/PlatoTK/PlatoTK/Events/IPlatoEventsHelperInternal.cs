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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using xTile.Layers;

namespace PlatoTK.Events
{
    internal interface IPlatoEventsHelperInternal : IPlatoEventsHelper
    {

        void HandleTileAction(string[] commands, Farmer who, GameLocation location, Point position, Action<bool> callback);
    }
}
