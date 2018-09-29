using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore.Menus.MenuComponentsAndResources
{
    public class GameMenuComponentPair
    {
        public ClickableTextureComponentExtended menuTab;
        public IClickableMenu menuPage;

        public GameMenuComponentPair(ClickableTextureComponentExtended ClickableTextureComponent, IClickableMenu MenuPage)
        {
            menuTab = ClickableTextureComponent;
            menuPage = MenuPage;
        }

    }
}
