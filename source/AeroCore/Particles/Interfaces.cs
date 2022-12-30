/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AeroCore.Particles
{
    public interface IParticleBehavior
    {
        public void Tick(ref Vector2[] positions, ref int[] life, ref int[] maxLife, int millis);
        public void Cleanup();
        public void Startup();
        public void Init(int count);
    }
    public interface IParticleSkin
    {
        public void Draw(SpriteBatch batch, Vector2[] positions, int[] life, int[] maxLife, Vector2 scale, Vector2 offset = new(), float depth = 0f);
        public void Cleanup();
        public void Startup();
        public void Init(int count);
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
}
