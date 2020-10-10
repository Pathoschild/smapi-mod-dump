/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore
{
    public interface ISaveCoreAPI
    {
        /*********
        ** Events
        *********/
        /// <summary>
        ///     Event that fires before game save
        /// </summary>
        event EventHandler BeforeSave;
        /// <summary>
        ///     Event that fires after game save
        /// </summary>
        event EventHandler AfterSave;
        /// <summary>
        ///     Event that fires after game load
        /// </summary>
        event EventHandler AfterLoad;
    }
}
