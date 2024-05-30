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
using Netcode;
using StardewDruid.Cast.Effect;
using StardewDruid.Cast.Mists;
using StardewDruid.Cast.Weald;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.GameData.HomeRenovations;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
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

        public Dictionary<string, bool> spawnIndex = new();

        public List<TemporaryAnimatedSprite> castAnimations = new();

        public int castInterval;

        public int castTimer;

        public bool castActive;

        public List<Vector2> vectorList = new();

        public Dictionary<rites, List<string>> witnesses = new();

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
            etherCharge,
            chaosCharge,
            shadeCharge

        }

        public Dictionary<rites, charges> riteCharges = new()
        {
            [rites.weald] = charges.wealdCharge,
            [rites.mists] = charges.mistsCharge,
            [rites.stars] = charges.starsCharge,
            [rites.fates] = charges.fatesCharge,
            [rites.ether] = charges.etherCharge,

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
                if (spawnIndex.Count == 0 && Mod.instance.activeEvent.Count == 0)
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

        public void charge(string button)
        {

            if(button == "none") { return; }

            if (button == "Special")
            {

                if (!castActive) { return; }

                if (castType == rites.ether) { return; }

                if (!ChargeRequirement())
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

            if(button == "Action")
            {

                if (!chargeActive) { return; }

                if (Mod.instance.AttuneableWeapon() == -1) { return; }

                if (chargeCooldown > 0) { return; }

                chargeTimer += 900;

                /*if (Game1.player.Stamina <= (Game1.player.MaxStamina / 4) || Game1.player.health <= (Game1.player.maxHealth / 3))
                {
                    
                    Mod.instance.AutoConsume();

                }*/

                int radius = Mod.instance.eventRegister.ContainsKey("transform") ? 160 : 92;

                List<StardewValley.Monsters.Monster> checkMonsters;

                switch (chargeType)
                {

                    /*case charges.chaosCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, 256, 192);

                        if (checkMonsters.Count > 0)
                        {

                            SpellHandle chaoseffect = new(Game1.player, checkMonsters, Mod.instance.CombatDamage() / 2);

                            chaoseffect.type = SpellHandle.spells.chaos;

                            chaoseffect.display = IconData.impacts.flashbang;

                            Mod.instance.spellRegister.Add(chaoseffect);

                        }

                        chargeCooldown = 30;

                        break;*/

                    case charges.fatesCharge:

                        /*if (!Mod.instance.eventRegister.ContainsKey("shield"))
                        {

                            checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                            if (checkMonsters.Count > 0)
                            {

                                ShieldEvent shieldEvent = new(Game1.player.Position);

                                shieldEvent.EventTrigger();

                                Microsoft.Xna.Framework.Rectangle cursorRect = Mod.instance.iconData.CursorRect(IconData.cursors.shield);

                                TemporaryAnimatedSprite cursorAnimation = new(0, 300, 1, 1, Game1.player.Position - new Vector2(32,32), false, false)
                                {

                                    sourceRect = cursorRect,

                                    sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                                    texture = Mod.instance.iconData.cursorTexture,

                                    scale = 4f,

                                    layerDepth = 0.001f,

                                    alpha = 0.25f,

                                };

                                Game1.player.currentLocation.temporarySprites.Add(cursorAnimation);

                            }

                        }*/

                        break;

                    case charges.starsCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {

                            SpellHandle knockeffect = new(Game1.player, checkMonsters, 0);

                            knockeffect.type = SpellHandle.spells.explode;

                            knockeffect.added.Add(SpellHandle.effects.knock);

                            knockeffect.monsters = checkMonsters;
                            
                            knockeffect.display = IconData.impacts.flashbang;

                            knockeffect.instant = true;

                            knockeffect.local = true;

                            Mod.instance.spellRegister.Add(knockeffect);

                            chargeCooldown = 120;

                        }

                        break;

                    case charges.mistsCharge:

                        checkMonsters = GetMonstersAround(Game1.player.FacingDirection, radius, radius);

                        if (checkMonsters.Count > 0)
                        {

                            Veil veil = new();

                            veil.CastActivate();

                            chargeCooldown = 60;

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

                            chargeCooldown = 30;

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

            if (chargeAnimations.Count == 0)
            {

                TemporaryAnimatedSprite cursor = Mod.instance.iconData.CursorIndicator(
                    Game1.player.currentLocation, 
                    Game1.player.Position, 
                    (IconData.cursors)Enum.Parse(typeof(IconData.cursors),chargeType.ToString()), 
                    new() { interval = 4000f, scale = 3.5f, loops = 1, rotation = 120f, }
                    );

                cursor.Parent = Game1.player.currentLocation;

                chargeAnimations.Add(cursor);

            }
            else
            {
                foreach (TemporaryAnimatedSprite sprite in chargeAnimations)
                {

                    if (chargeTimer % 120 == 0 || sprite.timer >= 2000f)
                    {

                        sprite.reset();

                    }

                    sprite.position = Game1.player.Position - new Vector2(24, 24);

                    sprite.layerDepth =  (sprite.position.Y - 32) / 10000;

                }

            }

        }

        public bool ChargeRequirement()
        {

            switch (castType)
            {

                case rites.weald:

                    if (Mod.instance.questHandle.IsGiven(QuestHandle.wealdFive))
                    {

                        return true;

                    }

                    break;

                case rites.mists:

                    if (Mod.instance.questHandle.IsGiven(QuestHandle.mistsFour))
                    {

                        return true;

                    }

                    break;

            }

            return false;

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

        public bool Witnessed(rites type, NPC witness)
        {

            if (!witnesses.ContainsKey(type))
            {

                witnesses[type] = new()
                {
                    witness.Name,

                };

                return false;

            }

            if (!witnesses[type].Contains(witness.Name))
            {

                witnesses[type].Add(witness.Name);

                return false;

            }

            return true;

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

            spawnIndex = SpawnData.SpawnIndex(castLocation);

            if (spawnIndex.Count == 0 && Mod.instance.eventRegister.ContainsKey("active"))
            {

                spawnIndex = SpawnData.SpawnTemplate();

            }

        }

        public void CastVector()
        {

            switch (castType)
            {

                case rites.mists:
                case rites.fates:

                    List<int> targetList = GetTargetCursor(Game1.player.Tile, Game1.player.FacingDirection, 5);

                    castVector = new(targetList[1], targetList[2]);

                    break;

                default: // earth / stars / ether

                    castVector = Game1.player.Tile;

                    break;

            }

        }

        public static List<StardewValley.Monsters.Monster> GetMonstersAround(int direction, int distance, int radius)
        {

            List<int> checkVectors = GetTargetDirectional(Game1.player.Tile, direction, (int)(distance/64));

            Vector2 checkVector = new(checkVectors[1]*64, checkVectors[2]*64);

            List<StardewValley.Monsters.Monster> checkMonsters = ModUtility.MonsterProximity(Game1.player.currentLocation, new() { checkVector }, radius);

            return checkMonsters;

        }

        public static List<int> GetTargetCursor(Vector2 vector, int direction, int distance = 5, int threshhold = 1)
        {

            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                return GetTargetDirectional(vector, direction, distance);

            }

            Vector2 playerPosition = Game1.player.Position;

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            float vectorDistance = Vector2.Distance(playerPosition, mousePosition);

            if (vectorDistance <= (threshhold * 64) + 32)
            {

                return GetTargetDirectional(vector, direction, distance);

            }

            Vector2 macroVector = mousePosition - playerPosition;

            int vectorLimit = (distance * 64) + 32;

            if (vectorDistance > vectorLimit)
            {

                float adjustmentRatio = vectorLimit / vectorDistance;

                macroVector *= adjustmentRatio;

            }

            int microX = Convert.ToInt32(macroVector.X / 64);

            int microY = Convert.ToInt32(macroVector.Y / 64);

            int newDirection;

            if (Math.Abs(microY) < Math.Abs(microX))
            {
                if (microX > 0) // right
                {
                    newDirection = 1;
                }
                else // left
                {
                    newDirection = 3;

                }
            }
            else
            {
                if (microY > 0) // down
                {
                    newDirection = 2;
                }
                else // up
                {
                    newDirection = 0;

                }
            }

            List<int> targetList = new()
            {
                newDirection,
                microX + (int)vector.X,
                microY + (int)vector.Y
            };

            return targetList;

        }

        public static List<int> GetTargetDirectional(Vector2 vector, int direction, int distance = 5)
        {

            Dictionary<int, Vector2> vectorIndex = new()
            {

                [0] = vector + new Vector2(0, -distance),// up
                [1] = vector + new Vector2(distance, 0), // right
                [2] = vector + new Vector2(0, distance),// down
                [3] = vector + new Vector2(-distance, 0), // left

            };

            Vector2 targetVector = vectorIndex[direction];

            List<int> targetList = new()
            {
                direction,
                (int)targetVector.X,
                (int)targetVector.Y
            };

            return targetList;

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

                    //CastStars();

                    break;

                case rites.mists:

                    CastMists();

                    break;

                case rites.fates:

                    //CastFates();

                    break;

                case rites.ether:

                    //CastTransform();

                    //CreateTreasure();

                    break;

                default:

                    CastWeald();

                    break;
            }

            float oldStamina = Game1.player.Stamina;

            float staminaCost = Math.Min(castCost, oldStamina - 16);

            if(staminaCost > 0)
            {

                Game1.player.Stamina -= staminaCost;

            }

            Game1.player.checkForExhaustion(oldStamina);

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

            if (castLocation.objects.Count() > 0 && spawnIndex["weeds"])
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

            if (!witnesses.ContainsKey(rites.weald))
            {

                witnesses[rites.weald] = new();

            }

            GameLocation location = Game1.player.currentLocation;

            Vector2 origin = Game1.player.Position;

            List<NPC> villagers = ModUtility.GetFriendsInLocation(location, true);

            float threshold = 640;

            foreach (NPC witness in villagers)
            {

                if (witnesses[rites.weald].Contains(witness.Name))
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

                witnesses[rites.weald].Add(witness.Name);

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

                        if (witnesses[rites.weald].Contains(witness.Name))
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

                    if (witnesses[rites.weald].Contains(pair.Value.myID.ToString()))
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, origin) >= threshold)
                    {

                        continue;

                    }

                    ModUtility.PetAnimal(Game1.player, pair.Value);
                }


            }

            if (location is AnimalHouse animalLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in animalLocation.animals.Pairs)
                {

                    if (witnesses[rites.weald].Contains(pair.Value.myID.ToString()))
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, origin) >= threshold)
                    {

                        continue;

                    }

                    ModUtility.PetAnimal(Game1.player, pair.Value);
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

            rockSpell.projectile = 2;

            switch (Mod.instance.randomIndex.Next(2))
            {

                case 0:

                    rockSpell.scheme = IconData.schemes.rock;

                    break;

                default:

                    rockSpell.scheme = IconData.schemes.rockTwo;

                    break;

            }

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

            if (Mod.instance.questHandle.IsGiven(QuestHandle.questEffigy))
            {

                //ChannelMists();
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

            bool extraDebris = Mod.instance.questHandle.IsComplete(QuestHandle.mistsFour);

            if (castLocation.resourceClumps.Count > 0)
            {

                for (int r = castLocation.resourceClumps.Count -1; r >= 0; r--)
                {

                    ResourceClump resourceClump = castLocation.resourceClumps[r];

                    int cost = 0;

                    if (Vector2.Distance(resourceClump.Tile, castVector) <= 4)
                    {

                        switch (resourceClump.parentSheetIndex.Value)
                        {

                            case ResourceClump.stumpIndex:
                            case ResourceClump.hollowLogIndex:

                                cost = Math.Max(8, 32 - Game1.player.ForagingLevel * 3);

                                if (resourceClump.parentSheetIndex.Value == ResourceClump.hollowLogIndex)
                                {

                                    cost = (int)(castCost * 1.5);

                                }

                                Mod.instance.iconData.AnimateBolt(castLocation, resourceClump.Tile * 64 + new Vector2(32));

                                ModUtility.DestroyStump(castLocation, resourceClump, resourceClump.Tile, extraDebris);

                                resourceClump = null;

                                castCost += cost;

                                sundered++;

                                break;

                            default:

                                cost = Math.Max(8, 32 - Game1.player.MiningLevel * 3);

                                Mod.instance.iconData.AnimateBolt(castLocation, resourceClump.Tile * 64 + new Vector2(32));

                                ModUtility.DestroyBoulder(castLocation, resourceClump, resourceClump.Tile, extraDebris);

                                resourceClump = null;

                                castCost += cost;

                                sundered++;

                                break;

                        }

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

            if (spawnIndex["fishspot"])
            {

                if (ModUtility.WaterCheck(castLocation, castVector))
                {
                    castCost = Math.Max(8, 32 - (Game1.player.FishingLevel * 3));

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

                    Mod.instance.iconData.AnimateBolt(volcanoLocation, castVector * 64 + new Vector2(32));

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

            foreach(StardewValley.Monsters.Monster victim in victims)
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

                SpellHandle bolt = new(Game1.player, new() { victim, }, damage);

                bolt.type = SpellHandle.spells.bolt;

                bolt.critical = crits[0];

                bolt.criticalModifier = crits[1];

                bolt.added = new() { effects.push };

                Mod.instance.spellRegister.Add(bolt);

                castCost = Math.Max(6, 12 - Game1.player.CombatLevel / 2);

                smiteCount++;

            }

            // ---------------------------------------------
            // Villager iteration
            // ---------------------------------------------

            if(castLevel % 3 != 0)
            {

                return;

            }

            if (!witnesses.ContainsKey(rites.mists))
            {

                witnesses[rites.mists] = new();

            }

            List<NPC> villagers = ModUtility.GetFriendsInLocation(Game1.player.currentLocation, true);

            float threshold = 640;

            foreach (NPC witness in villagers)
            {

                if (witnesses[rites.mists].Contains(witness.Name))
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

                Mod.instance.spellRegister.Add(bolt);

                witness.faceTowardFarmerForPeriod(3000, 4, false, Game1.player);

                Game1.player.changeFriendship(-10, witness);

                ReactionData.ReactTo(witness, ReactionData.reactions.mists, -10);

                witnesses[rites.mists].Add(witness.Name);

            }

        }

        /*public void ChannelMists()
        {

            if (castLevel != 0) { 
                
                return; 
            
            }

            if (Mod.instance.activeEvent.Count > 0)
            {

                CastWisps();

                return;

            }

            Vector2 summonVector = Location.LocationData.SummoningVectors(castLocation);

            if (summonVector == Vector2.Zero)
            {

                CastWisps();

                return;

            }

            if (Vector2.Distance(summonVector, Game1.player.Position) >= 192f)
            {

                CastWisps();

                return;
            
            }

            if (Mod.instance.eventRegister.ContainsKey("summon"))
            {

                if (Mod.instance.eventRegister["summon"] is Summon summon)
                {

                    if (summon.eventLocked)
                    {
                        return;
                    }

                    if (!summon.AttemptAbort())
                    {

                        return;

                    }

                    summon.EventAbort();

                    summon.EventRemove();

                }

            }

            Summon summonEvent = new();

            summonEvent.EventSetup(Game1.player.Position, "summon");

            summonEvent.EventActivate();

        }*/

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

                    if (!wispEvent.AttemptAbort())
                    {

                        return;

                    }

                    wispEvent.EventAbort();

                    wispEvent.EventRemove();

                }

            }

            Wisps wispNew = new();

            wispNew.EventSetup(Game1.player.Position, "wisps");

            wispNew.EventActivate();

        }

        /*
        public void CastStars()
        {

            List<Vector2> meteorVectors = new();

            int difficulty = Mod.instance.Config.meteorBehaviour;

            int meteorLimit = 1;

            if (Mod.instance.questHandle.QuestComplete("starsOne") && randomIndex.Next(2) == 0)
            {

                meteorLimit = 2;

            }

            if(castLevel % 4 == 0 || vectorList.Count == 0)
            {

                if(difficulty == 5)
                {
                    
                    vectorList = new();

                    List<Vector2> innerTiles = ModUtility.GetTilesWithinRadius(castLocation, Vector2.Zero, 3, false);

                    for (int iv = 0; iv < 3; iv++)
                    {
                        
                        vectorList.Add(innerTiles[randomIndex.Next(innerTiles.Count)]);
                        
                    }

                    List<Vector2> outerTiles = ModUtility.GetTilesWithinRadius(castLocation, Vector2.Zero, 4, false);

                    for (int ov = 0; ov < 3; ov++)
                    {
                        
                        vectorList.Add(outerTiles[randomIndex.Next(outerTiles.Count)]);

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
                    
                    Vector2 randomVector = vectorList[randomIndex.Next(vectorList.Count)];

                    vectorList.Remove(randomVector);

                    meteorVectors.Add(castVector + randomVector);

                }

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

                    if (randomIndex.Next(3) == 0) { extra++; }

                    break;

                case 5:

                    damage *= 1.6f;

                    if(randomIndex.Next(2) == 0) { extra++; }

                    break;

            }

            foreach (Vector2 meteorVector in meteorVectors)
            {

                effectCasts[meteorVector] = new Cast.Stars.Meteor(meteorVector, damage, extra);

            }

            if (Mod.instance.eventRegister.ContainsKey("wisp"))
            {

                foreach(Vector2 meteorVector in meteorVectors)
                {

                    (Mod.instance.eventRegister["wisp"] as Weald.WispEvent).UpdateWisp(meteorVector, 3);

                }

            }

        }

        public void CastComet(GameLocation location, Vector2 tile)
        {


            TemporaryAnimatedSprite startAnimation = new(0, 1000f, 1, 1, tile * 64 - new Vector2(128, 128), false, false)
            {

                sourceRect = new(128, 0, 64, 64),

                sourceRectStartingPos = new Vector2(128, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),

                scale = 5f,

                layerDepth = 0.0001f,

                rotationChange = 0.06f,

                timeBasedMotion = true,

                alpha = 0.75f,

            };

            location.temporarySprites.Add(startAnimation);

            SpellHandle meteor = new(Game1.player, tile * 64, 5 * 64, Mod.instance.CombatDamage() * 4);

            meteor.type = SpellHandle.spells.meteor;

            meteor.scheme = SpellHandle.schemes.stars;

            meteor.indicator = IconData.cursors.stars;

            meteor.display = SpellHandle.displays.Impact;

            meteor.projectile = 5;

            meteor.sound = sounds.explosion;

            meteor.environment = 8;

            meteor.power = 4;

            meteor.terrain = 5;

            Mod.instance.spellRegister.Add(meteor);

        }

        public void CastFates()
        {

            string locationName = castLocation.Name;

            int fatesLevel = 1;

            if (Mod.instance.save.milestone > Journal.QuestHandle.milestones.weald_lessons)
            {

                fatesLevel = 5;

            }
            else
            {

                if (Mod.instance.questHandle.QuestGiven("fatesTwo")) { fatesLevel++; }
                if (Mod.instance.questHandle.QuestGiven("fatesThree")) { fatesLevel++; }
                if (Mod.instance.questHandle.QuestGiven("fatesFour")) { fatesLevel++; }
                if (Mod.instance.questHandle.QuestGiven("fatesFive")) { fatesLevel++; }

            }

            List<Vector2> centerVectors = new();

            if (!Mod.instance.specialCasts.ContainsKey(locationName))
            {

                Mod.instance.specialCasts[locationName] = new();

            }

            List<string> specialCasts = Mod.instance.specialCasts[locationName];

            if (!Mod.instance.targetCasts.ContainsKey(locationName))
            {

                Mod.instance.targetCasts[locationName] = ModUtility.LocationTargets(castLocation);

            }

            // ---------------------------------------------
            // Enchant
            // ---------------------------------------------

            if (castLocation.objects.Count() > 0 && fatesLevel >= 3)
            {

                int castAttempt = (castLevel % 8);

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(castLocation, castVector, castAttempt + 1); // 1, 2, 3, 4, 5, 6, 7, 8

                List<Vector2> betweenVectors = ModUtility.GetTilesBetweenPositions(castLocation, Game1.player.Position, castVector * 64);

                tileVectors.AddRange(betweenVectors);

                List<string> craftIndex = SpawnData.MachineList();

                List<Vector2> objectVectors = new();

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (Mod.instance.targetCasts[locationName].ContainsKey(tileVector))
                    {

                        continue;

                    }

                    if (castLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        if (craftIndex.Contains(targetObject.Name))
                        {

                            if (targetObject.heldObject.Value == null || targetObject.MinutesUntilReady > 10)
                            {

                                objectVectors.Add(tileVector);

                            }

                            continue;

                        }

                    }

                }

                if (objectVectors.Count > 0)
                {

                    List<int> cycleList = new()
                    {
                       1, 2, 2, 3, 3, 4, 4, 5,
                    };

                    int castCycle = cycleList[castAttempt];

                    for (int i = 0; i < castCycle; i++)
                    {

                        int useSource = ConsumeSource();

                        if (useSource == -1)
                        {

                            break;

                        }

                        int selectedIndex = randomIndex.Next(objectVectors.Count);

                        Vector2 tileVector = objectVectors[selectedIndex];

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        effectCasts[tileVector] = new Cast.Fates.Enchant(tileVector, targetObject, useSource);

                        Mod.instance.targetCasts[locationName][tileVector] = "Machine";

                        objectVectors.RemoveAt(selectedIndex);

                        centerVectors.Add(tileVector);

                        Mod.instance.iconData.CursorIndicator(castLocation, tileVector * 64, IconData.cursors.fates, 600, 3f);

                        if (objectVectors.Count == 0)
                        {
                            break;
                        }

                    }

                    return;

                }

            }

            // ---------------------------------------------
            // Escape
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (castLevel == 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("escape")) { break; }

                if (castLocation.warps.Count() <= 0) { if (castLocation is not MineShaft) { break; } }

                Escape escapeEvent = new(Game1.player.Position);

                escapeEvent.EventTrigger();

            }

            // ---------------------------------------------
            // Gravity
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (fatesLevel < 4) { break; }

                if (!spawnIndex["gravity"]) { break; }

                //if (castLocation.objects.Count() <= 0) { break; }

                if (castLevel % 2 == 1) { break; }

                //if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                Vector2 wellVector = castVector;

                for (int i = 0; i < 3; i++)
                {

                    List<Vector2> wellVectors = ModUtility.GetTilesWithinRadius(castLocation, wellVector, i);

                    foreach (Vector2 tileVector in wellVectors)
                    {

                        if (!castLocation.objects.ContainsKey(tileVector)) { continue; }

                        StardewValley.Object targetObject = castLocation.objects[tileVector];

                        if (!targetObject.IsScarecrow()) { continue; }

                        string scid = "gravity_" + castLocation.Name + "_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                        if (specialCasts.Contains(scid)) { continue; }

                        effectCasts[tileVector] = new Cast.Fates.Blackhole(tileVector, 0);

                        Mod.instance.specialCasts[locationName].Add(scid);

                        return;

                    }

                }

            }

            // ---------------------------------------------
            // Gravity - Monster
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {
                if (fatesLevel < 4) { break; }

                if (castLocation.characters.Count <= 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                Vector2 wellVector = castVector;

                foreach (NPC nonPlayableCharacter in castLocation.characters)
                {

                    if (nonPlayableCharacter is not StardewValley.Monsters.Monster monsterCharacter) { continue; }

                    if (monsterCharacter.Health <= 0 || monsterCharacter.IsInvisible) { continue; }

                    float monsterDifference = Vector2.Distance(monsterCharacter.Position, wellVector * 64);

                    if (monsterDifference > 640f) { continue; }

                    effectCasts[wellVector] = new Cast.Fates.Blackhole(wellVector, 1);

                    return;

                }

            }
            // ---------------------------------------------
            // Gravity - Teahouse
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {
                if (fatesLevel < 4) { break; }

                if (!spawnIndex["teahouse"]) { break; }

                if (Mod.instance.eventRegister.ContainsKey("gravity")) { break; }

                Vector2 wellVector = castVector;

                string scid = "gravity_" + castLocation.Name + "_" + wellVector.X.ToString() + "_" + wellVector.Y.ToString();

                if (specialCasts.Contains(scid)) { continue; }

                effectCasts[wellVector] = new Cast.Fates.Blackhole(wellVector,0);

                Mod.instance.specialCasts[locationName].Add(scid);

                return;

            }


            // ---------------------------------------------
            // Whisk
            // ---------------------------------------------

            for (int a = 0; a < 1; a++)
            {

                if (effectCasts.Count > 0) { break; }

                if (Mod.instance.eventRegister.ContainsKey("whisk")) { break; }

                if (Mod.instance.eventRegister.ContainsKey("transform")) { break; };

                if (!spawnIndex["whisk"]) { break; }

                if (castLevel != 0) { break; }

                int whiskRange = 18;

                if (Mod.instance.questHandle.QuestComplete("fatesOne"))
                {

                    whiskRange += 6;

                }

                Vector2 originVector = Game1.player.Tile;

                for (int i = whiskRange; i > 8; i--)
                {

                    List<int> targetList = GetTargetCursor(Game1.player.Tile, Game1.player.FacingDirection, i, 8);

                    Vector2 whiskDestiny = new(targetList[1], targetList[2]);


                    if (ModUtility.GroundCheck(castLocation, whiskDestiny) != "ground")
                    {

                        continue;

                    }

                    Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();

                    int diffX = (int)(boundingBox.X - Game1.player.Position.X);

                    int diffY = (int)(boundingBox.Y - Game1.player.Position.Y);

                    boundingBox.X = (int)(whiskDestiny.X * 64) + diffX;

                    boundingBox.Y = (int)(whiskDestiny.Y * 64) + diffY;

                    if (castLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, Game1.player, pathfinding: false))
                    {
                        
                        continue;

                    }

                    effectCasts[whiskDestiny] = new Cast.Fates.Whisk(originVector, whiskDestiny*64);

                    break;

                }

            }

            if (effectCasts.Count > 0 && Mod.instance.eventRegister.ContainsKey("escape"))
            {

                if (Mod.instance.eventRegister["escape"].activeCounter >= 2)
                {

                    Mod.instance.eventRegister["escape"].expireEarly = true;

                }

            }

        }

        public int ConsumeSource()
        {

            List<int> source = (randomIndex.Next(2) == 0) ? new() { 768, 769 } : new() { 769, 768 };

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {

                Item checkItem = Game1.player.Items[i];

                // ignore empty slots
                if (checkItem == null || Game1.player.Items[i].Stack == 0)
                {

                    continue;

                }

                int itemIndex = checkItem.ParentSheetIndex;

                if (itemIndex == source[0])
                {
                    
                    Game1.player.Items[i].Stack -= 1;

                    return source[0];

                }

                if (itemIndex == source[1])
                {
                    Game1.player.Items[i].Stack -= 1;

                    return source[1];
                }

            }

            return -1;

        }

        public void CastTransform()
        {

            for (int index = 0; index < 1; ++index)
            {

                if (castLevel != 0)
                {

                    break;
                }

                if (Mod.instance.eventRegister.ContainsKey("transform"))
                {

                    Mod.instance.eventRegister["transform"].AttemptAbort();

                    break;

                }

                int extend = 120;

                if (Mod.instance.questHandle.QuestComplete("etherOne")) // transform mastery
                {
                    extend = 180;
                }

                Transform transform = new Transform(Game1.player.Position, extend);

                if (Mod.instance.questHandle.QuestGiven("etherTwo"))
                {
                    transform.leftActive = true;

                }

                if (Mod.instance.questHandle.QuestGiven("etherThree"))
                {
                    transform.rightActive = true;

                }

                transform.EventTrigger();

            }

        }

        public void CreateTreasure()
        {

            if (!Mod.instance.questHandle.QuestGiven("etherFive"))
            {

                return;

            }

            if (!spawnIndex["crate"])
            {

                return;

            }

            if (!Mod.instance.specialCasts.ContainsKey(castLocation.Name))
            {

                Mod.instance.specialCasts[castLocation.Name] = new();

            }

            if (Mod.instance.specialCasts[castLocation.Name].Contains("crate"))
            {

                return;

            }

            if (Mod.instance.eventRegister.ContainsKey("crate"))
            {

                if (Mod.instance.eventRegister["crate"].targetLocation.Name != castLocation.Name)
                {

                    Mod.instance.eventRegister["crate"].EventAbort();

                    Mod.instance.eventRegister["crate"].EventRemove();

                }
                else
                {

                    return;

                }

            }

            Crate treasure = new Crate(castVector);

            treasure.EventTrigger();

        }*/

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