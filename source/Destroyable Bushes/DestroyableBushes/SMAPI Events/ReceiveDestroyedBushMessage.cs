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
