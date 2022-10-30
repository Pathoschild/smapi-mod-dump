/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Slothsoft.Challenger.Menus;

internal class ChallengeMenu : BaseOptionsMenu {

    public ChallengeMenu() {
        var pageBounds = CreatePageBounds();
        AddPage(new ChallengePage(pageBounds));
    }

    private Rectangle CreatePageBounds() {
        return new Rectangle(
            xPositionOnScreen, 
            yPositionOnScreen,
            width + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? Game1.tileSize * 3 / 2 : Game1.tileSize / 2), 
            height
        );
    }
}