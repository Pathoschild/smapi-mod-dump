using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Buildings;

namespace ChildToNPC
{
    /* To Do:
     * Let NPCs pathfind around the FarmHouse?
     * Let NPCs teleport to the spouse area, like spouses do?
     * Make gifts/talking configurable (how many points to talk, how many gifts per week) 
     * Multiple Dialogues in a day, like your spouse.
     * Making children collide the same way the farmer does? to avoid children walking through walls.
     */

    /* ChildToNPC is a modding tool which converts a Child to an NPC 
     * for the purposes of creating Content Patcher mods.
     * 
     * ChildToNPC creates an NPC which is outwardly identical to your child
     * and removes your child from the farmhouse during the day, effectively replacing them.
     * 
     * This version of Child To NPC is compatible with SMAPI 3.0/Stadew Valley 1.4.
     */

    /* This mod makes use of IContentPatcherAPI
     * The Content Patcher API allows mods to make their own tokens
     * to be used in the Content Patcher content packs.
     * In this case, my custom tokens are for the identities of the children being patched.
     * This allows modders to get access to child data.
     * Because the tokens return null when that child isn't available,
     * the patches will not be applied at all when the child isn't present,
     * which prevents new NPCs from being wrongfully generated.
     */

    /* This mod makes use of Harmony and patches the following methods:
     * NPC.arriveAtFarmHouse
     * NPC.checkSchedule
     * NPC.parseMasterSchedule
     * NPC.performTenMinuteUpdate
     * NPC.prepareToDisembarkOnNewSchedulePath
     * PathfindController.handleWarps
     * (These methods should only trigger for custom NPCs)
     */

    class ModEntry : Mod
    {
        //The age at which the NPC takes over
        public static int ageForCP;
        //Variables for this class
        public static Dictionary<string, NPC> copies;
        public static List<Child> children;
        public static Dictionary<string, string> children_parents;
        public static IMonitor monitor;
        public static IModHelper helper;
        public static ModConfig Config;
        public bool updateNeeded = true;

