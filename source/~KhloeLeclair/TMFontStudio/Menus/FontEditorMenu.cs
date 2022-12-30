/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.ThemeManagerFontStudio.Managers;
using Leclair.Stardew.ThemeManagerFontStudio.Models;
using Leclair.Stardew.ThemeManagerFontStudio.Sources;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteFontPlus;

using StardewValley;
using StardewValley.Menus;

using StbTrueTypeSharp;

namespace Leclair.Stardew.ThemeManagerFontStudio.Menus;

public class FontEditorMenu : IClickableMenu {

	public readonly ModEntry Mod;

	// Flow Rendering
	public readonly ScrollableFlow PreviewFlow;

	// Proxied Flow Elements
	public ClickableTextureComponent btnPreviewPageUp;
	public ClickableTextureComponent btnPreviewPageDown;

	public List<ClickableComponent> PreviewFlowComponents;

	public readonly Dictionary<IFontData, Tuple<SpriteFont?>> BuildFonts = new();

	public bool FlowDirty;

	public FontEditorMenu(ModEntry mod) : base(0, 0, 1280, 720, true) {
		Mod = mod;

		// Flow
		PreviewFlow = new ScrollableFlow(
			parent: this,
			x: 0, y: 0, width: 0, height: 0,
			showScrollbar: ScrollVisibility.Visible
		);

		btnPreviewPageUp = PreviewFlow.btnPageUp;
		btnPreviewPageDown = PreviewFlow.btnPageDown;
		PreviewFlowComponents = PreviewFlow.DynamicComponents;

		Mod.SourceManager.LoadFonts();
		Mod.SourceManager.LoadingComplete += OnLoadingComplete;
		Mod.SourceManager.LoadingProgress += (sender, e) => {
			UpdateFlow(e);
		};

		UpdateFlow();

		UpdateLayout();
	}

	#region Discover Fonts

	public void UpdateFlow(ProgressEventArgs? progress = null) {

		FlowDirty = false;

		var builder = FlowHelper.Builder();

		var fonts = Mod.SourceManager.GetLoadedFonts();

		int i = 5;

		if (Mod.SourceManager.IsLoading) {
			builder.Text("Loading fonts...");
			if (progress is not null)
				builder
					.Text($"\n\nSources: {progress.FinishedSources} of {progress.TotalSources}")
					.Text($"\nFonts: {progress.FinishedFonts} of {progress.TotalFonts}");
			
		} else if (fonts is null || fonts.Count == 0)
			builder.Text("There are no fonts.");
		else {
			int c = 0;
			foreach (var font in fonts) {
				c++;
				SpriteFont? sfont = null;
				if (font.IsLoaded) {
					if (BuildFonts.TryGetValue(font, out var value))
						sfont = value.Item1;
					else {
						TtfFontBakerResult? result;
						try {
							result = FontUtilities.BakeFont(font, 25, 1024, 1024, new CharacterRange[] {
								CharacterRange.BasicLatin,
								CharacterRange.Latin1Supplement,
								CharacterRange.LatinExtendedA,
								CharacterRange.LatinExtendedB,
							});

							sfont = result.CreateSpriteFont(Game1.graphics.GraphicsDevice);

						} catch (Exception ex) {
							Mod.Log($"Error while baking font {font.FamilyName} ({font.SubfamilyName}) '{font.UniqueId}': {ex}", StardewModdingAPI.LogLevel.Warn);
						}

						BuildFonts[font] = new(sfont);
					}

				} else if (i-- > 0 && c < 100)
					Mod.SourceManager.LoadFont(font, _ => FlowDirty = true);

				bool onClick(IFlowNodeSlice slice, int x, int y) {
					if (sfont is not null) {
						try {
							TtfFontBakerResult? result = FontUtilities.BakeFont(font, 25, 512, 512, new CharacterRange[] {
								new CharacterRange((char) 32, (char) 383),
								new CharacterRange((char) 8212, (char) 8250),
								new CharacterRange((char) 8364, (char) 8364),
								new CharacterRange((char) 9825, (char) 9825)
							});

							sfont = result.CreateSpriteFont(Game1.graphics.GraphicsDevice);
						} catch(Exception ex) {
							Mod.Log($"Could not create updated font: {ex}", StardewModdingAPI.LogLevel.Warn);
						}

						string path = Path.Join(Mod.Helper.DirectoryPath, "Fonts", $"{font.FamilyName} ({font.SubfamilyName}) 25.png");
						string datapath = Path.Join(Mod.Helper.DirectoryPath, "Fonts", $"{font.FamilyName} ({font.SubfamilyName}) 25.json");
						Directory.CreateDirectory(Path.GetDirectoryName(path)!);

						Color[] pixels = GC.AllocateUninitializedArray<Color>(512 * 512);
						sfont.Texture.GetData(pixels);

						int maxY = 512;

						for(int yp = 511; yp >= 0; yp--) {
							bool hit = false;
							for(int xp = 0; xp < 512; xp++) {
								int idx = yp * 512 + xp;
								Color val = pixels[idx];
								if (val.A != 0) {
									hit = true;
									break;
								}
							}
							if (hit)
								break;
							else
								maxY = yp;
						}

						sfont.Texture.SaveAsPng(File.Create(path), sfont.Texture.Width, Math.Min(sfont.Texture.Height, maxY + 2));

						var data = RawSpriteFontData.FromSpriteFont(sfont);
						Mod.Helper.Data.WriteJsonFile(Path.GetRelativePath(Mod.Helper.DirectoryPath, datapath), data);

					} else
						Mod.SourceManager.LoadFont(font, _ => FlowDirty = true);
					return true;
				};

				builder.Text($"{font.FamilyName} ({font.SubfamilyName})", onClick: onClick);
				if (sfont is not null)
					builder.Text($"This is a sentence in {font.FamilyName}.", new TextStyle(font: sfont, shadow: false));

				builder.Text($"\n{font.Source}: {font.UniqueId}\n\n");
			}

			Mod.Log($"Builder Nodes: {builder.Count}", StardewModdingAPI.LogLevel.Info);
		}

		PreviewFlow.Set(builder.Build());
	}

