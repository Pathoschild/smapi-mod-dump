/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Avalonia.Collections;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.ViewModels
{
    internal class EffectRootViewModel : ViewModelBase
    {
        public AvaloniaList<EffectViewModel> AttachedEffects { get; } = new AvaloniaList<EffectViewModel>();

        public bool HasEffect => AttachedEffects.Count > 0;

        public EffectRootViewModel()
        {
            AttachedEffects.CollectionChanged += AttachedEffects_CollectionChanged;
        }

        private void AttachedEffects_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(HasEffect));
        }
    }
}
