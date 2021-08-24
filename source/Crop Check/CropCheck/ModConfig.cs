/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/CropCheck
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace CropCheck
{
    class ModConfig
    {
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("N");
        public int FruitTreeAlertNumber  { get; set; } = 1;
    }
}