	#endregion

	#region Layout

	public void UpdateLayout() {
		xPositionOnScreen = 0;
		yPositionOnScreen = 0;
		width = Game1.uiViewport.Width;
		height = Game1.uiViewport.Height;

		if (upperRightCloseButton != null)
			upperRightCloseButton.bounds = new Rectangle(
				Game1.uiViewport.Width - upperRightCloseButton.bounds.Width,
				0,
				upperRightCloseButton.bounds.Width,
				upperRightCloseButton.bounds.Height
			);

		PreviewFlow.Reposition(
			x: (width / 2) + borderWidth + 8,
			y: yPositionOnScreen + borderWidth,
			width: (width / 2) - (borderWidth * 2) - 8,
			height: height - (borderWidth * 2),
			scrollTopMargin: 0,
			scrollBottomMargin: 0
		);
	}

	#endregion

	#region Events

	private void OnLoadingComplete(object? sender, EventArgs e) {
		UpdateFlow();
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
		UpdateLayout();
	}

	public override void leftClickHeld(int x, int y) {
		base.leftClickHeld(x, y);

		if (PreviewFlow.HasValue && PreviewFlow.LeftClickHeld(x, y))
			return;
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		if ((PreviewFlow.IsMiddleScrolling() || PreviewFlow.HasValue) && PreviewFlow.PerformMiddleScroll(x, y))
			return;

		PreviewFlow.PerformHover(x, y);
	}

	public override void receiveKeyPress(Keys key) {
		base.receiveKeyPress(key);

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (PreviewFlow.HasValue && PreviewFlow.Bounds.Contains(x, y)) {
			if (key == Keys.PageDown && PreviewFlow.ScrollPage(1))
				Game1.playSound("shiny4");

			if (key == Keys.PageUp && PreviewFlow.ScrollPage(-1))
				Game1.playSound("shiny4");

			if (key == Keys.Home && PreviewFlow.ScrollToStart())
				Game1.playSound("shiny4");

			if (key == Keys.End && PreviewFlow.ScrollToEnd())
				Game1.playSound("shiny4");

			return;
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		if (PreviewFlow.HasValue && PreviewFlow.ReceiveLeftClick(x, y, playSound))
			return;
	}

	public override void receiveRightClick(int x, int y, bool playSound = true) {
		base.receiveRightClick(x, y, playSound);

		if (PreviewFlow.HasValue && PreviewFlow.ReceiveRightClick(x, y))
			return;
	}

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (PreviewFlow.HasValue && PreviewFlow.Bounds.Contains(x, y)) {
			if (PreviewFlow.Scroll(direction > 0 ? -1 : 1)) {
				Game1.playSound("shiny4");
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}

			return;
		}
	}

	public override void releaseLeftClick(int x, int y) {
		base.releaseLeftClick(x, y);

		PreviewFlow.ReleaseLeftClick();
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {

		if (FlowDirty)
			UpdateFlow();

		// Background
		b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		// We need to tweak these variables because the base game is stupid.
		yPositionOnScreen -= 64;
		height += 64;

		Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true, ignoreTitleSafe: true);
		drawVerticalPartition(b, width / 2);

		yPositionOnScreen += 64;
		height -= 64;

		// Components

		// Flow Draw
		if (PreviewFlow.HasValue)
			PreviewFlow.Draw(
				batch: b,
				defaultColor: Game1.textColor,
				defaultShadowColor: null
			);

		base.draw(b);

		// Flow: Part 2
		PreviewFlow.DrawMiddleScroll(b);

		// Mouse
		drawMouse(b);

		// Tooltips
	}

	#endregion

}
