/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Editor.Services.Default;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.Services
{
    class ServiceKernel
    {

        public void RegisterServices()
        {
            Locator.CurrentMutable.RegisterConstant(new DefaultDialogService(), typeof(IDialogService));
            Locator.CurrentMutable.RegisterConstant(new YamlConfigFileSerializer(Locator.Current.GetService<IDialogService>()), typeof(IConfigFileSerializer));
        }

    }
}
