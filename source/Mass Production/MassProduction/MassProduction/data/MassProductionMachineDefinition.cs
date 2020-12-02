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
        public string BaseProducerName { get; protected set; }
        public List<object> BlacklistedInputKeys { get; protected set; }
        public MPMSettings Settings { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseProducerName">Name of the base production machine as used by PFM or the base game.</param>
        /// <param name="settings">How the machine affects inputs and outputs.</param>
        protected MassProductionMachineDefinition(string baseProducerName, MPMSettings settings)
        {
            BaseProducerName = baseProducerName;
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

            //PFM integration
            List<ProducerRule> allProducerRules = ProducerController.GetProducerRules();
            List<string> baseProducers = new List<string>();

            foreach (ProducerRule rule in allProducerRules)
            {
                if (ProducerController.GetProducerConfig(rule.ProducerName) != null && !baseProducers.Contains(rule.ProducerName))
                {
                    baseProducers.Add(rule.ProducerName);
                }
            }

            foreach (string baseProducerName in baseProducers)
            {
                ProducerConfig config = ProducerController.GetProducerConfig(baseProducerName);

                foreach (MPMSettings setting in settings.Values)
                {
                    if (config == null || 
                        (config.NoInputStartMode.HasValue && setting.InputRequirementEnum == InputRequirement.InputRequired) ||
                        (!config.NoInputStartMode.HasValue && setting.InputRequirementEnum == InputRequirement.NoInputsOnly))
                    {
                        continue;
                    }

                    mpms.Add(new MassProductionMachineDefinition(baseProducerName, setting));
                }
            }

            //Other vanilla machines
            foreach (string vanillaMachineName in StaticValues.SUPPORTED_VANILLA_MACHINES.Keys)
            {
                if (!baseProducers.Contains(vanillaMachineName))
                {
                    InputRequirement inputRequirement = StaticValues.SUPPORTED_VANILLA_MACHINES[vanillaMachineName];

                    foreach (MPMSettings setting in settings.Values)
                    {
                        if (setting.InputRequirementEnum == inputRequirement || setting.InputRequirementEnum == InputRequirement.NoRequirements)
                        {
                            mpms.Add(new MassProductionMachineDefinition(vanillaMachineName, setting));
                        }
                    }
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
