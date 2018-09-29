using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compatability.Vanilla
{
    class LoadGameMenu : MenuCompatabilityBase
    {
        public LoadGameMenu()
        {
            minY = 1;
            maxY = 4;
            minX = 1;
            maxX = 3;
            canMoveInMenu = true;
            this.width = Game1.viewport.Width / 2 - (1100 + IClickableMenu.borderWidth * 2) / 2;
            this.height = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;

            componentList = new Dictionary<Point, Rectangle>();


            componentList.Add(new Point(-1, -1), new Rectangle(-999, -999, 2, 2));
            CurrentLocationIndex = new Point(-1, -1);
            startingPositionIndex = new Point(1, 1);


            componentList.Clear();
            Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800, 600, 0, 0);
            //componentList.Add(new Point(1, 1), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 3 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));
            //componentList.Add(new Point(1, 2), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 2 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));
            //componentList.Add(new Point(1, 3), new Rectangle((int)topLeftPositionForCenteringOnScreen.X + Game1.tileSize / 2, (int)topLeftPositionForCenteringOnScreen.Y + 600 - 100 - Game1.tileSize * 1 - Game1.pixelZoom * 4, 800 - Game1.tileSize, Game1.tileSize));

          
              componentList.Add(new Point(1,1),new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + Game1.tileSize *10, Game1.activeClickableMenu.yPositionOnScreen + Game1.tileSize+ 1 * (int)(this.height*1.75f), this.width - Game1.tileSize / 2, this.height / 4 + Game1.pixelZoom));
            componentList.Add(new Point(1, 2), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + Game1.tileSize * 10, Game1.activeClickableMenu.yPositionOnScreen + Game1.tileSize + 2 * (int)(this.height * 1.75f), this.width - Game1.tileSize / 2, this.height / 4 + Game1.pixelZoom));
            componentList.Add(new Point(1, 3), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + Game1.tileSize * 10, Game1.activeClickableMenu.yPositionOnScreen + Game1.tileSize + 3 * (int)(this.height * 1.75f), this.width - Game1.tileSize / 2, this.height / 4 + Game1.pixelZoom));
            componentList.Add(new Point(1, 4), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + Game1.tileSize * 10, Game1.activeClickableMenu.yPositionOnScreen + Game1.tileSize + 4 * (int)(this.height * 1.75f), this.width - Game1.tileSize / 2, this.height / 4 + Game1.pixelZoom));


            componentList.Add(new Point(2, 1), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + this.width +  (int)(Game1.tileSize*17.5f) - Game1.pixelZoom, (int)(Game1.viewport.Height  * .15f), 12 * Game1.pixelZoom, 12 * Game1.pixelZoom));
            componentList.Add(new Point(2, 2), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + this.width + (int)(Game1.tileSize * 17.5f) - Game1.pixelZoom, (int)(Game1.viewport.Height * .35f), 12 * Game1.pixelZoom, 12 * Game1.pixelZoom));
            componentList.Add(new Point(2, 3), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + this.width + (int)(Game1.tileSize * 17.5f) - Game1.pixelZoom, (int)(Game1.viewport.Height * .55f), 12 * Game1.pixelZoom, 12 * Game1.pixelZoom));
            componentList.Add(new Point(2, 4), new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + this.width + (int)(Game1.tileSize * 17.5f) - Game1.pixelZoom, (int)(Game1.viewport.Height * .75f) , 12 * Game1.pixelZoom, 12 * Game1.pixelZoom));
            //   this.deleteButtons.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - Game1.tileSize - Game1.pixelZoom, this.yPositionOnScreen + Game1.tileSize / 2 + Game1.pixelZoom + i * (this.height / 4), 12 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "Delete File", Game1.mouseCursors, new Rectangle(322, 498, 12, 12), (float)Game1.pixelZoom * 3f / 4f, false));


            //back button
            componentList.Add(new Point(3, 1), new Rectangle(Game1.viewport.Width + -198 - 48, Game1.viewport.Height- 81 - 24, 198, 81));
            componentList.Add(new Point(3,2), new Rectangle(Game1.viewport.Width + -198 - 48, Game1.viewport.Height - 81 - 24, 198, 81));
            componentList.Add(new Point(3, 3), new Rectangle(Game1.viewport.Width + -198 - 48, Game1.viewport.Height - 81 - 24, 198, 81));
            componentList.Add(new Point(3, 4), new Rectangle(Game1.viewport.Width + -198 - 48, Game1.viewport.Height - 81 - 24, 198, 81));
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
            Log.AsyncG(CurrentLocationIndex);
            base.moveRight();
        }

        public override void moveDown()
        {
            if (canMoveInMenu == false) return;
            activateTimer();
            CurrentLocationIndex.Y++;

            Rectangle p;
            if (CurrentLocationIndex.Y >= maxY)
            {
                CurrentLocationIndex.Y = maxY;
                try
                {
                    object l = Game1.activeClickableMenu;

                    l = (StardewValley.Menus.LoadGameMenu)Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.activeClickableMenu);
                    //   FieldInfo f = Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                    //Log.AsyncG(l.GetType());
                    //  Game1.activeClickableMenu.GetType().GetProperty("subMenu").GetValue(Game1.activeClickableMenu, null);
                    MethodInfo dynMethod = l.GetType().GetMethod("receiveScrollWheelAction",
        BindingFlags.Public | BindingFlags.Instance);

                    dynMethod.Invoke(l, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Public, null, new object[] { -1 }, null);
                }
                catch (Exception e)
                {
                    Log.AsyncO(e);
                }

            }
            //  Log.AsyncC("CRY");
            componentList.TryGetValue(CurrentLocationIndex, out p);


            updateMouse(getComponentCenter(p));
        }

        public override void moveUp()
        {
            if (canMoveInMenu == false) return;
            activateTimer();
            CurrentLocationIndex.Y--;

            Rectangle p;

            //  Log.AsyncC("CRY");
            componentList.TryGetValue(CurrentLocationIndex, out p);

            if (CurrentLocationIndex.Y < minY)
            {
                CurrentLocationIndex.Y = minY;

                try
                {
                    object l = Game1.activeClickableMenu;

                    l = (StardewValley.Menus.LoadGameMenu)Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.activeClickableMenu);
                    //   FieldInfo f = Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                    //Log.AsyncG(l.GetType());
                    //  Game1.activeClickableMenu.GetType().GetProperty("subMenu").GetValue(Game1.activeClickableMenu, null);
                    MethodInfo dynMethod = l.GetType().GetMethod("receiveScrollWheelAction",
        BindingFlags.Public | BindingFlags.Instance);

                    dynMethod.Invoke(l, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Public, null, new object[] { 1 }, null);
                }
                catch (Exception e)
                {
                    Log.AsyncO(e);
                }

            }

            updateMouse(getComponentCenter(p));
        }

        public override void resize()
        {
            CompatabilityManager.compatabilityMenu = new TitleMenu();
        }
        public override void Update()
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);


            if (currentState.Buttons.A == ButtonState.Pressed && CurrentLocationIndex.X == 3)
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
