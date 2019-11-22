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
    /// Idle state main logic
    /// </summary>
    partial class IdleController : IController
    {
        public bool IsIdle => this.CheckIdleState();

        private IdleBehavior[] behaviors;
        private float[] tendencies;
        private readonly AI_StateMachine ai;
        private readonly IContentLoader loader;
        private IdleBehavior currentBehavior;

        public IdleController(AI_StateMachine ai, IContentLoader loader)
        {
            Dictionary<string, string> idleNpcDefinitions = loader.LoadStrings("Data/IdleNPCDefinitions");
            Dictionary<string, string> behaviorDefinitions = loader.LoadStrings("Data/IdleBehaviors");

            this.ai = ai;
            this.loader = loader;
            this.Setup(idleNpcDefinitions, behaviorDefinitions);
        }

        private int ChooseIdleBehavior()
        {
            float t = 0f;
            foreach (float f in this.tendencies)
                t += f;
            float b = (float)Game1.random.NextDouble() * t;
            for (int i = 0; i < this.tendencies.Length; i++)
            {
                b -= this.tendencies[i];
                if (b <= 0f)
                    return i;
            }
            return this.tendencies.Length - 1;
        }

        private bool CheckIdleState()
        {
            if (this.currentBehavior != null)
                return this.currentBehavior.IsCanceled();

            return false;
        }

        public void Activate()
        {
            int behaviorIndex = this.ChooseIdleBehavior();
            this.currentBehavior = this.behaviors[behaviorIndex];
            this.currentBehavior.StartBehavior();
        }

        public void Deactivate()
        {
            if (this.currentBehavior != null)
                this.currentBehavior.StopBehavior();
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (!Context.IsMultiplayer && !Context.IsPlayerFree)
                return;

            if (this.currentBehavior != null)
                this.currentBehavior.Update(e);
        }
    }

    /// <summary>
    /// Load, parse and assign NPC's idle behaviors
    /// </summary>
    partial class IdleController
    {
        /// <summary>
        /// Load, parse and assign idle behaviors and their's tendencies from definitions
        /// </summary>
        /// <param name="idleNpcDefinitions">Used behaviors and their's tendencies defined for an NPC</param>
        /// <param name="behaviorDefinitions">General idle behavior definitions</param>
        private void Setup(Dictionary<string, string> idleNpcDefinitions, Dictionary<string, string> behaviorDefinitions)
        {
            string npcName = this.ai.npc.Name;

            // Try to load idle definition for this NPC
            if (!idleNpcDefinitions.TryGetValue(npcName, out string idleDefinition))
                throw new Exception($"Can't fetch NPC idle behavior definition for `{npcName}`");

            // Parse idle definition (1 = behavior names, 2 = tendencies)
            var (behaviors, tendencies, _) = idleDefinition.Split('/');

            string[] behavs = behaviors.Split(' '); // Parse behaviors
            string[] tends = tendencies.Split(' '); // parse tendencies

            if (tends.Length != behavs.Length)
                throw new Exception($"Inconsistent lenght of behaviors and tendencies ({behavs.Length} != {tends.Length})");

            // Assign behaviors
            this.behaviors = new IdleBehavior[behavs.Length];
            for (int i = 0; i < behavs.Length; i++)
            {
                string behaviorName = behavs[i];

                if (!behaviorDefinitions.TryGetValue(behaviorName, out string b))
                    throw new Exception($"Cannot find behavior definition for `{behaviorName}`");

                // Parse behavior definition and create real behavior
                var (behaviorType, behaviorArgs) = b.Split('/');
                this.behaviors[i] = this.CreateBehavior(behaviorType, behaviorArgs);
            }
            
            // Assign tendencies
            this.tendencies = new float[tends.Length];
            for (int i = 0; i < tends.Length; i++)
                this.tendencies[i] = float.Parse(tends[i]);
        }

        private IdleBehavior CreateBehavior(string behaviorType, IList<string> args)
        {
            switch (behaviorType)
            {
                case "animate":
                    return new AnimateBehavior(this, this.loader.LoadStrings("Data/AnimationDescriptions"), args[0].Split(' '));
                case "idle":
                    return new IdleBehavior(this);
                default:
                    throw new Exception($"Unknown idle behavior `{behaviorType}`");
            }
        }
    }

    /// <summary>
    /// Idle behaviors
    /// </summary>
    partial class IdleController
    {
        private class IdleBehavior : Internal.IUpdateable
        {
            protected readonly IdleController controller;
            private readonly IMonitor monitor;

            public IdleBehavior(IdleController controller)
            {
                this.controller = controller;
                this.monitor = this.controller.ai.Monitor;
            }

            public virtual void StartBehavior() {
                this.monitor.Log($"Starting idle behavior `{this.GetType().Name}`");
            }

            public virtual void StopBehavior() {
                this.monitor.Log($"Stopping idle behavior `{this.GetType().Name}`");
            }

            public virtual void Update(UpdateTickedEventArgs e) { }

            internal bool IsCanceled()
            {
                Point fp = this.controller.ai.npc.GetBoundingBox().Center;
                Point lp = this.controller.ai.player.GetBoundingBox().Center;
                Vector2 diff = new Vector2(lp.X, lp.Y) - new Vector2(fp.X, fp.Y);

                return diff.Length() > 2.65f * Game1.tileSize;
            }
        }

        private class AnimateBehavior : IdleBehavior
        {
            private List<int[]> animations;
            private List<FarmerSprite.AnimationFrame> frames;

            public AnimateBehavior(IdleController controller, Dictionary<string, string> definitions, string[] animations) : base(controller) {
                this.animations = new List<int[]>();

                foreach (string animation in animations)
                {
                    if (!definitions.TryGetValue(animation, out string definition))
                        throw new Exception($"Cannot get animation definition for `{animation}`, NPC `{controller.ai.npc.Name}`");

                    var (_, loop, _) = definition.Split('/');
                    this.animations.Add(Utility.parseStringToIntArray(loop));
                }
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                int i = Game1.random.Next(0, this.animations.Count);
                this.frames = this.CreateAnimationFrames(this.animations[i]);
                this.controller.ai.npc.Sprite.setCurrentAnimation(this.frames);
                this.controller.ai.npc.Sprite.loop = true;
            }

            public override void StopBehavior()
            {
                base.StopBehavior();
                this.controller.ai.npc.Sprite.StopAnimation();
                this.controller.ai.npc.Sprite.faceDirectionStandard(0);
            }

            private List<FarmerSprite.AnimationFrame> CreateAnimationFrames(int[] frameIndexes)
            {
                var frames = new List<FarmerSprite.AnimationFrame>();

                foreach (var i in frameIndexes) {
                    frames.Add(new FarmerSprite.AnimationFrame(i, 100));
                }

                return frames;
            }
        }
    }
}
