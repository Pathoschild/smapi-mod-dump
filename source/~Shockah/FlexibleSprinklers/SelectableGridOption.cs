/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Map;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers;

internal partial class SelectableGridOption
{
	private const int RowHeight = 60;
	private const float MaxCellSize = 36f;
	private const float CellSpacing = 12f;
	private const float Margin = 16f;
	private static readonly string ClickSoundName = "drumkit6";
	private static readonly string BombSoundName = "explosion";
	private static readonly string WinSoundName = "crystal";

	private readonly Func<IReadOnlySet<IntPoint>> GetValues;
	private readonly Action<IReadOnlySet<IntPoint>> SetValues;
	private readonly Func<string> Name;
	private readonly Func<string>? Tooltip;
	private readonly Action? AfterValuesUpdated;

	private readonly Lazy<TextureRectangle> CenterTexture = new(() => new(Game1.objectSpriteSheet, new(336, 400, 16, 16)));
	private readonly Lazy<TextureRectangle> CheckedTexture = new(() => new(Game1.mouseCursors, OptionsCheckbox.sourceRectChecked));
	private readonly Lazy<TextureRectangle> UncheckedTexture = new(() => new(Game1.mouseCursors, OptionsCheckbox.sourceRectUnchecked));

	private readonly Lazy<TextureRectangle> BombTexture = new(() => new(Game1.objectSpriteSheet, new(368, 176, 16, 16)));
	private readonly Lazy<TextureRectangle> BigBombTexture = new(() => new(Game1.objectSpriteSheet, new(0, 192, 16, 16)));
	private readonly Lazy<TextureRectangle> FlagTexture = new(() => new(Game1.emoteSpriteSheet, new(0, 64, 16, 16)));

	private IReadOnlySet<IntPoint> OriginalValues = new HashSet<IntPoint>();
	private HashSet<IntPoint> CurrentValues = new();
	private bool? LastMouseLeftPressed;
	private bool? LastMouseRightPressed;
	private int Length = 3;

	private int ClicksUntilMinesweeper = 5;
	private Minesweeper? MinesweeperGame;
	private bool IsMinesweeperGameOver = true;

	public SelectableGridOption(
		Func<IReadOnlySet<IntPoint>> getValues,
		Action<IReadOnlySet<IntPoint>> setValues,
		Func<string> name,
		Func<string>? tooltip = null,
		Action? afterValuesUpdated = null
	)
	{
		this.GetValues = getValues;
		this.SetValues = setValues;
		this.Name = name;
		this.Tooltip = tooltip;
		this.AfterValuesUpdated = afterValuesUpdated;
	}

	private void Initialize()
	{
		OriginalValues = GetValues();
		CurrentValues = OriginalValues.ToHashSet();

		Length = 3;
		UpdateLength();
		Length = Math.Max(Length, 3);

		ClicksUntilMinesweeper = 5;
		MinesweeperGame = null;
		IsMinesweeperGameOver = true;
	}

	internal void AddToGMCM(IGenericModConfigMenuApi api, IManifest mod)
	{
		api.AddComplexOption(
			mod: mod,
			name: Name,
			tooltip: Tooltip,
			draw: Draw,
			height: () => GetHeight(),
			beforeMenuOpened: () =>
			{
				LastMouseLeftPressed = null;
				Initialize();
			},
			beforeMenuClosed: Initialize,
			afterReset: Initialize,
			beforeSave: BeforeSave
		);
	}

	private void BeforeSave()
	{
		SetValues(CurrentValues.ToHashSet());
		OriginalValues = CurrentValues.ToHashSet();
		AfterValuesUpdated?.Invoke();
	}

	private void UpdateLength()
	{
		if (CurrentValues.Count == 0)
			return;
		int max = CurrentValues.Max(p => Math.Max(Math.Abs(p.X), Math.Abs(p.Y)));
		int newLength = (max + 1) * 2 + 1;
		Length = Math.Max(Length, newLength);
	}

	private Vector2 GetGMCMSize()
		=> new(Math.Min(1200, Game1.uiViewport.Width - 200), Game1.uiViewport.Height - 128 - 116);

	private Vector2 GetGMCMPosition(Vector2? size = null)
	{
		Vector2 gmcmSize = size ?? GetGMCMSize();
		return new((Game1.uiViewport.Width - gmcmSize.X) / 2, (Game1.uiViewport.Height - gmcmSize.Y) / 2);
	}

