/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using System.IO;
using System.Collections.Generic;
using System.Text;

namespace DailyPlanner.Framework
{
    public class CheckList
    {
        /// <summary>List of items on the checklist.</summary>
        private readonly List<string> CheckListItems;

        private readonly string Filename;

        /// <summary>
        /// Construct a checklist by reading the Checklist.txt file, or create a file if it doesn't exist.
        /// </summary>
        public CheckList()
        {
            this.Filename = Path.Combine(StardewModdingAPI.Constants.CurrentSavePath, "DailyPlanner", "Checklist.txt");
            if (File.Exists(this.Filename))  // Read each line of the file into a list, then remove blank entries. 
            {
                this.CheckListItems = new List<string>(File.ReadAllLines(this.Filename, Encoding.UTF8));  
                this.CheckListItems.Remove("");
                this.CheckListItems.Remove(" ");
            } else  // If file doesn't exist, create a blank one.
            {
                Directory.CreateDirectory(Path.Combine(StardewModdingAPI.Constants.CurrentSavePath, "DailyPlanner"));
                File.WriteAllLines(this.Filename, System.Array.Empty<string>());
            }
        }

        public List<string> GetCheckListItems()
        {
            return this.CheckListItems;
        }

        /// <summary>
        /// Delete item from the checklist, then save the file.
        /// </summary>
        /// <param name="label">Text of the item being marked off.</param>
        public void CompleteTask(string label)
        {
            this.CheckListItems.Remove(label);
            File.WriteAllLines(this.Filename, this.CheckListItems);
        }

        /// <summary>
        /// Add items to the checklist, then save the file.
        /// </summary>
        /// <param name="label">Text of the item being added.</param>
        public void AddTask(string label)
        {
            this.CheckListItems.Insert(0, label);
            File.WriteAllLines(this.Filename, this.CheckListItems);
        }
    }
}
