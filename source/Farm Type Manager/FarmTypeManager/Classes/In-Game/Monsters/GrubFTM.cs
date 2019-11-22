using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using FarmTypeManager.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's Grub class, adjusted for use by this mod.</summary>
        class GrubFTM : Grub
        {
            /// <summary>Creates an instance of Stardew's Grub class, but with adjustments made for this mod.</summary>
            public GrubFTM()
                : base()
            {

            }

            /// <summary>Creates an instance of Stardew's Grub class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            /// <param name="hard">If true, this grub will be the Mutant Grub subtype.</param>
            public GrubFTM(Vector2 position, bool hard)
                : base(position, hard)
            {

            }

            //this override caues GrubFTM to spawn FlyFTM, rather than the base game's Fly
            public override void behaviorAtGameTick(GameTime time)
            {
                base.behaviorAtGameTick(time); //perform Grub's normal method

                //if this spawns a Fly, replace it with a FlyFTM (note: this method is used to avoid overriding the entire method & working around readonly object fields)
                if (Health == -500 && currentLocation.characters[currentLocation.characters.Count - 1] is Fly oldFly)
                {
                    int ID = Utility.RNG.Next(int.MinValue, -1); //generate a random ID for saving purposes (note: the ID is below -1 to avoid matching any known NPC values set by base game functions)
                    FlyFTM newFly = new FlyFTM(oldFly.Position, oldFly.hard) //make a replacement fly of the correct subclass
                    {
                        currentLocation = oldFly.currentLocation, //set its current location
                        id = ID //assign the ID to it
                    };
                    currentLocation.characters[currentLocation.characters.Count - 1] = newFly; //replace the old fly with the new one

                    SavedObject save = new SavedObject(currentLocation.Name, Position, SavedObject.ObjectType.Monster, ID, null, 1); //create save data for the new fly (set to expire overnight) 

                    if (Context.IsMainPlayer) //if this method was run by the host player
                    {
                        Utility.FarmDataList[0].Save.SavedObjects.Add(save); //store the save data in the first listed FarmData
                    }
                    else //if this method was run by a client player (farmhand)
                    {
                        //send a message to the host player to process this save data
                        Utility.Helper.Multiplayer.SendMessage<SavedObject>(save, "SavedObject", new string[] { Utility.Helper.ModRegistry.ModID }, new long[] { Game1.MasterPlayer.UniqueMultiplayerID });
                    }
                }
            }
        }
    }
}
