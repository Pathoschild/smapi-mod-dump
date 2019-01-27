using StardewModdingAPI;

namespace priceDrops
{
    internal class mailEditor : IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>Provides translations stored for the mod.</summary>
        private readonly ITranslationHelper translations;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">Provides translations stored for the mod.</param>
        public mailEditor(ITranslationHelper translations)
        {
            this.translations = translations;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Mail");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            var text = this.translations;

            // get dwarf discount terms
            string dwarfDisc1 = this.GetDwarfDiscountTerm(ModEntry.DISC_1);
            string dwarfDisc2 = this.GetDwarfDiscountTerm(ModEntry.DISC_2);
            string dwarfDisc3 = this.GetDwarfDiscountTerm(ModEntry.DISC_3);

            // inject letters
            var data = asset.AsDictionary<string, string>().Data;
            data["robin1"] = text.Get("letter.robin1", new { discount = ModEntry.DISC_1 });
            data["robin2"] = text.Get("letter.robin2", new { discount = ModEntry.DISC_2 });
            data["robin3"] = text.Get("letter.robin3", new { discount = ModEntry.DISC_3 });
            data["robinMaru"] = text.Get("letter.robin-maru", new { discount = ModEntry.BONUS_DISC });
            data["robinSebastian"] = text.Get("letter.robin-sebastian", new { discount = ModEntry.BONUS_DISC });

            data["marnie1"] = text.Get("letter.marnie1", new { discount = ModEntry.DISC_1 });
            data["marnie2"] = text.Get("letter.marnie2", new { discount = ModEntry.DISC_2 });
            data["marnie3"] = text.Get("letter.marnie3", new { discount = ModEntry.DISC_3 });
            data["marnieShane"] = text.Get("letter.marnie-shane", new { discount = ModEntry.BONUS_DISC });

            data["pierre1"] = text.Get("letter.pierre1", new { discount = ModEntry.DISC_1 });
            data["pierre2"] = text.Get("letter.pierre2", new { discount = ModEntry.DISC_2 });
            data["pierre3"] = text.Get("letter.pierre3", new { discount = ModEntry.DISC_3 });
            data["pierreAbigail"] = text.Get("letter.pierre-abigail", new { discount = ModEntry.BONUS_DISC });
            data["pierreCaroline"] = text.Get("letter.pierre-caroline", new { discount = ModEntry.BONUS_DISC });

            data["harvey1"] = text.Get("letter.harvey1", new { discount = ModEntry.DISC_1 });
            data["harvey2"] = text.Get("letter.harvey2", new { discount = ModEntry.DISC_2 });
            data["harvey3"] = text.Get("letter.harvey3", new { discount = ModEntry.DISC_3 });
            data["harveyMarried"] = text.Get("letter.harvey-married", new { discount = ModEntry.BONUS_DISC });

            data["gus1"] = text.Get("letter.gus1", new { discount = ModEntry.DISC_1 });
            data["gus2"] = text.Get("letter.gus2", new { discount = ModEntry.DISC_2 });
            data["gus3"] = text.Get("letter.gus3", new { discount = ModEntry.DISC_3 });

            data["clint1"] = text.Get("letter.clint1", new { discount = ModEntry.DISC_1 });
            data["clint2"] = text.Get("letter.clint2", new { discount = ModEntry.DISC_2 });
            data["clint3"] = text.Get("letter.clint3", new { discount = ModEntry.DISC_3 });

            data["sandy1"] = text.Get("letter.sandy1", new { discount = ModEntry.DISC_1 });
            data["sandy2"] = text.Get("letter.sandy2", new { discount = ModEntry.DISC_2 });
            data["sandy3"] = text.Get("letter.sandy3", new { discount = ModEntry.DISC_3 });

            data["willy1"] = text.Get("letter.willy1", new { discount = ModEntry.DISC_1 });
            data["willy2"] = text.Get("letter.willy2", new { discount = ModEntry.DISC_2 });
            data["willy3"] = text.Get("letter.willy3", new { discount = ModEntry.DISC_3 });

            data["dwarf1"] = text.Get("letter.dwarf1", new { discountTerm = dwarfDisc1 });
            data["dwarf2"] = text.Get("letter.dwarf2", new { discountTerm = dwarfDisc2 });
            data["dwarf3"] = text.Get("letter.dwarf3", new { discountTerm = dwarfDisc3 });

            data["krobus1"] = text.Get("letter.krobus1", new { discountTerm = dwarfDisc1 });
            data["krobus2"] = text.Get("letter.krobus2", new { discountTerm = dwarfDisc2 });
            data["krobus3"] = text.Get("letter.krobus3", new { discountTerm = dwarfDisc3 });

            data["wizard1"] = text.Get("letter.wizard1", new { discount = ModEntry.DISC_1 });
            data["wizard2"] = text.Get("letter.wizard2", new { discount = ModEntry.DISC_2 });
            data["wizard3"] = text.Get("letter.wizard3", new { discount = ModEntry.DISC_3 });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the translated text for a dwarf discount, like 'a fifth'.</summary>
        /// <param name="percentage">The percentage discount to represent.</param>
        private string GetDwarfDiscountTerm(int percentage)
        {
            ITranslationHelper text = this.translations;
            switch (percentage)
            {
                case 10:
                    return text.Get("discount-term.tenth");
                case 20:
                    return text.Get("discount-term.fifth");
                case 25:
                    return text.Get("discount-term.quarter");
                case 50:
                    return text.Get("discount-term.half");
                case 75:
                    return text.Get("discount-term.three-quarter");
                default:
                    return text.Get("discount-term.other", new { discount = percentage });
            }
        }
    }
}