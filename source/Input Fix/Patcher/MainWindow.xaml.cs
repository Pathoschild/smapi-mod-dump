using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Patcher
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

            Desc.Text = helper.GetString("L_SelectEXE");
        }

        private void Patch_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectPath();
            if (path == null) return;
            if (isSDVExe(path) || EnsureSDVExe(path))
            {
                try
                {
                    FileVersionInfo SelectedVersion = FileVersionInfo.GetVersionInfo(path);
                    Stream source = new FileStream(path, FileMode.Open);
                    if (Program.patchData.version != SelectedVersion.FileVersion)
                    {
                        Program.logger.Log("PatchData Version: {0}\n Selected Version: {1}\n Selected Path: {2}",
                            ConsoleLogger.LogLevel.Warn,
                            Program.patchData.version,
                            SelectedVersion.FileVersion,
                            path);
                        Desc.Text = helper.GetString("L_Patch_ExeVersion", Program.patchData.version);
                        return;
                    }
                    if (!Program.CheckSHA1(Program.patchData.Source_SHA1, source))
                    {
                        Desc.Text = helper.GetString("L_Patch_ExeSHA1", Program.patchData.version);
                        return;
                    }
                    Stream dest = PatchHelper.GetPatched(source, Program.patchData.patch);
                    if (!Program.CheckSHA1(Program.patchData.Dest_SHA1, dest))
                    {
                        Desc.Text = helper.GetString("L_Patch_Fail_SHA1", Program.patchData.version);
                        return;
                    }
                    string destDict = Path.GetDirectoryName(path);
                    string destName = "Stardew Valley_Patched.exe";
                    string finalPath = Path.Combine(destDict, destName);
                    StreamHelper.WriteFile(dest, finalPath);
                    Program.ExtractDlls(destDict);
                    Desc.Text = helper.GetString("L_Patch_Done", destName);
                    Command.Text = string.Format("\"{0}\" %command%", finalPath);
                    source.Close();
                }
                catch (Exception ex)
                {
                    Desc.Text = helper.GetString("L_Patch_Fail");
                    Program.logger.Log(ex.Message, ConsoleLogger.LogLevel.Error);
                    Program.logger.Log(ex.StackTrace, ConsoleLogger.LogLevel.Error);
                }
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Command.Text);
        }

        #region Before Patch

        private bool isSDVExe(string path)
        {
            return Path.GetFileName(path) == "Stardew Valley.exe";
        }

        private bool EnsureSDVExe(string path)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(helper.GetString("L_Patch_EnsureExe", path),
                "Ensure Patch", MessageBoxButton.YesNo);
            return messageBoxResult == MessageBoxResult.Yes;
        }

        private string SelectPath()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = helper.GetString("L_SelectEXE");
            openFileDialog.InitialDirectory = FindSDVPath();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        #endregion Before Patch

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