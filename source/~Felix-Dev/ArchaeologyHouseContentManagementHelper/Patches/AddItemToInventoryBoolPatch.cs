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

using SObject = StardewValley.Object;

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

            // Patch: Added [... || obj1.ParentSheetIndex == Constants.ID_GAME_OBJECT_LOST_BOOK]
            bool flag = obj1 == null || obj1.Stack != item.Stack || item is SpecialItem || obj1.ParentSheetIndex == Constants.ID_GAME_OBJECT_LOST_BOOK;
            if (item is SObject)
            {
                (item as SObject).reloadSprite();
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
                    if (item is SObject && (item as SObject).specialItem)
                    {
                        if ((item as SObject).bigCraftable.Value || item is Furniture)
                        {
                            if (!farmer.specialBigCraftables.Contains((item as SObject).ParentSheetIndex))
                            {
                                farmer.specialBigCraftables.Add((item as SObject).ParentSheetIndex);
                            }
                        }
                        else if (!farmer.specialItems.Contains((item as SObject).ParentSheetIndex))
                        {
                            farmer.specialItems.Add((item as SObject).ParentSheetIndex);
                        };
                    }

                    if (item is SObject && (item as SObject).Category == -2 && !(item as SObject).HasBeenPickedUpByFarmer)
                    {
                        farmer.foundMineral((item as SObject).ParentSheetIndex);
                    }
                    else if (!(item is Furniture) && item is SObject && ((item as SObject).Type != (string)null && (item as SObject).Type.Contains("Arch")) && !(item as SObject).HasBeenPickedUpByFarmer)
                    {
                        farmer.foundArtifact((item as SObject).ParentSheetIndex, 1);
                    }

                    if (item.ParentSheetIndex == Constants.ID_GAME_OBJECT_LOST_BOOK)
                    {
                        farmer.foundArtifact((item as SObject).ParentSheetIndex, 1);
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

                if (item is SObject && !item.hasBeenInInventory)
                {
                    if (!(item is Furniture) && !(item as SObject).bigCraftable.Value && !(item as SObject).HasBeenPickedUpByFarmer)
                    {
                        farmer.checkForQuestComplete((NPC)null, (item as SObject).ParentSheetIndex, (item as SObject).Stack, item, (string)null, 9, -1);
                    }

                    (item as SObject).HasBeenPickedUpByFarmer = true;

                    if ((item as SObject).questItem.Value)
                    {
                        return true;
                    }

                    if (Game1.activeClickableMenu == null)
                    {
                        switch ((item as SObject).ParentSheetIndex)
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
                if (item is SObject)
                {
                    string type = (item as SObject).Type;
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
            if (item is SObject && !item.hasBeenInInventory)
            {
                farmer.checkForQuestComplete((NPC)null, item.ParentSheetIndex, item.Stack, item, "", 10, -1);
            }

            item.hasBeenInInventory = true;
            return flag;
        }
    }
}
