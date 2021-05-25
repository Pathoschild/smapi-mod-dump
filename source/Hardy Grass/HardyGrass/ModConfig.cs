/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiscipleOfEris/HardyGrass
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace HardyGrass
{
    public class ModConfig
    {
        public float vanillaHardiness = 1.0f;
        public float vanillaAverageTufts = 2.0f;
        public float vanillaCutAverageTufts = 0.65f;
        public float vanillaSpreadAverageTufts = 1.3f;
        public float quickHardiness = 1.0f;
        public float quickAverageTufts = 4.0f;
        public float quickCutAverageTufts = 1.3f;
        public float quickSpreadAverageTufts = 2.6f;
        public bool simplifyGrassGrowth = true;
        public bool shortGrassStarters = true;
        public bool fixAnimalsEating = true;

        public static void Initialize(ModEntry mod)
        {
            var helper = mod.Helper;
            var manifest = mod.ModManifest;
            var config = ModEntry.config;

            var api = helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(manifest, () => config = ModEntry.config = new ModConfig(), () => helper.WriteConfig(config));

            api.RegisterClampedOption(manifest, "Vanilla Grass Hardiness", "Chance for vanilla grass to survive reaping or eating. Set to 0 for unmodded behavior. 1 recommended (for 100% chance).", () => config.vanillaHardiness, (float val) => config.vanillaHardiness = val, 0, 1);
            api.RegisterClampedOption(manifest, "Vanilla Grass Tufts Per Day", "Average number of tufts a patch of vanilla grass will grow each day. Set to 2 for unmodded behavior.", () => config.vanillaAverageTufts, (float val) => config.vanillaAverageTufts = val, 0, 4);
            api.RegisterClampedOption(manifest, "Vanilla Grass Tufts When Cut", "Average number of tufts a patch of vanilla grass will grow each day when it's completely cut. 0.65 recommended.", () => config.vanillaCutAverageTufts, (float val) => config.vanillaCutAverageTufts = val, 0, 4);
            api.RegisterClampedOption(manifest, "Vanilla Grass Tufts To Spread", "Average number of tufts a full patch of vanilla grass will spread to adjacent tiles each day, assuming all adjacent tiles are empty. Set to 1.3 for unmodded behavior.", () => config.vanillaSpreadAverageTufts, (float val) => config.vanillaSpreadAverageTufts = val, 0, 4);
            api.RegisterClampedOption(manifest, "Quick Grass Hardiness", "Chance for Quick Grass to survive reaping or eating. 1 recommended (for 100% chance).", () => config.quickHardiness, (float val) => config.quickHardiness = val, 0, 1);
            api.RegisterClampedOption(manifest, "Quick Grass Tufts Per Day", "Average number of tufts a patch of Quick Grass will grow each day. 4 recommended.", () => config.quickAverageTufts, (float val) => config.quickAverageTufts = val, 0, 4);
            api.RegisterClampedOption(manifest, "Quick Grass Tufts When Cut", "Average number of tufts a patch of Quick Grass will grow each day when it's completely cut. 1.3 recommended.", () => config.quickCutAverageTufts, (float val) => config.quickCutAverageTufts = val, 0, 4);
            api.RegisterClampedOption(manifest, "Quick Grass Tufts To Spread", "Average number of tufts a full patch of Quick Grass will spread to adjacent tiles each day, assuming all adjacent tiles are empty. 2.6 recommended.", () => config.quickSpreadAverageTufts, (float val) => config.quickSpreadAverageTufts = val, 0, 4);
            api.RegisterSimpleOption(manifest, "Simplify Grass Growth", "Fixes vanilla spread chance also applying to growth rate when it shouldn't. Set to off for unmodded behavior. On recommended.", () => config.simplifyGrassGrowth, (bool val) => config.simplifyGrassGrowth = val);
            api.RegisterSimpleOption(manifest, "Grass Starters Plant Short", "When grass starters are planted they will start as short grass rather than a fully grown patch. Set to off for unmodded behavior. On recommended.", () => config.shortGrassStarters, (bool val) => config.shortGrassStarters = val);
            api.RegisterSimpleOption(manifest, "Fix Animal Eating Habits", "Allegedly barn animals eat 4 tufts of grass while coop animals eat 2 tufts. However, the unmodded game will make animals full when they eat at all, even if there's only a single tuft of grass left in the patch. This fixes that behavior, so make sure your grassy pen is big enough. If you set to off, you should change the \"Tufts When Cut\" options to one-quarter of their suggested values for balance purposes. On recommended.", () => config.fixAnimalsEating, (bool val) => config.fixAnimalsEating = val);
        }
    }
}
