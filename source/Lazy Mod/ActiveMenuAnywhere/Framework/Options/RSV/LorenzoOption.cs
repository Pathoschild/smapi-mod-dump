/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class LorenzoOption : BaseOption
{
    public LorenzoOption(Rectangle sourceRect) :
        base(I18n.Option_Lorenzo(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}