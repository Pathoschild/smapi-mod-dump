/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StardewModdingAPI;

namespace PlayerCoordinates
{
    public class FileHandler
    {
        private List<string> _fileContents = new List<string>();

        private FileInfo _fileInfo;
        private Coordinates _coordsToAdd;
        private string _mapName;

        private IMonitor _monitor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Full file path</param>
        /// <param name="coordinates">Co-ordinates to log</param>
        /// <param name="mapName">Map name</param>
        /// <param name="monitor">SMAPI logger</param>
        public FileHandler(string fileName, Coordinates coordinates, string mapName, IMonitor monitor)
        {
            _fileInfo = new FileInfo(fileName);
            _coordsToAdd = new Coordinates(coordinates.x, coordinates.y);
            _mapName = mapName;
            _monitor = monitor;

            try
            {
                SaveCoords();
            }
            catch (Exception e)
            {
                _monitor.Log($"Exception: {e.Message}.", LogLevel.Error);
                _monitor.Log($"{e.Data}.", LogLevel.Error);
            }
        }

        private void SaveCoords()
        {
            LoadCoordsFromFile();
            AddNewCoords();
            WriteCoordsToFile();
        }

        private void AddNewCoords()
        {
            // Add new co-ordinates to the existing file contents, if any.
            _fileContents.Add($"Map: {_mapName}");
            _fileContents.Add($"X: {_coordsToAdd.x}, Y: {_coordsToAdd.y}");
        }

        private void LoadCoordsFromFile()
        {
            // If the file already exists, load the contents into our list. 
            List<string> oldFileContents = new List<string>();

            if (_fileInfo.Exists)
            {
                string[] contents = File.ReadAllLines(_fileInfo.FullName);

                foreach (string s in contents)
                {
                    oldFileContents.Add(s);
                }
            }

            _fileContents.Clear();
            _fileContents = oldFileContents;
        }

        private void WriteCoordsToFile()
        {
            StreamWriter fileWriter = new StreamWriter(_fileInfo.FullName);

            foreach (string s in _fileContents)
            {
                fileWriter.WriteLine(s);
            }
            
            fileWriter.WriteLine();
            fileWriter.Close();
        }
    }
}