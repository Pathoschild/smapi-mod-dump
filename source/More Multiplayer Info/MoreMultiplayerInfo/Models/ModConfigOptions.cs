/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

namespace MoreMultiplayerInfo
{
    public class ModConfigOptions
    {

        [OptionDisplay("Show Inventory")]
        public bool ShowInventory { get; set; } = true;


        [OptionDisplay("Show Info in Text Box")]
        public bool ShowReadyInfoInChatBox { get; set; } = true;


        [OptionDisplay("Last Player Alert")]
        public bool ShowLastPlayerReadyInfoInChatBox { get; set; } = true;

        [OptionDisplay("Hide in Single Player")]
        public bool HideInSinglePlayer { get; set; } = false;
    }
}