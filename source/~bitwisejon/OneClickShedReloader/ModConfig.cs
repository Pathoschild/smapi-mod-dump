/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitwiseJonMods.OneClickShedReloader
{
    public class ModConfig
    {
        public string configDescription { get; set; } = "You may attempt to add additional values to the lists below to support other mods and new buildings/containers added to the base game in the future. Use logNamesOnHover to help find the names of new buildings and items. But note that some modded buildings and items will never work, so you are on your own in finding the ones that do. If you completely mess up your config, delete the file and let the next launch of the game recreate it for you. Good luck!";

        /// <summary>
        /// Indicates if all item/building names should be logged when hovering over them. Use this to learn object/building names to include in the lists below.
        ///   Otherwise, leave this set to false since it junks up the SMAPI console.
        /// </summary>
        public ModConfigBoolItem logNamesOnHover { get; set; } = new ModConfigBoolItem()
        {
            description = "Set this to true to log all building and item names under your cursor to the SMAPI console. This will help you discover new building and items to add to the lists below.",
            value = false
        };

        /// <summary>
        /// A list of buildings to attempt to support. Will likely only work for the simplest of them.
        /// </summary>
        public ModConfigListItem buildingTypes { get; set; } = new ModConfigListItem()
        {
            description = "Add buildings to this list to have the mod *attempt* to support them, but note that only the simplest building types will likely work. Some buildings like the Cellar and Farmcave only load their contents for their owner.",
            values = new List<string>() {
                "Shed",
                "Barn",
                "Coop",
                "Greenhouse"
            }
        };

        /// <summary>
        /// The list of containers to attempt to support. This list should contain both reloadable (e.g., cask) and non-reloadable containers (e.g., statue of endless fortune).
        /// </summary>
        public ModConfigListItem containerTypes { get; set; } = new ModConfigListItem()
        {
            description = "Add container types to this list to have the mod *attempt* to support them. This list should contain both reloadable (e.g., Cask) and non-reloadable containers (e.g., Auto-Grabber).",
            values = new List<string>() {
                "Bee House",
                "Cask",
                "Charcoal Kiln",
                "Cheese Press",
                "Crystalarium",
                "Furnace",
                "Keg",
                "Loom",
                "Mayonnaise Machine",
                "Mushroom Box",
                "Oil Maker",
                "Preserves Jar",
                "Recycling Machine",
                "Seed Maker",
                "Auto-Grabber",
                "Statue Of Endless Fortune",
                "Statue Of Perfection"
            }
        };

        /// <summary>
        /// The list of non-reloadable containers to attempt to support. This list should only contain the non-reloadable containers from the list above.
        /// </summary>
        public ModConfigListItem nonReloadableContainerTypes { get; set; } = new ModConfigListItem()
        {
            description = "Add additional buildings (e.g., Stone Cabin) to this list to have the mod *attempt* to support them, but note that only the simplest building types will likely work.",
            values = new List<string>() {
                "Mushroom Box",
                "Auto-Grabber",
                "Statue Of Endless Fortune",
                "Statue Of Perfection"
            }
        };
    }

    public class ModConfigBoolItem
    {
        public string description { get; set; } = "";
        public bool value { get; set; } = false;
    }

    public class ModConfigListItem
    {
        public string description { get; set; } = "";
        public List<string> values { get; set; } = new List<string>();
    }
}
