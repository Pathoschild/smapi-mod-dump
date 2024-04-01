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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Kokoro.GMCM;

public class MultiSelectTextOption<T>
{
	private const int RowHeight = 60;
	private const float ColumnSpacing = 16f;
	private const float CheckboxScale = 4f;
	private const float Margin = 16f;
	private static readonly string ClickSoundName = "drumkit6";

	private readonly Func<T, bool> GetValue;
	private readonly Action<T> AddValue;
	private readonly Action<T> RemoveValue;
	private readonly Func<string> Name;
	private readonly Func<float, int> Columns;
	private readonly T[] AllowedValues;
	private readonly Func<string>? Tooltip;
	private readonly Func<T, string>? FormatAllowedValue;
	private readonly Action? AfterValuesUpdated;

	private readonly Lazy<Texture2D> CheckedTexture = new(() => Game1.mouseCursors);
	private readonly Lazy<Rectangle> CheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectChecked);
	private readonly Lazy<Texture2D> UncheckedTexture = new(() => Game1.mouseCursors);
	private readonly Lazy<Rectangle> UncheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectUnchecked);

	private ISet<T> OriginalValues = new HashSet<T>();
	private ISet<T> CurrentValues = new HashSet<T>();
	private readonly Lazy<int> ActualColumns;
	private bool? LastMouseLeftPressed;

	public MultiSelectTextOption(
		Func<T, bool> getValue,
		Action<T> addValue,
		Action<T> removeValue,
		Func<string> name,
		Func<float, int> columns,
		T[] allowedValues,
		Func<string>? tooltip = null,
		Func<T, string>? formatAllowedValue = null,
		Action? afterValuesUpdated = null
	)
	{
		this.GetValue = getValue;
		this.AddValue = addValue;
		this.RemoveValue = removeValue;
		this.Name = name;
		this.Columns = columns;
		this.AllowedValues = allowedValues;
		this.Tooltip = tooltip;
		this.FormatAllowedValue = formatAllowedValue;
		this.AfterValuesUpdated = afterValuesUpdated;

		ActualColumns = new(() => Columns(GetGMCMSize().X));
	}

	private void Initialize()
	{
		OriginalValues = AllowedValues.Where(v => GetValue(v)).ToHashSet();
		CurrentValues = OriginalValues.ToHashSet();
	}

	internal void AddToGMCM(IGenericModConfigMenuApi api, IManifest mod)
	{
		api.AddComplexOption(
			mod: mod,
			name: Name,
			tooltip: Tooltip,
			draw: Draw,
			height: GetHeight,
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
		bool hasAnyChange = false;
		foreach (var allowedValue in AllowedValues)
		{
			bool wasSet = OriginalValues.Contains(allowedValue);
			bool isSet = CurrentValues.Contains(allowedValue);
			if (wasSet != isSet)
			{
				if (isSet)
					AddValue(allowedValue);
				else
					RemoveValue(allowedValue);
				hasAnyChange = true;
			}
		}
		if (hasAnyChange)
		{
			OriginalValues = CurrentValues.ToHashSet();
			AfterValuesUpdated?.Invoke();
		}
	}

	private Vector2 GetGMCMSize()
		=> new(Math.Min(1200, Game1.uiViewport.Width - 200), Game1.uiViewport.Height - 128 - 116);

	private Vector2 GetGMCMPosition(Vector2? size = null)
	{
		Vector2 gmcmSize = size ?? GetGMCMSize();
		return new((Game1.uiViewport.Width - gmcmSize.X) / 2, (Game1.uiViewport.Height - gmcmSize.Y) / 2);
	}

	private int GetHeight()
	{
		int rows = (int)Math.Ceiling(1f * AllowedValues.Length / ActualColumns.Value) + 1; // extra row, we're not rendering inline
		return rows * RowHeight;
	}

	private void Draw(SpriteBatch b, Vector2 basePosition)
	{
		bool mouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
		bool didClick = mouseLeftPressed && LastMouseLeftPressed == false;
		LastMouseLeftPressed = mouseLeftPressed;
		int mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
		int mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

		int columns = ActualColumns.Value;
		Vector2 gmcmSize = GetGMCMSize();
		Vector2 gmcmPosition = GetGMCMPosition(gmcmSize);
		bool hoverGMCM = mouseX >= gmcmPosition.X && mouseY >= gmcmPosition.Y && mouseX < gmcmPosition.X + gmcmSize.X && mouseY < gmcmPosition.Y + gmcmSize.Y;
		float columnWidth = (gmcmSize.X - (columns - 1) * ColumnSpacing - Margin) / columns;
		Vector2 valueSize = new(columnWidth, RowHeight);

		int row = 1;
		int column = 0;
		foreach (T allowedValue in AllowedValues)
		{
			Vector2 valuePosition = new(gmcmPosition.X + Margin + (valueSize.X + ColumnSpacing) * column, basePosition.Y + valueSize.Y * row);
			bool isChecked = CurrentValues.Contains(allowedValue);
			Texture2D texture = isChecked ? CheckedTexture.Value : UncheckedTexture.Value;
			Rectangle textureSourceRect = isChecked ? CheckedTextureSourceRect.Value : UncheckedTextureSourceRect.Value;
			string text = FormatAllowedValue is null ? $"{allowedValue}" : FormatAllowedValue(allowedValue);

			b.Draw(texture, valuePosition + new Vector2(0, 3), textureSourceRect, Color.White, 0, Vector2.Zero, CheckboxScale, SpriteEffects.None, 0);
			Utility.drawTextWithShadow(b, text, Game1.dialogueFont, valuePosition + new Vector2(textureSourceRect.Width * CheckboxScale + 8, 0), Game1.textColor, 1f);

			bool hoverCheckbox = mouseX >= valuePosition.X && mouseY >= valuePosition.Y && mouseX < valuePosition.X + textureSourceRect.Width * CheckboxScale && mouseY < valuePosition.Y + textureSourceRect.Height * CheckboxScale;
			if (hoverGMCM && hoverCheckbox && didClick)
			{
				if (CurrentValues.Contains(allowedValue))
					CurrentValues.Remove(allowedValue);
				else
					CurrentValues.Add(allowedValue);
				Game1.playSound(ClickSoundName);
			}

			if (++column == columns)
			{
				row++;
				column = 0;
			}
		}
	}
}

