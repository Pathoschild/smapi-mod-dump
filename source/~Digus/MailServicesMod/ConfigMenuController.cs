/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailServicesMod.integrations;
using StardewModdingAPI;

namespace MailServicesMod
{
    internal class ConfigMenuController
    {
        private static GenericModConfigMenuApi _api;

        internal static void CreateConfigMenu(IManifest manifest)
        {

            if (GetApi() is GenericModConfigMenuApi api)
            {
                ModConfig modConfig = DataLoader.ModConfig;
                api.RegisterModConfig(manifest, () => DataLoader.ModConfig = new ModConfig(), () => DataLoader.Helper.WriteConfig(modConfig));

                api.RegisterLabel(manifest, "Enable Services:", "");
                api.RegisterSimpleOption(manifest, "Gift Service", "Let you send gifts to the villagers using the mailbox.", () => !modConfig.DisableGiftService, (bool val) => modConfig.DisableGiftService = !val);
                api.RegisterSimpleOption(manifest, "Quest Service", "Let you send items to complete quests using the mailbox.", () => !modConfig.DisableQuestService, (bool val) => modConfig.DisableQuestService = !val);
                api.RegisterSimpleOption(manifest, "Tool Delivery Service", "You will receive upgraded tools in the mailbox.", () => !modConfig.DisableToolDeliveryService, (bool val) => modConfig.DisableToolDeliveryService = !val);
                api.RegisterSimpleOption(manifest, "Tool Shipment Service", "Let you send tools to upgrade using the mailbox.", () => !modConfig.DisableToolShipmentService, (bool val) => modConfig.DisableToolShipmentService = !val);

                api.RegisterLabel(manifest, "Services Fees:", "Fee per use of service.");
                api.RegisterSimpleOption(manifest, "Gift Shipment (G)", "How much gold you'll be charged for sending gifts.", () => modConfig.GiftServiceFee, (int val) => modConfig.GiftServiceFee = val);
                api.RegisterSimpleOption(manifest, "Gift Shipment (%)", "How much in gift value percentage you'll be charged for sending gifts.", () => modConfig.GiftServicePercentFee, (int val) => modConfig.GiftServicePercentFee = val);
                api.RegisterSimpleOption(manifest, "Quest Item Shipment (G)", "How much gold you'll be charged for sending quest items.", () => modConfig.QuestServiceFee, (int val) => modConfig.QuestServiceFee = val);
                api.RegisterSimpleOption(manifest, "Tool Shipment (G)", "How much extra gold you'll be charged for sending tools to upgrade.", () => modConfig.ToolShipmentServiceFee, (int val) => modConfig.ToolShipmentServiceFee = val);
                api.RegisterSimpleOption(manifest, "Tool Shipment (%)", "How much extra in tool upgrade cost percentage you'll be charged for sending tools to upgrade.", () => modConfig.ToolShipmentServicePercentFee, (int val) => modConfig.ToolShipmentServicePercentFee = val);

                api.RegisterLabel(manifest, "General:", "");
                api.RegisterSimpleOption(manifest, "Show Dialog On Shipment", "Show the npc dialog as if you were delivering something in person. Works for gifts and quest completion.", () => modConfig.ShowDialogOnItemDelivery, (bool val) => modConfig.ShowDialogOnItemDelivery = val);
                
                api.RegisterLabel(manifest, "Tool Upgrade Service:", "Properties related to the tool upgrade service.");
                api.RegisterSimpleOption(manifest, "Ask to Upgrade Tool", "When placing the tool in the mailbox you will have to confirm if you want to upgrade it.", () => modConfig.EnableAskToUpgradeTool, (bool val) => modConfig.EnableAskToUpgradeTool = val);

                api.RegisterLabel(manifest, "Gift Service:", "Properties related to the gift service.");
                api.RegisterSimpleOption(manifest, "Minimum Friendship Points", "Friendship points needed to send gifts to a NPC. 250 friendship points equal 1 heart level.", () => modConfig.MinimumFriendshipPointsToSendGift, (int val) => modConfig.MinimumFriendshipPointsToSendGift = val);
                api.RegisterSimpleOption(manifest, "NPC Page Size", "Number of villagers shown per page on gift shipment.", () => modConfig.GiftChoicePageSize, (int val) => modConfig.GiftChoicePageSize = val);
                api.RegisterSimpleOption(manifest, "Jealousy", "Make it possible for your spouse to be jealous of gifts sent by mail like of gifts given in person.", () => modConfig.EnableJealousyFromMailedGifts, (bool val) => modConfig.EnableJealousyFromMailedGifts = val);
                api.RegisterSimpleOption(manifest, "Max Friendship", "Make it possible to send gifts to friends with maxed friendship.", () => modConfig.EnableGiftToNpcWithMaxFriendship, (bool val) => modConfig.EnableGiftToNpcWithMaxFriendship = val);

                string recoveryServiceDescription = "You'll receive the items you lost on passing out in the mailbox, according to the other config options.";
                string recoverAllItemsDescription = "You'll receive all lost items you can pay for in the mailbox.";
                string recoverForFreeDescription = "You won't have to pay for the recovered items.";
                string clearLostItemsDescription = "If 'recover all items' isn't enabled, once a random item is recovered, the others are cleared and lost forever. If disabled, items won't be cleared and you'll receive an random item each day until all are recovered, or you chose an item with Marlon, or you pass out again.";

                api.RegisterLabel(manifest, "Recovery Service (Default):", "Properties related to the recovery of lost items. This properties are the ones used if you don't enable per framer configuration. They're also the ones that a farmer starts with.");
                api.RegisterSimpleOption(manifest, "In Game Config Changes", "Let in game events change the farmer's recovery config. If per farmer config is disabled, the default properties will be changed.", () => !modConfig.DisableRecoveryConfigInGameChanges, (bool val) => modConfig.DisableRecoveryConfigInGameChanges = !val);
                api.RegisterSimpleOption(manifest, "Recovery Service", recoveryServiceDescription, () => modConfig.EnableRecoveryService, (bool val) => modConfig.EnableRecoveryService = val);
                api.RegisterSimpleOption(manifest, "Recover All Items", recoverAllItemsDescription, () => modConfig.RecoverAllItems, (bool val) => modConfig.RecoverAllItems = val);
                api.RegisterSimpleOption(manifest, "Recover For Free", recoverForFreeDescription, () => modConfig.RecoverForFree, (bool val) => modConfig.RecoverForFree = val);
                api.RegisterSimpleOption(manifest, "Clear Lost Items", clearLostItemsDescription, () => !modConfig.DisableClearLostItemsOnRandomRecovery, (bool val) => modConfig.DisableClearLostItemsOnRandomRecovery = !val);
                api.RegisterSimpleOption(manifest, "Per Farmer Config", "Farmers recovery config will be tracked individually.", () => !modConfig.DisablePerPlayerConfig, (bool val) => modConfig.DisablePerPlayerConfig = !val);

                if (modConfig.PlayerRecoveryConfig.Count > 0)
                {
                    api.RegisterLabel(manifest, "Recovery Config - Farmers List:", "Once you open a save file or create a new game, the farmer config should be tracked here.");

                    foreach (KeyValuePair<long, PlayerRecoveryConfig> playerRecoveryConfig in modConfig.PlayerRecoveryConfig)
                    {
                        api.RegisterPageLabel(manifest, playerRecoveryConfig.Value.PlayerName, "", playerRecoveryConfig.Key.ToString());
                    }

                    foreach (KeyValuePair<long, PlayerRecoveryConfig> playerRecoveryConfig in modConfig.PlayerRecoveryConfig)
                    {
                        PlayerRecoveryConfig recoveryConfig = playerRecoveryConfig.Value;
                        api.StartNewPage(manifest, playerRecoveryConfig.Key.ToString());
                        api.OverridePageDisplayName(manifest, playerRecoveryConfig.Key.ToString(), recoveryConfig.PlayerName);
                        api.RegisterSimpleOption(manifest, "In Game Config Changes", "Let in game events change the farmer's recovery config.", () => !recoveryConfig.DisableRecoveryConfigInGameChanges, (bool val) => recoveryConfig.DisableRecoveryConfigInGameChanges = !val);
                        api.RegisterSimpleOption(manifest, "Recovery Service", recoveryServiceDescription, () => recoveryConfig.EnableRecoveryService, (bool val) => modConfig.EnableRecoveryService = val);
                        api.RegisterSimpleOption(manifest, "Recover All Items", recoverAllItemsDescription, () => recoveryConfig.RecoverAllItems, (bool val) => recoveryConfig.RecoverAllItems = val);
                        api.RegisterSimpleOption(manifest, "Recover For Free", recoverForFreeDescription, () => recoveryConfig.RecoverForFree, (bool val) => recoveryConfig.RecoverForFree = val);
                        api.RegisterSimpleOption(manifest, "Clear Lost Items", clearLostItemsDescription, () => !recoveryConfig.DisableClearLostItemsOnRandomRecovery, (bool val) => recoveryConfig.DisableClearLostItemsOnRandomRecovery = !val);
                        api.RegisterPageLabel(manifest, "Return", "Back to the main page", "");
                    }
                }
            }
        }

        internal static void DeleteConfigMenu(IManifest manifest)
        {
            GetApi()?.UnregisterModConfig(MailServicesModEntry.Manifest);
        }

        private static GenericModConfigMenuApi GetApi()
        {
            return _api ??= DataLoader.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        }
    }
}
