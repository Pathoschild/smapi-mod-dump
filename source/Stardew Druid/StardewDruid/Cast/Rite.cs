/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Netcode;
using StardewDruid.Cast.Effect;
using StardewDruid.Cast.Ether;
using StardewDruid.Cast.Fates;
using StardewDruid.Cast.Mists;
using StardewDruid.Cast.Stars;
using StardewDruid.Cast.Weald;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.GameData.HomeRenovations;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using xTile.Dimensions;
using xTile.Layers;
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Cast.Weald.Wildbounty;
using static StardewDruid.Data.IconData;
using static StardewValley.Menus.CharacterCustomization;
using static StardewValley.Minigames.BoatJourney;
using static StardewValley.Minigames.CraneGame;
using static System.Net.Mime.MediaTypeNames;


namespace StardewDruid.Cast
{
    public class Rite
    {
        // -----------------------------------------------------

        public enum rites
        {
            none,

            weald,
            mists,
            stars,
            fates,
            ether

        }

        public rites castType;

        public Dictionary<rites, Journal.QuestHandle.milestones> requirement = new()
        {
            [rites.weald] = QuestHandle.milestones.weald_weapon,
            [rites.mists] = QuestHandle.milestones.mists_weapon,
            [rites.stars] = QuestHandle.milestones.stars_weapon,
            [rites.fates] = QuestHandle.milestones.fates_weapon,
            [rites.ether] = QuestHandle.milestones.ether_weapon,

        };

        public Dictionary<rites, string> displayNames = new()
        {
            [rites.weald] = "Rite of the Weald",
            [rites.mists] = "Rite of Mists",
            [rites.stars] = "Rite of the Stars",
            [rites.fates] = "Rite of the Fates",
            [rites.ether] = "Rite of Ether",

        };

        public int castLevel;

        public int castCost;

        public Vector2 castVector;

        public int castTool;

        public StardewValley.GameLocation castLocation;

        public SpawnIndex spawnIndex = new();

        public List<TemporaryAnimatedSprite> castAnimations = new();

        public int castInterval;

        public int castTimer;

        public bool castActive;

        public List<Vector2> vectorList = new();

        public Dictionary<string, List<string>> specialCasts = new();

        public Dictionary<string, Dictionary<Vector2, string>> targetCasts = new();

        public Dictionary<string, Dictionary<Vector2, int>> terrainCasts = new();

        public rites appliedBuff;

        // ----------------------------------------------------

        public enum charges
        {
            none,

            wealdCharge,
            mistsCharge,
            starsCharge,
            fatesCharge,

        }

        public Dictionary<rites, charges> riteCharges = new()
        {
            [rites.weald] = charges.wealdCharge,
            [rites.mists] = charges.mistsCharge,
            [rites.stars] = charges.starsCharge,
            [rites.fates] = charges.fatesCharge,

        };

        public Dictionary<charges, IconData.cursors> chargeCursors = new()
        {
            [charges.wealdCharge] = IconData.cursors.wealdCharge,
            [charges.mistsCharge] = IconData.cursors.mistsCharge,
            [charges.starsCharge] = IconData.cursors.starsCharge,
            [charges.fatesCharge] = IconData.cursors.fatesCharge,

        };

        public Dictionary<rites, string> chargeRequirement = new()
        {
            [rites.weald] = QuestHandle.wealdFour,
            [rites.mists] = QuestHandle.mistsFour,
            [rites.stars] = QuestHandle.starsOne,
            [rites.fates] = QuestHandle.fatesTwo,
        };

        public charges chargeType;

        public int chargeTimer;

        public int chargeCooldown;

        public bool chargeActive;

        public StardewValley.GameLocation chargeLocation;

        public List<TemporaryAnimatedSprite> chargeAnimations = new();

        // ----------------------------------------------------

        public Rite()
        {

            reset();

        }

        public void shutdown()
        {

            if (castAnimations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite sprite in castAnimations)
                {

                    sprite.Parent.temporarySprites.Remove(sprite);

                }

                castAnimations.Clear();

            }

            castActive = false;

        }

        public void reset()
        {

            castInterval = 60;

            int castFast = 5;

            if (castType == rites.mists)
            {

                castInterval = 40;

                castFast = 3;

            }

            if (castType == rites.fates)
            {

                castInterval = 120;

                castFast = 8;

            }

            if (Mod.instance.herbalData.applied.ContainsKey(HerbalData.herbals.celeri))
            {

                castInterval -= (int)(castFast * Mod.instance.herbalData.applied[HerbalData.herbals.celeri].level);

            }

            vectorList = new();

            castLevel = 0;

            castLocation = Game1.player.currentLocation;

        }

        public bool start()
        {

            rites blessing;

            int tool = Mod.instance.AttuneableWeapon();

            if (Mod.instance.Config.slotAttune)
            {

                blessing = GetSlotBlessing();

                if (blessing == rites.none)
                {

                    if (Mod.instance.CheckTrigger())
                    {
                        
                        return false;

                    }

                    if (Mod.instance.save.milestone == Journal.QuestHandle.milestones.none)
                    {

                        Mod.instance.CastMessage(Mod.instance.Config.journalButtons.ToString() + " to open Druid Journal and get started");
                    
                    }
                    else
                    {

                        Mod.instance.CastMessage("No rite attuned to slot " + (Game1.player.CurrentToolIndex + 1));

                    }

                    return false;

                }

            }
            else
            {

                if (tool == -1)
                {

                    Mod.instance.CastMessage("Rite requires a melee weapon or tool");

                    return false;

                }

                if (Mod.instance.Attunement.ContainsKey(tool))
                {

                    blessing = RequirementCheck(Mod.instance.Attunement[tool]);

                    if (blessing == rites.none)
                    {

                        if (Mod.instance.CheckTrigger())
                        {

                            return false;

                        }

                        Mod.instance.CastMessage("I'm not attuned to this artifact... perhaps the Effigy can help");

                        return false;

                    }

                }
                else
                {

                    blessing = Mod.instance.save.rite;

                }

            }

            if(blessing != castType)
            {

                shutdown();

            }

            castType = blessing;

            castTool = tool;

            GetLocation();

            CastVector();

            reset();

            castActive = true;

            return true;

        }

