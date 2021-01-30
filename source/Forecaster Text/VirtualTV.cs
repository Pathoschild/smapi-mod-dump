/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

using StardewValley.Objects;

namespace ForecasterText {
    public class VirtualTV : TV {
        
        public VirtualTV() : base() {}

        public int GetRerunWeek() {
            return base.getRerunWeek();
        }
        
    }
}