public static class MultiSelectTextOptionExtensions
{
	public static void AddMultiSelectTextOption<T>(
		this IGenericModConfigMenuApi api,
		IManifest mod,
		Func<T, bool> getValue,
		Action<T> addValue,
		Action<T> removeValue,
		Func<string> name,
		Func<float, int> columns,
		T[] allowedValues,
		Func<string>? tooltip = null,
		Func<T, string>? formatAllowedValue = null,
		Action? afterValuesUpdated = null
	)
	{
		var option = new MultiSelectTextOption<T>(getValue, addValue, removeValue, name, columns, allowedValues, tooltip, formatAllowedValue, afterValuesUpdated);
		option.AddToGMCM(api, mod);
	}

	public static void AddMultiSelectTextOption<T>(
		this IGenericModConfigMenuApi api,
		IManifest mod,
		Func<IReadOnlySet<T>> getValues,
		Action<IReadOnlySet<T>> setValues,
		Func<string> name,
		Func<float, int> columns,
		T[] allowedValues,
		Func<string>? tooltip = null,
		Func<T, string>? formatAllowedValue = null
	)
	{
		Lazy<IReadOnlySet<T>> originalValues = new(() => getValues());
		Lazy<ISet<T>> currentValues = new(() => originalValues.Value.ToHashSet());
		AddMultiSelectTextOption(
			api: api,
			mod: mod,
			getValue: originalValues.Value.Contains,
			addValue: value => currentValues.Value.Add(value),
			removeValue: value => currentValues.Value.Remove(value),
			name: name,
			columns: columns,
			allowedValues: allowedValues,
			tooltip: tooltip,
			formatAllowedValue: formatAllowedValue,
			afterValuesUpdated: () => setValues((IReadOnlySet<T>)currentValues.Value)
		);
	}
}