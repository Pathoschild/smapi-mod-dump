/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

#pragma warning disable IDE0074 // Use compound assignment

using iTile.Client.UI.Impl;
using iTile.Core;
using iTile.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections;
using System.Collections.Generic;

namespace iTile.Client.UI.Framework
{
    public abstract class UIElement : IClickableMenu, IEnumerable
    {
        public static readonly Color defaultColor = new Color(45, 45, 45, 200);
        public Color color = defaultColor;
        public UIElement parent;
        public bool show;
        public bool logicEnabled = true;
        public bool processClicks;
        public Rectangle transform;
        protected List<UIElement> children;
        private Point delta;
        private Rectangle prevTransform;

        public UIElement(string name, Rectangle localTransform, UIElement parent = null)
        {
            Name = name;
            SetParent(parent);
            LocalPosition = new Point(localTransform.X, localTransform.Y);
            transform.Width = localTransform.Width;
            transform.Height = localTransform.Height;
            prevTransform = transform;
            SubscribeEvents();
        }

        protected virtual void SubscribeEvents()
        {
            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Input.ButtonReleased += OnButtonReleased;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!processClicks)
                return;

            OnButtonPressed(e);
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!processClicks)
                return;

            OnButtonReleased(e);
        }

        protected virtual void OnButtonPressed(ButtonPressedEventArgs e)
        {
        }

        protected virtual void OnButtonReleased(ButtonReleasedEventArgs e)
        {
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!logicEnabled)
                return;

            if (MouseHovered())
                OnMouseHovered();

            Update();
        }

        public Point LocalToGlobalPos(Point localPos)
        {
            return new Point(transform.Location.X + localPos.X, transform.Location.Y + localPos.Y);
        }

        protected virtual void OnMouseHovered()
        {
        }

        protected virtual void Update()
        {
        }

        public static IModHelper Helper
            => iTile._Helper;

        public static CoreManager CoreManager
            => CoreManager.Instance;

        public static AssetsManager AssetsManager
            => CoreManager.Instance.assetsManager;

        public static UIManager UIManager
            => CoreManager.Instance.uiManager;

        public List<UIElement> Children
        {
            get => children ?? (children = new List<UIElement>());
            set => children = value;
        }

        public Rectangle ParentTransform
            => parent == null ? new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height) : parent.transform;

        public Point LocalPosition
        {
            get
            {
                if (parent != null)
                    return new Point(transform.X - parent.transform.X, transform.Y - parent.transform.Y);
                else
                    return new Point(transform.X, transform.Y);
            }
            set
            {
                transform.Location = new Point(
                    (parent != null ? parent.transform.X : 0) + value.X,
                    (parent != null ? parent.transform.Y : 0) + value.Y
                );
            }
        }

        public string Name { get; protected set; }

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        public UIElement Hide()
        {
            show = false;
            return this;
        }

        public void MaybeDraw()
        {
            if (show)
            {
                Draw();
                UpdateChildren();
            }
        }

        public void NewChildren(params UIElement[] children)
        {
            if (children == null || children.Length == 0) return;

            foreach (UIElement elem in children)
            {
                if (elem == null) continue;

                elem.parent?.Children.Remove(elem);
                Children.Add(elem);
                elem.parent = this;
            }
        }

        public void SetParent(UIElement parent)
        {
            if (parent == null) return;

            this.parent?.Children.Remove(this);
            parent.Children.Add(this);
            this.parent = parent;
        }

        public virtual void Center(bool horizontally = true, bool vertically = true)
        {
            Point center = ParentTransform.Center;
            Point target = new Point(
                horizontally ? center.X - transform.Width / 2 : transform.X,
                vertically ? center.Y - transform.Height / 2 : transform.Y);
            transform.Location = target;
        }

        public UIElement Show()
        {
            show = true;
            return this;
        }

        public UIElement Toggle()
        {
            show = !show;
            return this;
        }

        protected virtual void Draw()
        {
        }

        protected void UpdateChildren()
        {
            if (Children.Count == 0) return;

            delta = Math.GetDelta(prevTransform, transform);
            prevTransform = transform;
            Children.ForEach(child =>
            {
                child.transform.X += delta.X;
                child.transform.Y += delta.Y;
                child.MaybeDraw();
            });
        }

        public bool MouseHovered() =>
            Game1.getMouseX().OnIntervalStrict(transform.X, transform.X + transform.Width) &&
            Game1.getMouseY().OnIntervalStrict(transform.Y, transform.Y + transform.Height);
    }
}