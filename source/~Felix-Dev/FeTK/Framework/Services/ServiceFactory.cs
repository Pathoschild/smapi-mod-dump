using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// The <see cref="ServiceFactory"/> class provides an API to access different services a consuming mod can use. 
    /// </summary>
    /// <remarks>
    /// This class uses the Singleton pattern: Each consuming mod can have exactly one <see cref="ServiceFactory"/> instance.
    /// </remarks>
    public class ServiceFactory
    {
        /// <summary>Contains the created <see cref="ServiceFactory"/> instances. Maps a mod (via the mod ID) to its service factory instance.</summary>
        private static readonly IDictionary<string, ServiceFactory> serviceFactories = new Dictionary<string, ServiceFactory>();

        /// <summary>The internal <see cref="IMailManager"/> instance to use for <see cref="IMailService"/> instances.</summary>
        private static IMailManager mailManager;

        /// <summary>The unique ID of the mod for which this service factory was created.</summary>
        private readonly string modId;

        /// <summary>
        /// Contains the created <see cref="MailService"/> instance for a service factory. Each factory has at most one instance.
        /// </summary>
        private IMailSender mailService;

        /// <summary>
        /// Get an instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        /// <param name="modId">The unique ID of the mod requesting the service factory.</param>
        /// <returns>A service factory for the specified mod.</returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="modId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        public static ServiceFactory GetFactory(string modId)
        {
            if (string.IsNullOrWhiteSpace(modId))
            {
                throw new ArgumentException(nameof(modId));
            }

            if (!serviceFactories.TryGetValue(modId, out ServiceFactory serviceFactory))
            {
                serviceFactories.Add(modId, serviceFactory = new ServiceFactory(modId));
            }

            return serviceFactory;
        }

        /// <summary>
        /// Get an <see cref="IMailService"/> instance.
        /// </summary>
        /// <returns>
        /// An existing <see cref="IMailService"/> instance if available; otherwise a fresh instance.
        /// </returns>
        public IMailService GetMailService()
        {
            if (this.mailService == null)
            {
                this.mailService = new MailService(modId, mailManager);

                mailManager.RegisterMailSender(modId, mailService);
            }

            return mailService;
        }

        /// <summary>
        /// Setup work data for <see cref="ServiceFactory"/> instances.
        /// </summary>
        /// <param name="mailManager">The mail manager to use for <see cref="IMailService"/> instances.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mailManager"/> is <c>null</c>.</exception>
        internal static void Setup(IMailManager mailManager)
        {
            ServiceFactory.mailManager = mailManager ?? throw new ArgumentNullException(nameof(mailManager));
        }

        /// <summary>
        /// Create a new instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        /// <param name="modId">The unique ID of the relevant mod.</param>
        private ServiceFactory(string modId)
        {
            this.modId = modId;
        }
    }
}
