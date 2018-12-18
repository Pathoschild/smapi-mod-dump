using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Objects.Tools
{
    public class BasicToolInfo
    {
        /// <summary>
        /// The name of the tool.
        /// </summary>
        public string name;

        /// <summary>
        /// The upgrade level of the tool.
        /// </summary>
        public int level;

        /// <summary>
        /// The description of the tool.
        /// </summary>
        public string description;

        /// <summary>
        /// Constructor used to hold generic info shared across all tools.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Level"></param>
        /// <param name="Description"></param>
        public BasicToolInfo(String Name, int Level, string Description)
        {
            this.name = Name;
            this.level = Level;
            this.description = Description;
        }

    }
}
