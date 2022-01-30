/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

using Events.Display;
using Events.GameLoop;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Manages treasure hunt events for Scavenger professions.</summary>
internal class ScavengerHunt : TreasureHunt
{
    private readonly IEnumerable<int> _artifactsThatCanBeFound = new HashSet<int>
    {
        100, // chipped amphora
        101, // arrowhead
        103, // ancient doll
        109, // ancient sword
        113, // chicken statue
        114, // ancient seed
        115, // prehistoric tool
        119, // bone flute
        120, // prehistoric handaxe
        123, // ancient drum
        124, // golden mask
        125, // golden relic
        126, // strange doll
        127, // strange doll
        588 // palm fossil
    };

    #region public methods

    /// <summary>Construct an instance.</summary>
    public ScavengerHunt()
    {
        huntStartedMessage = ModEntry.ModHelper.Translation.Get("scavenger.huntstarted");
        huntFailedMessage = ModEntry.ModHelper.Translation.Get("scavenger.huntfailed");
        iconSourceRect = new(80, 656, 16, 16);
    }

    /// <summary>Try to start a new scavenger hunt at this location.</summary>
    /// <param name="location">The game location.</param>
    public override void TryStartNewHunt(GameLocation location)
    {
        if (!base.TryStartNewHunt()) return;

        TreasureTile = ChooseTreasureTile(location);
        if (TreasureTile is null) return;

        location.MakeTileDiggable(TreasureTile.Value);
        timeLimit = (uint) (location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                            ModEntry.Config.ScavengerHuntHandicap);
        timeLimit = Math.Max(timeLimit, 30);

        elapsed = 0;
        EventManager.Enable(typeof(IndicatorUpdateTickedEvent), typeof(ScavengerHuntRenderedHudEvent),
            typeof(ScavengerHuntUpdateTickedEvent));
        Game1.addHUDMessage(new HuntNotification(huntStartedMessage, iconSourceRect));
    }

    /// <inheritdoc />
    public override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 v;
        var failsafe = 0;
        do
        {
            if (failsafe > 69) return null;

            var x = random.Next(location.Map.DisplayWidth / Game1.tileSize);
            var y = random.Next(location.Map.DisplayHeight / Game1.tileSize);
            v = new(x, y);
            ++failsafe;
        } while (!location.IsTileValidForTreasure(v));

        return v;

        //var candidates = Tiles.FloodFill(Game1.player.getTileLocation(), location.Map.DisplayWidth / Game1.tileSize,
        //    location.Map.DisplayHeight / Game1.tileSize, location.IsTileValidForTreasure);
        //if (candidates.Count > 0) return candidates.ElementAt(random.Next(candidates.Count));