        public override void Entry(IModHelper helper)
        {
            //initialize variables
            monitor = Monitor;
            ModEntry.helper = helper;
            copies = new Dictionary<string, NPC>();
            children = new List<Child>();
            children_parents = new Dictionary<string, string>();
            //create config
            Config = helper.ReadConfig<ModConfig>();
            ageForCP = Config.AgeWhenKidsAreModified;
            if (Config.ModdingCommands)
            {
                helper.ConsoleCommands.Add("AddChild", "AddChild immediately triggers a naming event, adding a child to your home.", AddChild);
                helper.ConsoleCommands.Add("RemoveChild", "RemoveChild removes the named child from the farm.", RemoveChild);
                helper.ConsoleCommands.Add("AgeChild", "Ages the named child to toddler age.", AgeChild);
            }
            //Event handlers
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.OneSecondUpdateTicking += OnOneSecondUpdateTicking;
            //Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("Loe2run.ChildToNPC");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.arriveAtFarmHouse)),
                postfix: new HarmonyMethod(typeof(Patches.NPCArriveAtFarmHousePatch), nameof(Patches.NPCArriveAtFarmHousePatch.Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkSchedule)),
                prefix: new HarmonyMethod(typeof(Patches.NPCCheckSchedulePatch), nameof(Patches.NPCCheckSchedulePatch.Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "parseMasterSchedule"),
                prefix: new HarmonyMethod(typeof(Patches.NPCParseMasterSchedulePatch), nameof(Patches.NPCParseMasterSchedulePatch.Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.performTenMinuteUpdate)),
                prefix: new HarmonyMethod(typeof(Patches.NPCPerformTenMinuteUpdatePatch), nameof(Patches.NPCPerformTenMinuteUpdatePatch.Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "prepareToDisembarkOnNewSchedulePath"),
                postfix: new HarmonyMethod(typeof(Patches.NPCPrepareToDisembarkOnNewSchedulePathPatch), nameof(Patches.NPCPrepareToDisembarkOnNewSchedulePathPatch.Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(PathFindController), nameof(PathFindController.handleWarps)),
                prefix: new HarmonyMethod(typeof(Patches.PFCHandleWarpsPatch), nameof(Patches.PFCHandleWarpsPatch.Prefix))
            );
            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            IModInfo cpinfo = helper.ModRegistry.Get("Pathoschild.ContentPatcher");
        }

        /* OnDayStarted
         * Every time the game is saved, the children are re-added to the FarmHouse
         * So every morning, I check if there are children in the FarmHouse and remove them,
         * and I add their dopplegangers to the FarmHouse.
         */
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);

            int index = 1;

            foreach (Child child in farmHouse.getChildren())
            {
                //If the child just aged up/first time loading save
                if (child.daysOld >= ageForCP && children != null && !children.Contains(child))
                {
                    //Add child to list & remove from farmHouse
                    children.Add(child);
                    farmHouse.getCharacters().Remove(child);

                    //Set the parent for the child, from config or from default
                    foreach (string name in Config.ChildParentPairs.Keys)
                    {
                        if (child.Name.Equals(name))
                        {
                            Config.ChildParentPairs.TryGetValue(child.Name, out string parentName);
                            children_parents.Add(child.Name, parentName);
                        }
                    }
                    if (!children_parents.ContainsKey(child.Name))
                        children_parents.Add(child.Name, Game1.player.spouse);

                    //Create childCopy, add childCopy to list, add to farmHouse at random spot
                    Point openPoint = farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30);
                    Point defaultBedPoint = farmHouse.getChildBed(child.Gender);
                    defaultBedPoint = new Point(defaultBedPoint.X, defaultBedPoint.Y);

                    Vector2 location = openPoint == null ? new Vector2(openPoint.X * 64f, openPoint.Y * 64f) : new Vector2(defaultBedPoint.X * 64f, defaultBedPoint.Y * 64f);

                    Dictionary<string, string> dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                    if (dispositions.ContainsKey(child.Name))
                    {
                        string[] defaultPosition = dispositions[child.Name].Split('/')[10].Split(' ');
                        location = new Vector2(int.Parse(defaultPosition[1]) * 64f, int.Parse(defaultPosition[2]) * 64f);
                    }

                    //new NPC(new AnimatedSprite("Characters\\George", 0, 16, 32), new Vector2(1024f, 1408f), "JoshHouse", 0, "George", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\George"));
                    NPC childCopy = new NPC(child.Sprite, location, "FarmHouse", 2, child.Name, false, null, null) //schedule null, portrait null
                    {
                        DefaultMap = Game1.player.homeLocation.Value,
                        DefaultPosition = location,
                        Breather = false,
                        HideShadow = false,
                        Position = location,
                        displayName = child.Name
                    };

                    copies.Add(child.Name, childCopy);
                    farmHouse.addCharacter(childCopy);

                    //Check if I've made this NPC before & set gift info
                    try
                    {
                        NPCFriendshipData childCopyFriendship = helper.Data.ReadJsonFile<NPCFriendshipData>(helper.Content.GetActualAssetKey("assets/data_" + childCopy.Name + ".json", ContentSource.ModFolder));
                        if (childCopyFriendship != null)
                        {
                            Game1.player.friendshipData.TryGetValue(child.Name, out Friendship childFriendship);
                            childFriendship.GiftsThisWeek = childCopyFriendship.GiftsThisWeek;
                            childFriendship.LastGiftDate = new WorldDate(childCopyFriendship.GetYear(), childCopyFriendship.GetSeason(), childCopyFriendship.GetDay());
                        }
                    }
                    catch (Exception) { }
                }
                //If NPC was already generated previously
                else if (copies.ContainsKey(child.Name))
                {
                    //Remove child
                    farmHouse.getCharacters().Remove(child);

                    //Add copy at random location in the house
                    copies.TryGetValue(child.Name, out NPC childCopy);

                    childCopy.Position = childCopy.DefaultPosition;
                    farmHouse.addCharacter(childCopy);
                }

                index++;
            }
        }

