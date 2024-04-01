/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using HarmonyLib;
using PlatonicRelationships.Framework;
using StardewModdingAPI.Events;

namespace PlatonicRelationships
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private readonly AddDatingPrereq Editor = new AddDatingPrereq();

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (this.Config.AddDatingRequirementToRomanticEvents)
                helper.Events.Content.AssetRequested += this.OnAssetRequested;

            //apply harmony patches
            this.ApplyPatches();
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (this.Editor.CanEdit(e.NameWithoutLocale))
                e.Edit(this.Editor.Edit);
        }

        public void ApplyPatches()
        {
            var harmony = new Harmony("cherry.platonicrelationships");

            try
            {
                this.Monitor.Log("Transpile patching SocialPage.drawNPCSlotHeart");
                harmony.Patch(
                    original: AccessTools.Method(typeof(SocialPage), name: "drawNPCSlotHeart"),
                    prefix: new HarmonyMethod(methodType: typeof(PatchDrawNpcSlotHeart), nameof(PatchDrawNpcSlotHeart.Prefix))
                );
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed in Patching SocialPage.drawNPCSlotHeart: \n{e}", LogLevel.Error);
                return;
            }

            try
            {
                this.Monitor.Log("Postfix patching Utility.GetMaximumHeartsForCharacter");
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), name: "GetMaximumHeartsForCharacter"),
                    postfix: new HarmonyMethod(typeof(patchGetMaximumHeartsForCharacter), nameof(patchGetMaximumHeartsForCharacter.Postfix))
                );
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed in Patching Utility.GetMaximumHeartsForCharacter: \n{e}", LogLevel.Error);
                return;
            }
        }
    }
}
