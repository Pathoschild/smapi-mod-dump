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
using System.Linq;

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.GameData.LocationContexts;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Leclair.Stardew.CloudySkies.Menus;

public class TVWeatherMenu : IClickableMenu {

	private readonly ModEntry Mod;
	public readonly TV Television;

	private IClickableMenu? childMenu;

	private TemporaryAnimatedSprite? ScreenBase;
	private TemporaryAnimatedSprite? ScreenOverlay;
	private TemporaryAnimatedSprite? WeatherOverlay;

	private readonly Dictionary<string, LocationContextExtensionData> Contexts;

	private readonly ScrollableFlow Flow;

	[SkipForClickableAggregation]
	public ClickableTextureComponent btnPageUp;
	[SkipForClickableAggregation]
	public ClickableTextureComponent btnPageDown;

	public List<ClickableComponent> FlowComponents;

	private string? LastHoverContext = null;
	private string? LastHoverWeather = null;

	public TVWeatherMenu(ModEntry mod, TV tv) {
		Mod = mod;
		Television = tv;

		// We do non standard rendering, so there.
		width = 1240;
		height = 200;

		initialize((int) Utility.getTopLeftPositionForCenteringOnScreen(width, height).X, Game1.uiViewport.Height - height - 48, width, height);

		// Right. We are taking over all rendering, so turn off the TV.
		Television.turnOffTV();

		// Now, let's see which locations we have access to!
		Mod.LoadContextData();
		Contexts = new();

		foreach (var entry in Mod.ContextData) {
			if (!entry.Value.IncludeInWeatherChannel)
				continue;

			if (!string.IsNullOrEmpty(entry.Value.WeatherChannelCondition) && !GameStateQuery.CheckConditions(entry.Value.WeatherChannelCondition))
				continue;

			// It's good. Add it!
			Contexts[entry.Key] = entry.Value;
		}

		// And add the light source.
		bool is_plasma = Television.QualifiedItemId == "(F)1468";

		Game1.currentLightSources.Add(
			new LightSource(
				2,
				Television.getScreenPosition() + (is_plasma ? new Vector2(88, 80) : new Vector2(38, 48)),
				is_plasma ? 1f : 0.55f,
				Color.Black,
				70907,
				LightSource.LightContext.None,
				0L
			)
		);

		Flow = new(this, xPositionOnScreen + 20, yPositionOnScreen + 20, width - 36, height - 36);
		btnPageUp = Flow.btnPageUp;
		btnPageDown = Flow.btnPageDown;
		FlowComponents = Flow.DynamicComponents;

		if (Contexts.Count == 0)
			exitThisMenu();

		else if (Contexts.Count == 1)
			ShowWeatherFor(Contexts.Keys.First());

		else
			ShowList();

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	public void DrawWorld(SpriteBatch batch) {
		ScreenBase?.draw(batch);
		ScreenOverlay?.draw(batch);
		WeatherOverlay?.draw(batch);
	}

	private TemporaryAnimatedSprite MakeSprite(string texture, Point source, int frames, float speed = 150f, bool isSmall = false, int layerOffset = 0) {
		float layerDepth = (Television.boundingBox.Bottom - 1f) / 10000f + 1E-05f;
		while (layerOffset-- > 0)
			layerDepth = MathF.BitIncrement(layerDepth);

		Rectangle sourceRect;
		if (isSmall)
			sourceRect = new Rectangle(source.X, source.Y, 13, 13);
		else
			sourceRect = new Rectangle(source.X, source.Y, 42, 28);

		return new(
			texture,
			sourceRect,
			speed,
			frames,
			999999,
			Television.getScreenPosition() + (isSmall ? new Vector2(3f, 3f) * Television.getScreenSizeModifier() : Vector2.Zero),
			flicker: false,
			flipped: false,
			layerDepth,
			0f,
			Color.White,
			Television.getScreenSizeModifier(),
			0f,
			0f,
			0f
		);
	}

	public void SetUpBackground(LocationContextExtensionData? data, string? weather) {

		if (LastHoverContext == data?.Id && LastHoverWeather == weather)
			return;

		LastHoverContext = data?.Id;
		LastHoverWeather = weather;

		if (data is null) {
			ScreenBase = null;
			ScreenOverlay = null;
			WeatherOverlay = null;
			return;
		}

		string texture;
		Point source;
		int frames;
		float speed;

		if (data.WeatherChannelBackgroundTexture != null) {
			texture = data.WeatherChannelBackgroundTexture;
			source = data.WeatherChannelBackgroundSource;
			frames = data.WeatherChannelBackgroundFrames;
			speed = data.WeatherChannelBackgroundSpeed;

		} else {
			texture = Game1.mouseCursorsName;
			source = new Point(413 + (weather is null ? 0 : (42 * 2)), 305);
			frames = weather is null ? 2 : 1;
			speed = 150f;
		}

		if (frames < 1)
			frames = 1;

		ScreenBase = MakeSprite(texture, source, frames, speed, false, 0);

		if (data.WeatherChannelOverlayTexture == null && data.WeatherChannelBackgroundTexture == null) {
			ScreenOverlay = null;

		} else {
			if (data.WeatherChannelOverlayTexture == null) {
				texture = ModEntry.WEATHER_OVERLAY_DEFAULT_ASSET;
				source = weather is null ? Point.Zero : new Point(42 * 2, 0);
				frames = weather is null ? 2 : 1;
				speed = 150f;

			} else {
				texture = data.WeatherChannelOverlayTexture;
				if (weather is null) {
					source = data.WeatherChannelOverlayIntroSource;
					frames = data.WeatherChannelOverlayIntroFrames;
					speed = data.WeatherChannelOverlayIntroSpeed;
				} else {
					source = data.WeatherChannelOverlayWeatherSource
						?? new Point(data.WeatherChannelOverlayIntroSource.X + (42 * 2), data.WeatherChannelOverlayIntroSource.Y);
					frames = data.WeatherChannelOverlayWeatherFrames;
					speed = data.WeatherChannelOverlayWeatherSpeed;
				}
			}

			if (frames < 1)
				frames = 1;

			ScreenOverlay = MakeSprite(
				texture,
				source,
				frames,
				speed,
				false,
				1
			);
		}

		if (Mod.TryGetWeather(weather, out var weatherData)) {
			if (weatherData?.TVTexture is null)
				WeatherOverlay = null;
			else {
				texture = weatherData.TVTexture;
				source = weatherData.TVSource;
				frames = weatherData.TVFrames;
				speed = weatherData.TVSpeed;

				if (frames < 1)
					frames = 1;

				// Doesn't use MakeSprite because it has a different size + offset.
				WeatherOverlay = MakeSprite(
					texture,
					source,
					frames,
					speed,
					true,
					2
				);
			}

		} else if (weather != null) {
			// Reflect our way to vanilla weather.
			AccessTools.Method(typeof(TV), "setWeatherOverlay", [typeof(string)])?.Invoke(Television, [weather]);
			var spriteField = Mod.Helper.Reflection.GetField<TemporaryAnimatedSprite?>(Television, "screenOverlay");
			WeatherOverlay = spriteField.GetValue();
			spriteField.SetValue(null);

		} else
			WeatherOverlay = null;
	}


	private readonly static TextStyle FADED = new TextStyle(TextStyle.FANCY, opacity: 0.5f);

	public SelectableNode BuildNode(LocationContextExtensionData entry, List<TextNode> nodeList) {
		var text = new TextNode(Mod.TokenizeText(entry.DisplayName ?? entry.Id), FADED);
		nodeList.Add(text);

		return new SelectableNode(
			[text],
			onClick: (_, _, _) => {
				ShowWeatherFor(entry.Id);
				return false;
			},
			onHover: (_, _, _) => {
				foreach (var node in nodeList)
					node.Style = FADED;
				text.Style = TextStyle.FANCY;

				SetUpBackground(entry, null);
				return true;
			}
		) {
			HoverTexture = Game1.mouseCursors,
			HoverSource = new Rectangle(448, 128, 64, 64),
			HoverColor = Color.White,
			HoverScale = 1f
		};
	}

	public void ShowList() {
		var builder = FlowHelper.Builder();

		builder.Text($"{I18n.Tv_SelectLocation()}\n", TextStyle.FANCY);
		builder.Text("\n", scale: 0.5f);

		var normalStyle = TextStyle.FANCY;
		var fadedStyle = new TextStyle(normalStyle, opacity: 0.5f);

		List<TextNode> nodes = new();

		foreach (var entry in Contexts.Values)
			builder.Add(BuildNode(entry, nodes));

		{
			var text = new TextNode(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13118"), fadedStyle);
			nodes.Add(text);

			var node = new SelectableNode(
				[text],
				onClick: (_, _, _) => {
					exitThisMenu();
					return true;
				},
				onHover: (_, _, _) => {
					foreach (var node in nodes)
						node.Style = fadedStyle;
					text.Style = normalStyle;

					SetUpBackground(null, null);

					return true;
				}
			) {
				HoverTexture = Game1.mouseCursors,
				HoverSource = new Rectangle(448, 128, 64, 64),
				HoverColor = Color.White,
				HoverScale = 1f
			};

			builder.Add(node);
		}

		Flow.Set(builder.Build());
		populateClickableComponentList();

		height = (int) Math.Clamp(Flow.ContentHeight, 200, 300) + 32;
		yPositionOnScreen = Game1.uiViewport.Height - height - 32;

		Flow.Reposition(xPositionOnScreen + 32, yPositionOnScreen + 24, width - 48, height - 48);
	}

	private string? GetForecastFor(string contextId) {
		WorldDate tomorrow = new WorldDate(Game1.Date);
		tomorrow.TotalDays++;

		string? forecast;
		if (contextId == "Default") {
			forecast = Game1.IsMasterGame ? Game1.weatherForTomorrow : Game1.netWorldState.Value.WeatherForTomorrow;
			return Game1.getWeatherModificationsForDate(tomorrow, forecast);

		} else {
			if (Utility.isFestivalDay(tomorrow.DayOfMonth, tomorrow.Season, contextId))
				return "Festival";
			else if (Utility.IsPassiveFestivalDay(tomorrow.DayOfMonth, tomorrow.Season, contextId)) {
				// TODO: Passive festival weather???
			}

			return Game1.netWorldState.Value.GetWeatherForLocation(contextId)?.WeatherForTomorrow;
		}
	}

	public void ShowWeatherFor(string contextId) {
		Flow.Set(null);
		populateClickableComponentList();

		if (!Contexts.TryGetValue(contextId, out var extData) || LocationContexts.Require(contextId) is not LocationContextData data) {
			exitThisMenu();
			return;
		}

		string? weather = GetForecastFor(contextId);

		string? forecast = null;
		if (string.IsNullOrEmpty(weather)) {
			// Do nothing.

		} else if (Mod.TryGetWeather(weather, out var weatherData)) {
			if (weatherData.ForecastByContext != null && weatherData.ForecastByContext.TryGetValue(contextId, out string? msg))
				forecast = msg;
			else
				forecast = weatherData.Forecast;

		} else {
			var method = AccessTools.Method(typeof(TV), "getWeatherForecast", [typeof(string)]);
			if (method.Invoke(Television, [weather]) is string sout)
				forecast = sout;
		}

		// If forecast is null, show a message for that.
		forecast ??= Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");

		// Add our prefix, if there is one.
		if (extData.WeatherForecastPrefix != null)
			forecast = extData.WeatherForecastPrefix + forecast;

		// Set up our sprites.
		SetUpBackground(extData, weather);

		// Show a dialogue sub-menu.
		childMenu = new DialogueBox(Mod.TokenizeText(forecast));
		Game1.afterDialogues = () => {
			exitThisMenu(false);
		};
	}

	#region Menu Stuff

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = FlowComponents.Count > 0 ? FlowComponents[0] : null;
		if (currentlySnappedComponent != null)
			snapCursorToCurrentSnappedComponent();
	}

	protected override void cleanupBeforeExit() {
		base.cleanupBeforeExit();

		childMenu?.emergencyShutDown();
		childMenu = null;

		Utility.removeLightSource(70907);
	}

	public override void emergencyShutDown() {
		base.emergencyShutDown();

		childMenu?.emergencyShutDown();
		childMenu = null;
	}

	public override bool readyToClose() {
		if (childMenu != null && !childMenu.readyToClose())
			return false;

		return base.readyToClose();
	}

	public override void receiveScrollWheelAction(int direction) {
		if (childMenu is not null) {
			childMenu.receiveScrollWheelAction(direction);
			return;
		}

		base.receiveScrollWheelAction(direction);

		if (Flow.Scroll(direction > 0 ? -1 : 1)) {
			if (Game1.options.SnappyMenus)
				snapCursorToCurrentSnappedComponent();
			Game1.playSound("shwip");
		}
	}

	public override void releaseLeftClick(int x, int y) {
		if (childMenu is not null) {
			childMenu.releaseLeftClick(x, y);
			return;
		}

		base.releaseLeftClick(x, y);
		Flow.ReleaseLeftClick();
	}

	public override void leftClickHeld(int x, int y) {
		if (childMenu != null) {
			childMenu.leftClickHeld(x, y);
			return;
		}

		base.leftClickHeld(x, y);
		Flow.LeftClickHeld(x, y);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		if (childMenu is not null) {
			childMenu.receiveLeftClick(x, y, playSound);
			return;
		}

		base.receiveLeftClick(x, y, playSound);
		if (Flow.ReceiveLeftClick(x, y, playSound))
			return;

		if (x < xPositionOnScreen || x > (xPositionOnScreen + width) || y < yPositionOnScreen || y > (yPositionOnScreen + height))
			exitThisMenu();
	}

	public override void performHoverAction(int x, int y) {
		if (childMenu is not null) {
			childMenu.performHoverAction(x, y);
			x = -1;
			y = -1;
		}

		base.performHoverAction(x, y);

		if (Flow.PerformMiddleScroll(x, y))
			return;

		Flow.PerformHover(x, y);
	}

	public override void receiveGamePadButton(Buttons b) {
		if (childMenu is not null) {
			childMenu.receiveGamePadButton(b);
			return;
		}

		base.receiveGamePadButton(b);
	}

	public override void receiveKeyPress(Keys key) {
		if (childMenu is not null) {
			childMenu.receiveKeyPress(key);
			return;
		}

		base.receiveKeyPress(key);
	}

	public override void gamePadButtonHeld(Buttons b) {
		if (childMenu is not null) {
			childMenu.gamePadButtonHeld(b);
			return;
		}

		base.gamePadButtonHeld(b);
	}

	public override void receiveRightClick(int x, int y, bool playSound = true) {
		if (childMenu is not null) {
			childMenu.receiveRightClick(x, y, playSound);
			return;
		}

		base.receiveRightClick(x, y, playSound);
	}

	#endregion

	public override void update(GameTime time) {
		base.update(time);

		ScreenBase?.update(time);
		ScreenOverlay?.update(time);
		WeatherOverlay?.update(time);

		childMenu?.update(time);
	}

	public override void draw(SpriteBatch b) {

		if (childMenu is not null) {
			childMenu.draw(b);
			return;
		}

		// Background
		RenderHelper.DrawDialogueBox(b, xPositionOnScreen - 16, yPositionOnScreen - 16, width + 32, height + 32);

		Flow.Draw(b);

		Flow.DrawMiddleScroll(b);

		Game1.mouseCursorTransparency = 1f;
		drawMouse(b);

	}

}
