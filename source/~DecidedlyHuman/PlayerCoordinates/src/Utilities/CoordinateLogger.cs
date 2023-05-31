/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
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
        private readonly Coordinates _coordsToAdd;

        private readonly FileInfo _fileInfo;
        private readonly string _mapName;

        private readonly IMonitor _monitor;

        // Strictly speaking, this doesn't need to be a list. Consider just using a string?
        // I anticipate no problems with it being a list, though. Definitely consider it, though,
        // given how 32-bit SDV can runs into memory issues for some people.
        private List<string> _fileContents = new();

        private readonly bool _logTrackingTarget;
        private readonly bool _trackingCursor;

        /// <summary>
        /// </summary>
        /// <param name="fileName">Full file path</param>
        /// <param name="coordinates">Co-ordinates to log</param>
        /// <param name="mapName">Map name</param>
        /// <param name="monitor">SMAPI logger</param>
        public CoordinateLogger(string fileName, Coordinates coordinates, string mapName, bool trackingCursor,
            bool logTrackingTarget, IMonitor monitor)
        {
            this._fileInfo = new FileInfo(fileName);
            this._coordsToAdd = new Coordinates(coordinates.x, coordinates.y);
            this._mapName = mapName;
            this._monitor = monitor;
            this._trackingCursor = trackingCursor;
            this._logTrackingTarget = logTrackingTarget;
        }

        public bool LogCoordinates()
        {
            return this.SaveCoords();
        }

        private bool SaveCoords()
        {
            // Best to fail safe, and not try to write the co-ordinates to file if either loading the file, or adding
            // them to the list fails.
            try
            {
                this.LoadCoordsFromFile();
                this.AddNewCoords();
            }
            catch (Exception e)
            {
                Logger.LogException(this._monitor, e);

                return false;
            }

            try
            {
                this.WriteCoordsToFile();
            }
            catch (Exception e)
            {
                Logger.LogException(this._monitor, e);

                return false;
            }

            return true;
        }

        private void AddNewCoords()
        {
            // Add new co-ordinates to the existing file contents, if any.
            this._fileContents.Add($"Map: {this._mapName}");
            if (this._logTrackingTarget) // If the player doesn't want to log the co-ordinate source, we needn't do anything!
                this._fileContents.Add($"Tracking: {(this._trackingCursor ? "Cursor" : "Player")}");
            this._fileContents.Add($"Tile X: {this._coordsToAdd.x}, Tile Y: {this._coordsToAdd.y}");
        }

        private void LoadCoordsFromFile()
        {
            // If the file already exists, load the contents into our list. 
            var oldFileContents = new List<string>();

            if (this._fileInfo.Exists)
            {
                string[] contents = File.ReadAllLines(this._fileInfo.FullName);

                foreach (string s in contents) oldFileContents.Add(s);
            }

            this._fileContents.Clear();
            this._fileContents = oldFileContents;
        }

        private void WriteCoordsToFile()
        {
            var fileWriter = new StreamWriter(this._fileInfo.FullName);

            foreach (string s in this._fileContents) fileWriter.WriteLine(s);

            fileWriter.WriteLine();
            fileWriter.Close();
        }
    }
}
