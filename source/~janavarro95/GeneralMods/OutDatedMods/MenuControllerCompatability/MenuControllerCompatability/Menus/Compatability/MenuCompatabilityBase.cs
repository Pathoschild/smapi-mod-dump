using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Timers;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Input;

namespace Compatability
{
    class MenuCompatabilityBase : CompatInterface
    {

        public static Point startingPositionIndex;
        public static Point CurrentLocationIndex;
        public Dictionary<Point, Rectangle> componentList;
        public int width;
        public int height;

        public int maxX;
        public int minX;
        public int maxY;
        public int minY;

        public bool doUpdate;

        public static int millisecondMoveDelay;
        public static System.Timers.Timer movementTimer;
        public static bool canMoveInMenu;

        public static void activateTimer() { 
             SetTimer();        
   }

    private static void SetTimer()
    {
        
            // Create a timer with a two second interval.
            if (canMoveInMenu == true)
            {
                movementTimer = new System.Timers.Timer(millisecondMoveDelay);
                // Hook up the Elapsed event for the timer. 
                movementTimer.Elapsed += OnTimedEvent;
                movementTimer.AutoReset = false;
                movementTimer.Enabled = true;
                canMoveInMenu = false;
            }
            else
            {
                return;
            }
    }

    private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
            movementTimer.Enabled = false;
           // movementTimer.Dispose();
            canMoveInMenu = true;
          
    }

    public Dictionary<Point, Rectangle> ComponentList
        {
            get
            {
                return this.componentList;
                //  throw new NotImplementedException();
                //  return this.componentList;
            }

            set
            {
                // throw new NotImplementedException();
            }
        }

  

        public virtual void Compatability()
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if ((double)currentState.ThumbSticks.Left.X < 0 || currentState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                moveLeft();
            }
            if ((double)currentState.ThumbSticks.Left.X > 0 || currentState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                moveRight();
            }

            if ((double)currentState.ThumbSticks.Left.Y < 0 || currentState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                moveDown();
            }

            if ((double)currentState.ThumbSticks.Left.Y > 0 || currentState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                moveUp();
            }

            Update();
        }

        

        public virtual void moveLeft()
        {
            if (canMoveInMenu == false) return;
            activateTimer();
            CurrentLocationIndex.X--;

            Rectangle p;
            if (CurrentLocationIndex.X <= minX)
            {
                CurrentLocationIndex.X = minX;
            }
            //  Log.AsyncC("CRY");
            componentList.TryGetValue(CurrentLocationIndex, out p);



            updateMouse(getComponentCenter(p));
        }

        public virtual void moveRight()
        {
            if (canMoveInMenu == false) return;
            activateTimer();
            CurrentLocationIndex.X++;

            Rectangle p;

            if (CurrentLocationIndex.X >= maxX)
            {
                CurrentLocationIndex.X = maxX;
            }



            //  Log.AsyncC("CRY");
            componentList.TryGetValue(CurrentLocationIndex, out p);
            updateMouse(getComponentCenter(p));
        }

        public virtual void Update()
        {
            Rectangle p;
            componentList.TryGetValue(CurrentLocationIndex, out p);
            updateMouse(getComponentCenter(p));
        }

        public virtual void updateMouse(Point p)
        {
            
            if (p.X == 0 || p.Y == 0) p = startingPositionIndex;
                Game1.setMousePosition(p);
            
        }

        public virtual Point getComponentCenter(Rectangle r)
        {
            // throw new NotImplementedException();
            return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }

        public virtual void moveUp()
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
            }

            updateMouse(getComponentCenter(p));
        }

        public virtual void moveDown()
        {
            if (canMoveInMenu == false) return;
            activateTimer();
            CurrentLocationIndex.Y++;

            Rectangle p;
            if (CurrentLocationIndex.Y > maxY)
            {
                CurrentLocationIndex.Y = maxY;
            }
            //  Log.AsyncC("CRY");
            componentList.TryGetValue(CurrentLocationIndex, out p);


            updateMouse(getComponentCenter(p));
        }

        public virtual void resize()
        {
            
        }
    }
}
