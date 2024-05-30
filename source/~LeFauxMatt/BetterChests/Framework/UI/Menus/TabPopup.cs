/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.UI.Menus;

using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.Common.UI.Menus;

internal sealed class TabPopup : BaseMenu
{
    public TabPopup(
        IIconRegistry iconRegistry,
        int? x = null,
        int? y = null,
        int? width = null,
        int? height = null,
        bool showUpperRightCloseButton = false)
        : base(x, y, width, height, showUpperRightCloseButton)
    {
        // var selectIcon = new SelectIcon(
        //     inputHelper,
        //     reflectionHelper,
        //     iconRegistry.GetIcons(),)
    }
}