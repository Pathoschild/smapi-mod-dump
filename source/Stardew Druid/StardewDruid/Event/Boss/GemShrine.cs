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
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewDruid.Monster.Boss;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Minigames;
using System.Collections.Generic;
using static StardewValley.IslandGemBird;
using static StardewValley.Objects.BedFurniture;

namespace StardewDruid.Event.Boss
{
    public class GemShrine : BossHandle
    {

        public List<StardewDruid.Monster.Boss.Boss> bossMonsters;

        public GemShrine(Vector2 target,  Quest quest)
            : base(target, quest)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 90;

            bossMonsters = new();

        }

        public override void EventTrigger()
        {
            
            cues = DialogueData.DialogueScene(questData.name);

            AddActor(new Vector2(24, 22) * 64);

            Cast.Stars.Meteor meteorCast = new(targetVector, Mod.instance.DamageLevel());

            meteorCast.targetLocation = targetLocation;

            meteorCast.CastEffect();

            Mod.instance.RegisterEvent(this, "active");

            Mod.instance.CastMessage("Gem Shrine challenge initiated");

        }


        public override void RemoveMonsters()
        {

            if (bossMonsters.Count > 0)
            {

                foreach (StardewDruid.Monster.Boss.Boss boss in bossMonsters)
                {

                    Mod.instance.rite.castLocation.characters.Remove(boss);

                }

                bossMonsters.Clear();

            }

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {
                if (expireEarly)
                {
                    
                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 991);

                    if (!questData.name.Contains("Two"))
                    {

                        Game1.createObjectDebris("74", 24, 21);

                    }

                    Game1.createObjectDebris("69", 24, 21);

                    Game1.createObjectDebris("835", 24, 21);

                    EventComplete();

                }
                else
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 992);

                }

                eventLinger = 3;

                RemoveMonsters();

                return true;

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (eventLinger != -1)
            {

                return;

            }

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, activeCounter);

            if (activeCounter < 7)
            {

                Game1.playSound("batFlap");
                return;
            }

            if (activeCounter == 7)
            {

                Dictionary<string, Vector2> gemVectors = new()
                {
                    ["IslandWest"] = new Vector2(21 - 2, 27 + 1),
                    ["IslandEast"] = new Vector2(27 + 2, 27 - 1),
                    ["IslandSouth"] = new Vector2(24 + 1, 28 + 2),
                    ["IslandNorth"] = new Vector2(24 - 1, 25 - 2),

                };

                foreach(KeyValuePair<string, Vector2> gemPair in gemVectors)
                {
                    
                    ModUtility.AnimateQuickWarp(targetLocation, gemPair.Value);

                    GemBirdType gemBirdType = GetBirdTypeForLocation(gemPair.Key);

                    StardewDruid.Monster.Boss.Demonki boss = new(gemPair.Value, Mod.instance.CombatModifier());

                    targetLocation.characters.Add(boss);

                    boss.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

                    boss.ChampionMode();

                    boss.netScheme.Set(gemBirdType.ToString());

                    boss.SchemeLoad();

                    if (!questData.name.Contains("Two"))
                    {

                        boss.HardMode();

                    }

                    bossMonsters.Add(boss);

                }

                SetTrack("cowboy_boss");

                return;

            }

            for(int i = bossMonsters.Count-1;i >= 0; i--)
            {

                StardewDruid.Monster.Boss.Boss bossMonster = bossMonsters[i];

                if (!ModUtility.MonsterVitals(bossMonster, targetLocation))
                {

                    Mod.instance.rite.castLocation.characters.Remove(bossMonster);

                    bossMonsters.RemoveAt(i);

                }

            }

            if (bossMonsters.Count == 0)
            {

                expireEarly = true;

            }

        }


    }
}
