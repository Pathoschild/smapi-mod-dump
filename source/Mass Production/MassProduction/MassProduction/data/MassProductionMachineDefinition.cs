/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using ProducerFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    /// <summary>
    /// Sets up the mass production capable machines.
    /// </summary>
    public class MassProductionMachineDefinition
    {
        public static readonly string PRODUCER_NAME_PREFIX = "Mass Production";

        public string BaseProducerName { get; protected set; }
        public string ProducerNameSuffix { get; protected set; }
        public List<object> BlacklistedInputKeys { get; protected set; }
        public MPMSettings Settings { get; protected set; }

        public string ProducerName { get { return PRODUCER_NAME_PREFIX + " " + BaseProducerName + " (" + ProducerNameSuffix + ")"; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseProducerName">Name of the base production machine as used by PFM.</param>
        /// <param name="suffix">Will be added to end of producer names.</param>
        /// <param name="settings">How the machine affects inputs and outputs.</param>
        protected MassProductionMachineDefinition(string baseProducerName, string suffix, MPMSettings settings)
        {
            BaseProducerName = baseProducerName;
            ProducerNameSuffix = suffix;
            Settings = settings;
            BlacklistedInputKeys = new List<object>();
        }

        /// <summary>
        /// Defines all mass production machines.
        /// </summary>
        /// <param name="settings">Specifies the kinds of machines being set up.</param>
        /// <returns>All mass production machine definitions.</returns>
        public static List<MassProductionMachineDefinition> Setup(Dictionary<string, MPMSettings> settings)
        {
            List<MassProductionMachineDefinition> mpms = new List<MassProductionMachineDefinition>();
            List<ProducerRule> allProducerRules = ProducerController.GetProducerRules();
            List<string> baseProducers = new List<string>();

            foreach (ProducerRule rule in allProducerRules)
            {
                if (!baseProducers.Contains(rule.ProducerName))
                {
                    baseProducers.Add(rule.ProducerName);
                }
            }

            foreach (string baseProducerName in baseProducers)
            {
                ProducerConfig config = ProducerController.GetProducerConfig(baseProducerName);

                foreach (string settingName in settings.Keys)
                {
                    if (config == null || (config.NoInputStartMode.HasValue && !settings[settingName].AllowInputlessBases))
                    {
                        continue;
                    }

                    mpms.Add(new MassProductionMachineDefinition(baseProducerName, settingName, settings[settingName]));
                }
            }

            return mpms;
        }

        /// <summary>
        /// Gets the PFM config for the base machine.
        /// </summary>
        /// <returns></returns>
        public ProducerConfig GetBaseProducerConfig()
        {
            return ProducerController.GetProducerConfig(BaseProducerName);
        }
    }
}
