/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace HarvestCalendar.Framework
{
    internal class Configuration
    {
        public Boolean ToggleMod { get; set; } = true;
        public int IconSize { get; set; } = 2;

        public float IconX { get; set; } = 1f;
        public float IconY { get; set; } = 0f;

        public List<string> CropBlackList { get; set; } = new();

        public KeybindList ToggleBlackListKeybind { get; set; } = new KeybindList(new Keybind(SButton.LeftAlt, SButton.H),
                                                                        new Keybind(SButton.RightAlt, SButton.H));

        public KeybindList BlacklistTheCropKeybind { get; set; } = new KeybindList(new Keybind(SButton.LeftAlt, SButton.G),
                                                                        new Keybind(SButton.RightAlt, SButton.G));
        public KeybindList ToggleCalendarDayDetailKeybind { get; set; } = new KeybindList(new Keybind(SButton.MouseLeft),
                                                                        new Keybind(SButton.ControllerA));

        private IModHelper? _helper;

        public void AddHelper(IModHelper helper) 
        {
            _helper= helper;
        }
        public void AddToBlackList(string cropId) 
        {
            CropBlackList.Add(cropId);
            _helper?.WriteConfig(this);
        }

        public void RemoveFromBlackList(string cropId)
        {
            CropBlackList.Remove(cropId);
            _helper?.WriteConfig(this);
        }
    }
}
