/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using CarryYourPet.Patches;
using DecidedlyShared.APIs;
using DecidedlyShared.Logging;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using SObject = StardewValley.Object;

namespace CarryYourPet
{
    public class ModEntry : Mod
    {
        // SMAPI gubbins.
        private static IModHelper helper;
        private static IMonitor monitor;
        private static Logger logger;
        private static ModConfig config;

        // NPC stuff.
        private CarriedCharacter carriedCharacter;

        private readonly SObject dummyObject = new();

        // Patch stuff
        private Patches.Patches patches;

        private string thing = "";

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            ModEntry.helper = helper;
            monitor = this.Monitor;
            logger = new Logger(monitor, null);
            this.carriedCharacter = new CarriedCharacter();
            this.patches = new Patches.Patches(config, this.carriedCharacter, logger);
            I18n.Init(helper.Translation);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
                postfix: new HarmonyMethod(typeof(Patches.Patches), nameof(Patches.Patches.PetCheckAction_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Pet), nameof(Pet.draw),
                    new[] { typeof(SpriteBatch) }),
                new HarmonyMethod(typeof(Patches.Patches), nameof(Patches.Patches.PetDraw_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                new HarmonyMethod(typeof(Patches.Patches), nameof(Patches.Patches.FarmAnimalPet_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.draw),
                    new[] { typeof(SpriteBatch) }),
                new HarmonyMethod(typeof(Patches.Patches), nameof(Patches.Patches.FarmAnimalDraw_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(Farmer), nameof(Farmer.IsCarrying)),
                postfix: new HarmonyMethod(typeof(Patches.Patches), nameof(Patches.Patches.FarmerIsCarrying_Postfix)));

            // If I do ever re-enable carrying NPCs, these are the patches.
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(NPC), nameof(NPC.draw),
            //         parameters: new Type[] {typeof(SpriteBatch), typeof(float)}),
            //     prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Patches.NpcDraw_Prefix)));
            //
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
            //     prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Patches.NpcCheckAction_Postfix)));

            helper.Events.Display.RenderedWorld += this.DisplayOnRenderedWorld;
            helper.Events.Player.Warped += this.PlayerOnWarped;
            helper.Events.GameLoop.GameLaunched += (sender, args) => { this.RegisterWithGmcm(); };
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi == null)
            {
                logger.Log(I18n.CarryYourPet_Message_GmcmNotInstalled());

                return;
            }

            configMenuApi.Register(this.ModManifest,
                () => config = new ModConfig(),
                () => this.Helper.WriteConfig(config));

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.CarryYourPet_Keybind_HoldToCarry(),
                tooltip: () => I18n.CarryYourPet_Keybind_HoldToCarry_Tooltip(),
                getValue: () => config.HoldToCarryNpc,
                setValue: bind => config.HoldToCarryNpc = bind);
        }

        private void PlayerOnWarped(object sender, WarpedEventArgs e)
        {
            // On warp, we want to "drop" the NPC, which is to say, set the carried property to null.
            this.carriedCharacter.Npc = null;
        }

        private void DisplayOnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (this.carriedCharacter.Npc != null)
            {
                if (this.carriedCharacter.Npc is Pet)
                {
                    var pet = (Pet)this.carriedCharacter.Npc;

                    this.LockCharacterToPlayer(pet);

                    this.carriedCharacter.ShouldDraw = true;
                    pet.draw(e.SpriteBatch);
                    this.carriedCharacter.ShouldDraw = false;
                }
                else if (this.carriedCharacter.Npc is FarmAnimal)
                {
                    var animal = (FarmAnimal)this.carriedCharacter.Npc;
                    this.carriedCharacter.Npc.collidesWithOtherCharacters.Value = false;
                    this.LockCharacterToPlayer(animal);

                    this.carriedCharacter.ShouldDraw = true;
                    animal.draw(e.SpriteBatch);
                    this.carriedCharacter.ShouldDraw = false;
                }
            }
        }

        private void LockCharacterToPlayer(Character character)
        {
            if (character is Pet)
                character.position.Value = Game1.player.position.Value + new Vector2(-32f, -112f);
            else if (character is FarmAnimal)
                character.position.Value = Game1.player.position.Value + new Vector2(-32f, -168f);
        }
    }
}
