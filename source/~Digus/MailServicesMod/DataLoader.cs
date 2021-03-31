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
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;

namespace MailServicesMod
{
    internal class DataLoader
    {
        internal const string ToolUpgradeMailId = "MailServicesMod.ToolUpgrade";
        internal const string ItemRecoveryMailId = "MailServicesMod.ItemRecovery";
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;

        public DataLoader(IModHelper modHelper)
        {
            Helper = modHelper;
            I18N = modHelper.Translation;
            ModConfig = Helper.ReadConfig<ModConfig>();

            MailDao.SaveLetter(
                new Letter(
                    "MailServiceMod.DeliveryQuestsInfo"
                    , I18N.Get("Shipment.Quest.DeliveryQuestsLetter")
                    , (l) => !DataLoader.ModConfig.DisableQuestService && !Game1.player.mailReceived.Contains(l.Id) && SDate.Now() >= new SDate(2, "spring", 1)
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
                {
                    Title = I18N.Get("Shipment.Quest.DeliveryQuestsLetter.Title")
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    "MailServiceMod.ToolUpgradeInfo"
                    , I18N.Get("Shipment.Clint.UpgradeLetter")
                    , (l) => !DataLoader.ModConfig.DisableToolShipmentService && !Game1.player.mailReceived.Contains(l.Id) && SDate.Now() >= new SDate(6, "spring", 1)
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
                {
                    Title = I18N.Get("Shipment.Clint.UpgradeLetter.Title")
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    "MailServiceMod.GiftShipmentInfo"
                    , I18N.Get("Shipment.Wizard.GiftShipmentLetter")
                    , (l) => !DataLoader.ModConfig.DisableGiftService && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.eventsSeen.Contains(112)
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
                {
                    Title = I18N.Get("Shipment.Wizard.GiftShipmentLetter.Title"),
                    WhichBG = 2
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    "MailServiceMod.MarlonRecoveryReward"
                    , I18N.Get("Delivery.Marlon.RecoveryRewardLetter")
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && Game1.player.hasCompletedAllMonsterSlayerQuests.Value && !GetRecoveryConfig(Game1.player).DisableRecoveryConfigInGameChanges
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id);
                        SaveRecoveryConfig(Game1.player, true, true, true);
                    }
                )
                {
                    Title = I18N.Get("Delivery.Marlon.RecoveryRewardLetter.Title"),
                    GroupId = "MailServicesMod.GuildRecovery"
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    "MailServicesMod.MarlonRecoveryOffer"
                    , I18N.Get("Delivery.Marlon.RecoveryOfferLetter")
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("MailServiceMod.MarlonRecoveryReward") && Game1.player.mailReceived.Contains("guildMember") && !DataLoader.GetRecoveryConfig(Game1.player).EnableRecoveryService && !GetRecoveryConfig(Game1.player).DisableRecoveryConfigInGameChanges
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id);
                        GuildRecoveryController.OpenOfferDialog();
                    }
                )
                {
                    Title = I18N.Get("Delivery.Marlon.RecoveryOfferLetter.Title"),
                    GroupId = "MailServicesMod.GuildRecovery"
                }
            );

            Letter upgradeLetter = new Letter(
                ToolUpgradeMailId
                , I18N.Get("Delivery.Clint.UpgradeLetter")
                , (l) => !DataLoader.ModConfig.DisableToolDeliveryService && Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0
                , (l) =>
                {
                    if(!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    if (Game1.player.toolBeingUpgraded.Value != null)
                    {
                        Tool tool = Game1.player.toolBeingUpgraded.Value;
                        Game1.player.toolBeingUpgraded.Value = null;
                        Game1.player.hasReceivedToolUpgradeMessageYet = false;
                        Game1.player.holdUpItemThenMessage(tool);
                        if (tool is GenericTool)
                        {
                            tool.actionWhenClaimed();
                        }
                        if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
                        {
                            Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                            multiplayer.globalChatInfoMessage("IridiumToolUpgrade", Game1.player.Name, tool.DisplayName);
                        }
                    }
                }
            )
            {
                Title = I18N.Get("Delivery.Clint.UpgradeLetter.Title"),
                DynamicItems = (l) => Game1.player.toolBeingUpgraded.Value != null ? new List<Item> { Game1.player.toolBeingUpgraded.Value } : new List<Item>()
            };
            MailDao.SaveLetter(upgradeLetter);

            Letter recoveryLetter = new Letter(
                ItemRecoveryMailId
                , I18N.Get("Delivery.Marlon.RecoveryLetter")
                , (l) => GetRecoveryConfig(Game1.player).EnableRecoveryService && GuildRecoveryController.GetItemsToRecover()?.Count > 0
                , (l) =>
                {
                    if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    GuildRecoveryController.ItemsRecovered();
                }
            )
            {
                Title = I18N.Get("Delivery.Marlon.RecoveryLetter.Title"),
                DynamicItems = (l) => GuildRecoveryController.GetItemsToRecover()
            };
            MailDao.SaveLetter(recoveryLetter);
        }

        public static void SaveRecoveryConfig(Farmer player, bool enableRecoveryService, bool? recoveryAllItems = null, bool? recoveryForFree = null)
        {
            RecoveryConfig recoveryConfig = GetRecoveryConfig(player);
            if (!recoveryConfig.DisableRecoveryConfigInGameChanges)
            {
                recoveryConfig.EnableRecoveryService = enableRecoveryService;
                recoveryConfig.RecoverAllItems = recoveryAllItems ?? recoveryConfig.RecoverAllItems;
                recoveryConfig.RecoverForFree = recoveryForFree ?? recoveryConfig.RecoverForFree;
                Helper.WriteConfig<ModConfig>(DataLoader.ModConfig);
            }
        }

        public static RecoveryConfig GetRecoveryConfig(Farmer player)
        {
            if (ModConfig.DisablePerPlayerConfig)
            {
                return ModConfig;
            }
            else
            {
                if (!ModConfig.PlayerRecoveryConfig.ContainsKey(player.UniqueMultiplayerID))
                {
                    ModConfig.PlayerRecoveryConfig[player.UniqueMultiplayerID] = new PlayerRecoveryConfig()
                    {
                        PlayerName = player.Name,
                        DisableRecoveryConfigInGameChanges = ModConfig.DisableRecoveryConfigInGameChanges,
                        EnableRecoveryService = ModConfig.EnableRecoveryService,
                        RecoverAllItems = ModConfig.RecoverAllItems,
                        RecoverForFree = ModConfig.RecoverForFree,
                        DisableClearLostItemsOnRandomRecovery = ModConfig.DisableClearLostItemsOnRandomRecovery
                    };
                    Helper.WriteConfig<ModConfig>(DataLoader.ModConfig);
                    ConfigMenuController.DeleteConfigMenu(MailServicesModEntry.Manifest);
                    ConfigMenuController.CreateConfigMenu(MailServicesModEntry.Manifest);
                }
                return ModConfig.PlayerRecoveryConfig[player.UniqueMultiplayerID];
            }
        }
    }
}
