/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
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
using StardewValley.TerrainFeatures;
using Harmony;

namespace DestroyableBushes
{
    public partial class ModEntry : Mod
    {
        private void SaveModData(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer) //if this is the main player, not a multiplayer farmhand
            {
                try
                {
                    if (Data != null) //if any save data exists
                    {
                        Helper.Data.WriteSaveData<ModData>(ModManifest.UniqueID, Data); //write this mod's data to the player's save file
                    }
                }
                catch (Exception ex)
                {
                    Instance.Monitor.Log($"Error saving this mod's save data:\n{ex.ToString()}", LogLevel.Error);
                    Instance.Monitor.Log($"Previously destroyed bushes might not regrow while this error persists.", LogLevel.Error);
                }
            }
        }
    }
}
