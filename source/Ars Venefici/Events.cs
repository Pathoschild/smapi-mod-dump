/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Emit;
using ArsVenefici.Framework.Interfaces.Spells;
using static HarmonyLib.Code;
using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Spells.Shape;
using StardewValley.Mods;
using StardewValley.Characters;
using SpaceCore;
using System.Reflection.PortableExecutable;
using ArsVenefici.Framework.Skill;
using SpaceCore.Content;
using System.Reflection;
using SpaceShared.APIs;
using ArsVenefici.Framework.GUI.Menus;
using ArsVenefici.Framework.Spells.Effects;
using StardewValley.Network;

namespace ArsVenefici
{
    public class Events
    {
        ModEntry modEntryInstance;

        public Events(ModEntry modEntry)
        {
            modEntryInstance = modEntry;
        }

        /// <summary>
        /// Event that is called when the game launches.
        /// </summary>
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Game Launched Event argument</param>
        /// <remarks> This is used to load the content packs</remarks>
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = modEntryInstance.Helper.ModRegistry.GetGenericModConfigMenuApi(modEntryInstance.Monitor);

            if (configMenu != null)
            {
                configMenu.Register(
                    mod: modEntryInstance.ModManifest,
                    reset: () => modEntryInstance.Config = new ModConfig(),
                    save: () => modEntryInstance.Helper.WriteConfig(modEntryInstance.Config)
                );

                configMenu.AddNumberOption(
                    mod: modEntryInstance.ModManifest,
                    name: () => modEntryInstance.Helper.Translation.Get("config.position_x.name"),
                    tooltip: () => modEntryInstance.Helper.Translation.Get("config.position_x.tooltip"),
                    getValue: () => modEntryInstance.Config.Position.X,
                    setValue: value => modEntryInstance.Config.Position = new(value, modEntryInstance.Config.Position.Y)
                );

                configMenu.AddNumberOption(
                    mod: modEntryInstance.ModManifest,
                    name: () => modEntryInstance.Helper.Translation.Get("config.position_y.name"),
                    tooltip: () => modEntryInstance.Helper.Translation.Get("config.position_y.tooltip"),
                    getValue: () => modEntryInstance.Config.Position.Y,
                    setValue: value => modEntryInstance.Config.Position = new(modEntryInstance.Config.Position.X, value)
                );

                configMenu.AddKeybind(
                    mod: modEntryInstance.ModManifest,
                    name: () => modEntryInstance.Helper.Translation.Get("config.open_spellbook_key.name"),
                    tooltip: () => modEntryInstance.Helper.Translation.Get("config.open_spellbook_key.tooltip"),
                    getValue: () => modEntryInstance.Config.OpenSpellBookButton,
                    setValue: value => modEntryInstance.Config.OpenSpellBookButton = value
                );

                configMenu.AddKeybind(
                     mod: modEntryInstance.ModManifest,
                     name: () => modEntryInstance.Helper.Translation.Get("config.move_spell_lable_key.name"),
                     tooltip: () => modEntryInstance.Helper.Translation.Get("config.move_spell_lable_key.tooltip"),
                     getValue: () => modEntryInstance.Config.MoveCurrentSpellLable,
                     setValue: value => modEntryInstance.Config.MoveCurrentSpellLable = value
                 );

                configMenu.AddKeybind(
                     mod: modEntryInstance.ModManifest,
                     name: () => modEntryInstance.Helper.Translation.Get("config.next_spell_key.name"),
                     tooltip: () => modEntryInstance.Helper.Translation.Get("config.next_spell_key.tooltip"),
                     getValue: () => modEntryInstance.Config.NextSpellButton,
                     setValue: value => modEntryInstance.Config.NextSpellButton = value
                 );

                configMenu.AddKeybind(
                     mod: modEntryInstance.ModManifest,
                     name: () => modEntryInstance.Helper.Translation.Get("config.previous_spell_key.name"),
                     tooltip: () => modEntryInstance.Helper.Translation.Get("config.previous_spell_key.tooltip"),
                     getValue: () => modEntryInstance.Config.PreviousSpellButton,
                     setValue: value => modEntryInstance.Config.PreviousSpellButton = value
                 );

                configMenu.AddKeybind(
                     mod: modEntryInstance.ModManifest,
                     name: () => modEntryInstance.Helper.Translation.Get("config.cast_spell_key.name"),
                     tooltip: () => modEntryInstance.Helper.Translation.Get("config.cast_spell_key.tooltip"),
                     getValue: () => modEntryInstance.Config.CastSpellButton,
                     setValue: value => modEntryInstance.Config.CastSpellButton = value
                 );
            }

