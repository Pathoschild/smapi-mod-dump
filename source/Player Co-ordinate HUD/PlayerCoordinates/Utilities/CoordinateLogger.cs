/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/PlayerCoordinates
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;

// A lot of refactoring of *everything* here is necessary for me to be happy with this, but given this
// isn't meant to be extended upon, it's okay for now.
namespace PlayerCoordinates.Utilities
{
    public class CoordinateLogger
    {
        // Strictly speaking, this doesn't need to be a list. Consider just using a string?
        // I anticipate no problems with it being a list, though. Definitely consider it, though,
        // given how 32-bit SDV can runs into memory issues for some people.
        private List<string> _fileContents = new List<string>();

        private readonly FileInfo _fileInfo;
        private readonly Coordinates _coordsToAdd;
        private readonly string _mapName;
        private readonly IMonitor _monitor;

        private bool _logTrackingTarget;
        private bool _trackingCursor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Full file path</param>
        /// <param name="coordinates">Co-ordinates to log</param>
        /// <param name="mapName">Map name</param>
        /// <param name="monitor">SMAPI logger</param>
        public CoordinateLogger(string fileName, Coordinates coordinates, string mapName, bool trackingCursor,
                                bool logTrackingTarget, IMonitor monitor)
        {
            _fileInfo = new FileInfo(fileName);
            _coordsToAdd = new Coordinates(coordinates.x, coordinates.y);
            _mapName = mapName;
            _monitor = monitor;
            _trackingCursor = trackingCursor;
            _logTrackingTarget = logTrackingTarget;
        }

        public bool LogCoordinates()
        {
            return SaveCoords();
        }

        private bool SaveCoords()
        {
            // Best to fail safe, and not try to write the co-ordinates to file if either loading the file, or adding
            // them to the list fails.
            try
            {
                LoadCoordsFromFile();
                AddNewCoords();
            }
            catch (Exception e)
            {
                Logger.LogException(_monitor, e);

                return false;
            }

            try
            {
                WriteCoordsToFile();
            }
            catch (Exception e)
            {
                Logger.LogException(_monitor, e);

                return false;
            }

            return true;
        }

        private void AddNewCoords()
        {
            // Add new co-ordinates to the existing file contents, if any.
            _fileContents.Add($"Map: {_mapName}");
            if (_logTrackingTarget) // If the player doesn't want to log the co-ordinate source, we needn't do anything!
                _fileContents.Add($"Tracking: {(_trackingCursor ? "Cursor" : "Player")}");
            _fileContents.Add($"Tile X: {_coordsToAdd.x}, Tile Y: {_coordsToAdd.y}");
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