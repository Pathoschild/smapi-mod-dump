/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using StardewValley;
//using StardewValley.Objects;
//using StardewValley.Tools;
//using Netcode;
//using StardewValley.ItemTypeDefinitions;
//using Newtonsoft.Json.Linq;
//using System.Collections.Generic;

//namespace MarketTown
//{
//    public class FurniturePatches
//    {
//        private static Dictionary<string, string> _frontTextureName;

//        public static bool clicked_Prefix(ref Furniture __instance, Farmer who, ref bool __result)
//        {
//            Game1.haltAfterCheck = false;
//            if ((int)__instance.furniture_type == 11 && who.ActiveObject != null && __instance.heldObject.Value == null)
//            {
//                __result = false;
//                return false;
//            }
//            if (__instance.heldObject.Value != null)
//            {
//                Object item = __instance.heldObject.Value;
//                __instance.heldObject.Value = null;
//                bool result = false;
//                if (item is WeaponProxy weaponProxy)
//                {
//                    result = who.addItemToInventoryBool(weaponProxy.Weapon);
//                }
//                else
//                {
//                    result = who.addItemToInventoryBool(item);
//                }

//                if (result)
//                {
//                    item.performRemoveAction();
//                    Game1.playSound("coin");
//                    __result = true;
//                    return false;
//                }
//                __instance.heldObject.Value = item;
//            }
//            __result = false;
//            return false;
//        }

//        public static void performObjectDropInAction_Postfix(ref Furniture __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
//        {
//            if (dropInItem is MeleeWeapon dropInWeapon)
//            {
//                // If this is a table and it doesn't have any items on it...
//                if (((int)__instance.furniture_type == 11 || (int)__instance.furniture_type == 5) && __instance.heldObject.Value == null)
//                {
//                    var x = new WeaponProxy(dropInWeapon);
//                    x.ItemId = dropInItem.ItemId;

//                    //Game1.chatBox.addInfoMessage("b4 " + item.QualifiedItemId + "_" + item.ItemId + "_" + ItemRegistry.GetDataOrErrorItem(item.ItemId).DisplayName + "_" + item.Name);

//                    __instance.heldObject.Value = x;
//                    __instance.heldObject.Value.tileLocation.Value = __instance.TileLocation;
//                    __instance.heldObject.Value.boundingBox.X = __instance.boundingBox.X;
//                    __instance.heldObject.Value.boundingBox.Y = __instance.boundingBox.Y;
//                    __instance.heldObject.Value.performDropDownAction(who);


//                    //Game1.chatBox.addInfoMessage("af " + __instance.heldObject.Value.QualifiedItemId + "_" + __instance.heldObject.Value.ItemId + "_" + ItemRegistry.GetDataOrErrorItem(__instance.heldObject.Value.ItemId).DisplayName + "_" + __instance.heldObject.Value.Name);
//                    if (!probe)
//                    {
//                        who.currentLocation.playSound("woodyStep");
//                        who?.reduceActiveItemByOne();
//                        who.CurrentTool = null;
//                    }
//                    __result = true;
//                }
//            }
//        }

//        public static bool draw_Prefix(ref Furniture __instance, NetInt ___sourceIndexOffset, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
//        {
//            if (__instance.isTemporarilyInvisible)
//            {
//                return false;
//            }
//            Rectangle drawn_source_rect = __instance.sourceRect.Value;
//            drawn_source_rect.X += drawn_source_rect.Width * ___sourceIndexOffset.Value;
//            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.ItemId);
//            Texture2D texture = dataOrErrorItem.GetTexture();
//            string textureName = dataOrErrorItem.TextureName;


//            //if (__instance.heldObject.Value !=null) Game1.chatBox.addInfoMessage(__instance.heldObject.Value.Name);

//            if (dataOrErrorItem.IsErrorItem)
//            {

//                drawn_source_rect = dataOrErrorItem.GetSourceRect();
//            }

//            if (_frontTextureName == null)
//            {
//                _frontTextureName = new Dictionary<string, string>();
//            }

