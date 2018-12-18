using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardustCore.UIUtilities.MenuComponents.Delegates;
using StardustCore.UIUtilities.MenuComponents.Delegates.Functionality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardustCore.UIUtilities.MenuComponents.Delegates.Delegates;

namespace StardustCore.UIUtilities.MenuComponents
{
    public enum ExtraTextureDrawOrder
    {
        before,
        after
    }

    public class Button : StardewValley.Menus.ClickableTextureComponent
    {



        public Animations.AnimationManager animationManager;
        public Color textureColor;
        public Color textColor;

        public ButtonFunctionality buttonFunctionality;

        /// <summary>
        /// A list of textures to be drawn on top of the button.
        /// </summary>
        public List<KeyValuePair<StardewValley.Menus.ClickableTextureComponent,ExtraTextureDrawOrder>> extraTextures;

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public Button(Rectangle Bounds,Texture2DExtended Texture,Rectangle sourceRect,float Scale): base(Bounds, Texture.getTexture(), sourceRect, Scale)
        {
            this.animationManager = new Animations.AnimationManager(Texture, new Animations.Animation(sourceRect), false);
        }

        public Button(string Name,Rectangle Bounds, Texture2DExtended Texture, Rectangle sourceRect, float Scale) : base(Bounds, Texture.getTexture(), sourceRect, Scale)
        {
            this.name = Name;
            this.animationManager = new Animations.AnimationManager(Texture, new Animations.Animation(sourceRect), false);
        }

        public Button(string Name, string displayText, Rectangle Bounds, Texture2DExtended Texture, Rectangle sourceRect, float Scale) : base(Bounds, Texture.getTexture(), sourceRect, Scale)
        {
            this.name = Name;
            this.label = displayText;
            this.animationManager = new Animations.AnimationManager(Texture, new Animations.Animation(sourceRect), false);
        }

        /// <summary>
        /// Basic Button constructor.
        /// </summary>
        /// <param name="Bounds"></param>
        /// <param name="Texture">The texture sheet to be drawn to the screen. Used with the animation manager this allows you to reference different parts of the sheet at any given time.</param>
        /// <param name="sourceRect"></param>
        /// <param name="Scale"></param>
        /// <param name="defaultAnimation"></param>
        /// <param name="AnimationEnabled"></param>
        public Button(string Name,Rectangle Bounds,Texture2DExtended Texture,string displayText,Rectangle sourceRect,float Scale,Animations.Animation defaultAnimation, Color DrawColor,Color TextColor, ButtonFunctionality Functionality, bool AnimationEnabled=true,List<KeyValuePair<ClickableTextureComponent,ExtraTextureDrawOrder>> extraTexture=null) : base(Bounds,Texture.getTexture(), sourceRect,Scale)
        {
            this.animationManager = new Animations.AnimationManager(Texture, defaultAnimation,AnimationEnabled);
            this.label = displayText;
            this.name = Name;
            this.textureColor = DrawColor;
            if (this.textureColor == null)
            {
                this.textureColor = StardustCore.IlluminateFramework.Colors.getColorFromList("White");
            }
            this.textColor = TextColor;
            if (this.textColor == null)
            {
                this.textColor = StardustCore.IlluminateFramework.Colors.getColorFromList("White");
            }
            this.buttonFunctionality = Functionality;
            if (extraTexture == null) extraTexture = new List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>>();
            extraTextures = extraTexture;

            this.scale = Scale;
        }