        public void update()
        {

            castTimer--;

            ChargeUpdate();

            if (!castActive)
            {
                
                return;

            }

            if (castTimer <= 0)
            {

                if (!(castLevel == 0 || Mod.instance.Config.riteButtons.GetState() == SButtonState.Held))
                {

                    shutdown();

                    return;

                }

                if(Game1.player.currentLocation.Name != castLocation.Name)
                {

                    GetLocation();

                }

                if (Mod.instance.Config.slotAttune)
                {

                    rites slot = GetSlotBlessing();

                    if (castType != slot)
                    {

                        if (!start())
                        {

                            shutdown();

                            return;

                        }

                    }

                }
                else
                {

                    int toolIndex = Mod.instance.AttuneableWeapon();

                    if (castTool != toolIndex)
                    {

                        if (!start())
                        {

                            shutdown();

                            return;

                        }

                    }

                }

                if (castLevel == 0)
                {

                    if (Mod.instance.CheckTrigger())
                    {

                        shutdown();

                        return;

                    }

                }

                if (castType == rites.none)
                {

                    if (Mod.instance.save.milestone == Journal.QuestHandle.milestones.none)
                    {

                        Mod.instance.CastMessage(Mod.instance.Config.journalButtons.ToString() + " to open Druid Journal and get started");
                    }
                    else
                    {

                        Mod.instance.CastMessage("Nothing happened... ");
                    }

                    shutdown();

                    return;

                }

                //if (spawnIndex.Count == 0 && !Mod.instance.eventRegister.ContainsKey("active"))
                if (!spawnIndex.cast && Mod.instance.activeEvent.Count == 0)
                {

                    Mod.instance.CastMessage("Unable to reach the otherworldly plane from this location");

                    shutdown();

                    return;

                }

                if (Game1.player.Stamina <= (Game1.player.MaxStamina / 4) || Game1.player.health <= (Game1.player.maxHealth / 3))
                {

                    Mod.instance.AutoConsume();

                    if (Game1.player.Stamina <= 16)
                    {

                        if (castLevel > 0)
                        {
                            Mod.instance.CastMessage("Not enough energy to continue rite", 3);

                        }
                        else
                        {
                            Mod.instance.CastMessage("Not enough energy to perform rite", 3);

                        }

                        shutdown();

                        return;

                    }

                }

                CastVector();

                cast();

                castTimer = castInterval;

                castLevel++;

                if (castAnimations.Count > 0)
                {

                    TemporaryAnimatedSprite decoration = castAnimations.First();

                    if (!decoration.Parent.temporarySprites.Contains(decoration))
                    {

                        castAnimations.Clear();

                    }

                    if(decoration.Parent != castLocation)
                    {

                        foreach(TemporaryAnimatedSprite sprite in castAnimations)
                        {

                            sprite.Parent.temporarySprites.Remove(sprite);

                        }

                        castAnimations.Clear();

                    }

                }

            }

            if (castAnimations.Count == 0)
            {

                castAnimations.Add(Mod.instance.iconData.DecorativeIndicator(castLocation, Game1.player.Position, Mod.instance.iconData.riteDecorations[castType], 3f, new() { interval = 2400, }));

            }
            else
            {
                
                foreach (TemporaryAnimatedSprite sprite in castAnimations)
                {

                    if (sprite.timer >= 2000)
                    {

                        sprite.reset();

                    }

                    sprite.position = Game1.player.Position - new Vector2(64, 64);

                    sprite.layerDepth = (sprite.position.Y) / 10000;

                }

            }

        }

        public void charge(EventHandle.actionButtons button = EventHandle.actionButtons.action)
        {

            if(button == EventHandle.actionButtons.none || button == EventHandle.actionButtons.rite) { return; }

            if (button == EventHandle.actionButtons.special)
            {

                if (!castActive) { return; }

                if (castType == rites.ether) { return; }

                if (!Mod.instance.questHandle.IsGiven(chargeRequirement[castType]))
                {

                    return;

                }

                if (chargeActive && chargeType != riteCharges[castType])
                {

                    ChargeShutdown();

                }
                
                ChargeSet(riteCharges[castType]);

                return;

            }

            if(button == EventHandle.actionButtons.action)
            {

                if (!chargeActive) { return; }

                if (Mod.instance.AttuneableWeapon() == -1) { return; }

                if (chargeCooldown > 0) { return; }

                float celeri = 1f;

                if (Mod.instance.herbalData.applied.ContainsKey(HerbalData.herbals.celeri))
                {

                    celeri -= 0.1f * (float)Mod.instance.herbalData.applied[HerbalData.herbals.celeri].level;

                }

                chargeTimer += 900;

                int radius = Mod.instance.eventRegister.ContainsKey("transform") ? 160 : 92;

                List<StardewValley.Monsters.Monster> checkMonsters;

                switch (chargeType)
                {

                    case charges.fatesCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {

                            if (!Mod.instance.questHandle.IsComplete(QuestHandle.fatesTwo))
                            {

                                Mod.instance.questHandle.UpdateTask(QuestHandle.fatesTwo, 1);

                            }

                            SpellHandle knockeffect = new(Game1.player, checkMonsters, 0);

                            knockeffect.type = SpellHandle.spells.explode;

                            switch (Mod.instance.randomIndex.Next(4))
                            {
                                case 0:

                                    knockeffect.added.Add(SpellHandle.effects.daze);
                                    break;

                                case 1:

                                    knockeffect.added.Add(SpellHandle.effects.mug);
                                    break;

                                case 2:

                                    knockeffect.added.Add(SpellHandle.effects.morph);
                                    break;

                                default:
                                case 3:

                                    knockeffect.added.Add(SpellHandle.effects.doom);
                                    break;
                            }

                            knockeffect.monsters = checkMonsters;

                            knockeffect.display = IconData.impacts.deathwhirl;

                            knockeffect.scheme = schemes.fates;

                            knockeffect.instant = true;

                            knockeffect.local = true;

                            Mod.instance.spellRegister.Add(knockeffect);

                            chargeCooldown = (int)(180f * celeri);

                        }

                        break;

                    case charges.starsCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {

                            int impes = 1;

                            if (Mod.instance.herbalData.applied.ContainsKey(HerbalData.herbals.impes))
                            {

                                impes = Mod.instance.herbalData.applied[HerbalData.herbals.impes].level;

                            }

                            int burst = Mod.instance.PowerLevel * 5 * impes;

                            SpellHandle knockeffect = new(Game1.player, checkMonsters, burst);

                            knockeffect.type = SpellHandle.spells.explode;

                            knockeffect.added.Add(SpellHandle.effects.knock);

                            knockeffect.monsters = checkMonsters;
                            
                            knockeffect.display = IconData.impacts.flashbang;

                            knockeffect.instant = true;

                            knockeffect.local = true;

                            Mod.instance.spellRegister.Add(knockeffect);

                            chargeCooldown = (int)(120f * celeri);

                        }

                        break;

                    case charges.mistsCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {


                            SpellHandle draineffect = new(Game1.player, checkMonsters, Mod.instance.CombatDamage());

                            draineffect.type = SpellHandle.spells.effect;

                            draineffect.added.Add(SpellHandle.effects.drain);

                            draineffect.local = true;

                            Mod.instance.spellRegister.Add(draineffect);

                            chargeCooldown = (int)(60f * celeri);

                        }

                        break;

                    default: // weald

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {

                            SpellHandle sapeffect = new(Game1.player, checkMonsters, 0);

                            sapeffect.type = SpellHandle.spells.effect;

                            sapeffect.added.Add(SpellHandle.effects.sap);

                            sapeffect.local = true;

                            Mod.instance.spellRegister.Add(sapeffect);

                            chargeCooldown = (int)(30f * celeri);

                        }

                        break;

                }

            }

        }

        public void ChargeUpdate()
        {

            if (!chargeActive)
            {

                return;

            }

            if (chargeCooldown > 0)
            {

                chargeCooldown--;

            }

            if (chargeTimer > 0)
            {

                chargeTimer--;

            }

            if (chargeTimer <= 0)
            {

                ChargeShutdown();

                return;

            }

            if(chargeLocation.Name != Game1.player.currentLocation.Name)
            {

                ChargeShutdown();

                chargeActive = true;

                chargeLocation = Game1.player.currentLocation;

            }

            if(chargeTimer % 240 == 0)
            {

                for(int s = chargeAnimations.Count - 1; s >= 0; s--)
                {

                    TemporaryAnimatedSprite sprite = chargeAnimations.ElementAt(s);

                    Game1.player.currentLocation.TemporarySprites.Remove(sprite);

                }

                chargeAnimations.Clear();

            }

            if (chargeAnimations.Count == 0)
            {

                TemporaryAnimatedSprite cursor = Mod.instance.iconData.CursorIndicator(
                    Game1.player.currentLocation,
                    Game1.player.Position, 
                    chargeCursors[chargeType], 
                    new() { interval = 5000f, scale = 3f, loops = 1, rotation = 120f, }
                    );

                cursor.Parent = Game1.player.currentLocation;

                chargeAnimations.Add(cursor);

            }
            else
            {
                
                foreach (TemporaryAnimatedSprite sprite in chargeAnimations)
                {

                    sprite.position = Game1.player.Position + new Vector2(32 - 24 * 3, 32 - 24 * 3);

                    sprite.layerDepth =  (sprite.position.Y - 32) / 10000;

                }

            }

        }

