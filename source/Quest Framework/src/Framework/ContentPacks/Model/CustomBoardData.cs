/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using QuestFramework.Framework.Structures;
using System.Collections.Generic;

namespace QuestFramework.Framework.ContentPacks.Model
{
    class CustomBoardData
    {
        public string BoardName { get; set; }
        public BoardType BoardType { get; set; } = BoardType.Quests;
        public string Location { get; set; }
        public Point Tile { get; set; }
        public Dictionary<string, string> UnlockWhen { get; set; }
        public bool ShowIndicator { get; set; } = true;
        public string Texture { get; set; }
        public Vector2 IndicatorOffset { get; set; } = Vector2.Zero;
    }
}
