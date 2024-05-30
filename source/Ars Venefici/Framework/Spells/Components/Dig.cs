/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/


using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using SpaceCore;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static ArsVenefici.Framework.GUI.DragNDrop.ShapeGroupArea;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Dig : AbstractComponent
    {
        ModEntry modEntry;

        public Dig(ModEntry modEntry) : base()
        { 
            this.modEntry = modEntry;
        }

        public override string GetId()
        {
            return "dig";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

            TilePos blockPos = target.GetTilePos();

            // create fake tools
            Axe axe = new();
            Pickaxe pickaxe = new();

            int toolLevel = 0;

            if (Game1.player.GetCustomSkillLevel(ModEntry.Skill) >= 2 && Game1.player.GetCustomSkillLevel(ModEntry.Skill) < 4)
            {
                toolLevel = 1;
            }
            else if (Game1.player.GetCustomSkillLevel(ModEntry.Skill) >= 4 && Game1.player.GetCustomSkillLevel(ModEntry.Skill) < 6)
            {
                toolLevel = 2;
            }
            else if (Game1.player.GetCustomSkillLevel(ModEntry.Skill) >= 6 && Game1.player.GetCustomSkillLevel(ModEntry.Skill) < 8)
            {
                toolLevel = 3;
            }
            else if (Game1.player.GetCustomSkillLevel(ModEntry.Skill) >= 8)
            {
                toolLevel = 4;
            }

            var helper = SpellHelper.Instance();
            float miningPower = helper.GetModifiedStat(1, new SpellPartStats(SpellPartStatType.POWER), modifiers, spell, caster, target, index);

            if(toolLevel + (int)miningPower < 4)
            {
                toolLevel += (int)miningPower;
            }
            else if (toolLevel + (int)miningPower > 4)
            {
                toolLevel = 4;
            }

            foreach (var t in new Tool[] { axe, pickaxe })
            {
                t.UpgradeLevel = toolLevel;
                t.IsEfficient = true; // don't drain stamina
                modEntry.Helper.Reflection.GetField<Farmer>(t, "lastUser").SetValue(caster.entity as Farmer);
            }

            //modEntry.Monitor.Log("axe:" + axe.UpgradeLevel.ToString(), LogLevel.Info);

            Vector2 tile = blockPos.GetVector();
            Vector2 toolPixel = (tile * Game1.tileSize) + new Vector2(Game1.tileSize / 2f); // center of tile

            if (gameLocation.objects.TryGetValue(blockPos.GetVector(), out StardewValley.Object obj))
            {
                // select tool
                Tool tool = null;

                if (this.IsAxeDebris(gameLocation, obj))
                    tool = axe;
                else if (this.IsPickaxeDebris(gameLocation, obj))
                    tool = pickaxe;

                if (tool != null)
                {
                    //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

                    ((Farmer)caster.entity).lastClick = toolPixel;
                    tool.DoFunction(gameLocation, (int)toolPixel.X, (int)toolPixel.Y, 0, ((Farmer)caster.entity));

                    return new SpellCastResult(SpellCastResultType.SUCCESS);
                }
            }
            else if (gameLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is not HoeDirt or Flooring)
            {
                if (feature is Tree)
                {
                    
                }

                if (feature.performToolAction(axe, 0, tile) || feature is Grass || (feature is Tree && feature.performToolAction(axe, 0, tile)))
                {
                   
                    //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

                    gameLocation.terrainFeatures.Remove(tile);
                }

                if (feature is Grass && gameLocation is Farm farm)
                {
                    //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

                    farm.tryToAddHay(1);
                    gameLocation.localSound("swordswipe", tile);

                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(28, tile * Game1.tileSize + new Vector2(Game1.random.Next(-Game1.pixelZoom * 4, Game1.pixelZoom * 4), Game1.random.Next(-Game1.pixelZoom * 4, Game1.pixelZoom * 4)), Color.Green, 8, Game1.random.NextDouble() < 0.5, Game1.random.Next(60, 100)));
                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.objectSpriteSheetName, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, ((Character)caster.entity).position.Value - new Vector2(0.0f, Game1.tileSize * 2), false, false, ((Character)caster.entity).position.Y / 10000f, 0.005f, Color.White, Game1.pixelZoom, -0.005f, 0.0f, 0.0f)
                    {
                        motion = { Y = -1f },
                        layerDepth = (float)(1.0 - Game1.random.Next(100) / 10000.0),
                        delayBeforeAnimationStart = Game1.random.Next(350)
                    });
                }

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }
            else if (axe.UpgradeLevel >= 2 || pickaxe.UpgradeLevel >= 2)
            {

                ICollection<ResourceClump> clumps = gameLocation.resourceClumps;

                if (gameLocation is Woods woods)
                    clumps = woods.resourceClumps;

                if (clumps != null)
                {
                    foreach (var rc in clumps)
                    {
                        if (new Rectangle((int)rc.Tile.X, (int)rc.Tile.Y, rc.width.Value, rc.height.Value).Contains(tile.X, tile.Y))
                        {
                            if (rc.performToolAction(axe, 1, tile) || rc.performToolAction(pickaxe, 1, tile))
                            {
                                clumps.Remove(rc);
                                return new SpellCastResult(SpellCastResultType.SUCCESS);
                            }

                            //break;
                        }
                    }
                }

            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        /// <summary>Get whether a given object is debris which can be cleared with a pickaxe.</summary>
        /// <param name="location">The location containing the object.</param>
        /// <param name="obj">The world object.</param>
        private bool IsPickaxeDebris(GameLocation location, StardewValley.Object obj)
        {
            if (obj is not Chest or null)
            {
                // stones
                if (obj.Name is "Weeds" or "Stone")
                    return true;

                // spawned mine objects
                if (location is MineShaft && obj.IsSpawnedObject)
                    return true;
            }

            return false;
        }

        /// <summary>Get whether a given object is debris which can be cleared with an axe.</summary>
        /// <param name="location">The location containing the object.</param>
        /// <param name="obj">The world object.</param>
        private bool IsAxeDebris(GameLocation location, StardewValley.Object obj)
        {
            if (obj is not Chest or null)
            {
                // twig
                if (obj.ParentSheetIndex is 294 or 295)
                    return true;

                // weeds
                if (obj.Name is "Weeds")
                    return true;

                // spawned mine objects
                if (location is MineShaft && obj.IsSpawnedObject)
                    return true;
            }

            return false;
        }

        public override int ManaCost()
        {
            return 5;
        }
    }
}
