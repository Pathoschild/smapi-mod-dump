/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.IO;
using IOPath = System.IO.Path;

namespace Randomizer
{
	/// <summary>
	/// Used to log the randomization so players can see what was done
	/// </summary>
	public class SpoilerLogger
	{
		/// <summary>
		/// The path to the spoiler log
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// The text to write to the spoiler log - it's best to write a big block of text at once
		/// rather than several small ones to avoid the overhead of opening and disposing the file
		/// </summary>
		public string TextToWrite { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="farmName">The name of the farm - used to easily identify the log</param>
		public SpoilerLogger(string farmName)
		{
			if (!Globals.Config.CreateSpoilerLog) { return; }

			var spoilerLogName = string.Join("", farmName.Split(IOPath.GetInvalidFileNameChars()));
			Path = Globals.GetFilePath($"SpoilerLog-{spoilerLogName}.txt");
			File.Create(Path).Close();
		}

		/// <summary>
		/// Adds a line to the buffer
		/// </summary>
		/// <param name="line">The line</param>
		public void BufferLine(string line)
		{
			TextToWrite += $"{line}{Environment.NewLine}";
		}

		/// <summary>
		/// Writes a line to the end of the file
		/// </summary>
		/// <param name="line">The line</param>
		public void WriteFile()
		{
			if (!Globals.Config.CreateSpoilerLog)
			{
				TextToWrite = "";
				return;
			}

			using StreamWriter file = new(Path, true);
			file.WriteLine(TextToWrite);
		}
	}
}
