using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using xTile.Tiles;
using xTile.Dimensions;
using xTile.Layers;

using SFarmer = StardewValley.Farmer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace PetEnhancements
{
    public class PetEnhancement : Mod
    {
        private const int FRIENDSHIP_POINTS = 0;

        private bool active = false;
        private PetActionHandler actionHandler = null;
        private MouseState previousMouseState;

        public override void Entry(IModHelper helper)
        {

            GameEvents.UpdateTick += GameEvents_UpdateTick;
            //Helper.ConsoleCommands.Add("print_pet_info", "Shows information about your current pet", print_pet_info_CommandFired());


        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null) return;

            SFarmer farmer = Game1.player;
            Pet pet;

            if (Context.IsWorldReady)
            {

                pet = (Pet)(Game1.getCharacterFromName(farmer.getPetName()));
            } else
            {
                return;
            }



            if (actionHandler == null && pet != null)
            {
                actionHandler = new PetActionHandler(pet);
                actionHandler.intialize();
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
            {
                var cursorTile = Game1.currentCursorTile;

                bool intersects = Utility.doesRectangleIntersectTile(pet.GetBoundingBox(), (int)cursorTile.X, (int)cursorTile.Y);

                if (intersects && pet.friendshipTowardFarmer >= FRIENDSHIP_POINTS)
                {
                    active = !active;
                    if (active)
                    {
                        pet.jump();
                    }
                }
            }

            if (active)
            {
                actionHandler.performAction();


            }

            previousMouseState = mouseState;
        }
    }




}