        /* OnOneSecondUpdateTicking
         * This isn't ideal, it will keep trying to load the sprite if the mod fails,
         * but this is my current solution for executing this code after Content Patcher packs are ready.
         */
        private void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (updateNeeded && Context.IsWorldReady)
            {
                FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);

                foreach (NPC childCopy in copies.Values)
                {
                    //Try to load child sprite
                    try
                    {
                        childCopy.Sprite = new AnimatedSprite("Characters/" + childCopy.Name, 0, 16, 32);
                    }
                    catch (Exception) { }

                    //Try to load DefaultPosition from dispositions
                    try
                    {
                        Dictionary<string, string> dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                        if (dispositions.ContainsKey(childCopy.Name))
                        {
                            string[] defaultPosition = dispositions[childCopy.Name].Split('/')[10].Split(' ');
                            Vector2 location = new Vector2(int.Parse(defaultPosition[1]) * 64f, int.Parse(defaultPosition[2]) * 64f);

                            childCopy.Position = location;
                            childCopy.DefaultPosition = location;
                            farmHouse.characters.Remove(childCopy);
                            farmHouse.addCharacter(childCopy);
                        }
                    }
                    catch (Exception) { }
                }

                updateNeeded = false;
            }
        }

        /* OnSaving
         * When the game saves overnight, I add the child back to the FarmHouse.characters list
         * so that if the mod is uninstalled, the child is returned properly.
         * Additionally, I remove the child copy NPC for the same reason.
         * If the mod is uninstalled, the new NPC shouldn't be in the save data.
         * 
         * I save the Friendship data for the generated NPC here.
         * Otherwise, exiting the game would reset gift data.
         */
        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (NPC childCopy in copies.Values)
            {
                //Remove childcopy from save file first
                foreach (GameLocation location in Game1.locations)
                {
                    if (location.characters.Contains(childCopy))
                        location.getCharacters().Remove(childCopy);
                }
                //Check indoor locations for a child NPC
                foreach (BuildableGameLocation location in Game1.locations.OfType<BuildableGameLocation>())
                {
                    foreach (Building building in location.buildings)
                    {
                        if (building.indoors.Value != null && building.indoors.Value.characters.Contains(childCopy))
                            building.indoors.Value.getCharacters().Remove(childCopy);
                    }
                }

                //Save NPC Gift data
                Game1.player.friendshipData.TryGetValue(childCopy.Name, out Friendship friendship);
                if (friendship != null)
                {
                    if (friendship.LastGiftDate != null)//null when loading from Child for the first time
                    {
                        string lastGiftDate = friendship.LastGiftDate.DayOfMonth + " " + friendship.LastGiftDate.Season + " " + friendship.LastGiftDate.Year;
                        NPCFriendshipData childCopyData = new NPCFriendshipData(friendship.Points, friendship.GiftsThisWeek, lastGiftDate);
                        helper.Data.WriteJsonFile("assets/data_" + childCopy.Name + ".json", childCopyData);
                    }
                }
            }

            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);
            //Add children
            foreach (Child child in children)
            {
                if (!farmHouse.getCharacters().Contains(child))
                    farmHouse.addCharacter(child);
            }
        }

        /* OnReturnedToTitle
         * Returning to title and loading new save causes NPCs to load in the wrong save.
         * So this clears out the children list/copies dictionary on return to title.
         * (Children exist in the save data and NPCs don't,
         *  so this won't cause people to lose their children when reloading from save.)
         */
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            copies = new Dictionary<string, NPC>();
            children = new List<Child>();
            children_parents = new Dictionary<string, string>();
            updateNeeded = true;
        }

        /* AddChild, RemoveChild, AgeChild
         * These are for the console commands for testing.
         * This makes it easier to generate new children.
         * (They are definitely a little buggy, proceed with caution.)
         */
        private void AddChild(string arg1, string[] arg2)
        {
            Monitor.Log("Generating new child.");

            if (!Context.IsWorldReady)
                return;

            if (Game1.farmEvent == null)
            {
                Game1.farmEvent = new CustomEvent.CustomBirthingEvent();
                Game1.farmEvent.setUp();
            }
            else
            {
                Monitor.Log("Current Game1.farmEvent is not null.");
            }
        }

        private void RemoveChild(string arg1, string[] arg2)
        {
            string childName = arg2[0];
            Child c = (Child)Game1.getCharacterFromName(childName);
            if (c != null)
            {
                Game1.getLocationFromName("FarmHouse").getCharacters().Remove(c);
                Monitor.Log(childName + " has been removed.");
            }
            else
            {
                Monitor.Log("Lookup returned a null result.");
            }
        }

        private void AgeChild(string arg1, string[] arg2)
        {
            string childName = arg2[0];
            Child c = (Child)Game1.getCharacterFromName(childName);
            if (c != null)
            {
                if (c.daysOld.Value < 54)
                    c.daysOld.Value = 54;
                Monitor.Log(childName + " is now " + c.daysOld + " days old.");
            }
            else
            {
                Monitor.Log("Lookup returned a null result.");
            }
        }

        /* OnGameLaunched
         * This is where I set up the IContentPatcherAPI tokens.
         * Tokens are in the format of (Child Order)Child(Field)
         * I.e. The first child's name is FirstChildName,
         *      the third child's birthday is ThirdChildBirthday
         */
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IContentPatcherAPI api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (api == null)
                return;

            ChildToken token = new ChildToken(1);
            api.RegisterToken(
                mod: ModManifest,
                name: "NumberTotalChildren",
                updateContext: token.TotalChildrenUpdateContext,
                isReady: token.IsReady,
                getValue: token.TotalChildrenGetValue,
                allowsInput: false,
                requiresInput: false
            );

            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildBirthday",
                updateContext: token.BirthdayUpdateContext,
                isReady: token.IsReady,
                getValue: token.BirthdayGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildBed",
                updateContext: token.BedUpdateContext,
                isReady: token.IsReady,
                getValue: token.BedGetValue,
                allowsInput: true,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildGender",
                updateContext: token.GenderUpdateContext,
                isReady: token.IsReady,
                getValue: token.GenderGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildParent",
                updateContext: token.ParentUpdateContext,
                isReady: token.IsReady,
                getValue: token.ParentGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(2);
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildBirthday",
                updateContext: token.BirthdayUpdateContext,
                isReady: token.IsReady,
                getValue: token.BirthdayGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildBed",
                updateContext: token.BedUpdateContext,
                isReady: token.IsReady,
                getValue: token.BedGetValue,
                allowsInput: true,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildGender",
                updateContext: token.GenderUpdateContext,
                isReady: token.IsReady,
                getValue: token.GenderGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildParent",
                updateContext: token.ParentUpdateContext,
                isReady: token.IsReady,
                getValue: token.ParentGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(3);
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildBirthday",
                updateContext: token.BirthdayUpdateContext,
                isReady: token.IsReady,
                getValue: token.BirthdayGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildBed",
                updateContext: token.BedUpdateContext,
                isReady: token.IsReady,
                getValue: token.BedGetValue,
                allowsInput: true,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildGender",
                updateContext: token.GenderUpdateContext,
                isReady: token.IsReady,
                getValue: token.GenderGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildParent",
                updateContext: token.ParentUpdateContext,
                isReady: token.IsReady,
                getValue: token.ParentGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(4);
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildBirthday",
                updateContext: token.BirthdayUpdateContext,
                isReady: token.IsReady,
                getValue: token.BirthdayGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildBed",
                updateContext: token.BedUpdateContext,
                isReady: token.IsReady,
                getValue: token.BedGetValue,
                allowsInput: true,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildGender",
                updateContext: token.GenderUpdateContext,
                isReady: token.IsReady,
                getValue: token.GenderGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildParent",
                updateContext: token.ParentUpdateContext,
                isReady: token.IsReady,
                getValue: token.ParentGetValue,
                allowsInput: false,
                requiresInput: false
            );

            //I'm stopping at four for now.
            //If FamilyPlanning expands past four, I'll need to come back to this.
        }

        /* GetChildNPC(field) & GetBedSpot
         * These are for getting access to information for ContentPatcher tokens.
         */
        public static string GetChildNPCName(int childNumber)
        {
            if (children != null && children.Count >= childNumber && children[childNumber - 1].daysOld >= ageForCP)
                return children[childNumber - 1].Name;
            return null;
        }

        public static string GetChildNPCBirthday(int childNumber)
        {
            if (!Context.IsWorldReady)
                return null;

            if (children != null && children.Count >= childNumber && children[childNumber - 1].daysOld >= ageForCP)
            {
                SDate todaySDate = new SDate(Game1.dayOfMonth, Game1.currentSeason, Game1.year);
                SDate birthdaySDate = new SDate(1, "spring");
                try
                {
                    birthdaySDate = todaySDate.AddDays(-children[childNumber - 1].daysOld);
                }
                catch (ArithmeticException) { }

                return birthdaySDate.Season + " " + birthdaySDate.Day;
            }
            return null;
        }

        public static string GetChildNPCGender(int childNumber)
        {
            if (children != null && children.Count >= childNumber && children[childNumber - 1].daysOld >= ageForCP)
                return (children[childNumber - 1].Gender == 0) ? "male" : "female";
            return null;
        }

        public static string GetChildNPCParent(int childNumber)
        {
            if (children != null && children.Count >= childNumber && children[childNumber - 1].daysOld >= ageForCP)
            {
                children_parents.TryGetValue(children[childNumber - 1].Name, out string parentName);
                return parentName;
            }
            return null;
        }

        public static string GetTotalChildren()
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);

            if (farmHouse == null)
                return null;

            if (children != null)
                return (farmHouse.getChildrenCount() + children.Count).ToString();
            else
                return farmHouse.getChildrenCount().ToString();
        }

        public static string GetBedSpot(int birthNumber)
        {
            //This code is copied from Family Planning
            //This is how I determine whose bed is whose
            if (children == null)
                return null;

            int boys = 0;
            int girls = 0;
            int baby = 0;

            foreach (Child child in children)
            {
                if (child.daysOld >= ageForCP)
                {
                    if (child.Gender == 0)
                        boys++;
                    else
                        girls++;
                }
                else
                    baby++;
            }

            if (children.Count - baby < birthNumber)
                return null;

            Point childBed = new Point(23, 5);

            if (birthNumber != 1 && boys + girls <= 2)
            {
                childBed = new Point(27, 5);
            }
            else if (birthNumber != 1 && boys + girls > 2)
            {
                if (children[0].Gender == children[1].Gender)
                {
                    if (birthNumber == 2)
                        childBed = new Point(22, 5);
                    else if (birthNumber == 3)
                        childBed = new Point(27, 5);
                    else if (birthNumber == 4)
                        childBed = new Point(26, 5);
                }
                else
                {
                    if (birthNumber == 2)
                        childBed = new Point(27, 5);
                    else if (children[2].Gender == children[3].Gender)
                    {
                        if (birthNumber == 3)
                            childBed = new Point(26, 5);
                        else
                            childBed = new Point(22, 5);
                    }
                    else
                    {
                        if (birthNumber == 3)
                            childBed = new Point(22, 5);
                        else
                            childBed = new Point(26, 5);
                    }
                }
            }

            string result = "FarmHouse " + childBed.X + " " + childBed.Y;
            return result;
        }

        /* IsChildNPC
         * I only want to trigger Harmony patches when I'm applying the method to an NPC copy,
         * so this method verifies that the NPC in question is on my list.
         */
        public static bool IsChildNPC(Character c)
        {
            return (copies != null && copies.ContainsValue(c as NPC));
        }

        public static bool IsChildNPC(NPC npc)
        {
            return (copies != null && copies.ContainsValue(npc));
        }

        /* GetBirthOrder
         * Tells you the birth order of an NPC
         * (I'll want to go back and use this in other places)
         */
        public static int GetBirthOrder(NPC npc)
        {
            int birthNumber = 1;
            foreach (Child child in children)
            {
                if (child.Name.Equals(npc.Name))
                    return birthNumber;
                birthNumber++;
            }
            return -1;
        }

        /* GetFarmerParentId
         * Returns the parentId from the child given their NPC/Character copy
         */
        public static long GetFarmerParentId(Character c)
        {
            foreach(Child child in children)
            {
                if (child.Name.Equals(c.Name))
                {
                    return child.idOfParent.Value;
                }
            }
            return 0L;
        }
    }
}

