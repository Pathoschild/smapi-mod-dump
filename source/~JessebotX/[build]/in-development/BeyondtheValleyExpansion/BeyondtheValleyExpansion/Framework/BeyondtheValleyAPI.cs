using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using BeyondTheValleyExpansion.Framework.ContentPacks;
using BeyondTheValleyExpansion;

namespace BeyondTheValleyExpansion.Framework
{
    public class BeyondtheValleyAPI : IBeyondtheValleyAPI
    {
        /// <summary> instance of <see cref="AvailableEdits"/> that contains available assets to edit</summary>
        AvailableEdits _NewEdit = new AvailableEdits();

        /// <summary> Load a new asset instead of the Default/Content Pack edit </summary>
        /// <param name="replaceFile"> the file to replace relative to "Jessebot.BeyondtheValley"'s root folder </param>
        /// <param name="newMap"> the new map asset </param>
        public void LoadNewAsset(string replaceFile, Map newMap)
        {
            // wip, todo
            if (replaceFile == RefFile.bveFarm)
            {
                _NewEdit.api_newFarm = newMap;
            }
        }
    }
}
