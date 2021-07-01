/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;

namespace GenericModConfigMenu.UI
{
    public abstract class Element
    {
        public object UserData { get; set; }

        public Container Parent { get; internal set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Position
        {
            get
            {
                if (Parent != null)
                    return Parent.Position + LocalPosition;
                return LocalPosition;
            }
        }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public bool Hover { get; private set; } = false;
        public virtual string HoveredSound => null;

        public bool ClickGestured { get; private set; } = false;
        public bool Clicked => Hover && ClickGestured;
        public virtual string ClickedSound => null;

        public virtual void Update(bool hidden = false)
        {
            int mouseX;
            int mouseY;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                mouseX = Game1.getMouseX();
                mouseY = Game1.getMouseY();
            }
            else
            {
                mouseX = Game1.getOldMouseX();
                mouseY = Game1.getOldMouseY();
            }

            bool newHover = !hidden && !GetRoot().Obscured && Bounds.Contains(mouseX, mouseY);
            if (newHover && !Hover && HoveredSound != null)
                Game1.playSound(HoveredSound);
            Hover = newHover;

            var input = Mod.instance.Helper.Reflection.GetField< InputState >( typeof( Game1 ), "input" ).GetValue();
            ClickGestured = Game1.oldMouseState.LeftButton == ButtonState.Released && input.GetMouseState().LeftButton == ButtonState.Pressed;
            if (Clicked && ClickedSound != null)
                Game1.playSound(ClickedSound);
        }

        public abstract void Draw(SpriteBatch b);
        
        public RootElement GetRoot()
        {
            return GetRootImpl();
        }
        
        internal virtual RootElement GetRootImpl()
        {
            if (Parent == null)
                throw new Exception("Element must have a parent.");
            return Parent.GetRoot();
        }
    }
}
