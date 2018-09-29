using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodGuard
{
    [Serializable]
    public class ProfessionFixConfig
    {
        /// <summary>
        /// Whether to enable the guard against overflow when petting with a relevant profession.
        /// </summary>
        public bool Enabled = true;
    }
}
