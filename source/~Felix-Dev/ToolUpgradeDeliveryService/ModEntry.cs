using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.Common;
using StardewMods.ToolUpgradeDeliveryService.Framework;
using StardewMods.ToolUpgradeDeliveryService.Framework.Menus;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Constants = StardewMods.ToolUpgradeDeliveryService.Common.Constants;

namespace StardewMods.ToolUpgradeDeliveryService
{
    internal class ModEntry : Mod, IAssetEditor
    {
        public static CommonServices CommonServices { get; private set; }

        private MailDeliveryService mailDeliveryService;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Start services
            CommonServices = new CommonServices(Monitor, helper.Translation, helper.Reflection, helper.Content);

            mailDeliveryService = new MailDeliveryService();
            mailDeliveryService.Start();
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
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_AXE_1, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_AXE_2, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_AXE_3, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_AXE_4, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_AXE_5, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_AXE));

            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_PICKAXE_1, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_PICKAXE_2, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_PICKAXE_3, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_PICKAXE_4, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_PICKAXE_5, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_PICKAXE));

            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_HOE_1, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_HOE_2, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_HOE_3, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_HOE_4, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_HOE_5, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_HOE));

            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_SHEARS_1, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_SHEARS_2, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_SHEARS_3, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_SHEARS_4, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_SHEARS_5, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_SHEARS));

            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_1, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_2, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_3, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_4, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN));
            mails.Add(Constants.MAIL_KEY_TOOL_UPGRADE_WATERING_CAN_5, Helper.Translation.Get(Constants.TRANSLATION_KEY_MAIL_TOOL_UPGRADE_WATERING_CAN));
        }
    }
}
