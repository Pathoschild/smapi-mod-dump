/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miome/MultitoolMod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MultitoolMod
{
    class ModConfig
    {
        public KeybindList ToolButton { get; set; } = KeybindList.ForSingle(SButton.Q);
        public KeybindList InfoButton { get; set; } = KeybindList.ForSingle(SButton.N);
        public KeybindList CleanButton { get; set; } = KeybindList.ForSingle(SButton.C);
    }
}
