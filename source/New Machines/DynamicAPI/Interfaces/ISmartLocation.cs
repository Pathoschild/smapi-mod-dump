/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Locations;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface ISmartLocation
    {
        SmartLocationProxy Proxy { get; }
    }
}