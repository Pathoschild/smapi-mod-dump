/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
	public enum ChestType
	{
		Normal,
		Stone,
		Fridge,
		MiniFridge,
		Mill,
		JunimoHut,
		Special
	}

	public class TypedChest
	{
		public Chest Chest { get; private set; }

		public ChestType ChestType { get; private set; }

		public TypedChest(Chest chest, ChestType chestType)
		{
			Chest = chest;
			ChestType = chestType;
		}

		public static ChestType GetChestType(Chest chest)
		{
			if (chest.SpecialChestType != Chest.SpecialChestTypes.None)
			{
				return ChestType.Special;
			}

			switch (chest.ParentSheetIndex)
			{
				case 130:
				default:
					return ChestType.Normal;
				case 232:
					return ChestType.Stone;
				case 216:
					return ChestType.MiniFridge;
			}
		}

		public static bool IsBuildingChestType(ChestType chestType)
        {
			return chestType == ChestType.Mill || chestType == ChestType.JunimoHut;
        }

		/// <summary>Returns the number of tooltip position indexes skipped while drawing (due to buildings occupying > 1 index).</summary>
		public int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex)
		{
			int newPosIndex;

			int x = toolTipPosition.X + 20 + 46 * (posIndex % 8);
			int y = toolTipPosition.Y + 40 + 52 * (posIndex / 8);

			switch (this.ChestType)
			{
				case ChestType.Normal:
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, !this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? 168 : this.Chest.ParentSheetIndex, 16, 32),
						this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? this.Chest.Tint : this.Chest.playerChoiceColor.Value,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
					);
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y + 42),
						new Rectangle(0, 168 / 8 * 32 + 53, 16, 11),
						Color.White,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f
					);
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.startingLidFrame.Value + 46, 16, 32),
						Color.White,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 2E-05f
					);

					return 0;
				case ChestType.Stone:
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
						this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? this.Chest.Tint : this.Chest.playerChoiceColor.Value,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
					);
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y + 42),
						new Rectangle(0, this.Chest.ParentSheetIndex / 8 * 32 + 53, 16, 11),
						Color.White,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f
					);
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.startingLidFrame.Value + 8, 16, 32),
						Color.White,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 2E-05f
					);

					return 0;
				case ChestType.Fridge:
					if (Game1.currentLocation is StardewValley.Locations.FarmHouse)
					{
						spriteBatch.Draw(CachedTextures.FarmHouse,
							new Vector2(x, y + 3),
							new Rectangle(16 * 5, 48 * 4 + 13, 16, 35),
							Color.White,
							0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f
						);
					}
					else
					{
						// Island house
						spriteBatch.Draw(CachedTextures.FarmHouse,
							new Vector2(x, y + 3),
							new Rectangle(16 * 6, 48 * 6 + 29, 16, 35),
							Color.White,
							0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f
						);
					}

					return 0;
				case ChestType.MiniFridge:
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y + 8),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
						Color.White,
						0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f
					);

					return 0;
				case ChestType.Mill:
					newPosIndex = posIndex;

					if (posIndex % 8 == 7)
					{
						newPosIndex++;
						x = toolTipPosition.X + 20 + 46 * (newPosIndex % 8);
						y = toolTipPosition.Y + 40 + 52 * (newPosIndex / 8);
					}

					spriteBatch.Draw(CachedTextures.Mill,
						new Vector2(x + 8, y),
						new Rectangle(0, 64, 64, 64),
						Color.White,
						0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
					);

					newPosIndex++;

					return newPosIndex - posIndex;
				case ChestType.JunimoHut:
					newPosIndex = posIndex;

					if (posIndex % 8 == 7)
					{
						newPosIndex++;
						x = toolTipPosition.X + 20 + 46 * (newPosIndex % 8);
						y = toolTipPosition.Y + 40 + 52 * (newPosIndex / 8);
					}

					spriteBatch.Draw(CachedTextures.JunimoHut,
						new Vector2(x + 9, y - 16),
						new Rectangle(Utility.getSeasonNumber(Game1.currentSeason) * 48, 0, 48, 64),
						Color.White,
						0f, Vector2.Zero, 1.25f, SpriteEffects.None, 1f
					);

					newPosIndex++;

					return newPosIndex - posIndex;
				case ChestType.Special:
				default:
					spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
						new Vector2(x, y),
						Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
						Color.White,
						0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
					);

					return 0;
			}
		}
	}
}
