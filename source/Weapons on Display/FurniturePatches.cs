/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/WeaponsOnDisplay
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using System.Collections.Generic;
using StardewModdingAPI;
using System.Xml.Serialization;
using System.IO;

namespace WeaponsOnDisplay
{
	public class FurniturePatches
	{
		private static string XmlSerialize<T>(T toSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
			using(StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, toSerialize);
				return textWriter.ToString();
			}
		}
		public static bool clicked_Prefix(ref Furniture __instance, Farmer who, ref bool __result)
		{
			Game1.haltAfterCheck = false;
			if (__instance.furniture_type.Value == 11 && who.ActiveObject != null && __instance.heldObject.Value == null)
			{
				__result = false;
				return false;
			}
			if (__instance.heldObject.Value != null)
			{
				Object item = __instance.heldObject.Value;
				__instance.heldObject.Value = null;
				bool result = false;
				if (item is WeaponProxy weaponProxy)
				{
					result = who.addItemToInventoryBool(weaponProxy.Weapon);
				}
				else if (item is SlingshotProxy slingshotProxy)
				{
					result = who.addItemToInventoryBool(slingshotProxy.Weapon);
				}
				else
				{
					result = who.addItemToInventoryBool(item);
				}

				if (result)
				{
					item.performRemoveAction();
					Game1.playSound("coin");
					__result = true;
					return false;
				}
				__instance.heldObject.Value = item;
			}
			__result = false;
			return false;
		}

