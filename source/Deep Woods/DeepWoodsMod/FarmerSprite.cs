using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepWoodsMod
{
    class FarmerSprite : StardewValley.FarmerSprite
    {
        public FarmerSprite()
            : base()
        {
        }

        public FarmerSprite(string texture)
            : base(texture)
        {
        }

        public FarmerSprite(StardewValley.FarmerSprite copyFrom)
            : base(copyFrom.textureName.Value)
        {
            this.interval = copyFrom.interval;
            this.SpriteWidth = copyFrom.SpriteWidth;
            this.SpriteHeight = copyFrom.SpriteHeight;

            this.currentAnimationIndex = copyFrom.currentAnimationIndex;
            this.oldFrame = copyFrom.oldFrame;
            if (copyFrom.currentAnimation == null)
            {
#if SDVBETA
                this.CurrentAnimation = null;
#else
                this.currentAnimation = null;
#endif
            }
            else
            {
                if (this.currentAnimation == null)
                {
#if SDVBETA
                    this.CurrentAnimation = new List<AnimationFrame>();
#else
                    this.currentAnimation = new List<AnimationFrame>();
#endif
                }
                this.currentAnimation.Clear();
                this.currentAnimation.AddRange(copyFrom.currentAnimation);
            }
            this.textureUsesFlippedRightForLeft = copyFrom.textureUsesFlippedRightForLeft;
            this.ignoreStopAnimation = copyFrom.ignoreStopAnimation;
            this.ignoreSourceRectUpdates = copyFrom.ignoreSourceRectUpdates;
            this.loop = copyFrom.loop;
            this.tempSpriteHeight = copyFrom.tempSpriteHeight;
            this.currentFrame = copyFrom.currentFrame;
            this.framesPerAnimation = copyFrom.framesPerAnimation;
            this.interval = copyFrom.interval;
            this.timer = copyFrom.timer;
            this.sourceRect = copyFrom.sourceRect;
            // this.endOfAnimationFunction = copyFrom.endOfAnimationFunction;
            // this.contentManager = copyFrom.contentManager;

            this.SpriteWidth = copyFrom.SpriteWidth;
            this.SpriteHeight = copyFrom.SpriteHeight;
            this.SourceRect = copyFrom.SourceRect;
            this.CurrentFrame = copyFrom.CurrentFrame;
            this.CurrentAnimation = copyFrom.CurrentAnimation;

            this.intervalModifier = copyFrom.intervalModifier;
            this.animatingBackwards = copyFrom.animatingBackwards;
            this.nextOffset = copyFrom.nextOffset;
            this.currentStep = copyFrom.currentStep;
            this.currentSingleAnimationInterval = copyFrom.currentSingleAnimationInterval;
            this.ignoreDefaultActionThisTime = copyFrom.ignoreDefaultActionThisTime;
            this.pauseForSingleAnimation = copyFrom.pauseForSingleAnimation;
            this.freezeUntilDialogueIsOver = copyFrom.freezeUntilDialogueIsOver;
            this.loopThisAnimation = copyFrom.loopThisAnimation;
            this.animateBackwards = copyFrom.animateBackwards;

            this.CurrentToolIndex = copyFrom.CurrentToolIndex;
            this.PauseForSingleAnimation = copyFrom.PauseForSingleAnimation;
            this.CurrentFrame = copyFrom.CurrentFrame;
        }

        // We can't override most of our parent classes stuff,
        // but we can override CurrentFrame, which luckily gets accessed
        // every time our parent class has just updated CurrentAnimation,
        // which is what we actually want to hook into.
        public override int CurrentFrame
        {
            get
            {
                FixCurrentAnimation();
                return base.CurrentFrame;
            }
            set
            {
                FixCurrentAnimation();
                base.CurrentFrame = value;
            }
        }

        public override void setCurrentAnimation(List<FarmerSprite.AnimationFrame> animation)
        {
            base.setCurrentAnimation(animation);
            FixCurrentAnimation();
        }

        private bool IsHarvestAnimation()
        {
            return this.CurrentAnimation != null
                && this.CurrentAnimation.Count == 6
                && (this.CurrentAnimation[0].frame == 62 || this.CurrentAnimation[0].frame == 54 || this.CurrentAnimation[0].frame == 58)
                && this.CurrentAnimation[1].frameBehavior != null
                && this.CurrentAnimation[1].frameBehavior == Farmer.showItemIntake;
        }

        private void FixCurrentAnimation()
        {
            if (IsHarvestAnimation())
            {
                this.CurrentAnimation = new AnimationFrame[6]
                {
                    FixAnimationFrame(this.CurrentAnimation[0]),
                    FixAnimationFrame(this.CurrentAnimation[1]),
                    FixAnimationFrame(this.CurrentAnimation[2]),
                    FixAnimationFrame(this.CurrentAnimation[3]),
                    FixAnimationFrame(this.CurrentAnimation[4]),
                    FixAnimationFrame(this.CurrentAnimation[5])
                }.ToList();
            }
        }

        private AnimationFrame FixAnimationFrame(AnimationFrame animationFrame)
        {
            return new AnimationFrame(animationFrame.frame, animationFrame.milliseconds, animationFrame.secondaryArm, animationFrame.flip, FixAnimationFrameBehavior(animationFrame.frameBehavior), animationFrame.behaviorAtEndOfFrame);
        }

        private endOfAnimationBehavior FixAnimationFrameBehavior(endOfAnimationBehavior frameBehavior)
        {
            if (frameBehavior == null)
                return null;

            return InterceptFarmerShowItemIntake;
        }

        private static void InterceptFarmerShowItemIntake(Farmer who)
        {
            if (who.mostRecentlyGrabbedItem is EasterEggItem easterEggItem)
            {
                ShowEasterEggItemIntake(who, easterEggItem);
            }
            else
            {
                Farmer.showItemIntake(who);
            }
        }

        private static void ShowEasterEggItemIntake(Farmer who, EasterEggItem easterEggItem)
        {
            if (easterEggItem == null)
                return;

            bool finishedPickingUp = who.ActiveObject is EasterEggItem && who.FarmerSprite.currentAnimationIndex == 5;

            if (!finishedPickingUp)
            {
                TemporaryAnimatedSprite temporaryAnimatedSprite = CreateTemporaryAnimatedSpriteForEasterEggItemIntake(who, easterEggItem);
                who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
            }

            if (who.FarmerSprite.currentAnimationIndex == 5)
            {
                who.Halt();
                who.FarmerSprite.CurrentAnimation = null;
            }
        }

        private static TemporaryAnimatedSprite CreateTemporaryAnimatedSpriteForEasterEggItemIntake(Farmer who, EasterEggItem easterEggItem)
        {
            float layerDepth = who.getStandingY() / 10000f + 0.01f;
            if (who.FacingDirection == 0)
                layerDepth = who.getStandingY() / 10000f - 0.001f;

            Vector2 location = who.Position + GetEasterEggItemIntakeOffsetFor(who.FacingDirection, who.FarmerSprite.currentAnimationIndex);

            float animationInterval = 100f;
            if (who.FarmerSprite.currentAnimationIndex > 3)
                animationInterval = 200f;

            float alphaFade = 0;
            float scaleChange = 0;
            if (who.FarmerSprite.currentAnimationIndex == 5)
            {
                alphaFade = 0.02f;
                scaleChange = -0.02f;
            }

            return CreateTemporaryAnimatedSpriteForEasterEggItemIntake(easterEggItem, animationInterval, location, layerDepth, alphaFade, scaleChange);
        }

        private static Vector2 GetEasterEggItemIntakeOffsetFor(int facingDirection, int animationIndex)
        {
            switch (facingDirection)
            {
                case 0:
                    switch (animationIndex)
                    {
                        case 1:
                            return new Vector2(0.0f, -32f);
                        case 2:
                            return new Vector2(0.0f, -43f);
                        case 3:
                            return new Vector2(0.0f, -128f);
                        case 4:
                            return new Vector2(0.0f, -120f);
                        case 5:
                            return new Vector2(0.0f, -120f);
                    }
                    break;
                case 1:
                    switch (animationIndex)
                    {
                        case 1:
                            return new Vector2(28f, -64f);
                        case 2:
                            return new Vector2(24f, -72f);
                        case 3:
                            return new Vector2(4f, -128f);
                        case 4:
                            return new Vector2(0.0f, -124f);
                        case 5:
                            return new Vector2(0.0f, -124f);
                    }
                    break;
                case 2:
                    switch (animationIndex)
                    {
                        case 1:
                            return new Vector2(0.0f, -32f);
                        case 2:
                            return new Vector2(0.0f, -43f);
                        case 3:
                            return new Vector2(0.0f, -128f);
                        case 4:
                            return new Vector2(0.0f, -120f);
                        case 5:
                            return new Vector2(0.0f, -120f);
                    }
                    break;
                case 3:
                    switch (animationIndex)
                    {
                        case 1:
                            return new Vector2(-32f, -64f);
                        case 2:
                            return new Vector2(-28f, -76f);
                        case 3:
                            return new Vector2(-16f, -128f);
                        case 4:
                            return new Vector2(0.0f, -124f);
                        case 5:
                            return new Vector2(0.0f, -124f);
                    }
                    break;
            }

            throw new ArgumentException("facingDirection and/or animationIndex out of range: " + facingDirection + ", " + animationIndex);
        }

        private static TemporaryAnimatedSprite CreateTemporaryAnimatedSpriteForEasterEggItemIntake(EasterEggItem easterEggItem, float animationInterval, Vector2 location, float layerDepth, float alphaFade, float scaleChange)
        {
            Rectangle sourceRectangle = Game1.getSourceRectForStandardTileSheet(easterEggItem.texture, easterEggItem.eggTileIndex, 16, 16);

            return new TemporaryAnimatedSprite("Maps\\Festivals", sourceRectangle, animationInterval, 1, 0, location, false, false, layerDepth, alphaFade, Color.White, 4f, scaleChange, 0.0f, 0.0f, false);
        }
    }
}
