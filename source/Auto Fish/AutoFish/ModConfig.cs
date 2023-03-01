/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WhiteMinds/mod-sv-autofish
**
*************************************************/

using System;
namespace AutoFish
{
    public class ModConfig
    {
        /// <summary>
        ///     强制最大力度抛竿
        /// </summary>
        public bool maxCastPower { get; set; } = true;
        /// <summary>
        ///     上钩时自动点击
        /// </summary>
        public bool autoHit { get; set; } = true;
        /// <summary>
        ///     快速上钩
        /// </summary>
        public bool fastBite { get; set; } = false;
        /// <summary>
        ///     捕捉宝箱（当遇到传说鱼时忽略）
        /// </summary>
        public bool catchTreasure { get; set; } = true;
        /// <summary>
        ///     浮漂会更快的移动以增加捕获率
        /// </summary>
        public bool fasterSpeed { get; set; } = false;
    }
}
