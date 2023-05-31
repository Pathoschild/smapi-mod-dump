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

namespace Omegasis.Revitalize.Framework.SaveData
{
    /// <summary>
    /// Base class for handling persistent save data for the game for individual saves.
    /// </summary>
    public class SaveDataInfo
    {

        public SaveDataInfo()
        {

        }

        public virtual void save()
        {

        }
        /// <summary>
        /// Writes this save intormation to a specific file.
        /// </summary>
        /// <param name="FileName"></param>
        public virtual void save(string FileName)
        {

        }

        public virtual void load()
        {

        }

    }
}
