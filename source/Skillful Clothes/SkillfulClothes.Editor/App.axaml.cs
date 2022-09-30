/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SkillfulClothes.Editor.Services;
using SkillfulClothes.Editor.ViewModels;
using SkillfulClothes.Editor.Views;
using Splat;

namespace SkillfulClothes.Editor
{
    public partial class App : Application
    {
        private static MainWindow? MainWindow { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static void Exit()
        {
            MainWindow?.Close();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ServiceKernel serviceKernel = new ServiceKernel();
            serviceKernel.RegisterServices();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(Locator.Current.GetService<IConfigFileSerializer>(), Locator.Current.GetService<IDialogService>()),
                };
                desktop.MainWindow = MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