// (These are some notes for myself about pathfinding.)

/* How does pathfinding work?
 * --------------------------
 * At the very top, in the Game1 class, the game executes Game1.performTenMinuteClockUpdate()
 * Inside of this method, the game executes an important piece of code.
 * -> foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
 * -> {
 * ->   location.performTenMinuteUpdate(Game1.timeOfDay);
 * ->   if (location is Farm)
 * ->     ((BuildableGameLocation) location).timeUpdate(10);
 * -> }
 * 
 * Important methods now:
 * GameLocation.performTenMinuteUpdate(Game1.timeOfDay)
 * BuildableGameLocation.timeUpdate(10);
 * 
 * GameLocation.performTenMinuteUpdate(Game1.timeOfDay)
 * ----------------------------------------------------
 * 
 * Inside of GameLocation.performTenMinuteUpdate(Game1.timeOfDay), we see this.
 * -> for (int index = 0; index < this.characters.Count; ++index)
 * -> {
 * ->   if (!this.characters[index].IsInvisible)
 * ->   {
 * ->     this.characters[index].checkSchedule(timeOfDay);
 * ->     this.characters[index].performTenMinuteUpdate(timeOfDay, this);
 * ->   }
 * -> }
 * 
 * So the GameLocation that a character is in finds that character, 
 * checks their schedule, then asks them to performTenMinuteUpdate.
 * 
 * NPC.checkSchedule(int timeOfDay)
 * ----------------------------
 * This method tries to get the schedule for this timeOfDay
 * and sets the PathfindController to the destination.
 * (It also handles what to do when a character is "running late")
 * 
 * An important method inside of checkSchedule is NPC.prepareToDisembarkOnNewSchedulePath();
 * This sets the temporary controller to escort the NPC out of the FarmHouse
 * and sets the controller and schedule to null if the character is on the Farm.
 * 
 * NPC.performTenMinuteUpdate(int timeOfDay, GameLocation l)
 * -------------------------------------
 * (I'm not super confident here, but it looks like NPC.performTenMinuteUpdate() only generates SayHi dialogue?)
 * 
 * BuildableGameLocation.timeUpdate(10)
 * ------------------------------------
 * 
 * After the GameLocations perform their tenMinuteUpdate, the Farm executes timeUpdate(10).
 * The Farm goes through every AnimalHouse is has, and executes for each animal
 * FarmAnimal.updatePerTenMinutes(Game1.timeOfDay, ...building.indoors);
 * 
 * ----------------------------------------------------------------------------------------------------------------
 * Another factor in what's going on is that when the day starts,
 * the game executes NPC.dayUpdate(int dayOfMonth) for each character.
 * 
 * In this method, the game runs NPC.getSchedule(int dayOfMonth) to set the NPC.Schedule/NPC.schedule.
 * NPC.getSchedule(int dayOfMonth)
 */
