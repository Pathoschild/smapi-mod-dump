/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jbossjaslow/Chatter
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chatter {
	internal class ShowWhenNPCNeedsChat : IDisposable {
		private readonly PerScreen<float> _yMovementPerDraw = new();
		private readonly PerScreen<float> _alpha = new();
		private readonly PerScreen<int> _pauseTicks = new(createNewState: () => 60);
		private readonly IModHelper _helper;
		private readonly IMonitor _monitor;
		private readonly ModConfig _config;
		private readonly Dictionary<string, int> _npcOffsets;

		// Custom indicator properties
		private readonly Texture2D defaultIndicatorTexture;
		private readonly Texture2D? customIndicatorTexture;
		private readonly Rectangle defaultIndicatorBounds;
		private readonly Rectangle? customIndicatorBounds;
		private readonly Texture2D defaultBirthdayIndicatorTexture;
		private readonly Texture2D? customBirthdayIndicatorTexture;
		private readonly Rectangle defaultBirthdayIndicatorBounds;
		private readonly Rectangle? customBirthdayIndicatorBounds;

		// Indicator properties
		private readonly Color indicatorColor = Color.White * 0.9f;
		private readonly float indicatorRotation = 0.0f;
		private readonly Vector2 indicatorOrigin = Vector2.Zero;
		private readonly SpriteEffects indicatorSpriteEffects = SpriteEffects.None;
		private readonly float indicatorLayerDepth = 1f;

		// NPCs that should not receive indicators
		private readonly string[] nonInteractableVillagers = {
			"BabyPig",
			"Pig",
			"White Chicken",
			"Brown Chicken",
			"White Cow",
			"BabyWhite Cow",
			"PonyRide"
		};

		public ShowWhenNPCNeedsChat(IModHelper helper, IMonitor monitor, ModConfig config, Dictionary<string, int> npcOffsets) {
			_helper = helper;
			_monitor = monitor;
			_config = config;
			_npcOffsets = npcOffsets;

			defaultIndicatorTexture = Game1.emoteSpriteSheet;
			int spriteSize = Game1.tileSize / 4;
			defaultIndicatorBounds = new(
				3 * spriteSize % Game1.emoteSpriteSheet.Width,
				3 * spriteSize / Game1.emoteSpriteSheet.Width * spriteSize,
				spriteSize,
				spriteSize
			);

			try {
				customIndicatorTexture = _helper.ModContent.Load<Texture2D>("Customization/indicator.png");
				customIndicatorBounds = customIndicatorTexture.Bounds;
				_monitor.Log($"Custom indicator exists", LogLevel.Debug);
			} catch {
				_monitor.Log($"Custom indicator does not exist", LogLevel.Info);
			}

			try {
				defaultBirthdayIndicatorTexture = _helper.ModContent.Load<Texture2D>("birthdayIndicator.png");
				defaultBirthdayIndicatorBounds = defaultBirthdayIndicatorTexture.Bounds;
				_monitor.Log($"Default birthday indicator exists", LogLevel.Debug);
			} catch {
				defaultBirthdayIndicatorTexture = defaultIndicatorTexture;
				defaultBirthdayIndicatorBounds = defaultIndicatorBounds;
				_monitor.Log($"Default birthday indicator does not exist, replacing with default indicator", LogLevel.Error);
			}

			try {
				customBirthdayIndicatorTexture = _helper.ModContent.Load<Texture2D>("Customization/birthdayIndicator.png");
				customBirthdayIndicatorBounds = customBirthdayIndicatorTexture.Bounds;
				_monitor.Log($"Custom birthday indicator exists", LogLevel.Debug);
			} catch {
				_monitor.Log($"Custom birthday indicator does not exist", LogLevel.Info);
			}
		}

		public void ToggleMod(bool isOn) {
			_helper.Events.Player.Warped -= OnWarped;
			_helper.Events.Display.RenderedWorld -= OnRenderedWorld_DrawNPCHasChat;
			_helper.Events.GameLoop.UpdateTicked -= UpdateTicked;

			if (isOn) {
				_helper.Events.Player.Warped += OnWarped;
				_helper.Events.Display.RenderedWorld += OnRenderedWorld_DrawNPCHasChat;
				_helper.Events.GameLoop.UpdateTicked += UpdateTicked;
			}
		}

		/// <summary>Raised before drawing the world</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnRenderedWorld_DrawNPCHasChat(object? sender, RenderedWorldEventArgs e) {
			if (Game1.currentLocation == null) return;

			if (Game1.activeClickableMenu != null && !_config.showIndicatorsWhenMenuIsOpen)
				return;

			// draws the static icon
			foreach (var npc in GetNPCsInCurrentLocation()) {
				if (_config.enableDebugOutput && _pauseTicks.Value % 60 == 0) {
					_monitor.Log($"Checking if {Game1.player.Name} can chat with {npc.Name}: {ShouldShowChatIndicatorFor(npc)}", LogLevel.Debug);
				}

				if (ShouldShowBirthdayIndicatorFor(npc)) {
					DrawIndicator(npc, IndicatorTexture(true), IndicatorBounds(true));
				} else if (ShouldShowChatIndicatorFor(npc)) {
					DrawIndicator(npc, IndicatorTexture(false), IndicatorBounds(false));
				}
			}
		}

		private void DrawIndicator(NPC npc, Texture2D texture, Rectangle bounds) {
			float scale = _config.indicatorScale;
			var position = GetChatPositionAboveNPC(npc);
			if (!_config.disableIndicatorBob)
				position.Y += (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 300.0 + npc.Name.GetHashCode()) * 5.0);

			// NPC.draw instead???
			Game1.spriteBatch.Draw(
				texture,
				position,
				bounds,
				indicatorColor,
				indicatorRotation,
				indicatorOrigin,
				scale,
				indicatorSpriteEffects,
				indicatorLayerDepth);
		}

		private Texture2D IndicatorTexture(bool isBirthday) {
			var tuple = (isBirthday, _config.useCustomIndicatorImage, _config.useCustomBirthdayIndicatorImage);

			return tuple switch {
				(true, _, true) => customBirthdayIndicatorTexture ?? defaultBirthdayIndicatorTexture,
				(true, _, false) => defaultBirthdayIndicatorTexture,
				(false, true, _) => customIndicatorTexture ?? defaultIndicatorTexture,
				(false, false, _) => defaultIndicatorTexture
			};
		}

		private Rectangle IndicatorBounds(bool isBirthday) {
			var tuple = (isBirthday, _config.useCustomIndicatorImage, _config.useCustomBirthdayIndicatorImage);

			return tuple switch {
				(true, _, true) => customBirthdayIndicatorBounds ?? defaultBirthdayIndicatorBounds,
				(true, _, false) => defaultBirthdayIndicatorBounds,
				(false, true, _) => customIndicatorBounds ?? defaultIndicatorBounds,
				(false, false, _) => defaultIndicatorBounds
			};
		}

		private Vector2 GetChatPositionAboveNPC(NPC npc) {
			Vector2 position = npc.getLocalPosition(Game1.viewport);
			float scaleAdjustmentX = (_config.indicatorScale * -8) + 32;
			float scaleAdjustmentY = (_config.indicatorScale * -16) - 68;

			// Character sprite height/width may be future idea for more accurate offsets

			if (_config.useDebugOffsetsForAllNPCs) {
				position.X += scaleAdjustmentX;
				position.Y += _config.debugIndicatorYOffset;
			} else if (_npcOffsets.TryGetValue(npc.Name, out int offset)) {
				position.X += scaleAdjustmentX;
				position.Y += scaleAdjustmentY + offset;
			} else if (npc is Child) {
				position.X += scaleAdjustmentX;
				position.Y += scaleAdjustmentY + 39;
			} else {
				// only looks good at scale == 2
				position.X += 16;
				position.Y += -100;
			}
			return position;
		}

		/// <summary>Get a list of all NPCs in the current location</summary>
		private NetCollection<NPC> GetNPCsInCurrentLocation() {
			NetCollection<NPC> npcs;

			if (IsCutscene()) {
				npcs = _config.showIndicatorsDuringCutscenes ? new NetCollection<NPC>(Game1.CurrentEvent.actors) : new();
			} else if (Game1.isFestival()) {
				npcs = new NetCollection<NPC>(Game1.CurrentEvent.actors);
			} else {
				npcs = new NetCollection<NPC>(Game1.currentLocation.characters);
			}

			npcs.Filter(c => {
				// At the grange festival (fall 16), the animals are classified as villagers for some reason
				// They cannot be talked to, so they are removed from the list
				if (nonInteractableVillagers.Contains(c.Name))
					return false;

				return c.isVillager();
			});
			return npcs;
		}

		private static bool IsCutscene() {
			return Game1.CurrentEvent != null && !Game1.isFestival();
		}

		/// <summary>Whether the indicator for the given npc should be shown</summary>
		private bool ShouldShowChatIndicatorFor(NPC npc) {
			// if npc is sleeping or they cannot socialize, then they can't chat with us
			if (npc.isSleeping.Value || !npc.CanSocialize) {
				return false;
			}

			// check friendship values with npc
			if (Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship friendshipValues)) {
				int maxHeartPoints = Utility.GetMaximumHeartsForCharacter(npc) * 250;
				if (_config.disableIndicatorsForMaxHearts && friendshipValues.Points >= maxHeartPoints) {
					// if player has reached max hearts with npc, they don't need to talk to them
					return false;
				}

				// At this point, player has not reached max hearts with npc
				// Return if player has talked to npc today
				return !friendshipValues.TalkedToToday;
			} else {
				// if friendship data doesn't exist for that npc (aka meeting them for the first time),
				// then of course you can talk to them
				return true;
			}
		}

		private bool ShouldShowBirthdayIndicatorFor(NPC npc) {
			// if the config is disabled OR it's not their birthday, then return false
			if (!_config.showBirthdayIndicator || !npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
				return false;

			// We know it's their birthday, now we need to determine if they can receive a gift
			// They can receive a gift on their birthday, regardless of how many gifts they've been given this week
			// So all we need to do is check whether they're received a gift today

			// check friendship values with npc
			if (Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship friendshipValues))
				// if npc has been met before, check gifts given today
				return friendshipValues.GiftsToday == 0;
			else
				// npc hasn't been met today, so we can give them a gift
				return true;
		}

		/// <summary>Raised after a player warps to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnWarped(object? sender, WarpedEventArgs e) {
			if (e.IsLocalPlayer) {
				_pauseTicks.Value = 60;
			}
		}

		/// <summary>Raised after the game state is updated (≈60 times per second).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void UpdateTicked(object? sender, UpdateTickedEventArgs e) {
			if (Game1.eventUp || Game1.activeClickableMenu != null)
				return;

			--_pauseTicks.Value;

			// update chat draw. If _pauseTicks is positive then don't render.
			if (e.IsMultipleOf(2) && _pauseTicks.Value <= 0) {
				_yMovementPerDraw.Value += 0.3f;
				_alpha.Value -= 0.014f;
				if (_alpha.Value < 0.1f) {
					_pauseTicks.Value = 60;
					_yMovementPerDraw.Value = -3f;
					_alpha.Value = 1f;
				}
			}
		}

		public void Dispose() {
			ToggleMod(false);
		}
	}
}
