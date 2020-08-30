using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint.Config
{
    class SprintConfig
    {
        /// <summary> The player's sprinting speed. </summary>
        public int SprintSpeed { get; set; } = 5;

        /// <summary> Add extra speed if 'SprintKey = LeftShift'. </summary>
        public int LeftShiftKeybindExtraSpeed { get; set; } = 7;
    }
}
