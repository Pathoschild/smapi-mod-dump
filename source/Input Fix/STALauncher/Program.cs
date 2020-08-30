using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace STALauncher
{
    internal class Program
    {
        private const string SMAPIEXE = "StardewModdingAPI.exe";
        private const string SDVEXE = "Stardew Valley.exe";

        public static LangHelper helper = new LangHelper();
        public static ConsoleLogger logger = new ConsoleLogger(helper);

        [STAThread]
        private static void Main(string[] args)
        {
            if (File.Exists(SMAPIEXE))
            {
                logger.LogTrans("L_FINDED_SMAPI", ConsoleLogger.LogLevel.Info, Path.GetFullPath(SMAPIEXE));
                logger.LogTrans("L_WARN_STALAUNCHER_PATH", ConsoleLogger.LogLevel.Warn);
                logger.Log(string.Format("\"{0}\" %command%", typeof(Program).Assembly.Location), ConsoleLogger.LogLevel.Warn);
                IconHelper.SetConsoleIcon(IconHelper.GetIconOf(SMAPIEXE));
                BootStrapSMAPI(args);
            }
            else
                Handle_SMAPINotFound();
        }

        private static void BootStrapSMAPI(string[] args)
        {
            try
            {
                Type program = Type.GetType("StardewModdingAPI.Program, " + "StardewModdingAPI", true);
                MethodInfo main = program.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
                main.Invoke(null, new object[] { args });
            }
            catch (Exception e)
            {
                logger.LogTrans("L_FAIL_BOOTSTRAP", ConsoleLogger.LogLevel.Error);
                logger.Log(e.ToString(), ConsoleLogger.LogLevel.Error);
                logger.Log(e.StackTrace, ConsoleLogger.LogLevel.Error);
                logger.Pause();
            }
        }

        private static void Handle_SMAPINotFound()
        {
            string text;
            if (File.Exists(SDVEXE))
                text = helper.GetString("L_WARN_SMAPINOTFOUND");
            else
                text = helper.GetString("L_WARN_GAMENOTFOUND");
            logger.Log(text, ConsoleLogger.LogLevel.Warn);
            MainWindow window = new MainWindow();
            window.Closed += new EventHandler((sender, e) => { Application.Exit(); });
            window.Notice.Content = text;
            window.Show(); window.Activate();
            Application.Run();
        }
    }
}