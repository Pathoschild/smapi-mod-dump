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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using SObject = StardewValley.Object;

namespace MailServicesMod
{
    internal class ItemShipmentQuestOverrides
    {
        public static bool mailbox()
        {
            try
            {
                if (!DataLoader.ModConfig.DisableQuestService && Game1.player.mailbox.Count == 0 && Game1.player.ActiveObject != null)
                {
                    foreach (Quest quest in Game1.player.questLog)
                    {
                        if (quest is ItemDeliveryQuest itemDeliveryQuest && !itemDeliveryQuest.completed.Value)
                        {
                            var item = Game1.player.ActiveObject;
                            if ((item.ParentSheetIndex == itemDeliveryQuest.item.Value || item.Category == itemDeliveryQuest.item.Value))
                            {
                                NPC npc = NpcUtility.getCharacterFromName(itemDeliveryQuest.target.Value);
                                if (item.Stack >= itemDeliveryQuest.number.Value)
                                {
                                    if (Game1.player.Money >= DataLoader.ModConfig.QuestServiceFee)
                                    {
                                        ShopMenu.chargePlayer(Game1.player, 0, DataLoader.ModConfig.QuestServiceFee);
                                        Game1.player.ActiveObject.Stack -= (int)itemDeliveryQuest.number.Value - 1;
                                        if (DataLoader.ModConfig.ShowDialogOnItemDelivery)
                                        {
                                            itemDeliveryQuest.reloadDescription();
                                            npc.CurrentDialogue.Push(new Dialogue(itemDeliveryQuest.targetMessage, npc));
                                            Game1.drawDialogue(npc);
                                        }
                                        Game1.player.reduceActiveItemByOne();
                                        if ((bool)itemDeliveryQuest.dailyQuest.Value)
                                        {
                                            Game1.player.changeFriendship(150, npc);
                                            if (itemDeliveryQuest.deliveryItem.Value == null)
                                            {
                                                itemDeliveryQuest.deliveryItem.Value = new SObject(Vector2.Zero, itemDeliveryQuest.item.Value, 1);
                                            }
                                            itemDeliveryQuest.moneyReward.Value = itemDeliveryQuest.deliveryItem.Value.Price * 3;
                                        }
                                        else
                                        {
                                            Game1.player.changeFriendship(255, npc);
                                        }
                                        itemDeliveryQuest.questComplete();
                                    }
                                    else
                                    {
                                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Quest.ItemDelivery.NoMoney"));
                                    }
                                }
                                else
                                {
                                    if (DataLoader.ModConfig.ShowDialogOnItemDelivery)
                                    {
                                        npc.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13615", itemDeliveryQuest.number.Value), npc));
                                        Game1.drawDialogue(npc);
                                    }
                                    else
                                    {
                                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Quest.ItemDelivery.NotEnoughItems", new { Npc = npc.displayName, Number = itemDeliveryQuest.number.Value }));
                                    }
                                }
                                return false;
                            }
                        }
                        else if (quest is LostItemQuest lostItemQuest && !lostItemQuest.completed.Value)
                        {
                            if (lostItemQuest.itemFound.Value && Game1.player.hasItemInInventory(lostItemQuest.itemIndex.Value, 1))
                            {
                                lostItemQuest.questComplete();
                                NPC npc = NpcUtility.getCharacterFromName(lostItemQuest.npcName.Value);

                                if (DataLoader.ModConfig.ShowDialogOnItemDelivery)
                                {
                                    Dictionary<int, string> questData = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
                                    string thankYou = (questData[lostItemQuest.id.Value].Length > 9) ? questData[lostItemQuest.id.Value].Split('/')[9] : Game1.content.LoadString("Data\\ExtraDialogue:LostItemQuest_DefaultThankYou");
                                    npc.setNewDialogue(thankYou);
                                    Game1.drawDialogue(npc);
                                }
                                Game1.player.changeFriendship(250, npc);
                                Game1.player.removeFirstOfThisItemFromInventory(lostItemQuest.itemIndex.Value);
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to send object to complete quest.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
            return true;
        }
    }
}
