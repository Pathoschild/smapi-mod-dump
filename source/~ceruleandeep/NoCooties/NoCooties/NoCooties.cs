/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Network;

namespace NoCooties
{
    /// <summary>The mod entry point.</summary>
    public class NoCooties : Mod
    {
        private static ModConfig Config;

        private static Multiplayer mp;
        private static IMonitor SMonitor;

        private IModInfo PPaFModInfo;
        private bool UsingPPaFConfig;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;
            mp = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            var harmony = new Harmony("ceruleandeep.nocooties");
            harmony.Patch(
                AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                new HarmonyMethod(typeof(NoCooties), nameof(NPC_checkAction_Prefix))
            );

            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void LoadPPaFConfig()
        {
            PPaFModInfo = Helper.ModRegistry.Get("Amaranthacyan.PlatonicPartnersandFriendships");
            if (PPaFModInfo == null)
            {
                Monitor.Log($"PPaF not installed, cannot sync config");
                return;
            }

            var property = PPaFModInfo.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property == null)
            {
                Monitor.Log($"Can't access directory path property for PPaF", LogLevel.Error);
                return;
            }

            var directoryPath = property.GetValue(PPaFModInfo)?.ToString();
            if (directoryPath == null)
            {
                Monitor.Log($"Can't access directory path for PPaF", LogLevel.Error);
                return;
            }
            Monitor.Log($"Loading PPaF config from {directoryPath}", LogLevel.Debug);

            var contentPack = this.Helper.ContentPacks.CreateFake(directoryPath);
            var pmc = contentPack.ReadJsonFile<PPaFModConfig>("config.json");
            if (pmc?.PlatonicNPCs is null || pmc.PlatonicNPCs.Length <= 0) return;
            Monitor.Log($"Platonic NPCs: {pmc.PlatonicNPCs}", LogLevel.Debug);
            Config.HuggingNPCs = Config.cslToList(pmc.PlatonicNPCs);
            UsingPPaFConfig = true;
        }

        private static void SetHugger(string npc, bool hugger)
        {
            switch (hugger)
            {
                case true when !Config.HuggingNPCs.Contains(npc):
                    Config.HuggingNPCs.Add(npc);
                    break;
                case false when Config.HuggingNPCs.Contains(npc):
                    Config.HuggingNPCs.Remove(npc);
                    break;
            }
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            LoadPPaFConfig();
            SetupGMCM();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            var oldMenuName = e.OldMenu?.ToString();
            if (oldMenuName is null) return;
            if (!oldMenuName.Contains("GenericModConfigMenu.Framework.SpecificModConfigMenu")) return;
            LoadPPaFConfig();
            SetupGMCM();
        }
        
        private void SetupGMCM()
        {
            var gmcm_api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm_api != null)
            {
                gmcm_api.UnregisterModConfig(ModManifest);
                gmcm_api.RegisterModConfig(ModManifest, () => Config = new ModConfig(),
                    () => Helper.WriteConfig(Config));
                gmcm_api.SetDefaultIngameOptinValue(ModManifest, true);

                gmcm_api.RegisterSimpleOption(ModManifest,
                    "Endless hugs",
                    "Remove one-per-day limit on hugs (and kisses). Still only one relationship boost per day though!",
                    () => Config.EndlessHugs,
                    (val) => Config.EndlessHugs = val);
                
                gmcm_api.RegisterSimpleOption(ModManifest,
                    "Custom NPCs are huggers",
                    "Should NPCs who aren't in the list below hug?",
                    () => Config.CustomNPCsAreHuggers,
                    (val) => Config.CustomNPCsAreHuggers = val);
                
                if (UsingPPaFConfig)
                {
                    gmcm_api.RegisterLabel(ModManifest, "Huggers (set in PPaF)",
                        "No cooties from these NPCs!");
                    foreach (var npc in Config.HuggingNPCs.Distinct())
                    {
                        gmcm_api.RegisterImage(ModManifest, $"Portraits\\{npc}", new Rectangle(64, 0, 64, 64), 1);
                    }
                }
                else
                {
                    foreach (var npc in Config.knownNPCs)
                    {
                        gmcm_api.RegisterSimpleOption(ModManifest, npc, $"{npc} gets hugs instead of kisses",
                            () => Config.HuggingNPCs.Contains(npc),
                            (val) => SetHugger(npc, val));
                    }
                }
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            // reload the config to pick up any changes made in GMCM on the title screen
            Config = Helper.ReadConfig<ModConfig>();
            LoadPPaFConfig();
            SetupGMCM();
        }