	private float GetCellScale(Vector2? gmcmSize = null)
	{
		gmcmSize ??= GetGMCMSize();
		float availableWidth = gmcmSize.Value.X - Margin;
		float requiredWidthAtFullScale = Length * MaxCellSize + (Length - 1) * CellSpacing;
		float scale = Math.Min(availableWidth / requiredWidthAtFullScale, 1f);
		return scale;
	}

	private int GetHeight(float? cellScale = null, Vector2? gmcmSize = null)
	{
		cellScale ??= GetCellScale(gmcmSize: gmcmSize);
		return (int)Math.Ceiling(MaxCellSize * cellScale.Value * Length + CellSpacing * cellScale.Value * (Length - 1)) + RowHeight; // extra row, we're not rendering inline
	}

	private void Draw(SpriteBatch b, Vector2 basePosition)
	{
		bool mouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
		bool mouseRightPressed = Game1.input.GetMouseState().RightButton == ButtonState.Pressed;
		bool didClickLeft = mouseLeftPressed && LastMouseLeftPressed == false;
		bool didClickRight = mouseRightPressed && LastMouseRightPressed == false;
		LastMouseLeftPressed = mouseLeftPressed;
		LastMouseRightPressed = mouseRightPressed;
		int mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
		int mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

		Vector2 gmcmSize = GetGMCMSize();
		Vector2 gmcmPosition = GetGMCMPosition(gmcmSize);
		float cellScale = GetCellScale(gmcmSize);
		float cellLength = MaxCellSize * cellScale;
		float cellSpacing = CellSpacing * cellScale;
		bool hoverGMCM = mouseX >= gmcmPosition.X && mouseY >= gmcmPosition.Y && mouseX < gmcmPosition.X + gmcmSize.X && mouseY < gmcmPosition.Y + gmcmSize.Y;

		Vector2 startPosition = new(gmcmPosition.X + Margin, basePosition.Y + RowHeight);
		float widthLeft = gmcmSize.X - Margin;
		float totalCellWidth = cellLength * Length + cellSpacing * (Length - 1);
		startPosition.X += widthLeft / 2f - totalCellWidth / 2f;

		for (int drawY = 0; drawY < Length; drawY++)
		{
			int valueY = drawY - Length / 2;
			for (int drawX = 0; drawX < Length; drawX++)
			{
				int valueX = drawX - Length / 2;

				TextureRectangle GetTexture()
				{
					if (MinesweeperGame is null)
						return (valueX == 0 && valueY == 0 ? CenterTexture : (CurrentValues.Contains(new(valueX, valueY)) ? CheckedTexture : UncheckedTexture)).Value;

					if (IsMinesweeperGameOver && MinesweeperGame.Tiles[new(drawX, drawY)] == Minesweeper.Tile.Bomb)
						return MinesweeperGame.Markings[new(drawX, drawY)] == Minesweeper.Marking.Check ? BigBombTexture.Value : BombTexture.Value;
					return MinesweeperGame.Markings[new(drawX, drawY)] switch
					{
						Minesweeper.Marking.None => CheckedTexture.Value,
						Minesweeper.Marking.Check => UncheckedTexture.Value,
						Minesweeper.Marking.Flag => FlagTexture.Value,
						_ => throw new ArgumentException($"{nameof(Minesweeper.Marking)} has an invalid value."),
					};
				}

				TextureRectangle texture = GetTexture();

				float iconScale = 1f;
				if (texture.Rectangle.Width * iconScale < cellLength)
					iconScale = 1f * cellLength / texture.Rectangle.Width;
				if (texture.Rectangle.Height * iconScale < cellLength)
					iconScale = 1f * cellLength / texture.Rectangle.Height;
				if (texture.Rectangle.Width * iconScale > cellLength)
					iconScale = 1f * cellLength / texture.Rectangle.Width;
				if (texture.Rectangle.Height * iconScale > cellLength)
					iconScale = 1f * cellLength / texture.Rectangle.Height;

				Vector2 texturePosition = new(startPosition.X + drawX * (cellLength + cellSpacing), startPosition.Y + drawY * (cellLength + cellSpacing));
				Vector2 textureCenterPosition = new(texturePosition.X + cellLength / 2f, texturePosition.Y + cellLength / 2f);
				b.Draw(texture.Texture, textureCenterPosition, texture.Rectangle, Color.White, 0f, new Vector2(texture.Rectangle.Width / 2f, texture.Rectangle.Height / 2f), iconScale, SpriteEffects.None, 4f);

				if (MinesweeperGame is not null && MinesweeperGame.Markings[new(drawX, drawY)] == Minesweeper.Marking.Check && MinesweeperGame.Tiles[new(drawX, drawY)] != Minesweeper.Tile.Bomb)
				{
					int bombCount = MinesweeperGame.BombCountCache[new(drawX, drawY)];
					if (bombCount != 0)
					{
						float digitScale = cellLength / MaxCellSize * 4f;
						float digitWidth = Utility.getWidthOfTinyDigitString(bombCount, digitScale);
						float digitHeight = 7 * digitScale;
						Utility.drawTinyDigits(bombCount, b, new Vector2(texturePosition.X + cellLength / 2f - digitWidth / 2f, texturePosition.Y + cellLength / 2f - digitHeight / 2f), digitScale, 0f, Minesweeper.GetColorForBombCount(bombCount));
					}
				}

				if (hoverGMCM)
				{
					bool hoverTexture = mouseX >= texturePosition.X && mouseY >= texturePosition.Y && mouseX < texturePosition.X + cellLength && mouseY < texturePosition.Y + cellLength;
					if (hoverTexture)
					{
						if (MinesweeperGame is null)
						{
							if (!didClickLeft)
								continue;

							if (valueX == 0 && valueY == 0)
							{
								ClicksUntilMinesweeper--;
								if (ClicksUntilMinesweeper == 0)
									StartMinesweeper();
							}
							else
							{
								CurrentValues.Toggle(new(valueX, valueY));
								UpdateLength();
							}
							Game1.playSound(ClickSoundName);
						}
						else
						{
							if (didClickLeft)
							{
								if (IsMinesweeperGameOver)
								{
									IsMinesweeperGameOver = false;
									MinesweeperGame.RegenerateWithAGoodStart((int)(Length * Length * 0.17), new Random(), new(drawX, drawY));
									Game1.playSound(ClickSoundName);
								}
								else
								{
									var result = MinesweeperGame.Check(new(drawX, drawY));
									switch (result)
									{
										case Minesweeper.CheckResult.KeepGoing:
											Game1.playSound(ClickSoundName);
											break;
										case Minesweeper.CheckResult.GameOver:
											Game1.playSound(BombSoundName);
											IsMinesweeperGameOver = true;
											break;
										case Minesweeper.CheckResult.Win:
											Game1.playSound(WinSoundName);
											IsMinesweeperGameOver = true;
											break;
									}
								}
							}
							else if (didClickRight)
							{
								if (IsMinesweeperGameOver)
									continue;
								MinesweeperGame.ToggleFlag(new(drawX, drawY));
								Game1.playSound(ClickSoundName);
							}
						}
					}
				}
			}
		}
	}

