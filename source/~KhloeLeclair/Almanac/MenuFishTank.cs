/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Leclair.Stardew.Almanac;

public class MenuFishTank : FishTankFurniture {

	public static readonly Rectangle BG_SOURCE = new(16, 272, 28, 28);
	public static readonly Rectangle BUBBLE_SOURCE = new(0, 240, 16, 16);

	public Texture2D BackgroundTexture;
	public Rectangle BackgroundSource;
	public Color BackgroundColor;

	public Texture2D? FloorTexture;
	public Rectangle? FloorSource;
	public Color? FloorColor;

	public Texture2D? GlassTexture;
	public Rectangle? GlassSource;
	public Color? GlassColor;

	public Texture2D? FrameTexture;
	public Rectangle? FrameSource;
	public Color? FrameColor;

	private Rectangle _Bounds;
	private Rectangle _Inside;

	public MenuFishTank(Rectangle bounds)
	: base("(F)2304", Vector2.Zero) {

		_Bounds = bounds;
		_Inside = new Rectangle(
			_Bounds.X + 4,
			_Bounds.Y + 4,
			_Bounds.Width - 8,
			_Bounds.Height - 8
		);

		BackgroundTexture = Game1.uncoloredMenuTexture;
		BackgroundSource = new Rectangle(16, 272, 28, 28);
		BackgroundColor = new Color(0x29, 0x6D, 0x96);
	}

	public Rectangle InsideBounds => _Inside;

	public Rectangle Bounds {
		get => _Bounds;
		set {
			_Bounds = value;
			_Inside = new Rectangle(
				_Bounds.X + 4,
				_Bounds.Y + 4,
				_Bounds.Width - 8,
				_Bounds.Height - 8
			);

			foreach (TankFish fish in tankFish)
				fish.ConstrainToTank();
		}
	}

	public override int getTilesWide() {
		return _Bounds.Width / 64;
	}

	public override int getTilesHigh() {
		return _Bounds.Height / 64;
	}

	public override Rectangle GetTankBounds() {
		return new Rectangle(
			_Inside.X + Game1.viewport.X,
			_Inside.Y + Game1.viewport.Y,
			_Inside.Width,
			_Inside.Height
		);
	}

	public override void draw(SpriteBatch batch, int x, int y, float alpha = 1) {
		if (x != _Bounds.X || y != _Bounds.Y)
			Bounds = new Rectangle(
				x, y,
				_Bounds.Width, _Bounds.Height
			);

		draw(batch, alpha);
	}

	public void draw(SpriteBatch batch, float alpha = 1) {
		if (isTemporarilyInvisible)
			return;

		// Update our fish
		updateWhenCurrentLocation(Game1.currentGameTime);

		RenderHelper.WithScissor(batch, SpriteSortMode.FrontToBack, _Inside, () => {

			// Draw the background
			if (BackgroundTexture != null)
				batch.Draw(
					BackgroundTexture,
					_Inside,
					BackgroundSource,
					BackgroundColor * alpha
				);

			// Draw the floor
			if (FloorTexture != null)
				for (int ix = 0; ix < _Bounds.Width; ix += 64) {
					batch.Draw(
						FloorTexture,
						new Vector2(
							_Bounds.Left + ix,
							_Bounds.Bottom - 64
						),
						FloorSource ?? FloorTexture.Bounds,
						(FloorColor ?? Color.White) * alpha,
						0f,
						Vector2.Zero,
						4f,
						((ix / 64) % 3) == 0 ?
							SpriteEffects.None : SpriteEffects.FlipHorizontally,
						0.001f
					);
				}

			// Draw the fish
			Vector2 fishSort = GetFishSortRegion();

			int hatted = 0;

			for (int i = 0; i < tankFish.Count; i++) {
				TankFish fish = tankFish[i];
				float layer = Utility.Lerp(
					fishSort.Y,
					fishSort.X,
					fish.zPosition / 20f
				) + 1E-07f * i;

				fish.Draw(batch, alpha, layer);

				// TODO: Hat support?
				if (fish.fishIndex == 86) {
					int hat = 0;
					foreach (var item in heldItems) {
						if (item is Hat) {
							if (hatted == hat) {
								item.drawInMenu(
									batch,
									Game1.GlobalToLocal(fish.GetWorldPosition()) + new Vector2(
										(fish.facingLeft ? -4 : 0) - 30f,
										-55f
									),
									0.75f,
									1f,
									layer + 1E-08f,
									StackDrawType.Hide
								);
								hatted++;
								break;
							}
							hat++;
						}
					}
				}
			}

			// Draw the decorations
			Texture2D tex = GetAquariumTexture();

			foreach (var _deco in floorDecorations) {
				if (!_deco.HasValue) continue;
				var deco = _deco.Value;

				Rectangle source = deco.Key;
				Vector2 pos = deco.Value;

				float layer = Utility.Lerp(
					fishSort.Y, fishSort.X,
					pos.Y / 20f
				) - 1E-06f;

				batch.Draw(
					texture: tex,
					position: new Vector2(
						_Inside.Left + pos.X * 4f,
						_Inside.Bottom - 4 - pos.Y * 4f
					),
					sourceRectangle: source,
					color: Color.White * alpha,
					rotation: 0f,
					origin: new Vector2(
						source.Width / 2,
						source.Height / 2
					),
					scale: 4f,
					effects: SpriteEffects.None,
					layerDepth: layer
				);
			}

			// Draw the bubbles
			foreach (Vector4 bubble in bubbles) {
				float layer = Utility.Lerp(
					fishSort.Y, fishSort.X,
					bubble.Z / 20f
				) - 1E-06f;

				batch.Draw(
					texture: tex,
					position: new Vector2(
						_Inside.Left + bubble.X,
						_Inside.Bottom - 4 - bubble.Y - bubble.Z * 4f
					),
					sourceRectangle: BUBBLE_SOURCE,
					color: Color.White * alpha,
					rotation: 0f,
					origin: new Vector2(8f, 8f),
					scale: 4f,
					effects: SpriteEffects.None,
					layerDepth: layer
				);
			}

			// Draw the glass
			if (GlassTexture != null)
				batch.Draw(
					texture: GlassTexture,
					destinationRectangle: _Inside,
					sourceRectangle: GlassSource ?? GlassTexture.Bounds,
					color: (GlassColor ?? Color.White) * alpha,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f
				);

			// Tear down our graphics scissor.

		});

		// And finally, draw the top texture.
		if (FrameTexture != null)
			RenderHelper.DrawBox(
				batch,
				texture: FrameTexture,
				sourceRect: FrameSource ?? FrameTexture.Bounds,
				_Bounds.X, _Bounds.Y,
				_Bounds.Width, _Bounds.Height,
				color: (FrameColor ?? Color.White) * alpha,
				scale: 4f,
				drawShadow: false
			);
	}

}
