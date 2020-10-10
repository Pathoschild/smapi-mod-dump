/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rei2hu/stardewvalley-esp
**
*************************************************/

using StardewValley;

namespace StardewValleyEsp.Detectors
{
    interface IDetector
    {
        IDetector SetLocation(GameLocation loc);
        EntityList Detect();
    }
}
