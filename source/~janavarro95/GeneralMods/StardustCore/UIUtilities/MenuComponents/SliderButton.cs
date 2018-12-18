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

    /// <summary>
    /// An enum describing the types of slider bars that can exist.
    /// </summary>
    public enum SliderStyle
    {
        Horizontal,
        Vertical,
        Square
    }

    /// <summary>
    /// Deals with all of the information pertaining to the limitations and movement of the slider bar button.
    /// </summary>
    public class SliderInformation
    {
        /// <summary>
        /// The yPosition offset of the slider bar button.
        /// </summary>
        public int xPos;

        /// <summary>
        /// The xPosition offset of the slider bar button.
        /// </summary>
        public int yPos;

        /// <summary>
        /// Sensitivity of the slider bar. Probably should be called sluggishness since a higher number makes it move slower.
        /// </summary>
        public int sensitivity;

        /// <summary>
        /// Min possible x position.
        /// </summary>
        public int minX;

        /// <summary>
        /// Min possible y position.
        /// </summary>
        public int minY;

        /// <summary>
        /// Max possible x position.
        /// </summary>
        public int maxX;
        /// <summary>
        /// Max possible y positon;
        /// </summary>
        public int maxY;


        /// <summary>
        /// Enum dealing with what kind of slider this is.
        /// </summary>
        public SliderStyle sliderStyle;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="startingPosition"></param>
        /// <param name="sensitivity"></param>
        public SliderInformation(SliderStyle style, int startingPosition,int sensitivity=1)
        {
            this.sliderStyle = style;
            if (style == SliderStyle.Horizontal)
            {
                this.xPos = startingPosition;
            }
            else if (style == SliderStyle.Vertical)
            {
                this.yPos = startingPosition;
            }
            else if (style == SliderStyle.Square)
            {
                this.xPos = startingPosition;
                this.yPos = startingPosition;
            }
            this.minX = 0;
            this.minY = 0;
            this.maxX = 100;
            this.maxY = 100;
            this.sensitivity = sensitivity;
        }

        public SliderInformation(SliderStyle style, int startingPositionX, int startingPositionY, int sensitivity = 1)
        {
            this.sliderStyle = style;
            if (style == SliderStyle.Horizontal)
            {
                this.xPos = startingPositionX;
            }
            else if (style == SliderStyle.Vertical)
            {
                this.yPos = startingPositionY;
            }
            else if (style == SliderStyle.Square)
            {
                this.xPos = startingPositionX;
                this.yPos = startingPositionY;
            }
            this.minX = 0;
            this.minY = 0;
            this.maxX = 100;
            this.maxY = 100;
            this.sensitivity = sensitivity;
        }

        public SliderInformation(SliderStyle style, int startingPositionX, int startingPositionY, int minX, int maxX, int minY,int maxY, int sensitivity = 1)
        {
            this.sliderStyle = style;
            if (style == SliderStyle.Horizontal)
            {
                this.xPos = startingPositionX;
            }
            else if (style == SliderStyle.Vertical)
            {
                this.yPos = startingPositionY;
            }
            else if (style == SliderStyle.Square)
            {
                this.xPos = startingPositionX;
                this.yPos = startingPositionY;
            }
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
            this.sensitivity = sensitivity;
        }

        /// <summary>
        /// Offset that occurs when the slider button should be clicked and dragged.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void movementOffset(int x, int y)
        {
            if (this.sliderStyle == SliderStyle.Horizontal|| this.sliderStyle==SliderStyle.Square)
            {
                if(x-this.xPos>=sensitivity)
                this.xPos += (x-this.xPos)/sensitivity;
            }
            else if (this.sliderStyle == SliderStyle.Vertical || this.sliderStyle==SliderStyle.Square)
            {
                if (y - this.yPos >= sensitivity)
                {
                    this.yPos += (y - this.yPos) / sensitivity;
                }
            }
        }

        /// <summary>
        /// Offset that occurs when a button is pushed to modify the slider button position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void offset(int x, int y)
        {
            this.xPos += x;
            this.yPos += y;
        }

        /// <summary>
        /// Gets the slider information for a label.
        /// </summary>
        /// <returns></returns>
        public string getLabelInformation(bool getExtraInformation=true)
        {
            if (getExtraInformation == false) return "";
            if (this.sliderStyle == SliderStyle.Horizontal)
            {
                return this.xPos.ToString();
            }
            else if (this.sliderStyle == SliderStyle.Vertical)
            {
                return this.yPos.ToString();
            }
            else if (this.sliderStyle == SliderStyle.Square)
            {
                return this.xPos.ToString() + "," + this.yPos.ToString();
            }
            return "";
        }
    }

    /// <summary>
    /// A menu component that is a sider bar so that you can adjust for different settings.
    /// </summary>
    public class SliderButton: Button
    {

        /// <summary>
        /// Holds all of the information for a slider.
        /// </summary>
        public SliderInformation sliderInformation;
        /// <summary>
        /// Determines whether or not to get extra information for the slider bar.
        /// </summary>
        public bool getLabelXYPos;

        /// <summary>
        /// The texture for the actual slider bar.
        /// </summary>
        public Button sliderBar;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="buttonTexture"></param>
        /// <param name="barTexture"></param>
        /// <param name="SourceRect"></param>
        /// <param name="scale"></param>
        /// <param name="sliderInformation"></param>
        /// <param name="getLabelXYPos"></param>
        public SliderButton(Rectangle bounds, Texture2DExtended buttonTexture,Button barTexture, Rectangle SourceRect, float scale,SliderInformation sliderInformation, bool getLabelXYPos=true): base(bounds, buttonTexture, SourceRect, scale)
        {
            this.sliderInformation = sliderInformation;
            this.getLabelXYPos = getLabelXYPos;
            this.sliderBar = barTexture;
            initializeBounds();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="displayText"></param>
        /// <param name="bounds"></param>
        /// <param name="buttonTexture"></param>
        /// <param name="barTexture"></param>
        /// <param name="SourceRect"></param>
        /// <param name="scale"></param>
        /// <param name="sliderInformation"></param>
        /// <param name="defaultAnimation"></param>
        /// <param name="DrawColor"></param>
        /// <param name="TextColor"></param>
        /// <param name="buttonFunctionality"></param>
        /// <param name="AnimationEnabled"></param>
        /// <param name="extraTexture"></param>
        /// <param name="getLabelXYPos"></param>
        public SliderButton(string Name,string displayText, Rectangle bounds, Texture2DExtended buttonTexture, Button barTexture, Rectangle SourceRect, float scale, SliderInformation sliderInformation, Animations.Animation defaultAnimation, Color DrawColor, Color TextColor, ButtonFunctionality buttonFunctionality, bool AnimationEnabled, List<KeyValuePair<ClickableTextureComponent,ExtraTextureDrawOrder>> extraTexture, bool getLabelXYPos = true) : base(Name,bounds, buttonTexture,displayText, SourceRect, scale,defaultAnimation,DrawColor,TextColor,buttonFunctionality,AnimationEnabled,extraTexture)
        {
            this.sliderInformation = sliderInformation;
            this.getLabelXYPos = getLabelXYPos;
            this.sliderBar = barTexture;
            initializeBounds();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="displayText"></param>
        /// <param name="bounds"></param>
        /// <param name="buttonTexture"></param>
        /// <param name="barTexture"></param>
        /// <param name="SourceRect"></param>
        /// <param name="scale"></param>
        /// <param name="sliderInformation"></param>
        /// <param name="defaultAnimation"></param>
        /// <param name="DrawColor"></param>
        /// <param name="TextColor"></param>
        /// <param name="buttonFunctionality"></param>
        /// <param name="AnimationEnabled"></param>
        /// <param name="animationsToPlay"></param>
        /// <param name="startingKey"></param>
        /// <param name="startingAnimationFrame"></param>
        /// <param name="extraTexture"></param>
        /// <param name="getLabelXYPos"></param>
        public SliderButton(string Name, string displayText, Rectangle bounds, Texture2DExtended buttonTexture, Button barTexture, Rectangle SourceRect, float scale, SliderInformation sliderInformation, Animations.Animation defaultAnimation, Color DrawColor, Color TextColor, ButtonFunctionality buttonFunctionality, bool AnimationEnabled,Dictionary<string,List<Animations.Animation>> animationsToPlay,string startingKey,int startingAnimationFrame ,List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> extraTexture, bool getLabelXYPos = true) : base(Name, bounds, buttonTexture, displayText, SourceRect, scale, defaultAnimation,animationsToPlay,startingKey, DrawColor, TextColor, buttonFunctionality,startingAnimationFrame ,AnimationEnabled, extraTexture)
        {
            this.sliderInformation = sliderInformation;
            this.getLabelXYPos = getLabelXYPos;
            this.sliderBar = barTexture;
            initializeBounds();
        }

        /// <summary>
        /// Interfaces the offset function for sliderInformation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void offset(int x, int y)
        {

            if (x < sliderBar.bounds.X)
            {
                x = sliderBar.bounds.X;
            }
            if (y < sliderBar.bounds.Y)
            {
                y = sliderBar.bounds.Y;
            }
            if (x > (sliderBar.bounds.X) + (sliderBar.bounds.Width))
            {
                x = (sliderBar.bounds.X) + (sliderBar.bounds.Width);
            }
            if (y > (sliderBar.bounds.Y) + (sliderBar.bounds.Height))
            {
                y = (sliderBar.bounds.Y) + (sliderBar.bounds.Height);
            }

            //Get offset from button.
            int xOffset = (bounds.X - x); //267-300
            int yOffset = (bounds.Y - y);


            if (sliderInformation.sliderStyle == SliderStyle.Horizontal || sliderInformation.sliderStyle== SliderStyle.Square)
            {

                this.sliderInformation.xPos = (x - sliderBar.bounds.X) / (int)scale;
                this.bounds.X = this.sliderBar.bounds.X + this.sliderInformation.xPos*(int)scale;
                if (this.bounds.X > this.sliderBar.bounds.X + this.sliderBar.bounds.Width)
                {
                    this.bounds.X = this.sliderBar.bounds.X + this.sliderBar.bounds.Width;
                }
            }

            if (sliderInformation.sliderStyle == SliderStyle.Vertical || sliderInformation.sliderStyle == SliderStyle.Square)
            {
                this.sliderInformation.yPos = (y - sliderBar.bounds.Y) / (int)scale;
                this.bounds.Y = this.sliderBar.bounds.Y + this.sliderInformation.yPos*(int)scale;

                if (this.bounds.Y > this.sliderBar.bounds.Y + this.sliderBar.bounds.Height)
                {
                    this.bounds.Y = this.sliderBar.bounds.Y + this.sliderBar.bounds.Height;
                }
            }

            
            

          
            //this.sliderInformation.offset(xOffset, yOffset);
        }


        /// <summary>
        /// Sets the initial position for the button at sliderPosition.x=0;
        /// </summary>
        public void initializeBounds()
        {
            this.bounds.X +=this.sliderInformation.xPos*(int)scale;
            this.bounds.Y +=this.sliderInformation.yPos*(int)scale;
        }

        /// <summary>
        /// Interfaces the movementOffset function for sliderInformation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void movementOffset(int x, int y)
        {
            offset(x, y);
        }

        /// <summary>
        /// Draws the slider.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="color"></param>
        public override void draw(SpriteBatch b, Color color)
        {
            draw(b, color, Vector2.Zero, 0.0f);
        }

        /// <summary>
        /// Checks if the button or the slider bar has been clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool containsPoint(int x, int y)
        {
            //Got to check to see if I am also dealing with my sliderBar
            if (this.sliderBar.containsPoint(x, y)) return true;

            return base.containsPoint(x, y);
        }

        /// <summary>
        /// Triggers when the slider itself is clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void onLeftClick(int x, int y)
        {
            if (this.sliderBar.containsPoint(x, y))
            {
                int xPos = x;
                int yPos = y;
                this.offset(xPos, yPos);
            }
        }

        /// <summary>
        /// Triggers when the button component of the slider bar is held.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void onLeftClickHeld(int x, int y)
        {
            if (this.containsPoint(x, y))
            {
                this.movementOffset(x, y);
            }
        }



        /// <summary>
        /// D   raws the slider.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="color"></param>
        /// <param name="offset"></param>
        /// <param name="layerDepth"></param>
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
            //Draw the slider bar.
            sliderBar.draw(b, color, offset, layerDepth);
            //b.Draw(this..getTexture(), new Vector2(this.bounds.X + (int)offset.X, this.bounds.Y + (int)offset.Y), this.sourceRect, color, 0f, Vector2.Zero, this.scale, SpriteEffects.None, layerDepth);
            b.Draw(this.animationManager.getTexture(), new Vector2(this.bounds.X + (int)offset.X, this.bounds.Y), this.sourceRect, color, 0f, Vector2.Zero, this.scale, SpriteEffects.None, layerDepth+0.01f);

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
            b.DrawString(Game1.smallFont, this.label+this.sliderInformation.getLabelInformation(this.getLabelXYPos), new Vector2((float)((sliderBar.bounds.X + sliderBar.bounds.Width+offset.X+this.bounds.Width)), (float)sliderBar.bounds.Y + ((float)(sliderBar.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)+offset.X), textColor,0f,Vector2.Zero,1f,SpriteEffects.None,layerDepth+0.02f);
            
        }


    }
}
