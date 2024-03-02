/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Underscore76/SDVPracticeMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
//using Rectangle = xTile.Dimensions.Rectangle;


namespace SpeedrunPractice.Framework
{
    public class AnimationCancelHelper
    {
        private bool isCancellableSwing;
        private const int FadeCounterMax = 60;
        private int fadeCounter;
        private Color TooEarlyColor = Color.Red;
        private Color ValidColor = Color.Green;
        private int CurrentFrame;
        private List<int> AnimationFrames;
        private List<Color> AnimationColors;
        private TimeSpan FrameTimeSpan = new TimeSpan(166667);
        private List<int> ValidAnimationTypes;

        public AnimationCancelHelper()
        {
            ValidAnimationTypes = new List<int>()
            {
                FarmerSprite.toolUp,
                FarmerSprite.toolRight,
                FarmerSprite.toolDown,
                FarmerSprite.toolLeft,
                180, 172, 164, 188 // watering can based
            };
        }

        public void Update(IMonitor monitor, IModHelper helper)
        {
            if (PlayerInfo.UsingTool && !(PlayerInfo.CurrentTool is MeleeWeapon) && PlayerInfo.CurrentSprite != null)
            {
                int animationType = PlayerInfo.AnimationType;
                if (!isCancellableSwing)
                    CurrentFrame = 1;
                else
                    CurrentFrame++;
                isCancellableSwing = ValidAnimationTypes.Contains(animationType);
                fadeCounter = FadeCounterMax;
                GetAnimationCancelDetails(PlayerInfo.CurrentSprite, out AnimationFrames, out AnimationColors);
                
            }
            else
            {
                isCancellableSwing = false;
                if (fadeCounter > 0)
                {
                    fadeCounter--;
                }
                if (fadeCounter <= 0)
                {
                    AnimationFrames = null;
                    AnimationColors = null;
                    CurrentFrame = -1;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw centered quarter screen
            // draw 1/3 down from top
            // draw rectangle
            Vector2 playerTile = Game1.player.Tile;
            Rectangle progressRectGlobal = new Rectangle((int)(playerTile.X - 3 + 0.5) * Game1.tileSize, (int)(playerTile.Y - 2) * Game1.tileSize, Game1.tileSize * 6, Game1.tileSize / 2);
            Rectangle progressRect = Game1.GlobalToLocal(Game1.viewport, progressRectGlobal);
            if (fadeCounter > 0)
            {
                DrawHelper.DrawProgressBar(spriteBatch, progressRect, AnimationFrames, AnimationColors, CurrentFrame, Color.LightYellow);
            }
        }

        private void GetAnimationCancelDetails(FarmerSprite sprite, out List<int> animationFrames, out List<Color> animationState)
        {
            animationFrames = new List<int>();
            animationState = new List<Color>();
            bool canAnimCancel = false;
            for(int i = 0; i < sprite.CurrentAnimation.Count; i++)
            {
                int frames = Math.Max(1, (int)((sprite.CurrentAnimation[i].milliseconds + FrameTimeSpan.Milliseconds - 1) / FrameTimeSpan.TotalMilliseconds));
                animationFrames.Add(frames);
                if (sprite.CurrentAnimation[i].frameStartBehavior != null && sprite.CurrentAnimation[i].frameStartBehavior.Method.Name.Equals("useTool"))
                    canAnimCancel = true;
                if (i > 0 && sprite.CurrentAnimation[i-1].frameEndBehavior != null && sprite.CurrentAnimation[i-1].frameEndBehavior.Method.Name.Equals("useTool"))
                    canAnimCancel = true;
                animationState.Add(canAnimCancel ? ValidColor : TooEarlyColor);
            }
        }
    }
}
