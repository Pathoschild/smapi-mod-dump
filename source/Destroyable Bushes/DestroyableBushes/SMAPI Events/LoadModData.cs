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
        private void LoadModData(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer) //if this is the main player, not a multiplayer farmhand
            {
                try
                {
                    Data = Helper.Data.ReadSaveData<ModData>(ModManifest.UniqueID) ?? new ModData(); //load this mod's data from the player's save file (or if none exists, create a new instance)
                }
                catch (Exception ex)
                {
                    Instance.Monitor.Log($"Error loading this mod's save data:\n{ex.ToString()}", LogLevel.Error);
                    Instance.Monitor.Log($"Previously destroyed bushes might not regrow while this error persists.", LogLevel.Error);
                }
            }
        }
    }
}
