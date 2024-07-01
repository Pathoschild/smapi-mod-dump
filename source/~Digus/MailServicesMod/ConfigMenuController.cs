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
            if (GetApi() is not { } api) return;
            api.Register(manifest, () => DataLoader.ModConfig = new ModConfig(), () => DataLoader.Helper.WriteConfig(DataLoader.ModConfig));

            api.AddSectionTitle(manifest, () => "Enable Services:", () => "");
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableGiftService, (bool val) => DataLoader.ModConfig.DisableGiftService = !val, () => "Gift Service", () => "Let you send gifts to the villagers using the mailbox.");
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableQuestService, (bool val) => DataLoader.ModConfig.DisableQuestService = !val, () => "Quest Service", () => "Let you send items to complete quests using the mailbox.");
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableToolDeliveryService, (bool val) => DataLoader.ModConfig.DisableToolDeliveryService = !val, () => "Tool Delivery Service", () => "You will receive upgraded tools in the mailbox.");
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableToolShipmentService, (bool val) => DataLoader.ModConfig.DisableToolShipmentService = !val, () => "Tool Shipment Service", () => "Let you send tools to upgrade using the mailbox.");

            api.AddSectionTitle(manifest, () => "Services Fees:", () => "Fee per use of service.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.GiftServiceFee, (int val) => DataLoader.ModConfig.GiftServiceFee = val, () => "Gift Shipment (G)", () => "How much gold you'll be charged for sending gifts.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.GiftServicePercentFee, (int val) => DataLoader.ModConfig.GiftServicePercentFee = val, () => "Gift Shipment (%)", () => "How much in gift value percentage you'll be charged for sending gifts.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.QuestServiceFee, (int val) => DataLoader.ModConfig.QuestServiceFee = val, () => "Quest Item Shipment (G)", () => "How much gold you'll be charged for sending quest items.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.ToolShipmentServiceFee, (int val) => DataLoader.ModConfig.ToolShipmentServiceFee = val, () => "Tool Shipment (G)", () => "How much extra gold you'll be charged for sending tools to upgrade.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.ToolShipmentServicePercentFee, (int val) => DataLoader.ModConfig.ToolShipmentServicePercentFee = val, () => "Tool Shipment (%)", () => "How much extra in tool upgrade cost percentage you'll be charged for sending tools to upgrade.");

            api.AddSectionTitle(manifest, () => "General:", () => "");
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.ShowDialogOnItemDelivery, (bool val) => DataLoader.ModConfig.ShowDialogOnItemDelivery = val, () => "Show Dialog On Shipment", () => "Show the npc dialog as if you were delivering something in person. Works for gifts and quest completion.");
                
            api.AddSectionTitle(manifest, () => "Tool Upgrade Service:", () => "Properties related to the tool upgrade service.");
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableAskToUpgradeTool, (bool val) => DataLoader.ModConfig.EnableAskToUpgradeTool = val, () => "Ask to Upgrade Tool", () => "When placing the tool in the mailbox you will have to confirm if you want to upgrade it.");

            api.AddSectionTitle(manifest, () => "Gift Service:", () => "Properties related to the gift service.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.MinimumFriendshipPointsToSendGift, (int val) => DataLoader.ModConfig.MinimumFriendshipPointsToSendGift = val, () => "Minimum Friendship Points", () => "Friendship points needed to send gifts to a NPC. 250 friendship points equal 1 heart level.");
            api.AddNumberOption(manifest, () => DataLoader.ModConfig.GiftChoicePageSize, (int val) => DataLoader.ModConfig.GiftChoicePageSize = val, () => "NPC Page Size", () => "Number of villagers shown per page on gift shipment.");
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableJealousyFromMailedGifts, (bool val) => DataLoader.ModConfig.EnableJealousyFromMailedGifts = val, () => "Jealousy", () => "Make it possible for your spouse to be jealous of gifts sent by mail like of gifts given in person.");
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableGiftToNpcWithMaxFriendship, (bool val) => DataLoader.ModConfig.EnableGiftToNpcWithMaxFriendship = val, () => "Max Friendship", () => "Make it possible to send gifts to friends with maxed friendship.");

            string recoveryServiceDescription = "You'll receive the items you lost on passing out in the mailbox, according to the other config options.";
            string recoverAllItemsDescription = "You'll receive all lost items you can pay for in the mailbox.";
            string recoverForFreeDescription = "You won't have to pay for the recovered items.";
            string clearLostItemsDescription = "If 'recover all items' isn't enabled, once a random item is recovered, the others are cleared and lost forever. If disabled, items won't be cleared and you'll receive an random item each day until all are recovered, or you chose an item with Marlon, or you pass out again.";

            api.AddSectionTitle(manifest, () => "Recovery Service (Default):", () => "Properties related to the recovery of lost items. This properties are the ones used if you don't enable per framer configuration. They're also the ones that a farmer starts with.");
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableRecoveryConfigInGameChanges, (bool val) => DataLoader.ModConfig.DisableRecoveryConfigInGameChanges = !val, () => "In Game Config Changes", () => "Let in game events change the farmer's recovery config. If per farmer config is disabled, the default properties will be changed.");
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableRecoveryService, (bool val) => DataLoader.ModConfig.EnableRecoveryService = val, () => "Recovery Service", () => recoveryServiceDescription);
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.RecoverAllItems, (bool val) => DataLoader.ModConfig.RecoverAllItems = val, () => "Recover All Items", () => recoverAllItemsDescription);
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.RecoverForFree, (bool val) => DataLoader.ModConfig.RecoverForFree = val, () => "Recover For Free", () => recoverForFreeDescription);
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisableClearLostItemsOnRandomRecovery, (bool val) => DataLoader.ModConfig.DisableClearLostItemsOnRandomRecovery = !val, () => "Clear Lost Items", () => clearLostItemsDescription);
            api.AddBoolOption(manifest, () => !DataLoader.ModConfig.DisablePerPlayerConfig, (bool val) => DataLoader.ModConfig.DisablePerPlayerConfig = !val, () => "Per Farmer Config", () => "Farmers recovery config will be tracked individually.");

            if (DataLoader.ModConfig.PlayerRecoveryConfig.Count > 0)
            {
                api.AddSectionTitle(manifest, () => "Recovery Config - Farmers List:", () => "Once you open a save file or create a new game, the farmer config should be tracked here.");

                foreach (KeyValuePair<long, PlayerRecoveryConfig> playerRecoveryConfig in DataLoader.ModConfig.PlayerRecoveryConfig)
                {
                    api.AddPageLink(manifest, playerRecoveryConfig.Key.ToString(), () => playerRecoveryConfig.Value.PlayerName, () => "");
                }

                foreach (KeyValuePair<long, PlayerRecoveryConfig> playerRecoveryConfig in DataLoader.ModConfig.PlayerRecoveryConfig)
                {
                    PlayerRecoveryConfig recoveryConfig = playerRecoveryConfig.Value;
                    api.AddPage(manifest, playerRecoveryConfig.Key.ToString(), () => recoveryConfig.PlayerName);
                    api.AddBoolOption(manifest, () => !recoveryConfig.DisableRecoveryConfigInGameChanges, (bool val) => recoveryConfig.DisableRecoveryConfigInGameChanges = !val, () => "In Game Config Changes", () => "Let in game events change the farmer's recovery config.");
                    api.AddBoolOption(manifest, () => recoveryConfig.EnableRecoveryService, (bool val) => DataLoader.ModConfig.EnableRecoveryService = val, () => "Recovery Service", () => recoveryServiceDescription);
                    api.AddBoolOption(manifest, () => recoveryConfig.RecoverAllItems, (bool val) => recoveryConfig.RecoverAllItems = val, () => "Recover All Items", () => recoverAllItemsDescription);
                    api.AddBoolOption(manifest, () => recoveryConfig.RecoverForFree, (bool val) => recoveryConfig.RecoverForFree = val, () => "Recover For Free", () => recoverForFreeDescription);
                    api.AddBoolOption(manifest, () => !recoveryConfig.DisableClearLostItemsOnRandomRecovery, (bool val) => recoveryConfig.DisableClearLostItemsOnRandomRecovery = !val, () => "Clear Lost Items", () => clearLostItemsDescription);
                    api.AddPageLink(manifest, "Return", () => "Back to the main page", () => "");
                }
            }
        }

        internal static void DeleteConfigMenu(IManifest manifest)
        {
            GetApi()?.Unregister(MailServicesModEntry.Manifest);
        }

        private static GenericModConfigMenuApi GetApi()
        {
            return _api ??= DataLoader.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        }
    }
}
