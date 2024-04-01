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

public class TfrCheckbox : Checkbox
{
    private TfrFeature associatedFeature;

    public TfrCheckbox(Rectangle bounds, string name, Texture2D texture, SpriteFont font, ref TfrFeature feature) : base(bounds, name, texture, font)
    {
        this.associatedFeature = feature;
    }

    public override void ReceiveLeftClick()
    {
        base.ReceiveLeftClick();

        if (this.isChecked)
        {
            this.associatedFeature.actionToTake = TfrAction.Process;
        }
        else
        {
            this.associatedFeature.actionToTake = TfrAction.Ignore;
        }
    }
}
