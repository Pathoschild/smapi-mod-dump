/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CustomSpousePatioWizard
{
    public interface ICustomSpousePatioApi
    {
        Dictionary<string, object> GetCurrentSpouseAreas();
        Dictionary<string, Point> GetDefaultSpouseOffsets();
        void RemoveAllSpouseAreas();
        void ReloadSpouseAreaData(); 
        void AddTileSheets();
        void ShowSpouseAreas();
        void ReloadPatios();
        bool RemoveSpousePatio(string spouse);
        void AddSpousePatioHere(string spouse_tilesOf, Point cursorLoc);
        bool MoveSpousePatio(string whichAnswer, Point cursorLoc);
    }
}