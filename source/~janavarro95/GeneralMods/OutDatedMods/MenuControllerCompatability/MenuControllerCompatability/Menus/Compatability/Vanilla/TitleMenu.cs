using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;

namespace Compatability.Vanilla
{
    class TitleMenu:MenuCompatabilityBase
    {
        public TitleMenu()
        {
            minY = 1;
            maxY = 2;
            minX = 1;
            maxX = 5;
            canMoveInMenu = true;
            this.width = Game1.viewport.Width;
            this.height = Game1.viewport.Height;

            componentList = new Dictionary<Point, Rectangle>();


            componentList.Add(new Point(-1, -1), new Rectangle(-999, -999, 2, 2));
            CurrentLocationIndex = new Point(-1, -1);
            startingPositionIndex = new Point(2, 2);

            componentList.Clear();
            componentList.Add(new Point(2, 2), new Rectangle(width / 2 - 333 - 48, height - 174 - 24, 222, 174));//play
            componentList.Add(new Point(3, 2), new Rectangle(this.width / 2 - 111 - 24, this.height - 174 - 24, 222, 174));//load
            componentList.Add(new Point(4, 2), new Rectangle(this.width / 2 + 111, this.height - 174 - 24, 222, 174));//exit
            componentList.Add(new Point(5, 2), new Rectangle(this.width + -66 - 48, this.height - 75 - 24, 66, 75)); //about
            //int end = componentList.Count+1;
            
            //full screen button
            for (int i = 4; i <= 5; i++)
            {
                componentList.Add(new Point(i, 1), new Rectangle(Game1.viewport.Width - 9 * Game1.pixelZoom - Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            }
            //MUTE BUTTON
            componentList.Add(new Point(1, 1),new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            componentList.Add(new Point(2, 1), new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            componentList.Add(new Point(3, 1), new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            //add in menu secrets here


            CompatabilityManager.characterCustomizer = false;
            MenuCompatabilityBase.millisecondMoveDelay = 100;
        }


        public override void Compatability()
        {
            base.Compatability();
        }

     
        //same code as movement but no change
      

        public override void moveLeft()
        {
            base.moveLeft();
        }
        public override void moveRight()
        {
            base.moveRight();
        }

        public override void moveDown()
        {
            base.moveDown();
        }

        public override void moveUp()
        {
            base.moveUp();
        }

        public override void resize()
        {
           CompatabilityManager.compatabilityMenu = new TitleMenu();
        }
        public override void Update()
        {

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            
            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 2 && CurrentLocationIndex.Y == 2)
            {

               // Menus.Compatability.CompatabilityManager.doUpdate = false;
                CompatabilityManager.characterCustomizer = true;

                // Log.AsyncC("A pressed");
                return;
            }

            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 3 && CurrentLocationIndex.Y == 2)
            {

               // Menus.Compatability.CompatabilityManager.doUpdate = false;
                CompatabilityManager.loadMenu = true;
                CompatabilityManager.compatabilityMenu = new Compatability.Vanilla.LoadGameMenu();
                // Log.AsyncC("A pressed");
                return;
            }

            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 5 && CurrentLocationIndex.Y == 2)
            {

                CompatabilityManager.compatabilityMenu = new Compatability.Vanilla.AboutMenu();
                CompatabilityManager.aboutMenu = true;

                // Log.AsyncC("A pressed");
                return;
            }

            base.Update();
        }

    }
}
