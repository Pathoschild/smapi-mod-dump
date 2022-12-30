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
using StardewValley;
using System;

namespace AeroCore.Particles
{
    public class Emitter : IParticleEmitter
    {
        public Rectangle Region { get; set; } = new();
        public int Rate
        {
            get => rate;
            set => rate = Math.Max(value, 1);
        }
        private int rate = 100;
        public int RateVariance
        {
            get => rateVariance;
            set => rateVariance = Math.Max(value, 0);
        }
        private int rateVariance = 0;
        public int BurstMin
        {
            get => burstMin;
            set => burstMin = Math.Max(value, 1);
        }
        private int burstMin = 1;
        public int BurstMax
        {
            get => burstMax;
            set => burstMax = Math.Max(value, burstMin);
        }
        private int burstMax = 1;
        public bool Radial { get; set; } = false;

        private int timeSinceLast;
        private int variance;

        private int lastIndex = 0;

        public Emitter()
        {
            Reset();
        }
        public Emitter(IParticleEmitter from) : this()
        {
            Region = from.Region;
            Rate = from.Rate;
            RateVariance = from.RateVariance;
            BurstMin = from.BurstMin;
            BurstMax = from.BurstMax;
            Radial = from.Radial;
        }

        public void Reset()
        {
            timeSinceLast = 0;
            variance = Game1.random.Next(rateVariance * 2) - rateVariance;
        }

        public void Tick(ref Vector2[] position, ref int[] life, int millis)
        {
            timeSinceLast += millis;
            while (timeSinceLast >= rate + variance)
            {
                int pindex = lastIndex;
                int start = lastIndex;
                timeSinceLast -= rate + variance;
                int count = Game1.random.Next(burstMax - burstMin) + burstMin;
                while (count > 0 && pindex - start < life.Length)
                {
                    count--;
                    while (pindex - start < life.Length && life[pindex % life.Length] != 0)
                        pindex++;
                    if (pindex - start < life.Length)
                    {
                        Emit(ref position, ref life, pindex % life.Length, timeSinceLast + 1);
                        lastIndex = pindex;
                    }
                }
                variance = Game1.random.Next(rateVariance * 2) - rateVariance;
            }
        }
        private void Emit(ref Vector2[] pos, ref int[] life, int index, int millis)
        {
            if (Radial)
            {
                float dir = (float)Game1.random.NextDouble() * MathF.PI * 2f;
                float dist = (float)Game1.random.NextDouble();
                pos[index] = new Vector2(MathF.Cos(dir) * dist * Region.Width, MathF.Sin(dir) * dist * Region.Height);
            } else
            {
                pos[index] = new(Game1.random.Next(Region.Width) + Region.X, Game1.random.Next(Region.Height) + Region.Y);
            }
            life[index] = -millis;
        }
    }
}
