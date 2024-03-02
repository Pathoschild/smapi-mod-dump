/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SimonK1122/More-Lively-Sewer-Overhaul-code-patches
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile;




namespace AimonSewerFrogTest
{

    public class ModEntry : Mod
    {

        public override void Entry(IModHelper helper)
        {
            //Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Player.Warped += Player_Warped;
            Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdate_Ticking;




            GameLocation.RegisterTouchAction("Aimon.SewerOverhaul_SewerUnderway", this.SewerUnderway);
            GameLocation.RegisterTouchAction("Aimon.SewerOverhaul_SewerUnderway1", this.SewerUnderway1);
          
        }

        private void SewerUnderway(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            IAssetDataForMap mapHelper = this.Helper.GameContent.GetPatchHelper(location.map).AsMap();
            mapHelper.PatchMap(
                this.Helper.ModContent.Load<Map>("assets/Aimon_sewerUnderwaypatch.tmx"),
                sourceArea: new Rectangle(13, 24, 7, 4),
                targetArea: new Rectangle(13, 24, 7, 4),
                patchMode: PatchMapMode.Replace
            );
        }
        private void SewerUnderway1(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {

            //if (location is Sewer)
            //{


            IAssetDataForMap mapHelper = this.Helper.GameContent.GetPatchHelper(location.map).AsMap();
            mapHelper.PatchMap(
                this.Helper.ModContent.Load<Map>("assets/Aimon_sewerUnderwaypatch1.tmx"),
                sourceArea: new Rectangle(13, 24, 7, 4),
                targetArea: new Rectangle(13, 24, 7, 4),
                patchMode: PatchMapMode.Replace
            );


            // }
        }

        private void OneSecondUpdate_Ticking(object? sender, OneSecondUpdateTickingEventArgs e)
        {
     
            if (Game1.currentLocation?.Name == "Sewer")
            Game1.ambientLight = new Color(190, 180, 150);
        }
            private void Player_Warped(object? sender, WarpedEventArgs e)
        {
           
            {
               
                if (e.NewLocation.Name == "Sewer")
               
                {
                    Game1.ambientLight = new Color(190, 180, 150);

                    //is it stupid to do randomisation in that way? Probably but at least there's frogs.
                    Random randomNumber = new Random();
                    int i = randomNumber.Next(1, 5);
                    List<Critter> critters = Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").GetValue();


                    if (i == 1)
                    {

                        {
                            {
                                Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                            }
                            e.NewLocation.addCritter(new Frog(new Vector2(11, 26), true, true));
                            e.NewLocation.addCritter(new Frog(new Vector2(5, 29), true, false));
                            e.NewLocation.addCritter(new Frog(new Vector2(24, 27), true, true));
                            e.NewLocation.addCritter(new Frog(new Vector2(35, 17), true, false));
                        }

                    }
                    else if (i == 2)
                    {

                        {
                            {
                                Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                            }
                            e.NewLocation.addCritter(new Frog(new Vector2(18, 28), true, false));
                            e.NewLocation.addCritter(new Frog(new Vector2(10, 15), true, true));
                            e.NewLocation.addCritter(new Frog(new Vector2(5, 14), true, false));


                        }

                    }
                    else if (i == 3)
                    {

                        {
                            {
                                Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                            }
                            e.NewLocation.addCritter(new Frog(new Vector2(5, 8), true, false));
                            e.NewLocation.addCritter(new Frog(new Vector2(34, 21), true, false));


                        }

                    }
                    else if (i == 4)
                    {

                        {
                            {
                                Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                            }
                            e.NewLocation.addCritter(new Frog(new Vector2(4, 27), true, false));
                        }
                    }

                }


                //gets +25 hp after snake milk event
                if
                    (!Game1.player.mailReceived.Contains("AimonSewerSnakeMilkGot"))
                {
                    if (Game1.player.eventsSeen.Contains("194050010"))
                    {
                        Game1.player.maxHealth += 25;
                        Game1.player.mailReceived.Add("AimonSewerSnakeMilkGot");
                    }
                }

                /* QIMILK QUEST vanilla code 
                 * case "qimilk":
                        if (!Game1.player.Contains("qiCave"))
                        {
                            Game1.player.maxHealth += 25;
                            Game1.player.mailReceived.Add("qiCave");
                        } */

            }

        }
    }
}