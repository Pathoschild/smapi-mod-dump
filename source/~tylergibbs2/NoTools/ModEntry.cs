/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NoTools
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveCreated += OnSaveCreated;

            // Activate the Harmony patch
            Harmony harmonyInstance = new(helper.ModRegistry.ModID);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
        {
            Game1.player.clearBackpack();
        }
    }
}
