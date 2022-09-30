/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.Services
{
    interface IConfigFileSerializer
    {

        IList<ClothingItemViewModel<T>> LoadFromFile<T>(string filename);

        IList<ClothingItemViewModel<T>> Load<T>(Stream stream);

    }
}
