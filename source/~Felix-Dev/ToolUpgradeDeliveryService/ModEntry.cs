using StardewModdingAPI;
using StardewMods.Common;
using StardewMods.ToolUpgradeDeliveryService.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ToolUpgradeDeliveryService
{
    internal class ModEntry : Mod
    {
        public static CommonServices CommonServices { get; private set; }

        private MailDeliveryService mailDeliveryService;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Add services
            CommonServices = new CommonServices(Monitor, helper.Translation, helper.Reflection, helper.Content);

            // Setup services
            var mailGenerator = new MailGenerator();
            helper.Content.AssetEditors.Add(mailGenerator);

            mailDeliveryService = new MailDeliveryService(mailGenerator);

            // Start services
            mailDeliveryService.Start();
        }
    }
}
