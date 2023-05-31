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
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;

namespace Omegasis.Revitalize.Framework.World.Objects.Interfaces
{
    /// <summary>
    /// The base interface provided for all modded objects.
    /// </summary>
    public interface IBasicItemInfoProvider
    {
        public BasicItemInformation basicItemInformation { get; set; }

        public string Id { get; }
    }
}
