/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkillfulClothes.Effects;
using Avalonia.Collections;
using ReactiveUI;
using SkillfulClothes.Editor.Utils;
using System.Windows.Input;
using Splat;
using SkillfulClothes.Editor.Services;

namespace SkillfulClothes.Editor.ViewModels
{
    class EffectViewModel : ViewModelBase
    {
        public IDialogService DialogService { get; }

        public IEffect Model { get; }

        public string DisplayName { get; }

        public string Description => String.Join("\n", Model.EffectDescription.Select(x => x.Text));

        public bool HasParameters => Parameters.Count > 0;

        public AvaloniaList<EffectParameterViewModel> Parameters { get; } = new AvaloniaList<EffectParameterViewModel>();

        public ICommand AddSiblingEffectCommand { get; }

        public EffectViewModel(IEffect model, IDialogService? dialogService = null)
        {
            DialogService = dialogService ?? Locator.Current.GetService<IDialogService>();

            Model = model ?? throw new ArgumentNullException(nameof(model));
            DisplayName = NameFormatting.FormatEffectDisplayName(model.GetType().Name);
            
            if (Model is ICustomizableEffect customEffect)
            {
                var paramObject = customEffect.ParameterObject;
                foreach (var prop in paramObject.GetType().GetProperties())
                {
                    var value = prop.GetValue(paramObject);
                    EffectParameterViewModel paramVm;

                    if (prop.PropertyType == typeof(IEffect))
                    {                        
                        paramVm = new NestedEffectParameterViewModel(prop.Name, prop.PropertyType, null, value);                        
                    } else
                    {                        
                        paramVm = new EffectParameterViewModel(prop.Name, prop.PropertyType, null, value);                        
                    }

                    paramVm.PropertyChanged += ParamVm_PropertyChanged;
                    Parameters.Add(paramVm);
                }
            }

            AddSiblingEffectCommand = ReactiveCommand.Create(() => DialogService.SelectEffectFromLibrary((effect) => { }));
        }        

        private void ParamVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Model is ICustomizableEffect customEffect && sender is EffectParameterViewModel evm && e.PropertyName != null)
            {
                var newValue = sender?.GetType()?.GetProperty(e.PropertyName)?.GetValue(sender);

                var pType = customEffect.ParameterObject.GetType();
                if (pType.GetProperty(evm.ParameterName) is PropertyInfo pInfo)
                {
                    if (newValue is IConvertible)
                    {
                        pInfo.SetValue(customEffect.ParameterObject, Convert.ChangeType(newValue, evm.ParameterType));
                    } else
                    {
                        pInfo.SetValue(customEffect.ParameterObject, newValue);
                    }                    
                    customEffect.ReloadParameters();
                    this.RaisePropertyChanged(nameof(Description));                    
                }
            }
        }
    }
}
