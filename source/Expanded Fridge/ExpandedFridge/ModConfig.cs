/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Uwazouri/ExpandedFridge
**
*************************************************/

using StardewModdingAPI;


namespace ExpandedFridge
{
    /// <summary>
    /// Stores options for the Manager.
    /// </summary>
    public class ModConfig
    {
        public bool HideMiniFridges { get; set; } = true;
        public SButton NextFridgeTabButton { get; set; } = SButton.RightTrigger;
        public SButton LastFridgeTabButton { get; set; } = SButton.LeftTrigger;
    }
}
