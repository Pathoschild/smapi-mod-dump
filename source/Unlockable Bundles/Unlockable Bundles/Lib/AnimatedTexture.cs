/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib
{
    public class AnimatedTexture
    {
        public Texture2D Texture;
        private string Animation;
        private long LastUpdatedTick; //Supposed to prevent animations being updated multiple times for the same tick in splitscreen and BundleOverviewMenu
        private int Frame = 0;
        private long Timer = 0;
        private List<KeyValuePair<int, int>> Sequence = new List<KeyValuePair<int, int>>();  //ImageIndex, Tempo
        private int BaseWidth;
        private int BaseHeight;

        public AnimatedTexture(Texture2D texture, string animation, int baseWidth, int baseHeight)
        {
            Texture = texture;
            Animation = animation;
            BaseWidth = baseWidth;
            BaseHeight = baseHeight;
            resetFrames();
        }

        public Rectangle getOffsetRectangle() => getOffsetRectangle(ref Frame, Sequence, Texture, BaseWidth, BaseHeight);
        public static Rectangle getOffsetRectangle(ref int frame, List<KeyValuePair<int, int>> sequence, Texture2D texture, int baseWidth, int baseHeight)
        {
            var sourceRectangle = new Rectangle(0, 0, baseWidth, baseHeight);

            if (sequence.Count > 0 && (texture.Width / baseWidth) > 1)
                sourceRectangle.X = sourceRectangle.Width * sequence.ElementAt(frame).Key;

            return sourceRectangle;
        }

        public void resetFrames() => resetFrames(Animation, ref Frame, Sequence);
        public static void resetFrames(string animation, ref int frame, List<KeyValuePair<int, int>> sequence)
        {
            if (animation is null || animation.Trim() == "")
                return;

            frame = 0;
            sequence.Clear();

            var currentTempo = 100;
            foreach (var entry in animation.Split(",")) {
                var tempoSplit = entry.Split("@");

                if (tempoSplit.Count() > 1)
                    currentTempo = int.Parse(tempoSplit.Last());

                var framesSplit = tempoSplit.First().Split("-");

                var from = int.Parse(framesSplit.First());
                var to = int.Parse(framesSplit.Last());

                if (from < to)
                    for (int i = from; i <= to; i++)
                        sequence.Add(new KeyValuePair<int, int>(i, currentTempo));
                else
                    for (int i = from; i >= to; i--)
                        sequence.Add(new KeyValuePair<int, int>(i, currentTempo));

            }
        }

        public void update(GameTime time) => update(time, ref LastUpdatedTick, ref Timer, ref Frame, Sequence);
        public static void update(GameTime time, ref long lastUpdatedTick, ref long timer, ref int frame, List<KeyValuePair<int, int>> sequence)
        {
            if (time.TotalGameTime.Ticks == lastUpdatedTick)
                return;
            lastUpdatedTick = time.TotalGameTime.Ticks;

            if (sequence.Count == 0)
                return;

            if (timer > 0)
                timer -= time.ElapsedGameTime.Milliseconds;

            if (timer <= 0) {
                frame++;
                if (frame >= sequence.Count)
                    frame = 0;

                timer = sequence.ElementAt(frame).Value;
            }
        }
    }
}
