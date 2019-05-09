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
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.bveFarm"/> </summary>
        public bool replaceFarm;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.bveFarm"/> </summary>
        public bool api_replaceFarm;
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.bveFarm_Combat"/></summary>
        public bool replaceFarm_Combat;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.bveFarm"/> </summary>
        public bool api_replaceFarm_Combat;
        /// <summary> check if content pack(s) are replacing <see cref="RefFile.bveFarm_Foraging"/></summary>
        public bool replaceFarm_Foraging;
        /// <summary> check if a mod using BeyondtheValleyAPI are replacing <see cref="RefFile.bveFarm"/> </summary>
        public bool api_replaceFarm_Foraging;

        /*********
         ** Asset storage
         *********/
        // Farm Maps
        /// <summary> stores the read 'Farm' asset from the content pack </summary>
        public Map newFarm;
        /// <summary> stores the read 'Farm' asset from the mod using BeyondtheValleyAPI </summary>
        public Map api_newFarm;
        /// <summary> stores the read 'Farm_Combat' asset from the content pack </summary>
        public Map newFarm_Combat;
        /// <summary> stores the read 'Farm_Combat' asset from the mod using BeyondtheValleyAPI </summary>
        public Map api_newFarm_Combat;
        /// <summary> stores the read 'Farm_Foraging' asset from the content pack </summary>
        public Map newFarm_Foraging;
        /// <summary> stores the read 'Farm_Foraging' asset from the mod using BeyondtheValleyAPI </summary>
        public Map api_newFarm_Foraging;
    }
}
