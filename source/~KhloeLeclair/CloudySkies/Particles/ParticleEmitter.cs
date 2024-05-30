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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.CloudySkies.Particles;


public class ParticleEmitter : IDisposable {

	public List<Particle> Particles { get; private set; }

	public Vector2 Position { get; set; }

	public Texture2D Texture { get; private set; }

	#region Life Cycle

	private bool isDisposed;

	public ParticleEmitter(Texture2D texture, int capacity) {
		Texture = texture;
		Particles = new List<Particle>(capacity);
	}

	protected virtual void Dispose(bool disposing) {
		if (!isDisposed) {
			if (disposing) {
				// TODO: dispose managed state (managed objects)
			}

			Texture = null!;
			Particles = null!;

			isDisposed = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~ParticleEmitter()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion


	public bool Update(GameTime time) {

		return false;

	}


}
