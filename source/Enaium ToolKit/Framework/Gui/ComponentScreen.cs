/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using EnaiumToolKit.Framework.Screen.Components.Slots;
using Microsoft.Xna.Framework.Graphics;

namespace EnaiumToolKit.Framework.Gui
{
    public class ComponentScreen : GuiScreen
    {
        protected override void Init()
        {
            AddComponent(new Button("Button", "Button", 20, 20, 150, 50));
            var slot = new Slot<LabelSlot>("Slot", "", 10, 60, 200, 560, 80);
            for (int i = 0; i < 10; i++)
            {
                slot.AddEntry(new LabelSlot(i + ""));
            }

            AddComponent(slot);
            base.Init();
        }
    }
}