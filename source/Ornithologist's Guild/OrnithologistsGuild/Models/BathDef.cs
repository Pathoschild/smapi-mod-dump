/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Linq;
using DynamicGameAssets.Game;
using OrnithologistsGuild.Content;

namespace OrnithologistsGuild.Models
{
    public class BathDef
    {
        public string ID;
        public bool Heated;

        public int ZOffset;

        public static BathDef FromBath(CustomBigCraftable bath)
        {
            return ContentManager.Baths.FirstOrDefault(bathDef => bathDef.ID == bath.Id);
        }
    }
}
