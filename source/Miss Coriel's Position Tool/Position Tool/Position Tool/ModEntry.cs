/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Position-Tool
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Microsoft.Xna.Framework;
using System.IO;

namespace Position_Tool
{
    public class ModEntry : Mod
    {
        List<string> taggedLocations = new List<string>();

        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add("addhere", "usage: adds the mapname and coordinates of the farmer's position", AddLocationCommand);
            helper.ConsoleCommands.Add("saveas", "'useage: saveas <name>' Saves a text file with all coordinates and locations.", SaveFileCommand);
        }
        private void AddLocationCommand(string command, string[] parameters)
        {
            string rawLocation = Game1.player.currentLocation.ToString();
            string mapLocation = rawLocation.Substring(rawLocation.LastIndexOf('.') + 1);
            int mapCoordX = (int)Game1.player.getTileX();
            int mapCoordY = (int)Game1.player.getTileY();
            string position = $"{mapLocation}: {mapCoordX}, {mapCoordY}";
            taggedLocations.Add(position);
            this.Monitor.Log($"Added {position} to list!", LogLevel.Debug);
        }
        private void SaveFileCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                if(!Directory.Exists(Environment.CurrentDirectory + "\\Coordinates"))
                {
                    Directory.CreateDirectory("Coordinates");
                }

                string fileName = parameters[0];
                File.WriteAllLines($"Coordinates/{fileName}.txt", GetCoords(taggedLocations));
                this.Monitor.Log($"{fileName}.txt has been created.  Check your Stardew Valley folder.", LogLevel.Info);
            }
            catch (Exception ex) { this.Monitor.Log(ex.Message, LogLevel.Warn); }
        }
        private IEnumerable<string> GetCoords(List<string> list)
        {
            foreach (object entry in list)
                yield return entry.ToString();
        }
    }
}
