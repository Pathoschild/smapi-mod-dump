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
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && SDate.Now() >= new SDate(2, "spring", 1)
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
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && SDate.Now() >= new SDate(6, "spring", 1)
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
                {
                    Title = I18N.Get("Shipment.Clint.UpgradeLetter.Title")
                }
            );

            Letter upgradeLetter = new Letter(
                ToolUpgradeMailId
                , I18N.Get("Delivery.Clint.UpgradeLetter")
                , (l) => Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0
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
                Title = I18N.Get("Delivery.Clint.UpgradeLetter.Title")
            };
            upgradeLetter.DynamicItems = (l) => Game1.player.toolBeingUpgraded.Value != null ? new List<Item> {Game1.player.toolBeingUpgraded.Value} : new List<Item>();
            MailDao.SaveLetter(upgradeLetter);
        }
    }
}