        //return null;
    }

    /// <inheritdoc />
    public override void Fail()
    {
        End();
        Game1.addHUDMessage(new HuntNotification(huntFailedMessage));
        ModData.Write(DataField.ScavengerHuntStreak, "0");
    }

    #endregion public methods

    #region protected methods

    /// <inheritdoc />
    protected override void CheckForCompletion()
    {
        if (Game1.player.CurrentTool is not Hoe || !Game1.player.UsingTool) return;

        var actionTile = new Vector2((int) (Game1.player.GetToolLocation().X / Game1.tileSize),
            (int) (Game1.player.GetToolLocation().Y / Game1.tileSize));
        if (TreasureTile is null || actionTile != TreasureTile.Value) return;

        End();
        var getTreasure = new DelayedAction(200, BeginFindTreasure);
        Game1.delayedActions.Add(getTreasure);
        ModData.Increment<uint>(DataField.ScavengerHuntStreak);
    }

    /// <inheritdoc />
    protected override void End()
    {
        EventManager.Disable(typeof(ScavengerHuntRenderedHudEvent), typeof(ScavengerHuntUpdateTickedEvent));
        TreasureTile = null;
    }

    #endregion protected methods

    #region private methods

    /// <summary>Play treasure chest found animation.</summary>
    private void BeginFindTreasure()
    {
        Game1.currentLocation.TemporarySprites.Add(new(
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"), new(64, 1920, 32, 32), 500f, 1, 0,
            Game1.player.Position + new Vector2(-32f, -160f), false, false,
            Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
        {
            motion = new(0f, -0.128f),
            timeBasedMotion = true,
            endFunction = OpenChestEndFunction,
            extraInfoForEndBehavior = 0,
            alpha = 0f,
            alphaFade = -0.002f
        });
    }

    /// <summary>Play open treasure chest animation.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenChestEndFunction(int extra)
    {
        Game1.currentLocation.localSound("openChest");
        Game1.currentLocation.TemporarySprites.Add(new(
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"), new(64, 1920, 32, 32), 200f, 4, 0,
            Game1.player.Position + new Vector2(-32f, -228f), false, false,
            Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
        {
            endFunction = OpenTreasureMenuEndFunction,
            extraInfoForEndBehavior = 0
        });
    }

    /// <summary>Open the treasure chest menu.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenTreasureMenuEndFunction(int extra)
    {
        Game1.player.completelyStopAnimatingOrDoingAction();
        var treasures = GetTreasureContents();
        Game1.activeClickableMenu = new ItemGrabMenu(treasures).setEssential(true);
        ((ItemGrabMenu) Game1.activeClickableMenu).source = 3;
    }

    /// <summary>Choose the contents of the treasure chest.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private List<Item> GetTreasureContents()
    {
        List<Item> treasures = new();
        var chance = 1.0;
        while (random.NextDouble() <= chance)
        {
            chance *= 0.4f;
            if (Game1.currentSeason == "spring" && !(Game1.currentLocation is Beach) && random.NextDouble() < 0.1)
                treasures.Add(new SObject(273,
                    random.Next(2, 6) + (random.NextDouble() < 0.25 ? 5 : 0))); // rice shoot

            if (random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                treasures.Add(new SObject(890,
                    random.Next(1, 3) + (random.NextDouble() < 0.25 ? 2 : 0))); // qi beans

            switch (random.Next(4))
            {
                case 0:
                    List<int> possibles = new();
                    if (random.NextDouble() < 0.4) possibles.Add(386); // iridium ore

                    if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(384); // gold ore

                    if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(380); // iron ore

                    if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(378); // copper ore

                    if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(388); // wood

                    if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(390); // stone

                    possibles.Add(382); // coal
                    treasures.Add(new SObject(possibles.ElementAt(random.Next(possibles.Count)),
                        random.Next(2, 7) *
                        (!(random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.015) ? 1 : 2)));
                    if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03) treasures.Last().Stack *= 2;

                    break;

                case 1:
                    if (random.NextDouble() < 0.25 && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
                        treasures.Add(new SObject(774, 5 + (random.NextDouble() < 0.25 ? 5 : 0))); // wild bait
                    else
                        treasures.Add(new SObject(685, 10)); // bait

                    break;

                case 2:
                    if (random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound.Value < 21 &&
                        Game1.player.hasOrWillReceiveMail("lostBookFound"))
                        treasures.Add(new SObject(102, 1)); // lost book
                    else if (Game1.player.archaeologyFound.Any()) // artifacts
                        treasures.Add(new SObject(
                            random.NextDouble() < 0.5
                                ? _artifactsThatCanBeFound.ElementAt(random.Next(_artifactsThatCanBeFound.Count()))
                                : random.NextDouble() < 0.25
                                    ? 114
                                    : 535, 1));
                    else
                        treasures.Add(new SObject(382, random.Next(1, 3))); // coal

                    break;

                case 3:
                    switch (random.Next(3))
                    {
                        case 0:
                            treasures.Add(new SObject(random.Next(535, 538), random.Next(1, 4))); // geodes
                            if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
                                treasures.Last().Stack *= 2;

                            break;

                        case 1:
                            switch (random.Next(4))
                            {
                                case 0: // fire quartz else ruby or emerald
                                    treasures.Add(new SObject(
                                        random.NextDouble() < 0.3 ? 82 : random.NextDouble() < 0.5 ? 64 : 60,
                                        random.Next(1, 3)));
                                    break;

                                case 1: // frozen tear else jade or aquamarine
                                    treasures.Add(new SObject(
                                        random.NextDouble() < 0.3 ? 84 : random.NextDouble() < 0.5 ? 70 : 62,
                                        random.Next(1, 3)));
                                    break;

                                case 2: // earth crystal else amethyst or topaz
                                    treasures.Add(new SObject(
                                        random.NextDouble() < 0.3 ? 86 : random.NextDouble() < 0.5 ? 66 : 68,
                                        random.Next(1, 3)));
                                    break;

                                case 3:
                                    treasures.Add(random.NextDouble() < 0.28
                                        ? new(72, 1) // diamond
                                        : new SObject(80, random.Next(1, 3))); // quartz
                                    break;
                            }

                            if (random.NextDouble() < 0.05) treasures.Last().Stack *= 2;

                            break;

                        case 2:
                            var luckModifier = 1.0 + Game1.player.DailyLuck * 10;
                            var streak = ModData.ReadAs<uint>(DataField.ScavengerHuntStreak);
                            if (random.NextDouble() < 0.025 * luckModifier &&
                                !Game1.player.specialItems.Contains(15))
                                treasures.Add(new MeleeWeapon(15) {specialItem = true}); // forest sword

                            if (random.NextDouble() < 0.025 * luckModifier &&
                                !Game1.player.specialItems.Contains(20))
                                treasures.Add(new MeleeWeapon(20) {specialItem = true}); // elf blade

                            if (random.NextDouble() < 0.07 * luckModifier)
                                switch (random.Next(3))
                                {
                                    case 0:
                                        treasures.Add(new Ring(516 +
                                                               (random.NextDouble() < Game1.player.LuckLevel / 11f
                                                                   ? 1
                                                                   : 0))); // (small) glow ring
                                        break;

                                    case 1:
                                        treasures.Add(new Ring(518 +
                                                               (random.NextDouble() < Game1.player.LuckLevel / 11f
                                                                   ? 1
                                                                   : 0))); // (small) magnet ring
                                        break;

                                    case 2:
                                        treasures.Add(new Ring(random.Next(529, 535))); // gemstone ring
                                        break;
                                }

                            if (random.NextDouble() < 0.02 * luckModifier)
                                treasures.Add(new SObject(166, 1)); // treasure chest

                            if (random.NextDouble() < 0.005 * luckModifier * Math.Pow(2, streak))
                                treasures.Add(new SObject(74, 1)); // prismatic shard

                            if (random.NextDouble() < 0.01 * luckModifier)
                                treasures.Add(new SObject(126, 1)); // strange doll

                            if (random.NextDouble() < 0.01 * luckModifier)
                                treasures.Add(new SObject(127, 1)); // strange doll

                            if (random.NextDouble() < 0.01 * luckModifier)
                                treasures.Add(new Ring(527)); // iridium band

                            if (random.NextDouble() < 0.01 * luckModifier)
                                treasures.Add(new Boots(random.Next(504, 514))); // boots

                            if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
                                random.NextDouble() < 0.01 * luckModifier)
                                treasures.Add(new SObject(928, 1)); // golden egg

                            if (treasures.Count == 1) treasures.Add(new SObject(72, 1)); // consolation diamond

                            break;
                    }

                    break;
            }
        }

        if (random.NextDouble() < 0.4)
            switch (Game1.currentSeason) // forage seeds
            {
                case "spring":
                    treasures.Add(new SObject(495, 1));
                    break;

                case "summer":
                    treasures.Add(new SObject(496, 1));
                    break;

                case "fall":
                    treasures.Add(new SObject(497, 1));
                    break;

                case "winter":
                    treasures.Add(new SObject(498, 1));
                    break;
            }
        else
            treasures.Add(new SObject(770, random.Next(1, 4) * 5)); // wild seeds

        return treasures;
    }

    #endregion private methods
}