using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class PatchConfig
    {
        public Dictionary<string, bool> EventPatch { get; set; }
        public Dictionary<string, bool> FarmPatch { get; set; }
        public Dictionary<string, bool> FarmHousePatch { get; set; }
        public Dictionary<string, bool> Game1Patch { get; set; }
        public Dictionary<string, bool> GameLocationPatch { get; set; }
        public Dictionary<string, bool> NPCPatch { get; set; }
        public Dictionary<string, bool> ObjectPatch { get; set; }
        public Dictionary<string, bool> PetPatch { get; set; }
        public Dictionary<string, bool> SaveGamePatch { get; set; }
        public Dictionary<string, bool> TitleMenuPatch { get; set; }
        public Dictionary<string, bool> WandPatch { get; set; }
        public Dictionary<string, bool> WorldChangeEventPatch { get; set; }
    }
}
