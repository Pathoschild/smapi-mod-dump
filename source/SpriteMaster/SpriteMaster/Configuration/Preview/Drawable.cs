/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;

namespace SpriteMaster.Configuration.Preview;

readonly struct Drawable {
	private readonly XTexture2D? Texture = null;
	private readonly Bounds? Source = null;
	private readonly AnimatedTexture? AnimatedTexture = null;
	private readonly float Rotation;
	private readonly int Offset;

	internal readonly int Width => Source?.Width ?? Texture?.Width ?? AnimatedTexture?.Size.Width ?? 0;
	internal readonly int Height => Source?.Height ?? Texture?.Height ?? AnimatedTexture?.Size.Height ?? 0;

	private static float DegreesToRadians(int degrees) => (MathF.PI / 180.0f) * degrees;

	private Drawable(XTexture2D? texture, in Bounds? source, AnimatedTexture? animatedTexture, float rotation, int offset) {
		Texture = texture;
		Source = source;
		AnimatedTexture = animatedTexture;
		Rotation = Math.Clamp(rotation, 0.0f, MathF.PI * 2.0f);
		Offset = offset;
	}

	internal Drawable(XTexture2D texture, in Bounds? source = null, float rotation = 0.0f, int offset = 0) {
		Texture = texture;
		Source = source;
		Rotation = Math.Clamp(rotation, 0.0f, MathF.PI * 2.0f);
		Offset = offset;
	}

	internal Drawable(XTexture2D texture, in Bounds? source, int rotationDegrees) : this(texture, source, DegreesToRadians(rotationDegrees)) { }

	internal Drawable(AnimatedTexture texture, float rotation = 0.0f, int offset = 0) {
		AnimatedTexture = texture;
		Rotation = Math.Clamp(rotation, 0.0f, MathF.PI * 2.0f);
		Offset = offset;
	}

	internal Drawable(AnimatedTexture texture, int rotationDegrees) : this(texture, DegreesToRadians(rotationDegrees)) { }

	public static implicit operator Drawable(AnimatedTexture animatedTexture) => new(animatedTexture);

	internal readonly void Tick() {
		AnimatedTexture?.Tick();
	}

	internal readonly Drawable Rotate(float rotation) => new(
		Texture,
		Source,
		AnimatedTexture,
		Rotation + rotation,
		Offset
	);

	internal readonly Drawable Rotate(int rotationDegrees) => new(
		Texture,
		Source,
		AnimatedTexture,
		Rotation + DegreesToRadians(rotationDegrees),
		Offset
	);

	public static bool operator ==(in Drawable left, in Drawable right) =>
		(left.Texture == right.Texture) &&
		(left.Source == right.Source) &&
		(left.AnimatedTexture == right.AnimatedTexture) &&
		(left.Rotation == right.Rotation);

	public static bool operator !=(in Drawable left, in Drawable right) => !(left == right);

	public override readonly bool Equals(object? obj) {
		if (obj is Drawable drawable) {
			return this == drawable;
		}
		return false;
	}

	public override readonly int GetHashCode() => Hashing.Combine32(
		Texture?.GetHashCode(),
		Source?.GetHashCode(),
		AnimatedTexture?.GetHashCode(),
		Rotation.GetHashCode()
	);

	internal readonly void Draw(Scene scene, XNA.Graphics.SpriteBatch batch, Vector2I location, float layerDepth = 0.0f) {
		/*
		int spriteHeight = Height / 16;
		if (spriteHeight > 1 && (spriteHeight & 1) == 1) {
			// If the height is even, it needs to be offset.
			location.Y -= 32;
		}
		*/
		location.Y += Offset;

		if (Texture is not null) {
			scene.DrawAt(
				batch,
				Texture,
				location,
				Source,
				rotation: Rotation,
				layerDepth: layerDepth
			);
		}
		else if (AnimatedTexture is not null) {
			scene.DrawAt(
				batch,
				AnimatedTexture,
				location,
				rotation: Rotation,
				layerDepth: layerDepth
			);
		}
	}

	internal readonly Drawable Clone(Random? rand = null) => new(
		Texture,
		Source,
		AnimatedTexture?.Clone(rand),
		Rotation,
		Offset
	);
}

readonly struct DrawableInstance {
	private readonly Drawable Drawable;
	private readonly Vector2I Location;
	internal readonly int LayerDepth;

	internal DrawableInstance(in Drawable drawable, Vector2I location) {
		Drawable = drawable;
		Location = location;

		int height = (Drawable.Height / 2) / 16;

		if (height != 0) {
			height *= 64;
			height++;
		}

		LayerDepth = Location.Y + height;
	}

	internal readonly void Tick() {
		Drawable.Tick();
	}

	internal readonly void Draw(Scene scene, XNA.Graphics.SpriteBatch batch, int index) {
		float layerDepth = (LayerDepth + scene.Region.Height + (index * 0.0001f)) / (scene.Region.Height * 4.0f);

		Drawable.Draw(
			scene: scene,
			batch: batch,
			location: Location,
			layerDepth: layerDepth
		);
	}

	internal readonly void Draw(Scene scene, XNA.Graphics.SpriteBatch batch, Vector2I offset, int index) {
		float layerDepth = (LayerDepth + scene.Region.Height + offset.Y + (index * 0.0001f)) / (scene.Region.Height * 4.0f);

		Drawable.Draw(
			scene: scene,
			batch: batch,
			location: Location + offset,
			layerDepth: layerDepth
		);
	}

	internal readonly Vector2B IsOffscreenStart(Scene scene) {
		Vector2I min = Location - (new Vector2I(Drawable.Width, Drawable.Height) * 4);

		return new(
			min.X < 0,
			min.Y < 0
		);
	}

	internal readonly Vector2B IsOffscreenEnd(Scene scene) {
		Vector2I max = Location + (new Vector2I(Drawable.Width, Drawable.Height) * 4);

		return new(
			max.Width > scene.Region.Width,
			max.Height > scene.Region.Height
		);
	}
}
