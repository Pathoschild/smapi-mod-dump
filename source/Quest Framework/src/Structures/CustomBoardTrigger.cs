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
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Framework.Menus;
using StardewModdingAPI;
using StardewValley.Menus;
using System;

namespace QuestFramework.Framework.Structures
{
    public sealed class CustomBoardTrigger
    {
        public string LocationName { get; set; }
        public Point Tile { get; set; }
        public string BoardName { get; set; }
        public bool ShowIndicator { get; set; } = true;
        public BoardType BoardType { get; set; } = BoardType.Quests;
        public Texture2D Texture { get; set; }
        public Vector2 IndicatorOffset { get; set; } = Vector2.Zero;

        public Func<bool> unlockConditionFunc;

        public bool IsUnlocked()
        {
            return this.unlockConditionFunc == null || this.unlockConditionFunc();
        }
    }

    public enum BoardType
    {
        Quests,
        SpecialOrders,
    }
}