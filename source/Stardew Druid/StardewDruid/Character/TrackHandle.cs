/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using static StardewValley.Minigames.TargetGame;


namespace StardewDruid.Character
{
    public class TrackHandle
    {
        public CharacterHandle.characters trackFor;
        
        public string trackLocation;

        public Farmer followPlayer;

        public Dictionary<Vector2, int> nodes = new();

        public bool standby;

        public int warpDelay;

        public TrackHandle(CharacterHandle.characters For, Farmer follow = null)
        {
            
            if(follow == null)
            {
                
                followPlayer = Game1.player;

            }
            else
            {

                followPlayer = follow;

            }

            trackFor = For;

        }

        public void TrackPlayer()
        {

            if (trackLocation != followPlayer.currentLocation.Name)
            {

                trackLocation = followPlayer.currentLocation.Name;

                nodes.Clear();

            }

            if (followPlayer.currentLocation is FarmHouse || followPlayer.currentLocation is IslandFarmHouse)
            {

                return;

            }

            if (warpDelay > 0)
            {

                warpDelay--;

            }

            if (Mod.instance.characters[trackFor].currentLocation.Name != followPlayer.currentLocation.Name)
            {

                if (Mod.instance.characters[trackFor].netSceneActive.Value)
                {

                    return;

                }
                
                if(warpDelay > 0)
                {

                    return;

                }

                // Player is not on the same map now

                if (WarpToLocation())
                {

                    nodes.Clear();

                    return; 
                
                }

                // attempt warp every second

                warpDelay = 10;

                return;

            }

            Vector2 center = new Vector2((int)(followPlayer.Position.X / 64), (int)(followPlayer.Position.Y / 64));

            int access = ModUtility.TileAccessibility(followPlayer.currentLocation, center);

            if (access == 2)
            {

                // Path is broken, possibly due to player warping, so will need to warp to player

                nodes.Clear();

                return;
            
            }

            if (nodes.Count == 0)
            {

                if(access == 1)
                {
                    
                    // can't start path on a jump point

                    return;

                }

                nodes.Add(center,access);

                return;

            }

            if (center != nodes.Keys.Last())
            {

                if (nodes.ContainsKey(center))
                {

                    // path doubles back

                    nodes.Clear();

                    nodes.Add(center, access);

                    return;

                }

                // building a legitimate route to player

                nodes.Add(center, access);

            }

        }

        public bool WarpToLocation()
        {

            int direction = -1;

            if(Game1.xLocationAfterWarp > 0)
            {

                Vector2 afterWarp = new(Game1.xLocationAfterWarp, Game1.yLocationAfterWarp);

                float afterSpace = Vector2.Distance(followPlayer.Position, afterWarp);

                if (afterSpace >= 256)
                {

                    //warp between player and warp point

                    direction = ModUtility.DirectionToTarget(followPlayer.Position, afterWarp)[2];

                }
                else
                {
                    // warp to other side of warp point

                    direction = (direction + 4) % 8;

                }

            }

            if (WarpToPlayer(direction))
            {

                //Mod.instance.characters[trackFor].currentLocation.characters.Remove(Mod.instance.characters[trackFor]);

                //Mod.instance.characters[trackFor].currentLocation = followPlayer.currentLocation;

                //Mod.instance.characters[trackFor].currentLocation.characters.Add(Mod.instance.characters[trackFor]);

                //Mod.instance.iconData.AnimateQuickWarp(Mod.instance.characters[trackFor].currentLocation, Mod.instance.characters[trackFor].Position - new Vector2(0, 32));

                //Mod.instance.characters[trackFor].DeactivateStandby();

                //Mod.instance.characters[trackFor].ResetActives();

                //Mod.instance.characters[trackFor].attentionTimer = 360;

                //Mod.instance.characters[trackFor].update(Game1.currentGameTime, Mod.instance.characters[trackFor].currentLocation);

                return true;

            }

            return false;

        }

        public bool WarpToPlayer(int direction = -1)
        {

            if(warpDelay > 0)
            {

                return false;

            }

            if (direction == -1 && nodes.Count > 0)
            {

                direction = ModUtility.DirectionToTarget(followPlayer.Position, nodes.Keys.Last())[2];

            }

            if (direction == -1)
            {

                // try for center

                direction = ModUtility.DirectionToCenter(followPlayer.currentLocation, followPlayer.Position)[2];

            }

            // get player tile

            Vector2 center = ModUtility.PositionToTile(followPlayer.Position);// new Vector2((int)(followPlayer.Position.X / 64), (int)(followPlayer.Position.Y / 64));

            // get occupiable tiles

            List<Vector2> options = ModUtility.GetOccupiableTilesNearby(followPlayer.currentLocation, center, direction, 1, 2);

            // check who else might warp there

            List<Vector2> occupied = new();

            foreach (KeyValuePair<CharacterHandle.characters, StardewDruid.Character.Character> friends in Mod.instance.characters)
            {

                if (friends.Key == trackFor) { continue; }

                if (friends.Value is Actor) { continue; }

                if (friends.Value.currentLocation.Name != followPlayer.currentLocation.Name) { continue; }

                occupied.Add(friends.Value.occupied);

            }

            // if options available

            if (options.Count > 0)
            {

                foreach (Vector2 warppoint in options)
                {

                    // avoid if another character got there first

                    if (occupied.Contains(warppoint)) { continue; }

                    //Mod.instance.characters[trackFor].Position = warppoint*64;

                    //Mod.instance.characters[trackFor].occupied = warppoint;

                    CharacterMover mover = new(trackFor);

                    mover.WarpSet(followPlayer.currentLocation.Name, warppoint * 64, true);

                    Mod.instance.movers[trackFor] = mover;

                    Mod.instance.characters[trackFor].attentionTimer = 360;

                    warpDelay = 30;

                    return true;

                }

            }

            return false;

        }


        public Dictionary<Vector2,int> NodesToTraversal()
        {

            // first get the closest vector on path from termination

            if(nodes.Count == 0)
            {

                return new();

            }

            List<Vector2> paths = new() { nodes.Keys.Last(), };

            Vector2 origin = Mod.instance.characters[trackFor].occupied;

            int direct = ModUtility.DirectionToTarget(origin, nodes.Keys.Last())[2];

            List<int> accept = new()
            {
                direct,
                (direct + 1) % 8,
                (direct + 7) % 8,
            };

            float threshold = Vector2.Distance(origin, nodes.Keys.Last());

            if (nodes.Count > 1)
            {
                
                for (int n = nodes.Count - 2; n >= 0; n--)
                {

                    KeyValuePair<Vector2, int> node = nodes.ElementAt(n);

                    float closer = Vector2.Distance(origin, node.Key);

                    int way = ModUtility.DirectionToTarget(origin, node.Key)[2];

                    if (!accept.Contains(way))
                    {

                        break;

                    }

                    if (closer > threshold)
                    {

                        break;

                    }

                    paths.Prepend(node.Key);

                }

            }

            return ModUtility.PathsToTraversal(Mod.instance.characters[trackFor].currentLocation, paths, nodes, 2);

        }

    }

}
