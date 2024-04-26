/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

namespace CustomNames
{
    public class Config
    {
        /// <summary>
        /// The price of renaming a tool, weapon, or trinket
        /// </summary>
        public int CostToName { get; set; } = 5;

        /// <summary>
        /// Whether or not to clear the custom name when the unforge button is clicked
        /// </summary>
        public bool UnforgeClearsName { get; set; } = false;

        /// <summary>
        /// Whether or not to show the name of a companion on hover
        /// </summary>
        public bool ShowCompanionName { get; set; } = true;

        /// <summary>
        /// Whether or not to show the name of the selected tool for a second on select
        /// </summary>
        public bool ShowToolbarName { get; set; } = true;
    }
}