	private void StartMinesweeper()
	{
		ClicksUntilMinesweeper = 0;
		MinesweeperGame = new(Length, Length);
		IsMinesweeperGameOver = true;
	}
}

public static class SelectableGridOptionExtensions
{
	public static void AddSelectableGridOption(
		this IGenericModConfigMenuApi api,
		IManifest mod,
		Func<IReadOnlySet<IntPoint>> getValues,
		Action<IReadOnlySet<IntPoint>> setValues,
		Func<string> name,
		Func<string>? tooltip = null,
		Action? afterValuesUpdated = null
	)
	{
		var option = new SelectableGridOption(getValues, setValues, name, tooltip, afterValuesUpdated);
		option.AddToGMCM(api, mod);
	}
}

public static class SelectableGridOptionForHelper
{
	public static void AddSelectableGridOption(
		this GMCMI18nHelper helper,
		string keyPrefix,
		Func<IReadOnlySet<IntPoint>> getValues,
		Action<IReadOnlySet<IntPoint>> setValues,
		Action? afterValuesUpdated = null,
		object? tokens = null
	)
	{
		helper.Api.AddSelectableGridOption(
			mod: helper.Mod,
			name: () => helper.Translations.Get($"{keyPrefix}.name", tokens),
			tooltip: helper.GetOptionalTranslatedStringDelegate($"{keyPrefix}.tooltip", tokens),
			getValues: getValues,
			setValues: setValues,
			afterValuesUpdated: afterValuesUpdated
		);
	}
}

partial class SelectableGridOption
{
	public class Minesweeper
	{
		public enum Tile { Empty, Bomb }
		public enum Marking { None, Check, Flag }
		public enum CheckResult { KeepGoing, GameOver, Win }

		public ArrayMap<Tile> Tiles { get; private set; }
		public ArrayMap<Marking> Markings { get; private set; }
		public ArrayMap<int> BombCountCache { get; private set; }

