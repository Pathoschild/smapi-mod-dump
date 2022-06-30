/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Menus;

namespace ForecasterText.Objects {
    internal delegate IEnumerable<ChatSnippet> ConfigMessageRenderer(ConfigEmojiMessage message);
    internal delegate string ConfigMessageParsingRenderer(ConfigEmojiMessage message);
}
