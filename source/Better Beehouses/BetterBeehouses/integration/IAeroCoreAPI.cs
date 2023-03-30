/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace BetterBeehouses.integration
{
	public interface IAeroCoreAPI
	{
		public event Action<SpriteBatch> OnDrawingWorld;

		/// <summary>Creates a new particle system from a mod data asset</summary>
		public IParticleManager CreateParticleSystem(IModContentHelper helper, string path, IParticleEmitter emitter, int count);
	}

	public interface IParticleManager
	{
		public Rectangle? ClipRegion { get; set; }
		public Vector2 Offset { get; set; }
		public Vector2 Scale { get; set; }
		public float Depth { get; set; }
		public IParticleEmitter Emitter { get; }

		public void Draw(SpriteBatch batch);
		public void Tick(int millis);
		public void Cleanup();
	}
	public interface IParticleEmitter
	{
		public Rectangle Region { get; set; }
		public int Rate { get; set; }
		public int RateVariance { get; set; }
		public int BurstMin { get; set; }
		public int BurstMax { get; set; }
		public bool Radial { get; set; }
	}
	public class ParticleEmitter : IParticleEmitter
	{
		public Rectangle Region { get; set; }
		public int Rate { get; set; }
		public int RateVariance { get; set; }
		public int BurstMin { get; set; }
		public int BurstMax { get; set; }
		public bool Radial { get; set; }
	}
}
