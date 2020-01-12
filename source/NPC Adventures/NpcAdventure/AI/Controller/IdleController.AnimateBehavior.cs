using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NpcAdventure.Internal;
using NpcAdventure.Loader;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NpcAdventure.AI.Controller
{
    /// <summary>
    /// Idle behaviors
    /// </summary>
    partial class IdleController
    {
        private class AnimateBehavior : IdleBehavior
        {
            private List<IdleAnimation> animations;
            private List<FarmerSprite.AnimationFrame> frames;
            private int currentAnimationIndex = -1;

            private class IdleAnimation
            {
                public int[] intro;
                public int[] loop;
                public int[] outro;
            }

            public AnimateBehavior(IdleController controller, Dictionary<string, string> definitions, string[] animations) : base(controller)
            {
                this.animations = new List<IdleAnimation>();

                foreach (string animation in animations)
                {
                    if (!definitions.TryGetValue(animation, out string definition))
                        throw new Exception($"Cannot get animation definition for `{animation}`, NPC `{controller.ai.npc.Name}`");

                    var (intro, loop, outro, _) = definition.Split('/');
                    this.animations.Add(
                        new IdleAnimation {
                            intro = Utility.parseStringToIntArray(intro),
                            loop = Utility.parseStringToIntArray(loop),
                            outro = Utility.parseStringToIntArray(outro),
                        }
                    );
                }
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                this.ChangeAnimation(true);
            }

            public override void Poke()
            {
                base.Poke();
                this.ChangeAnimation();
            }

            public override void StopBehavior()
            {
                base.StopBehavior();
                this.npc.Sprite.StopAnimation();
                this.npc.Sprite.faceDirectionStandard(0);
                this.currentAnimationIndex = -1;
            }

            private void ChangeAnimation(bool skipOutro = false)
            {
                int i = Game1.random.Next(0, this.animations.Count);

                if (this.currentAnimationIndex == i)
                    i = ++i % this.animations.Count; // Generated the same animation? Roll next in range!

                if (!skipOutro && this.currentAnimationIndex >= 0)
                {
                    // Play outro before animation change
                    this.npc.Sprite.setCurrentAnimation(
                        this.CreateAnimationFrames(this.animations[this.currentAnimationIndex].outro, (Farmer who) => this.SetBahaviorAnimation(i))
                    );
                    this.controller.ai.Monitor.Log($"Play outro animation {this.currentAnimationIndex}");
                    return;
                }

                this.SetBahaviorAnimation(i);
            }

            private void SetBahaviorAnimation(int index)
            {
                this.frames = this.CreateAnimationFrames(this.animations[index].intro, (Farmer who) => {
                    // Play loop after intro
                    this.npc.Sprite.setCurrentAnimation(this.CreateAnimationFrames(this.animations[index].loop));
                    this.npc.Sprite.loop = true;
                    this.controller.ai.Monitor.Log($"Play animation loop {index}");
                });
                // play intro first
                this.npc.Sprite.setCurrentAnimation(this.frames);
                this.currentAnimationIndex = index;
                this.controller.ai.Monitor.Log($"Play intro animation {index}");
            }

            private List<FarmerSprite.AnimationFrame> CreateAnimationFrames(int[] frameIndexes, AnimatedSprite.endOfAnimationBehavior frameBehavior = null)
            {
                var frames = new List<FarmerSprite.AnimationFrame>();

                for (int i = 0; i < frameIndexes.Length; ++i)
                {
                    if (i == frameIndexes.Length - 1)
                        frames.Add(new FarmerSprite.AnimationFrame(frameIndexes[i], 100, false, false, frameBehavior));
                    else
                        frames.Add(new FarmerSprite.AnimationFrame(frameIndexes[i], 100));
                }

                return frames;
            }
        }
    }
}
