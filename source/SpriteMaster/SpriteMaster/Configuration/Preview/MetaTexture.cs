/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpriteMaster.Configuration.Preview;

abstract class MetaTexture : IDisposable {
	internal readonly Texture2D Texture;

	protected MetaTexture(Texture2D? texture) {
		if (texture is null) {
			throw new NullReferenceException(nameof(texture));
		}
		Texture = texture;
	}

	protected MetaTexture(string textureName) : this(StardewValley.Game1.content.Load<Texture2D>(textureName)) { }

	~MetaTexture() {
		Dispose(false);
	}

	internal void Dispose(bool disposing) {
		if (disposing) {
			Texture?.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public void Dispose() {
		Dispose(true);
	}
}
