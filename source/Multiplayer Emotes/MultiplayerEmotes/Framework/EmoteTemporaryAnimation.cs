
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace MultiplayerEmotes.Framework {

	/// <summary>
	/// TEST - Animation trough sprite broadcast.
	/// Issue: Sprite position does not change for other players.
	/// </summary>
	public class EmoteTemporaryAnimation {

		private readonly IReflectionHelper Reflection;
		List<TemporaryAnimatedSprite> temporaryAnimationList = new List<TemporaryAnimatedSprite>();

		public EmoteTemporaryAnimation(IReflectionHelper reflectionHelper, IModEvents events) {
			Reflection = reflectionHelper;
			events.Display.Rendered += OnRendered;
		}

		/// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnRendered(object sender, RenderedEventArgs e) {
			foreach(TemporaryAnimatedSprite animatedSprite in temporaryAnimationList) {
				//animatedSprite.update(Game1.currentGameTime);
				animatedSprite.Position = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160);
			}
		}

		public void BroadcastEmote(int whichEmote) {

			TemporaryAnimatedSprite emoteStart = new TemporaryAnimatedSprite("TileSheets\\emotes", new Rectangle(0, 0, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				interval = 80f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 0.9f,
				local = false,
				timeBasedMotion = true,
				attachedCharacter = Game1.player,
				extraInfoForEndBehavior = 0,
				endFunction = FinishedAnimation
			};

			TemporaryAnimatedSprite emote = new TemporaryAnimatedSprite("TileSheets\\emotes", new Rectangle(0, whichEmote * 16, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				delayBeforeAnimationStart = 100,
				interval = 250f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 1f,
				local = false,
				timeBasedMotion = true,
				attachedCharacter = Game1.player,
				extraInfoForEndBehavior = 1,
				endFunction = FinishedAnimation
			};

			TemporaryAnimatedSprite emoteEnding = new TemporaryAnimatedSprite("TileSheets\\emotes", new Rectangle(0, 0, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				delayBeforeAnimationStart = 800,
				interval = 80f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 0.9f,
				local = false,
				timeBasedMotion = true,
				pingPong = true,                            // This makes the animation play, forwards, and when finished backwards
				sourceRect = new Rectangle(48, 0, 16, 16),  // Set the current sprite position to the last animation image
				currentParentTileIndex = 3,                 // To play the animation backwards, we tell it that its in the last frame
				attachedCharacter = Game1.player,
				extraInfoForEndBehavior = 2,
				endFunction = FinishedAnimation
			};

			temporaryAnimationList = new List<TemporaryAnimatedSprite>() {
				emoteStart,
				emote,
				emoteEnding
			};

			Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.broadcastSprites(Game1.player.currentLocation, temporaryAnimationList);

		}

		private void FinishedAnimation(int extraInfo) {
			switch(extraInfo) {
				case 0:
					ModEntry.ModMonitor.Log($"Finished animation 0!");
					break;
				case 1:
					ModEntry.ModMonitor.Log($"Finished animation 1!");
					break;
				case 2:
					ModEntry.ModMonitor.Log($"Finished animation 2!");
					break;
			}
			temporaryAnimationList[extraInfo].delayBeforeAnimationStart = 0;
		}
	}

}
