using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.CheatCore
{
    class DoorsToWarps
    {

        public static void makeAllDoorsWarps()
        {
            foreach(var v in Game1.locations)
            {
                foreach(var door in v.doors)
                {
                   // ModCore.CoreMonitor.Log(v.name.ToString());
                   // ModCore.CoreMonitor.Log(door.Key.ToString());
                   // ModCore.CoreMonitor.Log(door.Value);

                    foreach(var warp in Game1.getLocationFromName(door.Value).warps)
                    {
                        if (warp.TargetName == v.name && warp.TargetX==door.Key.X&& warp.TargetY==door.Key.Y+1)
                        {
                            Warp w = new Warp(door.Key.X, door.Key.Y, door.Value, warp.X, warp.Y - 1,false);
                            v.warps.Add(w);
                            ModCore.CoreMonitor.Log("Star AI: Cheat Core: Adding warp on door at:" + door.Value + " " + new Vector2(door.Key.X, door.Key.Y));
                        }
                    }
                  
                }
            }
        }
    }
}
