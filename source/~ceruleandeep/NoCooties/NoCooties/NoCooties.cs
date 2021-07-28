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
using ContentPatcher;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Network;

namespace NoCooties
{
    /// <summary>The mod entry point.</summary>
    public class NoCooties : Mod
    {
        internal static ModConfig Config;
        public static IContentPatcherAPI cp_api;

        public static Multiplayer mp;
        public static IMonitor SMonitor;

        internal IModInfo PPaFModInfo;
        internal bool UsingPPaFConfig;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;
            mp = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            HarmonyInstance harmony = HarmonyInstance.Create("ceruleandeep.nocooties");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(NoCooties), nameof(NPC_checkAction_Prefix))
            );

            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        internal void LoadPPaFConfig()
        {
            PPaFModInfo = Helper.ModRegistry.Get("Amaranthacyan.PlatonicPartnersandFriendships");
            if (PPaFModInfo == null)
            {
                Monitor.Log($"PPaF not installed, cannot sync config");
                return;
            }

            PropertyInfo property = PPaFModInfo.GetType().GetProperty("DirectoryPath",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property == null)
            {
                Monitor.Log($"Can't access directory path for PPaF", LogLevel.Error);
                return;
            }

            string directoryPath = property.GetValue(PPaFModInfo).ToString();
            Monitor.Log($"Loading PPaF config from {directoryPath}", LogLevel.Debug);

            IContentPack contentPack = this.Helper.ContentPacks.CreateFake(directoryPath);
            PPaFModConfig pmc = contentPack.ReadJsonFile<PPaFModConfig>("config.json");
            if (pmc.PlatonicNPCs.Length > 0)
            {
                Monitor.Log($"Platonic NPCs: {pmc.PlatonicNPCs}", LogLevel.Debug);
                Config.HuggingNPCs = Config.cslToList(pmc.PlatonicNPCs);
                UsingPPaFConfig = true;
            }
        }

        internal void SetHugger(string npc, bool hugger)
        {
            if (hugger && !Config.HuggingNPCs.Contains(npc)) Config.HuggingNPCs.Add(npc);
            if (!hugger && Config.HuggingNPCs.Contains(npc)) Config.HuggingNPCs.Remove(npc);
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            LoadPPaFConfig();
            SetupGMCM();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu == null) return;
            if (!e.OldMenu.ToString().Contains("GenericModConfigMenu.Framework.SpecificModConfigMenu")) return;
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
                    foreach (string npc in Config.HuggingNPCs.Distinct())
                    {
                        gmcm_api.RegisterImage(ModManifest, $"Portraits\\{npc}", new Rectangle(64, 0, 64, 64), 1);
                    }
                }
                else
                {
                    foreach (string npc in Config.knownNPCs)
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
        void OnSaveLoaded(object sender, EventArgs e)
        {
            // reload the config to pick up any changes made in GMCM on the title screen
            Config = Helper.ReadConfig<ModConfig>();
            LoadPPaFConfig();
            SetupGMCM();
        }

        /// <summary>Raised after a the game is saved</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnSaving(object sender, SavingEventArgs e)
        {
            Helper.WriteConfig(Config);
        }

        [HarmonyBefore(new string[] {"aedenthorn.MultipleSpouses"})]
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
                        __instance.faceGeneralDirection(who.getStandingPosition(), 0, opposite: false,
                            useTileCalculations: false);
                        who.faceGeneralDirection(__instance.getStandingPosition(), 0, opposite: false,
                            useTileCalculations: false);
                        if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                        {
                            int spouseFrame = 28;
                            bool facingRight = true;
                            string name = __instance.Name;
                            if (name == "Sam")
                            {
                                spouseFrame = 36;
                            }
                            else if (name == "Penny")
                            {
                                spouseFrame = 35;
                            }
                            else if (name == "Sebastian")
                            {
                                spouseFrame = 40;
                            }
                            else if (name == "Alex")
                            {
                                spouseFrame = 42;
                            }
                            else if (name == "Krobus")
                            {
                                spouseFrame = 16;
                            }
                            else if (name == "Maru")
                            {
                                spouseFrame = 28;
                                facingRight = false;
                            }
                            else if (name == "Emily")
                            {
                                spouseFrame = 33;
                                facingRight = false;
                            }
                            else if (name == "Harvey")
                            {
                                spouseFrame = 31;
                                facingRight = false;
                            }
                            else if (name == "Shane")
                            {
                                spouseFrame = 34;
                                facingRight = false;
                            }
                            else if (name == "Elliott")
                            {
                                spouseFrame = 35;
                                facingRight = false;
                            }
                            else if (name == "Leah")
                            {
                                spouseFrame = 25;
                            }
                            else if (name == "Abigail")
                            {
                                spouseFrame = 33;
                                facingRight = false;
                            }

                            bool flip = (facingRight && __instance.FacingDirection == 3) ||
                                        (!facingRight && __instance.FacingDirection == 1);
                            if (who.getFriendshipHeartLevelForNPC(__instance.Name) > 9 && __instance.sleptInBed.Value)
                            {
                                int delay = Game1.IsMultiplayer ? 1000 : 10;
                                __instance.movementPause = delay;
                                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                                {
                                    new FarmerSprite.AnimationFrame(spouseFrame, delay, false, flip,
                                        new AnimatedSprite.endOfAnimationBehavior(__instance.haltMe), true)
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
                                            new TemporaryAnimatedSprite("LooseSprites\\emojis",
                                                new Microsoft.Xna.Framework.Rectangle(0, 0, 9, 9), 2000f, 1, 0,
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
                                            new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                                                new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0,
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

                            int playerFaceDirection = 1;
                            if ((facingRight && !flip) || (!facingRight && flip))
                            {
                                playerFaceDirection = 3;
                            }

                            who.PerformKiss(playerFaceDirection);
                            who.CanMove = false;
                            who.FarmerSprite.PauseForSingleAnimation = false;
                            who.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(101, 1000, 0, false, who.FacingDirection == 3, null,
                                    false, 0),
                                new FarmerSprite.AnimationFrame(6, 1, false, who.FacingDirection == 3,
                                    new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating), false)
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