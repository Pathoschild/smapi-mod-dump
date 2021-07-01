/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Magic.Framework.Game.Interface;
using Magic.Framework.Schools;
using Magic.Framework.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Magic.Framework
{
    // TODO: Refactor this mess
    internal static class Magic
    {
        public static Skill Skill;

        public static EventHandler<AnalyzeEventArgs> OnAnalyzeCast;

        private static IInputHelper InputHelper;

        private static Texture2D SpellBg;
        private static Texture2D ManaBg;
        private static Texture2D ManaFg;

        /// <summary>The ID of the event in which the player learns magic from the Wizard.</summary>
        public const int LearnedMagicEventId = 90001;

        /// <summary>The number of mana points gained per magic level.</summary>
        public const int ManaPointsPerLevel = 100;

        /// <summary>The active effects, spells, or projectiles which should be updated or drawn.</summary>
        private static readonly IList<IActiveEffect> ActiveEffects = new List<IActiveEffect>();

        public const string MsgCast = "spacechase0.Magic.Cast";

        /// <summary>The self-updating views of magic metadata for each player.</summary>
        /// <remarks>This should only be accessed through <see cref="GetSpellBook"/> or <see cref="Extensions.GetSpellBook"/> to make sure an updated instance is retrieved.</remarks>
        private static readonly IDictionary<long, SpellBook> SpellBookCache = new Dictionary<long, SpellBook>();

        internal static void Init(IModEvents events, IInputHelper inputHelper, Func<long> getNewId)
        {
            Magic.InputHelper = inputHelper;

            Magic.LoadAssets();

            SpellManager.Init(getNewId);

            events.GameLoop.UpdateTicked += Magic.OnUpdateTicked;

            events.Input.ButtonPressed += Magic.OnButtonPressed;
            events.Input.ButtonReleased += Magic.OnButtonReleased;

            events.GameLoop.TimeChanged += Magic.OnTimeChanged;
            events.Player.Warped += Magic.OnWarped;

            SpaceEvents.OnBlankSave += Magic.OnBlankSave;
            SpaceEvents.OnItemEaten += Magic.OnItemEaten;
            SpaceEvents.ActionActivated += Magic.ActionTriggered;
            Networking.RegisterMessageHandler(Magic.MsgCast, Magic.OnNetworkCast);

            events.Display.RenderingHud += Magic.OnRenderingHud;
            events.Display.RenderedHud += Magic.OnRenderedHud;

            Magic.OnAnalyzeCast += Magic.OnAnalyze;
            Magic.OnAnalyzeCast += (sender, e) =>
            {
                Mod.Instance.Api.InvokeOnAnalyzeCast(sender as Farmer);
            };

            Skills.RegisterSkill(Magic.Skill = new Skill());

            Command.Register("player_learnspell", Magic.LearnSpellCommand);
            Command.Register("magicmenu", Magic.MagicMenuCommand);

            PyTK.CustomTV.CustomTVMod.addChannel("magic", Mod.Instance.Helper.Translation.Get("tv.analyzehints.name"), Magic.OnTvChannelSelected);
        }

        /// <summary>Get a self-updating view of a player's magic metadata.</summary>
        /// <param name="player">The player whose spell book to get.</param>
        public static SpellBook GetSpellBook(Farmer player)
        {
            if (!Magic.SpellBookCache.TryGetValue(player.UniqueMultiplayerID, out SpellBook book) || !object.ReferenceEquals(player, book.Player))
                Magic.SpellBookCache[player.UniqueMultiplayerID] = book = new SpellBook(player);

            return book;
        }

        private static void OnAnalyze(object sender, AnalyzeEventArgs e)
        {
            var farmer = sender as Farmer;
            if (farmer != Game1.player)
                return;

            SpellBook spellBook = farmer.GetSpellBook();

            List<string> spellsLearnt = new List<string>();
            if (farmer.CurrentItem != null)
            {
                if (farmer.CurrentTool != null)
                {
                    if (farmer.CurrentTool is StardewValley.Tools.Axe || farmer.CurrentTool is StardewValley.Tools.Pickaxe)
                        spellsLearnt.Add("toil:cleardebris");
                    else if (farmer.CurrentTool is StardewValley.Tools.Hoe)
                        spellsLearnt.Add("toil:till");
                    else if (farmer.CurrentTool is StardewValley.Tools.WateringCan)
                        spellsLearnt.Add("toil:water");
                }
                else if (farmer.CurrentItem is Boots)
                {
                    spellsLearnt.Add("life:evac");
                }
                else if (farmer.ActiveObject != null)
                {
                    if (!farmer.ActiveObject.bigCraftable.Value)
                    {
                        int index = farmer.ActiveObject.ParentSheetIndex;
                        if (index == 395) // Coffee
                            spellsLearnt.Add("life:haste");
                        else if (index == 773) // Life elixir
                            spellsLearnt.Add("life:heal");
                        else if (index == 86) // Earth crystal
                            spellsLearnt.Add("nature:shockwave");
                        else if (index == 82) // Fire quartz
                            spellsLearnt.Add("elemental:fireball");
                        else if (index == 161) // Ice Pip
                            spellsLearnt.Add("elemental:frostbolt");
                    }
                }
            }
            foreach (var lightSource in farmer.currentLocation.sharedLights.Values)
            {
                if (Utility.distance(e.TargetX, lightSource.position.X, e.TargetY, lightSource.position.Y) < lightSource.radius.Value * Game1.tileSize)
                {
                    spellsLearnt.Add("nature:lantern");
                    break;
                }
            }
            var tilePos = new Vector2(e.TargetX / Game1.tileSize, e.TargetY / Game1.tileSize);
            if (farmer.currentLocation.terrainFeatures.TryGetValue(tilePos, out TerrainFeature feature) && feature is HoeDirt dirt && dirt.crop != null)
                spellsLearnt.Add("nature:tendrils");

            // TODO: Add proper tilesheet check
            var tile = farmer.currentLocation.map.GetLayer("Buildings").Tiles[(int)tilePos.X, (int)tilePos.Y];
            if (tile != null && tile.TileIndex == 173)
                spellsLearnt.Add("elemental:descend");
            if (farmer.currentLocation is Farm farm)
            {
                foreach (var clump in farm.resourceClumps)
                {
                    if (clump.parentSheetIndex.Value == 622 && new Rectangle((int)clump.tile.Value.X, (int)clump.tile.Value.Y, clump.width.Value, clump.height.Value).Contains((int)tilePos.X, (int)tilePos.Y))
                        spellsLearnt.Add("eldritch:meteor");
                }
            }
            if (farmer.currentLocation.doesTileHaveProperty((int)tilePos.X, (int)tilePos.Y, "Action", "Buildings") == "EvilShrineLeft")
                spellsLearnt.Add("eldritch:lucksteal");
            if (farmer.currentLocation is StardewValley.Locations.MineShaft { mineLevel: 100 } ms && ms.waterTiles[(int)tilePos.X, (int)tilePos.Y])
                spellsLearnt.Add("eldritch:bloodmana");

            for (int i = spellsLearnt.Count - 1; i >= 0; --i)
                if (spellBook.KnowsSpell(spellsLearnt[i], 0))
                    spellsLearnt.RemoveAt(i);
            if (spellsLearnt.Count > 0)
            {
                Game1.playSound("secret1");
                foreach (string spell in spellsLearnt)
                {
                    Log.Debug("Player learnt spell: " + spell);
                    spellBook.LearnSpell(spell, 0, true);
                    //Game1.drawObjectDialogue(Mod.instance.Helper.Translation.Get("spell.learn", new { spellName = Mod.instance.Helper.Translation.Get("spell." + spell + ".name") }));
                    Game1.addHUDMessage(new HUDMessage(Mod.Instance.Helper.Translation.Get("spell.learn", new { spellName = SpellManager.Get(spell).GetTranslatedName() })));
                }
            }

            // Temporary - 0.3.0 will add dungeons to get these
            bool knowsAll = true;
            foreach (string schoolId in School.GetSchoolList())
            {
                var school = School.GetSchool(schoolId);

                bool knowsAllSchool = true;
                foreach (var spell in school.GetSpellsTier1())
                {
                    if (!spellBook.KnowsSpell(spell, 0))
                    {
                        knowsAll = knowsAllSchool = false;
                        break;
                    }
                }
                foreach (var spell in school.GetSpellsTier2())
                {
                    if (!spellBook.KnowsSpell(spell, 0))
                    {
                        knowsAll = knowsAllSchool = false;
                        break;
                    }
                }

                // Have to know all other spells for the arcane one
                if (schoolId == SchoolId.Arcane)
                    continue;

                var ancientSpell = school.GetSpellsTier3()[0];
                if (knowsAllSchool && !spellBook.KnowsSpell(ancientSpell, 0))
                {
                    Log.Debug("Player learnt ancient spell: " + ancientSpell);
                    spellBook.LearnSpell(ancientSpell, 0, true);
                    Game1.addHUDMessage(new HUDMessage(Mod.Instance.Helper.Translation.Get("spell.learn.ancient", new { spellName = ancientSpell.GetTranslatedName() })));
                }
            }

            var rewindSpell = School.GetSchool(SchoolId.Arcane).GetSpellsTier3()[0];
            if (knowsAll && !spellBook.KnowsSpell(rewindSpell, 0))
            {
                Log.Debug("Player learnt ancient spell: " + rewindSpell);
                spellBook.LearnSpell(rewindSpell, 0, true);
                Game1.addHUDMessage(new HUDMessage(Mod.Instance.Helper.Translation.Get("spell.learn.ancient", new { spellName = rewindSpell.GetTranslatedName() })));
            }
        }

        private static void OnNetworkCast(IncomingMessage msg)
        {
            Farmer player = Game1.getFarmer(msg.FarmerID);
            if (player == null)
                return;

            IActiveEffect effect = player.GetSpellBook().CastSpell(msg.Reader.ReadString(), msg.Reader.ReadInt32(), msg.Reader.ReadInt32(), msg.Reader.ReadInt32());
            if (effect != null)
                Magic.ActiveEffects.Add(effect);
        }

        private static void OnBlankSave(object sender, EventArgs args)
        {
            Magic.PlaceAltar(Mod.Config.AltarLocation, Mod.Config.AltarX, Mod.Config.AltarY, 54 * 4);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // update active effects
            for (int i = Magic.ActiveEffects.Count - 1; i >= 0; i--)
            {
                IActiveEffect effect = Magic.ActiveEffects[i];
                if (!effect.Update(e))
                    Magic.ActiveEffects.RemoveAt(i);
            }
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open). Content drawn to the sprite batch at this point will appear under the HUD.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // draw active effects
            foreach (IActiveEffect effect in Magic.ActiveEffects)
                effect.Draw(e.SpriteBatch);
        }

        /// <summary>Raised after drawing the HUD (item toolbar, clock, etc) to the sprite batch, but before it's rendered to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Game1.activeClickableMenu != null || Game1.eventUp || !Game1.player.eventsSeen.Contains(Magic.LearnedMagicEventId))
                return;

            SpriteBatch b = e.SpriteBatch;

            Vector2 manaPos = new Vector2(20, Game1.uiViewport.Height - 56 * 4 - 20);

            bool hasFifthSpellSlot = Game1.player.HasCustomProfession(Skill.ProfessionFifthSpellSlot);

            int spotYAffector = -1;
            if (hasFifthSpellSlot)
                spotYAffector = 0;
            Point[] spots =
            {
                new((int)manaPos.X + Magic.ManaBg.Width * 4 + 20 + 60 * 0, Game1.uiViewport.Height - 20 - 50 - 60 * ( 4 + spotYAffector )),
                new((int)manaPos.X + Magic.ManaBg.Width * 4 + 20 + 60 * 0, Game1.uiViewport.Height - 20 - 50 - 60 * ( 3 + spotYAffector )),
                new((int)manaPos.X + Magic.ManaBg.Width * 4 + 20 + 60 * 0, Game1.uiViewport.Height - 20 - 50 - 60 * ( 2 + spotYAffector )),
                new((int)manaPos.X + Magic.ManaBg.Width * 4 + 20 + 60 * 0, Game1.uiViewport.Height - 20 - 50 - 60 * ( 1 + spotYAffector )),
                new((int)manaPos.X + Magic.ManaBg.Width * 4 + 20 + 60 * 0, Game1.uiViewport.Height - 20 - 50 - 60 * ( 0 + spotYAffector ))
            };

            // read spell info
            SpellBook spellBook = Game1.player.GetSpellBook();
            PreparedSpellBar prepared = spellBook.GetPreparedSpells();
            if (prepared == null)
                return;

            // render empty spell slots
            for (int i = 0; i < (hasFifthSpellSlot ? 5 : 4); ++i)
                b.Draw(Magic.SpellBg, new Rectangle(spots[i].X, spots[i].Y, 50, 50), Color.White);

            // render spell bar count
            string prepStr = (spellBook.SelectedPrepared + 1) + "/" + spellBook.Prepared.Count;
            b.DrawString(Game1.dialogueFont, prepStr, new Vector2(spots[Game1.down].X + 25 + 2, spots[Game1.up].Y - 35 + 0), Color.Black, 0, new Vector2(Game1.dialogueFont.MeasureString(prepStr).X / 2, 0), 0.6f, SpriteEffects.None, 0);
            b.DrawString(Game1.dialogueFont, prepStr, new Vector2(spots[Game1.down].X + 25 - 2, spots[Game1.up].Y - 35 + 0), Color.Black, 0, new Vector2(Game1.dialogueFont.MeasureString(prepStr).X / 2, 0), 0.6f, SpriteEffects.None, 0);
            b.DrawString(Game1.dialogueFont, prepStr, new Vector2(spots[Game1.down].X + 25 + 0, spots[Game1.up].Y - 35 + 2), Color.Black, 0, new Vector2(Game1.dialogueFont.MeasureString(prepStr).X / 2, 0), 0.6f, SpriteEffects.None, 0);
            b.DrawString(Game1.dialogueFont, prepStr, new Vector2(spots[Game1.down].X + 25 + 0, spots[Game1.up].Y - 35 - 2), Color.Black, 0, new Vector2(Game1.dialogueFont.MeasureString(prepStr).X / 2, 0), 0.6f, SpriteEffects.None, 0);
            b.DrawString(Game1.dialogueFont, prepStr, new Vector2(spots[Game1.down].X + 25, spots[Game1.up].Y - 35), Color.White, 0, new Vector2(Game1.dialogueFont.MeasureString(prepStr).X / 2, 0), 0.6f, SpriteEffects.None, 0);

            // render spell bar
            for (int i = 0; i < (hasFifthSpellSlot ? 5 : 4) && i < prepared.Spells.Count; ++i)
            {
                PreparedSpell prep = prepared.GetSlot(i);
                if (prep == null)
                    continue;

                Spell spell = SpellManager.Get(prep.SpellId);
                if (spell == null || spell.Icons.Length <= prep.Level || spell.Icons[prep.Level] == null)
                    continue;

                b.Draw(spell.Icons[prep.Level], new Rectangle(spots[i].X, spots[i].Y, 50, 50), spellBook.CanCastSpell(spell, prep.Level) ? Color.White : new Color(128, 128, 128));
            }
        }

        private static void LoadAssets()
        {
            Magic.SpellBg = Content.LoadTexture("interface/spellbg.png");
            Magic.ManaBg = Content.LoadTexture("interface/manabg.png");

            Color manaCol = new Color(0, 48, 255);
            Magic.ManaFg = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Magic.ManaFg.SetData(new[] { manaCol });
        }

        private static bool CastPressed;

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            bool hasFifthSpellSlot = Game1.player.HasCustomProfession(Skill.ProfessionFifthSpellSlot);

            if (e.Button == Mod.Config.Key_Cast)
                Magic.CastPressed = true;

            if (Game1.activeClickableMenu != null)
                return;

            if (e.Button == Mod.Config.Key_SwapSpells)
            {
                Game1.player.GetSpellBook().SwapPreparedSet();
            }
            else if (Magic.CastPressed &&
                     (e.Button == Mod.Config.Key_Spell1 || e.Button == Mod.Config.Key_Spell2 ||
                      e.Button == Mod.Config.Key_Spell3 || e.Button == Mod.Config.Key_Spell4 ||
                      (e.Button == Mod.Config.Key_Spell5 && hasFifthSpellSlot)))
            {
                int slotIndex = 0;
                if (e.Button == Mod.Config.Key_Spell1) slotIndex = 0;
                else if (e.Button == Mod.Config.Key_Spell2) slotIndex = 1;
                else if (e.Button == Mod.Config.Key_Spell3) slotIndex = 2;
                else if (e.Button == Mod.Config.Key_Spell4) slotIndex = 3;
                else if (e.Button == Mod.Config.Key_Spell5) slotIndex = 4;

                Magic.InputHelper.Suppress(e.Button);

                SpellBook spellBook = Game1.player.GetSpellBook();

                PreparedSpellBar prepared = spellBook.GetPreparedSpells();
                PreparedSpell slot = prepared?.GetSlot(slotIndex);
                if (slot == null)
                    return;

                Spell spell = SpellManager.Get(slot.SpellId);
                if (spell == null)
                    return;

                if (spellBook.CanCastSpell(spell, slot.Level))
                {
                    Log.Trace("Casting " + slot.SpellId);

                    IActiveEffect effect = spellBook.CastSpell(spell, slot.Level);
                    if (effect != null)
                        Magic.ActiveEffects.Add(effect);
                    Game1.player.AddMana(-spell.GetManaCost(Game1.player, slot.Level));
                }
            }
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == Mod.Config.Key_Cast)
            {
                Magic.CastPressed = false;
            }
        }

        private static float CarryoverManaRegen;
        private static void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            float manaRegen = Game1.player.GetCustomSkillLevel(Magic.Skill) / 2 + Magic.CarryoverManaRegen;
            if (Game1.player.HasCustomProfession(Skill.ProfessionManaRegen2))
                manaRegen *= 3;
            else if (Game1.player.HasCustomProfession(Skill.ProfessionManaRegen1))
                manaRegen *= 2;

            Game1.player.AddMana((int)manaRegen);
            Magic.CarryoverManaRegen = manaRegen - (int)manaRegen;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            // update spells
            EvacSpell.OnLocationChanged();

            // check events
            if (e.NewLocation.Name == "WizardHouse" && !Game1.player.eventsSeen.Contains(Magic.LearnedMagicEventId) && Game1.player.friendshipData.TryGetValue("Wizard", out Friendship wizardFriendship) && wizardFriendship.Points > 750)
            {
                string eventStr = "WizardSong/0 5/Wizard 8 5 0 farmer 8 15 0/skippable/ignoreCollisions farmer/move farmer 0 -8 0/speak Wizard \"{0}#$b#{1}#$b#{2}#$b#{3}#$b#{4}#$b#{5}#$b#{6}#$b#{7}#$b#{8}\"/textAboveHead Wizard \"{9}\"/pause 750/fade 750/end";
                eventStr = string.Format(
                    eventStr,
                    Mod.Instance.Helper.Translation.Get("event.wizard.1"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.2"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.3"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.4"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.5"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.6"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.7"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.8"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.9"),
                    Mod.Instance.Helper.Translation.Get("event.wizard.abovehead")
                );
                e.NewLocation.currentEvent = new Event(eventStr, Magic.LearnedMagicEventId);
                Game1.eventUp = true;
                Game1.displayHUD = false;
                Game1.player.CanMove = false;
                Game1.player.showNotCarrying();

                Game1.player.AddCustomSkillExperience(Magic.Skill, Magic.Skill.ExperienceCurve[0]);
                Magic.FixManaPoolIfNeeded(Game1.player, overrideMagicLevel: 1); // let player start using magic immediately

                SpellBook spellBook = Game1.player.GetSpellBook();
                spellBook.LearnSpell("arcane:analyze", 0, true);
                spellBook.LearnSpell("arcane:magicmissle", 0, true);
                spellBook.LearnSpell("arcane:enchant", 0, true);
                spellBook.LearnSpell("arcane:disenchant", 0, true);

                Game1.player.eventsSeen.Add(Magic.LearnedMagicEventId);
            }
        }

        private static void ActionTriggered(object sender, EventArgsAction args)
        {
            if (args.Action == "MagicAltar")
            {
                if (!Game1.player.eventsSeen.Contains(Magic.LearnedMagicEventId))
                {
                    Game1.drawObjectDialogue(Mod.Instance.Helper.Translation.Get("altar.glow"));
                }
                else
                {
                    Game1.playSound("secret1");
                    Game1.activeClickableMenu = new MagicMenu();// School.getSchool(actionArgs[1]));
                }
            }
        }

        private static void OnItemEaten(object sender, EventArgs args)
        {
            if (Game1.player.itemToEat == null)
            {
                Log.Warn("No item eaten for the item eat event?!?");
                return;
            }
            if (Game1.player.itemToEat.ParentSheetIndex == Mod.Ja.GetObjectId("Magic Elixir"))
                Game1.player.AddMana(Game1.player.GetMaxMana());
        }

        private static void OnTvChannelSelected(TV tv, TemporaryAnimatedSprite sprite, Farmer farmer, string answer)
        {
            TemporaryAnimatedSprite tas = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 305, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);

            string transKey = "tv.analyzehints.notmagical";
            Random r = new Random((int)Game1.stats.DaysPlayed + (int)(Game1.uniqueIDForThisGame / 2));
            if (Game1.player.GetMaxMana() > 0)
                transKey = "tv.analyzehints." + (r.Next(12) + 1);

            PyTK.CustomTV.CustomTVMod.showProgram(tas, Mod.Instance.Helper.Translation.Get(transKey));
        }

        public static void PlaceAltar(string locName, int x, int y, int baseAltarIndex)
        {
            Log.Debug($"Placing altar @ {locName}({x}, {y})");

            // AddTileSheet sorts the tilesheets by ID after adding them.
            // The game sometimes refers to tilesheets by their index (such as in Beach.fixBridge)
            // Prepending this to the ID should ensure that this tilesheet is added to the end,
            // which preserves the normal indices of the tilesheets.
            char comeLast = '\u03a9'; // Omega

            GameLocation loc = Game1.getLocationFromName(locName);

            var tileSheet = Content.LoadTilesheet("altarsobjects", loc.Map, out Dictionary<int, SpaceCore.Content.TileAnimation> anims);
            tileSheet.Id = comeLast + tileSheet.Id;
            loc.map.AddTileSheet(tileSheet);
            if (Game1.currentLocation == loc)
                loc.map.LoadTileSheets(Game1.mapDisplayDevice);

            var front = loc.Map.GetLayer("Front");
            var buildings = loc.Map.GetLayer("Buildings");

            front.Tiles[x + 0, y - 1] = anims[baseAltarIndex + 0 + 0 * 18].MakeTile(tileSheet, front);
            front.Tiles[x + 1, y - 1] = anims[baseAltarIndex + 1 + 0 * 18].MakeTile(tileSheet, front);
            front.Tiles[x + 2, y - 1] = anims[baseAltarIndex + 2 + 0 * 18].MakeTile(tileSheet, front);
            buildings.Tiles[x + 0, y + 0] = anims[baseAltarIndex + 0 + 1 * 18].MakeTile(tileSheet, buildings);
            buildings.Tiles[x + 1, y + 0] = anims[baseAltarIndex + 1 + 1 * 18].MakeTile(tileSheet, buildings);
            buildings.Tiles[x + 2, y + 0] = anims[baseAltarIndex + 2 + 1 * 18].MakeTile(tileSheet, buildings);
            buildings.Tiles[x + 0, y + 1] = anims[baseAltarIndex + 0 + 2 * 18].MakeTile(tileSheet, buildings);
            buildings.Tiles[x + 1, y + 1] = anims[baseAltarIndex + 1 + 2 * 18].MakeTile(tileSheet, buildings);
            buildings.Tiles[x + 2, y + 1] = anims[baseAltarIndex + 2 + 2 * 18].MakeTile(tileSheet, buildings);
            loc.setTileProperty(x + 0, y + 0, "Buildings", "Action", "MagicAltar");
            loc.setTileProperty(x + 1, y + 0, "Buildings", "Action", "MagicAltar");
            loc.setTileProperty(x + 2, y + 0, "Buildings", "Action", "MagicAltar");
            loc.setTileProperty(x + 0, y + 1, "Buildings", "Action", "MagicAltar");
            loc.setTileProperty(x + 1, y + 1, "Buildings", "Action", "MagicAltar");
            loc.setTileProperty(x + 2, y + 1, "Buildings", "Action", "MagicAltar");
        }

        /// <summary>Fix the player's mana pool to match their skill level if needed.</summary>
        /// <param name="player">The player to fix.</param>
        /// <param name="overrideMagicLevel">The magic skill level, or <c>null</c> to get it from the player.</param>
        public static void FixManaPoolIfNeeded(Farmer player, int? overrideMagicLevel = null)
        {
            int magicLevel = overrideMagicLevel ?? Game1.player.GetCustomSkillLevel(Skill.MagicSkillId);
            int expectedPoints = magicLevel * Magic.ManaPointsPerLevel;

            if (Game1.player.GetMaxMana() < expectedPoints)
            {
                Game1.player.SetMaxMana(expectedPoints);
                Game1.player.AddMana(expectedPoints);
            }
        }

        private static void LearnSpellCommand(string[] args)
        {
            SpellBook spellBook = Game1.player.GetSpellBook();

            if (args.Length == 1 && args[0] == "all")
            {
                foreach (string spellName in SpellManager.GetAll())
                {
                    var curSpell = SpellManager.Get(spellName);
                    spellBook.LearnSpell(curSpell, curSpell.GetMaxCastingLevel(), true);
                }

                return;
            }

            if (args.Length != 2 || (args.Length > 1 && (args[0] == "" || args[1] == "")))
            {
                Log.Info("Usage: player_learnspell <spell> <level>");
                return;
            }

            Spell spell = SpellManager.Get(args[0]);
            if (spell == null)
            {
                Log.Error($"Spell '{args[0]}' does not exist.");
                return;
            }

            if (!int.TryParse(args[1], out int level))
            {
                Log.Error($"That spell only casts up to level {spell.GetMaxCastingLevel()}.");
                return;
            }

            spellBook.LearnSpell(spell, level, true);
        }

        private static void MagicMenuCommand(string[] args)
        {
            Game1.activeClickableMenu = new MagicMenu();
        }
    }
}
