using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility.Config
{
    public class GameSound : Attribute
    {
        public string Name { get; private set; }

        public GameSound(string name)
        {
            this.Name = name;
        }
    }
}
