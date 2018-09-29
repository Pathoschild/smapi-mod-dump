using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.common
{
    public class CustomEvent
    {
        public string Key;
        public string Script;

        public CustomEvent(string key, string script)
        {
            Key = key;
            Script = script;
        }
    }
}
