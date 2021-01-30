/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using Newtonsoft.Json;
using static BNC.Spawner;
using StardewModdingAPI;
using BNC.TwitchApp;
using StardewValley.Characters;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace BNC.Actions
{
    class SpawnCat : BaseAction
    {

        public static void Init()
        {
            //Turned off for now
            //BNC_Core.helper.Events.Player.Warped += WarpEvent;
        }

        public override ActionResponse Handle()
        {
            try
            {

                BNC_Core.Logger.Log($"Spawning Cat:{from} from Actions", LogLevel.Error);
                Cat cat = new StardewValley.Characters.Cat();
                Point pos = (Game1.getLocationFromName("Farm") as Farm).GetPetStartLocation();
                cat.setTilePosition(pos);
                cat.displayName = "bnc_" + from;
                cat.Name = "bnc_" + from;

                Game1.getFarm().getCharacters().Add(cat);

                return ActionResponse.Done;
            }
            catch (ArgumentNullException)
            {
                BNC_Core.Logger.Log($"Error trying to Spawn Cat from Actions", LogLevel.Error);
                return ActionResponse.Done;
            }

        }

        private static void WarpEvent(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is Farm)
            {
                SpawnCat.tryMoveCats();
            }
        }


        public static void tryRemoveCat()
        {
            foreach (NPC npc in Game1.getFarm().getCharacters())
            {
                if (npc is Cat)
                {
                    if (npc.displayName.StartsWith("bnc_"))
                    {
                        if (Game1.random.NextDouble() < 0.45d)
                        {
                            Game1.getFarm().characters.Remove(npc);
                            Game1.pauseThenMessage(2000, $" {npc.displayName.Substring(4)} the cat wandered too far from the farm.", showProgressBar: false);
                            break;
                        }
                    }
                }
            }
        }

        public static void tryMoveCats()
        {
            foreach (NPC npc in Game1.getFarm().getCharacters())
            {
                if (npc is Cat)
                {
                    if (!(npc.currentLocation is Farm))
                        return;

                    if (npc.displayName.StartsWith("bnc_"))
                    {
                        int tries = 0;
                        while(tries++ < 10)
                        {
                            int randX = Game1.random.Next(npc.currentLocation.map.GetLayer("Back").LayerWidth);
                            int randY = Game1.random.Next(npc.currentLocation.map.GetLayer("Back").LayerHeight);
                            Vector2 pos = new Vector2(randX, randY);
                            if (!npc.currentLocation.isTileOccupied(pos) && !npc.currentLocation.isWaterTile(randX, randY) && npc.currentLocation.isTileOnMap(pos))
                            {
                               ((Cat)npc).setTilePosition(randX, randY);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

