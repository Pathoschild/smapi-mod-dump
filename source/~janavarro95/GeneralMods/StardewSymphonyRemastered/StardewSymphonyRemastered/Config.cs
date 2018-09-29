using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered
{
    public class Config
    {
        public bool EnableDebugLog { get; set; }=false;
        public int MinimumDelayBetweenSongsInMilliseconds { get; set; }=5000;
        public int MaximumDelayBetweenSongsInMilliseconds { get; set; }=60000;
        public string KeyBinding { get; set; }="L";


    }
}
