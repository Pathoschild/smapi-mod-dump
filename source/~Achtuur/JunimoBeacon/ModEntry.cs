/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Patches;
using AchtuurCore.Utility;
using JunimoBeacon.Patches;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

namespace JunimoBeacon;

public class ModEntry : Mod
{
    internal readonly string ContentPackPath = Path.Combine("assets", "ContentPack");
    internal static ModEntry Instance;
    internal ModConfig Config;


    internal PlaceBeaconOverlay placeBeaconOverlay;

    internal JsonAssets.IApi JsonAssetsAPI;

    internal List<JunimoGroup> JunimoGroups;


    /// <summary>
    /// Returns whether placing a beacon at <paramref name="beacon_position"/> would intersect a hut's range in one of the groups
    /// </summary>
    /// <param name="beacon_position"></param>
    public bool IsInRangeOfAnyGroup(Vector2 beacon_position)
    {
        return JunimoGroups.Any(group => group.IsInRange(beacon_position));
    }

    private void TryAddBeaconToGroups(JunimoBeacon beacon)
    {
        foreach (JunimoGroup group in JunimoGroups)
        {
            group.TryAddBeacon(beacon);
        }
    }

    private void RemoveGroupWithHut(JunimoHut hut)
    {
        this.JunimoGroups = this.JunimoGroups.Where(group => !group.Hut.Equals(hut)).ToList();
    }

    private void TryRemoveBeaconFromGroups(SObject beacon_object)
    {
        foreach (JunimoGroup group in JunimoGroups)
        {
            group.TryRemoveBeacon(beacon_object);
        }
    }

    private void TryAddJunimoGroup(JunimoHut hut)
    {
        if (JunimoGroups.Any(group => group.Hut == hut))
            return;

        JunimoGroups.Add(new JunimoGroup(hut));
    }

    /// <summary>
    /// Get the <see cref="JunimoGroup"/> from <see cref="JunimoGroups"/> that <paramref name="hut"/> is a part of.
    /// </summary>
    /// <param name="hut"></param>
    /// <returns></returns>
    internal JunimoGroup GetHutGroup(JunimoHut hut)
    {
        return JunimoGroups.Find(group => group.Hut.Equals(hut));
    }

    public override void Entry(IModHelper helper)
    {

        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new MatureCropsWithinRadiusPatcher(),
            new PathFindToNewCropPatcher(),
            new MaxJunimoCountPatcher()
        );

        this.Config = this.Helper.ReadConfig<ModConfig>();

        this.JunimoGroups = new List<JunimoGroup>();

        placeBeaconOverlay = new PlaceBeaconOverlay();


        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;

        /// Add/Remove beacons or groups
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
        helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;

        /// Rendering
        helper.Events.Display.RenderedWorld += this.OnRenderedWorld;

        /// Debug
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (!e.Name.IsEquivalentTo("Tilesheets/Craftables") 
            || JunimoBeacon.ID is null 
            || !Context.IsWorldReady
            || (Context.IsSplitScreen && !Context.IsMainPlayer))
            return;

