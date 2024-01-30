/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewModdingAPI;

namespace WhoLivesHereCore
{
    internal class WhoLivesHereConfig
    {
        public KeybindList ToggleKey { get; set; } = new KeybindList(new Keybind(SButton.LeftControl,SButton.N), new Keybind(SButton.RightControl,SButton.N));
        public int AutoOnTime { get; set; } = 0;
        public int AutoOffTime { get; set;} = 0;
        public int PageDelay { get; set; } = 250;
        public bool HideEmptyTabs { get; set; } = true;
        public bool ShowAnimalCount { get; set; } = false;
    }
}
