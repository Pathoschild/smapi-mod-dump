/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Client.UI.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace iTile.Client.UI
{
    public class UIButton : UIPanel
    {
        public Color hoverColor = new Color(180, 180, 180, 200);
        public Color pressedColor = new Color(75, 191, 29, 255);
        public bool pressed;
        public bool canBeClicked = true;
        public bool canBePressed;
        public bool changeColorOnHover = true;
        public delegate void OnClickEvent();
        public OnClickEvent onClick;

        public UIButton(string name, Rectangle transform, UIElement parent = null, Texture2D texture = null)
            : base(name, transform, parent, texture)
        {
            processClicks = true;
        }

        protected override void Update()
        {
            base.Update();
            if (canBePressed && pressed) color = pressedColor;
            else if (!MouseHovered()) color = defaultColor;
        }

        private void OnClick()
        {
            InvokeOnClickDelegate();
            if (canBePressed)
                TogglePressed();
        }

        public void TogglePressed()
        {
            pressed = !pressed;
        }

        protected override void OnMouseHovered()
        {
            if (!canBePressed || !pressed)
                color = hoverColor;
        }

        private void InvokeOnClickDelegate()
        {
            if (onClick != null && onClick.GetInvocationList().Length != 0)
            {
                onClick.Invoke();
            }
        }

        protected override void OnButtonPressed(ButtonPressedEventArgs e)
        {
            base.OnButtonPressed(e);
            if (GetClicked(e.Button))
                OnClick();
        }

        private bool GetClicked(SButton button)
        {
            return show && canBeClicked && MouseHovered() && button == SButton.MouseLeft;
        }
    }
}
