/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici
{
    public class ModConfig
    {
        /// <summary>The pixel position at which to draw the spell lable, relative to the top-left corner of the screen.</summary>
        public Point Position { get; set; } = new(10, 10);

        /// <summary>Press Shift and this button to open the spell book.</summary>
        public SButton OpenSpellBookButton { get; set; }

        /// <summary>Press Shift and this button to move the spell lable.</summary>
        public SButton MoveCurrentSpellLable { get; set; } = SButton.OemTilde;

        /// <summary>The button to move to the next spell. Press Shift and this button to move to the next shape group on the selected spell.</summary>
        public SButton NextSpellButton { get; set; } = SButton.X;

        /// <summary>The button to move to the previous spell. Press Shift and this button to move to the previous shape group on the selected spell.</summary>
        public SButton PreviousSpellButton { get; set; } = SButton.Z;

        /// <summary>The button to cast the current spell.</summary>
        public SButton CastSpellButton { get; set; } = SButton.Q;

        public ModConfig()
        {
            this.OpenSpellBookButton = SButton.B;
        }
    }
}