        /// <summary>
        /// A more advanced Button constructor that deals with an animation manager.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Bounds"></param>
        /// <param name="Texture"></param>
        /// <param name="displayText"></param>
        /// <param name="sourceRect"></param>
        /// <param name="Scale"></param>
        /// <param name="defaultAnimation"></param>
        /// <param name="animationsToPlay"></param>
        /// <param name="startingAnimationKey"></param>
        /// <param name="startingAnimationFrame"></param>
        /// <param name="AnimationEnabled"></param>
        public Button(string Name,Rectangle Bounds,Texture2DExtended Texture, string displayText, Rectangle sourceRect,float Scale, Animations.Animation defaultAnimation,Dictionary<string, List<Animations.Animation>> animationsToPlay,string startingAnimationKey,Color DrawColor,Color TextColor, ButtonFunctionality Functionality,int startingAnimationFrame=0,bool AnimationEnabled=true, List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> extraTexture =null) : base(Bounds, Texture.getTexture(), sourceRect, Scale)
        {
            this.animationManager = new Animations.AnimationManager(Texture, defaultAnimation, animationsToPlay, startingAnimationKey, startingAnimationFrame, AnimationEnabled);
            this.label = displayText;
            this.name = Name;
            this.textureColor = DrawColor;
            if (this.textureColor == null)
            {
                this.textureColor = StardustCore.IlluminateFramework.Colors.getColorFromList("White");
            }
            this.textColor = DrawColor;
            if (this.textColor == null)
            {
                this.textColor = StardustCore.IlluminateFramework.Colors.getColorFromList("White");
            }
            this.buttonFunctionality = Functionality;
            if (extraTexture == null) extraTexture = new List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>>();
            this.extraTextures = extraTexture;

            this.scale = Scale;
        }

        /// <summary>
        /// Draws the button and all of it's components.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="layerDepth"></param>
        public new void draw(SpriteBatch b,Color color ,float layerDepth)
        {
            if (this.extraTextures != null)
            {
                foreach (var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.before)
                    {
                        v.Key.draw(b);
                    }
                }
            }

