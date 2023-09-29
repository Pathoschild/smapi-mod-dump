/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using FarmTypeManager.Monsters;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's Grub class, adjusted for use by this mod.</summary>
        public class GrubFTM : Grub
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
                if (Health == -500 && currentLocation.characters.Count > 0 && currentLocation.characters[currentLocation.characters.Count - 1] is Fly oldFly)
                {
                    int ID = Utility.RNG.Next(int.MinValue, -1); //generate a random ID for saving purposes (note: the ID is below -1 to avoid matching any known NPC values set by base game functions)
                    FlyFTM newFly = new FlyFTM(oldFly.Position, oldFly.hard) //make a replacement fly of the correct subclass
                    {
                        currentLocation = oldFly.currentLocation, //set its current location
                        id = ID //assign the ID to it
                    };
                    currentLocation.characters[currentLocation.characters.Count - 1] = newFly; //replace the old fly with the new one

                    SavedObject save = new SavedObject() //create save data for the new fly (set to expire overnight) 
                    {
                        MapName = currentLocation.Name,
                        Tile = Position,
                        Type = SavedObject.ObjectType.Monster,
                        ID = ID,
                        DaysUntilExpire = 1
                    };

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
