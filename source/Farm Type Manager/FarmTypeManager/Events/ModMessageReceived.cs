/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed when a mod message is received.</summary>
        private void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            if (e.FromModID != ModManifest.UniqueID) { return; } //if this message is from another mod, don't do anything

            //handle SavedObject messages
            if (e.Type.Equals("SavedObject", StringComparison.OrdinalIgnoreCase))
            {
                SavedObject save = e.ReadAs<SavedObject>(); //get the saved object

                /*
                 * NOTE: This is a messy solution to saving objects that weren't generated directly by a FarmConfig, which means they aren't associated with a SavedObjects list.
                 *       For now, the objects will be added to the first available FarmData entry and forced to expire overnight (preventing them actually being written to a file).
                 */
                
                save.DaysUntilExpire = 1; //set the object to expire overnight
                Utility.FarmDataList[0].Save.SavedObjects.Add(save); //store it in the first listed FarmData
            }
        }
    }
}
