/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jeffgillean/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace FrugalFarmMenu
{
	public class InventoryPage : StardewValley.Menus.InventoryPage
	{
		private readonly StardewModdingAPI.Mod mod;
		private readonly IReflectedField<int> hoverAmountField;
		private readonly IReflectedField<Item> hoveredItemField;
		private readonly IReflectedField<string> hoverTextField;
		private readonly IReflectedField<string> hoverTitleField;
		private readonly IReflectedField<float> trashCanLidRotationField;

		private Action<SpriteBatch> drawAction;

		public InventoryPage(StardewModdingAPI.Mod mod, int xPositionOnScreen, int yPositionOnScreen, int width, int height) : base(xPositionOnScreen, yPositionOnScreen, width, height)
		{
			this.mod = mod;

			hoverAmountField = mod.Helper.Reflection.GetField<int>(this, "hoverAmount");
			hoveredItemField = mod.Helper.Reflection.GetField<Item>(this, "hoveredItem");
			hoverTextField = mod.Helper.Reflection.GetField<string>(this, "hoverText");
			hoverTitleField = mod.Helper.Reflection.GetField<string>(this, "hoverTitle");
			trashCanLidRotationField = mod.Helper.Reflection.GetField<float>(this, "trashCanLidRotation");

			drawAction = DrawModInventoryPage;
		}

		public sealed override void draw(SpriteBatch spriteBatch)
		{
			drawAction.Invoke(spriteBatch);
		}

		public void DrawModInventoryPage(SpriteBatch spriteBatch)
		{
			try
			{
				DrawPartition(spriteBatch);
				DrawInventory(spriteBatch);
				DrawEquipment(spriteBatch);
				DrawFarmer(spriteBatch);
				DrawFarmInfo(spriteBatch);
				DrawAnimals(spriteBatch);
				DrawUtilities(spriteBatch);
				DrawToolTip(spriteBatch);
			}
			catch (Exception ex)
			{
				mod.Monitor.Log($"Failed to draw Inventory Page:\n{ex}", LogLevel.Error);
				drawAction = base.draw;
			}
		}

		protected virtual void DrawPartition(SpriteBatch spriteBatch)
		{
			drawHorizontalPartition(spriteBatch, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 192);
		}

		protected virtual void DrawInventory(SpriteBatch spriteBatch)
		{
			inventory.draw(spriteBatch);
		}

		protected virtual void DrawEquipment(SpriteBatch spriteBatch)
		{
			foreach (ClickableComponent equipmentIcon in equipmentIcons)
			{
				switch (equipmentIcon.name)
				{
					case "Hat":
						Item item = Game1.player.hat?.Value;
						int tile = (item == null) ? 42 : 10;

						spriteBatch.Draw(Game1.menuTexture, equipmentIcon.bounds, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, tile), Color.White);
						item?.drawInMenu(spriteBatch, new Vector2(equipmentIcon.bounds.X, equipmentIcon.bounds.Y), equipmentIcon.scale, 1f, 0.866f, StackDrawType.Hide);
						break;
					case "Right Ring":
						DrawEquipmentTile(spriteBatch, equipmentIcon, Game1.player.rightRing?.Value, 41);
						break;
					case "Left Ring":
						DrawEquipmentTile(spriteBatch, equipmentIcon, Game1.player.leftRing?.Value, 41);
						break;
					case "Boots":
						DrawEquipmentTile(spriteBatch, equipmentIcon, Game1.player.boots?.Value, 40);
						break;
					case "Shirt":
						DrawEquipmentTile(spriteBatch, equipmentIcon, Game1.player.shirtItem?.Value, 69);
						break;
					case "Pants":
						DrawEquipmentTile(spriteBatch, equipmentIcon, Game1.player.pantsItem?.Value, 68);
						break;
				}
			}
		}

		protected virtual void DrawFarmer(SpriteBatch spriteBatch)
		{
			Texture2D background = Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg;
			Color shadowColor = Game1.timeOfDay >= 1900 ? Color.DarkBlue * 0.3f : Color.Transparent;

			spriteBatch.Draw(background, new Vector2(xPositionOnScreen + 192 - 64 - 8, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 256 - 8), Color.White);

			FarmerRenderer.isDrawingForUI = true;

			int frame = Game1.player.bathingClothes.Value ? 108 : 0;
			int height = Game1.player.bathingClothes.Value ? 576 : 0;

			Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(0, frame, secondaryArm: false, flip: false), frame, new Rectangle(0, height, 16, 32), new Vector2(xPositionOnScreen + 192 - 8 - 32, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 320 - 32 - 8), Vector2.Zero, 0.8f, 2, Color.White, 0f, 1f, Game1.player);
			Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(0, frame, secondaryArm: false, flip: false), frame, new Rectangle(0, height, 16, 32), new Vector2(xPositionOnScreen + 192 - 8 - 32, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 320 - 32 - 8), Vector2.Zero, 0.8f, 2, shadowColor, 0f, 1f, Game1.player);

			FarmerRenderer.isDrawingForUI = false;

			DrawCenteredText(spriteBatch, Game1.player.Name, xPositionOnScreen + 192 - 8, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 456);
		}

		protected virtual void DrawFarmInfo(SpriteBatch spriteBatch)
		{
			Dictionary<string, string> tokens = new()
			{
				{ "totalSpent", Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned - Game1.player.Money + 500) },
				{ "totalEarned", Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned) },
			};

			string row1 = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", Game1.player.farmName.Value);
			string row2 = mod.Helper.Translation.Get("Inventory_TotalSpent").Tokens(tokens);
			string row3 = mod.Helper.Translation.Get("Inventory_TotalEarned").Tokens(tokens);

			float offset = 32f;
			float xPositionFarm = xPositionOnScreen + offset + 512f + 32f;
			float yPositionFarm = yPositionOnScreen + borderWidth + spaceToClearTopBorder;

			DrawCenteredText(spriteBatch, row1, xPositionFarm, yPositionFarm + 260);
			DrawCenteredText(spriteBatch, row2, xPositionFarm, yPositionFarm + 320);
			DrawCenteredText(spriteBatch, row3, xPositionFarm, yPositionFarm + 380);
		}

		protected virtual void DrawAnimals(SpriteBatch spriteBatch)
		{
			float offset = 32f;

			Pet pet = Game1.MasterPlayer.getPet();
			string petDisplayName = Game1.MasterPlayer.getPetDisplayName();

			if (pet != null)
			{
				Utility.drawTextWithShadow(spriteBatch, petDisplayName, Game1.dialogueFont, new Vector2(xPositionOnScreen + offset + 320f + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), yPositionOnScreen + borderWidth + spaceToClearTopBorder + 448 + 8), Game1.textColor);
				Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(xPositionOnScreen + offset + 256f + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), yPositionOnScreen + borderWidth + spaceToClearTopBorder + 448 - 4), new Rectangle(160 + ((!Game1.MasterPlayer.catPerson) ? 48 : 0) + pet.whichBreed.Value * 16, 208, 16, 16), Color.White, 0f, Vector2.Zero, 4f);
			}

			if (!string.IsNullOrEmpty(Game1.player.horseName.Value))
			{
				Utility.drawTextWithShadow(spriteBatch, Game1.player.horseName.Value, Game1.dialogueFont, new Vector2(xPositionOnScreen + offset + 384f + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + ((petDisplayName != null) ? Math.Max(64f, Game1.dialogueFont.MeasureString(petDisplayName).X) : 0f), yPositionOnScreen + borderWidth + spaceToClearTopBorder + 448 + 8), Game1.textColor);
				Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(xPositionOnScreen + offset + 320f + 8f + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + ((petDisplayName != null) ? Math.Max(64f, Game1.dialogueFont.MeasureString(petDisplayName).X) : 0f), yPositionOnScreen + borderWidth + spaceToClearTopBorder + 448 - 4), new Rectangle(193, 192, 16, 16), Color.White, 0f, Vector2.Zero, 4f);
			}
		}

		protected virtual void DrawUtilities(SpriteBatch spriteBatch)
		{
			organizeButton?.draw(spriteBatch);
			junimoNoteIcon?.draw(spriteBatch);
			trashCan.draw(spriteBatch);

			spriteBatch.Draw(Game1.mouseCursors, new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40), new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10), Color.White, GetTrashCanLidRotation(), new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);

			if (checkHeldItem())
			{
				Game1.player.CursorSlotItem.drawInMenu(spriteBatch, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
			}
		}

		protected virtual void DrawToolTip(SpriteBatch spriteBatch)
		{
			int hoverAmount = GetHoverAmount();
			string hoverText = GetHoverText();
			string hoverTitle = GetHoverTitle();

			if (!string.IsNullOrEmpty(hoverText))
			{
				if (hoverAmount > 0)
				{
					drawToolTip(spriteBatch, hoverText, hoverTitle, null, heldItem: true, -1, 0, -1, -1, null, hoverAmount);
				}
				else
				{
					drawToolTip(spriteBatch, hoverText, hoverTitle, GetHoveredItem(), checkHeldItem());
				}
			}
		}

		protected int GetHoverAmount()
		{
			return hoverAmountField.GetValue();
		}

		protected Item GetHoveredItem()
		{
			return hoveredItemField.GetValue();
		}

		protected string GetHoverText()
		{
			return hoverTextField.GetValue();
		}

		protected string GetHoverTitle()
		{
			return hoverTitleField.GetValue();
		}

		protected float GetTrashCanLidRotation()
		{
			return trashCanLidRotationField.GetValue();
		}

		private static void DrawEquipmentTile(SpriteBatch spriteBatch, ClickableComponent equipmentIcon, Item item, int tile)
		{
			spriteBatch.Draw(Game1.menuTexture, equipmentIcon.bounds, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, item == null ? tile : 10), Color.White);
			item?.drawInMenu(spriteBatch, new Vector2(equipmentIcon.bounds.X, equipmentIcon.bounds.Y), equipmentIcon.scale);
		}

		private static void DrawCenteredText(SpriteBatch spriteBatch, string text, float xMid, float yTop)
		{
			Utility.drawTextWithShadow(spriteBatch, text, Game1.dialogueFont, new Vector2(xMid - Game1.dialogueFont.MeasureString(text).X / 2f, yTop), Game1.textColor);
		}
	}
}