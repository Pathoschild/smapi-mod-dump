/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WizardsLizards/CauldronOfChance
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace CauldronOfChance
{
    class ObjectPatches
    {
        public static IMonitor IMonitor { get; private set; }
        public static IModHelper IModHelper { get; private set; }

        public static void Initialize(IMonitor IMonitor, IModHelper IHelper)
        {
            ObjectPatches.IMonitor = IMonitor;
            ObjectPatches.IModHelper = IHelper;
        }

        public static bool checkAction_Prefix(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            try
            {
                if (who.currentLocation.Name.Equals("WizardHouse"))
                {
                    string property = Game1.currentLocation.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                    if (property != null && property.Equals("CauldronOfChance"))
                    {
                        Game1.locations.Where(x => x.Name.Equals("WizardHouse")).First().localSound("bubbles");

                        if (Game1.player.eventsSeen.Contains(ModEntry.eventId) == false)
                        {
                            CauldronMagic.errorMessageProgress = "Event not seen";
                            Game1.activeClickableMenu = new DialogueBox("A gigantic cauldron. It smells like the forest after a rainy day.");
                            return false;
                        }
                        else if (ModEntry.userIds.Contains(Game1.player.UniqueMultiplayerID)
                            //&& false //TODO: REMOVE!!!!
                            )
                        {
                            CauldronMagic.errorMessageProgress = "Event already seen today";
                            Game1.activeClickableMenu = new DialogueBox("The cauldron is bubbling with the ingredients you've added today.");
                            return false;
                        }
                        else
                        {
                            CauldronMagic.errorMessageProgress = "Opening menu";
                            Game1.activeClickableMenu = new CauldronMenu();
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed in {nameof(checkAction_Prefix)}:\n{ex}\nProgress: {CauldronMagic.errorMessageProgress}", LogLevel.Error);
                return true;
            }
        }
    }
}
