/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace BiggerCraftables
{
    public interface IApi
    {
        bool IsBiggerCraftable( StardewValley.Object obj );
        Vector2 GetBaseCraftable( GameLocation loc, Vector2 pos );
    }

    public class Api : IApi
    {
        public bool IsBiggerCraftable( StardewValley.Object obj )
        {
            if ( !obj.bigCraftable.Value )
                return false;

            return Mod.entries.FirstOrDefault( e => e.Name == obj.Name ) == null ? false : true;
        }

        public Vector2 GetBaseCraftable( GameLocation loc, Vector2 pos )
        {
            if ( !loc.Objects.ContainsKey( pos ) )
                return new Vector2( -1, -1 );

            var obj = loc.Objects[ pos ];
            if ( !IsBiggerCraftable( obj ) )
                return new Vector2( -1, -1 );

            var entry = Mod.entries.First( e => e.Name == obj.Name );
            int ind = obj.GetBiggerIndex();

            int relPosX = ind % entry.Width, relPosY = entry.Length - 1 - ind / entry.Width;
            Vector2 basePos = new Vector2( pos.X - relPosX, pos.Y - relPosY );
            return basePos;
        }
    }
}
