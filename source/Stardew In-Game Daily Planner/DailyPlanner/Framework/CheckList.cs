using System.IO;
using System.Collections.Generic;
using System.Text;

namespace DailyPlanner.Framework
{
    class CheckList
    {
        /// <summary>List of items on the checklist.</summary>
        private readonly List<string> CheckListItems;

        /// <summary>Construct a checklist by reading the Checklist.txt file, or create a file if it doesn't exist.</summary>
        public CheckList()
        {
            string filename = Path.Combine("Mods", "DailyPlanner", "Plans", "Checklist.txt");  // TODO: Make sure we don't need the complete path from the root directory.
            if (File.Exists(filename))  // Read each line of the file into a list, then remove blank entries. 
            {
                this.CheckListItems = new List<string>(File.ReadAllLines(filename, Encoding.UTF8));  
                this.CheckListItems.Remove("");
                this.CheckListItems.Remove(" ");
            } else  // If file doesn't exist, create it and fill it with instructions on how to edit it.
            {
                string[] list = new string[] { "Find DailyPlanner/Plans/Checklist.txt.", "Open it in notepad.", "Add your tasks.", "Open this menu agian." };
                File.WriteAllLines(filename, list);
            }
             
            
        }

        public List<string> GetCheckListItems()
        {
            return this.CheckListItems;
        }

        /// <summary>Delete item from the checklist, then save the file.</summary>
        /// <param name="label">Text of the item being marked off.</param>
        public void CompleteTask(string label)
        {
            this.CheckListItems.Remove(label);
            string filename = Path.Combine("Mods", "DailyPlanner", "Plans", "Checklist.txt");
            File.WriteAllLines(filename, this.CheckListItems);
        }
    }
}
