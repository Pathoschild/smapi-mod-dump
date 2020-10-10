/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Patcher.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Patcher
{
    public class Program
    {
        private const string SDVEXE = "Stardew Valley.exe";
        public static LangHelper helper = new LangHelper();
        public static ConsoleLogger logger = new ConsoleLogger(helper);
        public static PatchData patchData;

        [STAThread]
        private static void Main(string[] args)
        {
            //CreateSDVPatch();
            patchData = PatchData.GetDataFrom(new MemoryStream(Resources.PatchData));
            if (File.Exists(SDVEXE))
            {
                logger.LogTrans("L_FINDED_SDV", ConsoleLogger.LogLevel.Info, Path.GetFullPath(SDVEXE));
                PatchConsole(Path.GetFullPath(SDVEXE));
                logger.Pause();
            }
            else
                Handle_SDVNotFound();
        }

        private static void PatchConsole(string path)
        {
            try
            {
                FileVersionInfo SelectedVersion = FileVersionInfo.GetVersionInfo(path);
                Stream source = new FileStream(path, FileMode.Open);
                if (patchData.version != SelectedVersion.FileVersion)
                {
                    logger.Log("PatchData Version: {0}\n Selected Version: {1}\n Selected Path: {2}",
                        ConsoleLogger.LogLevel.Warn,
                        patchData.version,
                        SelectedVersion.FileVersion,
                        path);
                    logger.LogTrans("L_Patch_ExeVersion", ConsoleLogger.LogLevel.Warn, patchData.version);
                    return;
                }
                if (!CheckSHA1(patchData.Source_SHA1, source))
                {
                    logger.LogTrans("L_Patch_ExeSHA1", ConsoleLogger.LogLevel.Warn, patchData.version);
                    return;
                }
                Stream dest = PatchHelper.GetPatched(source, patchData.patch);
                if (!CheckSHA1(patchData.Dest_SHA1, dest))
                {
                    logger.LogTrans("L_Patch_Fail_SHA1", ConsoleLogger.LogLevel.Warn, patchData.version);
                    return;
                }
                string destDict = Path.GetDirectoryName(path);
                string destName = "Stardew Valley_Patched.exe";
                string finalPath = Path.Combine(destDict, destName);
                StreamHelper.WriteFile(dest, finalPath);
                ExtractDlls(destDict);
                logger.LogTrans("L_Patch_Done", ConsoleLogger.LogLevel.Info, destName);
                logger.Log(string.Format("\"{0}\" %command%", finalPath), ConsoleLogger.LogLevel.Warn);
                source.Close();
            }
            catch (Exception ex)
            {
                logger.LogTrans("L_Patch_Fail");
                logger.Log(ex.Message, ConsoleLogger.LogLevel.Error);
                logger.Log(ex.StackTrace, ConsoleLogger.LogLevel.Error);
            }
        }

        #region Dlls

        public static void ExtractDlls(string path)
        {
            WriteDll(Resources.libtfWrapper, Path.Combine(path, "libtfWrapper.dll"));
            WriteDll(Resources.ImeSharp, Path.Combine(path, "ImeSharp.dll"));
            WriteDll(Resources._0Harmony, Path.Combine(path, "0Harmony.dll"));
        }

        private static void WriteDll(byte[] dlldata, string path)
        {
            var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            file.Write(dlldata, 0, dlldata.Length);
            file.Flush();
            file.Close();
        }

        #endregion Dlls

        #region SHA1 Compare

        public static bool CheckSHA1(byte[] sha1, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] sourceSHA1 = SHA1.Create().ComputeHash(stream);
            return ByteArraysEqual(sha1, sourceSHA1);
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        #endregion SHA1 Compare

        private static void Handle_SDVNotFound()
        {
            MainWindow window = new MainWindow();
            window.Closed += new EventHandler((sender, e) => { Application.Exit(); });
            window.Show(); window.Activate();
            Application.Run();
        }

        public static void CreateSDVPatch()
        {
            string path = @"E:\Codes\source\repos\InputFix\Patcher\DataSources";
            string PATCHED = "Stardew Valley_Fixed.exe";

            patchData = new PatchData(Path.Combine(path, SDVEXE),
                                      Path.Combine(path, PATCHED),
                                      FileVersionInfo.GetVersionInfo(Path.Combine(path, SDVEXE)).FileVersion);
            StreamHelper.WriteFile(patchData.ToStream(), Path.Combine(path, "PatchData"));
        }
    }
}