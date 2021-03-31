/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace CampfireCooking {
    public class ModEntry : Mod {
        private bool m_isMenuOn = false;
        private Config.ModConfig m_config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            m_config = helper.ReadConfig<Config.ModConfig>();
            //check if m_config is correct
            if(m_config == null) {
                this.Monitor.Log("Read configure file error.", LogLevel.Error);
                return;
            }
            if(!m_config.isEnable) {
                return;
            }

            helper.Events.Input.ButtonReleased += onButtonReleased;
            helper.Events.Display.MenuChanged += onMenuChanged;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onButtonReleased(object sender, ButtonReleasedEventArgs e) {
            //only when player is ready shall we start process button event
            if(!Context.IsWorldReady || !Context.IsPlayerFree) {
                return;
            }
            if(!e.Button.IsActionButton()) {
                return;
            }
            tryCampfireCooking();
            /*
            if(m_isMenuOn) {
                e.SuppressButton();
            }
            */
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e) {
            // on menu closed
            if (m_isMenuOn && e.NewMenu == null)
            {
                //SObject cf = tryGetCampfire();
                //if(cf == null) {
                    //return;
                //}
                //cf.checkForAction(Game1.player);
                m_isMenuOn = false;
            }
        }

        private void tryCampfireCooking() {
            SObject cf = tryGetCampfire();
            if(cf != null) {
                //Game1.showGlobalMessage("A campfire");
                m_isMenuOn = true;
                Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, 0, 0);
                Game1.activeClickableMenu = (IClickableMenu)new CraftingPage((int)centeringOnScreen.X, (int)centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true);
            }
        }

        private SObject tryGetCampfire() {
            Vector2 placePos = Game1.player.getTileLocation();
            int d = Game1.player.FacingDirection;
            if((d & 1) == 1) {
                placePos.X += 2 - d;
            } else {
                placePos.Y += d - 1;
            }
            if(Game1.player.currentLocation.objects.ContainsKey(placePos)) {
                SObject  ret =  Game1.player.currentLocation.objects[placePos];
                if(ret != null && m_config.cookableItemNames.Contains(ret.Name) && ret.IsOn) {
                    return ret;
                }
            } else if (Game1.player.currentLocation is StardewValley.Locations.DecoratableLocation dl) {
#if !DEBUG
                foreach (SObject obj in dl.furniture) {
                    if(m_config.cookableItemNames.Contains(obj.Name)) {
                        if (obj.boundingBox.Value.Contains((int)placePos.X * 64 + 32, (int)placePos.Y * 64 + 32) && obj.IsOn) {
                            return obj;
                        }
                    }
                }
#endif
            }
            return null;
        }
    }
}