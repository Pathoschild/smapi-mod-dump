using Harmony;
using Microsoft.Xna.Framework;
using StardewMods.Common.StardewValley;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Patches
{
    public class AddItemToInventoryBoolPatch
    {
        private static Farmer farmer;

        /// <summary>Apply the Harmony patch.</summary>
        /// <param name="harmony">The Harmony instance.</param>
        public void Apply(HarmonyInstance harmony)
        {
            MethodBase method = AccessTools.Method(typeof(Farmer), "addItemToInventoryBool");

            MethodInfo prefix = AccessTools.Method(this.GetType(), nameof(AddItemToInventoryBoolPatch.Prefix));
            MethodInfo postfix = AccessTools.Method(this.GetType(), nameof(AddItemToInventoryBoolPatch.Postfix));

            harmony.Patch(method, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        }

        public static bool Prefix(Farmer __instance)
        {
            farmer = __instance;

            return false;
        }

        public static bool Postfix(bool __result, Item item, bool makeActiveObject = false)
        {
            if (item == null)
            {
                return false;
            }

            int stack = item.Stack;
            Item obj1 = farmer.IsLocalPlayer ? farmer.addItemToInventory(item) : (Item)null;

            // Patch: Added [... || obj1.ParentSheetIndex == Constants.GAME_OBJECT_LOST_BOOK_ID]
            bool flag = obj1 == null || obj1.Stack != item.Stack || item is SpecialItem || obj1.ParentSheetIndex == Constants.ID_GAME_OBJECT_LOST_BOOK;
            if (item is Object)
            {
                (item as Object).reloadSprite();
            }

            if (!flag || !farmer.IsLocalPlayer)
            {
                return false;
            }

            if (item != null)
            {
                if (farmer.IsLocalPlayer && !item.hasBeenInInventory)
                {
                    if (item is SpecialItem)
                    {
                        (item as SpecialItem).actionWhenReceived(farmer);
                        return true;
                    }
                    if (item is Object && (item as Object).specialItem)
                    {
                        if ((item as Object).bigCraftable.Value || item is Furniture)
                        {
                            if (!farmer.specialBigCraftables.Contains((item as Object).ParentSheetIndex))
                            {
                                farmer.specialBigCraftables.Add((item as Object).ParentSheetIndex);
                            }
                        }
                        else if (!farmer.specialItems.Contains((item as Object).ParentSheetIndex))
                        {
                            farmer.specialItems.Add((item as Object).ParentSheetIndex);
                        };
                    }

                    if (item is Object && (item as Object).Category == -2 && !(item as Object).HasBeenPickedUpByFarmer)
                    {
                        farmer.foundMineral((item as Object).ParentSheetIndex);
                    }
                    else if (!(item is Furniture) && item is Object && ((item as Object).Type != (string)null && (item as Object).Type.Contains("Arch")) && !(item as Object).HasBeenPickedUpByFarmer)
                    {
                        farmer.foundArtifact((item as Object).ParentSheetIndex, 1);
                    }

                    if (item.ParentSheetIndex == Constants.ID_GAME_OBJECT_LOST_BOOK)
                    {
                        farmer.foundArtifact((item as Object).ParentSheetIndex, 1);
                        farmer.removeItemFromInventory(item);
                    }
                    else
                    {
                        switch (item.ParentSheetIndex)
                        {
                            case 378:
                                Game1.stats.CopperFound += (uint)item.Stack;
                                break;
                            case 380:
                                Game1.stats.IronFound += (uint)item.Stack;
                                break;
                            case 384:
                                Game1.stats.GoldFound += (uint)item.Stack;
                                break;
                            case 386:
                                Game1.stats.IridiumFound += (uint)item.Stack;
                                break;
                        }
                    }
                }

                if (item is Object && !item.hasBeenInInventory)
                {
                    if (!(item is Furniture) && !(item as Object).bigCraftable.Value && !(item as Object).HasBeenPickedUpByFarmer)
                    {
                        farmer.checkForQuestComplete((NPC)null, (item as Object).ParentSheetIndex, (item as Object).Stack, item, (string)null, 9, -1);
                    }

                    (item as Object).HasBeenPickedUpByFarmer = true;

                    if ((item as Object).questItem.Value)
                    {
                        return true;
                    }

                    if (Game1.activeClickableMenu == null)
                    {
                        switch ((item as Object).ParentSheetIndex)
                        {
                            case Constants.ID_GAME_OBJECT_LOST_BOOK:
                                ++Game1.stats.NotesFound;
                                Game1.playSound("newRecipe");
                                farmer.holdUpItemThenMessage(item, true);

                                return true;
                            case 378:
                                if (!Game1.player.hasOrWillReceiveMail("copperFound"))
                                {
                                    Game1.addMailForTomorrow("copperFound", true, false);
                                    break;
                                }
                                break;
                            case 390:
                                ++Game1.stats.StoneGathered;
                                if (Game1.stats.StoneGathered >= 100U && !Game1.player.hasOrWillReceiveMail("robinWell"))
                                {
                                    Game1.addMailForTomorrow("robinWell", false, false);
                                    break;
                                }
                                break;
                            case 535:
                                if (!Game1.player.hasOrWillReceiveMail("geodeFound"))
                                {
                                    farmer.mailReceived.Add("geodeFound");
                                    farmer.holdUpItemThenMessage(item, true);
                                    break;
                                }
                                break;
                        }
                    }
                }

                Color color = Color.WhiteSmoke;
                string displayName = item.DisplayName;
                if (item is Object)
                {
                    string type = (item as Object).Type;
                    if (!(type == "Arch"))
                    {
                        if (!(type == "Fish"))
                        {
                            if (!(type == "Mineral"))
                            {
                                if (!(type == "Vegetable"))
                                {
                                    if (type == "Fruit")
                                        color = Color.Pink;
                                }
                                else
                                    color = Color.PaleGreen;
                            }
                            else
                                color = Color.PaleVioletRed;
                        }
                        else
                            color = Color.SkyBlue;
                    }
                    else
                    {
                        color = Color.Tan;
                        displayName += Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1954");
                    }
                }
                if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ItemGrabMenu))
                {
                    Game1.addHUDMessage(new HUDMessage(displayName, Math.Max(1, item.Stack), true, color, item));
                }

                if (farmer.freezePause <= 0)
                {
                    farmer.mostRecentlyGrabbedItem = item;
                }

                if (obj1 != null & makeActiveObject && item.Stack <= 1)
                {
                    int indexOfInventoryItem = farmer.getIndexOfInventoryItem(item);
                    Item obj2 = farmer.Items[farmer.CurrentToolIndex];
                    farmer.Items[farmer.CurrentToolIndex] = farmer.Items[indexOfInventoryItem];
                    farmer.Items[indexOfInventoryItem] = obj2;
                }
            }
            if (item is Object && !item.hasBeenInInventory)
            {
                farmer.checkForQuestComplete((NPC)null, item.ParentSheetIndex, item.Stack, item, "", 10, -1);
            }

            item.hasBeenInInventory = true;
            return flag;
        }
    }
}
