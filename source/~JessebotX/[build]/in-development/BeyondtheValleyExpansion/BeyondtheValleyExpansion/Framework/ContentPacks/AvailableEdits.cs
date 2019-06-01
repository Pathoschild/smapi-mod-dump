using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace BeyondTheValleyExpansion.Framework.ContentPacks
{
    class AvailableEdits
    {
        /*********
         ** Check if asset is being replaced
         *********/
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.BveFarm"/> </summary>
        public bool ReplaceFarm;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.BveFarm"/> </summary>
        public bool API_replaceFarm;
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.BveFarm_Combat"/></summary>
        public bool ReplaceFarm_Combat;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.BveFarm"/> </summary>
        public bool API_replaceFarm_Combat;
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.BveFarm_Foraging"/></summary>
        public bool ReplaceFarm_Foraging;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.BveFarm"/> </summary>
        public bool API_replaceFarm_Foraging;

        /*********
         ** Asset storage
         *********/
        // Farm Maps
        /// <summary> stores the read 'Farm' asset from the content pack </summary>
        public Map NewFarm;
        /// <summary> stores the read 'Farm' asset from the mod using BeyondtheValleyAPI </summary>
        public Map API_newFarm;
        /// <summary> stores the read 'Farm_Combat' asset from the content pack </summary>
        public Map NewFarm_Combat;
        /// <summary> stores the read 'Farm_Combat' asset from the mod using BeyondtheValleyAPI </summary>
        public Map API_newFarm_Combat;
        /// <summary> stores the read 'Farm_Foraging' asset from the content pack </summary>
        public Map NewFarm_Foraging;
        /// <summary> stores the read 'Farm_Foraging' asset from the mod using BeyondtheValleyAPI </summary>
        public Map API_newFarm_Foraging;
    }
}
