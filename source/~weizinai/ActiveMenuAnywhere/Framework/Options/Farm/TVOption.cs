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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.Options;

internal class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_TV(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}