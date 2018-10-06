using StardewModdingAPI;
using StardewMods.ArchaeologyHouseContentManagementHelper.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.Common
{
    public class CommonServices
    {
        public CommonServices(IMonitor monitor, ITranslationHelper translationHelper, IReflectionHelper reflectionHelper, IContentHelper contentHelper)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            TranslationHelper = translationHelper ?? throw new ArgumentNullException(nameof(translationHelper));
            ReflectionHelper = reflectionHelper ?? throw new ArgumentNullException(nameof(reflectionHelper));
            ContentHelper = contentHelper ?? throw new ArgumentNullException(nameof(contentHelper));
        }

        public IMonitor Monitor { get; }

        public ITranslationHelper TranslationHelper { get; }

        public IReflectionHelper ReflectionHelper { get; }
        public IContentHelper ContentHelper { get; }
    }
}
