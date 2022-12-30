/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace AeroCore.Particles
{
    public class Manager : IParticleManager
    {
        public Rectangle? ClipRegion { get; set; } = null;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = new(1f, 1f);
        public float Depth { get; set; } = 0f;
        public IParticleEmitter Emitter => emitter;

        private int[] life;
        private int[] maxLife;
        private Vector2[] positions;
        private readonly IParticleBehavior behavior;
        private readonly IParticleSkin skin;
        private readonly Emitter emitter;
        private bool isSetup = false;

        public Manager(int count, IParticleBehavior behavior, IParticleSkin skin, IParticleEmitter emitter)
        {
            this.behavior = behavior;
            life = new int[count];
            positions = new Vector2[count];
            maxLife = new int[count];
            this.skin = skin;
            this.emitter = emitter is Emitter e ? e : new(emitter);
            this.behavior.Init(count);
            this.skin.Init(count);
        }

        public void Cleanup()
        {
            skin.Cleanup();
            behavior.Cleanup();
            isSetup = false;
            life.Clear();
            positions.Clear();
            maxLife.Clear();
        }

        public void Draw(SpriteBatch batch)
        {
            if(isSetup)
                if(ClipRegion is null || Game1.viewport.ToRect().Intersects((Rectangle)ClipRegion))
                    skin.Draw(batch, positions, life, maxLife, Scale, Offset, Depth);
            else
                ModEntry.monitor.LogOnce($"Particle Emitter was not ticked before attempting to draw!", LogLevel.Warn);
        }

        public void Tick(int millis)
        {
            if (!isSetup)
                Setup();
            for (int i = 0; i < positions.Length; i++)
                if (life[i] == 1)
                    life[i] = millis;
            emitter.Tick(ref positions, ref life, millis);
            behavior.Tick(ref positions, ref life, ref maxLife, millis);
        }
        private void Setup()
        {
            emitter.Reset();
            behavior.Startup();
            skin.Startup();
            isSetup = true;
        }
    }
}
