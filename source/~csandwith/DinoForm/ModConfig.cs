/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace DinoForm
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public KeybindList TransformKey { get; set; } = new KeybindList();
        public KeybindList FireKey { get; set; } = new KeybindList(SButton.MouseLeft);
        public string TransformSound { get; set; } = "cowboy_explosion";
        public string FireSound { get; set; } = "furnace";
        public int FireDamage { get; set; } = 10;
        public int FireDistance { get; set; } = 256;
        public string EatTrigger { get; set; } = "Purple Mushroom";
        public int EatLastSeconds { get; set; } = 60;
        public int MoveSpeed { get; set; } = -3;
        public int Defense { get; set; } = 3;
        public int StaminaUse { get; set; } = 0;
    }
}
