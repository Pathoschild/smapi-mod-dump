/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu.Framework.ModOption;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared.UI;

namespace GenericModConfigMenu.Framework
{
    internal class ComplexModOptionWidget : Element
    {
        /*********
        ** Accessors
        *********/
        public ComplexModOption ModOption { get; }

        /// <inheritdoc />
        public override int Width { get; } = 0;

        /// <inheritdoc />
        public override int Height { get => ModOption.Height(); }


        /*********
        ** Public methods
        *********/
        public ComplexModOptionWidget(ComplexModOption modOption)
        {
            this.ModOption = modOption;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            // intentionally not calling Element.Update
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (this.IsHidden())
                return;

            this.ModOption.Draw(b, this.Position);
        }
    }
}
