/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Characters;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    /// <summary>
    ///   A Junimo class just for the crop event.
    /// </summary>
    public class EventJunimo : Junimo
    {
        // Random bright colors.
        private static readonly Color[] colors = new Color[] { Color.AliceBlue, Color.Chartreuse, Color.Cornsilk, Color.DarkSeaGreen, Color.ForestGreen, Color.Fuchsia, Color.OrangeRed, Color.HotPink, Color.Violet };

        private Vector2 starting;
        private Vector2 targetVector;
        private double? firstUpdateTime;
        private double startDelay;
        private const double timeToGetToTarget = 1000;
        private const double maxStartingDelay = 1000;

        public EventJunimo(Vector2 starting, Vector2 targetVector)
            : this(starting, targetVector, colors[Game1.random.Next(colors.Length)])
        {
        }

        public EventJunimo(Vector2 starting, Vector2 targetVector, Color color)
            : base(starting * 64f + new Vector2(0, 5), -1, temporary: true)
        {
            this.SetColor(color);

            this.starting = this.Position;
            this.startDelay = new Random().NextDouble() * maxStartingDelay;
            this.targetVector = targetVector * 64f;
        }

        private void SetColor(Color color)
        {
            // No need for try/catch here really - if this throws due to a code change, it'll break the event,
            // but that won't block progress and it'll leave a very clear trail of destruction in the log file.
            var colorField = typeof(Junimo).GetField(I("color"), BindingFlags.NonPublic | BindingFlags.Instance);
            ((NetColor)colorField!.GetValue(this)!).Value = color;
        }

        public override void update(GameTime gameTime, GameLocation location)
        {
            if (this.firstUpdateTime is null)
            {
                this.firstUpdateTime = gameTime.TotalGameTime.TotalMilliseconds;
            }
            else
            {
                double msSinceStart = gameTime.TotalGameTime.TotalMilliseconds - this.firstUpdateTime.Value - this.startDelay;
                float progressAsFraction = (float)Math.Max(0, Math.Min(1, msSinceStart/timeToGetToTarget));
                this.Position = this.starting + this.targetVector * progressAsFraction;
            }

            base.update(gameTime, location);
        }

        public void GoBack()
        {
            this.firstUpdateTime = null;
            this.starting = this.Position;
            this.targetVector = new Vector2(0,0) - this.targetVector;
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            base.draw(b, alpha);
            base.DrawEmote(b);
        }
    }
}
