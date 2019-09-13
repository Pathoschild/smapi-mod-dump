using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to add custom mails to the game's content mail asset.
    /// </summary>
    internal class MailAssetEditor : AssetEditor
    {
        /// <summary>Stardew Valley's mail asset location in its content folder.</summary>
        private const string STARDEW_VALLEY_MAIL_DATA = "Data/mail";

        /// <summary>The mail data to add to the game's mail asset cache.</summary>
        private readonly List<MailAssetDataEntry> mailAssetData = new List<MailAssetDataEntry>();

        /// <summary>
        /// Raised when the game begins to reload its mail asset cache.
        /// </summary>
        /// <remarks>
        /// Use this event to setup the mail data you want to be added to the asset during this reload process. 
        /// Call <see cref="AddMailAssetData"/> to pass the prepared data to this asset editor so it can be added.
        /// </remarks>
        public event EventHandler<MailAssetLoadingEventArgs> MailAssetLoading;

        /// <summary>
        /// Create a new instance of the <see cref="MailAssetEditor"/> class.
        /// </summary>
        public MailAssetEditor() 
            : base(STARDEW_VALLEY_MAIL_DATA) { }

        /// <summary>
        /// Edit a matched asset.
        /// </summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public override void Edit<T>(IAssetData asset)
        {
            MailAssetLoading?.Invoke(this, new MailAssetLoadingEventArgs());

            IDictionary<string, string> mails = asset.AsDictionary<string, string>().Data;
            foreach (var mailData in mailAssetData)
            {
                mails[mailData.Id] = mailData.Content;
            }

            mailAssetData.Clear();
        }

        /// <summary>
        /// Add mail data for this asset editor to add to the game's mail asset cache. The data is added during the next 
        /// reload of the game's mail asset cache.
        /// </summary>
        /// <param name="data">The mail data to add to the game's mail asset cache.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="data"/> is <c>null</c>.</exception>
        public void AddMailAssetData(List<MailAssetDataEntry> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            mailAssetData.AddRange(data);
        }
    }
}
