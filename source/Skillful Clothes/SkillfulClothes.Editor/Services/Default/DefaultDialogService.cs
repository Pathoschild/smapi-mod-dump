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
using Avalonia.Controls.ApplicationLifetimes;
using SkillfulClothes.Editor.ViewModels;
using SkillfulClothes.Editor.Views;
using SkillfulClothes.Effects;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.Services.Default
{
    class DefaultDialogService : IDialogService
    {
        protected Window? GetDialogOwner()
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            return null;
        }

        public void SelectEffectFromLibrary(Action<IEffect> onSelection)
        {            
            EffectLibraryWindow w = new EffectLibraryWindow();
            w.DataContext = new EffectLibraryViewModel(Locator.Current.GetService<IDialogService>());
            w.ShowDialog(GetDialogOwner());
        }
    }
}
