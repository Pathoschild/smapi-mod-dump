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
using StardewValley.BellsAndWhistles;
using System;
using System.Reflection;

namespace Shockah.Kokoro.GMCM
{
	public class MultiPageLinkOption<T>
	{
		private static readonly BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
		private static readonly string GMCM_Mod_QualifiedName = "GenericModConfigMenu.Mod, GenericModConfigMenu";
		private static readonly string GMCM_SpecificModConfigMenu_QualifiedName = "GenericModConfigMenu.Framework.SpecificModConfigMenu, GenericModConfigMenu";

		private static bool IsReflectionSetup = false;
		private static Func<object? /* SpecificModConfigMenu */> GetActiveSpecificModConfigMenuDelegate = null!;
		private static Action<object /* SpecificModConfigMenu */, string> OpenPageDelegate = null!;

		private const int RowHeight = 60;
		private const float ColumnSpacing = 16f;
		private const float Margin = 16f;
		private static readonly string HoverSoundName = "shiny4";

		private readonly Func<string> Name;
		private readonly Func<T, string> PageID;
		private readonly Func<T, string> PageName;
		private readonly Func<float, int> Columns;
		private readonly T[] PageValues;
		private readonly Func<string>? Tooltip;

		private readonly Lazy<int> ActualColumns;
		private bool? LastMouseLeftPressed;
		private IntPoint? LastHoverPosition;

		public MultiPageLinkOption(
			Func<string> name,
			Func<T, string> pageID,
			Func<T, string> pageName,
			Func<float, int> columns,
			T[] pageValues,
			Func<string>? tooltip = null
		)
		{
			this.Name = name;
			this.PageID = pageID;
			this.PageName = pageName;
			this.Columns = columns;
			this.PageValues = pageValues;
			this.Tooltip = tooltip;

			ActualColumns = new(() => Columns(GetGMCMSize().X));
		}

		internal void AddToGMCM(IGenericModConfigMenuApi api, IManifest mod)
		{
			api.AddComplexOption(
				mod: mod,
				name: Name,
				tooltip: Tooltip,
				draw: (b, position) => Draw(b, position),
				height: () => GetHeight(),
				beforeMenuOpened: () =>
				{
					LastMouseLeftPressed = null;
					LastHoverPosition = null;
				},
				beforeMenuClosed: () => { },
				afterReset: () => { },
				beforeSave: () => { }
			);
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
			int rows = (int)Math.Ceiling(1f * PageValues.Length / ActualColumns.Value) + 1; // extra row, we're not rendering inline
			return rows * RowHeight;
		}

		private void Draw(SpriteBatch b, Vector2 basePosition)
		{
			IntPoint? newHoverPosition = null;

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
			foreach (T pageValue in PageValues)
			{
				Vector2 valuePosition = new(gmcmPosition.X + Margin + (valueSize.X + ColumnSpacing) * column, basePosition.Y + valueSize.Y * row);
				string text = PageName(pageValue);

				Vector2 measure = new(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text));
				bool hoverLink = mouseX >= valuePosition.X && mouseY >= valuePosition.Y && mouseX < valuePosition.X + measure.X && mouseY < valuePosition.Y + measure.Y;
				SpriteText.drawString(b, text, (int)valuePosition.X, (int)valuePosition.Y, layerDepth: 1, color: hoverGMCM && hoverLink ? SpriteText.color_Gray : -1);

				if (hoverGMCM && hoverLink)
				{
					newHoverPosition = new(column, row);
					if (didClick)
						OpenPage(PageID(pageValue));
				}

				if (++column == columns)
				{
					row++;
					column = 0;
				}
			}

			if (newHoverPosition is not null && newHoverPosition != LastHoverPosition)
				Game1.playSound(HoverSoundName);
			LastHoverPosition = newHoverPosition;
		}

		private static void SetupReflectionIfNeeded()
		{
			if (IsReflectionSetup)
				return;

			Type gmcmModType = Type.GetType(GMCM_Mod_QualifiedName)!;
			Type gmcmSpecificModConfigMenuType = Type.GetType(GMCM_SpecificModConfigMenu_QualifiedName)!;

			MethodInfo activeConfigMenuGetter = gmcmModType.GetMethod("get_ActiveConfigMenu", allFlags)!;
			GetActiveSpecificModConfigMenuDelegate = () =>
			{
				var result = activeConfigMenuGetter.Invoke(null, null);
				return result?.GetType().Name == "SpecificModConfigMenu" ? result : null;
			};

			FieldInfo openPageField = gmcmSpecificModConfigMenuType.GetField("OpenPage", allFlags)!;
			OpenPageDelegate = (menu, pageID) => ((Action<string>)openPageField.GetValue(menu)!).Invoke(pageID);

			IsReflectionSetup = true;
		}

		private void OpenPage(string pageID)
		{
			SetupReflectionIfNeeded();

			var menu = GetActiveSpecificModConfigMenuDelegate();
			if (menu is null)
				return;
			OpenPageDelegate(menu, pageID);
		}
	}

	public static class MultiPageLinkOptionExtensions
	{
		public static void AddMultiPageLinkOption<T>(
			this IGenericModConfigMenuApi api,
			IManifest mod,
			Func<string> name,
			Func<T, string> pageID,
			Func<T, string> pageName,
			Func<float, int> columns,
			T[] pageValues,
			Func<string>? tooltip = null
		)
		{
			var option = new MultiPageLinkOption<T>(name, pageID, pageName, columns, pageValues, tooltip);
			option.AddToGMCM(api, mod);
		}
	}
}
