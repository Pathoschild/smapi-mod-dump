/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using StardewValley.Locations;

namespace Slothsoft.Informant.Implementation.Decorator;

internal class BundleDecorator : IDecorator<Item> {

    private static Texture2D? _bundle;
    
    private readonly IModHelper _modHelper;
    
    public BundleDecorator(IModHelper modHelper) {
        _modHelper = modHelper;
        _bundle ??= modHelper.ModContent.Load<Texture2D>("assets/bundle.png");
    }

    public string Id => "bundles";
    public string DisplayName => _modHelper.Translation.Get("BundleTooltipDecorator");
    public string Description => _modHelper.Translation.Get("BundleTooltipDecorator.Description");

    public bool HasDecoration(Item input) {
        if (_bundle != null && input is SObject obj && !obj.bigCraftable.Value) {

            int[]? allowedAreas;
            
            if (!Game1.player.mailReceived.Contains("canReadJunimoText")) {
                // if player can't read Junimo text, they can't have bundles yet
                allowedAreas = null;
            } else {
                // let the community center calculate which bundles are allowed
                var communityCenter = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
                allowedAreas = communityCenter?.areasComplete
                    .Select((complete, index) => new { complete, index})
                    .Where(area => communityCenter.shouldNoteAppearInArea(area.index) && !area.complete)
                    .Select(area => area.index)
                    .ToArray();
            }
            
            return GetNeededItems(allowedAreas, InformantMod.Instance?.Config.DecorateLockedBundles ?? false).Contains(input.ParentSheetIndex);
        }
        return false;
    }

    internal static IEnumerable<int> GetNeededItems(int[]? allowedAreas, bool decorateLockedBundles) {
        // BUNDLE DATA
        // ============
        // See https://stardewvalleywiki.com/Modding:Bundles
        // The "main" data of the bundle has three values per item:
        // ParentSheetIndex Stack Quality (-> BundleGenerator.ParseItemList)
        //
        // Examples:
        //
        // bundleTitle = Pantry/0
        // bundleData = Spring Crops/O 465 20/24 1 0 188 1 0 190 1 0 192 1 0/0/4/0
        //
        // bundleTitle = Boiler Room/22
        // bundleData = Adventurer's/R 518 1/766 99 0 767 10 0 768 1 0 881 10 0/1/2/22

        if ((allowedAreas == null || allowedAreas.Length == 0) && !decorateLockedBundles) {
            // no areas are allowed, and we don't decorate locked bundles; so no bundle is needed yet
            yield break;
        }

        var bundleData = Game1.netWorldState.Value.BundleData;
        var bundlesCompleted = Game1.netWorldState.Value.Bundles.Pairs
            .ToDictionary(p => p.Key, p => p.Value.ToArray());

        foreach (var bundleTitle in bundleData.Keys) {
            var bundleTitleSplit = bundleTitle.Split('/');
            var bundleTitleId = bundleTitleSplit[0];
            if ((allowedAreas != null && !allowedAreas.Contains(CommunityCenter.getAreaNumberFromName(bundleTitleId))) && !decorateLockedBundles) {
                // bundle was not yet unlocked or already completed
                continue;
            }
            var bundleIndex = Convert.ToInt32(bundleTitleSplit[1]);
            var bundleDataSplit = bundleData[bundleTitle].Split('/');
            var indexStackQuality = bundleDataSplit[2].Split(' ');
            for (var index = 0; index < indexStackQuality.Length; index += 3) {
                if (!bundlesCompleted[bundleIndex][index / 3]) {
                    var parentSheetIndex = Convert.ToInt32(indexStackQuality[index]);
                    if (parentSheetIndex > 0) {
                        yield return parentSheetIndex;
                    }
                }
            }
        }
    }
    
    public Decoration Decorate(Item input) {
        return new Decoration(_bundle!);
    }
}