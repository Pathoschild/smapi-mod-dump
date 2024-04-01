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
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;

namespace StardewDruid.Character
{
    public class TrackHandle
    {
        public string trackFor;
        
        public string trackLocation;

        public Farmer followPlayer;
        
        public Vector2 trackPlayer;
        
        public List<Vector2> trackVectors;
        
        public int trackLimit;

        public bool standby;

        public Vector2 trackOffset;

        public Vector2 trackDelay;

        public TrackHandle(string For, Farmer follow = null)
        {
            
            if(follow == null)
            {
                
                followPlayer = Game1.player;

            }
            else
            {

                followPlayer = follow;

            }

            trackVectors = new List<Vector2>();
            
            trackPlayer = new Vector2(-1);
            
            trackLimit = 24;
           
            trackFor = For;

        }

        public void TrackPlayer()
        {

            if (trackLocation != followPlayer.currentLocation.Name)
            {

                trackLocation = followPlayer.currentLocation.Name;
                
                trackPlayer = new Vector2(-1);
                
                trackVectors.Clear();
            
            }

            if (followPlayer.currentLocation is FarmHouse || followPlayer.currentLocation is IslandFarmHouse)
            {

                return;

            }

            Vector2 position = followPlayer.Position;

            if(ModUtility.GroundCheck(followPlayer.currentLocation, followPlayer.Tile) == "water")
            {
                
                return;
            
            }

            int offset = 0;

            foreach (KeyValuePair<string, TrackHandle> tracker in Mod.instance.trackRegister)
            {

                if (tracker.Key == trackFor)
                {

                    break;

                }

                offset++;

            }

            if ((double)Vector2.Distance(position, trackPlayer) >= 64.0)
            {

                if(trackPlayer != new Vector2(-1))
                {

                    trackVectors.Add(trackPlayer);

                    if (trackVectors.Count >= trackLimit)
                    {

                        trackVectors.RemoveAt(0);

                    }

                }

                trackPlayer = position - (new Vector2(32,32) * offset);

            }

            if (trackVectors.Count < (1 + offset))
            {

                return;

            }

            if(trackVectors.Count > 6)
            {

                if(Vector2.Distance(trackVectors.First(),trackVectors.Last()) < 192)
                {

                    TruncateTo(3);

                }

            }

            if (Mod.instance.characters[trackFor].netSceneActive.Value)
            {

                return;

            }

            if(trackVectors.Count == 0)
            {

                return;

            }

            if (Mod.instance.characters[trackFor].currentLocation.Name != followPlayer.currentLocation.Name)
            {

                WarpToTarget();

            }

            if (!Utility.isOnScreen(Mod.instance.characters[trackFor].Position, 128))
            {

                WarpToTarget();

            }

            if(Vector2.Distance(Mod.instance.characters[trackFor].Position, followPlayer.Position) > 800f)
            {

                WarpToTarget();

            }

        }

        public void WarpToTarget()
        {

            if(trackVectors.Count == 0)
            {
                return;
            }

            if (Mod.instance.characters[trackFor].currentLocation.Name != followPlayer.currentLocation.Name)
            {

                Mod.instance.characters[trackFor].currentLocation.characters.Remove(Mod.instance.characters[trackFor]);

                Mod.instance.characters[trackFor].currentLocation = followPlayer.currentLocation;

                Mod.instance.characters[trackFor].currentLocation.characters.Add(Mod.instance.characters[trackFor]);

            }

            Mod.instance.characters[trackFor].Position = NextVector();

            ModUtility.AnimateQuickWarp(Mod.instance.characters[trackFor].currentLocation, Mod.instance.characters[trackFor].Position - new Vector2(0, 32));

            Mod.instance.characters[trackFor].DeactivateStandby();

            Mod.instance.characters[trackFor].ResetActives();

        }


        public void TruncateTo(int requirement)
        {
            
            int num = Math.Min(requirement, trackVectors.Count);
            
            List<Vector2> vector2List = new List<Vector2>();
            
            for (int index = trackVectors.Count - num; index < trackVectors.Count; ++index)
            {
                
                vector2List.Add(trackVectors[index]);
            
            }
            
            trackVectors = vector2List;

        }

        public Vector2 NextVector()
        {
            
            if (trackVectors.Count == 0)
            {
                return Vector2.Zero;
            }

            Vector2 trackVector = trackVectors[0];
            
            trackVectors.RemoveAt(0);

            return trackVector;
        
        }

        public Vector2 LastVector()
        {

            Vector2 trackVector = trackVectors.Last();

            trackVectors.Clear();

            return trackVector;

        }
    
    }

}
