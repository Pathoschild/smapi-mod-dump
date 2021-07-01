/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TransparencySettings
{
    /// <summary>This mod's config.json setting data.</summary>
    public class ModConfig
    {
        public ObjectSettings BuildingSettings = new ObjectSettings()
        {
            TileDistance = 3
        };
        public ObjectSettings BushSettings = new ObjectSettings();
        public ObjectSettings TreeSettings = new ObjectSettings();
        public KeybindSettings KeyBindings = new KeybindSettings();
    }

    /* Setting collections (used for descriptive organization in the config.json file) */

    public class ObjectSettings
    {
        public bool Enable = true;
        public bool BelowPlayerOnly = true;
        public int TileDistance = 5;
    }

    public class KeybindSettings
    {
        private KeybindList _disableTransparency = new KeybindList(new Keybind(SButton.OemTilde, SButton.LeftShift));
        public KeybindList DisableTransparency
        {
            get { return _disableTransparency; }
            set { _disableTransparency = value ?? new KeybindList(); } //prevent null values
        }

        private KeybindList _fullTransparency = new KeybindList(new Keybind(SButton.OemTilde));
        public KeybindList FullTransparency
        {
            get { return _fullTransparency; }
            set { _fullTransparency = value ?? new KeybindList(); } //prevent null values
        }
    }
}
