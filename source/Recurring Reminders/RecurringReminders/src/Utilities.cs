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

namespace Dem1se.RecurringReminders.Utilities
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
            int width = 0;
            char[] characters = reminderMessage.ToCharArray();
            // keep adding arbitrary pixel values
            // add time number
            switch (characters[1])
            {
            case ' ':
                // 1 digit time
                width += 20;
                break;
            case ':':
                // 3 digit time
                width += 80;
                break;
            default:
                if (characters[2] == ':')
                    // 4 digit time
                    width += 100; // 20 extra for colon
                else
                    // 2 digit time
                    width += 40;
                break;
            }


            // add space
            width += 24;
            // add AM/PM
            width += 68;

            width += 255;
            // add space
            width += 24;
            // add two digits
            width += 40;
            // add year
            width += 156;
            // add two spaces
            width += 48;
            return width;
        }
    }

    /// <summary>Contains methods related to parsing/converting formats for data of this mod.</summary>
    static class Converts
    {
        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ConvertToDays(int date, int season, int year)
        {
            return (season * 28) + ((year - 1) * 112) + date;
        }

        /// <summary>
        /// Converts DaysSinceStart to pretty date format (Season Day)
        /// </summary>
        /// <param name="daysSinceStart">The DaysSinceStart of the date to convert</param>
        /// <returns>A string formatted like "Summer 12, Year 3" </returns>
        public static string ConvertToPrettyDate(int daysSinceStart)
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
            return $"{month} {day}, Year {years + 1}";
        }

        /// <summary>
        /// Converts the 24hrs time int to 12hrs string
        /// </summary>
        /// <param name="timeIn24"></param>
        /// <returns></returns>
        public static string ConvertToPrettyTime(int timeIn24)
        {
            string prettyTime;
            if (timeIn24 <= 1230) // Pre noon
            {
                prettyTime = Convert.ToString(timeIn24);
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
                prettyTime = Convert.ToString(timeIn24 - 1200);
                if (prettyTime.EndsWith("00")) // ends with 00
                {
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
    static class Files
    {
        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="reminderMessage">The message that will pop up in reminder</param>
        /// <param name="daysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="time">The time of the reminder in 24hrs format</param>
        public static void WriteToFile(string reminderMessage, int reminderStartDate, int daysInterval, int time)
        {
            RecurringReminderModel ReminderData = new RecurringReminderModel(reminderMessage, reminderStartDate, daysInterval, time);
            string pathToWrite = Path.Combine(Data.Helper.DirectoryPath, "data", Data.SaveFolderName);
            string serializedReminderData = JsonConvert.SerializeObject(ReminderData, Formatting.Indented);
            int reminderCount = 0;
            bool bWritten = false;
            while (!bWritten)
            {
                if (!File.Exists(Path.Combine(pathToWrite, $"reminder_{reminderStartDate}_{time}_{reminderCount}.json")))
                {
                    File.WriteAllText(Path.Combine(pathToWrite, $"reminder_{reminderStartDate}_{time}_{reminderCount}.json"), serializedReminderData);
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
            foreach (string path in Directory.EnumerateFiles(Path.Combine(Data.Helper.DirectoryPath, "data", Data.SaveFolderName)))
            {
                if (reminderIndex == iterationIndex)
                {
                    File.Delete(path);
                    iterationIndex++;
                }
                else
                    iterationIndex++;
            }
        }
    }

    /// <summary>Contains data values that are used across classes and namespaces</summary>
    static class Data
    {
        /// <summary>IModHelper instance for classes to access without need it be an parameter everywhere.</summary>
        /// <remarks>Assigned in SetUpStatics() in CustomReminders.cs</remarks>
        public static IModHelper Helper;

        /// <summary>IMonitor instance for classes to access without need it be a parameter everywhere.</summary>
        /// <remarks>Assigned in SetUpStatics() in CustomReminders.cs</remarks>
        public static IMonitor Monitor;

        /// <summary>
        /// <para>
        /// Contains the save folder name for mulitplayer support.
        /// Host generates own value, peers recieve value from host.
        /// </para>
        /// <para>
        /// This is a critical field, and will cause multiple exceptions across namespaces and classes if null. 
        /// </para>
        /// </summary>
        public static string SaveFolderName;

        /// <summary>The menu button of the player, required for suppressing</summary>
        public static SButton MenuButton;

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
