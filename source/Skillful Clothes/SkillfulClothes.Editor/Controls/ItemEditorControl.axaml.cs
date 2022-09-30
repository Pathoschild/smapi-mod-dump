/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SkillfulClothes.Editor.Controls
{
    public partial class ItemEditorControl : UserControl
    {
        public ItemEditorControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
