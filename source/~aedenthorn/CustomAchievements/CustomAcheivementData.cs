/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace CustomAchievements
{
    public class CustomAcheivementData
    {
        public string ID;
        public string name;
        public string description;
        public string iconPath = "";
        public Rectangle? iconRect;
        public bool drawFace = true;
        public bool achieved = false;
    }
}