/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using Calcifer.Features;

namespace Calcifer;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        CategoryNameOverrideHooks.InitializeEventHooks();
        FurnitureActionHooks.InitializeEventHooks();
        FurnitureOffsetHooks.InitializeEventHooks();

    }
}
