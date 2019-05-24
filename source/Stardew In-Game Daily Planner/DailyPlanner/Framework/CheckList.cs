using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner.Framework
{
    class CheckList
    {
        private List<string> CheckListItems;

        public CheckList()
        {
            string filename = Path.Combine("Mods", "DailyPlanner", "Plans", "Checklist.txt");
            if (File.Exists(filename))
            {
                this.CheckListItems = new List<string>(File.ReadAllLines(filename, Encoding.UTF8));
                this.CheckListItems.Remove("");
                this.CheckListItems.Remove(" ");
            } else
            {
                string[] list = new string[] { "Find DailyPlanner/Plans/Checklist.txt.", "Open it in notepad.", "Add your tasks.", "Open this menu agian." };
                File.WriteAllLines(filename, list);
            }
             
            
        }

        public List<string> GetCheckListItems()
        {
            return this.CheckListItems;
        }

        public void CompleteTask(string label)
        {
            this.CheckListItems.Remove(label);
            string filename = Path.Combine("Mods", "DailyPlanner", "Plans", "Checklist.txt");
            File.WriteAllLines(filename, this.CheckListItems);
        }
    }
}
