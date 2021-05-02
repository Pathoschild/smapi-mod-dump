/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;

namespace StardustCore.Utilities
{
    public class JunimoAdvanceMoveData
    {
        public string junimoActorID;
        public int maxFrames;
        public int tickSpeed;
        public bool loop;
        public List<Point> points;


        private int currentIndex;
        private int currentFrameAmount;
        private bool finished;

        public JunimoAdvanceMoveData()
        {

        }

        public JunimoAdvanceMoveData(string Actor, List<Point> Points, int FramesToPoint, int tickSpeed = 1,bool Loop=false)
        {
            this.junimoActorID = Actor;
            this.points = Points;
            this.maxFrames = FramesToPoint;

            this.tickSpeed = tickSpeed;
            this.currentFrameAmount = 0;
            this.currentIndex = 0;
            this.loop = Loop;
        }

        public void update()
        {
            if (Game1.CurrentEvent == null) return;
            else
            {
                if (this.finished) return;
                Junimo junimo=(Junimo)Game1.CurrentEvent.actors.Find(i => i.Name.Equals(this.junimoActorID));
                if (junimo == null) return;
                Point nextPoint = this.getNextPoint();
                if (this.finished) return;

                if (nextPoint.X > this.getCurrentPoint().X)
                {
                    junimo.flip = false;

                    //junimo.Sprite.Animate(Game1.currentGameTime, 0, 8, 50f);
                    if (junimo.Sprite.CurrentAnimation==null)
                    {
                        junimo.Sprite.Animate(Game1.currentGameTime, 16, 8, 50f);
                    }
                }
                if (nextPoint.X < this.getCurrentPoint().X)
                {
                    junimo.flip = true;

                    //junimo.Sprite.Animate(Game1.currentGameTime, 0, 8, 50f);
                    if (junimo.Sprite.CurrentAnimation== null)
                    {
                        junimo.Sprite.Animate(Game1.currentGameTime, 16, 8, 50f);
                    }
                }
                if (nextPoint.Y < this.getCurrentPoint().Y)
                {
                    junimo.flip = false;

                    //junimo.Sprite.Animate(Game1.currentGameTime, 0, 8, 50f);
                    if (junimo.Sprite.CurrentAnimation == null)
                    {
                        junimo.Sprite.Animate(Game1.currentGameTime, 32, 8, 50f);
                    }
                }
                if(nextPoint.Y > this.getCurrentPoint().Y)
                {
                    junimo.flip = false;

                    if (junimo.Sprite.CurrentAnimation==null) {
                        junimo.Sprite.Animate(Game1.currentGameTime, 0, 8, 50f);
                    }
                }

                

                junimo.Position= Vector2.Lerp(new Vector2(this.getCurrentPoint().X,this.getCurrentPoint().Y),new Vector2(nextPoint.X,nextPoint.Y),(float)((float)this.currentFrameAmount/(float)this.maxFrames));

                ++this.currentFrameAmount;

                if (this.currentFrameAmount >= this.maxFrames)
                {
                    this.currentFrameAmount = 0;
                    this.currentIndex++;
                    junimo.Sprite.StopAnimation();
                    if (this.currentIndex >= this.points.Count)
                    {
                        if (this.loop == false)
                        {
                            this.finished = true;
                        }
                        else
                        {
                            this.currentIndex = 0;
                        }
                    }
                }
            }
        }

        Point getNextPoint()
        {
            if (this.currentIndex+1 >= this.points.Count)
            {
                if (this.loop == false)
                {
                    this.finished=true;
                    return new Point(0, 0);
                }
                return this.points[0];
            }
            else
            {
                return this.points[this.currentIndex+1];
            }

        }


        Point getCurrentPoint()
        {
            if (this.currentIndex >= this.points.Count)
            {
                return this.points[0];
            }
            else
            {
                return this.points[this.currentIndex];
            }

        }

    }
}
