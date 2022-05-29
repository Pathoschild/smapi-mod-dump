/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SkillfulClothes.Editor.ViewModels;
using SkillfulClothes.Editor.Views;
using SkillfulClothes.Editor.Views.EffectParameters;
using SkillfulClothes.Effects;
using System;

namespace SkillfulClothes.Editor
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            if (data is NestedEffectParameterViewModel nestedEffectParamVm)
            {
                return new NestedEffectParameterView();
            } else
            if (data is EffectParameterViewModel effectParamVm)
            {                
                if (effectParamVm.ParameterType == typeof(int) ||
                    effectParamVm.ParameterType == typeof(float) ||
                    effectParamVm.ParameterType == typeof(double))
                {
                    return new NumericParameterView();
                }

                if (effectParamVm.ParameterType.IsEnum)
                {
                    return new EnumParameterView();
                }

                if (effectParamVm.ParameterType.IsAssignableTo(typeof(IEffect)))
                {
                    return new EffectParameterView();
                }                
            }
            else
            {
                var name = data.GetType().FullName!.Replace("ViewModel", "View");
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }                
            }

            // default
            return new TextBlock { Text = "Not Found: " + data?.GetType()?.FullName ?? "null" };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
