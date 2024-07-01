/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Particle;
using AchtuurCore.Framework.Particle.StartBehaviour;
using AchtuurCore.Framework.Particle.UpdateBehaviour;
using AchtuurCore.Patches;
using AchtuurCore.Utility;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MultiplayerExpShare.Integrations;
using MultiplayerExpShare.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.Minigames.TargetGame;

namespace MultiplayerExpShare;

internal class ModEntry : Mod
{
    /// <summary>
    /// Color used for Farming exp particle, green
    /// </summary>
    internal readonly Color FarmingColor = new Color(5, 230, 15);
    /// <summary>
    /// Color used for Foraging exp particle, brown
    /// </summary>
    internal readonly Color ForagingColor = new Color(166, 74, 5);
    /// <summary>
    /// Color used for Fishing exp particle, blue
    /// </summary>
    internal readonly Color FishingColor = new Color(15, 80, 255);
    /// <summary>
    /// Color used for Mining exp particle, red
    /// </summary>
    internal readonly Color MiningColor = new Color(230, 10, 15);
    /// <summary>
    /// Color used for Combat exp particle, cyan
    /// </summary>
    internal readonly Color CombatColor = new Color(0f, 220, 180);

    internal readonly Vector2 ParticleSize = new Vector2(6f, 6f);
    internal readonly int ParticleTrailLength = 10;

    internal readonly int LargeParticleThreshold = 15;
    internal readonly int MediumParticleThreshold = 5;

    internal readonly float LargeParticleScale = 2f;
    internal readonly float MediumParticleScale = 1.5f;


    internal static ModEntry Instance;
    public ModConfig Config;
    public ISpaceCoreApi SpaceCoreAPI;

    internal TileShareRangeOverlay TileUIOverlay;

    internal PerScreen<Dictionary<string, int>> skillMaxLevels;

    internal static Dictionary<string, TrailParticle> ShareTrailParticles;


    public static bool PlayerHasMastery()
    {
        return FarmerHasMastery(Game1.player);
    }

    public static bool FarmerHasMastery(Farmer farmer)
    {
        return farmer.Level >= 25 && MasteryTrackerMenu.getCurrentMasteryLevel() < 5; // this is used in base game code?
    }

    public static Farmer GetFarmerFromMultiplayerID(long id)
    {
        return Game1.getOnlineFarmers().ToList().Find(farmer => farmer.UniqueMultiplayerID == id);
    }

    public static void ShareExpWithFarmers(Farmer[] nearbyFarmers, string skill_id, int amount, string messageName)
    {
        // Send message about exp sharing
        long[] nearbyFarmerIds = nearbyFarmers.Select(f => f.UniqueMultiplayerID).ToArray();
        ExpGainData expdata = new ExpGainData(Game1.player.UniqueMultiplayerID, nearbyFarmerIds, skill_id, amount);
        ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainData>(expdata, messageName, modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });

        // Spawn particles to farmers you share exp with
        foreach (Farmer target in nearbyFarmers)
        {
            SpawnParticles(Game1.player, target, skill_id, amount);
        }
    }

    /// <summary>
    /// Spawns multiple particles, based on <paramref name="amount"/>
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="skill_id"></param>
    /// <param name="amount"></param>
    public static void SpawnParticles(Farmer origin, Farmer target, string skill_id, int amount)
    {
        // If skill has no valid particle or origin/target not in same location, do not spawn
        if (!ShareTrailParticles.ContainsKey(skill_id))
            return;

        int n_particles = Math.Max(1, amount / Instance.Config.ExpPerParticle);
        bool sameLocation = origin.currentLocation == target.currentLocation;

        while(n_particles > 0)
        {
            float size_multiplier;
            if (n_particles >= Instance.LargeParticleThreshold)
            {
                size_multiplier = Instance.LargeParticleScale;
                n_particles -= Instance.LargeParticleThreshold;
            }
            else if (n_particles >= Instance.MediumParticleThreshold)
            {
                size_multiplier = Instance.MediumParticleScale;
                n_particles -= Instance.MediumParticleThreshold;
            }
            else
            {
                size_multiplier = 1f;
                n_particles -= 1;
            }

            if (sameLocation)
                SpawnParticleSameLocation(origin, target, skill_id, size_multiplier);
            else
                SpawnParticleDifferentLocation(origin, target, skill_id, size_multiplier);
        }
    }

    private static void SpawnParticleDifferentLocation(Farmer origin, Farmer target, string skill_id, float size_multiplier)
    {
        TrailParticle particle = new TrailParticle(ShareTrailParticles[skill_id]);
        particle.SetSize(Instance.ParticleSize * size_multiplier);

        if (origin != Game1.player) // receiving end
        {
            particle.AddState<EdgeOfMapStartBehaviour>();
            particle.AddState<MovementBehaviour>();
            particle.AddState<OrbitMovementBehaviour>();
            particle.SetTargetFarmer(target);
        } 
        else // transmitting end
        {
            particle.SetInitialPosition(Game1.player.Position);
            particle.AddState<RandomStartBehaviour>();
            particle.AddState<EscapeMapMovementBehaviour>();
        }

        particle.Start();
    }

    private static void SpawnParticleSameLocation(Farmer origin, Farmer target, string skill_id, float size_multiplier)
    {
        // Clone particle from dictionary, so that original remains for further copying
        TrailParticle particle = new TrailParticle(ShareTrailParticles[skill_id]);
        particle.SetSize(Instance.ParticleSize * size_multiplier);
        particle.AddState<RandomStartBehaviour>();
        particle.AddState<MovementBehaviour>();
        particle.AddState<OrbitMovementBehaviour>();

        // Set initial position of 
        particle.SetInitialPosition(origin.Position);
        particle.SetTargetFarmer(target);
        particle.Start();
    }

    /// <summary>
    /// Returns whether <paramref name="other_farmer"/> is nearby <c>Game1.player</c>, based on <see cref="ModConfig.ExpShareType"/>
    /// </summary>
    /// <param name="other_farmer"></param>
    /// <returns></returns>
    public static bool FarmerIsNearby(Farmer other_farmer)
    {
        switch (Instance.Config.ExpShareType)
        {
            case ExpShareType.Tile:
                return other_farmer.currentLocation == Game1.player.currentLocation && IsInRange(other_farmer);
            case ExpShareType.Map:
                return other_farmer.currentLocation == Game1.player.currentLocation;
            case ExpShareType.Global:
                return true;
            default: return false;
        }
    }

    /// <summary>
    /// Calculates euclidian distance between current tile of <c> name="Game1.player" </c> and <paramref name="other_farmer"/> and returns true if that value is less than or equal to <see cref="ModConfig.NearbyPlayerTileRange"/>
    /// </summary>
    /// <param name="other_farmer"></param>
    /// <returns></returns>
    public static bool IsInRange(Farmer other_farmer)
    {

        int dx = (int) (Game1.player.Tile.X - other_farmer.Tile.X);
        int dy = (int) (Game1.player.Tile.Y - other_farmer.Tile.Y);

        return dx * dx + dy * dy <= Instance.Config.NearbyPlayerTileRange * Instance.Config.NearbyPlayerTileRange;
    }

    public static IEnumerable<Farmer> GetNearbyPlayers()
    {
        // return all players that are close to the main player
        foreach (Farmer online_farmer in Game1.getOnlineFarmers())
        {
            // Add other player to list if they are close enough to main player
            if (FarmerIsNearby(online_farmer))
            {
                yield return online_farmer;
            }
        }
    }

    /// <summary>
    /// Get exp percentage for actor based on settings and skill level (if setting enabled)
    /// </summary>
    /// <param name="level">Optional, skill level of current skill being evaluated for exp</param>
    /// <returns></returns>
    public static float GetActorExpPercentage(Farmer actor, int level, string skill_id)
    {
        if (Instance.Config.ShareAllExpAtMaxLevel && level >= Instance.skillMaxLevels.Value[skill_id] && !FarmerHasMastery(actor))
        {
            return 0f;
        }

        return Instance.Config.ExpPercentageToActor;
    }

    public static float GetSharedExpPercentage(Farmer actor, int actor_level, string skill_id)
    {
        return 1f - GetActorExpPercentage(actor, actor_level, skill_id);
    }

    /// <summary>
    /// Get max level for vanilla skills
    /// </summary>
    /// <param name="skill_id"></param>
    /// <returns></returns>
    public static int GetMaxLevelVanilla(int skill_id)
    {
        if (!Instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
            return 10;

        try
        {
            // Get instance of MARGO skill for this skill id
            Type MargoSkill = AccessTools.TypeByName("DaLion.Overhaul.Modules.Professions.Skill");
            string SkillName = AchtuurCore.Utility.Skills.GetSkillNameFromId(skill_id);
            // GetValue with null since the field is static
            var SkillInstance = AccessTools.Field(MargoSkill, SkillName).GetValue(null);

            // Return max level property
            return (int)AccessTools.Property(MargoSkill, "MaxLevel").GetValue(SkillInstance);
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.ErrorLog(Instance.Monitor, $"Something went wrong when looking up max level of MARGO skill:\n{e}");
            return 10;
        }
    }

    /// <summary>
    /// Get max level of skill for spacecore skills
    /// </summary>
    /// <param name="skill_id"></param>
    /// <returns></returns>
    public static int GetMaxLevelSpaceCore(string skill_id)
    {
        return 10;

        // TODO: fix this maybe
        if (!Instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
            return 10;


        try
        {
            Type MargoSCSkill = AccessTools.TypeByName("DaLion.Overhaul.Modules.Professions.SCSkill");
            Type SpaceCoreSkill = AccessTools.TypeByName("SpaceCore.Skills");

            // BiMap<SCSkill, SpaceCore.Skills.Skill> (WHAT THE FUCK IS A BIMAP)
            var SpaceCoreMap = (Dictionary<object, object>)AccessTools.Field(MargoSCSkill, "SpaceCoreMap").GetValue(null);

            foreach (KeyValuePair<object, object> keyValuePair in SpaceCoreMap)
            {
                // Check if this space core skill matches input
                string spacecore_skillid = (string)AccessTools.Property(SpaceCoreSkill, "Id").GetValue(keyValuePair.Value);
                if (spacecore_skillid == skill_id)
                {
                    // Return max level
                    return (int)AccessTools.Property(MargoSCSkill, "MaxLevel").GetValue(keyValuePair.Key);
                }
            }
            return 10;
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.WarningLog(Instance.Monitor, $"Something went wrong when looking up max level of MARGO skill:\n{e}");
            return 10;
        }

    }

    private void UpdateMaxLevels()
    {
        if (!Context.IsWorldReady)
            return;

        if (this.skillMaxLevels.Value is null)
            this.skillMaxLevels.Value = new Dictionary<string, int>();

        // Update vanilla skill max levels
        for (int i = 0; i <= 5; i++)
        {
            int maxLevel = GetMaxLevelVanilla(i);
            string skillName = AchtuurCore.Utility.Skills.GetSkillNameFromId(i);
            this.skillMaxLevels.Value[skillName] = maxLevel;
        }

        // Update spacecore skill max levels
        if (this.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
        {
            foreach (string skill_id in SpaceCoreAPI.GetCustomSkills())
            {
                int maxLevel = GetMaxLevelSpaceCore(skill_id);
                this.skillMaxLevels.Value[skill_id] = maxLevel;
            }
        }
    }

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new GainExperiencePatch(),
            new SpaceCoreExperiencePatch()
        );

        this.Config = helper.ReadConfig<ModConfig>();

        this.TileUIOverlay = new TileShareRangeOverlay();
        this.skillMaxLevels = new PerScreen<Dictionary<string, int>>();

        IntializeVanillaTrailParticles();

        helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoad;
        helper.Events.GameLoop.SaveCreated += OnSaveCreate;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Display.RenderedWorld += OnRenderedWorld;
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    private void IntializeVanillaTrailParticles()
    {
        // Create particles that can be copied
        ShareTrailParticles = new Dictionary<string, TrailParticle>
        {
            { "Farming", new TrailParticle(ParticleTrailLength, FarmingColor, ParticleSize) },
            { "Foraging", new TrailParticle(ParticleTrailLength, ForagingColor, ParticleSize) },
            { "Fishing", new TrailParticle(ParticleTrailLength, FishingColor, ParticleSize) },
            { "Mining", new TrailParticle(ParticleTrailLength, MiningColor, ParticleSize) },
            { "Combat", new TrailParticle(ParticleTrailLength, CombatColor, ParticleSize) }
        };

        // Add trail color and use default size (by not calling method)
        ShareTrailParticles["Farming"].SetTrailColors(new List<Color> { FarmingColor, Color.WhiteSmoke, Color.WhiteSmoke });
        ShareTrailParticles["Foraging"].SetTrailColors(new List<Color> { ForagingColor, Color.WhiteSmoke, Color.WhiteSmoke });
        ShareTrailParticles["Fishing"].SetTrailColors(new List<Color> { FishingColor, Color.WhiteSmoke, Color.WhiteSmoke });
        ShareTrailParticles["Mining"].SetTrailColors(new List<Color> { MiningColor, Color.WhiteSmoke, Color.WhiteSmoke });
        ShareTrailParticles["Combat"].SetTrailColors(new List<Color> { CombatColor, Color.WhiteSmoke, Color.WhiteSmoke });
    }

    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        this.TileUIOverlay.DrawOverlay(e.SpriteBatch);
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        // Check if overlay button is pressed
        if (e.Button == this.Config.OverlayButton && Context.IsPlayerFree)
        {
            this.TileUIOverlay.Toggle();
        }

        Debug.DebugOnlyExecute(() =>
        {
            if (e.Button == SButton.J)
            {
                // simulate diamond node breaking
                int n_particles = 150 / Instance.Config.ExpPerParticle;
                //int n_particles = 1;

                while (n_particles > 0)
                {
                    float size_multiplier;
                    if (n_particles >= Instance.LargeParticleThreshold)
                    {
                        size_multiplier = Instance.LargeParticleScale;
                        n_particles -= Instance.LargeParticleThreshold;
                    }
                    else if (n_particles >= Instance.MediumParticleThreshold)
                    {
                        size_multiplier = Instance.MediumParticleScale;
                        n_particles -= Instance.MediumParticleThreshold;
                    }
                    else
                    {
                        size_multiplier = 1f;
                        n_particles -= 1;
                    }
                    // Clone particle from dictionary, so that original remains for further copying
                    TrailParticle particle = new TrailParticle(ShareTrailParticles["Farming"]);
                    particle.SetSize(Instance.ParticleSize * size_multiplier);
                    //Color color = new Color(15, 80, 255);
                    //particle.SetColor(color);
                    //particle.SetTrailColors(new List<Color> { color, Color.WhiteSmoke, Color.WhiteSmoke });
                    //particle.AddState<MovementBehaviour>();
                    // Set initial position of 
                    //particle.SetInitialPosition(e.Cursor.AbsolutePixels);
                    //particle.SetTargetFarmer(Game1.player);

                    particle.AddState<EdgeOfMapStartBehaviour>();
                    particle.AddState<MovementBehaviour>();
                    particle.AddState<OrbitMovementBehaviour>();
                    particle.SetTargetFarmer(Game1.player);

                    particle.Start();
                }
            }
        });
    }

    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        UpdateMaxLevels();
    }

    private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID == this.ModManifest.UniqueID && (e.Type == "SharedExpGained" || e.Type == "SharedExpGainedSpaceCore"))
        {
            ExpGainData msg_expdata = e.ReadAs<ExpGainData>();
            // if the source is self or self was not nearby, don't add exp
            if (msg_expdata.actor_multiplayerid == Game1.player.UniqueMultiplayerID || !msg_expdata.nearby_farmer_ids.Contains(Game1.player.UniqueMultiplayerID))
                return;

            Farmer nearby_actor = GetFarmerFromMultiplayerID(msg_expdata.actor_multiplayerid);
            SpawnParticles(nearby_actor, Game1.player, msg_expdata.skill_id, msg_expdata.amount);

            AchtuurCore.Logger.DebugLog(Instance.Monitor, $"Received {msg_expdata.amount} exp in {msg_expdata.skill_id} from ({nearby_actor.Name})!");

            if (e.Type == "SharedExpGained")
                GainExperiencePatch.InvokeGainExperience(Game1.player, msg_expdata);
            else if (e.Type == "SharedExpGainedSpaceCore")
                SpaceCoreExperiencePatch.InvokeGainExperience(Game1.player, msg_expdata);
        }
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.SpaceCoreAPI = Instance.Helper.ModRegistry.GetApi<Integrations.ISpaceCoreApi>("spacechase0.SpaceCore");

        this.Config.createMenu();
    }
    private void OnSaveLoad(object sender, SaveLoadedEventArgs e)
    {
        // Create config menu again, in case some Spacecore based mods are loaded after this mod
        this.Config.createMenu();
        UpdateMaxLevels();
    }

    private void OnSaveCreate(object sender, SaveCreatedEventArgs e)
    {
        // Create config menu again, in case some Spacecore based mods are loaded after this mod
        this.Config.createMenu();
        UpdateMaxLevels();
    }
}
