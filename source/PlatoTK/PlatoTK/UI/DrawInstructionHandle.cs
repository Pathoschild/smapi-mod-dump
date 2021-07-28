/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.UI.Components;

namespace PlatoTK.UI
{
    internal class DrawInstructionHandle : IDrawInstructionsHandle
    {
        public IDrawInstruction Instructions { get; }

        public IComponent Component { get; }

        public IComponent Parent { get; }

        public DrawInstructionHandle(IDrawInstruction instructions, IComponent component, IComponent parent)
        {
            Instructions = instructions;
            Component = component;
            Parent = parent;
        }
    }
}
