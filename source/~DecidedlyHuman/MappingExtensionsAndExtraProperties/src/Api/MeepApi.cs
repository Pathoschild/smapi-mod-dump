/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace MappingExtensionsAndExtraProperties.Api;

public class MeepApi
{
    public bool IsFakeNpc(NPC npc) => npc is FakeNpc;
}
