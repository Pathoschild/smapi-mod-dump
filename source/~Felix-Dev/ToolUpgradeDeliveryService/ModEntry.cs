using FelixDev.StardewMods.ToolUpgradeDeliveryService.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.ToolUpgradeDeliveryService
{
    /// <summary>
    /// Represents the entry point for the Tool-Upgrade Delivery Service mod. Initializes and starts all the needed
    /// services (such as the mail delivery service).
    /// </summary>
    internal class ModEntry : Mod
    {
        /// <summary>The mail-delivery service to use.</summary>
        private MailDeliveryService mailDeliveryService;

        /// <summary>The mod configuration from the player.</summary>
        public static ModConfig ModConfig { get; private set; }

        /// <summary>Provides access to the simplified APIs for writing mods provided by SMAPI.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
        public static IMonitor _Monitor { get; private set; }

        /// <summary>Provides access to the <see cref="IManifest"/> API provided by SMAPI.</summary>
        public static IManifest _ModManifest { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            _Monitor = this.Monitor;
            _ModManifest = this.ModManifest;

            // Setup services & mod configuration
            ModConfig = helper.ReadConfig<ModConfig>();

            mailDeliveryService = new MailDeliveryService();

            // Start services
            mailDeliveryService.Start();
        }
    }
}
