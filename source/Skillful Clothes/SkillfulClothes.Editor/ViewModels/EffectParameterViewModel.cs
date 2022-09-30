/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using ReactiveUI;
using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.ViewModels
{
    class EffectParameterViewModel : ViewModelBase
    {        
        public string ParameterName { get; }

        public Type ParameterType { get; }

        public object? DefaultValue { get; }

        object? _value = null;
        public object? Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }

        public object? WrappedValue { get; }

        public List<object> AvailableValues { get; } = new List<object>();

        public EffectParameterViewModel(string parameterName, Type parameterType, object? defaultValue, object? value)
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            DefaultValue = defaultValue;
            Value = value;

            if (value is IEffect effect)
            {   
                WrappedValue = new EffectViewModel(effect);
                value = effect;

                ((EffectViewModel)WrappedValue).PropertyChanged += (s, e) =>
                {
                    // propagate changes of nested effects upwards
                    if (e.PropertyName == nameof(EffectViewModel.Description))
                    {
                        this.RaisePropertyChanged(nameof(Value));
                    }
                };
            }

            if (ParameterType.IsEnum)
            {
                foreach(var en in Enum.GetValues(ParameterType))
                {
                    AvailableValues.Add(en);
                }                
            }
        }
    }
}
