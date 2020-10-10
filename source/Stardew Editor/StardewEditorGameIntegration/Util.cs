/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewEditor
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditorGameIntegration
{
    public class Util
    {
        public static uint swapIfLittleEndian( uint i )
        {
            if (!BitConverter.IsLittleEndian)
                return i;

            return ( ( i & 0x000000FF ) << 24 ) | 
                   ( ( i & 0x0000FF00 ) <<  8 ) | 
                   ( ( i & 0x00FF0000 ) >>  8 ) | 
                   ( ( i & 0xFF000000 ) >> 24 );
        }
    }
}
