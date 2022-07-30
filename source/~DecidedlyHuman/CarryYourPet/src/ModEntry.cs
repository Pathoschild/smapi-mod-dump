/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using CarryYourPet.Patches;
using DecidedlyShared.Logging;
using DecidedlyShared.APIs;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace CarryYourPet
{
	public class ModEntry : Mod
	{
		// SMAPI gubbins.
		private static IModHelper helper;
		private static IMonitor monitor;
		private static Logger logger;
		private static ModConfig config;
		
		// Patch stuff
		private NPCPatches patches;
		
		// NPC stuff.
        private CarriedCharacter carriedCharacter;

		public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
			ModEntry.helper = helper;
			monitor = Monitor;
			logger = new Logger(monitor, null);
            carriedCharacter = new CarriedCharacter();
			patches = new NPCPatches(config, carriedCharacter, logger);
            I18n.Init(helper.Translation);

            Harmony harmony = new Harmony(ModManifest.UniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
				postfix: new HarmonyMethod(typeof(NPCPatches), nameof(Patches.NPCPatches.PetCheckAction_Postfix)));
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Pet), nameof(Pet.draw),
                    parameters: new Type[] {typeof(SpriteBatch)}),
                prefix: new HarmonyMethod(typeof(NPCPatches), nameof(Patches.NPCPatches.PetDraw_Prefix)));
            
            // If I do ever re-enable carrying NPCs, these are the patches.
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(NPC), nameof(NPC.draw),
            //         parameters: new Type[] {typeof(SpriteBatch), typeof(float)}),
            //     prefix: new HarmonyMethod(typeof(NPCPatches), nameof(Patches.NPCPatches.NpcDraw_Prefix)));
            //
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
            //     prefix: new HarmonyMethod(typeof(NPCPatches), nameof(Patches.NPCPatches.NpcCheckAction_Postfix)));
			
			helper.Events.Display.RenderedWorld += DisplayOnRenderedWorld;
            helper.Events.Player.Warped += PlayerOnWarped;
            helper.Events.GameLoop.GameLaunched += (sender, args) => { RegisterWithGmcm(); };
        }

        private void RegisterWithGmcm()
        {
            IGenericModConfigMenuApi configMenuApi =
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi == null)
            {
                logger.Log(I18n.CarryYourPet_Message_GmcmNotInstalled(), LogLevel.Info);

                return;
            }

            configMenuApi.Register(ModManifest,
                () => config = new ModConfig(),
                () => Helper.WriteConfig(config));
            
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.CarryYourPet_Keybind_HoldToCarry(),
                tooltip: () => I18n.CarryYourPet_Keybind_HoldToCarry_Tooltip(),
                getValue: () => config.HoldToCarryNpc,
                setValue: bind => config.HoldToCarryNpc = bind);
        }

        private void PlayerOnWarped(object sender, WarpedEventArgs e)
        {
            // On warp, we want to "drop" the NPC, which is to say, set the carried property to null.
            carriedCharacter.Npc = null;
        }

        private void DisplayOnRenderedWorld(object? sender, RenderedWorldEventArgs e)
		{
            if (carriedCharacter.Npc != null)
            {
                Pet pet = (Pet)this.carriedCharacter.Npc;

                LockPetToPlayer(pet);

                this.carriedCharacter.ShouldDraw = true;
                pet.draw(e.SpriteBatch);
                this.carriedCharacter.ShouldDraw = false;
            }
        }

        private void LockPetToPlayer(Pet pet)
        {
            pet.position.Value = Game1.player.position.Value + new Vector2(-32f, -40);
            if (pet.CurrentBehavior != 55)
            {
                pet.stopWithoutChangingFrame();
                pet.CurrentBehavior = 55;
            }
        }
    }
}