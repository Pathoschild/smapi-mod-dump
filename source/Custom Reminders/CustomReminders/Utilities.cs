using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Dem1se.CustomReminders.Utilities
{
    /// <summary>
    /// Contains some useful functions that other classes use.
    /// </summary>
    static class Utilities
    {
        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="ReminderMessage">The message that will pop up in reminder</param>
        /// <param name="DaysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="Time">The time of the reminder in 24hrs format</param>
        public static void WriteToFile(string ReminderMessage, int DaysSinceStart, int Time, IModHelper Helper)
        {
            Random rnd = new Random();
            ReminderModel ReminderData = new ReminderModel()
            {
                DaysSinceStart = DaysSinceStart,
                ReminderMessage = ReminderMessage,
                Time = Time
            };

            string PathToWrite = Path.Combine(Helper.DirectoryPath, "data", Constants.SaveFolderName);
            string SerializedReminderData = JsonConvert.SerializeObject(ReminderData, Formatting.Indented);
            int ReminderCount = 0;
            do
            {
                ReminderCount = rnd.Next(1000, 10000);
                if (!File.Exists(Path.Combine(PathToWrite, $"reminder{ReminderCount}.json")))
                {
                    File.WriteAllText(Path.Combine(PathToWrite, $"reminder{ReminderCount}.json"), SerializedReminderData);
                    break;
                }
            } while (File.Exists(Path.Combine(PathToWrite, $"reminder{ReminderCount}.json")));
        }
        
        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ConvertToDays(int date, int season, int year)
        {
            int DaysSinceStart = (season * 28) + ((year - 1) * 112) + date;
            return DaysSinceStart;
        }

        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date">The date in int, 1 to 28</param>
        /// <param name="season">The season in int, where 1 is summer, ... , winter is 4</param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ConvertToDays(int date, int season)
        {
            int year = SDate.Now().Year;
            int DaysSinceStart = (season * 28) + ((year - 1) * 112) + date;
            return DaysSinceStart;
        }

        /// <summary>
        /// Returns the button that is set to open the menu in current save.
        /// </summary>
        /// <returns>the button that opens menu as SButton</returns>
        public static SButton GetMenuButton()
        {
            var SaveFile = XDocument.Load(Path.Combine(Constants.CurrentSavePath, Constants.SaveFolderName));
            var query = from xml in SaveFile.Descendants("menuButton")
                        select xml.Element("InputButton").Element("key").Value;
            string MenuString = "";
            foreach (string Key in query)
            {
                MenuString = Key;
            }
            SButton MenuButton = (SButton)Enum.Parse(typeof(SButton), MenuString);
            return MenuButton;
        }

        /// <summary>
        /// Makes platform sensitive relative paths from absoulute paths
        /// </summary>
        /// <param name="FilePathAbsolute">The file path from the directory enumerator</param>
        /// <param name="monitor">The SMAPI Monitor for logging purposed within the function</param>
        /// <returns>Relative path that starts from mod folder.</returns>
        public static string MakeRelativePath(string FilePathAbsolute, IMonitor monitor)
        {
            // Make relative path from absolute path
            string FilePathRelative = "";
            string[] FilePathAbsoulute_Parts;
            int FilePathIndex;

            // windows style
            if (Constants.TargetPlatform.ToString() == "Windows")
            {
                monitor.Log("Parsing the paths Windows style", LogLevel.Trace);
                FilePathAbsoulute_Parts = FilePathAbsolute.Split('\\');
                FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                {
                    FilePathRelative += FilePathAbsoulute_Parts[i] + "\\";
                }
                // Remove the trailing forward slash in Relative path
                FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("\\"));
            }
            //unix style
            else if (Constants.TargetPlatform.ToString() == "Mac" || Constants.TargetPlatform.ToString() == "Linux")
            {
                monitor.Log("Parsing the paths Unix style", LogLevel.Trace);
                FilePathAbsoulute_Parts = FilePathAbsolute.Split('/');
                FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                {
                    FilePathRelative += FilePathAbsoulute_Parts[i] + "/";
                }
                // Remove the trailing slash in Relative path
                FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("/"));
            }
            else
            {
                monitor.Log("Invalid platform: " + Constants.TargetPlatform.ToString(), LogLevel.Error);
            }

            return FilePathRelative;
        }
    }
}
