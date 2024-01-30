/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class Portal : EventHandle
    {

        List<int> portalConfig;

        public Portal(Vector2 target, Rite rite)
            : base(target, rite)
        {
            targetVector = target;
            voicePosition = (target - new Vector2(0, 1)) * 64;
        }

        public override void EventTrigger()
        {

            targetLocation.objects.Remove(targetVector);

            portalConfig = PortalConfig();

            monsterHandle = new(targetVector, riteData.castLocation);

            monsterHandle.spawnFrequency = portalConfig[1];

            monsterHandle.spawnAmplitude = portalConfig[2];

            monsterHandle.spawnSpecial = portalConfig[3];

            monsterHandle.spawnCombat *= 1 + (portalConfig[5] / 4);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + portalConfig[4];

            monsterHandle.spawnIndex = new()
            {
                0,1,2,3,4,99,

            };

            monsterHandle.specialIndex = new()
            {
                11,12,13,

            };

            Brazier brazier = new(targetLocation, targetVector);

            braziers.Add(brazier);

            Vector2 boltVector = new(targetVector.X, targetVector.Y - 1);

            ModUtility.AnimateBolt(targetLocation, boltVector);

            SetTrack("tribal");

            Mod.instance.RegisterEvent(this, "active");

            Game1.addHUDMessage(new HUDMessage($"Portal Strength " + portalConfig[0].ToString(), ""));

        }

        public List<int> PortalConfig()
        {

            int strength = 0;

            List<Vector2> removeList = new();

            for (int i = 1; i < 4; i++)
            {

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, i);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (targetLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = targetLocation.objects[tileVector];

                        if (targetObject is Torch && targetObject.ParentSheetIndex == 93) // crafted candle torch
                        {

                            removeList.Add(tileVector);

                        }

                    }

                }

            }

            foreach (Vector2 tileVector in removeList)
            {

                targetLocation.objects.Remove(tileVector);

                strength++;

            }

            Dictionary<int, List<int>> configs = new()
            {

                [0] = new() { 1, 2, 1, 0, 60, 0, }, // 30 monsters,
                [1] = new() { 2, 3, 2, 0, 72, 0, }, // 48 monsters, 
                [2] = new() { 3, 1, 1, 0, 72, 1, }, // 72 monsters, 1.25x difficulty
                [3] = new() { 4, 1, 1, 1, 72, 1, }, // 72 monsters, 1 boss, 1.25x difficulty
                [4] = new() { 5, 2, 3, 1, 72, 1, }, // 108 monsters, 1 boss, 1.25x difficulty
                [5] = new() { 6, 1, 1, 2, 96, 2, }, // 96 monsters, 2 bosses, 1.5x difficulty
                [6] = new() { 7, 1, 1, 3, 120, 2, }, // 120 monsters, 3 bosses, 1.5x difficulty

            };

            int setting = Math.Min(strength, configs.Count - 1);

            return configs[setting];

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                if (!riteData.castTask.ContainsKey("masterPortal"))
                {

                    Mod.instance.UpdateTask("lessonPortal", 1);

                }

                int tileX = (int)targetVector.X;

                int tileY = (int)targetVector.Y;

                switch (portalConfig[0])
                {
                    case 2:
                    case 3:
                        for (int i = 0; i < 2; i++)
                        {
                            Game1.createObjectDebris(335, tileX, tileY);
                        }
                        CastVoice("good", 2000);
                        break;
                    case 4:
                    case 5:
                        for (int i = 0; i < 2; i++)
                        {
                            Game1.createObjectDebris(336, tileX, tileY);
                        }
                        CastVoice("great", 2000);
                        break;
                    case 6:
                        for (int i = 0; i < 2; i++)
                        {
                            Game1.createObjectDebris(337, tileX, tileY);
                        }
                        CastVoice("superb", 2000);
                        break;
                    case 7:
                        if (riteData.randomIndex.Next(3) == 0)
                        {
                            Game1.createObjectDebris(74, tileX, tileY);
                        }
                        else
                        {
                            Game1.createObjectDebris(446, tileX, tileY, itemQuality: 2);
                        }
                        CastVoice("brilliant", 2000);
                        break;
                    default:
                        for (int i = 0; i < 2; i++)
                        {
                            Game1.createObjectDebris(334, tileX, tileY);
                        }
                        CastVoice("sufficient", 2000);
                        break;

                }

                RemoveMonsters();

                eventLinger = 2;

                return true;

            }

            return base.EventExpire();

        }

        public override void EventAbort()
        {

            Game1.addHUDMessage(new HUDMessage($"The portal through the veil has collapsed", ""));

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            monsterHandle.SpawnInterval();

            if (activeCounter % 30 == 0)
            {

                ResetBraziers();

            }

        }

    }

}
