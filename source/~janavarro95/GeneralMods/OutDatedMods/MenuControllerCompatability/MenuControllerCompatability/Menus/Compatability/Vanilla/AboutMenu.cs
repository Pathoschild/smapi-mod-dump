using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compatability.Vanilla
{
    class AboutMenu : MenuCompatabilityBase
    {
        public AboutMenu()
        {
            minY = 1;
            maxY = 4;
            minX = 1;
            maxX = 1;
            canMoveInMenu = true;
            this.width = Game1.viewport.Width;
            this.height = Game1.viewport.Height;

            componentList = new Dictionary<Point, Rectangle>();

            componentList.Add(new Point(-1, -1), new Rectangle(-999, -999, 2, 2));
            CurrentLocationIndex = new Point(-1, -1);
            startingPositionIndex = new Point(1, 1);


            componentList.Clear();
            Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800, 600, 0, 0);
            componentList.Add(new Point(1, 1), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 3 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));
            componentList.Add(new Point(1, 2), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 2 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));
            componentList.Add(new Point(1, 3), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 1 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));
            componentList.Add(new Point(1, 4), new Rectangle(this.width + -198 - 48, this.height - 81 - 24, 198, 81));
            /*
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
            componentList.Add(new Point(1, 1), new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            componentList.Add(new Point(2, 1), new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            componentList.Add(new Point(3, 1), new Rectangle(Game1.tileSize / 4, Game1.tileSize / 4, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom));
            //add in menu secrets here


            CompatabilityManager.characterCustomizer = false;
            */
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
            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 1 && CurrentLocationIndex.Y == 1)
            {

              //  Menus.Compatability.CompatabilityManager.doUpdate = false;
                // Log.AsyncC("A pressed");
                return;
            }

            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 1 && CurrentLocationIndex.Y == 2)
            {

              //  Menus.Compatability.CompatabilityManager.doUpdate = false;
                //CompatabilityManager.loadMenu = true;

                // Log.AsyncC("A pressed");
                return;
            }

            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 1 && CurrentLocationIndex.Y == 3)
            {

               // Menus.Compatability.CompatabilityManager.doUpdate = false;
              

                // Log.AsyncC("A pressed");
                return;
            }

            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 1 && CurrentLocationIndex.Y == 4)
            {

               // Menus.Compatability.CompatabilityManager.doUpdate = false;
                CompatabilityManager.aboutMenu = false;
                CompatabilityManager.compatabilityMenu = new Compatability.Vanilla.TitleMenu();

                // Log.AsyncC("A pressed");
                return;
            }

            base.Update();
        }



    }
}
