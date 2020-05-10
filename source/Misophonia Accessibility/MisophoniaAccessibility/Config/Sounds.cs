using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility.Config
{
    public class Sounds
    {
        [GameSound("eat")]
        public bool DisableEatSound { get; set; } = true;

        [GameSound("gulp")]
        public bool DisableDrinkSound { get; set; } = false;
    }
}
