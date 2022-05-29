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
using SkillfulClothes.Configuration;
using SkillfulClothes.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SkillfulClothes.Effects;
using SkillfulClothes.Editor.Services;

namespace SkillfulClothes.Editor.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        IDialogService DialogService { get; }

        EffectLibrary EffectLibrary { get; } = EffectLibrary.Default;

        public AvaloniaList<ClothingItemViewModel<Shirt>> Shirts { get; } = new AvaloniaList<ClothingItemViewModel<Shirt>>();

        public MainWindowViewModel(IDialogService dialogService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // single effect
            var cItem = new ClothingItemViewModel<Shirt>(Shirt.BackpackShirt);
            cItem.EffectRoot.NestedEffects.Add(new EffectViewModel(EffectLibrary.Default.CreateEffectInstance("IncreaseAttack")));
            Shirts.Add(cItem);

            // nested effects
            cItem = new ClothingItemViewModel<Shirt>(Shirt.FakeMusclesShirt);
            var seasonalEffect = EffectLibrary.Default.CreateEffectInstance("Seasonal");
            var eParams = ((ICustomizableEffect)seasonalEffect).ParameterObject;
            var ep = eParams.GetType().GetProperty("Effect");
            ep.SetValue(eParams, EffectLibrary.Default.CreateEffectInstance("IncreaseSpeed"));
            ((ICustomizableEffect)seasonalEffect).ReloadParameters();
            cItem.EffectRoot.NestedEffects.Add(new EffectViewModel(seasonalEffect));            
            Shirts.Add(cItem);
        }
    }
}
