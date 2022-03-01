/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using StardewModdingAPI;
using System;

namespace BetterBeehouses
{
    class Config
    {
        internal enum UsableOptions { Outdoors, Greenhouse, Anywhere }
        internal enum ProduceWhere { Never, Indoors, Always }
        public ProduceWhere ProduceInWinter { get; set; } = ProduceWhere.Indoors;
        public ProduceWhere UsePottedFlowers { get; set; } = ProduceWhere.Always;
        public UsableOptions UsableIn { get; set; } = UsableOptions.Greenhouse;
        public int DaysToProduce { get; set; } = 4;
        public int FlowerRange { get; set; } = 5;
        public bool UseForageFlowers { get; set; } = false;
        public float ValueMultiplier { get; set; } = 1f;
        public bool Particles { get; set; } = true;
        public bool UseQuality { get; set; } = false;

        private ITranslationHelper i18n => ModEntry.helper.Translation;

        public void ResetToDefault()
        {
            ProduceInWinter = ProduceWhere.Indoors;
            UsePottedFlowers = ProduceWhere.Always;
            UsableIn = UsableOptions.Greenhouse;
            DaysToProduce = 4;
            FlowerRange = 5;
            ValueMultiplier = 1f;
            UseForageFlowers = false;
            Particles = true;
            UseQuality = false;
        }

        public void ApplyConfig()
        {
            ModEntry.helper.WriteConfig(this);
            ModEntry.helper.Content.InvalidateCache("Mods/aedenthorn.ParticleEffects/dict");
        }

        public void RegisterModConfigMenu(IManifest manifest)
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;

            var api = ModEntry.helper.ModRegistry.GetApi<integration.IGMCMAPI>("spacechase0.GenericModConfigMenu");

            api.Register(manifest, ResetToDefault, ApplyConfig);

            api.AddSectionTitle(manifest, () => i18n.Get("config.title"));
            api.AddNumberOption(manifest,
                () => DaysToProduce,
                (n) => DaysToProduce = n,
                () => i18n.Get("config.daysToProduce.name"),
                () => i18n.Get("config.daysToProduce.desc"),
                1, 7
            );
            api.AddNumberOption(manifest,
                () => FlowerRange,
                (n) => FlowerRange = n,
                () => i18n.Get("config.flowerRange.name"),
                () => i18n.Get("config.flowerRange.desc"),
                1, 14
            );
            api.AddTextOption(manifest,
                () => UsableIn.ToString(),
                (s) => UsableIn = (UsableOptions)Enum.Parse(typeof(UsableOptions), s),
                () => i18n.Get("config.usableIn.name"),
                () => i18n.Get("config.usableIn.desc"),
                Enum.GetNames(typeof(UsableOptions)),
                (s) => TranslatedOption("usableOptions",s)
            ); 
            api.AddTextOption(manifest,
                 () => ProduceInWinter.ToString(),
                 (s) => ProduceInWinter = (ProduceWhere)Enum.Parse(typeof(ProduceWhere), s),
                 () => i18n.Get("config.produceInWinter.name"),
                 () => i18n.Get("config.produceInWinter.desc"),
                 Enum.GetNames(typeof(ProduceWhere)),
                 (s) => TranslatedOption("produceWhere", s)
            );
            api.AddTextOption(manifest,
                 () => UsePottedFlowers.ToString(),
                 (s) => UsePottedFlowers = (ProduceWhere)Enum.Parse(typeof(ProduceWhere), s),
                 () => i18n.Get("config.usePottedFlowers.name"),
                 () => i18n.Get("config.usePottedFlowers.desc"),
                 Enum.GetNames(typeof(ProduceWhere)),
                 (s) => TranslatedOption("produceWhere", s)
            );
            api.AddNumberOption(manifest,
                () => ValueMultiplier,
                (n) => ValueMultiplier = n,
                () => i18n.Get("config.valueMultiplier.name"),
                () => i18n.Get("config.valueMultiplier.desc"),
                .1f, 2f, .1f
            );
            api.AddBoolOption(manifest,
                () => UseForageFlowers,
                (b) => UseForageFlowers = b,
                () => i18n.Get("config.useForage.name"),
                () => i18n.Get("config.useForage.desc")
            );
            api.AddBoolOption(manifest,
                () => UseQuality,
                (b) => UseQuality = b,
                () => i18n.Get("config.useQuality.name"),
                () => i18n.Get("config.useQuality.desc")
            );
            if (ModEntry.helper.ModRegistry.IsLoaded("aedenthorn.ParticleEffects"))
                api.AddBoolOption(manifest,
                    () => Particles,
                    (b) => Particles = b,
                    () => i18n.Get("config.particles.name"),
                    () => i18n.Get("config.particles.desc")
                );
        }
        public string TranslatedOption(string enumName, string value)
        {
            return i18n.Get("config." + enumName + "." + value);
        }
        public Config()
        {
            ResetToDefault();
        }
    }
}
