using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace FollowerNPC
{
    public class ModEntry : Mod
    {

        #region Members and Entry Function
        public static ModConfig config;
        public static IMonitor monitor;
        public static IModHelper modHelper;

        public CompanionsManager companionsManager;

        public static MethodInfo applyVelocity;

        private List<FarmerSprite.AnimationFrame> fishingLeftAnim;
        private List<FarmerSprite.AnimationFrame> fishingRightAnim;

        public override void Entry(IModHelper helper)
        {
            // Initialize variables //
            config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;
            modHelper = helper;
            companionsManager = new CompanionsManager();
            //**********************//

            // Patch methods //
            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.FollowerNPC");

            Type[] isCollidingPositionTypes0 = new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            Type[] isCollidingPositionTypes1 = new Type[] { typeof(GameLocation), typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            MethodInfo isCollidingPositionOriginal = typeof(GameLocation).GetMethod("isCollidingPosition", isCollidingPositionTypes0);
            MethodInfo isCollidingPositionprefix = typeof(Patches).GetMethod("Prefix", isCollidingPositionTypes1);
            MethodInfo isCollidingPositionpostfix = typeof(Patches).GetMethod("Postfix", isCollidingPositionTypes1);
            harmony.Patch(isCollidingPositionOriginal, new HarmonyMethod(isCollidingPositionprefix), new HarmonyMethod(isCollidingPositionpostfix));

            //Type[] movePositionTypes0 = new Type[] { typeof(GameTime), typeof(Rectangle), typeof(GameLocation) };
            //Type[] movePositionTypes1 = new Type[] { typeof(NPC), typeof(GameTime), typeof(Rectangle), typeof(GameLocation) };
            //MethodInfo movePositionOriginal = typeof(NPC).GetMethod("MovePosition");
            //MethodInfo movePositionPrefix = typeof(Patches).GetMethod("Prefix", movePositionTypes1);
            //harmony.Patch(movePositionOriginal, new HarmonyMethod(movePositionPrefix), null);

            Type[] updateMovementTypes0 = new Type[] { typeof(GameLocation), typeof(GameTime) };
            Type[] updateMovementTypes1 = new Type[] { typeof(NPC), typeof(GameLocation), typeof(GameTime) };
            MethodInfo updateMovementOriginal = typeof(NPC).GetMethod("updateMovement");
            MethodInfo updateMovementPrefix = typeof(Patches).GetMethod("Prefix", updateMovementTypes1);
            harmony.Patch(updateMovementOriginal, new HarmonyMethod(updateMovementPrefix), null);

            Type[] faceTowardFarmerForPeriodTypes1 = new Type[] {typeof(NPC)};
            MethodInfo faceTowardFarmerForPeriodOriginal = typeof(NPC).GetMethod("faceTowardFarmerForPeriod");
            MethodInfo faceTowardFarmerForPeriodPrefix =
                typeof(Patches).GetMethod("Prefix", faceTowardFarmerForPeriodTypes1);
            harmony.Patch(faceTowardFarmerForPeriodOriginal, new HarmonyMethod(faceTowardFarmerForPeriodPrefix), null);

            Type[] gainExperienceTypes1 = new Type[] { typeof(Farmer), typeof(int).MakeByRefType() };
            MethodInfo gainExperienceOriginal = typeof(Farmer).GetMethod("gainExperience");
            MethodInfo gainExperiencePrefix = typeof(Patches).GetMethod("Prefix", gainExperienceTypes1);
            harmony.Patch(gainExperienceOriginal, new HarmonyMethod(gainExperiencePrefix), null);

            fishingLeftAnim = new List<FarmerSprite.AnimationFrame>
            {
                new FarmerSprite.AnimationFrame(0, 4000, false, true, null, false),
                new FarmerSprite.AnimationFrame(0, 4000, false, true, null, false)
            };
            fishingRightAnim = new List<FarmerSprite.AnimationFrame>
            {
                new FarmerSprite.AnimationFrame(20, 4000),
                new FarmerSprite.AnimationFrame(21, 4000)
            };

            applyVelocity =
                typeof(Character).GetMethod("applyVelocity", BindingFlags.NonPublic | BindingFlags.Instance);
            //**********************//

            // Subscribe to events //
            //Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            //**********************//
        }


        #endregion

        #region Event Functions

        // Just used for debug commands
        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady || companionsManager == null)
                return;

            //if (e.Button == SButton.K)
            //{
            //    companionsManager.farmer.eventsSeen.Remove(471942);
            //}
        }

        #endregion
    }

    /// <summary>
    /// A collection of Harmony patches that allow me to modify, add, or remove certain
    /// functionalities of CA's code to make it work nicely with mine. 
    /// </summary>
    class Patches
    {
        static public string companion;

        /// <summary>
        /// A weird, roundabout way of allowing Companions to pass through invisible
        /// barriers that normally block NPC's. Might want to consider making this a
        /// little less, strange(?), in the future.
        /// </summary>
        #region isCollidingPosition
        static public bool flag;
        static public string bypass = "NPC";
        static public string name;

        static public void Prefix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (companion != null
                && character != null
                && character.Name != null
                && character.Name.Equals(companion)
                && !character.eventActor)
            {
                flag = true;
                name = character.Name;
                character.Name = bypass;
            }
        }

        static public void Postfix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (flag)
            {
                flag = false;
                character.Name = name;
            }
        }
        #endregion

        /// <summary>
        /// The Companion's daily schedule is remade from scratch once they are dismissed.
        /// This pops off any routes from their schedule that are in the past until it reaches
        /// the most recent route they would have traveled. It then pops off that route and stores
        /// it as the location they will be warped to after they are finished being dismissed.
        /// </summary>
        #region checkSchedule
        static public Point scheduleCurrentDestination;

        static public void Postfix(NPC __instance, int timeOfDay)
        {
            if (companion != null && companion == __instance.Name)
            {
                SchedulePathDescription spd;
                if (__instance.Schedule.TryGetValue(timeOfDay, out spd) && spd != null)
                {
                    while (spd.route.Count != 0)
                        scheduleCurrentDestination = spd.route.Pop();
                }
            }
        }
        #endregion

        /// <summary>
        /// Prevents the Companion from updating their movement via CA's movementUpdate function
        /// while they are the farmer's companion.
        /// </summary>
        #region movePosition
        
        static public bool Prefix(NPC __instance, GameTime time, Rectangle viewport, GameLocation currentLocation)
        {
            bool dontSkip = (companion == null) || !__instance.Name.Equals(companion);
            if (!dontSkip)
            {
                object[] parameters = new object[] {__instance.currentLocation};
                ModEntry.applyVelocity.Invoke(__instance, parameters);
            }
            return dontSkip;
        }
        #endregion

        #region updateMovement

        static public bool Prefix(NPC __instance, GameLocation location, GameTime time)
        {
            bool dontSkip = (companion == null) || !__instance.Name.Equals(companion);
            return dontSkip;
        }

        #endregion

        #region faceTowardFarmerForPeriod

        static public bool dontFace;

        static public bool Prefix(NPC __instance)
        {
            if (dontFace && __instance.Name.Equals(companion))
                return false;
            return true;
        }
        #endregion

        #region gainExperience

        static public bool increaseExperience;
        static public string farmer;

        static public void Prefix(Farmer __instance, ref int howMuch)
        {
            if (increaseExperience && __instance.Name.Equals(farmer))
            {
                int expIncrease = Math.Max((int)(howMuch * 0.05f), 1);
                howMuch += expIncrease;
            }
        }

        #endregion
    }

    /// <summary>
    /// Harmony patche(s) that I use(d) for debug purposes. These are not (or should not) be
    /// used in any release build of this mod.
    /// </summary>
    class DebugPatches
    {
        static public void Prefix(GameLocation __instance, int waterDepth)
        {
            ModEntry.monitor.Log(waterDepth.ToString());
        }
    }

}

