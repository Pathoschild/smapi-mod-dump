/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

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