        /// <summary>Raised after a the game is saved</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            Helper.WriteConfig(Config);
        }

        [HarmonyBefore("aedenthorn.FreeLove")]
        [HarmonyPriority(Priority.High)]
        public static bool NPC_checkAction_Prefix(ref NPC __instance, ref Farmer who, ref bool __result)
        {
            try
            {
                if (who.currentLocation is Farm || who.currentLocation is FarmHouse)
                {
                    if (__instance.Sprite.CurrentAnimation == null)
                    {
                        __instance.faceDirection(-3);
                    }

                    if (__instance.Sprite.CurrentAnimation == null && !__instance.hasTemporaryMessageAvailable() &&
                        __instance.currentMarriageDialogue.Count == 0 && __instance.CurrentDialogue.Count == 0 &&
                        Game1.timeOfDay < 2200 && !__instance.isMoving() && who.ActiveObject == null)
                    {
                        __instance.faceGeneralDirection(who.getStandingPosition(), 0, false, false);
                        who.faceGeneralDirection(__instance.getStandingPosition(), 0, false, false);
                        if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                        {
                            int spouseFrame = 28;
                            bool facingRight = true;
                            string name = __instance.Name;
                            switch (name)
                            {
                                case "Sam":
                                    spouseFrame = 36;
                                    break;
                                case "Penny":
                                    spouseFrame = 35;
                                    break;
                                case "Sebastian":
                                    spouseFrame = 40;
                                    break;
                                case "Alex":
                                    spouseFrame = 42;
                                    break;
                                case "Krobus":
                                    spouseFrame = 16;
                                    break;
                                case "Maru":
                                    spouseFrame = 28;
                                    facingRight = false;
                                    break;
                                case "Emily":
                                    spouseFrame = 33;
                                    facingRight = false;
                                    break;
                                case "Harvey":
                                    spouseFrame = 31;
                                    facingRight = false;
                                    break;
                                case "Shane":
                                    spouseFrame = 34;
                                    facingRight = false;
                                    break;
                                case "Elliott":
                                    spouseFrame = 35;
                                    facingRight = false;
                                    break;
                                case "Leah":
                                    spouseFrame = 25;
                                    break;
                                case "Abigail":
                                    spouseFrame = 33;
                                    facingRight = false;
                                    break;
                            }

                            bool flip = facingRight && __instance.FacingDirection == 3 ||
                                        !facingRight && __instance.FacingDirection == 1;
                            if (who.getFriendshipHeartLevelForNPC(__instance.Name) > 9 && __instance.sleptInBed.Value)
                            {
                                var delay = Game1.IsMultiplayer ? 1000 : 10;
                                __instance.movementPause = delay;
                                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                                {
                                    new(spouseFrame, delay, false, flip,
                                        __instance.haltMe, true)
                                });
                                if (Config.EndlessHugs || !__instance.hasBeenKissedToday.Value)
                                {
                                    if (!__instance.hasBeenKissedToday.Value)
                                    {
                                        who.changeFriendship(10, __instance);
                                    }

                                    if (Config.IsHugger(__instance.displayName))
                                    {
                                        mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[]
                                        {
                                            new("LooseSprites\\emojis",
                                                new Rectangle(0, 0, 9, 9), 2000f, 1, 0,
                                                new Vector2((float) __instance.getTileX(),
                                                    (float) __instance.getTileY()) * 64f + new Vector2(16f, -64f),
                                                false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                                            {
                                                motion = new Vector2(0f, -0.5f),
                                                alphaFade = 0.01f
                                            }
                                        });
                                    }
                                    else
                                    {
                                        mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[]
                                        {
                                            new("LooseSprites\\Cursors",
                                                new Rectangle(211, 428, 7, 6), 2000f, 1, 0,
                                                new Vector2((float) __instance.getTileX(),
                                                    (float) __instance.getTileY()) * 64f + new Vector2(16f, -64f),
                                                false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f, false)
                                            {
                                                motion = new Vector2(0f, -0.5f),
                                                alphaFade = 0.01f
                                            }
                                        });
                                    }

                                    __instance.currentLocation.playSound("dwop", NetAudio.SoundContext.NPC);
                                    who.exhausted.Value = false;
                                    __instance.hasBeenKissedToday.Value = true;
                                    __instance.Sprite.UpdateSourceRect();
                                }

                                __instance.hasBeenKissedToday.Value = true;
                                __instance.Sprite.UpdateSourceRect();
                            }
                            else
                            {
                                __instance.faceDirection((Game1.random.NextDouble() < 0.5) ? 2 : 0);
                                __instance.doEmote(12, true);
                            }

                            var playerFaceDirection = 1;
                            if (facingRight && !flip || !facingRight && flip)
                            {
                                playerFaceDirection = 3;
                            }

                            who.PerformKiss(playerFaceDirection);
                            who.CanMove = false;
                            who.FarmerSprite.PauseForSingleAnimation = false;
                            who.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>
                            {
                                new(101, 1000, 0, false, who.FacingDirection == 3, null,
                                    false, 0),
                                new(6, 1, false, who.FacingDirection == 3,
                                    Farmer.completelyStopAnimating, false)
                            }.ToArray(), null);
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(NPC_checkAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }

            return true;
        }
    }
}