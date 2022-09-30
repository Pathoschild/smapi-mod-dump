/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday.Framework.Configs
{
    public class ConfigManager
    {
        /// <summary>
        /// The config that handles information with regards to the mail.
        /// </summary>
        public MailConfig mailConfig;

        /// <summary>The mod configuration.</summary>
        public ModConfig modConfig;

        public ConfigManager()
        {


        }

        public virtual void initializeConfigs()
        {
            this.mailConfig = MailConfig.InitializeConfig();
            this.modConfig = ModConfig.InitializeConfig();
            this.modConfig.fallbackToEnglishTranslationWhenPossible = true;
        }


        /// <summary>
        /// Gets the path to the given config directory.
        /// </summary>
        /// <param name="GetFullPath">If true, returns the fully qualified path. If false, returns the relative path.</param>
        /// <returns></returns>
        public string getConfigDirectory(bool GetFullPath)
        {
            Directory.CreateDirectory(Path.Combine(HappyBirthdayModCore.Instance.Helper.DirectoryPath, "Configs"));
            if (GetFullPath)
            {
                return Path.Combine(HappyBirthdayModCore.Instance.Helper.DirectoryPath, "Configs");
            }
            return "Configs";
        }

        /// <summary>
        /// Gets the path to a config file.
        /// </summary>
        /// <param name="GetFullPath"></param>
        /// <param name="ConfigFileName"></param>
        /// <returns></returns>
        public string getConfigPath(bool GetFullPath, string ConfigFileName)
        {
            Directory.CreateDirectory(Path.Combine(HappyBirthdayModCore.Instance.Helper.DirectoryPath, "Configs"));
            if (GetFullPath)
            {
                return Path.Combine(HappyBirthdayModCore.Instance.Helper.DirectoryPath, "Configs",ConfigFileName);
            }
            return Path.Combine("Configs",ConfigFileName);
        }

        /// <summary>
        /// Checks to see if the config exists at the fully qualified path.
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <returns></returns>
        public bool doesConfigExist(string ConfigName)
        {
            string pathToConfig = this.getConfigPath(true, ConfigName);
            return File.Exists(pathToConfig);
        }

        /// <summary>
        /// Reads a config from a .json file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigName"></param>
        /// <returns></returns>
        public virtual T ReadConfig<T>(string ConfigName) where T : class
        {
            return HappyBirthdayModCore.Instance.Helper.Data.ReadJsonFile<T>(this.getConfigPath(false, ConfigName));
        }

        /// <summary>
        /// Writes a config to a .json file.
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <param name="Config"></param>
        public virtual void WriteConfig(string ConfigName, object Config)
        {
            HappyBirthdayModCore.Instance.Helper.Data.WriteJsonFile(this.getConfigPath(false,ConfigName), Config);
        }

    }
}