		public static bool performObjectDropInAction_Prefix(ref Furniture __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
		{
			if (__instance.Location == null)
			{
				__result = false;
				return false;
			}

			if (dropInItem is MeleeWeapon dropInWeapon)
			{
				// If this is a table and it doesn't have any items on it...
				if (__instance.IsTable() && __instance.heldObject.Value == null)
				{
					if (!probe)
					{
						__instance.heldObject.Value = new WeaponProxy((MeleeWeapon)dropInWeapon.getOne())
						{
							Location = __instance.Location,
							TileLocation = __instance.TileLocation
						};
						__instance.heldObject.Value.boundingBox.X = __instance.boundingBox.X;
						__instance.heldObject.Value.boundingBox.Y = __instance.boundingBox.Y;
						__instance.heldObject.Value.performDropDownAction(who);
						who.currentLocation.playSound("woodyStep");

						if (who != null)
						{
							who.reduceActiveItemByOne();
							who.CurrentTool = null;

							if (returnFalseIfItemConsumed)
							{
								__result = false;
								return false;
							}
						}
					}
					__result = true;
					return false;
				}
			}
			else if (dropInItem is Slingshot dropInSlingshot)
			{
				// If this is a table and it doesn't have any items on it...
				if (__instance.IsTable() && __instance.heldObject.Value == null)
				{
					if (!probe)
					{
						__instance.heldObject.Value = new SlingshotProxy((Slingshot)dropInSlingshot.getOne())
						{
							Location = __instance.Location,
							TileLocation = __instance.TileLocation
						};
						__instance.heldObject.Value.boundingBox.X = __instance.boundingBox.X;
						__instance.heldObject.Value.boundingBox.Y = __instance.boundingBox.Y;
						__instance.heldObject.Value.performDropDownAction(who);
						who.currentLocation.playSound("woodyStep");

						if (who != null)
						{
							who?.reduceActiveItemByOne();
							who.CurrentTool = null;

							if (returnFalseIfItemConsumed)
							{
								__result = false;
								return false;
							}
						}
					}
					__result = true;
					return false;
				}
			}
			return true;
		}

		public static bool draw_Prefix(ref Furniture __instance, Dictionary<string, string> ____frontTextureName, NetInt ___sourceIndexOffset, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1.0f)
		{
			if (__instance.isTemporarilyInvisible)
			{
				return false;
			}
			Rectangle drawn_source_rect = __instance.sourceRect.Value;
			drawn_source_rect.X += drawn_source_rect.Width * ___sourceIndexOffset.Value;
			ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
			Texture2D texture = itemData.GetTexture();
			string textureName = itemData.TextureName;
			if (itemData.IsErrorItem)
			{
				drawn_source_rect = itemData.GetSourceRect(0, null);
			}
			if (____frontTextureName == null)
			{
				____frontTextureName = new Dictionary<string, string>();
			}
			if (Furniture.isDrawingLocationFurniture)
			{
				string frontTexturePath;
				if (!____frontTextureName.TryGetValue(textureName, out frontTexturePath))
				{
					frontTexturePath = textureName + "Front";
					____frontTextureName[textureName] = frontTexturePath;
				}
				Texture2D frontTexture = null;
				if (__instance.HasSittingFarmers() || __instance.SpecialVariable == 388859)
				{
					try
					{
						frontTexture = Game1.content.Load<Texture2D>(frontTexturePath);
					}
					catch
					{
						frontTexture = null;
					}
				}
				Vector2 actualDrawPosition = Game1.GlobalToLocal(Game1.viewport, ___drawPosition.Value + ((__instance.shakeTimer > 0) ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero));
				SpriteEffects spriteEffects = __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				Color color = Color.White * alpha;
				if (__instance.HasSittingFarmers())
				{
					spriteBatch.Draw(texture, actualDrawPosition, new Rectangle?(drawn_source_rect), color, 0f, Vector2.Zero, 4f, spriteEffects, (float)(__instance.boundingBox.Value.Top + 16) / 10000f);
					if (frontTexture != null && drawn_source_rect.Right <= frontTexture.Width && drawn_source_rect.Bottom <= frontTexture.Height)
					{
						spriteBatch.Draw(frontTexture, actualDrawPosition, new Rectangle?(drawn_source_rect), color, 0f, Vector2.Zero, 4f, spriteEffects, (float)(__instance.boundingBox.Value.Bottom - 8) / 10000f);
					}
				}
				else
				{
					spriteBatch.Draw(texture, actualDrawPosition, new Rectangle?(drawn_source_rect), color, 0f, Vector2.Zero, 4f, spriteEffects, (__instance.furniture_type.Value == 12) ? (2E-09f + __instance.TileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - ((__instance.furniture_type.Value == 6 || __instance.furniture_type.Value == 17 || __instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
					if (__instance.SpecialVariable == 388859 && frontTexture != null && drawn_source_rect.Right <= frontTexture.Width && drawn_source_rect.Bottom <= frontTexture.Height)
					{
						spriteBatch.Draw(frontTexture, actualDrawPosition, new Rectangle?(drawn_source_rect), color, 0f, Vector2.Zero, 4f, spriteEffects, (float)(__instance.boundingBox.Value.Bottom - 2) / 10000f);
					}
				}
			}
			else
			{
				spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)), (float)(y * 64 - (drawn_source_rect.Height * 4 - __instance.boundingBox.Height) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(drawn_source_rect), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.furniture_type.Value == 12) ? (2E-09f + __instance.TileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - ((__instance.furniture_type.Value == 6 || __instance.furniture_type.Value == 17 || __instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
			}

			if (__instance.heldObject.Value != null)
			{
				Furniture furniture = __instance.heldObject.Value as Furniture;
				if (furniture != null)
				{
					furniture.drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(__instance.boundingBox.Center.X - 32), (float)(__instance.boundingBox.Center.Y - furniture.sourceRect.Height * 4 - (__instance.drawHeldObjectLow.Value ? -16 : 16)))), (float)(__instance.boundingBox.Bottom - 7) / 10000f, alpha);
				}
				else
				{
					ParsedItemData heldItemData = ItemRegistry.GetDataOrErrorItem(__instance.heldObject.Value.QualifiedItemId);
					spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(__instance.boundingBox.Center.X - 32), (float)(__instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85)))) + new Vector2(32f, 53f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)__instance.boundingBox.Bottom / 10000f);

					// Add support for rendering weapons on tables.
					if (__instance.heldObject.Value is WeaponProxy weapon)
					{
						heldItemData = ItemRegistry.GetDataOrErrorItem(weapon.Weapon.GetDrawnItemId());
					}
					else if (__instance.heldObject.Value is SlingshotProxy slingshot)
					{
						heldItemData = ItemRegistry.GetDataOrErrorItem(slingshot.Weapon.QualifiedItemId);
					}

					if (__instance.heldObject.Value is ColoredObject)
					{
						__instance.heldObject.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(__instance.boundingBox.Center.X - 32), (float)(__instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85)))), 1f, 1f, (float)(__instance.boundingBox.Bottom + 1) / 10000f, StackDrawType.Hide, Color.White, false);
					}
					else
					{
						spriteBatch.Draw(heldItemData.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(__instance.boundingBox.Center.X - 32), (float)(__instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85)))), new Rectangle?(heldItemData.GetSourceRect(0, null)), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(__instance.boundingBox.Bottom + 1) / 10000f);
					}
				}
			}
			if (__instance.IsOn && __instance.furniture_type.Value == 14)
			{
				Rectangle bounds = __instance.GetBoundingBoxAt(x, y);
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 12, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (x * 3047) + (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(bounds.Bottom - 2) / 10000.0f);
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 - 4, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (x * 2047) + (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(bounds.Bottom - 1) / 10000.0f);
			}
			else if (__instance.IsOn && __instance.furniture_type.Value == 16)
			{
				Rectangle bounds2 = __instance.GetBoundingBoxAt(x, y);
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 20, __instance.boundingBox.Center.Y - 105.6f)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (x * 3047) + (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(bounds2.Bottom - 2) / 10000.0f);
			}
			if (Game1.debugMode)
			{
				spriteBatch.DrawString(Game1.smallFont, __instance.QualifiedItemId, Game1.GlobalToLocal(Game1.viewport, ___drawPosition.Value), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			return false;
		}
	}
}