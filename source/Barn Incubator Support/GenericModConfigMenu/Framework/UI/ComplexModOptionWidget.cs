/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu.ModOption;
using Microsoft.Xna.Framework.Graphics;

namespace GenericModConfigMenu.Framework.UI
{
    internal class ComplexModOptionWidget : Element
    {
        public ComplexModOption ModOption { get; }

        public ComplexModOptionWidget(ComplexModOption modOption)
        {
            this.ModOption = modOption;
        }

        public override int Width => 0;
        public override int Height => 0;

        public override void Update(bool hidden = false)
        {
            // intentionally not calling Element.Update
            this.ModOption.Update(this.Position);
        }

        public override void Draw(SpriteBatch b)
        {
            this.ModOption.Draw(b, this.Position);
        }
    }
}
