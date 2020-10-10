/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace STALauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LangHelper helper;

        public MainWindow()
        {
            InitializeComponent();
            Resources.MergedDictionaries.Clear();
            helper = Program.helper;
            Resources.MergedDictionaries.Add(helper.langRd);
        }

        private void onMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Desc.Content = helper.GetString("L_" + (sender as Button).Name + "_Desc");
        }

        private void INSTALL_SMAPI_MouseClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://smapi.io");
        }

        private void INSTALL_STA_MouseClick(object sender, RoutedEventArgs e)
        {
            string path = SelectPath();
            if (path == null) return;
            if (isSDVFolder(path) || EnsureSDVPath())
            {
                try
                {
                    path = Path.Combine(path, "STALauncher.exe");
                    Program.logger.Log("Copying STALauncher to:" + path);
                    File.Copy(GetType().Assembly.Location, path, true);
                    Desc.Content = helper.GetString("L_INSTALL_STA_Desc_Done");
                }
                catch (Exception ex)
                {
                    Desc.Content = helper.GetString("L_INSTALL_STA_Desc_Fail");
                    Program.logger.Log(ex.Message, ConsoleLogger.LogLevel.Error);
                    Program.logger.Log(ex.StackTrace, ConsoleLogger.LogLevel.Error);
                }
            }
        }

        private bool isSDVFolder(string path)
        {
            return File.Exists(path + "/Stardew Valley.exe");
        }

        private bool EnsureSDVPath()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(helper.GetString("L_INSTALL_STA_Desc_NotGamePath"), "Ensure Copy", MessageBoxButton.YesNo);
            return messageBoxResult == MessageBoxResult.Yes;
        }

        private string SelectPath()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = helper.GetString("L_INSTALL_STA_Desc");
            openFileDialog.InitialDirectory = FindSDVPath();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return Path.GetDirectoryName(openFileDialog.FileName);
            }
            return null;
        }

        #region SMAPI GameFinder

        //https://github.com/Pathoschild/SMAPI/blob/7900a84bd68d7c9450bba719ce925b61043875f3/src/SMAPI.Toolkit/Framework/GameScanning/GameScanner.cs

        private string FindSDVPath()
        {
            IDictionary<string, string> registryKeys = new Dictionary<string, string>
            {
                [@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 413150"] = "InstallLocation", // Steam
                [@"SOFTWARE\WOW6432Node\GOG.com\Games\1453375253"] = "PATH", // GOG on 64-bit Windows
            };
            foreach (var pair in registryKeys)
            {
                string path = GetLocalMachineRegistryValue(pair.Key, pair.Value);
                if (!string.IsNullOrWhiteSpace(path))
                    return path;
            }
            return "";
        }

        /// <summary>Get the value of a key in the Windows HKLM registry.</summary>
        /// <param name="key">The full path of the registry key relative to HKLM.</param>
        /// <param name="name">The name of the value.</param>
        private string GetLocalMachineRegistryValue(string key, string name)
        {
            RegistryKey localMachine = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : Registry.LocalMachine;
            RegistryKey openKey = localMachine.OpenSubKey(key);
            if (openKey == null)
                return null;
            using (openKey)
                return (string)openKey.GetValue(name);
        }

        #endregion SMAPI GameFinder
    }
}