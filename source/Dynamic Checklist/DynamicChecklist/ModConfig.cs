namespace DynamicChecklist
{
    using System;
    using System.Collections.Generic;
    using ObjectLists;

    public class ModConfig
    {
        public ModConfig()
        {
            this.IncludeTask = new Dictionary<TaskName, bool>();
            var listNames = (TaskName[])Enum.GetValues(typeof(TaskName));
            foreach (var listName in listNames)
            {
                this.IncludeTask.Add(listName, true);
            }
        }

        public enum ButtonLocation
        {
            BelowJournal, LeftOfJournal
        }

        public string OpenMenuKey { get; set; } = "NumPad1";

        public bool ShowAllTasks { get; set; } = false;

        public bool AllowMultipleOverlays { get; set; } = true;

        public bool ShowArrow { get; set; } = true;

        public bool ShowOverlay { get; set; } = true;

        public ButtonLocation OpenChecklistButtonLocation { get; set; } = ButtonLocation.BelowJournal;

        public Dictionary<TaskName, bool> IncludeTask { get; set; }
    }
}
