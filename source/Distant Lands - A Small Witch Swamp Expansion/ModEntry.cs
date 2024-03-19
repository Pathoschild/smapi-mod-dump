/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SimonK1122/WitchSwampOverhaulPatches
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




namespace AimonsWitchSwampOverhaulPatches
{

    public class ModEntry : Mod
    {
        internal static Random Random = new Random();
        public override void Entry(IModHelper helper)
        {
            //Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Player.Warped += Player_Warped;
            //Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdate_Ticking;





        }


        private void Player_Warped(object? sender, WarpedEventArgs e)
        {


            if (e.NewLocation.Name == "WitchSwamp" || e.NewLocation.Name == "Aimon111.WitchSwampOverhaulCP_event" || e.NewLocation.Name == "Aimon111.WitchSwampOverhaulCP_event2")


            {
                
                List<Critter> critters = Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").GetValue();
                if (critters == null)
                {
                    Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                }

                // firefly spawn1
                for (int i = 0; i < 15; i++)
            {
                Firefly firefly = new Firefly(new Vector2(28, 40)); 
                

                    // firefly colors

                    var light_source = Helper.Reflection.GetField<LightSource>(firefly, "light");
                    

                    int color = ModEntry.Random.Next(0, 3);

                    if (color == 0)
                    {
                        light_source.GetValue().color.Value = new Color(0, 0, 160);

                    }
                    else if (color == 1)
                    {
                        light_source.GetValue().color.Value = new Color(255, 0, 0);

                    }

                    else if (color == 2)
                    {
                        light_source.GetValue().color.Value = new Color(0, 120, 0);
                    }

                    light_source.GetValue().radius.Value = 0.75F; 
                

                e.NewLocation.addCritter(firefly);
           }


                // firefly spawn2


                for (int i = 0; i < 15; i++)
                {
                    Firefly firefly = new Firefly(new Vector2(11, 37));


                

                    var light_source = Helper.Reflection.GetField<LightSource>(firefly, "light");


                    int color = ModEntry.Random.Next(0, 3);

                    if (color == 0)
                    {
                        light_source.GetValue().color.Value = new Color(0, 0, 160); //yellow

                    }
                    else if (color == 1)
                    {
                        light_source.GetValue().color.Value = new Color(255, 0, 0); //cyan

                    }

                    else if (color == 2)
                    {
                        light_source.GetValue().color.Value = new Color(0, 120, 0); //purple
                    }

                    light_source.GetValue().radius.Value = 0.75F;


                    e.NewLocation.addCritter(firefly);
                }

                // firefly spawn3
                for (int i = 0; i < 15; i++)
                {
                    Firefly firefly = new Firefly(new Vector2(28, 24));


                    // These fireflies have weird colors.

                    var light_source = Helper.Reflection.GetField<LightSource>(firefly, "light");


                    int color = ModEntry.Random.Next(0, 3);

                    if (color == 0)
                    {
                        light_source.GetValue().color.Value = new Color(0, 0, 160);

                    }
                    else if (color == 1)
                    {
                        light_source.GetValue().color.Value = new Color(255, 0, 0);

                    }

                    else if (color == 2)
                    {
                        light_source.GetValue().color.Value = new Color(0, 120, 0);
                    }

                    light_source.GetValue().radius.Value = 0.75F;


                    e.NewLocation.addCritter(firefly);
                }
                for (int i = 0; i < 15; i++)

                // firefly spawn4
                {
                    Firefly firefly = new Firefly(new Vector2(8, 17));


                   
                    var light_source = Helper.Reflection.GetField<LightSource>(firefly, "light");


                    int color = ModEntry.Random.Next(0, 3);

                    if (color == 0)
                    {
                        light_source.GetValue().color.Value = new Color(0, 0, 160);

                    }
                    else if (color == 1)
                    {
                        light_source.GetValue().color.Value = new Color(255, 0, 0);

                    }

                    else if (color == 2)
                    {
                        light_source.GetValue().color.Value = new Color(0, 120, 0);
                    }

                    light_source.GetValue().radius.Value = 0.75F;


                    e.NewLocation.addCritter(firefly);
                }

            }



     



            }


        }

}