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
using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkillfulClothes.Editor.ViewModels
{
    class NestedEffectParameterViewModel : EffectParameterViewModel
    {
        public AvaloniaList<EffectViewModel> NestedEffects { get; } = new AvaloniaList<EffectViewModel>();

        public NestedEffectParameterViewModel(string parameterName, Type parameterType, object? defaultValue, object? value)
             : base(parameterName, parameterType, defaultValue, value)
        {
            NestedEffects.CollectionChanged += NestedEffects_CollectionChanged;

            if (value is EffectSet effectSet)
            {
                foreach (var effect in effectSet.Effects)
                {
                    NestedEffects.Add(new EffectViewModel(effect));
                }
            }
            else
            if (value is IEffect effect)
            {
                NestedEffects.Add(new EffectViewModel(effect));
            }
        }

        private void NestedEffects_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach(var item in e.OldItems.OfType<EffectViewModel>())
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<EffectViewModel>())
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EffectViewModel.Description))
            {
                this.RaisePropertyChanged(nameof(Value));
            }
        }        

    }
}
