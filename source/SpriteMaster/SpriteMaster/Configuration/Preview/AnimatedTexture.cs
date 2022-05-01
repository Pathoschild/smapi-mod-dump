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

class AnimatedTexture : MetaTexture {
	internal readonly Vector2I Size;
	private readonly Vector2I[] SpriteOffsets;
	private readonly int TicksPerFrame;
	private uint CurrentTick = 0;

	internal Bounds Current {
		get {
			int maxTicks = SpriteOffsets.Length * TicksPerFrame;
			uint normalizedTick = CurrentTick % (uint)maxTicks;
			int offset = (int)normalizedTick / TicksPerFrame;
			return new Bounds(SpriteOffsets[offset], Size);
		}
	}

	private AnimatedTexture(
		XTexture2D texture,
		Vector2I size,
		Vector2I[] spriteOffsets,
		int ticksPerFrame,
		uint currentTick
	) : base(texture) {
		Size = size;
		SpriteOffsets = spriteOffsets;
		TicksPerFrame = ticksPerFrame;
		CurrentTick = currentTick;
	}

	internal AnimatedTexture(
		string textureName,
		Vector2I spriteSize,
		Vector2I spriteOffset,
		int spritesPerRow,
		int numSprites,
		int ticksPerFrame
	) : base(textureName) {
		Size = spriteSize;
		TicksPerFrame = ticksPerFrame;

		SpriteOffsets = new Vector2I[numSprites];
		for (int i = 0; i < numSprites; i++) {
			Vector2I offset = (i % spritesPerRow, i / spritesPerRow);
			SpriteOffsets[i] = spriteOffset + (spriteSize * offset);
		}
	}

	protected AnimatedTexture(
		string textureName,
		Vector2I spriteSize,
		Vector2I spriteOffset,
		int spritesPerRow,
		Vector2I[] spriteIndices,
		int ticksPerFrame
	) : base(textureName) {
		Size = spriteSize;
		TicksPerFrame = ticksPerFrame;

		int numSprites = spriteIndices.Length;

		SpriteOffsets = new Vector2I[numSprites];
		for (int i = 0; i < numSprites; i++) {
			SpriteOffsets[i] = spriteOffset + (spriteSize * spriteIndices[i]);
		}
	}

	internal void Tick() => ++CurrentTick;

	internal AnimatedTexture Clone(Random? rand = null) {
		var result = new AnimatedTexture(
			Texture,
			Size,
			SpriteOffsets,
			TicksPerFrame,
			(rand is not null) ? (uint)rand.Next(SpriteOffsets.Length * TicksPerFrame) : CurrentTick
		);

		return result;
	}
}

class UniformAnimatedTexture : AnimatedTexture {
	internal UniformAnimatedTexture(
		string textureName,
		Vector2I spriteSize,
		Vector2I spriteOffset,
		int spritesPerRow,
		int numSprites,
		int ticksPerFrame
	) : base(
		textureName,
		spriteSize,
		spriteOffset * spriteSize,
		spritesPerRow,
		numSprites,
		ticksPerFrame
	){
	}

	protected UniformAnimatedTexture(
		string textureName,
		Vector2I spriteSize,
		Vector2I spriteOffset,
		int spritesPerRow,
		Vector2I[] spriteIndices,
		int ticksPerFrame
	) : base(
		textureName,
		spriteSize,
		spriteOffset * spriteSize,
		spritesPerRow,
		spriteIndices,
		ticksPerFrame
	) {
	}
}

class UniformExplicitAnimatedTexture : UniformAnimatedTexture {
	internal UniformExplicitAnimatedTexture(
		string textureName,
		Vector2I spriteSize,
		Vector2I spriteOffset,
		int spritesPerRow,
		Vector2I[] spriteIndices,
		int ticksPerFrame
	) : base(
		textureName,
		spriteSize,
		spriteOffset,
		spritesPerRow,
		spriteIndices,
		ticksPerFrame
	) {
	}
}
