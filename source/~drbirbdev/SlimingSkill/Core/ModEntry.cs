/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewModdingAPI;
using BirbShared.APIs;
using BirbShared.Mod;
using SpaceCore;

namespace SlimingSkill
{
    public class ModEntry : Mod
    {
        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiConfig]
        internal static Config Config;
        [SmapiCommand]
        internal static Command Command;
        [SmapiAsset]
        internal static Assets Assets;
        [SmapiApi(UniqueID = "DaLion.Overhaul", IsRequired = false)]
        internal static IMargo MargoAPI;
        internal static bool MargoLoaded
        {
            get
            {
                if (MargoAPI is null)
                {
                    return false;
                }
                IMargo.IModConfig config = MargoAPI.GetConfig();
                return config.EnableProfessions;
            }
        }

        internal ITranslationHelper I18n => this.Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this, true);
            mod.ApisLoaded += this.ModClassParser_ApisLoaded;

            this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        }



        private void ModClassParser_ApisLoaded(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            Skills.RegisterSkill(new SlimingSkill());
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if (MargoLoaded)
            {
                string id = Skills.GetSkill(SlimingSkill.ID).Id;
                MargoAPI.RegisterCustomSkillForPrestige(id);
            }
        }
    }
}
