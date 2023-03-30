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
using StardewValley;
using System;

namespace AeroCore.Particles
{
    public class SimpleBehavior : IParticleBehavior
    {
        public float Direction { get; set; } = 0f;
        public float DirectionVariance { get; set; } = 0f;
        public int MinSpeed { get; set; } = 16;
        public int MaxSpeed { get; set; } = 16;
        public int MinLife { get; set; } = 10;
        public int MaxLife { get; set; } = 10;
        public float MinCurve { get; set; } = 0f;
        public float MaxCurve { get; set; } = 0f;
        public float MinAcceleration { get; set; } = 0f;
        public float MaxAcceleration { get; set; } = 0f;

        private float[] speed;
        private float[] dir;
        private double[] curve;
        private float[] acceleration;

        public void Init(int count)
        {
            speed = new float[count];
            curve = new double[count];
            acceleration = new float[count];
            dir = new float[count];
        }

        public void Cleanup()
        {
            speed.Clear();
            curve.Clear();
            acceleration.Clear();
        }

        public void Startup(){
            //noop
        }

        public void Tick(ref Vector2[] positions, ref int[] life, ref int[] maxLife, int millis)
        {
            for(int i = 0; i < life.Length; i++)
            {
                int vtime = millis;
                if (life[i] == 0)
                    continue;
                int clife = life[i];
                if (clife < 0) //just spawned
                {
                    if (maxLife[i] == 0)
                    {
                        var max = Math.Max(MinSpeed, MaxSpeed);
                        speed[i] = Game1.random.Next(MinSpeed, max) / 1000f;
                        dir[i] = Data.DegToRad((float)Game1.random.NextDouble() * DirectionVariance * 2f - DirectionVariance + Direction);
                        max = Math.Max(MinLife, MaxLife);
                        maxLife[i] = Math.Max(1, Game1.random.Next(MinLife, max) * 1000);
                        var maxf = MathF.Max(MinCurve, MaxCurve);
                        curve[i] = Data.DegToRad(Game1.random.NextDouble() * (maxf - MinCurve) + MinCurve) / 1000f;
                        maxf = MathF.Max(MinAcceleration, MaxAcceleration);
                        acceleration[i] = (float)Game1.random.NextDouble() * (maxf - MinAcceleration) + MinAcceleration;
                        clife = -clife;
                    } else
                    {
                        clife = millis - clife;
                        vtime = clife;
                        life[i] = clife;
                    }
                }
                else
                {
                    life[i] += millis;
                    clife += millis;
                }
                if (clife >= maxLife[i]) //dying
                {
                    life[i] = 0;
                    maxLife[i] = 0;
                }
                else //moving
                {
                    var delt = speed[i] * vtime;
                    var pos = positions[i];
					positions[i] = new(pos.X + MathF.Cos(dir[i]) * delt, pos.Y + MathF.Sin(dir[i]) * delt);
                    dir[i] += (float)(curve[i] * vtime);
                    speed[i] += acceleration[i] * vtime;
                }
            }
        }
    }
}
