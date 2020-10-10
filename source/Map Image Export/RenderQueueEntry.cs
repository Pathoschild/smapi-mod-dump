/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/MapImageExporter
**
*************************************************/

using StardewValley;
using System;

namespace MapImageExporter
{
    [Flags]
    public enum RenderFlags
    {
        None = 0,
        Tiles = 1 << 0,
        Lighting = 1 << 1,
        Characters = 1 << 2,
        Player = 1 << 3,
        Event = 1 << 4,
        Weather = 1 << 5,
        Location = 1 << 6,
    }

    class RenderQueueEntry
    {
        public GameLocation loc;
        public RenderFlags flags;
        
        public bool Tiles           { get { return flags.HasFlag(RenderFlags.Tiles          ); } }
        public bool Lighting        { get { return flags.HasFlag(RenderFlags.Lighting       ); } }
        public bool Characters      { get { return flags.HasFlag(RenderFlags.Characters     ); } }
        public bool Player          { get { return flags.HasFlag(RenderFlags.Player         ); } }
        public bool Event           { get { return flags.HasFlag(RenderFlags.Event          ); } }
        public bool Weather         { get { return flags.HasFlag(RenderFlags.Weather        ); } }
        public bool Location        { get { return flags.HasFlag(RenderFlags.Location       ); } }

        public RenderQueueEntry( GameLocation loc, RenderFlags flags )
        {
            this.loc = loc;
            this.flags = flags;
        }
    }
}
