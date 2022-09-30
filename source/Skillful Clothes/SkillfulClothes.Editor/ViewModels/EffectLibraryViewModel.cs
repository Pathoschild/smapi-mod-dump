/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using DynamicData;
using SkillfulClothes.Configuration;
using SkillfulClothes.Editor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.ViewModels
{
    class EffectLibraryViewModel : ViewModelBase
    {
        IDialogService DialogService { get; }

        public ObservableCollection<string> AvailableEffects { get; } = new ObservableCollection<string>();

        public EffectLibraryViewModel(IDialogService dialogService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AvailableEffects.AddRange(EffectLibrary.Default.GetAllEffectTypes().Select(x => x.Name));
        }

    }
}