            // hook Mana Bar
            {
                var manaBar = modEntryInstance.Helper.ModRegistry.GetApi<IManaBarApi>("spacechase0.ManaBar");
                if (manaBar == null)
                {
                    modEntryInstance.Monitor.Log("No mana bar API???", LogLevel.Error);
                    return;
                }

                ModEntry.Mana = manaBar;
            }
        }

        /// <summary>
        /// Event that is called when a save is loaded.
        /// </summary> 
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Save Loaded Event arguement</param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            modEntryInstance.FixManaPoolIfNeeded(Game1.player);

            if (Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu != null || Game1.eventUp || !Context.IsPlayerFree)
                    return;

                SpellBook spellBook = Game1.player.GetSpellBook();

                if (spellBook != null)
                {
                    string filePath = Path.Combine(modEntryInstance.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");

                    if (File.Exists(filePath))
                    {
                        spellBook.SyncSpellBook(modEntryInstance);
                    }

                    spellBook.CreateSpells(modEntryInstance);
                    spellBook.TurnToSpell();

                    if (Game1.player.HasCustomProfession(Skill.ManaEfficiencyProfession))
                    {
                        spellBook.SetManaCostReductionAmount(Skill.ManaEfficiencyProfession.GetValue<int>());
                    }
                    else if (Game1.player.HasCustomProfession(Skill.ManaEfficiency2Profession))
                    {
                        spellBook.SetManaCostReductionAmount(Skill.ManaEfficiency2Profession.GetValue<int>());
                    }

                    spellBook.SaveSpellBook(modEntryInstance);
                }
            }
        }

        public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // update active effects
            for (int i = modEntryInstance.ActiveEffects.Count - 1; i >= 0; i--)
            {
                IActiveEffect effect = modEntryInstance.ActiveEffects[i];

                //if (!effect.Update(e))
                //    modEntryInstance.ActiveEffects.RemoveAt(i);

                if (effect != null)
                    effect.Update(e);
            }
        }

        public void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
        {

            if (Game1.activeClickableMenu != null || Game1.eventUp || !Context.IsPlayerFree)
                return;

            float factor = (Game1.player.GetCustomSkillLevel(ModEntry.Skill) + 1) / 2; // start at +1 mana at level 1

            if (Game1.player.HasCustomProfession(Skill.ManaRegen2Profession))
                factor *= 3;
            else if (Game1.player.HasCustomProfession(Skill.ManaRegen1Profession))
                factor *= 2;

            int manaRegenValue = (int)(2 * factor);

            Game1.player.AddMana(manaRegenValue);

            // update active effects
            for (int i = modEntryInstance.ActiveEffects.Count - 1; i >= 0; i--)
            {
                IActiveEffect effect = modEntryInstance.ActiveEffects[i];

                //if (!effect.OneSecondUpdate(e))
                //    modEntryInstance.ActiveEffects.RemoveAt(i);

                if (effect != null)
                    effect.OneSecondUpdate(e);
            }
        }

        public void OnItemEaten(object sender, EventArgs args)
        {
            if (Game1.player.itemToEat == null)
            {
                modEntryInstance.Monitor.Log("No item eaten for the item eat event?!?", LogLevel.Error);
                return;
            }

            if(Game1.player.itemToEat != null)
            {
                Game1.player.AddMana((int)(Game1.player.itemToEat.staminaRecoveredOnConsumption()));
            }

        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is SpellBookMenu)
            {
                string filePath = Path.Combine(modEntryInstance.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");

                if (File.Exists(filePath))
                {
                    Game1.player.GetSpellBook().SyncSpellBook(modEntryInstance);
                }

                Game1.player.GetSpellBook().CreateSpells(modEntryInstance);
            }
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {

        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu != null)
                return;

            if (!modEntryInstance.LearnedWizardy)
                return;

            Farmer farmer = Game1.player;
            SpellBook spellBook = farmer.GetSpellBook();
            var helper = SpellHelper.Instance();

            if (Context.IsPlayerFree)
            {
                var input = modEntryInstance.Helper.Input;

                if (e.Button == modEntryInstance.Config.OpenSpellBookButton)//if (input.IsDown(modEntryInstance.Config.OpenSpellBookButton))
                {
                    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift))
                        Game1.activeClickableMenu = new SpellBookMenu(modEntryInstance);
                }

                if (e.Button == modEntryInstance.Config.MoveCurrentSpellLable)
                {
                    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift))
                    {
                        modEntryInstance.Config.Position = new Point((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y);
                        modEntryInstance.Helper.WriteConfig(modEntryInstance.Config);
                    }
                }

                if (e.Button == modEntryInstance.Config.NextSpellButton)
                {
                    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift))
                    {
                        helper.NextShapeGroup(spellBook.GetCurrentSpell());
                    }
                    else
                    {
                        spellBook.SetCurrentSpellIndex(ValidateSpellIndex(spellBook.GetCurrentSpellIndex() + 1));

                        spellBook.TurnToSpell();
                        spellBook.SaveSpellBook(modEntryInstance);
                    }

                }

                if (e.Button == modEntryInstance.Config.PreviousSpellButton)
                {
                    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift))
                    {
                        helper.PrevShapeGroup(spellBook.GetCurrentSpell());
                    }
                    else
                    {
                        spellBook.SetCurrentSpellIndex(ValidateSpellIndex(spellBook.GetCurrentSpellIndex() - 1));

                        spellBook.TurnToSpell();
                        spellBook.SaveSpellBook(modEntryInstance);
                    }
                }

                if (e.Button == modEntryInstance.Config.CastSpellButton)
                {
                    CastSpell(farmer);
                }

                //if(e.Button == SButton.G)
                //{
                //    Game1.player.eventsSeen.Remove(modEntryInstance.LearnedWizardryEventId.ToString());
                //}
            }
        }

        public void OnNetworkCast(IncomingMessage msg)
        {
            Farmer player = Game1.getFarmer(msg.FarmerID);

            if (player == null)
                return;

            CastSpell(player);
        }

        private void CastSpell(Farmer farmer)
        {
            SpellHelper spellHelper = SpellHelper.Instance();
            ISpell spell = farmer.GetSpellBook().GetCurrentSpell();

            if (spell == null)
                modEntryInstance.Monitor.Log("Spell is null!", LogLevel.Info);

            if (spell != null && spell.IsValid())
            {
                //if (!spell.IsContinuous()) return;

                SpellCastResult result = spell.Cast(new CharacterEntityWrapper(farmer), farmer.currentLocation, 0, true, true);

                if (result.GetSpellCastResultType() == SpellCastResultType.EFFECT_FAILED)
                {
                    //Game1.addHUDMessage(new HUDMessage("Error Casting Spell", 3));
                    Game1.showRedMessage("Failed Casting Spell");
                }
                else if (result.GetSpellCastResultType() == SpellCastResultType.NOT_ENOUGH_MANA)
                {
                    Game1.showRedMessage("Failed Casting Spell: Not Enough Mana!");
                }
            }
        }

        public int ValidateSpellIndex(int index)
        {

            if (index < 0)
            {
                index = 9;
            }
            else if (index >= 10)
            {
                index = 0;
            }

            return index;
        }


        public void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (Game1.activeClickableMenu != null || Game1.eventUp || !Context.IsPlayerFree)
                return;

            RenderTouchIndicator(e.SpriteBatch);

            // draw active effects
            foreach (IActiveEffect effect in modEntryInstance.ActiveEffects)
                effect.Draw(e.SpriteBatch);
        }

        public void RenderTouchIndicator(SpriteBatch spriteBatch)
        {

            if (!modEntryInstance.LearnedWizardy)
                return;

            SpellBook spellBook = Game1.player.GetSpellBook();
            //ISpell spell = spellBook.GetCurrentSpell();
            ISpell spell = spellBook.GetSpells()[spellBook.GetCurrentSpellIndex()];

            if (spell != null)
            {
                if (Game1.activeClickableMenu == null && !Game1.eventUp && Game1.player.IsLocalPlayer && /*Game1.player.CurrentTool != null &&*/ (Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.options.alwaysShowToolHitLocation) && /*this.CurrentTool.doesShowTileLocationMarker() &&*/ (!Game1.options.hideToolHitLocationWhenInMotion || !Game1.player.isMoving()))
                {
                    Vector2 local = Vector2.One;
                    Texture2D texture = modEntryInstance.Helper.ModContent.Load<Texture2D>("assets/farmer/touch_indicator.png");

                    if (spell.FirstShape(spell.CurrentShapeGroupIndex()) is Touch)
                    {
                       //local = Game1.GlobalToLocal(Game1.viewport, Utility.clampToTile(Game1.player.GetToolLocation(true)));
                       local = Utils.AbsolutePosToScreenPos(Utility.clampToTile(Game1.player.GetToolLocation(true)));
                    }
                    else if(spell.FirstShape(spell.CurrentShapeGroupIndex()) is EtherialTouch)
                    {
                        //local = Game1.GlobalToLocal(Game1.viewport, Utility.clampToTile(new Vector2(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y)));    
                        local = Utils.AbsolutePosToScreenPos(Utility.clampToTile(new Vector2(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y)));    
                    }

                    spriteBatch.Draw(texture, local, new Rectangle(0, 0, 64, 64), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, local.Y / 10000f);
                }
            }
        }

        public void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            //// draw active effects
            //foreach (IActiveEffect effect in modEntryInstance.ActiveEffects)
            //    effect.Draw(e.SpriteBatch);
        }


        /// <summary>Raised after drawing the HUD (item toolbar, clock, etc) to the sprite batch, but before it's rendered to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Game1.activeClickableMenu != null || Game1.eventUp || !Context.IsPlayerFree)
                return;

            if (!modEntryInstance.LearnedWizardy)
                return;

            int x = modEntryInstance.Config.Position.X;
            int y = modEntryInstance.Config.Position.Y;

            RenderSpellLable(x, y, Game1.player.GetSpellBook());
        }

        public void RenderSpellLable(int x, int y, SpellBook spellBook)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            ClickableComponent spellNameLable = new ClickableComponent(new Rectangle(x, y, 64, 64), "Spell Name");
            //spellNameLable.draw(spriteBatch);

            int spellPageIndex = spellBook.GetCurrentSpellIndex() + 1;
            string spellName = "";
            int spellShapeGroupIndex;

            int manaCost;

            if (spellBook.GetCurrentSpell().GetName() != null)
                spellName = spellBook.GetCurrentSpell().GetName();

            spellShapeGroupIndex = spellBook.GetCurrentSpell().CurrentShapeGroupIndex() + 1;

            manaCost = spellBook.GetCurrentSpell().Mana();

            string spellText;

            if (spellName == "")
            {
                spellText = spellName;
            }
            else
            {
                spellText = spellName + " | " + modEntryInstance.Helper.Translation.Get("ui.spell_label.shape_group.name") + ": " + spellShapeGroupIndex + " | " + modEntryInstance.Helper.Translation.Get("ui.mana_cost.name") + ": "  + manaCost;
            }

            string text = spellPageIndex + " : " + spellText;

            //if (text != null)
            //{
            //    //IClickableMenu.drawTextureBox(spriteBatch, spellNameLable.bounds.X - 15, spellNameLable.bounds.Y - 50, spellNameLable.bounds.Width + 100 + text.Length, spellNameLable.bounds.Height, Color.White);
            //    //IClickableMenu.drawTextureBox(spriteBatch, spellNameLable.bounds.X - 15, spellNameLable.bounds.Y - 50, spellNameLable.bounds.Width + (int)Game1.smallFont.MeasureString(text).Y + 32 , spellNameLable.bounds.Height, Color.White);

            //    //IClickableMenu.drawTextureBox(spriteBatch, spellNameLable.bounds.X - 15, spellNameLable.bounds.Y - 50, text.Length + 400, spellNameLable.bounds.Height, Color.White);
            //}

            //Color color = Game1.textColor;
            Color color = Color.White;

            //Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(spellNameLable.bounds.X, spellNameLable.bounds.Y - 30), color);
            Utility.drawTextWithColoredShadow(spriteBatch, text, Game1.smallFont, new Vector2(spellNameLable.bounds.X, spellNameLable.bounds.Y - 30), color, Color.Gray);

            //if (text.Length > 0)
            //    Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(spellNameLable.bounds.X + Game1.tileSize / 3 - Game1.smallFont.MeasureString(text).X / 2f, spellNameLable.bounds.Y + Game1.tileSize / 2), color);
        }

        public void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            string eventSong = "WizardSong";
            string eventViewPort;
            string eventActors;
            string eventAttributes = "skippable/ignoreCollisions farmer";
            string eventMove;

            if (modEntryInstance.isSVEInstalled && e.NewLocation.Name == "Custom_WizardBasement")
            {
                eventViewPort = "15 7";
                eventActors = "Wizard 16 5 0 farmer 16 11 0";
                eventAttributes = "skippable/ignoreCollisions farmer";
                eventMove = "move farmer 0 -4 0/pause 500/faceDirection Wizard 1/faceDirection Wizard 2";

                WizardEvent(eventSong, eventViewPort, eventActors, eventAttributes, eventMove, e.NewLocation);
            }
            else if (!modEntryInstance.isSVEInstalled && e.NewLocation.Name == "WizardHouse")
            {
                eventViewPort = "0 5";
                eventActors = "Wizard 8 6 0 farmer 8 15 0";
                eventAttributes = "skippable/ignoreCollisions farmer";
                eventMove = "move farmer 0 -8 0/pause 500/faceDirection Wizard 1/faceDirection Wizard 2";

                WizardEvent(eventSong, eventViewPort, eventActors, eventAttributes, eventMove, e.NewLocation);
            }
        }

        private void WizardEvent(string eventSong, string eventViewPort, string eventActors, string eventAttributes, string eventMove, GameLocation gameLocation)
        {
            if (!modEntryInstance.LearnedWizardy && Game1.player.friendshipData.TryGetValue("Wizard", out Friendship wizardFriendship) && wizardFriendship.Points >= 1000)
            {
                string eventWizardDialogue1 = "pause 1500/speak Wizard \"{0}\"/pause 1000";
                eventWizardDialogue1 = string.Format(eventWizardDialogue1, modEntryInstance.Helper.Translation.Get("event.wizard.1"));

                string eventWizardDialogue2 = "speak Wizard \"{0}\"/pause 1000/speak Wizard \"{1}\"/pause 1000/jump farmer/pause 1000";
                eventWizardDialogue2 = string.Format(eventWizardDialogue2,
                    modEntryInstance.Helper.Translation.Get("event.wizard.2"),
                    modEntryInstance.Helper.Translation.Get("event.wizard.3")
                );

                string eventWizardDialogue3 = "speak Wizard \"{0}\"/pause 1000/playSound reward/message \"{1}\"/pause 1500";
                eventWizardDialogue3 = string.Format(eventWizardDialogue3,
                    modEntryInstance.Helper.Translation.Get("event.wizard.4"),
                    modEntryInstance.Helper.Translation.Get("event.message.1")
                );

                string eventWizardDialogue4 = "speak Wizard \"{0}#$b#{1}#$b#{2}#$b#{3}#$b#{4}#$b#{5}\"/pause 1500/message \"{6}\"/pause 1500/speak Wizard \"{7}\"/message \"{8}\"/pause 1000/message \"{9}\"/speak Wizard \"{10}\"";

                eventWizardDialogue4 = string.Format(eventWizardDialogue4,
                     modEntryInstance.Helper.Translation.Get("event.wizard.5"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.6"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.7"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.8"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.9"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.10"),
                     modEntryInstance.Helper.Translation.Get("event.message.2"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.11"),
                     modEntryInstance.Helper.Translation.Get("event.message.3"),
                     modEntryInstance.Helper.Translation.Get("event.message.4"),
                     modEntryInstance.Helper.Translation.Get("event.wizard.12")
                 );


                string eventWizardAboveHeadDialogue = "showFrame Wizard 16/textAboveHead Wizard \"{0}\"";

                eventWizardAboveHeadDialogue = string.Format(eventWizardAboveHeadDialogue,
                    modEntryInstance.Helper.Translation.Get("event.wizard.abovehead")
                );


                string eventEnd = "pause 750/fade 750/end";

                string eventStr = string.Join(
                    "/",
                    eventSong,
                    eventViewPort,
                    eventActors,
                    eventAttributes,
                    eventMove,
                    eventWizardDialogue1,
                    eventWizardDialogue2,
                    eventWizardDialogue3,
                    eventWizardDialogue4,
                    eventWizardAboveHeadDialogue,
                    eventEnd
                );


                //e.NewLocation.currentEvent = new Event(eventStr, ModEntry.LearnedMagicEventId);
                gameLocation.currentEvent = new Event(eventStr, null, modEntryInstance.LearnedWizardryEventId.ToString());

                Game1.eventUp = true;
                Game1.displayHUD = false;
                Game1.player.CanMove = false;
                Game1.player.showNotCarrying();

                Game1.player.AddCustomSkillExperience(ModEntry.Skill, ModEntry.Skill.ExperienceCurve[0]);
                modEntryInstance.FixManaPoolIfNeeded(Game1.player, overrideWizardryLevel: 1); // let player start using magic immediately
                Game1.player.eventsSeen.Add(modEntryInstance.LearnedWizardryEventId.ToString());
            }
        }
    }
}
