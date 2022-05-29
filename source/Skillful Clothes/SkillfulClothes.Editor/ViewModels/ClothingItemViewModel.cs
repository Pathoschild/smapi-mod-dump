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
using ReactiveUI;
using SkillfulClothes.Configuration;
using SkillfulClothes.Editor.Utils;
using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.ViewModels
{
    class ClothingItemViewModel<T> : ViewModelBase        
    {
        T Id { get; }

        public string DisplayName { get; }

        public string Description => String.Join(", ", EffectRoot.NestedEffects.Select(x => x.Description));

        public NestedEffectParameterViewModel EffectRoot { get; } = NestedEffectParameterViewModel.CreateRoot();

        IReactiveCommand AddEffectCommand { get; }

        public ClothingItemViewModel(T id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = NameFormatting.FormatClosingItemName(id?.ToString() ?? "");

            // TODO
            EffectRoot.PropertyChanged += EffectRoot_PropertyChanged;

            AddEffectCommand = ReactiveCommand.Create(() =>
            {
                
            });                               
        }

        private void EffectRoot_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EffectViewModel.Description) || e.PropertyName == nameof(EffectParameterViewModel.Value))
            {
                this.RaisePropertyChanged(nameof(Description));
            }
        }
    }
}
