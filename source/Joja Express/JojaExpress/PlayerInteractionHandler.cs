/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace JojaExpress
{
    public class PlayerInteractionHandler
    {
        public static void checkInv(object? sender, InventoryChangedEventArgs args)
        {
            foreach (Item item in args.Added)
            {
                if (item is StardewValley.Object obj && obj.QualifiedItemId == "(O)ofts.jojaExp.item.package.global" && obj.modData.Count() == 0 && ModEntry.tobeReceived.Count > 1)
                {
                    foreach (KeyValuePair<string, int> p in ModEntry.tobeReceived[0])
                    {
                        obj.modData.Add(p.Key, p.Value.ToString());
                    }
                    ModEntry.tobeReceived.RemoveAt(0);
                    if (!Context.IsMainPlayer) ModEntry._Helper.Multiplayer.SendMessage(1, "ofts.jojaExp.tobeReceivedPoped");
                }
            }
        }

        public static void sendMail(object? sender, DayEndingEventArgs e)
        {
            if (ModEntry.needMail)
                Game1.player.mailForTomorrow.Add("ofts.jojaExp.mail");
            ModEntry.needMail = false;
        }

        public static bool sentPurchasedItem(ISalable item, Farmer farmer, int amt)
        {
            if (ModEntry.localReceived.ContainsKey(item.QualifiedItemId)) ModEntry.localReceived[item.QualifiedItemId] += amt;
            else ModEntry.localReceived.Add(item.QualifiedItemId, amt);

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu menu) menu.heldItem = null;
            return false;
        }

        public static bool mailPurchasedItem(ISalable item, Farmer farmer, int amt)
        {
            if (ModEntry.tobeReceived.Last().ContainsKey(item.QualifiedItemId)) ModEntry.tobeReceived.Last()[item.QualifiedItemId] += amt;
            else ModEntry.tobeReceived.Last().Add(item.QualifiedItemId, amt);

            if (!Context.IsMainPlayer) ModEntry._Helper.Multiplayer.SendMessage(new KeyValuePair<string, int>(item.QualifiedItemId, amt), "ofts.jojaExp.tobeReceivedAdded");

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu menu) menu.heldItem = null;
            ModEntry.needMail = true;
            return false;
        }

        public static void openMenu()
        {
            List<KeyValuePair<string, string>> responses = new();
            if (!(ModEntry.fee_state == 1 && ModEntry.config.CloseWhenCCComplete))
                responses.Add(new KeyValuePair<string, string>("local", ModEntry._Helper.Translation.Get("local")));
            responses.Add(new KeyValuePair<string, string>("global", ModEntry._Helper.Translation.Get("global")));
            responses.Add(new KeyValuePair<string, string>("qi", ModEntry._Helper.Translation.Get("qi")));
            responses.Add(new KeyValuePair<string, string>("help", ModEntry._Helper.Translation.Get("help")));
            Game1.currentLocation.ShowPagedResponses(ModEntry._Helper.Translation.Get("prompt"), responses, delegate (string callId)
            {
                switch (callId)
                {
                    case "local":
                        {
                            GUI.openMenu("ofts.JojaExp.jojaLocal", sentPurchasedItem, GUI.getPostFixForLocalItem);
                            ModEntry.localReceived = new();
                            break;
                        }
                    case "global":
                        {
                            GUI.openMenu("ofts.JojaExp.jojaGlobal", mailPurchasedItem, GUI.getPostFixForItem);
                            break;
                        }
                    case "qi":
                        {
                            Utility.TryOpenShopMenu("QiGemShop", "AnyOrNone");
                            break;
                        }
                    case "help":
                        {
                            Game1.multipleDialogues(
                                ModEntry._Helper.Translation.Get(
                                    "info", new { percent = (ModEntry.getPriceModifier() - 1).ToString("P1") }
                                ).ToString().Split('$')
                            );
                            break;
                        }
                }
            });
        }

        public static void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (ModEntry.config != null && ModEntry.config.Open.JustPressed() && ModEntry.config.OpenByKey && 
                DataLoader.Shops(Game1.content).ContainsKey("ofts.JojaExp.jojaLocal") && 
                Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                openMenu();
                return;
            }
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null) return;
            bool found = false;
            foreach (InputButton key in Game1.options.actionButton)
            {
                if (e.IsDown(key.ToSButton()))
                {
                    found = true;
                    break;
                }
            }
            foreach (InputButton key in Game1.options.useToolButton)
            {
                if (e.IsDown(key.ToSButton()))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return;
            if (Game1.player.ActiveObject != null && (
                Game1.player.ActiveObject.QualifiedItemId == "(O)ofts.jojaExp.item.package.global" ||
                Game1.player.ActiveObject.QualifiedItemId == "(O)ofts.jojaExp.item.package.local"))
            {
                foreach (KeyValuePair<string, NetString> p in Game1.player.ActiveObject.modData.FieldDict)
                {
                    try
                    {
                        for (int cnt = int.Parse(p.Value.Value); cnt > 0; cnt -= 999)
                        {
                            Game1.currentLocation.debris.Add(Game1.createItemDebris(ItemRegistry.Create(p.Key, Math.Min(cnt, 999)), Game1.player.Position, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        ModEntry._Monitor.Log($"JojaExp cannot unpack the following item: \n{ex}", LogLevel.Error);
                    }
                }
                Game1.player.reduceActiveItemByOne();
            }
            else if (Game1.player.ActiveItem != null && ModEntry.config != null && ModEntry.config.OpenByPad &&
                Game1.player.ActiveItem.QualifiedItemId == "(T)ofts.jojaExp.item.jpad" && 
                DataLoader.Shops(Game1.content).ContainsKey("ofts.JojaExp.jojaLocal") && 
                Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                openMenu();
            }
        }
    }
}
