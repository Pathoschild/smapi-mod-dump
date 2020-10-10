/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/trienow/Stardew-CraftPriority
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace CraftPriority
{
    class ModEntry : Mod
    {
        private ModConfig cfg;

        private bool isInCoop = false;

        public override void Entry(IModHelper helper)
        {
            cfg = helper.ReadConfig<ModConfig>();

            Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;

            if (cfg.DisallowEatingInCoop)
            {
                Helper.Events.Player.Warped += this.Player_Warped;
            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            isInCoop = e.IsLocalPlayer && e.NewLocation.Map.Id.ToLower() == "coop";
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                Farmer player = Game1.player;
                Object activeObj = player.ActiveObject;
                if (activeObj != null)
                {
                    int actualEdibility = activeObj.Edibility;
                    if (actualEdibility == -300 && !activeObj.isPlaceable()) //isPlacable prevents machines turning into food (e.g. Chest -> Tuna)
                    {
                        //Create a dummy object and check if it normally has a different edibility
                        Object objReference = new Object(activeObj.ParentSheetIndex, activeObj.Stack, activeObj.IsRecipe, activeObj.Price, activeObj.Quality);
                        actualEdibility = objReference.Edibility;
                    }

                    //If it is actually edible, check if it can be used in the player's surroundings
                    if (actualEdibility != -300)
                    {
                        int px = player.getTileX();
                        int py = player.getTileY();

                        bool preventEating = isInCoop;
                        //Test for "action" blocks around the player
                        for (int y = -1; y <= 1 && !preventEating; y++)
                        {
                            for (int x = -1; x <= 1 && !preventEating; x++)
                            {
                                if (x != 0 && y != 0)
                                {
                                    Object tile = Game1.currentLocation.getObjectAtTile(px + x, py + y);
                                    if (tile != null)
                                    {
                                        Object tileCopy;

                                        //Make a copy to prevent modification of the real object
                                        if (tile is Cask)
                                        {
                                            //Casks have more properties, so copy it separately
                                            tileCopy = new Cask();
                                        }
                                        else
                                        {
                                            tileCopy = new Object(tile.TileLocation, tile.ParentSheetIndex);
                                            tileCopy.Name = tile.Name;
                                        }

                                        //Just in case... (maybe due to an update in the future...?)
                                        if (tileCopy == null)
                                        {
                                            string log = "Copying tile failed. Please report to https://github.com/trienow/Stardew-CraftPriority/issues/new or https://www.nexusmods.com/stardewvalley/mods/4174?tab=bugs\r\n" +
                                                $"Tile to copy: '{tile.Name}'\r\nThanks! -trienow";
                                            Monitor.Log(log, LogLevel.Error);
                                            continue;
                                        }

                                        //To ignore filled machines:
                                        //tileCopy.heldObject.Set(tile.heldObject.Value);

                                        //If a action can be performed, prevent the player from eating the object
                                        preventEating = tileCopy.performObjectDropInAction(activeObj, true, player);
                                    }
                                }
                            }
                        }

                        //If the object can be used in the surrounding tiles: BAN IT!
                        // otherwise unban the object and make it edible again
                        activeObj.Edibility = preventEating ? -300 : actualEdibility;
                    }
                }
            }
        }
    }
}
