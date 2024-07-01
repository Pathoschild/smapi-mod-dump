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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Cast.Ether
{
    public class Crate : EventHandle
    {

        public int crateTerrain;

        public bool crateThief;

        public List<StardewDruid.Monster.Boss> thieves;

        public Crate()
        {

        }

        public override bool TriggerActive()
        {

            if(location.Name == Game1.player.currentLocation.Name)
            {

                return true;

            }

            return false;

        }

        public override bool TriggerCheck()
        {

            return false;

        }

        public override void TriggerInterval()
        {

            triggerCounter++;

            if(triggerCounter % 4 == 0)
            {

                animations.Clear();

                TemporaryAnimatedSprite radiusAnimation = new(0, 4000, 1, 1, origin - new Vector2(16, 16), false, false)
                {

                    sourceRect = new(0, 64, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 64),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),

                    scale = 1.5f, //* size,

                    layerDepth = 900f,

                    rotationChange = (float)Math.PI / 1000,

                    alpha = 0.75f,

                };

                location.temporarySprites.Add(radiusAnimation);

                animations.Add(radiusAnimation);

            }


        }

        public override void EventActivate()
        {
            
            base.EventActivate();

            if(!crateThief)
            {

                eventComplete = true;

            }

        }

        public override void EventRemove()
        {

            if (thieves.Count > 0)
            {
    
                thieves.First().currentLocation.characters.Remove(thieves.First());
            
            }

            base.EventRemove();
        
        }


        public override void EventInterval()
        {

            activeCounter++;

            if (thieves.Count > 0)
            {

                if (!ModUtility.MonsterVitals(thieves.First(), location) || thieves.First().netWoundedActive.Value)
                {

                    origin = thieves.First().Position;

                    eventComplete = true;

                    return;

                }

                return;

            }

            activeLimit = 60;

            EventBar("Treasure Chase", 0);

            StardewDruid.Monster.Boss thief;

            switch (Mod.instance.randomIndex.Next(3))
            {
                default:
                case 0:
                    thief = new StardewDruid.Monster.DarkRogue(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());
                    break;
                case 1:
                    thief = new StardewDruid.Monster.DarkShooter(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());
                    break;
                case 2:
                    thief = new StardewDruid.Monster.DarkGoblin(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());
                    break;

            }

            thief.currentLocation = location;

            location.characters.Add(thief);

            thief.baseJuice = 1;

            thief.SetMode(3);

            thief.tempermentActive = Monster.Boss.temperment.coward;

            thief.netHaltActive.Set(true);

            thief.idleTimer = 60;

            thief.SetDirection(Game1.player.Position);

            thief.setWounded = true;

            thief.update(Game1.currentGameTime, location);

            EventDisplay bossBar = Mod.instance.CastDisplay("Treasure Thief", "Treasure Thief");

            bossBar.boss = thief;

            bossBar.type = EventDisplay.displayTypes.bar;

            bossBar.colour = Microsoft.Xna.Framework.Color.Red;

            Mod.instance.CastMessage("A thief has snatched the treasure!");

            thieves.Add(thief);

        }

        public override void EventCompleted()
        {
            
            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherFour))
            {

                Mod.instance.questHandle.UpdateTask(Journal.QuestHandle.etherFour, 1);

            }

            if (!Mod.instance.rite.specialCasts.ContainsKey(location.Name))
            {

                Mod.instance.rite.specialCasts[location.Name] = new();

            }

            Mod.instance.rite.specialCasts[location.Name].Add("crate");

            if (
                location is MineShaft mineShaft 
                && mineShaft.mineLevel != MineShaft.bottomOfMineLevel 
                && mineShaft.mineLevel != MineShaft.quarryMineShaft)
            {

                Layer layer = location.map.GetLayer("Buildings");

                Vector2 treasureVector = ModUtility.PositionToTile(origin);

                layer.Tiles[(int)treasureVector.X, (int)treasureVector.Y] = new StaticTile(layer, location.map.TileSheets[0], 0, mineShaft.mineLevel > 120 ? 174 : 173);

                Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int)treasureVector.X * 64, (int)treasureVector.Y * 64, 64, 64));

                Mod.instance.CastMessage("A way down has appeared");

                return;

            }

            if (Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherFour))
            {


            }

            SpellHandle crate = new(location, new(1), origin);

            crate.type = SpellHandle.spells.crate;

            crate.counter = 60;

            crate.Update();

            Mod.instance.spellRegister.Add(crate);

        }

    }

}
