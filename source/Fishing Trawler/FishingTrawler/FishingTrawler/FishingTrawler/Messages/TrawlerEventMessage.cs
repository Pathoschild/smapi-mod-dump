/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTrawler.Messages
{
    public enum EventType
    {
        Unknown,
        HullHole,
        NetTear,
        EngineFailure
    }

    internal class TrawlerEventMessage
    {
        public EventType EventType { get; set; }
        public Vector2 Tile { get; set; }
        public bool IsRepairing { get; set; }

        public TrawlerEventMessage()
        {

        }

        public TrawlerEventMessage(EventType eventType, Vector2 tile, bool isRepairing = false)
        {
            EventType = eventType;
            Tile = tile;
            IsRepairing = isRepairing;
        }
    }
}
