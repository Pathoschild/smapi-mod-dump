using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace FightingWithNPCs
{

    public class ModEntry : Mod, IAssetLoader
    {


        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites\\FightingWithNPCs"))
            {
                return true;
            }

            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites\\FightingWithNPCs"))
            {
                return this.Helper.Content.Load<T>("assets/images.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        public GameLocation location;


        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // Ignore if player has not loaded a save yet

            if (!Context.IsWorldReady)
                return;

            // i18n

            var i18n = this.Helper.Translation;

            // If using the Scythe...

            if ((e.Button.ToString() == "MouseLeft" || e.Button.ToString() == "C") && (Game1.player.CurrentTool != null) && (Game1.player.CurrentTool.Name == "Scythe" ||  Game1.player.CurrentTool.Name == "Golden Scythe"))
            {


                Vector2 player_position = new Vector2(Convert.ToInt32(Game1.player.position.X), Convert.ToInt32(Game1.player.position.Y));

                var npcs_list = new List<NPC>
                {
                    Game1.getCharacterFromName("Lewis"),
                    Game1.getCharacterFromName("Pierre"),
                    Game1.getCharacterFromName("Pam")
                };

                var beds_list = new List<Vector2>
                {
                    new Vector2(928, 280),
                    new Vector2(1308, 288),
                    new Vector2(1184, 792)
                };

                var sprites_list = new List<Vector4>
                {
                    new Vector4(0, 0, 16, 31),
                    new Vector4(16, 0, 16, 31),
                    new Vector4(32, 0, 16, 31)
                };

                var loot_list = new List<int>
                {
                    789, /* purple shorts */
                    170, /* broken glasses */
                    346  /* beer */
                };

                var provocation_text = new List<string>
                {
                    i18n.Get("provocation_text_1"),
                    i18n.Get("provocation_text_2"),
                    i18n.Get("provocation_text_3"),
                    i18n.Get("provocation_text_4"),
                    i18n.Get("provocation_text_5"),
                    i18n.Get("provocation_text_6"),
                    i18n.Get("provocation_text_7"),
                    i18n.Get("provocation_text_8"),
                    i18n.Get("provocation_text_9"),
                    i18n.Get("provocation_text_10"),
                    i18n.Get("provocation_text_11")
                };

                var repentance_text = new List<string>
                {
                    i18n.Get("repentance_text_1"),
                    i18n.Get("repentance_text_2"),
                    i18n.Get("repentance_text_3"),
                    i18n.Get("repentance_text_4"),
                    i18n.Get("repentance_text_5"),
                    i18n.Get("repentance_text_6"),
                    i18n.Get("repentance_text_7"),
                    i18n.Get("repentance_text_8"),
                    i18n.Get("repentance_text_9"),
                    i18n.Get("repentance_text_10"),
                    i18n.Get("repentance_text_11")
                };

                int foreach_offset = 0;

                Random random_number = new Random();


                foreach (NPC npc in npcs_list)
                {


                    Vector2 npc_position = new Vector2(Convert.ToInt32(npc.position.X), Convert.ToInt32(npc.position.Y));


                    // Speech bubble


                    if (Game1.currentLocation.name == npc.currentLocation.name)
                    {
                        if (Game1.currentLocation.name != "Hospital")
                        {
                            npc.showTextAboveHead(provocation_text[random_number.Next(0, provocation_text.Count)]);
                        }
                        else
                        {
                            npc.showTextAboveHead(repentance_text[random_number.Next(0, repentance_text.Count)]);
                        }
                    }


                    // Calculate the distance

                    if (
                        (Game1.currentLocation.name != "Hospital") &&
                        (Game1.currentLocation.name == npc.currentLocation.name) &&
                        ((player_position.X >= (npc_position.X - 100)) && (player_position.X <= (npc_position.X + 100))) &&
                        ((player_position.Y <= (npc_position.Y + 100)) && (player_position.Y >= (npc_position.Y - 100)))
                    )
                    {


                        // Warp NPC

                        npc.controller = null;
                        npc.temporaryController = null;
                        npc.ignoreScheduleToday = true;
                        npc.clearSchedule();
                        npc.followSchedule = false;
                        npc.Halt();

                        Game1.warpCharacter(npc, Game1.getLocationFromName("Hospital"), new Vector2(1, 1));
                        npc.position.X = beds_list[foreach_offset].X;
                        npc.position.Y = beds_list[foreach_offset].Y;

                        npc.playSleepingAnimation();


                        // Force use the Scythe and play a sound

                        Game1.player.CurrentTool.beginUsing(Game1.currentLocation, 0, 0, Game1.player);
                        Game1.playSound("Duck");


                        // Animate the "flying" NPC

                        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\FightingWithNPCs", new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(sprites_list[foreach_offset].X), Convert.ToInt32(sprites_list[foreach_offset].Y), Convert.ToInt32(sprites_list[foreach_offset].Z), Convert.ToInt32(sprites_list[foreach_offset].W)), 9999f, 1, 99999, new Vector2(Convert.ToInt32(sprites_list[foreach_offset].W), Convert.ToInt32(sprites_list[foreach_offset].Z)) * 64f, flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            position = new Vector2(npc_position.X, npc_position.Y - 27),
                            motion = new Vector2(4f, -8f),
                            rotationChange = (float)Math.PI / 16f,
                            shakeIntensity = 1f
                        });


                        // HUD message

                        Game1.showGlobalMessage(i18n.Get("hospital_hud_message", new { character_name = npc.getName() }));


                        // Emote

                        Game1.player.doEmote(12);


                        // Add money

                        Game1.player.Money += 1000;


                        // Loot :)

                        Game1.currentLocation.debris.Add(new Debris(loot_list[foreach_offset], new Vector2(npc_position.X, npc_position.Y), Game1.player.getStandingPosition()));


                        // Lose friendship

                        Game1.player.changeFriendship(-250, npc);


                    }
                    // if distance

                    foreach_offset++;


                }
                // end foreach NPC
          

            }
            // end using Scythe

            return;

        }
        // end onButtonPressed


    }
}