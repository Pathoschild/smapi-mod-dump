/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zoryn4163/SMAPI-Mods
**
*************************************************/

namespace BetterRNG.Framework
{
    internal interface IWeighted
    {
        int Weight { get; set; }
        object Value { get; set; }
    }
}
