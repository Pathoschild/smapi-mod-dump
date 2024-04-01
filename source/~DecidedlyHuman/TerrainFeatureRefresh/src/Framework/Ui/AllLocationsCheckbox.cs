/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrainFeatureRefresh.Framework.Ui;

public class AllLocationsCheckbox : Checkbox
{
    private TfrToggle toggle;

    public AllLocationsCheckbox(Rectangle bounds, string name, Texture2D texture, SpriteFont font, ref TfrToggle toggle) : base(bounds, name, texture, font)
    {
        this.toggle = toggle;
    }

    public override void ReceiveLeftClick()
    {
        base.ReceiveLeftClick();

        this.toggle.On = this.isChecked;
    }
}
