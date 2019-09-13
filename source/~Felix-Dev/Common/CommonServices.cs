using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.Common
{
    public class CommonServices
    {
        public CommonServices(IMonitor monitor, IModEvents events, ITranslationHelper translationHelper, IReflectionHelper reflectionHelper, IContentHelper contentHelper,
            IDataHelper dataHelper)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            TranslationHelper = translationHelper ?? throw new ArgumentNullException(nameof(translationHelper));
            ReflectionHelper = reflectionHelper ?? throw new ArgumentNullException(nameof(reflectionHelper));
            ContentHelper = contentHelper ?? throw new ArgumentNullException(nameof(contentHelper));
            DataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
        }

        public IMonitor Monitor { get; }

        /// <summary>
        /// Access to events raised by SAMPI.
        /// </summary>
        public IModEvents Events { get; }

        public ITranslationHelper TranslationHelper { get; }

        public IReflectionHelper ReflectionHelper { get; }

        public IContentHelper ContentHelper { get; }

        public IDataHelper DataHelper { get; }
    }
}
