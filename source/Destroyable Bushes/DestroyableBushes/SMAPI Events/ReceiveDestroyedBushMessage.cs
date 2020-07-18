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
        private void ReceiveDestroyedBushMessage(object sender, ModMessageReceivedEventArgs e)
        {
            if (Context.IsMainPlayer) //if this is the main player, not a multiplayer farmhand
            {
                try
                {
                    if (e.FromModID == ModManifest.UniqueID && e.Type == "DestroyedBush") //if this is a destroyed bush message
                        Data?.DestroyedBushes.Add(e.ReadAs<ModData.DestroyedBush>()); //add this destroyed bush to the list
                }
                catch (Exception ex)
                {
                    Instance.Monitor.Log($"Error saving a bush destroyed by a multiplayer farmhand:\n{ex.ToString()}", LogLevel.Error);
                    Instance.Monitor.Log($"Bushes destroyed by multiplayer farmhands might not regrow while this error persists.", LogLevel.Error);
                }
            }
        }
    }
}
