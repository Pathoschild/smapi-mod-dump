/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            //pass SMAPI utilities to the Utility class for global use
            Utility.Monitor.IMonitor = Monitor;
            Utility.Helper = helper;
            Utility.Manifest = ModManifest;

            Utility.LoadModConfig(); //attempt to load the config.json ModConfig file

            if (Utility.MConfig?.EnableConsoleCommands == true) //if enabled, pass the mod's console command methods to the helper
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                helper.ConsoleCommands.Add("list_monsters", "Outputs a list of available monster types, including custom types loaded by other mods.", ListMonsters);
                helper.ConsoleCommands.Add("remove_items", "Removes an item or object in front of the player.\nUse \"remove_items X Y\" to remove an item from a specific tile.\nUse \"remove_items permanent\" to remove any FTM items from your location that cannot be removed normally (due to the \"CanBePickedUp\" setting).", RemoveItems);
            }

            AddSMAPIEvents(helper); //pass any necessary event methods to SMAPI
            ApplyHarmonyPatches(); //pass any necessary patches to Harmony
        }
    }
}