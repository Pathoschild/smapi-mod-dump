
using MapPings.Framework.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = xTile.Dimensions.Rectangle;

namespace MapPings.Framework {

	public class MapOverlay : IDisposable {

		public bool DrawMapOverlay { get; set; }
		//public TemporaryAnimatedSprite PingArrow { get; set; }
		//public bool PingArrowAnimating { get; set; }
		public bool MapPinged { get; set; }

		private readonly ModConfig config;

		private readonly IReflectionHelper Reflection;

		public bool UpdatePingsPosition { get; set; }
		public Rectangle LastViewport { get; set; }

		private readonly IModHelper modHelper;

		public bool IsMapOpen { get; set; }

		private MapPage mapPage;
		private IReflectedField<int> mapXField;
		private IReflectedField<int> mapYField;

		public Dictionary<Farmer, PlayerMapPing> MapPings { get; set; }

		public MapOverlay(IModHelper helper, ModConfig modConfig) {

			modHelper = helper;
			config = modConfig;
			Reflection = helper.Reflection;

			this.LastViewport = new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);

			MapPings = new Dictionary<Farmer, PlayerMapPing>();

			if(!MapPings.ContainsKey(Game1.player)) {
				MapPings.Add(Game1.player, new PlayerMapPing());
			}

			SubscribeEvents();

		}

		private void SubscribeEvents() {
			MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			GameEvents.UpdateTick += GameEvents_UpdateTick;
			GraphicsEvents.OnPostRenderGuiEvent += GraphicsEvents_OnPostRenderGuiEvent;
			GraphicsEvents.Resize += GraphicsEvents_Resize;
			InputEvents.ButtonPressed += InputEvents_ButtonPressed;
		}

		private void UnsubscribeEvents() {
			MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
			GameEvents.UpdateTick -= GameEvents_UpdateTick;
			GraphicsEvents.OnPostRenderGuiEvent -= GraphicsEvents_OnPostRenderGuiEvent;
			GraphicsEvents.Resize -= GraphicsEvents_Resize;
			InputEvents.ButtonPressed -= InputEvents_ButtonPressed;
		}

		private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e) {

			if(Game1.activeClickableMenu is GameMenu gameMenu) {
				if(gameMenu.currentTab == GameMenu.mapTab) {

					mapPage = (MapPage)Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue()[GameMenu.mapTab];
					mapXField = Reflection.GetField<int>(mapPage, "mapX");
					mapYField = Reflection.GetField<int>(mapPage, "mapY");

					IsMapOpen = true;
				} else {
					IsMapOpen = false;
				}
			}

		}

		private void GameEvents_UpdateTick(object sender, EventArgs e) {
			Update(Game1.currentGameTime);
		}

		private void GraphicsEvents_OnPostRenderGuiEvent(object sender, EventArgs e) {
			Draw(Game1.spriteBatch);
		}

		private void GraphicsEvents_Resize(object sender, EventArgs e) {

			Rectangle newViewport = Game1.viewport;
			if(this.LastViewport.Width != newViewport.Width || this.LastViewport.Height != newViewport.Height) {
				newViewport = new Rectangle(newViewport.X, newViewport.Y, newViewport.Width, newViewport.Height);

				UpdatePingsPosition = true;

				this.LastViewport = newViewport;
			}

		}

		private void InputEvents_ButtonPressed(object sender, EventArgsInput e) {

			if(Game1.activeClickableMenu is GameMenu gameMenu) {
				if(gameMenu.currentTab == GameMenu.mapTab) {
					if(modHelper.Input.IsDown(SButton.LeftAlt) && modHelper.Input.IsDown(SButton.MouseLeft)) {

						Vector2 mapPos = Utility.getTopLeftPositionForCenteringOnScreen(Sprites.Map.SourceRectangle.Width * Game1.pixelZoom, Sprites.Map.SourceRectangle.Height * Game1.pixelZoom);
						Vector2 pingedCoord = new Vector2(e.Cursor.ScreenPixels.X - mapPos.X, e.Cursor.ScreenPixels.Y - mapPos.Y);

						int mapWidth = Sprites.Map.SourceRectangle.Width * Game1.pixelZoom;
						int mapHeight = Sprites.Map.SourceRectangle.Height * Game1.pixelZoom;
						if(IsPingWithinMapBounds(mapWidth, mapHeight, pingedCoord)) {

							//TODO: Send ping to players

							if(config.ShowPingsInChat) {

								string hoverText = Reflection.GetField<string>(mapPage, "hoverText").GetValue();

								if(!String.IsNullOrWhiteSpace(hoverText)) {
									hoverText = $"\"{GetHoverTextLocationName(hoverText)}\"";
								}

								if(!MapPings.ContainsKey(Game1.player)) {
									MapPings.Add(Game1.player, new PlayerMapPing(Color.Red));
								}

								MapPings[Game1.player].AddPing(Game1.player, pingedCoord, hoverText);

								string messageKey = "UserNotificationMessageFormat";
								string messageText = $"{Game1.player.Name} pinged {hoverText} [X:{pingedCoord.X}, Y:{pingedCoord.Y}]";

								if(Game1.IsMultiplayer) {
									Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
									multiplayer.globalChatInfoMessage(messageKey, messageText);
								} else {
									Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, messageText));
								}

							}
#if DEBUG
							ModEntry.ModLogger.Log($"MapCoords => (x: {pingedCoord.X}, y: {pingedCoord.Y})");
							ModEntry.ModLogger.Log($"Map (X: {mapPos.X}, Y: {mapPos.Y})");
#endif

						}

						e.SuppressButton(SButton.MouseLeft);

					}
				}
			}

		}

		private bool IsPingWithinMapBounds(int mapWidth, int mapHeight, Vector2 pingedCoords) {
#if DEBUG
			ModEntry.ModLogger.Log($"map(Width: {mapWidth}, Height: {mapHeight})", $"pingedCoords(X: {pingedCoords.X}, Y: {pingedCoords.Y})");
#endif
			return (pingedCoords.X >= 0 && pingedCoords.X <= mapWidth) && (pingedCoords.Y >= 0 && pingedCoords.Y <= mapHeight);
		}

		private string GetHoverTextLocationName(string hoverText) {
			return hoverText.Contains(Environment.NewLine) ? hoverText.Substring(0, hoverText.IndexOf(Environment.NewLine)) : hoverText;
		}

		private void DrawOverlay(SpriteBatch b) {

		}

		public void Update(GameTime time) {

			foreach(PlayerMapPing playerMapPing in MapPings.Values) {
				if(UpdatePingsPosition) {
					playerMapPing.UpdatePingPosition();
				}
				playerMapPing.update(time);
			}

			UpdatePingsPosition = false;
		}

		public void Draw(SpriteBatch b) {

			if(Game1.activeClickableMenu is GameMenu gameMenu) {
				if(gameMenu.currentTab == GameMenu.mapTab) {

					if(DrawMapOverlay) {
						DrawOverlay(b);
					}

					foreach(PlayerMapPing playerMapPing in MapPings.Values) {
						playerMapPing.draw(b);
					}

				}

			}

		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					UnsubscribeEvents();
				}

				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		#endregion

	}

}
