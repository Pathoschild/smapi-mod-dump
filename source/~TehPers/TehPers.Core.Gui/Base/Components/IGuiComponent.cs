using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TehPers.Core.Enums;
using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Units;

namespace TehPers.Core.Gui.Base.Components {
    public interface IGuiComponent {
        /// <summary>The location of this component.</summary>
        ResponsiveVector2<GuiInfo> Location { get; }

        /// <summary>The size of this component.</summary>
        ResponsiveVector2<GuiInfo> Size { get; }

        /// <summary>The <see cref="IGuiComponent"/> that contains this component.</summary>
        IGuiComponent Parent { get; }

        /// <summary>The <see cref="IGuiComponent"/>s contained by this component.</summary>
        IEnumerable<IGuiComponent> Children { get; }

        /// <summary>True if this component is focused, false if not.</summary>
        bool Focused { get; }

        /// <summary>Draws this component based on its parent's dimensions.</summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> to draw with.</param>
        /// <param name="resolvedLocation">The resolved location of this component based on its <see cref="Location"/>.</param>
        /// <param name="resolvedSize">The resolved size of this component based on its <see cref="Size"/></param>
        void Draw(SpriteBatch batch, ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize);

        /// <summary>Called when this component is clicked.</summary>
        /// <param name="resolvedLocation">The resolved location of this component.</param>
        /// <param name="resolvedSize">The resolved size of this component.</param>
        /// <param name="relativeLocation">The location of the click relative to this component.</param>
        /// <param name="buttons">The button being pressed. Only a single button will be clicked at a time.</param>
        /// <returns>Whether the click should be propagated to the parent component.</returns>
        bool Click(ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize, ResolvedVector2 relativeLocation, MouseButtons buttons);

        /// <summary>Called when a key is pressed.</summary>
        /// <param name="key">The key that was pressed.</param>
        void KeyPressed(Keys key);

        /// <summary>Called when a key is released.</summary>
        /// <param name="key">The key that was released.</param>
        void KeyReleased(Keys key);

        /// <summary>Focuses this component if able.</summary>
        /// <returns>True if focused, false if unable to be focused.</returns>
        bool Focus();

        /// <summary>Unfocuses this component.</summary>
        void Unfocus();
    }
}