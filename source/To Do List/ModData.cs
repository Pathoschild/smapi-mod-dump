using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ToDoMod
{
    class ModData
    {
        public StringCollection SavedTasks { get; set; } = new StringCollection();

    }
}