        public void ChargeShutdown()
        {

            if (chargeAnimations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite sprite in chargeAnimations)
                {

                    chargeLocation.temporarySprites.Remove(sprite);

                }

                chargeAnimations.Clear();

            }

            chargeActive = false;

        }

        public void ChargeSet(charges type)
        {

            /*switch (type)
            {

                case charges.mistsCharge:

                    if (chargeTimer > 0 && chargeType == charges.starsCharge && RequirementCheck(rites.stars) != rites.none)
                    {

                        type = charges.chaosCharge;

                    }

                    break;

                case charges.starsCharge:

                    if (chargeTimer > 0 && chargeType == charges.mistsCharge)
                    {

                        type = charges.chaosCharge;

                    }

                    break;

            }*/

            chargeActive = true;

            chargeType = type;

            chargeTimer = 1200;

            chargeLocation = Game1.player.currentLocation;

        }

        public rites RequirementCheck(rites id, bool next = false)
        {

            if((int)Mod.instance.save.milestone >= (int)requirement[id])
            {

                return id;

            }

            if (next)
            {

                while((int)id > 1)
                {

                    id = (rites)((int)id - 1);

                    if ((int)Mod.instance.save.milestone >= (int)requirement[id])
                    {

                        return id;

                    }

                }

            }

            return rites.none;

        }

        public rites GetSlotBlessing()
        {

            int num = Game1.player.CurrentToolIndex;

            /*if (Game1.player.CurrentToolIndex == 999 && Mod.instance.eventRegister.ContainsKey("transform"))
            {
                num = (Mod.instance.eventRegister["transform"] as Transform).toolIndex;

            }*/

            int real = num % 12;

            rites blessing = rites.none;

            Dictionary<int, string> slots = new()
            {
                [0] = Mod.instance.Config.slotOne,
                [1] = Mod.instance.Config.slotTwo,
                [2] = Mod.instance.Config.slotThree,
                [3] = Mod.instance.Config.slotFour,
                [4] = Mod.instance.Config.slotFive,
                [5] = Mod.instance.Config.slotSix,
                [6] = Mod.instance.Config.slotSeven,
                [7] = Mod.instance.Config.slotEight,
                [8] = Mod.instance.Config.slotNine,
                [9] = Mod.instance.Config.slotTen,
                [10] = Mod.instance.Config.slotEleven,
                [11] = Mod.instance.Config.slotTwelve,

            };

            switch (slots[real])
            {

                case "weald":

                    blessing = RequirementCheck(rites.weald);

                    break;

                case "mists":

                    blessing = RequirementCheck(rites.mists, true);

                    break;

                case "stars":

                    blessing = RequirementCheck(rites.stars, true);

                    break;

                case "fates":

                    blessing = RequirementCheck(rites.fates, true);

                    break;

                case "ether":

                    blessing = RequirementCheck(rites.ether, true);

                    break;


            }

            return blessing;

        }

        public void GetLocation()
        {
            
            castLocation = Game1.player.currentLocation;

            spawnIndex = new SpawnIndex(castLocation);

            if (!spawnIndex.cast && Mod.instance.eventRegister.ContainsKey("active"))
            {

                spawnIndex.cast = true;

            }

        }

        public void CastVector()
        {

            switch (castType)
            {

                case rites.mists:
                case rites.fates:

                    Vector2 cursorVector = GetTargetCursor(Game1.player.FacingDirection, 320);

                    castVector = ModUtility.PositionToTile(cursorVector);

                    break;

                default: // earth / stars / ether

                    castVector = Game1.player.Tile;

                    break;

            }

        }

        public static List<StardewValley.Monsters.Monster> GetMonstersAround(int direction, int distance, int radius)
        {

            Vector2 checkVector = GetTargetDirectional(direction, distance);

            List<StardewValley.Monsters.Monster> checkMonsters = ModUtility.MonsterProximity(Game1.player.currentLocation, new() { checkVector }, radius);

            return checkMonsters;

        }

        public static Vector2 GetTargetCursor(int direction, int distance = 320, int threshhold = 64)
        {

            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                
                return GetTargetDirectional(direction, distance);

            }

            Vector2 playerPosition = Game1.player.Position;

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            float vectorDistance = Vector2.Distance(playerPosition, mousePosition);

            if (vectorDistance <= threshhold + 32)
            {

                return GetTargetDirectional(direction, distance);

            }

            Vector2 diffVector = mousePosition - playerPosition;

            int vectorLimit = distance + 32;

            if (vectorDistance > vectorLimit)
            {

                diffVector *= vectorLimit;

                diffVector /= vectorDistance;

            }

            return playerPosition + diffVector;

        }

        public static Vector2 GetTargetDirectional(int direction, int distance = 320)
        {
            Vector2 vector = Game1.player.Position;

            Dictionary<int, Vector2> vectorIndex = new()
            {

                [0] = vector + new Vector2(0, -distance),// up
                [1] = vector + new Vector2(distance, 0), // right
                [2] = vector + new Vector2(0, distance),// down
                [3] = vector + new Vector2(-distance, 0), // left

            };

            return vectorIndex[direction];

        }

        public void cast()
        {

            /*Vector2 castVector = Game1.player.Tile;

            if (castLocation.terrainFeatures.ContainsKey(castVector))
            {

                if (castLocation.terrainFeatures[castVector] is StardewValley.TerrainFeatures.Grass)
                {

                    BuffEffects buffEffect = new();

                    buffEffect.Speed.Set(2);

                    string riteDisplay = displayNames[castType];

                    Buff speedBuff = new("184652",source: riteDisplay, displaySource: riteDisplay, duration:3000, displayName:"Druidic Freneticism",description:"Speed increased when casting amongst Grass", effects: buffEffect);

                    Game1.player.buffs.Apply(speedBuff);

                }

            }*/

            castCost = 0;

            switch (castType)
            {

                case rites.stars:

                    CastStars();

                    break;

                case rites.mists:

                    CastMists();

                    break;

                case rites.fates:

                    CastFates();

                    break;

                case rites.ether:

                    CastEther();

                    break;

                default:

                    CastWeald();

                    break;
            }

            ApplyCost();

        }

        public void ApplyCost()
        {

            float oldStamina = Game1.player.Stamina;

            float totalCost = castCost * (float)Mod.instance.ModDifficulty() * 0.4f;

            float staminaCost = Math.Min(castCost, oldStamina - 16);

            if (staminaCost > 0)
            {

                Game1.player.Stamina -= staminaCost;

            }

            Game1.player.checkForExhaustion(oldStamina);

            castCost = 0;

        }

        public void CastWeald()
        {

            int chargeFactor = castLevel % 4;

            int chargeLevel = (chargeFactor * 2) + 1;

            Layer backLayer = castLocation.Map.GetLayer("Back");

            Layer buildingLayer = castLocation.Map.GetLayer("Buildings");

            string locationName = castLocation.Name;

            float damageLevel = Mod.instance.CombatDamage();

            //---------------------------------------------
            // Weald Sound
            //---------------------------------------------

            if (castLevel == 0)
            {
                
                Game1.player.currentLocation.playSound("discoverMineral");

            }


            //---------------------------------------------
            // Weed destruction
            //---------------------------------------------

            if (castLocation.objects.Count() > 0 && spawnIndex.weeds)
            {

                Cast.Weald.Clearance clearance = new();

                for (int i = 0; i < 5; i++)
                {

                    List<Vector2> weedVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, i);

                    foreach (Vector2 tileVector in weedVectors)
                    {

                        if (castLocation.objects.ContainsKey(tileVector))
                        {

                            StardewValley.Object tileObject = castLocation.objects[tileVector];

                            if (tileObject.IsBreakableStone() && Mod.instance.questHandle.IsComplete(QuestHandle.wealdOne))
                            {

                                if (SpawnData.StoneIndex().Contains(tileObject.ParentSheetIndex))
                                {

                                    clearance.CastActivate(tileVector, damageLevel);

                                }

                            }
                            else if (tileObject.IsTwig() || tileObject.IsWeeds() || tileObject.QualifiedItemId == "(O)590" || tileObject.QualifiedItemId == "(O)SeedSpot")
                            {

                                clearance.CastActivate(tileVector, damageLevel);

                            }
                            else if (castLocation is MineShaft && tileObject is BreakableContainer)
                            {

                                clearance.CastActivate(tileVector, damageLevel);

                            }

                        }

                        if (castLocation.terrainFeatures.ContainsKey(tileVector))
                        {

                            if (castLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree treeFeature)
                            {
                                
                                if (treeFeature.growthStage.Value == 0 && ModUtility.NeighbourCheck(castLocation,tileVector).Count > 0)
                                {

                                    clearance.CastActivate(tileVector, damageLevel, false);

                                }

                            }

                        }

                    }

                }

            }

            //---------------------------------------------
            // Rockfall
            //---------------------------------------------

            if (castLocation is MineShaft || castLocation is VolcanoDungeon)
            {

                if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdFive))
                {

                    CastRockfall();

                    CastRockfall();

                }

                return;

            }

            //---------------------------------------------
            // Friendship
            //---------------------------------------------

            if (castLevel % 3 == 0)
            {

                if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdTwo))
                {

                    CastFriendship();

                }

            }

            //---------------------------------------------
            // Wild bounty / growth
            //---------------------------------------------


            if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdTwo))
            {

                CastWildGrowth();

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdFour))
            {

                CastCultivate();

            }


        }

        public void CastFriendship()
        {

            GameLocation location = Game1.player.currentLocation;

            Vector2 origin = Game1.player.Position;

            List<NPC> villagers = ModUtility.GetFriendsInLocation(location, true);

            float threshold = 640;

            foreach (NPC witness in villagers)
            {

                if (Mod.instance.Witnessed(ReactionData.reactions.weald,witness))
                {

                    continue;

                }

                if (Vector2.Distance(witness.Position, origin) >= threshold)
                {

                    continue;

                }

                witness.faceTowardFarmerForPeriod(3000, 4, false, Game1.player);

                Game1.player.friendshipData[witness.Name].TalkedToToday = true;

                Game1.player.changeFriendship(25, witness);

                ReactionData.ReactTo(witness, ReactionData.reactions.weald, 25);

                SpellHandle sparklesparkle = new(witness.Position, 192, IconData.impacts.nature, new()) { instant = true };

                Mod.instance.spellRegister.Add(sparklesparkle);

            }

            if (location is Farm farmLocation)
            {


                Vector2 bowl = farmLocation.GetStarterPetBowlLocation();

                if (Vector2.Distance(bowl * 64, origin) <= threshold)
                {

                    Mod.instance.virtualCan.WaterLeft = 100;

                    farmLocation.performToolAction(Mod.instance.virtualCan, (int)bowl.X+1, (int)bowl.Y);

                }

                foreach (NPC witness in location.characters)
                {

                    if (witness is Pet petPet)
                    {

                        if (Mod.instance.Witnessed(ReactionData.reactions.weald, witness))
                        {

                            continue;

                        }

                        if (Vector2.Distance(petPet.Position, origin) >= threshold)
                        {
                            continue;
                        }

                        petPet.checkAction(Game1.player, location);

                        continue;

                    }

                }

                foreach (KeyValuePair<long, FarmAnimal> pair in farmLocation.animals.Pairs)
                {

                    if (Mod.instance.Witnessed(ReactionData.reactions.weald, pair.Value.myID.ToString()))
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, origin) >= threshold)
                    {

                        continue;

                    }

                    ModUtility.PetAnimal(Game1.player, pair.Value);

                    Mod.instance.Witnessed(ReactionData.reactions.weald, pair.Value.myID.ToString(), true);
                
                }

            }

            if (location is AnimalHouse animalLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in animalLocation.animals.Pairs)
                {

                    if (Mod.instance.Witnessed(ReactionData.reactions.weald, pair.Value.myID.ToString()))
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, origin) >= threshold)
                    {

                        continue;

                    }


                    ModUtility.PetAnimal(Game1.player, pair.Value);

                    Mod.instance.Witnessed(ReactionData.reactions.weald, pair.Value.myID.ToString(), true);

                }

                for (int i = 0; i < location.map.Layers[0].LayerWidth; i++)
                {

                    for (int j = 0; j < location.map.Layers[0].LayerHeight; j++)
                    {

                        if (location.doesTileHaveProperty(i, j, "Trough", "Back") == null)
                        {
                            continue;
                        }
                        Vector2 trough = new Vector2(i, j);

                        if (!location.objects.ContainsKey(trough))
                        {
                            location.objects.Add(trough, new StardewValley.Object("178", 1));

                        }

                    }

                }

            }

        }

        public void CastWildGrowth()
        {

            // ---------------------------------------------
            // Random Effect Center Selection
            // ---------------------------------------------

            List<Vector2> centerVectors = new();

            if (!terrainCasts.ContainsKey(castLocation.Name))
            {

                terrainCasts[castLocation.Name] = new();

            }

            Vector2 playerTile = ModUtility.PositionToTile(Game1.player.Position);

            for (int i = 0; i < 4; i++)
            {

                List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(Game1.player.currentLocation,playerTile,4,true,Mod.instance.randomIndex.Next(8));

                if(castSelection.Count == 0)
                {

                    continue;

                }

                Vector2 useVector = castSelection.First();

                Vector2 sqtVector = new((int)(useVector.X - (useVector.X % 9)), (int)(useVector.Y -(useVector.Y % 9)));

                if (terrainCasts[castLocation.Name].ContainsKey(sqtVector))
                {

                    List<Microsoft.Xna.Framework.Color> colors = new()
                    {
                        Microsoft.Xna.Framework.Color.White,
                        Microsoft.Xna.Framework.Color.White,
                        Microsoft.Xna.Framework.Color.LightGreen,
                        Microsoft.Xna.Framework.Color.LightYellow,
                        Microsoft.Xna.Framework.Color.Tan,
                    };

                    Mod.instance.iconData.ImpactIndicator(castLocation, useVector * 64, IconData.impacts.glare, 1f + (Mod.instance.randomIndex.Next(5) * 0.2f), new() { alpha = 0.35f, color = colors[Mod.instance.randomIndex.Next(colors.Count)]});

                    continue;

                }

                terrainCasts[castLocation.Name][sqtVector] = 0;

                Wildbounty bounty = new(); bounty.CastActivate(castLocation, sqtVector);

                if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdThree))
                {

                    Wildgrowth growth = new(); growth.CastActivate(castLocation, sqtVector);

                }

            }

        }

        public void CastCultivate()
        {

            if (castLevel != 0) { return; }

            if (Mod.instance.eventRegister.ContainsKey("cultivate")) {


                if (Mod.instance.eventRegister["cultivate"] is Cultivate cultivate)
                {

                    if (!cultivate.eventLocked)
                    {

                        cultivate.eventAbort = true;

                    }

                }

                return; 
            
            }

            if (!castLocation.IsFarm && !castLocation.IsGreenhouse) { return; }

            Cultivate cultivateEvent = new();

            cultivateEvent.EventSetup(Game1.player.Position, "cultivate");

            cultivateEvent.EventActivate();

        }

        public void CastRockfall(bool scene = false)
        {

            Vector2 rockVector = Vector2.Zero;

            IconData.impacts display = IconData.impacts.impact;

            SpellHandle.sounds sound = SpellHandle.sounds.flameSpellHit;

            for (int i = 0; i < 3; i++)
            {

                List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(Game1.player.currentLocation, ModUtility.PositionToTile(Game1.player.Position), Mod.instance.randomIndex.Next(1, 6), true, Mod.instance.randomIndex.Next(8));

                if (castSelection.Count > 0)
                {

                    Vector2 tryVector = castSelection[Mod.instance.randomIndex.Next(castSelection.Count)];

                    if (!scene)
                    {

                        string ground = ModUtility.GroundCheck(Game1.player.currentLocation, tryVector);

                        if (ground == "ground")
                        {

                            rockVector = tryVector;

                            break;

                        }

                        if (ground == "water")
                        {

                            rockVector = tryVector;

                            display = IconData.impacts.splash;

                            sound = SpellHandle.sounds.dropItemInWater;

                            break;

                        }
                    }
                    else
                    {

                        rockVector = tryVector;

                        break;

                    }
                    
                }

            }

            if(rockVector == Vector2.Zero) { return; }

            float damage = -1f;

            if (Mod.instance.questHandle.IsComplete(QuestHandle.wealdFive))
            {
                
                damage = Mod.instance.CombatDamage() * 0.5f;

            }

            SpellHandle rockSpell = new(Game1.player, rockVector * 64, 192, damage);

            rockSpell.display = display;

            rockSpell.type = SpellHandle.spells.orbital;

            rockSpell.missile = missiles.rockfall;

            rockSpell.projectile = 2;

            rockSpell.terrain = 2;

            rockSpell.sound = sound;

            rockSpell.added = new() { SpellHandle.effects.stone,};

            Mod.instance.spellRegister.Add(rockSpell);

            if (!scene)
            {
                castCost += 2;

            }

        }

        public void CastMists()
        {

            //---------------------------------------------
            // Mists
            //---------------------------------------------

            if (castLevel == 0)
            {

                Game1.player.currentLocation.playSound("thunder_small");

            }

            //---------------------------------------------
            // Sunder
            //---------------------------------------------

            if (Mod.instance.questHandle.IsGiven(QuestHandle.mistsOne))
            {

                if(castLevel % 2 == 0)
                {

                    CastSunder();

                }

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.mistsTwo))
            {

                Artifice artifice = new();

                artifice.CastActivate(castVector);

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.mistsThree))
            {

                CastFishspot();

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.mistsFour))
            {

                CastSmite(new() { castVector*64,Game1.player.Position}, Mod.instance.CombatDamage());

            }

            if (Mod.instance.questHandle.IsComplete(QuestHandle.questEffigy))
            {

                CastWisps();

            }

            if (castLevel % 2 == 0)
            {
                
                Mod.instance.iconData.CursorIndicator(castLocation, castVector * 64, IconData.cursors.mists, new() { interval = 1200f, alpha = 1f, scale = 2f, fade = 0.0008f});

            }
            

        }

        public void CastSunder()
        {

            int sundered = 0;

            //bool extraDebris = Mod.instance.questHandle.IsComplete(QuestHandle.mistsFour);

            if (castLocation.resourceClumps.Count > 0)
            {

                for (int r = castLocation.resourceClumps.Count -1; r >= 0; r--)
                {

                    ResourceClump resourceClump = castLocation.resourceClumps[r];

                    if(resourceClump is GiantCrop)
                    {

                        continue;

                    }

                    int tryCost = 32 - Game1.player.ForagingLevel * 3;

                    int cost = tryCost < 8 ? 8 : tryCost;

                    if (Vector2.Distance(resourceClump.Tile, castVector) <= 4)
                    {

                        if (Mod.instance.questHandle.IsComplete(QuestHandle.mistsFour))
                        {

                            resourceClump.destroy(Mod.instance.virtualAxe, castLocation, resourceClump.Tile);
                        
                        }

                        Mod.instance.spellRegister.Add(new(resourceClump.Tile * 64 + new Vector2(32), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt, display = IconData.impacts.puff, });

                        resourceClump.destroy(Mod.instance.virtualAxe, castLocation, resourceClump.Tile);

                        resourceClump.health.Set(1f);

                        resourceClump.performToolAction(Mod.instance.virtualPick, 1, resourceClump.Tile);

                        resourceClump.NeedsUpdate = false;

                        castLocation._activeTerrainFeatures.Remove(resourceClump);

                        castLocation.resourceClumps.Remove(resourceClump);

                        castCost += cost;

                        sundered++;

                    }

                }

            }

            if (sundered > 0)
            {

                if (!Mod.instance.questHandle.IsComplete(QuestHandle.mistsOne))
                {

                    Mod.instance.questHandle.UpdateTask(QuestHandle.mistsOne, sundered);

                }
            
            }


        }

        public void CastFishspot()
        {

            // ---------------------------------------------
            // Water effect
            // ---------------------------------------------

            //if (progressLevel >= 11 && chargeLevel == 1)
            if((castLevel % 4) != 0)
            {
            
                return; 
            
            }

            if (spawnIndex.fishspot)
            {

                if (ModUtility.WaterCheck(castLocation, castVector))
                {

                    int tryCost = 32 - Game1.player.FishingLevel * 3;

                    castCost += tryCost < 8 ? 8 : tryCost;

                    Fishspot fishspotEvent = new();

                    fishspotEvent.EventSetup(castVector*64, "fishspot");

                    fishspotEvent.EventActivate();
                }

            }

            if (castLocation is VolcanoDungeon volcanoLocation)
            {
                int tileX = (int)castVector.X;
                int tileY = (int)castVector.Y;

                if (volcanoLocation.waterTiles[tileX, tileY] && !volcanoLocation.cooledLavaTiles.ContainsKey(castVector))
                {
                    int waterRadius = Math.Min(5, Mod.instance.PowerLevel);

                    for (int i = 0; i < waterRadius + 1; i++)
                    {

                        List<Vector2> radialVectors = ModUtility.GetTilesWithinRadius(volcanoLocation, castVector, i);

                        foreach (Vector2 radialVector in radialVectors)
                        {
                            int radX = (int)radialVector.X;
                            int radY = (int)radialVector.Y;

                            if (volcanoLocation.waterTiles[radX, radY] && !volcanoLocation.cooledLavaTiles.ContainsKey(radialVector))
                            {

                                volcanoLocation.CoolLava(radX, radY);

                                volcanoLocation.UpdateLavaNeighbor(radX, radY);

                            }

                        }

                    }

                    List<Vector2> fourthVectors = ModUtility.GetTilesWithinRadius(volcanoLocation, castVector, waterRadius + 1);

                    foreach (Vector2 fourthVector in fourthVectors)
                    {
                        int fourX = (int)fourthVector.X;
                        int fourY = (int)fourthVector.Y;

                        volcanoLocation.UpdateLavaNeighbor(fourX, fourY);

                    }

                    //Mod.instance.iconData.AnimateBolt(volcanoLocation, castVector * 64 + new Vector2(32));
                    Mod.instance.spellRegister.Add(new(castVector * 64 + new Vector2(32), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt });

                }

            }

        }

        public void CastSmite(List<Vector2> origins, float damage)
        {

            // ---------------------------------------------
            // Monster iteration
            // ---------------------------------------------

            int smiteCount = 0;

            int smiteLimit = Mod.instance.PowerLevel;

            List<StardewValley.Monsters.Monster> victims = ModUtility.MonsterProximity(Game1.player.currentLocation,origins,256,true);

            Dictionary<Vector2, List<StardewValley.Monsters.Monster>> victimSets = new();

            foreach (StardewValley.Monsters.Monster victim in victims)
            {

                if (victimSets.Count == 0)
                {

                    victimSets.Add(victim.Position, new() { victim });

                    continue;

                }

                bool added = false;

                foreach (KeyValuePair<Vector2, List<StardewValley.Monsters.Monster>> victimSet in victimSets)
                {

                    if (Vector2.Distance(victim.Position, victimSet.Key) <= 192)
                    {

                        victimSet.Value.Add(victim);

                        added = true;

                        break;

                    }

                }

                if (!added)
                {

                    victimSets.Add(victim.Position, new() { victim });

                }

            }


            foreach (KeyValuePair<Vector2,List<StardewValley.Monsters.Monster>> victimSet in victimSets)
            {

                if (smiteCount == smiteLimit)
                {
                    break;
                }

                List<float> crits = Mod.instance.CombatCritical();

                if (!Mod.instance.questHandle.IsComplete(QuestHandle.mistsFour))
                {

                    Mod.instance.questHandle.UpdateTask(QuestHandle.mistsFour, 1);

                }
                else
                {
                    crits[0] += 0.2f;
                }

                SpellHandle bolt = new(Game1.player, victimSet.Value, damage);

                bolt.type = SpellHandle.spells.bolt;

                bolt.critical = crits[0];

                bolt.criticalModifier = crits[1];

                bolt.added = new() { effects.push };

                Mod.instance.spellRegister.Add(bolt);

                smiteCount++;

            }


            if (smiteCount > 0)
            {
                
                int tryCost = 12 - Game1.player.CombatLevel / 2;

                castCost += tryCost < 6 ? 6 : tryCost;

            }

            // ---------------------------------------------
            // Villager iteration
            // ---------------------------------------------

            if (castLevel % 3 != 0 || Mod.instance.activeEvent.Count > 0)
            {

                return;

            }

            List<NPC> villagers = ModUtility.GetFriendsInLocation(Game1.player.currentLocation, true);

            float threshold = 640;

            foreach (NPC witness in villagers)
            {

                if (Mod.instance.Witnessed(ReactionData.reactions.mists, witness))
                {

                    continue;

                }

                if (Vector2.Distance(witness.Position, castVector*64) >= threshold)
                {

                    continue;

                }

                Microsoft.Xna.Framework.Rectangle box = witness.GetBoundingBox();

                SpellHandle bolt = new(new(box.Center.X, box.Top), 192, impacts.death, new());

                bolt.type = SpellHandle.spells.bolt;

                bolt.display = IconData.impacts.puff;

                Mod.instance.spellRegister.Add(bolt);

                witness.faceTowardFarmerForPeriod(3000, 4, false, Game1.player);

                Game1.player.changeFriendship(-10, witness);

                ReactionData.ReactTo(witness, ReactionData.reactions.mists, -10);

            }

        }

        public void CastWisps()
        {

            if (castLevel != 0)
            {

                return;

            }

            if (Mod.instance.eventRegister.ContainsKey("wisps"))
            {

                if (Mod.instance.eventRegister["wisps"] is Wisps wispEvent)
                {

                    if (wispEvent.eventLocked && Vector2.Distance(wispEvent.origin,Game1.player.Position) <= 960f)
                    {

                        return;

                    }

                    if (wispEvent.AttemptReset())
                    {

                        return;

                    }

                    wispEvent.EventRemove();

                }

            }

            Wisps wispNew = new();

            wispNew.EventSetup(Game1.player.Position, "wisps");

            wispNew.EventActivate();

        }

        public void CastStars()
        {

            if (Mod.instance.questHandle.IsGiven(QuestHandle.starsOne))
            {

                CastMeteors();

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.starsTwo))
            {

                CastBlackhole();

            }

            //CastTransform();

        }

        public void CastMeteors()
        {

            if (!castLocation.IsOutdoors)
            {

                if(castLocation is not MineShaft && castLocation is not VolcanoDungeon && castLocation is not Vault)
                {

                    return;

                }

            }

            if(castLocation.IsFarm && Game1.player.CurrentTool is not MeleeWeapon)
            {

                return;

            }

            List<Vector2> meteorVectors = new();

            int difficulty = Mod.instance.Config.meteorBehaviour;

            int meteorLimit = 1;

            bool comet = false;

            if (Mod.instance.questHandle.IsComplete(QuestHandle.starsOne) && Mod.instance.randomIndex.Next(2) == 0)
            {

                meteorLimit = 2;

            }

            if(castLevel % 4 == 0 || vectorList.Count < meteorLimit)
            {

                if(difficulty == 5)
                {
                    
                    vectorList = new();

                    List<Vector2> innerTiles = ModUtility.GetTilesWithinRadius(castLocation, Vector2.Zero, 3, false);

                    for (int iv = 0; iv < 3; iv++)
                    {
                        
                        vectorList.Add(innerTiles[Mod.instance.randomIndex.Next(innerTiles.Count)]);
                        
                    }

                    List<Vector2> outerTiles = ModUtility.GetTilesWithinRadius(castLocation, Vector2.Zero, 4, false);

                    for (int ov = 0; ov < 3; ov++)
                    {
                        
                        vectorList.Add(outerTiles[Mod.instance.randomIndex.Next(outerTiles.Count)]);

                    }

                }
                else
                {
                    vectorList = new()
                    {
                        new Vector2(3,-4),
                        new Vector2(-3,-4),
                        new Vector2(5,0),
                        new Vector2(-5,0),
                        new Vector2(3,4),
                        new Vector2(-3,4),
                    };

                }

            }

            if (Mod.instance.questHandle.IsComplete(QuestHandle.starsTwo))
            {

                if (Mod.instance.eventRegister.ContainsKey("blackhole"))
                {

                    if (Mod.instance.eventRegister["blackhole"] is Cast.Stars.Blackhole gravityEffect)
                    {

                        if (gravityEffect.blackhole)
                        {
                            
                            if (!gravityEffect.meteor)
                            {

                                gravityEffect.meteor = true;

                                meteorVectors.Add(ModUtility.PositionToTile(gravityEffect.target));

                                comet = true;

                                meteorLimit = 1;

                            }
                            else
                            {

                                return;

                            }

                        }

                    }

                }

            }

            if (difficulty == 1 || difficulty == 2)
            {

                foreach (NPC nonPlayableCharacter in castLocation.characters)
                {

                    if (meteorVectors.Count >= meteorLimit)
                    {

                        break;

                    }

                    if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                    {

                        Vector2 monsterVector = monsterCharacter.Tile;

                        if(meteorVectors.Count > 0)
                        {

                            if(Vector2.Distance(monsterVector,meteorVectors.First()) < 4)
                            {
                                
                                continue;
                            
                            }

                        }

                        if (Vector2.Distance(castVector, monsterVector) > 6)
                        {

                            continue;

                        }

                        meteorVectors.Add(monsterVector);

                    }

                }

            }

            if((difficulty == 1 || difficulty == 3) && (castLocation is MineShaft || castLocation is VolcanoDungeon))
            {

                for (int i = 2; i < 6; i++)
                {

                    if (meteorVectors.Count >= meteorLimit)
                    {

                        break;

                    }

                    List<Vector2> objectVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, i);

                    foreach (Vector2 objectVector in objectVectors)
                    {

                        if (meteorVectors.Count >= meteorLimit)
                        {

                            break;

                        }

                        if (castLocation.objects.ContainsKey(objectVector))
                        {

                            StardewValley.Object targetObject = castLocation.objects[objectVector];

                            if (targetObject.Name == "Stone")
                            {

                                if (meteorVectors.Count > 0)
                                {

                                    if (Vector2.Distance(objectVector, meteorVectors.First()) < 5)
                                    {

                                        continue;

                                    }

                                }

                                meteorVectors.Add(objectVector);

                            }

                        }

                    }

                }

            }

            if(meteorVectors.Count > 0)
            {

                foreach(Vector2 meteorVector in meteorVectors)
                {

                    for(int i = vectorList.Count - 1; i >= 0; i--)
                    {

                        Vector2 tryVector = castVector + vectorList[i];

                        if (Vector2.Distance(meteorVector, tryVector) <= 3)
                        {

                            vectorList.RemoveAt(i);

                            break;

                        }

                    }

                }

            }

            if(meteorVectors.Count < meteorLimit)
            {

                for(int i = 0; i < meteorLimit - meteorVectors.Count; i++)
                {
                    
                    Vector2 randomVector = vectorList[Mod.instance.randomIndex.Next(vectorList.Count)];

                    string groundCheck = ModUtility.GroundCheck(castLocation, castVector + randomVector);

                    if(groundCheck == "water" || groundCheck == "ground")
                    {

                        vectorList.Remove(randomVector);

                        meteorVectors.Add(castVector + randomVector);

                        break;

                    }

                }

            }

            if(meteorVectors.Count == 0)
            {

                return;

            }

            float damage = Mod.instance.CombatDamage() * 0.75f;

            int extra = 0;

            switch (difficulty)
            {
                case 2:
                case 3:

                    damage *= 1.1f;

                    break;

                case 4:

                    damage *= 1.25f;

                    if (Mod.instance.randomIndex.Next(3) == 0) { extra++; }

                    break;

                case 5:

                    damage *= 1.6f;

                    if(Mod.instance.randomIndex.Next(2) == 0) { extra++; }

                    break;

            }

            foreach (Vector2 meteorVector in meteorVectors)
            {
                
                if (!Mod.instance.questHandle.IsComplete(QuestHandle.starsOne))
                {

                    List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(castLocation, new() { meteorVector * 64, }, 224, true);

                    for (int i = monsters.Count - 1; i >= 0; i--)
                    {

                        Mod.instance.questHandle.UpdateTask(QuestHandle.starsOne, 1);

                    }

                }

                int radius = 3 + extra;

                SpellHandle.sounds sound = sounds.flameSpellHit;

                int scale = radius;

                Vector2 meteorTarget = meteorVector * 64;

                if (comet)
                {

                    radius = 8;

                    scale = 5;

                    sound = sounds.explosion;

                    damage *= 4;

                }

                SpellHandle meteor = new(Game1.player, meteorTarget, radius * 64, damage);

                meteor.type = spells.orbital;

                meteor.missile = missiles.meteor;

                if (damage > 0)
                {

                    meteor.indicator = IconData.cursors.stars;

                    meteor.display = IconData.impacts.bomb;

                    meteor.projectile = scale;

                    meteor.sound = sound;

                    meteor.environment = radius;

                    meteor.power = 3;

                }

                switch (ModUtility.GroundCheck(castLocation, meteorVector))
                {

                    case "water":

                        meteor.display = IconData.impacts.splash;

                        meteor.sound = sounds.dropItemInWater;

                        break;

                }

                Mod.instance.spellRegister.Add(meteor);

                int tryCost = 12 - Game1.player.CombatLevel;

                castCost += tryCost < 5 ? 5 : tryCost;

            }

        }

        public void CastBlackhole()
        {

            if (castLevel != 0)
            {

                return;

            }

            if (Mod.instance.eventRegister.ContainsKey("blackhole"))
            {

                if (Mod.instance.eventRegister["blackhole"] is Cast.Stars.Blackhole blackholeEvent)
                {

                    if (!blackholeEvent.eventLocked && Vector2.Distance(blackholeEvent.origin, Game1.player.Position) <= 960f)
                    {

                        return;

                    }

                    if (blackholeEvent.AttemptReset())
                    {

                        return;

                    }

                    blackholeEvent.EventRemove();

                }

                Mod.instance.eventRegister.Remove("blackhole");

            }


            Vector2 blackholeVector = GetTargetCursor(Game1.player.FacingDirection, 384);

            Cast.Stars.Blackhole blackholeNew = new();

            blackholeNew.GravitySetup(Game1.player.Position, "blackhole", blackholeVector);

            blackholeNew.EventActivate();

        }

        public void CastFates()
        {

            CastWhisk();

            if (Mod.instance.questHandle.IsGiven(Journal.QuestHandle.fatesThree))
            {

                CastTricks();

            }

            if (Mod.instance.questHandle.IsGiven(Journal.QuestHandle.fatesFour))
            {

                CastEnchant();

            }

        }

        public void CastWhisk()
        {

            // ---------------------------------------------
            // Whisk
            // ---------------------------------------------

            if (castLevel != 0)
            {

                return;

            }

            if (Mod.instance.eventRegister.ContainsKey("transform"))
            {

                return;

            };

            if (Mod.instance.eventRegister.ContainsKey("whisk"))
            {

                if (Mod.instance.eventRegister["whisk"] is Whisk whiskEvent)
                {

                    return;

                }

            }

            int whiskRange = 18;

            if (Mod.instance.questHandle.IsComplete(QuestHandle.fatesOne))
            {

                whiskRange += 6;

            }

            Vector2 farVector = GetTargetCursor(Game1.player.FacingDirection, whiskRange * 64);

            Vector2 nearVector = GetTargetCursor(Game1.player.FacingDirection, 6 * 64);

            List<Vector2> whiskTiles = ModUtility.GetTilesBetweenPositions(castLocation, farVector, nearVector);

            for (int i = whiskTiles.Count - 1; i >= 0; i--)
            {

                if (ModUtility.TileAccessibility(castLocation, whiskTiles[i]) != 0)
                {

                    continue;

                }

                Whisk newWhisk = new();

                newWhisk.EventSetup(whiskTiles[i] * 64, "whisk");

                castCost += 12;

                break;

            }

            if (Mod.instance.questHandle.IsGiven(QuestHandle.fatesTwo))
            {

                List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(castLocation, new() { farVector }, 320, true);

                if (monsters.Count > 0)
                {

                    if (!Mod.instance.eventRegister.ContainsKey("whisk"))
                    {
                        
                        Whisk newWhisk = new();

                        newWhisk.EventSetup(Game1.player.Position, "whisk");

                        newWhisk.whiskreturn = true;

                    }

                    Curse curseEffect;

                    if (!Mod.instance.eventRegister.ContainsKey("curse"))
                    {

                        curseEffect = new();

                        curseEffect.eventId = "curse";

                        curseEffect.EventActivate();

                    }
                    else
                    {

                        curseEffect = Mod.instance.eventRegister["curse"] as Curse;

                    }

                    foreach (StardewValley.Monsters.Monster monster in monsters)
                    {

                        if (!Mod.instance.questHandle.IsComplete(QuestHandle.fatesTwo))
                        {

                            Mod.instance.questHandle.UpdateTask(QuestHandle.fatesTwo, 1);

                        }

                        switch (Mod.instance.randomIndex.Next(4))
                        {
                            case 0:

                                curseEffect.AddTarget(castLocation, monster, SpellHandle.effects.daze);
  
                                break;

                            case 1:
                                curseEffect.AddTarget(castLocation, monster, SpellHandle.effects.mug);

                                break;

                            case 2:
                                curseEffect.AddTarget(castLocation, monster, SpellHandle.effects.morph);

                                break;

                            default:
                            case 3:
                                curseEffect.AddTarget(castLocation, monster, SpellHandle.effects.doom);

                                break;
                        }

                        castCost += 12;

                    }
                
                }

            }

        }

        public void CastTricks()
        {

            GameLocation location = Game1.player.currentLocation;

            List<NPC> villagers = ModUtility.GetFriendsInLocation(location, true);

            Vector2 castAt = GetTargetCursor(Game1.player.FacingDirection, 12 * 64);

            float threshold = 640;

            foreach (NPC witness in villagers)
            {

                if (Mod.instance.Witnessed(ReactionData.reactions.fates, witness))
                {

                    continue;

                }

                if (Vector2.Distance(witness.Position, castAt) >= threshold)
                {

                    continue;

                }

                // Witness behaviour
                if (!Mod.instance.questHandle.IsComplete(QuestHandle.fatesThree))
                {

                    Mod.instance.questHandle.UpdateTask(QuestHandle.fatesThree, 1);

                }

                witness.faceTowardFarmerForPeriod(3000, 4, false, Game1.player);

                witness.doEmote(8);

                Mod.instance.iconData.CursorIndicator(location, witness.Position + new Vector2(32), cursors.fates, new());

                // Trick reaction

                int roster = 3;

                if (Mod.instance.questHandle.IsComplete(QuestHandle.fatesThree))
                {

                    roster++;

                }

                int display = Mod.instance.randomIndex.Next(roster);

                int friendship = Mod.instance.randomIndex.Next(0, 5) * 25 - 25;

                Game1.player.changeFriendship(friendship, witness);

                Game1.player.friendshipData[witness.Name].TalkedToToday = true;

                ReactionData.ReactTo(witness, ReactionData.reactions.fates, friendship, new() { display, });

                // Trick performance

                if (display == 3)
                {

                    Levitate levitation = new(witness);

                    levitation.EventActivate();

                }
                else
                {

                    SpellHandle trick = new(witness.Position, 192, IconData.impacts.nature, new()) { instant = true };

                    trick.type = spells.trick;

                    trick.projectile = display;

                    Mod.instance.spellRegister.Add(trick);

                }

            }

        }

        public void CastEnchant()
        {

            if (castLevel != 0) { return; }

            if (Mod.instance.eventRegister.ContainsKey("enchant"))
            {


                if (Mod.instance.eventRegister["enchant"] is Enchant enchantment)
                {

                    if (!enchantment.eventLocked)
                    {

                        enchantment.eventAbort = true;

                    }

                }

                return;

            }

            if (!castLocation.IsFarm && !castLocation.IsGreenhouse && castLocation is not AnimalHouse && castLocation is not Shed) { return; }

            Enchant enchantmentEvent = new();

            enchantmentEvent.EventSetup(Game1.player.Position, "enchant");

            enchantmentEvent.EventActivate();

        }

        public void CastEther()
        {

            if (castLevel == 0)
            {

                CastTransform();

            }

            

        }

        public void CastTransform()
        {


            if (Mod.instance.eventRegister.ContainsKey("transform"))
            {

                if(Mod.instance.eventRegister["transform"] is Cast.Ether.Transform transformEvent)
                {
                    
                    if (transformEvent.AttemptReset())
                    {

                        return;

                    }

                    transformEvent.EventRemove();

                }

                Mod.instance.eventRegister.Remove("transform");

                return;

            }

            Cast.Ether.Transform transform = new();

            transform.leftActive = true;

            if (Mod.instance.questHandle.IsGiven(QuestHandle.etherTwo))
            {

                transform.rightActive = true;

            }

            transform.EventActivate();

        }


        public void CreateTreasure()
        {

            if (!Mod.instance.questHandle.IsGiven(QuestHandle.etherFour))
            {

                return;

            }

            if (Mod.instance.eventRegister.ContainsKey("crate"))
            {

                return;

            }

            if (!spawnIndex.anywhere && spawnIndex.locale != Game1.player.currentLocation.Name)
            {

                spawnIndex = new(Game1.player.currentLocation);

            }

            if (!spawnIndex.crate)
            {

                return;

            }

            if (!specialCasts.ContainsKey(castLocation.Name))
            {

                specialCasts[castLocation.Name] = new();

            }

            if (specialCasts[castLocation.Name].Contains("crate"))
            {

                return;

            }

            int layerWidth = Game1.player.currentLocation.map.Layers[0].LayerWidth;

            int layerHeight = Game1.player.currentLocation.map.Layers[0].LayerHeight;

            Crate treasure;

            for (int i = 0; i < 10; i++)
            {

                int X = Mod.instance.randomIndex.Next(3, layerWidth - 3);

                int Y = Mod.instance.randomIndex.Next(3, layerHeight - 3);

                Vector2 treasureVector = new Vector2(X, Y);

                if (ModUtility.NeighbourCheck(Game1.player.currentLocation, treasureVector, 0, 0).Count > 0)
                {

                    continue;

                }

                string treasureTerrain = ModUtility.GroundCheck(Game1.player.currentLocation, treasureVector);

                switch (treasureTerrain)
                {

                    case "ground":

                        treasure = new Crate();

                        treasure.EventSetup(treasureVector*64, "crate", true);

                        treasure.crateThief = Mod.instance.randomIndex.Next(2) == 0;

                        treasure.crateTerrain = 1;

                        treasure.location = Game1.player.currentLocation;

                        return;

                    case "water":

                        treasure = new Crate();

                        treasure.EventSetup(treasureVector * 64, "crate", true);

                        treasure.crateTerrain = 2;

                        treasure.location = Game1.player.currentLocation;

                        return;

                }

            }
            
        }

        public void RiteBuff()
        {

            int toolIndex = Mod.instance.AttuneableWeapon();

            if (toolIndex == -1)
            {

                RemoveBuff();

                return;

            }

            rites blessing = Mod.instance.save.rite;

            if (Mod.instance.save.milestone < Journal.QuestHandle.milestones.effigy)
            {

                RemoveBuff();

                return;

            }
            else if (Mod.instance.Config.slotAttune)
            {

                blessing = GetSlotBlessing();

            }
            else
            {

                if (Mod.instance.Attunement.ContainsKey(toolIndex))
                {

                    blessing = RequirementCheck(Mod.instance.Attunement[toolIndex]);

                }

            }

            if (blessing == rites.none)
            {

                RemoveBuff();

                return;

            }

            if(appliedBuff == blessing)
            {

                if (Game1.player.buffs.IsApplied("184651"))
                {
                    return;

                }

            }

            appliedBuff = blessing;

            Buff riteBuff = new(
                "184651", 
                source: "Stardew Druid", 
                displaySource: "Stardew Druid", 
                duration: Buff.ENDLESS, 
                iconTexture:Mod.instance.iconData.displayTexture, 
                iconSheetIndex: Convert.ToInt32(blessing)-1, 
                displayName: displayNames[blessing], 
                description: "Actively selected rite"
                );

            Game1.player.buffs.Apply(riteBuff);

        }

        public void RemoveBuff()
        {

            if (Game1.player.buffs.IsApplied("184651"))
            {

                Game1.player.buffs.Remove("184651");

            }

        }

    }

}