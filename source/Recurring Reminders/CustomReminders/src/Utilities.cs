/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dem1se/SDVMods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Dem1se.CustomReminders.Utilities
{
    /// <summary>Contains some useful functions that other classes use.</summary>
    static class Extras
    {
        /// <summary>
        /// Makes platform sensitive paths that start from the Content.ModFolder instead of the local disk from absoulute paths.
        /// </summary>
        /// <param name="filePathAbsolute">The file path from the directory enumerator</param>
        /// <param name="monitor">The SMAPI Monitor for logging purposed within the function</param>
        /// <returns>Relative path that starts from mod folder instead of full fs path.</returns>
        public static string MakeRelativePath(string filePathAbsolute)
        {
            // Make relative path from absolute path
            string filePathRelative = "";
            string[] filePathRelativeParts;
            int filePathIndex;
            char delimiter;
            if (Constants.TargetPlatform.ToString() == "Windows")
                delimiter = '\\';
            else if (Constants.TargetPlatform.ToString() == "Mac" || Constants.TargetPlatform.ToString() == "Linux")
                delimiter = '/';
            else
                throw new NotSupportedException("Operating system is not Windows, Mac or Linux. Unsupported Platform");

            filePathRelativeParts = filePathAbsolute.Split(delimiter);
            filePathIndex = Array.IndexOf(filePathRelativeParts, "data");
            for (int i = filePathIndex; i < filePathRelativeParts.Length; i++)
            {
                filePathRelative += filePathRelativeParts[i] + delimiter;
            }
            // Remove the trailing slash in Relative path
            filePathRelative = filePathRelative.Remove(filePathRelative.LastIndexOf(delimiter));
            return filePathRelative;
        }

        /// <summary>
        /// Estimates the amount of pixels a string will be wide.
        /// </summary>
        /// <param name="reminderMessage">The string to estimate for</param>
        /// <returns>{int} The pixel count of estimated witdth the string would take</returns>
        public static int EstimateStringDimension(string reminderMessage)
        {
            return StardewValley.BellsAndWhistles.SpriteText.getWidthOfString(reminderMessage) + 64;
        }
    }

    /// <summary>Contains methods related to parsing/converting formats for data of this mod.</summary>
    static class Convert
    {
        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ToDaysSinceStart(int date, int season, int year)
        {
            return (season * 28) + ((year - 1) * 112) + date;
        }

        /// <summary>
        /// Converts DaysSinceStart to pretty date format (Season Day)
        /// </summary>
        /// <param name="daysSinceStart">The DaysSinceStart of the date to convert</param>
        /// <returns></returns>
        public static string ToPrettyDate(int daysSinceStart)
        {
            int remainderAfterYears = daysSinceStart % 112;
            int years = (daysSinceStart - remainderAfterYears) / 112;
            int day = remainderAfterYears % 28;
            if (day == 0)
                day = 28;
            int months = (remainderAfterYears - day) / 28;
            Season month = (Season)months;
            // this is a special case (winter 28)
            if (months == -1)
            {
                month = (Season)3;
                years--;
            }
            string monthStr;
            switch (month)
            {
                case Season.Spring:
                    monthStr = Globals.Helper.Translation.Get("date.season.spring");
                    break;
                case Season.Summer:
                    monthStr = Globals.Helper.Translation.Get("date.season.summer");
                    break;
                case Season.Fall:
                    monthStr = Globals.Helper.Translation.Get("date.season.fall");
                    break;
                case Season.Winter:
                    monthStr = Globals.Helper.Translation.Get("date.season.winter");
                    break;
                default:
                    monthStr = "";
                    Globals.Monitor.Log("Season translation failed.", LogLevel.Error);
                    break;
            }

            return $"{monthStr} {day}, {Globals.Helper.Translation.Get("date.year")} {years + 1}";
        }


        /// <summary>
        /// Converts the 24hrs time int to 12hrs string
        /// </summary>
        /// <param name="timeIn24"></param>
        /// <returns></returns>
        public static string ToPrettyTime(int timeIn24)
        {
            string prettyTime;
            if (timeIn24 <= 1230) // Pre noon
            {
                prettyTime = System.Convert.ToString(timeIn24);
                if (prettyTime.EndsWith("00")) // ends with 00
                {
                    prettyTime = timeIn24 <= 930 ? prettyTime.Remove(1) : prettyTime.Remove(2);
                    if (prettyTime.StartsWith("0"))
                        prettyTime = prettyTime.Replace("0", " ");
                    prettyTime = prettyTime.Trim();
                    prettyTime += " AM";
                }
                else // ends with 30
                {
                    prettyTime = prettyTime.Replace("30", ":30");
                    if (prettyTime.StartsWith("0"))
                        prettyTime = prettyTime.Replace("0", " ");
                    prettyTime = prettyTime.Trim();
                    prettyTime += " AM";
                    if (timeIn24 == 1230)
                        prettyTime = prettyTime.Replace("AM", "PM");
                }
            }
            else // after noon
            {
                prettyTime = System.Convert.ToString(timeIn24 - 1200);
                if (prettyTime.EndsWith("00")) // ends with 00
                {
                    if (prettyTime == "1000")
                        prettyTime = prettyTime.Remove(2);
                    else
                        prettyTime = prettyTime.Replace("00", " ");

                    if (prettyTime.StartsWith("0"))
                        prettyTime = prettyTime.Replace("0", " ");
                    prettyTime = prettyTime.Trim();
                    prettyTime += " PM";
                }
                else // ends with 30
                {
                    prettyTime = prettyTime.Replace("30", ":30");
                    if (prettyTime.StartsWith("0"))
                        prettyTime = prettyTime.Replace("0", " ");
                    prettyTime = prettyTime.Trim();
                    prettyTime += " PM";
                }
            }
            return prettyTime;
        }
    }

    /// <summary>Contains methods related to reading and writing to files for this mod.</summary>
    static class File
    {
        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="reminderMessage">The message that will pop up in reminder</param>
        /// <param name="daysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="time">The time of the reminder in 24hrs format</param>
        public static void Write(string reminderMessage, int daysSinceStart, int time, int interval)
        {
            ReminderModel ReminderData = new(reminderMessage, daysSinceStart, time, interval);
            string pathToWrite = Path.Combine(Globals.Helper.DirectoryPath, "data", Globals.SaveFolderName);
            string serializedReminderData = JsonConvert.SerializeObject(ReminderData, Formatting.Indented);
            int reminderCount = 0;
            bool bWritten = false;
            while (!bWritten)
            {
                if (!System.IO.File.Exists(Path.Combine(pathToWrite, $"reminder_{daysSinceStart}_{time}_{reminderCount}.json")))
                {
                    System.IO.File.WriteAllText(Path.Combine(pathToWrite, $"reminder_{daysSinceStart}_{time}_{reminderCount}.json"), serializedReminderData);
                    bWritten = true;
                }
                else
                    reminderCount++;
            }
        }

        /// <summary>
        /// Deletes the reminder of the specified index. Index being the serial position of reminder file in directory.
        /// </summary>
        /// <param name="reminderIndex">Zero-indexed serial position of reminder file in directory</param>
        /// <param name="Helper">IModHelper instance for its fields</param>
        public static void DeleteReminder(int reminderIndex)
        {
            int iterationIndex = 1;
            foreach (string path in Directory.EnumerateFiles(Path.Combine(Globals.Helper.DirectoryPath, "data", Globals.SaveFolderName)))
            {
                if (reminderIndex == iterationIndex)
                {
                    System.IO.File.Delete(path);
                    iterationIndex++;
                }
                else
                    iterationIndex++;
            }
        }

        /// <summary>
        /// Checks if data subfolder exists for given saveFolderName, creates dir if not.
        /// Make sure, Globals.SaveFolderName is initialized before calling it. Will throw NullReferencesExceptions otherwise.
        /// </summary>
        public static void CreateDataSubfolder()
        {
            if (!Directory.Exists(Path.Combine(Globals.Helper.DirectoryPath, "data", Globals.SaveFolderName)))
            {
                Globals.Monitor.Log($"Reminders directory({Globals.SaveFolderName}) not found. Creating directory.", LogLevel.Info);
                Directory.CreateDirectory(Path.Combine(Globals.Helper.DirectoryPath, "data", Globals.SaveFolderName));
                Globals.Monitor.Log($"Reminders directory({Globals.SaveFolderName}) created successfully.", LogLevel.Info);
            }
        }
    }

    /// <summary>Contains data values that are used across classes and namespaces</summary>
    static class Globals
    {
        /// <summary>IModHelper instance for classes to access without needing it be an parameter everywhere.</summary>
        internal static IModHelper Helper;

        /// <summary>IMonitor instance for classes to access without needing it be a parameter everywhere.</summary>
        internal static IMonitor Monitor;

        /// <summary>
        /// Contains the save folder name. Added especially for mulitplayer support, but reqiured in all cases now.
        /// Host generates own value, peers recieve value from host.
        /// </summary>
        public static string SaveFolderName;

        /// <summary>The menu button of the player, required for suppressing</summary>
        public static SButton MenuButton;

        public static IManifest ModManifest;

        /// <summary>
        /// Returns the button that is set to open the menu in current save.
        /// </summary>
        /// <returns>the button that opens menu as SButton</returns>
        public static SButton GetMenuButton()
        {
            if (Context.IsMainPlayer)
            {
                var saveFile = XDocument.Load(Path.Combine(Constants.CurrentSavePath, Constants.SaveFolderName));
                var query = from xml in saveFile.Descendants("menuButton")
                            select xml.Element("InputButton").Element("key").Value;
                string menuButtonString = "";
                foreach (string key in query)
                {
                    menuButtonString = key;
                }
                SButton menuButton = (SButton)Enum.Parse(typeof(SButton), menuButtonString);
                return menuButton;
            }
            else
            {
                // default just in case.
                // assigned in OnSaveLoaded() in CustomReminders.cs
                return SButton.E;
            }
        }
    }

    enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
}
