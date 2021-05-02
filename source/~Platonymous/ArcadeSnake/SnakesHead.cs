/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using static Snake.SnakeMinigame;

namespace Snake
{
    public class SnakesHead : SnakeObject
    {
        public ISnakeSegement ChildSegment => (Child as ISnakeSegement);
        public int score = 0;

        public SnakesHead(SnakeMinigame gameInstance)
            : base(gameInstance.Board.position, gameInstance)
        {
            DrawColor = GameInstance.SnakeColor;
            DrawTexture = GameInstance.SpriteSheet;
        }

        public override void Turn(Direction direction)
        {
            if (direction == Direction.UP || direction == Direction.DOWN)
                return;

            if (direction == Direction.RIGHT)
                direction = (Direction)((int)Facing > 0 ? (int)Facing - 1 : 3);
            else if (direction == Direction.LEFT)
                direction = (Direction)((int)Facing < 3 ? (int)Facing + 1 : 0);


            base.Turn(direction);
        }

        public bool IsColliding(SnakeObject o)
        {
            return Math.Abs(position.X - o.position.X) < 1 && Math.Abs(position.Y - o.position.Y) < 1;
        }

        public override void Next()
        {
            base.Next();
            if (IsColliding(GameInstance.Board.nextCollectible))
            GameInstance.Board.nextCollectible.CollideWith(this);

            if (GameInstance.Board.Objects.Exists(e => e is TailSegment s && s.GetBoxPosition() == GetBoxPosition()))
            { 
                GameInstance.Board.GameOver = true;
                GameInstance.Board.Paused = true;

                setScore(SnakeMinigame.Helper,Game1.player.Name, score);

                Game1.currentSong.Stop(AudioStopOptions.Immediate);
                Game1.playSound("death");
            }
                
        }

        public void AddNewTailSegment()
        {
            if (ChildSegment == null)
                GameInstance.Board.Add(new TailSegment(this));
            else
                ChildSegment.AddNewTailSegment();
        }

        public override void Draw(SpriteBatch b)
        {
            base.Draw(b);

            if (GameInstance.hideObjects)
                return;

            b.Draw(DrawTexture, new Rectangle(Drawposition.X, Drawposition.Y, Size.X, Size.Y), new Rectangle?(new Rectangle(GameInstance.SpriteSize.X * (Child != null ? (int)Facing : ((int)Facing + 2) > 3 ? (int)Facing - 2 : (int)Facing + 2), 0, GameInstance.SpriteSize.X, GameInstance.SpriteSize.Y)), GameInstance.Board.GameOver ? Color.Red : DrawColor);
        }
    }
}