		public Minesweeper(int width, int height)
		{
			Tiles = new(Tile.Empty, width, height);
			Markings = new(Marking.None, width, height);
			BombCountCache = new(0, width, height);
		}

		public void Reset()
		{
			Tiles = new(Tile.Empty, Tiles.Bounds.Width, Tiles.Bounds.Height);
			Markings = new(Marking.None, Tiles.Bounds.Width, Tiles.Bounds.Height);
			BombCountCache = new(0, Tiles.Bounds.Width, Tiles.Bounds.Height);
		}

		public void RegenerateWithAGoodStart(int bombs, Random random, IntPoint start)
		{
			for (int i = 0; i < 100; i++)
			{
				Regenerate(bombs, random, start);
				Check(start);

				if (Markings.Bounds.AllPointEnumerator().Count(p => Markings[p] == Marking.Check) >= 8)
					return;
			}
		}

		public void Regenerate(int bombs, Random random, IntPoint? bombFreeSpace = null)
		{
			var allPoints = Tiles.Bounds.AllPointEnumerator().ToList();
			if (bombFreeSpace is not null)
				allPoints.Remove(bombFreeSpace.Value);
			if (allPoints.Count < bombs)
				throw new ArgumentException($"Cannot fit {bombs} bombs on a {Tiles.Bounds.Width}x{Tiles.Bounds.Height} grid.");

			allPoints.Shuffle(random);
			Reset();
			for (int i = 0; i < bombs; i++)
				Tiles[allPoints[i]] = Tile.Bomb;
			RegenerateBombCountCache();
		}

		private void RegenerateBombCountCache()
		{
			OutOfBoundsValuesMap<Tile> outOfBoundsValuesMap = new(Tiles, Tile.Empty);
			for (int y = Tiles.Bounds.Min.Y; y <= Tiles.Bounds.Max.Y; y++)
			{
				for (int x = Tiles.Bounds.Min.X; x <= Tiles.Bounds.Max.X; x++)
				{
					int count = 0;
					if (outOfBoundsValuesMap[new(x - 1, y - 1)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x, y - 1)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x + 1, y - 1)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x - 1, y)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x + 1, y)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x - 1, y + 1)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x, y + 1)] == Tile.Bomb)
						count++;
					if (outOfBoundsValuesMap[new(x + 1, y + 1)] == Tile.Bomb)
						count++;
					BombCountCache[new(x, y)] = count;
				}
			}
		}

		public CheckResult Check(IntPoint point)
		{
			if (Markings[point] == Marking.Check)
				return CheckResult.KeepGoing;
			Markings[point] = Marking.Check;
			if (Tiles[point] == Tile.Bomb)
				return CheckResult.GameOver;

			FloodFillZeroBombs(point);
			return Tiles.Bounds.AllPointEnumerator().Any(p => Tiles[p] == Tile.Empty && Markings[p] != Marking.Check) ? CheckResult.KeepGoing : CheckResult.Win;
		}

		private void FloodFillZeroBombs(IntPoint point)
		{
			if (BombCountCache[point] != 0)
				return;

			var toCheck = new LinkedList<IntPoint>();
			foreach (var neighbor in point.Neighbors)
				toCheck.AddLast(neighbor);

			while (toCheck.Count != 0)
			{
				var checking = toCheck.First!.Value;
				toCheck.RemoveFirst();

				if (!Tiles.Bounds.Contains(checking))
					continue;
				if (Markings[checking] == Marking.Check)
					continue;

				Markings[checking] = Marking.Check;
				if (BombCountCache[checking] != 0)
					continue;

				foreach (var neighbor in checking.Neighbors)
					toCheck.AddLast(neighbor);
			}
		}

		public void ToggleFlag(IntPoint point)
		{
			if (Markings[point] == Marking.None)
				Markings[point] = Marking.Flag;
			else if (Markings[point] == Marking.Flag)
				Markings[point] = Marking.None;
		}

		public static Color GetColorForBombCount(int bombCount)
		{
			return bombCount switch
			{
				1 => new Color(0f, 0f, 1f),
				2 => new Color(0f, 0.5f, 0f),
				3 => new Color(1f, 0f, 0f),
				4 => new Color(0f, 0f, 0.5f),
				5 => new Color(0.5f, 0f, 0f),
				6 => new Color(0f, 0.5f, 0.5f),
				7 => new Color(0.5f, 0f, 0.5f),
				8 => new Color(0.5f, 0.5f, 0.5f),
				_ => throw new ArgumentException($"Invalid {nameof(bombCount)} {bombCount}."),
			};
		}
	}
}