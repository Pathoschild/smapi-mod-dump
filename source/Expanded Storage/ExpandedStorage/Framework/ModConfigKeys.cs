/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ExpandedStorage.Framework
{
    public class ModConfigKeys
    {
        public KeybindList ScrollUp { get; set; } = KeybindList.ForSingle(SButton.DPadUp);
        public KeybindList ScrollDown { get; set; } = KeybindList.ForSingle(SButton.DPadDown);
        public KeybindList PreviousTab { get; set; } = KeybindList.ForSingle(SButton.DPadLeft);
        public KeybindList NextTab { get; set; } = KeybindList.ForSingle(SButton.DPadRight);
    }
}