            this.animationManager.tickAnimation();
            if (!this.visible)
                return;
            if (this.drawShadow)
                Utility.drawWithShadow(b, this.texture, new Vector2((float)this.bounds.X + (float)(this.sourceRect.Width / 2) * this.baseScale, (float)this.bounds.Y + (float)(this.sourceRect.Height / 2) * this.baseScale), this.sourceRect, this.textureColor, 0.0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height / 2)), this.scale, false, layerDepth, -1, -1, 0.35f);
            else
                b.Draw(this.texture, new Vector2((float)this.bounds.X + (float)(this.sourceRect.Width / 2) * this.baseScale, (float)this.bounds.Y + (float)(this.sourceRect.Height / 2) * this.baseScale), new Rectangle?(this.sourceRect), this.textureColor, 0.0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height / 2)), this.scale, SpriteEffects.None, layerDepth);
            if (string.IsNullOrEmpty(this.label)) {

            }
            else {
                b.DrawString(Game1.smallFont, this.label, new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)), this.textColor);
            }

            if (this.hoverText=="")
            {

            }
            else
            {
                //Game1.drawDialogueBox(Game1.getMousePosition().X, Game1.getMousePosition().Y, false, false, this.hoverText);
                //StardustCore.ModCore.ModMonitor.Log("HOVER???");
                b.DrawString(Game1.smallFont, this.hoverText, new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height) - Game1.smallFont.MeasureString(this.label).Y / 2f)), this.textColor,0f,Vector2.Zero,scale,SpriteEffects.None,layerDepth-0.5f);
            }
            if (this.extraTextures != null)
            {
                foreach(var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.after)
                    {
                        v.Key.draw(b);
                    }
                }
            }

        }

        /// <summary>
        /// Draw the button.
        /// </summary>
        /// <param name="b"></param>
        public virtual void draw(SpriteBatch b)
        {
            if (!this.visible)
                return;
            this.draw(b, Color.White, Vector2.Zero);
        }

        /// <summary>
        /// Draw the button.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="color"></param>
        public virtual void draw(SpriteBatch b, Color color)
        {
            if (!this.visible)
                return;
            this.draw(b, color, Vector2.Zero);
        }

        public virtual void draw(SpriteBatch b, Color color, Vector2 offset)
        {
            if (this.extraTextures != null)
            {
                foreach (var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.before)
                    {
                        v.Key.draw(b,color,0.4f);
                    }
                }
            }

            float depth = 0.4f;
            b.Draw(this.animationManager.getTexture(), new Vector2(this.bounds.X + (int)offset.X, this.bounds.Y + (int)offset.Y),this.sourceRect,color,0f,Vector2.Zero,this.scale,SpriteEffects.None, depth);

            if (this.extraTextures != null)
            {
                foreach (var v in this.extraTextures)
                {
                    if (v.Value == ExtraTextureDrawOrder.after)
                    {
                        v.Key.draw(b,color,0.4f);
                    }
                }
            }
            if (string.IsNullOrEmpty(this.label))
                return;
            b.DrawString(Game1.smallFont, this.label, new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)), textColor);

        }

        public virtual void draw(SpriteBatch b, Color color, Vector2 offset, float layerDepth)
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

            b.Draw(this.animationManager.getTexture(), new Vector2(this.bounds.X + (int)offset.X, this.bounds.Y + (int)offset.Y), this.sourceRect, color, 0f, Vector2.Zero, this.scale, SpriteEffects.None, layerDepth);

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
            b.DrawString(Game1.smallFont, this.label, new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)), textColor);

        }


        /// <summary>
        /// Swaps if the button is visible or not. Also toggles the animation manager appropriately.
        /// </summary>
        public void swapVisibility()
        {
            if (this.visible == true)
            {
                this.visible = false;
                this.animationManager.disableAnimation();
            }
        }
    
        /// <summary>
        /// The functionality that occurs when the button is clicked with the right mouse button.
        /// </summary>
        public void onRightClick()
        {
            if (this.buttonFunctionality == null) return;
            else
            {
                if (this.buttonFunctionality.rightClick == null) return;
                else this.buttonFunctionality.rightClick.run();
            }
        }

        /// <summary>
        /// The functionality that occurs when the button is clicked with the left mouse button.
        /// </summary>
        public virtual void onLeftClick()
        {
            if (this.buttonFunctionality == null) return;
            else
            {
                if (this.buttonFunctionality.leftClick == null) return;
                else this.buttonFunctionality.leftClick.run();
            }
        }

        public virtual void onLeftClick(int x, int y)
        {
            //IDK do something???
            if (this.containsPoint(x, y))
            {
                if (this.buttonFunctionality == null) return;
                else
                {
                    if (this.buttonFunctionality.leftClick == null) return;
                    else this.buttonFunctionality.leftClick.run();
                }
            }
        }

        /// <summary>
        /// Triggers when the button is consistently held.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void onLeftClickHeld(int x, int y)
        {

        }

        /// <summary>
        /// The functionality that occcurs when the button is hover overed.
        /// </summary>
        public virtual void onHover()
        {
            if (this.buttonFunctionality == null) return;
            else
            {
                if (this.buttonFunctionality.hover == null) return;
                else this.buttonFunctionality.hover.run();
            }
        }

        /// <summary>
        /// Returns a new object based off of the data of this object.
        /// </summary>
        /// <returns>A Button object that is identical to the one passed in.</returns>
        public virtual Button clone()
        {
            var b= new Button(this.name, this.bounds, this.animationManager.getExtendedTexture(), this.label, this.sourceRect, this.scale, this.animationManager.defaultDrawFrame, this.textureColor, this.textColor, this.buttonFunctionality, true);
            if (b.buttonFunctionality.hover == null)
            {
                StardustCore.ModCore.ModMonitor.Log("I'm null!");
            }
            return b;
        }

        /// <summary>
        /// Makes a clone of the passed in button but at a new position.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public virtual Button clone(Vector2 newPosition)
        {
            var b = new Button(this.name, new Rectangle((int)newPosition.X,(int)newPosition.Y,this.bounds.Width,this.bounds.Height), this.animationManager.getExtendedTexture(), this.label, this.sourceRect, this.scale, this.animationManager.defaultDrawFrame, this.textureColor, this.textColor, this.buttonFunctionality, true);
            if (b.buttonFunctionality.hover == null)
            {
                //StardustCore.ModCore.ModMonitor.Log("I'm null!");
            }
            return b;
        }

        /// <summary>
        /// Returns a new object based off of the data of this object.
        /// </summary>
        /// <returns>A Button object that is identical to the one passed in.</returns>
        public virtual Button copy()
        {
            return this.clone();
        }

        /// <summary>
        /// Generates a new "null" Button.
        /// </summary>
        /// <returns></returns>
        public static Button EmptyButton()
        {
            var b= new Button(new Rectangle(0, 0, 16, 16), new Texture2DExtended(), new Rectangle(0, 0, 16, 16), 1f);
            b.label = "Null";
            return b;
        }

        /// <summary>
        /// Generates a new "null" Button.
        /// </summary>
        /// <returns></returns>
        public static Button Empty()
        {
            return EmptyButton();
        }

        public string getDisplayText()
        {
            return this.label;
        }


    }
}
