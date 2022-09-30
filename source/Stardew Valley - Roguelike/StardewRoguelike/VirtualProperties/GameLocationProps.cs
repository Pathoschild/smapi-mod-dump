/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewRoguelike.Extensions;
using StardewRoguelike.Netcode;
using StardewValley;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static StardewRoguelike.RoguelikeUtility;

namespace StardewRoguelike.VirtualProperties
{
    public static class GameLocationSpeechBubbles
    {
        internal class Holder { public Dictionary<Vector2, SpeechBubble> Value { get; set; } = new(); }

        internal static ConditionalWeakTable<GameLocation, Holder> values = new();

        internal static Dictionary<Vector2, SpeechBubble> get_SpeechBubbles(this GameLocation location)
        {
            return values.GetOrCreateValue(location).Value;
        }
    }

    public static class GameLocationDebuffPlayersEvent
    {
        internal class Holder { public readonly NetEvent2Field<Rectangle, NetRectangle, int, NetInt> Value = new(); }

        internal static ConditionalWeakTable<GameLocation, Holder> values = new();

        public static NetEvent2Field<Rectangle, NetRectangle, int, NetInt> get_DebuffPlayerEvent(this GameLocation location)
        {
            bool got = values.TryGetValue(location, out Holder holder);
            if (got)
                return holder.Value;

            holder = values.GetOrCreateValue(location);
            holder.Value.onEvent += location.performDebuffPlayers;
            return holder.Value;
        }

        public static void unhook_getDebuffPlayerEvent(this GameLocation location)
        {
            bool got = values.TryGetValue(location, out Holder holder);
            if (!got)
                return;

            holder.Value.onEvent -= location.performDebuffPlayers;
        }
    }
}
