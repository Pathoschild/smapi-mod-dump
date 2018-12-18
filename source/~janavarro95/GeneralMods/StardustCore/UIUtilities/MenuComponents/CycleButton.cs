using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardustCore.UIUtilities.MenuComponents.Delegates.Functionality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.MenuComponents
{
    public class CycleButton :Button
    {

        public List<Button> buttons;
        public int buttonIndex;


        public CycleButton(Rectangle bounds, List<Button> buttons, Rectangle SourceRect, float scale) : base(bounds, buttons.ElementAt(0).animationManager.getExtendedTexture(), SourceRect, scale)
        {
            this.buttons = buttons;
            this.buttonIndex = 0;
        }

        public CycleButton(string Name, string displayText, Rectangle bounds, List<Button> buttons, Rectangle SourceRect, float scale, Animations.Animation defaultAnimation, Color DrawColor, Color TextColor, ButtonFunctionality buttonFunctionality, bool AnimationEnabled, List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> extraTexture) : base(Name, bounds, buttons.ElementAt(0).animationManager.getExtendedTexture(), displayText, SourceRect, scale, defaultAnimation, DrawColor, TextColor, buttonFunctionality, AnimationEnabled, extraTexture)
        {
            this.buttons = buttons;
            this.buttonIndex = 0;
        }

        public CycleButton(string Name, string displayText, Rectangle bounds, List<Button> buttons, Rectangle SourceRect, float scale, Animations.Animation defaultAnimation, Color DrawColor, Color TextColor, ButtonFunctionality buttonFunctionality, bool AnimationEnabled, Dictionary<string, List<Animations.Animation>> animationsToPlay, string startingKey, int startingAnimationFrame, List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> extraTexture) : base(Name, bounds, buttons.ElementAt(0).animationManager.getExtendedTexture(), displayText, SourceRect, scale, defaultAnimation, animationsToPlay, startingKey, DrawColor, TextColor, buttonFunctionality, startingAnimationFrame, AnimationEnabled, extraTexture)
        {
            this.buttons = buttons;
            this.buttonIndex = 0;
        }

        public override void onLeftClick()
        {

            //cycle button to next button and loop around if necessary.
            buttonIndex++;
            if (buttonIndex >= buttons.Count)
            {
                buttonIndex = 0;
            }

            base.onLeftClick();
        }

        public override void onLeftClick(int x, int y)
        {
            if (this.containsPoint(x, y))
            {
                StardustCore.ModCore.ModMonitor.Log("CLICK THE CYCLE BUTTON!");
                //cycle button to next button and loop around if necessary.
                buttonIndex++;
                StardustCore.ModCore.ModMonitor.Log("Index is! "+buttonIndex.ToString());
                if (buttonIndex >= buttons.Count)
                {
                    StardustCore.ModCore.ModMonitor.Log("NANIIII????");
                    buttonIndex = 0;
                }

                base.onLeftClick();
            }
        }

        public Button getCurrentButton()
        {
            return buttons.ElementAt(buttonIndex);
        }

        public string getCurrentButtonLabel()
        {
            return buttons.ElementAt(buttonIndex).label;
        }

        public string getCurrentButtonName()
        {
            return buttons.ElementAt(buttonIndex).name;
        }

        public override void draw(SpriteBatch b)
        {
            draw(b, Color.White);
        }

        //CHANGE ALL DRAW FUNCTIONS TO DRAW THE CURRENT BUTTON TEXTURE.
        //Also add in the code to also draw the label of the current button!!!
        public override void draw(SpriteBatch b, Color color)
        {
            draw(b, color, Vector2.Zero);
        }

        public override void draw(SpriteBatch b, Color color, Vector2 offset)
        {
            draw(b, color, offset, 0.5f);
        }


        public override void draw(SpriteBatch b, Color color, Vector2 offset, float layerDepth)
        {

            if (this.extraTextures != null)
            {
                foreach (var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.before)
                    {
                        v.Key.draw(b, color, layerDepth);
                    }
                }
            }

            b.Draw(getCurrentButton().animationManager.getTexture(), new Vector2(this.bounds.X + (int)offset.X, this.bounds.Y + (int)offset.Y), getCurrentButton().sourceRect, color, 0f, Vector2.Zero, getCurrentButton().scale, SpriteEffects.None, layerDepth);

            if (this.extraTextures != null)
            {
                foreach (var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.after)
                    {
                        v.Key.draw(b, color, layerDepth);
                    }
                }
            }
            if (string.IsNullOrEmpty(this.label))
                return;
            b.DrawString(Game1.smallFont, "Voice Mode: "+getCurrentButtonLabel(), new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)), textColor);

        }

    }
}