//            if (Furniture.isDrawingLocationFurniture)
//            {
//                if (!_frontTextureName.TryGetValue(textureName, out var value2))
//                {
//                    value2 = textureName + "Front";
//                    _frontTextureName[textureName] = value2;
//                }

//                Texture2D texture2D = null;
//                if (__instance.HasSittingFarmers())
//                {
//                    try
//                    {
//                        texture2D = Game1.content.Load<Texture2D>(value2);
//                    }
//                    catch
//                    {
//                        texture2D = null;
//                    }
//                }

//                Vector2 position = Game1.GlobalToLocal(Game1.viewport, ___drawPosition.Value + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero));
//                SpriteEffects effects = (__instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
//                Color color = Color.White * alpha;
//                if (__instance.HasSittingFarmers())
//                {
//                    spriteBatch.Draw(texture, position, drawn_source_rect, color, 0f, Vector2.Zero, 4f, effects, (float)(__instance.GetBoundingBox().Top + 16) / 10000f);
//                    if (texture2D != null && drawn_source_rect.Right <= texture2D.Width && drawn_source_rect.Bottom <= texture2D.Height)
//                    {
//                        spriteBatch.Draw(texture2D, position, drawn_source_rect, color, 0f, Vector2.Zero, 4f, effects, (float)(__instance.GetBoundingBox().Bottom - 8) / 10000f);
//                    }
//                }
//                else
//                {
//                    spriteBatch.Draw(texture, position, drawn_source_rect, color, 0f, Vector2.Zero, 4f, effects, ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.GetBoundingBox().Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f));
//                }
//            }
//            else
//            {
//                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 - (drawn_source_rect.Height * 4 - __instance.GetBoundingBox().Height) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), drawn_source_rect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.GetBoundingBox().Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f));
//            }
//            if (__instance.heldObject.Value != null)
//            {
//                if (__instance.heldObject.Value is Furniture)
//                {
//                    (__instance.heldObject.Value as Furniture).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.heldObject.Value as Furniture).sourceRect.Height * 4 - (__instance.drawHeldObjectLow ? -16 : 16))), (__instance.boundingBox.Bottom - 7) / 10000f, alpha);
//                }
//                else
//                {
//                    spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))) + new Vector2(32f, 53f), Game1.shadowTexture.Bounds, Color.White * alpha, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, __instance.boundingBox.Bottom / 10000f);

//                    // Add support for rendering weapons on tables.
//                    if (__instance.heldObject.Value is WeaponProxy weapon)
//                    {
//                        spriteBatch.Draw(Tool.weaponsTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32,
//                        __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))),
//                        ItemRegistry.GetDataOrErrorItem(weapon.ItemId).GetSourceRect(), Color.White * alpha, 0f, Vector2.Zero,
//                        4f, SpriteEffects.None, (__instance.boundingBox.Bottom + 1) / 10000f);
//                    }
//                    else
//                    {
//                        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32,
//                        __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))),
//                        GameLocation.getSourceRectForObject(__instance.heldObject.Value.ParentSheetIndex), Color.White * alpha, 0f, Vector2.Zero,
//                        4f, SpriteEffects.None, (__instance.boundingBox.Bottom + 1) / 10000f);
//                    }
//                }
//            }
//            if ((bool)__instance.isOn && (int)__instance.furniture_type == 14)
//            {
//                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 12, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + x * 3047 + y * 88) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.GetBoundingBox().Bottom - 2) / 10000f);
//                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 - 4, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + x * 2047 + y * 98) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.GetBoundingBox().Bottom - 1) / 10000f);
//            }
//            else if ((bool)__instance.isOn && (int)__instance.furniture_type == 16)
//            {
//                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 20, __instance.boundingBox.Center.Y - 105.6f)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + x * 3047 + y * 88) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.GetBoundingBox().Bottom - 2) / 10000f);
//            }
//            if (Game1.debugMode)
//            {
//                spriteBatch.DrawString(Game1.smallFont, string.Concat((object)__instance.parentSheetIndex), Game1.GlobalToLocal(Game1.viewport, new Vector2(___drawPosition.X, ___drawPosition.Y)), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
//            }
//            return false;
//        }
//    }
//}