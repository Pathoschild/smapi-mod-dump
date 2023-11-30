/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewDruid.Character
{
    public class TrackHandle
    {

        public string trackFor;

        public string trackLocation;

        public Vector2 trackPlayer;

        public List<Vector2> trackVectors;

        public int trackLimit;

        public TrackHandle(string For)
        {

            trackVectors = new();

            trackPlayer = new Vector2(-99);

            trackLimit = 24;

            trackFor = For;

        }

        public void TrackPlayer()
        {

            if(trackLocation != Game1.player.currentLocation.Name)
            {

                trackLocation = Game1.player.currentLocation.Name;

                trackPlayer = new Vector2(-99);

                trackVectors.Clear();

            }

            Vector2 playerPosition = Game1.player.Position;

            if (Vector2.Distance(playerPosition, trackPlayer) >= 64f)
            {

                trackPlayer = playerPosition;

                trackVectors.Add(playerPosition);

                if(trackVectors.Count >= trackLimit)
                {

                    trackVectors.RemoveAt(0);

                }

            }

            if (Mod.instance.characters[trackFor].currentLocation.Name != Game1.player.currentLocation.Name && trackVectors.Count >= 3)
            {

                Mod.instance.characters[trackFor].WarpToTarget();

            }

        }


        public void TruncateTo(int requirement)
        {

            int truncate = Math.Min(requirement, trackVectors.Count);

            List<Vector2> newList = new();

            for(int i = trackVectors.Count - truncate; i < trackVectors.Count; i++)
            {

                newList.Add(trackVectors[i]);
            
            }

            trackVectors = newList;

        }

        public Vector2 NextVector()
        {

            Vector2 returnVector = trackVectors[0];

            trackVectors.RemoveAt(0);
            
            return returnVector;

        }


    }

}
