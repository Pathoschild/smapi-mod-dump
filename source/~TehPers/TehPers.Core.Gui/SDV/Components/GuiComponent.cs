using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TehPers.Core.Enums;
using TehPers.Core.Gui.Base.Components;
using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Units;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui.SDV.Components {
    public abstract class GuiComponent : IGuiComponent {
        private readonly HashSet<IGuiComponent> _children;

        /// <inheritdoc />
        public virtual ResponsiveVector2<GuiInfo> Location { get; protected set; } = GuiVectors.Zero;

        /// <inheritdoc />
        public virtual ResponsiveVector2<GuiInfo> Size { get; protected set; } = GuiVectors.SameAsParent;

        /// <summary>Distance between the top border of this component and the top of the rectangle children will be drawn in.</summary>
        protected virtual ResponsiveUnit<GuiInfo> PaddingTop { get; } = GuiUnits.Zero;

        /// <summary>Distance between the bottom border of this component and the top of the rectangle children will be drawn in.</summary>
        protected virtual ResponsiveUnit<GuiInfo> PaddingBottom { get; } = GuiUnits.Zero;

        /// <summary>Distance between the left border of this component and the top of the rectangle children will be drawn in.</summary>
        protected virtual ResponsiveUnit<GuiInfo> PaddingLeft { get; } = GuiUnits.Zero;

        /// <summary>Distance between the right border of this component and the top of the rectangle children will be drawn in.</summary>
        protected virtual ResponsiveUnit<GuiInfo> PaddingRight { get; } = GuiUnits.Zero;

        /// <inheritdoc />
        public virtual IGuiComponent Parent { get; }

        /// <inheritdoc />
        public virtual IEnumerable<IGuiComponent> Children => this._children;

        /// <inheritdoc />
        public bool Focused { get; private set; }

        /// <inheritdoc />
        public virtual void Draw(SpriteBatch batch, ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize) {
            this.DrawSelf(batch, resolvedLocation, resolvedSize);

            // Resolve any padding this component has
            float resolvedPaddingTop = this.PaddingTop.Resolve(new GuiInfo(0, 0, resolvedSize.Y));
            float resolvedPaddingBottom = this.PaddingTop.Resolve(new GuiInfo(0, 0, resolvedSize.Y));
            float resolvedPaddingLeft = this.PaddingTop.Resolve(new GuiInfo(0, 0, resolvedSize.X));
            float resolvedPaddingRight = this.PaddingTop.Resolve(new GuiInfo(0, 0, resolvedSize.X));

            // Calculate the child bounds
            ResolvedVector2 childBoundsLoc = new ResolvedVector2(resolvedLocation.X + resolvedPaddingLeft, resolvedLocation.Y + resolvedPaddingTop);
            ResolvedVector2 childBoundsSize = new ResolvedVector2(resolvedSize.X - resolvedPaddingLeft - resolvedPaddingRight, resolvedSize.Y - resolvedPaddingTop - resolvedPaddingBottom);

            // Draw children
            foreach (IGuiComponent child in this.Children) {
                ResolvedVector2 childSize = child.Size.Resolve(new GuiInfo(childBoundsSize.X, resolvedSize.X), new GuiInfo(childBoundsSize.Y, resolvedSize.Y));
                ResolvedVector2 childLoc = child.Location.Resolve(new GuiInfo(childBoundsLoc.X, childBoundsSize.X, childSize.X), new GuiInfo(childBoundsLoc.Y, childBoundsSize.Y, childSize.Y));
                child.Draw(batch, childLoc, childSize);
            }
        }

        /// <summary>Draws this component.</summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> to draw with.</param>
        /// <param name="resolvedLocation">The resolved location of this component based on its <see cref="Location"/>.</param>
        /// <param name="resolvedSize">The resolved size of this component based on its <see cref="Size"/></param>
        protected abstract void DrawSelf(SpriteBatch batch, ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize);

        /// <inheritdoc />
        public virtual bool Click(ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize, ResolvedVector2 relativeLocation, MouseButtons buttons) {
            return true;
        }

        /// <inheritdoc />
        public virtual void KeyPressed(Keys key) { }

        /// <inheritdoc />
        public virtual void KeyReleased(Keys key) { }

        protected GuiComponent() : this(null, null) { }
        protected GuiComponent(IGuiComponent parent) : this(parent, null) { }
        protected GuiComponent(IEnumerable<IGuiComponent> children) : this(null, children) { }
        protected GuiComponent(IGuiComponent parent, IEnumerable<IGuiComponent> children) {
            this.Parent = parent;
            this._children = children?.ToHashSet() ?? new HashSet<IGuiComponent>();
        }

        /// <summary>Adds a child to this component.</summary>
        /// <param name="child">The child to add.</param>
        protected void AddChild(IGuiComponent child) {
            this._children.Add(child);
        }

        /// <summary>Removes a child from this component.</summary>
        /// <param name="child">The child to remove.</param>
        /// <returns>True if the child was removed, false if it was not found.</returns>
        protected bool RemoveChild(IGuiComponent child) {
            return this._children.Remove(child);
        }

        /// <inheritdoc />
        public bool Focus() {
            // Find root of this component system
            IGuiComponent root;
            for (root = this.Parent; root?.Parent != null; root = root.Parent) ;

            // Unfocus the whole system
            root?.Unfocus();

            // Set this component as focused
            this.Focused = true;

            return true;
        }

        /// <inheritdoc />
        public void Unfocus() {
            this.Focused = false;
            foreach (IGuiComponent child in this.Children) {
                child.Unfocus();
            }
        }

        /// <summary>Converts a local depth (between 0.0 and 1.0) to a global depth based on the number of parents this component has.</summary>
        /// <param name="localDepth">The local depth, which is a floating point number between 0.0 and 1.0 inclusive.</param>
        /// <returns>The global depth.</returns>
        protected float GetGlobalDepth(float localDepth) {
            if (localDepth < 0 || localDepth > 1) {
                throw new ArgumentOutOfRangeException(nameof(localDepth), localDepth, "Local depth must be between 0 and 1");
            }

            // Count number of parents
            int parents = 0;
            for (IGuiComponent cur = this; cur.Parent != null; cur = cur.Parent) {
                parents++;
            }

            // Convert the local depth to a global depth
            return 1f - (parents * this._guiDepthRange + localDepth * this._guiDepthRange);
        }

        private readonly float _guiDepthRange = 0.01f;
    }
}