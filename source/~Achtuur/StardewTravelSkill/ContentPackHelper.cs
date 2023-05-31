/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using ContentPatcher;
using SpaceCore;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewTravelSkill
{
    internal class ContentPackHelper
    {
        public ModEntry Instance;
        public static IContentPatcherAPI ContentPatcherAPI;

        public ContentPackHelper(ModEntry instance)
        {
            this.Instance = instance;
            ContentPackHelper.ContentPatcherAPI = this.Instance.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
        }

        public void CreateTokens()
        {
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionMovespeed", hasMovespeed);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionRestoreStamina", hasRestoreStamina);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionSprint", hasSprint);

            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionCheaperWarpTotems", hasCheaperWarpTotem);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionCheaperObelisks", hasCheaperObelisk);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionTotemReuse", hasTotemReuse);
        }

        private IEnumerable<string> hasMovespeed()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionMovespeed).ToString() };
        }

        private IEnumerable<string> hasRestoreStamina()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina).ToString() };
        }

        private IEnumerable<string> hasSprint()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionSprint).ToString() };
        }

        private IEnumerable<string> hasCheaperWarpTotem()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem).ToString() };
        }

        private IEnumerable<string> hasTotemReuse()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk).ToString() };
        }

        private IEnumerable<string> hasCheaperObelisk()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionTotemReuse).ToString() };
        }

    }
}
