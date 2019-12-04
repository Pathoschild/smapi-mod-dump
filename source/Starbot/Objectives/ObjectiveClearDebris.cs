using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbot.Objectives
{
    public class ObjectiveClearDebris : Objective
    {
        public override string AnnounceMessage => "Clear debris from the " + targetMap;

        public override string UniquePoolId => "cleardebris." + targetMap;

        public override bool Cooperative => true;

        string targetMap;
        public List<Tuple<int, int>> WoodSpots = new List<Tuple<int, int>>();
        public List<Tuple<int, int>> RockSpots = new List<Tuple<int, int>>();
        public List<Tuple<int, int>> WeedSpots = new List<Tuple<int, int>>();

        bool hasScanned = false;

        bool WasRoutingToWeed = false;
        bool WasRoutingToWood = false;
        bool WasRoutingToRock = false;

        bool WeedRoutingComplete = false;
        bool WoodRoutingComplete = false;
        bool RockRoutingComplete = false;

        public ObjectiveClearDebris(string map)
        {
            IsComplete = false;
            this.targetMap = map;
        }

        public override void Reset()
        {
            base.Reset();
            hasScanned = false;
            WasRoutingToWeed = false;
            WasRoutingToWood = false;
            WasRoutingToRock = false;
            WeedRoutingComplete = false;
            WoodRoutingComplete = false;
            RockRoutingComplete = false;
            WoodSpots.Clear();
            RockSpots.Clear();
            WeedSpots.Clear();
            IsComplete = false;
        }

        List<Tuple<int,int>> SortByDistance(List<Tuple<int,int>> lst)
        {
            List<Tuple<int, int>> output = new List<Tuple<int, int>>();
            output.Add(lst[NearestPoint(new Tuple<int, int>(Game1.player.getTileX(), Game1.player.getTileY()), lst)]);
            lst.Remove(output[0]);
            int x = 0;
            for (int i = 0; i < lst.Count + x; i++)
            {
                output.Add(lst[NearestPoint(output[output.Count - 1], lst)]);
                lst.Remove(output[output.Count - 1]);
                x++;
            }
            return output;
        }

        int NearestPoint(Tuple<int, int> srcPt, List<Tuple<int, int>> lookIn)
        {
            KeyValuePair<double, int> smallestDistance = new KeyValuePair<double, int>();
            for (int i = 0; i < lookIn.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow(srcPt.Item1 - lookIn[i].Item1, 2) + Math.Pow(srcPt.Item2 - lookIn[i].Item2, 2));
                if (i == 0)
                {
                    smallestDistance = new KeyValuePair<double, int>(distance, i);
                }
                else
                {
                    if (distance < smallestDistance.Key)
                    {
                        smallestDistance = new KeyValuePair<double, int>(distance, i);
                    }
                }
            }
            return smallestDistance.Value;
        }

        public override void Step()
        {
            base.Step();
            //short circuit here if energy is low
            if (Game1.player.Stamina <= 5)
            {
                Core.FailObjective();
                Mod.instance.Monitor.Log("Cancelling task, not enough stamina!");
                return;
            }

            //step one: are we on the target map?
            if (Game1.player.currentLocation.NameOrUniqueName != targetMap)
            {
                if (!Core.IsRouting)
                {
                    int tx = -3, ty = -3;
                    Utility.getDefaultWarpLocation(targetMap, ref tx, ref ty);
                    Core.RouteTo(targetMap, tx, ty, true);
                }
                return;
            }

            //step two: scan for forages on the map
            if (!hasScanned)
            {
                Mod.instance.Monitor.Log("Scanning for debris...", StardewModdingAPI.LogLevel.Warn);
                var ojs = Game1.currentLocation.objects;
                List<Vector2> vkeys = ojs.Keys.ToList();
                vkeys.Shuffle();
                foreach (var o in vkeys)
                {
                    if(ojs[o].Name.Contains("Weed"))
                    {
                        var key = new Tuple<int, int>((int)o.X, (int)o.Y);
                        WeedSpots.Add(key);
                    } else if(ojs[o].Name == "Stone")
                    {
                        var key = new Tuple<int, int>((int)o.X, (int)o.Y);
                        RockSpots.Add(key);
                    }
                    else if (ojs[o].Name.Contains("Twig"))
                    {
                        var key = new Tuple<int, int>((int)o.X, (int)o.Y);
                        WoodSpots.Add(key);
                    }
                }
                WeedSpots.Shuffle();
                WeedSpots = SortByDistance(WeedSpots);
                Mod.instance.Monitor.Log("found " + WeedSpots.Count + " weeds");
                RockSpots = SortByDistance(RockSpots);
                Mod.instance.Monitor.Log("found " + RockSpots.Count + " rocks");
                WoodSpots = SortByDistance(WoodSpots);
                Mod.instance.Monitor.Log("found " + WoodSpots.Count + " twigs");
                hasScanned = true;
                return;
            }
            
            //step three: is there any debris? if not, we're complete
            if (RockSpots.Count == 0 && WoodSpots.Count == 0 && WeedSpots.Count == 0)
            {
                IsComplete = true;
                return;
            }

            //check weeds
            //discard nonexistant ones that someone else dealt with
            while (WeedSpots.Count > 0 && !Game1.currentLocation.objects.ContainsKey(new Vector2(WeedSpots[0].Item1, WeedSpots[0].Item2))) WeedSpots.RemoveAt(0);
            if (WeedSpots.Count > 0 && WeedSpots.Count >= WoodSpots.Count && WeedSpots.Count >= RockSpots.Count)
            {
                int pathingCutoff = (int)((150f / WeedSpots.Count) * 150f);
                if (Core.EquipToolIfOnHotbar("Axe")){
                    var spot = WeedSpots[0];

                    if (!WasRoutingToWeed && !WeedRoutingComplete)
                    {
                        WasRoutingToWeed = true;
                        int x = spot.Item1;
                        int y = spot.Item2;
                        //try to route to an adjacent tile
                        List<Tuple<string, int, int>> sides = new List<Tuple<string, int, int>>();
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y + 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y - 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x + 1, y));
                        sides.Add(new Tuple<string, int, int>(targetMap, x - 1, y));
                        sides.Shuffle();
                        if (!Core.RouteTo(sides[0].Item1, sides[0].Item2, sides[0].Item3, false, pathingCutoff))
                        {
                            //try to route to the tile above it
                            if (!Core.RouteTo(sides[1].Item1, sides[1].Item2, sides[1].Item3, false, pathingCutoff))
                            {
                                //try to route to the tile right of it
                                if (!Core.RouteTo(sides[2].Item1, sides[2].Item2, sides[2].Item3, false, pathingCutoff))
                                {
                                    //try the tile left of it
                                    if (!Core.RouteTo(sides[3].Item1, sides[3].Item2, sides[3].Item3, false, pathingCutoff))
                                    {
                                        //we can't reach this one. remove it from the list
                                        WeedSpots.RemoveAt(0);
                                        WasRoutingToWeed = false;
                                    }
                                }
                            }
                        }
                        if (WasRoutingToWeed)
                        {
                            WasRoutingToWeed = false;
                            WeedRoutingComplete = true;
                        }
                        return;
                    }
                    if (WeedRoutingComplete)
                    {
                        //step five: face the forage
                        bool gotcha = (int)(Game1.player.GetToolLocation().X / 64f) == spot.Item1 && (int)(Game1.player.GetToolLocation().Y / 64f) == spot.Item2;
                        if (!gotcha) //we're in the wrong spot. try to face
                        {
                            int x = spot.Item1;
                            int y = spot.Item2;
                            int px = Game1.player.getTileX();
                            int py = Game1.player.getTileY();

                            Core.FaceTile(x, y);

                            WasRoutingToWeed = false;
                            return;
                        }

                        //pick
                        Core.FaceTile(spot.Item1, spot.Item2);
                        Core.SwingTool();
                        Mod.instance.Monitor.Log("Swish!", StardewModdingAPI.LogLevel.Warn);
                        WeedSpots.RemoveAt(0);
                        WeedRoutingComplete = false;
                    }
                    return;
                }
            }

            //check wood            
            //discard nonexistant ones that someone else dealt with
            while (WoodSpots.Count > 0 && !Game1.currentLocation.objects.ContainsKey(new Vector2(WoodSpots[0].Item1, WoodSpots[0].Item2))) WoodSpots.RemoveAt(0);
            if (WoodSpots.Count > 0 && WoodSpots.Count >= RockSpots.Count && WoodSpots.Count >= WeedSpots.Count)
            {
                int pathingCutoff = (int)((150f / WoodSpots.Count) * 150f);
                if (Core.EquipToolIfOnHotbar("Axe")){
                    var spot = WoodSpots[0];
                    if (!WasRoutingToWood && !WoodRoutingComplete)
                    {
                        WasRoutingToWood = true;
                        int x = spot.Item1;
                        int y = spot.Item2;
                        //try to route to an adjacent tile
                        List<Tuple<string, int, int>> sides = new List<Tuple<string, int, int>>();
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y + 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y - 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x + 1, y));
                        sides.Add(new Tuple<string, int, int>(targetMap, x - 1, y));
                        sides.Shuffle();
                        if (!Core.RouteTo(sides[0].Item1, sides[0].Item2, sides[0].Item3, false, pathingCutoff))
                        {
                            //try to route to the tile above it
                            if (!Core.RouteTo(sides[1].Item1, sides[1].Item2, sides[1].Item3, false, pathingCutoff))
                            {
                                //try to route to the tile right of it
                                if (!Core.RouteTo(sides[2].Item1, sides[2].Item2, sides[2].Item3, false, pathingCutoff))
                                {
                                    //try the tile left of it
                                    if (!Core.RouteTo(sides[3].Item1, sides[3].Item2, sides[3].Item3, false, pathingCutoff))
                                    {
                                        //we can't reach this one. remove it from the list
                                        WoodSpots.RemoveAt(0);
                                        WasRoutingToWood = false;
                                    }
                                }
                            }
                        }
                        if (WasRoutingToWood)
                        {
                            WasRoutingToWood = false;
                            WoodRoutingComplete = true;
                        }
                        return;
                    }
                    if (WoodRoutingComplete)
                    {
                        //step five: face the forage
                        bool gotcha = (int)(Game1.player.GetToolLocation().X / 64f) == spot.Item1 && (int)(Game1.player.GetToolLocation().Y / 64f) == spot.Item2;
                        if (!gotcha) //we're in the wrong spot. try to face
                        {
                            int x = spot.Item1;
                            int y = spot.Item2;
                            int px = Game1.player.getTileX();
                            int py = Game1.player.getTileY();

                            Core.FaceTile(x, y);

                            WasRoutingToWood = false;
                            return;
                        }

                        //pick
                        Core.FaceTile(spot.Item1, spot.Item2);
                        Core.SwingTool();
                        Mod.instance.Monitor.Log("Whack!", StardewModdingAPI.LogLevel.Warn);
                        WoodSpots.RemoveAt(0);
                        WoodRoutingComplete = false;
                    }
                    return;
                }
            }

            //check rocks
            //discard nonexistant ones that someone else dealt with
            while (RockSpots.Count > 0 && !Game1.currentLocation.objects.ContainsKey(new Vector2(RockSpots[0].Item1, RockSpots[0].Item2))) RockSpots.RemoveAt(0);
            if (RockSpots.Count > 0 && RockSpots.Count >= WoodSpots.Count && RockSpots.Count >= WeedSpots.Count)
            {
                int pathingCutoff = (int)((150f / RockSpots.Count) * 150f);
                if (Core.EquipToolIfOnHotbar("Pickaxe")){
                    var spot = RockSpots[0];
                    if (!WasRoutingToRock && !RockRoutingComplete)
                    {
                        WasRoutingToRock = true;
                        int x = spot.Item1;
                        int y = spot.Item2;
                        //try to route to an adjacent tile
                        List<Tuple<string, int, int>> sides = new List<Tuple<string, int, int>>();
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y + 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x, y - 1));
                        sides.Add(new Tuple<string, int, int>(targetMap, x + 1, y));
                        sides.Add(new Tuple<string, int, int>(targetMap, x - 1, y));
                        sides.Shuffle();
                        if (!Core.RouteTo(sides[0].Item1, sides[0].Item2, sides[0].Item3, false, pathingCutoff))
                        {
                            //try to route to the tile above it
                            if (!Core.RouteTo(sides[1].Item1, sides[1].Item2, sides[1].Item3, false, pathingCutoff))
                            {
                                //try to route to the tile right of it
                                if (!Core.RouteTo(sides[2].Item1, sides[2].Item2, sides[2].Item3, false, pathingCutoff))
                                {
                                    //try the tile left of it
                                    if (!Core.RouteTo(sides[3].Item1, sides[3].Item2, sides[3].Item3, false, pathingCutoff))
                                    {
                                        //we can't reach this one. remove it from the list
                                        RockSpots.RemoveAt(0);
                                        WasRoutingToRock = false;
                                    }
                                }
                            }
                        }
                        if (WasRoutingToRock)
                        {
                            WasRoutingToRock = false;
                            RockRoutingComplete = true;
                        }
                        return;
                    }
                    if (RockRoutingComplete)
                    {
                        //step five: face the forage
                        bool gotcha = (int)(Game1.player.GetToolLocation().X / 64f) == spot.Item1 && (int)(Game1.player.GetToolLocation().Y / 64f) == spot.Item2;
                        if (!gotcha) //we're in the wrong spot. try to face
                        {
                            int x = spot.Item1;
                            int y = spot.Item2;
                            int px = Game1.player.getTileX();
                            int py = Game1.player.getTileY();

                            Core.FaceTile(x, y);

                            WasRoutingToRock = false;
                            return;
                        }

                        //pick
                        Core.FaceTile(spot.Item1, spot.Item2);
                        Core.SwingTool();
                        Mod.instance.Monitor.Log("Chrakk!", StardewModdingAPI.LogLevel.Warn);
                        RockSpots.RemoveAt(0);
                        RockRoutingComplete = false;
                    }
                    return;
                }
            }
        }

        public override void CantMoveUpdate()
        {
            base.CantMoveUpdate();
        }
    }

}
