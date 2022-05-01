/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Emit;
using HarmonyLib;

namespace FarmHouseRedone
{

    class Farm_resetLocalState_Patch
    {
        public static void Postfix(Farm __instance)
        {
            TemporaryAnimatedSprite shippingBinLid = FarmHouseStates.reflector.GetField<TemporaryAnimatedSprite>(__instance, "shippingBinLid").GetValue();
            Vector2 lidPosition = new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f + new Vector2(2f, -7f) * 4f;
            shippingBinLid.Position = lidPosition;
            shippingBinLid.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1) / 10000f;
        }
    }


    class Farm_resetSharedState_Patch
    {
        public static void Postfix(Farm __instance)
        {
            IReflectedField<NetRectangle> houseSource = FarmHouseStates.reflector.GetField<NetRectangle>(__instance, "houseSource");
            IReflectedField<NetRectangle> greenhouseSource = FarmHouseStates.reflector.GetField<NetRectangle>(__instance, "greenhouseSource");
            houseSource.SetValue(new NetRectangle(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0)));
            greenhouseSource.SetValue(new NetRectangle(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0)));
            FarmState.init();
            FarmState.setUpFarm(__instance);
        }
    }

    class Farm_leftClick_Patch
    {
        public static Tuple<int,int> getPositionChecks(List<CodeInstruction> codes)
        {
            int start = -1;
            int end = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && end != -1)
                {
                    Logger.Log("Ended the comparison section at " + i);
                    break;
                }
                if(codes[i].opcode == OpCodes.Blt || codes[i].opcode == OpCodes.Bgt)
                {
                    Logger.Log("Found a comparator...");
                    if(start == -1)
                    {
                        Logger.Log("Found beginning index at " + (i - 4));
                        start = i - 4;
                    }
                    else
                    {
                        Logger.Log("Found possible ending index at " + i);
                        end = i;
                    }
                }
                else
                {
                    Logger.Log("Index " + i + ": " + codes[i].opcode.Name + " " + codes[i].ToString());
                }
            }
            Logger.Log("Finished iterating through codes...");
            if(start == -1 || end == -1)
            {
                Logger.Log("Failed to find the position checks range!");
                return new Tuple<int, int>(-1, -1);
            }
            return new Tuple<int, int>(start, end - start);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newobj)
                {
                    Logger.Log("Vector2 set to (" + (FarmState.shippingCrateLocation.X + 0.5f) + ", " + FarmState.shippingCrateLocation.Y + ")");
                    codes[i - 2] = new CodeInstruction(OpCodes.Ldc_R4, FarmState.shippingCrateLocation.X + 0.5f);
                    codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, FarmState.shippingCrateLocation.Y);
                }
                if (codes[i].opcode == OpCodes.Blt)
                {
                    if (codes[i - 1].operand.ToString().Contains("71"))
                    {
                        Logger.Log("< 71 -> < " + FarmState.shippingCrateLocation.X + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.X);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("13"))
                    {
                        Logger.Log("< 13 -> < " + (FarmState.shippingCrateLocation.Y-1) + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.Y-1);
                    }
                }
                else if (codes[i].opcode == OpCodes.Bgt)
                {
                    if (codes[i - 1].operand.ToString().Contains("72"))
                    {
                        Logger.Log("> 72 -> > " + (FarmState.shippingCrateLocation.X+1) + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.X+1);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("14"))
                    {
                        Logger.Log("> 14 -> > " + FarmState.shippingCrateLocation.Y + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.Y);
                    }
                }
            }
            return codes;
        }


        public static bool Prefix(int x, int y, Farmer who, ref bool __result, Farm __instance)
        {
            if(FarmState.shippingCrateLocation.X != 71f || FarmState.shippingCrateLocation.Y != 14f)
            {
                if (who.ActiveObject == null || x / 64 < FarmState.shippingCrateLocation.X || (x / 64 > FarmState.shippingCrateLocation.X + 1 || y / 64 < FarmState.shippingCrateLocation.Y - 1) || (y / 64 > FarmState.shippingCrateLocation.Y || !who.ActiveObject.canBeShipped() || (double)Vector2.Distance(who.getTileLocation(), FarmState.shippingCrateLocation) > 2.0))
                {
                    return true;
                }
                __instance.getShippingBin(who).Add((Item)who.ActiveObject);
                __instance.lastItemShipped = (Item)who.ActiveObject;
                who.showNotCarrying();
                __instance.showShipment(who.ActiveObject, true);
                who.ActiveObject = null;
                __result = true;
                return false;
            }
            return true;
        }
    }

    class Farm_addGrandpaCandles_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString().Contains("LooseSprites\\Cursors"))
                {
                    int offset = codes[i+1].opcode == OpCodes.Ldloc_0 ? -1 : 0;
                    Logger.Log("Shrine anchor set to (" + (FarmState.shrineLocation.X) + ", " + FarmState.shrineLocation.Y + ")");
                    codes[i + 6 + offset] = new CodeInstruction(OpCodes.Ldc_R4, (Convert.ToInt32(codes[i + 6 + offset].operand.ToString()) - 512f) + FarmState.shrineLocation.X);
                    codes[i + 7 + offset] = new CodeInstruction(OpCodes.Ldc_R4, (Convert.ToInt32(codes[i + 7 + offset].operand.ToString()) - 448f) + FarmState.shrineLocation.Y);
                    string patchInfo = "Patched candle sprite addition:\n";
                    for(int patchI = -2; patchI < 12 && i + patchI < codes.Count; patchI++)
                    {
                        patchInfo += codes[i + patchI].ToString() + "\n";
                    }
                    Logger.Log(patchInfo);
                }
            }
            return codes;
        }
    }

    class Farm_checkAction_Patch
    {
        public static void shipItem(Item i, Farmer who)
        {
            if (i == null)
                return;
            who.removeItemFromInventory(i);
            Game1.getFarm().getShippingBin(who).Add(i);
            if (i is StardewValley.Object)
                Game1.getFarm().showShipment(i as StardewValley.Object, false);
            Game1.getFarm().lastItemShipped = i;
            if (Game1.player.ActiveObject != null)
                return;
            Game1.player.showNotCarrying();
            Game1.player.Halt();
        }

        public static bool Prefix(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result, Farm __instance)
        {
            if (tileLocation.X >= FarmState.shippingCrateLocation.X && tileLocation.X <= FarmState.shippingCrateLocation.X + 1 && (tileLocation.Y >= FarmState.shippingCrateLocation.Y - 1 && tileLocation.Y <= FarmState.shippingCrateLocation.Y))
            {
                //IReflectedMethod shipItem = FarmHouseStates.reflector.GetMethod(__instance, "shipItem");
                ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>)null, true, false, new InventoryMenu.highlightThisItem(StardewValley.Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(shipItem), "", (ItemGrabMenu.behaviorOnItemSelect)null, true, true, false, true, false, 0, (Item)null, -1, (object)__instance);
                itemGrabMenu.initializeUpperRightCloseButton();
                int num1 = 0;
                itemGrabMenu.setBackgroundTransparency(num1 != 0);
                int num2 = 1;
                itemGrabMenu.setDestroyItemOnClick(num2 != 0);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = (IClickableMenu)itemGrabMenu;
                __instance.playSound("shwip");
                if (Game1.player.FacingDirection == 1)
                    Game1.player.Halt();
                Game1.player.showCarrying();
                __result = true;
                return false;
            }
            return true;
        }

        public static Tuple<int,int> getComparisonSection(List<CodeInstruction> codes)
        {
            for(int i=0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Ldfld && codes[i + 4].opcode == OpCodes.Ldfld && codes[i + 8].opcode == OpCodes.Ldfld && codes[i + 12].opcode == OpCodes.Ldfld)
                {
                    return new Tuple<int, int>(i-1, i + 15);
                }
            }
            Logger.Log("Failed to find comparison section within checkAction!");
            return new Tuple<int, int>(-1, -1);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Tuple<int, int> comparisonRange = getComparisonSection(codes);

            if(comparisonRange.Item1 == -1 || comparisonRange.Item2 == -1)
            {
                Logger.Log("Could not patch checkAction!");
            }

            for (int i = comparisonRange.Item1; i < comparisonRange.Item2; i++)
            {
                if (codes[i].opcode == OpCodes.Blt)
                {
                    if (codes[i - 1].operand.ToString().Contains("71"))
                    {
                        Logger.Log("< 71 -> < " + FarmState.shippingCrateLocation.X + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.X);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("13"))
                    {
                        Logger.Log("< 13 -> < " + (FarmState.shippingCrateLocation.Y - 1) + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.Y - 1);
                    }
                }
                else if (codes[i].opcode == OpCodes.Bgt)
                {
                    if (codes[i - 1].operand.ToString().Contains("72"))
                    {
                        Logger.Log("> 72 -> > " + (FarmState.shippingCrateLocation.X + 1) + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.X + 1);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("14"))
                    {
                        Logger.Log("> 14 -> > " + FarmState.shippingCrateLocation.Y + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int)FarmState.shippingCrateLocation.Y);
                    }
                }
            }
            return codes;
        }
    }

    class Farm_showShipment_Patch
    {
        public static bool Prefix(StardewValley.Object o, bool playThrowSound, Farm __instance)
        {
            if (playThrowSound)
                __instance.localSound("backpackIN");
            DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0, (GameLocation)null);
            int num1 = Game1.random.Next();
            List<TemporaryAnimatedSprite> temporarySprites1 = __instance.temporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 218, 34, 22), new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f + new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite1.interval = 100f;
            temporaryAnimatedSprite1.totalNumberOfLoops = 1;
            temporaryAnimatedSprite1.animationLength = 3;
            temporaryAnimatedSprite1.pingPong = true;
            temporaryAnimatedSprite1.scale = 4f;
            temporaryAnimatedSprite1.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            double num2 = (double)num1;
            temporaryAnimatedSprite1.id = (float)num2;
            int num3 = num1;
            temporaryAnimatedSprite1.extraInfoForEndBehavior = num3;
            TemporaryAnimatedSprite.endBehavior endBehavior = new TemporaryAnimatedSprite.endBehavior(((GameLocation)__instance).removeTemporarySpritesWithID);
            temporaryAnimatedSprite1.endFunction = endBehavior;
            temporarySprites1.Add(temporaryAnimatedSprite1);
            List<TemporaryAnimatedSprite> temporarySprites2 = __instance.temporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 230, 34, 10), new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f + new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite2.interval = 100f;
            temporaryAnimatedSprite2.totalNumberOfLoops = 1;
            temporaryAnimatedSprite2.animationLength = 3;
            temporaryAnimatedSprite2.pingPong = true;
            temporaryAnimatedSprite2.scale = 4f;
            temporaryAnimatedSprite2.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            double num4 = (double)num1;
            temporaryAnimatedSprite2.id = (float)num4;
            int num5 = num1;
            temporaryAnimatedSprite2.extraInfoForEndBehavior = num5;
            temporarySprites2.Add(temporaryAnimatedSprite2);
            List<TemporaryAnimatedSprite> temporarySprites3 = __instance.temporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int)((NetFieldBase<int, NetInt>)o.parentSheetIndex), 16, 16), new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f + new Vector2((float)(8 + Game1.random.Next(6)), 2f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite3.interval = 9999f;
            temporaryAnimatedSprite3.scale = 4f;
            temporaryAnimatedSprite3.alphaFade = 0.045f;
            temporaryAnimatedSprite3.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            Vector2 vector2_1 = new Vector2(0.0f, 0.3f);
            temporaryAnimatedSprite3.motion = vector2_1;
            Vector2 vector2_2 = new Vector2(0.0f, 0.2f);
            temporaryAnimatedSprite3.acceleration = vector2_2;
            double num6 = -0.0500000007450581;
            temporaryAnimatedSprite3.scaleChange = (float)num6;
            temporarySprites3.Add(temporaryAnimatedSprite3);
            return false;
        }
    }

    class Farm_addSpouseOutdoorArea_Patch
    {
        public static bool Prefix(string spouseName, Farm __instance)
        {
            PatioManager.applyPatio(__instance, spouseName);
            return false;
        }
    }


    class Farm_UpdateWhenCurrentLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            //int getPorchIndex = -1;

            //for(int i = 0; i < codes.Count; i++)
            //{
            //    if(codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("getPorchStandingSpot()"))
            //    {
            //        Logger.Log("Found the getPorchStandingSpot() call!  Index " + i);
            //        if(codes[i-1].opcode == OpCodes.Ldloc_1)
            //        {
            //            Logger.Log("Found the ldloc.1!");
            //            getPorchIndex = i - 1;
            //            break;
            //        }
            //    }
            //}

            //if(getPorchIndex == -1)
            //{
            //    Logger.Log("Failed to locate getPorchStandingSpot()...");
            //    return codes;
            //}

            //codes.RemoveAt(getPorchIndex);
            //codes[getPorchIndex] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), "getPorchStandingSpot"));

            int hasActiveFireplace = findFarmHouseFirePlaceCall(codes);
            if(hasActiveFireplace != -1)
            {
                codes[hasActiveFireplace] = new CodeInstruction(OpCodes.Ldc_I4_0);
                codes.RemoveAt(hasActiveFireplace - 1);
            }

            injectChimneyUpdateCall(ref codes);

            return codes;
        }

        public static void injectChimneyUpdateCall(ref List<CodeInstruction> codes)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_MasterPlayer()"))
                {
                    codes.Insert(i, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.updateChimneySmoke))));
                    return;
                }
            }
        }

        public static int findFarmHouseFirePlaceCall(List<CodeInstruction> codes)
        {
            bool foundMasterPlayerCall = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_MasterPlayer()"))
                {
                    Logger.Log("Found the get_MasterPlayer() call!  Index " + i);
                    foundMasterPlayerCall = true;
                }
                if(foundMasterPlayerCall && codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("hasActiveFireplace"))
                {
                    return i;
                }
            }
            Logger.Log("Failed to locate the hasActiveFireplace call for the master farmhouse!");
            return -1;
        }
    }

    //[HarmonyPriority(Priority.Last)]
    class Farm_draw_Patch
    {

        //public static void baseDraw(Farm location, SpriteBatch b)
        //{
        //    IReflectedMethod drawCharacters = FarmHouseStates.reflector.GetMethod(location, "drawCharacters");
        //    IReflectedMethod drawFarmers = FarmHouseStates.reflector.GetMethod(location, "drawFarmers");
        //    drawCharacters.Invoke(b);
        //    foreach (Projectile projectile in location.projectiles)
        //        projectile.draw(b);
        //    drawFarmers.Invoke(b);
        //    if (location.critters != null)
        //    {
        //        for (int index = 0; index < location.critters.Count; ++index)
        //            location.critters[index].draw(b);
        //    }
        //    location.drawDebris(b);
        //    if (!Game1.eventUp || location.currentEvent != null && location.currentEvent.showGroundObjects)
        //    {
        //        foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
        //            pair.Value.draw(b, (int)pair.Key.X, (int)pair.Key.Y, 1f);
        //    }
        //    foreach (TemporaryAnimatedSprite temporarySprite in location.TemporarySprites)
        //        temporarySprite.draw(b, false, 0, 0, 1f);
        //    location.interiorDoors.Draw(b);
        //    if (location.largeTerrainFeatures != null)
        //    {
        //        foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
        //            largeTerrainFeature.draw(b);
        //    }
        //    if (location.fishSplashAnimation != null)
        //        location.fishSplashAnimation.draw(b, false, 0, 0, 1f);
        //    if (location.orePanAnimation == null)
        //        return;
        //    location.orePanAnimation.draw(b, false, 0, 0, 1f);
        //}


        //public static void buildingDraw(Farm location, SpriteBatch b)
        //{


        //    foreach (Building building in location.buildings)
        //    {
        //        building.draw(b);
        //    }
        //}

        public static Tuple<int, int> removeShippingBin(List<CodeInstruction> codes)
        {
            Logger.Log("Removing shipping bin drawing...");
            int start = -1;
            int end = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && (codes[i].operand != null && codes[i].operand.ToString().Contains("shippingBinLid")))
                {
                    Logger.Log("Shipping bin start: " + (i - 1));
                    start = i - 1;
                }
                if (start > -1 && codes[i].opcode == OpCodes.Callvirt && codes[i].operand != null && codes[i].operand.ToString().Contains("draw"))
                {
                    Logger.Log("Shipping bin end: " + i);
                    end = i + 1;
                    break;
                }
            }
            Logger.Log("Start: " + start + "\nEnd: " + end);
            return new Tuple<int, int>(start, end);
        }

        public static Tuple<int, int> removeMailBox(List<CodeInstruction> codes)
        {
            Logger.Log("Removing mailbox drawing...");
            int start = -1;
            int end = -1;
            int drawCalls = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Call && (codes[i].operand != null && codes[i].operand.ToString().Contains("get_mailbox()")))
                {
                    Logger.Log("Mailbox start: " + i);
                    start = i;
                }
                if(start > -1 && codes[i].opcode == OpCodes.Callvirt && codes[i].operand != null && codes[i].operand.ToString().Contains("Draw("))
                {
                    drawCalls++;
                    Logger.Log("Found mailbox draw call: " + i);
                    if(drawCalls > 1)
                    {
                        Logger.Log("Mailbox end: " + i);
                        end = i + 1;
                        break;
                    }
                }
            }
            Logger.Log("Start: " + start + "\nEnd: " + end);
            return new Tuple<int, int>(start, end);
        }

        public static Tuple<int, int> removeShadows(List<CodeInstruction> codes)
        {
            Logger.Log("Removing building shadow drawing...");
            int start = -1;
            int end = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && (codes[i].operand != null && codes[i].operand.ToString().Contains("leftShadow")))
                {
                    Logger.Log("Found left shadow token: " + i);
                    for(int j = 0; j < i; j++)
                    {
                        if (codes[j].opcode == OpCodes.Ldarg_1)
                        {
                            start = j;
                        }
                    }
                    Logger.Log("Building shadow start: " + start);
                }
                if (start > -1 && codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null && codes[i].operand.ToString().Contains("rightShadow"))
                {
                    for(int j = i; j < codes.Count; j++)
                    {
                        if(codes[j].opcode == OpCodes.Callvirt && codes[j].operand != null && codes[j].operand.ToString().Contains("Draw("))
                        {
                            Logger.Log("Building shadow end: " + j);
                            Logger.Log("Building shadow end was:\n" + codes[j].operand);
                            end = j + 1;
                            break;
                        }
                    }
                    if (end > -1)
                        break;
                }
            }
            Logger.Log("Start: " + start + "\nEnd: " + end);
            for(int i = start; i < end; i++)
            {
                if (codes[i] != null)
                    Logger.Log(codes[i].ToString());
            }
            return new Tuple<int, int>(start, end);
        }

        public static void removeShadowsNew(ref List<CodeInstruction> codes)
        {
            Logger.Log("Removing building shadow drawing...");
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && (codes[i].operand != null && codes[i].operand.ToString().Contains("leftShadow")))
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
                }
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null && codes[i].operand.ToString().Contains("middleShadow"))
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
                }
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null && codes[i].operand.ToString().Contains("rightShadow"))
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
                }
            }
        }

        public static List<CodeInstruction> makeShadowsCodes()
        {
            List<CodeInstruction> outCodes = new List<CodeInstruction>();
            outCodes.Add(new CodeInstruction(OpCodes.Ldarg_1));
            outCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), "draw", new Type[] { typeof(SpriteBatch) })));

            return outCodes;
        }

        public static void changeNote(ref List<CodeInstruction> codes)
        {
            bool foundHasSeen = false;
            bool adjustedSpot = false;
            string patchSummary = "Patched note draw code: \n";
            for(int i = 0; i < codes.Count; i++)
            {
                if(!foundHasSeen && codes[i].opcode == OpCodes.Ldfld && codes[i].operand != null && codes[i].operand.ToString().Contains("hasSeenGrandpaNote"))
                {
                    foundHasSeen = true;
                }
                if(foundHasSeen && !adjustedSpot && codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null && codes[i].operand.ToString().Contains("mouseCursors"))
                {
                    float x = (Convert.ToSingle(codes[i + 2].operand.ToString()) - 512f) + FarmState.shrineLocation.X;
                    float y = (Convert.ToSingle(codes[i + 3].operand.ToString()) - 448f) + FarmState.shrineLocation.Y;
                    codes[i + 2] = new CodeInstruction(OpCodes.Ldc_R4, x);
                    codes[i + 3] = new CodeInstruction(OpCodes.Ldc_R4, y);
                    patchSummary += "Position now (" + x + ", " + y + ")\n";
                    adjustedSpot = true;
                }
                if(adjustedSpot && codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand != null && codes[i].operand.ToString().Contains("0.04"))
                {
                    float depth = (FarmState.shrineLocation.Y + 64f) / 64000f;
                    patchSummary += "Depth now " + depth;
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, depth);
                }
            }
            Logger.Log(patchSummary);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int startIndex = -1;
            int endIndex = -1;

            var codes = new List<CodeInstruction>(instructions);
            Logger.Log("Codes count: " + codes.Count);


            Tuple<int, int> mailbox = removeMailBox(codes);
            startIndex = mailbox.Item1;
            endIndex = mailbox.Item2;
            if (startIndex > -1 && endIndex > -1)
            {
                codes.RemoveRange(startIndex, endIndex - startIndex);
            }

            if (startIndex > -1)
                codes.InsertRange(startIndex, makeShadowsCodes());
            else
                Logger.Log("Mailbox draw code seems to have been altered by another mod.  This will result in unexpected behavior.", LogLevel.Warn);

            removeShadowsNew(ref codes);

            changeNote(ref codes);

            //Logger.Log("Codes count after mailbox removal: " + codes.Count);

            //Tuple<int, int> buildingShadow = removeShadows(codes);
            //startIndex = buildingShadow.Item1;
            //endIndex = buildingShadow.Item2;
            //if (startIndex > -1 && endIndex > -1)
            //{
            //    for(int i = startIndex; i < endIndex; i++)
            //    {
            //        codes[i].labels.Clear();
            //    }
            //    codes.RemoveRange(startIndex, endIndex - startIndex);
            //}

            //Logger.Log("Codes count after shadows removal: " + codes.Count);

            //foreach (CodeInstruction codeInst in codes)
            //{
            //    if(codeInst != null)
            //        Logger.Log(codeInst.ToString());
            //}

            return codes.AsEnumerable();
        }

        public static void Postfix(SpriteBatch b, Farm __instance)
        {
            //FarmState.draw(b);
        }
    }
}
