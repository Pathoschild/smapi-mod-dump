using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entoarox.Framework.UI;
using Microsoft.Xna.Framework;
using Entoarox.Framework;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;
using xTile;

namespace BusStopMenu
{
    public class BusStopMenu : Mod
    {
        public delegate void fun();

        private static FrameworkMenu Menu;
        private static List<StardewValley.Menus.ClickableTextureComponent> clickables;
        public static Dictionary<ButtonFormComponent, int> clickables2;




        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            StardewModdingAPI.Events.GraphicsEvents.OnPostRenderHudEvent += drawMenu;
            StardewModdingAPI.Events.GameEvents.UpdateTick += checkForClosure;
            clickables = new List<StardewValley.Menus.ClickableTextureComponent>();
            clickables2 = new Dictionary<ButtonFormComponent, int>();

            StardewModdingAPI.Events.GameEvents.UpdateTick += checkForBusStop;
        }

        private void checkForBusStop(object sender, EventArgs e)
        {
            if (Game1.player == null) return;
            if (Game1.player.currentLocation == null) return;
            if (Game1.player.currentLocation.name == "BusStop")
            {
               // Log.AsyncC("Step1");
                MouseState mState = Mouse.GetState();
                if (mState.LeftButton == ButtonState.Pressed)
                {
                    // Log.AsyncC("Step2");
                   // Log.AsyncC(getMouseTile());
                    if (getMouseTile().X == 7&& (getMouseTile().Y ==11 || getMouseTile().Y==10))
                    {
                      //  Log.AsyncC("Step3");
                        Menu = new FrameworkMenu(new Microsoft.Xna.Framework.Point(85, 5 * 11 + 22));
                        Menu.AddComponent(new LabelComponent(new Microsoft.Xna.Framework.Point(-3, -16), "Choose destination"));
                        clickables.Add(Menu.upperRightCloseButton);

                        var r = new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * 3), 65, "Sun Drop City", (t, p, m) => hmm()); //always *4
                        var s = new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * 1), 65, "Pellican Town", (t, p, m) => bleh()); //always *4
                        var d = new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * 2), 65, "Callico Desert", (t, p, m) => bleh()); //always *4
                        Menu.AddComponent(r);
                        Menu.AddComponent(s);
                        Menu.AddComponent(d);
                        clickables2.Add(s, 65);
                        clickables2.Add(r, 65);
                        clickables2.Add(d, 65);
                    }
                }
            }
        }

        private void checkForClosure(object sender, EventArgs e)
        {
            if (Menu != null)
            {
                MouseState mState = Mouse.GetState();
                if (mState.LeftButton == ButtonState.Pressed)
                {
                    Point p = new Point();
                    p.X = Game1.getMouseX();
                    p.Y = Game1.getMouseY();


                    if (Menu.upperRightCloseButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        Game1.player.CanMove = true;
                        Menu.exitThisMenu();
                        
                        if (Menu != null) Menu = null;
                        return;
                    }

                    foreach (KeyValuePair<ButtonFormComponent,int> v in clickables2)
                    {
                        /*
                        Log.AsyncC("THIS POINT" + v.Key.GetPosition());
                        foreach(var q in Menu.InteractiveComponents)
                        {
                          Log.AsyncG( q.GetPosition());

                        }
                        
                        Log.AsyncC("This Mouse" + p);
                        Log.AsyncC("This Menu Area "+Menu.Area);
                        Log.AsyncC("This Event Region" + Menu.EventRegion);
                        Log.AsyncC("This Thing X" + Menu.xPositionOnScreen);
                        Log.AsyncC("This Thing Y" + Menu.yPositionOnScreen);
                        Log.AsyncC("This menu width" + Menu.width);
                        */
                       bool b= v.Key.InBounds(p,new Point(0,0));
                     bool f=   isWithinComponentBounds(getMenuMousePoint(), v.Key);
                      //  Log.AsyncC("THIS IS B" + b);
                     //   Log.AsyncG(f);

                        
                        if (f==true)
                            {
                           
                               (v.Key as ButtonFormComponent).LeftClick(new Point(Game1.getMouseX(), Game1.getMouseY()), new Point(0, 0));
                            }
                            
                      //  Menu.getLastClickableComponentInThisListThatContainsThisYCoord(Menu.InteractiveComponents, getMenuMousePoint().Y);

                    }

                }
            }
            else return;
        }

       public Point getMenuMousePoint()
        {
            return new Point(Game1.getMouseX()-Menu.xPositionOnScreen,Game1.getMouseY()-Menu.yPositionOnScreen);
        }

        public Vector2 getMouseTile()
        {
            
            return new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y);
        }

        public bool isWithinComponentBounds(Point p,IInteractiveMenuComponent i)
        {
            var pX=p.X;
            var iPointX=i.GetPosition().X;
            var iPointXPlusWidth=i.GetPosition().X+Menu.width;
            var pY=p.Y;
            var iPointY=i.GetPosition().Y;
            var iPointYPlusHeight=i.GetPosition().Y+Menu.height;
            
            /*
            Log.AsyncG("is within component bounds?");
            Log.AsyncC("pX "+pX);
            Log.AsyncC("iPointX " +iPointX);
            Log.AsyncC("iPointXPlusWidth "+iPointXPlusWidth);
            Log.AsyncC("PY "+pY);
            Log.AsyncC("iPointY "+ iPointY);
            Log.AsyncC("iPointYPlusHeight "+iPointYPlusHeight);
            */
            int j=0;
            bool f= clickables2.TryGetValue((ButtonFormComponent)i, out j);

            const int height = 40;

           if(p.X>=i.GetPosition().X+(j/2) && p.X<=i.GetPosition().X+Menu.width-(j/2) && p.Y>=i.GetPosition().Y+height && p.Y<=i.GetPosition().Y +(height*2)) return true;

            return false;
        }

        private void hmm()
        {
            Game1.showRedMessage("YUP");
           // Game1.warpFarmer("Farm", 200, 200,false);
        }
        private void bleh()
        {
            Game1.showRedMessage("Awesome!");
        }

        private void drawMenu(object sender, EventArgs e)
        {
            if (Menu != null)
            {
                Game1.player.CanMove = false;
                Menu.draw(Game1.spriteBatch);


                Menu.drawMouse(Game1.spriteBatch);
            }
        }

        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            if (e.KeyPressed.ToString() == "Y")
            {
                
                Menu = new FrameworkMenu(new Microsoft.Xna.Framework.Point(85, 5 * 11 + 22));
                Menu.AddComponent(new LabelComponent(new Microsoft.Xna.Framework.Point(-3, -16), "Choose destination"));
                clickables.Add(Menu.upperRightCloseButton);

                var r = new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * 1), 65, "Hello", (t, p, m) => hmm()); //always *4
                var s = new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * 2), 65, "Bye", (t, p, m) => bleh()); //always *4
              
                Menu.AddComponent(r);
                Menu.AddComponent(s);
                clickables2.Add(s, 65);
                clickables2.Add(r,65);
               // clickables.Add(r);

            }
         
        }


    }
    }
