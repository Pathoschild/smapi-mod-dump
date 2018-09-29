using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace UiModSuite.UiMods {
    class SkipIntro {

        internal static void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {
            try {
                var menu = e.NewMenu as TitleMenu;
                menu.skipToTitleButtons();

                FieldInfo logoTimer = menu.GetType().GetField( "chuckleFishTimer", BindingFlags.Instance | BindingFlags.NonPublic );
                logoTimer.SetValue( menu, 0 );

                MenuEvents.MenuChanged -= SkipIntro.onMenuChange;

            } catch ( Exception exception ) {
                ModEntry.Log( "This should never be called." + exception );
            }
        }

    }
}
