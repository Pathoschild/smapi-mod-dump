using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ToolUpgradeDeliveryService.Common
{
    internal class Constants
    {
        /* Translation keys */

        public const string TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE = "MAIL_TOOL_UPGRADE_AXE";
        public const string TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE = "MAIL_TOOL_UPGRADE_PICKAXE";
        public const string TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE = "MAIL_TOOL_UPGRADE_HOE";
        public const string TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS = "MAIL_TOOL_UPGRADE_SHEARS";
        public const string TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN = "MAIL_TOOL_UPGRADE_WATERING_CAN";

        /* Mail keys */

        /*
         * Format for tool upgrade mail keys:
         * 
         * [CONTENT_UPGRADE]:[TOOL_TYPE]:[UPGRADE_LEVEL]
         * 
         * There are 5 tools: Axe, Pickaxe, Hoe, Shears, Watering Can
         * There are 4 upgrade levels per tool: Cooper, Steel, Gold, Iridium
         * 
         * With this format, we can map each mail to a specific tool upgrade.
         */

        // CONTENT_TYPE
        public const string TOOL_UPGRADE = "TOOL_UPGRADE";

        // TOOL_TYPE
        public const string TOOL_AXE = "1";
        public const string TOOL_PICKAXE = "2";
        public const string TOOL_HOE = "3";
        public const string TOOL_SHEARS = "4";
        public const string TOOL_WATERING_CAN = "5";

        // UPGRADE_LEVEL
        public const string TOOL_UPGRADE_LEVEL_COPPER = "1";
        public const string TOOL_UPGRADE_LEVEL_STEEL = "2";
        public const string TOOL_UPGRADE_LEVEL_GOLD = "3";
        public const string TOOL_UPGRADE_LEVEL_IRIDIUM = "4";
        public const string TOOL_UPGRADE_LEVEL_PRISMATIC = "5"; // Added by mod [Prismatic Tools]

        public const string MAIL_KEY_TOOL_UPGRADE_AXE_1 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        public const string MAIL_KEY_TOOL_UPGRADE_AXE_2 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        public const string MAIL_KEY_TOOL_UPGRADE_AXE_3 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        public const string MAIL_KEY_TOOL_UPGRADE_AXE_4 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        public const string MAIL_KEY_TOOL_UPGRADE_AXE_5 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        public const string MAIL_KEY_TOOL_UPGRADE_PICKAXE_1 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        public const string MAIL_KEY_TOOL_UPGRADE_PICKAXE_2 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        public const string MAIL_KEY_TOOL_UPGRADE_PICKAXE_3 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        public const string MAIL_KEY_TOOL_UPGRADE_PICKAXE_4 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        public const string MAIL_KEY_TOOL_UPGRADE_PICKAXE_5 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        public const string MAIL_KEY_TOOL_UPGRADE_HOE_1 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        public const string MAIL_KEY_TOOL_UPGRADE_HOE_2 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        public const string MAIL_KEY_TOOL_UPGRADE_HOE_3 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        public const string MAIL_KEY_TOOL_UPGRADE_HOE_4 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        public const string MAIL_KEY_TOOL_UPGRADE_HOE_5 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        public const string MAIL_KEY_TOOL_UPGRADE_SHEARS_1 = TOOL_UPGRADE + ":" + TOOL_SHEARS + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        public const string MAIL_KEY_TOOL_UPGRADE_SHEARS_2 = TOOL_UPGRADE + ":" + TOOL_SHEARS + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        public const string MAIL_KEY_TOOL_UPGRADE_SHEARS_3 = TOOL_UPGRADE + ":" + TOOL_SHEARS + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        public const string MAIL_KEY_TOOL_UPGRADE_SHEARS_4 = TOOL_UPGRADE + ":" + TOOL_SHEARS + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        public const string MAIL_KEY_TOOL_UPGRADE_SHEARS_5 = TOOL_UPGRADE + ":" + TOOL_SHEARS + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        public const string MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_1 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        public const string MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_2 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        public const string MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_3 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        public const string MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_4 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        public const string MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_5 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;
    }
}