        e.Edit(static asset =>
        {
            // Load beacon
            string season = (Context.IsWorldReady) ? Game1.currentSeason.ToString().ToLower() : "spring";
            IRawTextureData seasonalBeacon = ModEntry.Instance.Helper.ModContent.Load<IRawTextureData>($"assets/{season}.png");

            var img = asset.AsImage();

            Rectangle sourceRect = new Rectangle(0, 0, seasonalBeacon.Width, seasonalBeacon.Height);
            Rectangle targetRect = SObject.getSourceRectForBigCraftable(JunimoBeacon.ID.Value);


            if (targetRect.IsEmpty || !img.Data.Bounds.Contains(targetRect))
                return;

            if (targetRect.Y + targetRect.Height > 4096)
            {
                SpaceCore.TileSheetExtensions.PatchExtendedTileSheet(img, seasonalBeacon, sourceArea: sourceRect, targetArea: targetRect);
            }
            else
            {
                img.PatchImage(seasonalBeacon, sourceArea: sourceRect, targetArea: targetRect);
            }

        });
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        Debug.DebugOnlyExecute(() =>
        {
            /// Re-assign random colors
            if (e.Button.Equals(SButton.R))
            {
                foreach (JunimoGroup group in this.JunimoGroups)
                {
                    group.Color = ColorHelper.GetUniqueColor(alpha: 230);
                }
            }
            else if(e.Button.Equals(SButton.T))
            {
                // Force asset request of big craftable
                Instance.Helper.GameContent.InvalidateCache("Tilesheets/Craftables");
            }
        });
    }

    private void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
        // Check if groups and beacons still in range
        // This should only be wrong if junimo hut is moved through building interface
        // Otherwise, it is here as a failsafe
        foreach (JunimoGroup group in this.JunimoGroups)
        {
            group.RemoveOutOfRangeBeacons();
        }
    }

    private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
    {
        foreach (Building farmBuilding in e.Added)
        {
            if (TypeChecker.isType<JunimoHut>(farmBuilding))
            {
                TryAddJunimoGroup(farmBuilding as JunimoHut);
            }
        }

        foreach (Building farmBuilding in e.Removed)
        {
            if (TypeChecker.isType<JunimoHut>(farmBuilding))
                RemoveGroupWithHut(farmBuilding as JunimoHut);
        }
    }

    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        placeBeaconOverlay.Enable();
        this.placeBeaconOverlay.DrawOverlay(e.SpriteBatch);
    }

    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        // Force asset request of big craftable
        Instance.Helper.GameContent.InvalidateCache("Tilesheets/Craftables");

        foreach (Building farmBuilding in Game1.getFarm().buildings)
        {
            if (TypeChecker.isType<JunimoHut>(farmBuilding))
            {
                TryAddJunimoGroup(farmBuilding as JunimoHut);
            }
        }
    }

    private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
        // Check if beacon was added, and try to add it to a group
        foreach (KeyValuePair<Vector2, SObject> kvp in e.Added)
        {
            if (kvp.Value.ParentSheetIndex == JunimoBeacon.ID)
            {
                JunimoBeacon beacon = new JunimoBeacon(kvp.Value);
                TryAddBeaconToGroups(beacon);
            }
        }

        // Remove beacon
        foreach (KeyValuePair<Vector2, SObject> kvp in e.Removed)
        {
            if (kvp.Value.ParentSheetIndex == JunimoBeacon.ID)
            {
                TryRemoveBeaconFromGroups(kvp.Value);
            }
        }
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.JsonAssetsAPI = this.Helper.ModRegistry.GetApi<JsonAssets.IApi>("spacechase0.JsonAssets");

        JsonAssetsAPI.LoadAssets(Path.Combine(Helper.DirectoryPath, this.ContentPackPath));

        CreateRecipeUnlockMail();

        this.Config.createMenu();
    }

    private void OnSaveLoad(object sender, EventArgs e)
    {
        // Get id here, as id is not available before save loads
        JunimoBeacon.ID = JsonAssetsAPI.GetBigCraftableId(JunimoBeacon.ItemName);
        this.JunimoGroups = new List<JunimoGroup>();
    }

    private void CreateRecipeUnlockMail()
    {
        MailDao.SaveLetter(
            new Letter(
                id: "Achtuur.JunimoBeacon.BeaconRecipeMail",
                text: "mail_beaconrecipe.text",
                recipe: JunimoBeacon.ItemName,
                (l) =>
                {
                    return !Game1.player.mailReceived.Contains(l.Id) && // Hasn't received mail
                    Game1.getFarm().buildings.Any(b => TypeChecker.isType<JunimoHut>(b)); // Has a junimo hut
                },
                (l) => Game1.player.mailReceived.Add(l.Id),
                whichBG: 2 // wizard background
            )
            {
                Title = "mail_beaconrecipe.title",
                I18N = Helper.Translation
            }
        );
    }
}
