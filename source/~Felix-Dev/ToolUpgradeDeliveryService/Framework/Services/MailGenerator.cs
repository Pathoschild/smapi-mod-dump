using StardewModdingAPI;
using StardewMods.ToolUpgradeDeliveryService.Common;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translation = StardewMods.ToolUpgradeDeliveryService.Common.Translation;

namespace StardewMods.ToolUpgradeDeliveryService.Framework
{
    /// <summary>
    /// This class adds Tool Mails to the game.
    /// </summary>
    internal class MailGenerator : IAssetEditor
    {
        #region Constants

        /*
         * Format for tool upgrade mail keys:
         * 
         * [CONTENT_TYPE]:[TOOL_TYPE]:[UPGRADE_LEVEL]
         * 
         * There are 4 blacksmith-upgradable tools: Axe, Pickaxe, Hoe, Watering Can
         * There are 5 (4) upgrade levels per tool: Cooper, Steel, Gold, Iridium, Prismatic
         * (Level 5 represents the tool upgrade introduced by the mod [Prismatic Tools]).
         * 
         * With this format, we can map each mail to a specific tool upgrade.
         */

        // CONTENT_TYPE
        private const string TOOL_UPGRADE = "TOOL_UPGRADE";

        // TOOL_TYPE
        private const string TOOL_AXE = "1";
        private const string TOOL_PICKAXE = "2";
        private const string TOOL_HOE = "3";

        // Watering can is entry "5" because the non-upgradable tool [Shears] was erroneously 
        // included as entry "4". To avoid a breaking change, the number assigned to the
        // watering can has not been changed.
        private const string TOOL_WATERING_CAN = "5";

        // UPGRADE_LEVEL
        private const string TOOL_UPGRADE_LEVEL_COPPER = "1";
        private const string TOOL_UPGRADE_LEVEL_STEEL = "2";
        private const string TOOL_UPGRADE_LEVEL_GOLD = "3";
        private const string TOOL_UPGRADE_LEVEL_IRIDIUM = "4";
        private const string TOOL_UPGRADE_LEVEL_PRISMATIC = "5"; // Added by mod [Prismatic Tools]

        private const string TOOL_UPGRADE_AXE_1 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        private const string TOOL_UPGRADE_AXE_2 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        private const string TOOL_UPGRADE_AXE_3 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        private const string TOOL_UPGRADE_AXE_4 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        private const string TOOL_UPGRADE_AXE_5 = TOOL_UPGRADE + ":" + TOOL_AXE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        private const string TOOL_UPGRADE_PICKAXE_1 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        private const string TOOL_UPGRADE_PICKAXE_2 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        private const string TOOL_UPGRADE_PICKAXE_3 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        private const string TOOL_UPGRADE_PICKAXE_4 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        private const string TOOL_UPGRADE_PICKAXE_5 = TOOL_UPGRADE + ":" + TOOL_PICKAXE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        private const string TOOL_UPGRADE_HOE_1 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        private const string TOOL_UPGRADE_HOE_2 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        private const string TOOL_UPGRADE_HOE_3 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        private const string TOOL_UPGRADE_HOE_4 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        private const string TOOL_UPGRADE_HOE_5 = TOOL_UPGRADE + ":" + TOOL_HOE + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        private const string TOOL_UPGRADE_WATERING_CAN_1 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_COPPER;
        private const string TOOL_UPGRADE_WATERING_CAN_2 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_STEEL;
        private const string TOOL_UPGRADE_WATERING_CAN_3 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_GOLD;
        private const string TOOL_UPGRADE_WATERING_CAN_4 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_IRIDIUM;
        private const string TOOL_UPGRADE_WATERING_CAN_5 = TOOL_UPGRADE + ":" + TOOL_WATERING_CAN + ":" + TOOL_UPGRADE_LEVEL_PRISMATIC;

        #endregion

        private ITranslationHelper translationHelper;

        public MailGenerator()
        {
            translationHelper = ModEntry.CommonServices.TranslationHelper;
        }

        public string GenerateMail(Tool tool)
        {
            switch (Game1.player.toolBeingUpgraded.Value)
            {
                case Axe t:
                    return TOOL_UPGRADE + $":{TOOL_AXE}:{t.UpgradeLevel}";
                case Pickaxe t:
                    return TOOL_UPGRADE + $":{TOOL_PICKAXE}:{t.UpgradeLevel}";
                case Hoe t:
                    return TOOL_UPGRADE + $":{TOOL_HOE}:{t.UpgradeLevel}";
                case WateringCan t:
                    return TOOL_UPGRADE + $":{TOOL_WATERING_CAN}:{t.UpgradeLevel}";
                default:
                    return null;
            }
        }

        public (Type toolType, int level)? GetMailAssignedTool(string mailKey)
        {
            if (mailKey == null)
            {
                return null;

            }

            var keyParts = mailKey.Split(':');
            if (keyParts.Count() != 3)
            {
                return null;
            }

            if (ToolHelper.TryParse(keyParts[1], out Type tool) && int.TryParse(keyParts[2], out int level))
            {
                return (tool, level);
            }

            return null;
        }

        public bool IsToolMail(string mail)
        {        
            if (mail == null)
            {
                return false;
            }

            return mail.StartsWith(TOOL_UPGRADE);
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/mail") ? true : false;
        }

        public void Edit<T>(IAssetData asset)
        {
            IDictionary<string, string> mails = asset.AsDictionary<string, string>().Data;

            /*
             * We add a mail for each tool and its upgrade versions, to get a mapping [mail] -> [tool, upgrade level].
             */
            mails.Add(TOOL_UPGRADE_AXE_1, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_AXE));
            mails.Add(TOOL_UPGRADE_AXE_2, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_AXE));
            mails.Add(TOOL_UPGRADE_AXE_3, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_AXE));
            mails.Add(TOOL_UPGRADE_AXE_4, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_AXE));
            mails.Add(TOOL_UPGRADE_AXE_5, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_AXE));

            mails.Add(TOOL_UPGRADE_PICKAXE_1, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(TOOL_UPGRADE_PICKAXE_2, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(TOOL_UPGRADE_PICKAXE_3, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(TOOL_UPGRADE_PICKAXE_4, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(TOOL_UPGRADE_PICKAXE_5, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_PICKAXE));

            mails.Add(TOOL_UPGRADE_HOE_1, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_HOE));
            mails.Add(TOOL_UPGRADE_HOE_2, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_HOE));
            mails.Add(TOOL_UPGRADE_HOE_3, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_HOE));
            mails.Add(TOOL_UPGRADE_HOE_4, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_HOE));
            mails.Add(TOOL_UPGRADE_HOE_5, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_HOE));

            mails.Add(TOOL_UPGRADE_WATERING_CAN_1, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(TOOL_UPGRADE_WATERING_CAN_2, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(TOOL_UPGRADE_WATERING_CAN_3, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(TOOL_UPGRADE_WATERING_CAN_4, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(TOOL_UPGRADE_WATERING_CAN_5, translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_WATERING_CAN));
        }

        private class ToolHelper
        {
            public static bool TryParse(string s, out Type result)
            {
                switch (s)
                {
                    case TOOL_AXE:
                        result = typeof(Axe);
                        return true;
                    case TOOL_PICKAXE:
                        result = typeof(Pickaxe);
                        return true;
                    case TOOL_HOE:
                        result = typeof(Hoe);
                        return true;
                    case TOOL_WATERING_CAN:
                        result = typeof(WateringCan);
                        return true;
                }

                result = null;
                return false;
            }
        }
    }
}
