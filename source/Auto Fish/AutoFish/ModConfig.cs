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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;

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

        /// <summary>
        ///     是否启用持续自动钓鱼模式
        /// </summary>
        public bool triggerKeepAutoFish { get; set; } = true;

        /// <summary>
        ///     设置开始持续自动钓鱼按键 默认：Insert
        /// </summary>
        public KeybindList keepAutoFishKey { get; set; } = KeybindList.Parse("Insert");
        
        /// <summary>
        ///     是否自动收集宝箱内容
        /// </summary>
        public bool autoLootTreasure { get; set; } = true;
        
        /// <summary>
        ///     是否自动收集鱼和垃圾
        /// </summary>
        public bool autoLootFishAndTrash { get; set; } = true;
    }
}
