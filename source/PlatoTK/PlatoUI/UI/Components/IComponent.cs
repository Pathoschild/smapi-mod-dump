/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoUI.UI.Styles;
using System;
using System.Collections.Generic;

namespace PlatoUI.UI.Components
{
    public interface IDrawInstructionsHandle
    {
        IDrawInstruction Instructions { get; }
        IComponent Component { get; }
        IComponent Parent { get; }
    }

    public interface IComponent : IDisposable
    {
        string ComponentName { get; }

        int Priority { get; }
        Rectangle Bounds { get; set; }

        IComponent Parent { get; set; }
        string Id { get; }
        bool ShouldRedraw { get; set; }

        string[] Params { get; set; }

        void Recompose();
        void Repopulate();

        bool CacheRender { get; set; }
        bool PreRender { get; set; }

        bool IsSelected { get; }

        Rectangle AbsoluteBounds { get; }

        IComponent New(IPlatoUIHelper helper);

        IComponent Clone(IPlatoUIHelper helper, string id = "", IComponent parent = null);
        void AddDrawInstructions(IDrawInstruction drawInstructions);

        void RemoveDrawInstructions(IDrawInstruction drawInstructions);

        void RemoveDrawInstructions(Predicate<IDrawInstruction> match);

        event EventHandler<IDrawInstructionsHandle> OnDrawInstructionsCompose;

        void Compose(IComponent parent);

        void UpdateAbsoluteBounds();

        void RemoveStyle(IStyle style);

        void AddStyle(IStyle style);

        void AddChild(IComponent child);

        void AddTag(string tag);

        void RemoveTag(string tag);

        bool HasTag(string tag);

        void Select();

        void DeSelect();

        void DeSelectAll(Predicate<IComponent> match);

        void SelectAll(Predicate<IComponent> match);


        void RemoveChild(IComponent child);

        IComponent GetComponentById(string id);

        IEnumerable<IComponent> GetComponentsByTag(string tag);

        void ParseStyle(string property, string value, string option = "");

        void ParseAttribute(string attribute, string value);

        IWrapper GetWrapper();

        void Update(IComponent parent);

        void Draw(SpriteBatch spriteBatch);

        bool IsMouseOver();

        bool WasMouseOver();

        bool WasDragged(out Point from, out Point to);

        bool WasLeftClicked();

        bool WasRightClicked();

        int WasMouseWheelScrolled();
        IWrapper Wrapper { get; set; }
    }
}
