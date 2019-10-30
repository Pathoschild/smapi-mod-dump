using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals.events
{
    public class EmilysParrotDancer : TemporaryAnimatedSprite
    {
        public const int FlappingPhase = 1;
        public const int HoppingPhase = 0;
        public const int LookingSidewaysPhase = 2;
        public const int NappingPhase = 3;
        public const int HeadBobbingPhase = 4;
        private int _currentFrame;
        private int _currentFrameTimer;
        private int _currentPhaseTimer;
        private int _currentPhase;
        private int _shakeTimer;
        private readonly Random _random;

        public EmilysParrotDancer(Vector2 location)
        {
            this.texture = Game1.mouseCursors;
            this.sourceRect = new Rectangle(92, 148, 9, 16);
            this.sourceRectStartingPos = new Vector2(92f, 149f);
            this.position = location;
            this.initialPosition = this.position;
            this.scale = 4f;
            this.id = 5858586f;
            this._random = new Random((int)((long)Game1.uniqueIDForThisGame * 100000 + SDate.Now().Year * 1000 + Utility.getSeasonNumber(SDate.Now().Season) * 100 + SDate.Now().Day));
        }

        public override bool update(GameTime time)
        {
            this._currentPhaseTimer -= time.ElapsedGameTime.Milliseconds;
            if (this._currentPhaseTimer <= 0)
            {
                Game1.playSound("parrot");
                this._currentPhase = _random.Next(5);
                this.flipped = this._currentPhase != FlappingPhase && _random.Next(2) == 1;
                this._currentPhaseTimer = _random.Next(400, 1600);
                if (this._currentPhase == FlappingPhase)
                {
                    if (this._currentFrame == 0)
                        this.position.X = this.initialPosition.X;
                    else
                        this.position.X = this.initialPosition.X - 8f;
                }
                else
                    this.position = this.initialPosition;
            }
            TimeSpan elapsedGameTime;
            if (this._shakeTimer > 0)
            {
                this.shakeIntensity = 1f;
                int shakeTimer = this._shakeTimer;
                elapsedGameTime = time.ElapsedGameTime;
                int milliseconds = elapsedGameTime.Milliseconds;
                this._shakeTimer = shakeTimer - milliseconds;
            }
            else
                this.shakeIntensity = 0.0f;
            int currentFrameTimer = this._currentFrameTimer;
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds1 = elapsedGameTime.Milliseconds;
            this._currentFrameTimer = currentFrameTimer - milliseconds1;
            if (this._currentFrameTimer <= 0)
            {
                switch (this._currentPhase)
                {
                    case HoppingPhase:
                        if (this._currentFrame == 7)
                        {
                            this._currentFrame = 0;
                            this._currentFrameTimer = 60;
                            break;
                        }
                        if (_random.NextDouble() < 0.5)
                        {
                            this._currentFrame = 7;
                            this._currentFrameTimer = 30;
                            break;
                        }
                        break;
                    case FlappingPhase:
                        this._currentFrame = 6 - this._currentPhaseTimer % 400 / 64;
                        this._currentFrame = 3 - Math.Abs(this._currentFrame - 3);
                        this._currentFrameTimer = 0;
                        this.position.Y = this.initialPosition.Y - (float)(4 * (3 - this._currentFrame));
                        if (this._currentFrame == 0)
                        {
                            this.position.X = this.initialPosition.X;
                            break;
                        }
                        this.position.X = this.initialPosition.X - 8f;
                        break;
                    case LookingSidewaysPhase:
                        this._currentFrame = _random.Next(3, 5);
                        this._currentFrameTimer = 100;
                        break;
                    case NappingPhase:
                        this._currentFrame = this._currentFrame != 5 ? 5 : 6;
                        this._currentFrameTimer = 100;
                        break;
                    case HeadBobbingPhase:
                        this._currentFrame = this._currentFrame != 1 || _random.NextDouble() >= 0.1 ? (this._currentFrame != 2 ? _random.Next(2) : 1) : 2;
                        this._currentFrameTimer = 50;
                        break;
                }
            }
            if (this._currentPhase == FlappingPhase && this._currentFrame != 0)
            {
                this.sourceRect.X = 38 + this._currentFrame * 13;
                this.sourceRect.Width = 13;
            }
            else
            {
                this.sourceRect.X = 92 + this._currentFrame * 9;
                this.sourceRect.Width = 9;
            }
            return false;
        }
    }
}
