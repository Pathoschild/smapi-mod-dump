using System;
using StardewModdingAPI;
using StardewValley;

namespace StardewWeb
{
    internal static class I18n
    {
        private static ITranslationHelper Translations;
        public static void Init(ITranslationHelper translations)
        {
            Translations = translations;
        }

        //
        //  translations
        //

        //
        //  web messages
        //
        public static string WM_Achieve_Unknown(string sUnknown)
        {
            return GetByKey("wm.achieve.unknown", new { key = sUnknown });
        }
        public static string WM_Achieve_PreReq(string sPreReq)
        {
            return GetByKey("wm.achieve.prereq", new { achieve = sPreReq });
        }
        public static string WM_Mods_PFM()
        {
            return GetByKey("wm.mods.pfmmod");
        }
        public static string WM_Mods_JA_Clothing()
        {
            return GetByKey("wm.mods.ja.clothing");
        }
        public static string WM_Mods_JA_Weapons()
        {
            return GetByKey("wm.mods.ja.weapons");
        }
        public static string WM_Mods_JA_Hats()
        {
            return GetByKey("wm.mods.ja.hats");
        }
        public static string WM_Mods_JA_Trees()
        {
            return GetByKey("wm.mods.ja.trees");
        }
        public static string WM_Mods_JA_Other()
        {
            return GetByKey("wm.mods.ja.other");
        }
        public static string WM_Mod_JA_BigCraftable()
        {
            return GetByKey("wm.mods.ja.bc");
        }
        public static string WM_Mod_JA_Objects()
        {
            return GetByKey("wm.mods.ja.objects");
        }
        public static string WM_Mod_JA_Crops()
        {
            return GetByKey("wm.mods.ja.crops");
        }
        public static string WM_Mod_JA()
        {
            return GetByKey("wm.mods.jamod");
        }
        public static string WM_Calendar_Birthday(string sNPCName)
        {
            return GetByKey("wm.calendar.birthday", new { NPC = sNPCName });
        }
        public static string WM_EventType_TravellingCart()
        {
            return GetByKey("wm.eventtype.cart");
        }
        public static string WM_EventType_SalmonberrySeason()
        {
            return GetByKey("wm.eventtype.sberry");
        }
        public static string WM_EventType_Other()
        {
            return GetByKey("wm.eventtype.other");
        }
        public static string WM_EventType_Festival()
        {
            return GetByKey("wm.eventtype.festival");
        }
        public static string WM_EventType_BlackberrySeason()
        {
            return GetByKey("wm.eventtype.bberry");
        }
        public static string WM_EventType_Birthday()
        {
            return GetByKey("wm.eventtype.bday");
        }
        public static string WM_Calendar_Salmonberry()
        {
            return GetByKey("wm.calendar.salmonberry");
        }
        public static string WM_Calendar_Blackberry()
        {
            return GetByKey("wm.calendar.blackberry");
        }
        public static string WM_Calendar_Cart()
        {
            return GetByKey("wm.calendar.cart");
        }
        public static string WM_Search_NoReqs()
        {
            return GetByKey("wm.search.noreqs");
        }
        public static string WM_Search_Query(string sQuery)
        {
            return GetByKey("wm.search.query", new { query = sQuery });
        }
        public static string WM_Search_Name(string sName)
        {
            return GetByKey("wm.search.name", new { name = sName });
        }
        public static string WM_Search_Type(string sType)
        {
            return GetByKey("wm.search.type", new { type = sType });
        }
        public static string WM_Search_Cat(string sCat)
        {
            return GetByKey("wm.search.category", new { category = sCat });
        }
        public static string WM_Search_Tree(string sProd,string sSeason)
        {
            return GetByKey("wm.search.tree", new { crop = sProd, season = sSeason });
        }
        public static string WM_Search_Planted(string sSeason)
        {
            return GetByKey("wm.search.planted", new { season = sSeason });
        }
        public static string WM_Search_Nothing()
        {
            return GetByKey("wm.search.nothing");
        }
        public static string WM_Search_Price(int iPrice)
        {
            return GetByKey("wm.search.sells", new { price = iPrice });
        }
        public static string WM_Search_Value(int iIncrease)
        {
            return GetByKey("wm.search.value", new { increase = iIncrease });
        }
        public static string WM_Search_Edible(int iHealth, int iEnergy)
        {
            return GetByKey("wm.search.edible", new { health = iHealth, energy = iEnergy });
        }
        public static string WM_AddToInvent_Full()
        {
            return GetByKey("wm.additem.full");
        }
        public static string WM_AddToInvent_Okay(string sProdName,int iQty)
        {
            return GetByKey("wm.additem.okay", new { prod = sProdName, qty = iQty });
        }
        public static string WM_Tasks_NoQuests()
        {
            return GetByKey("dailytasks.noquests");
        }
        public static string WM_CacheCleared()
        {
            return GetByKey("wm.cachecleared");
        }
        public static string WM_WarpDisabled()
        {
            return GetByKey("wm.enablewarps");
        }
        public static string WM_PotentialEventsEnabled()
        {
            return GetByKey("wm.potential.notenabled");
        }
        public static string WM_AnyWeather()
        {
            return GetByKey("wm.weather.any");
        }
        //
        //  navbar captions
        //
        public static string NavBarBigCraftable()
        {
            return GetByKey("navbar.bigcraftable");
        }
        public static string NavBarArtisan()
        {
            return GetByKey("navbar.artisan");
        }
        public static string NavBarCrafting()
        {
            return GetByKey("navbar.crafting");
        }
        public static string NavBarWeapons()
        {
            return GetByKey("navbar.weapons");
        }
        public static string NavBarTools()
        {
            return GetByKey("navbar.tools");
        }
        public static string NavBarVegetables()
        {
            return GetByKey("navbar.vegetables");
        }
        public static string NavBarFruits()
        {
            return GetByKey("navbar.fruits");
        }
        public static string NavBarFlowers()
        {
            return GetByKey("navbar.flowers");
        }
        public static string NavBarFish()
        {
            return GetByKey("navbar.fish");
        }
        public static string NavBarMachines()
        {
            return GetByKey("navbar.machines");
        }
        public static string NavBarPlanning()
        {
            return GetByKey("navbar.planning");
        }
        public static string NavBarXNB()
        {
            return GetByKey("navbar.xnb");
        }
        public static string NavBarMap()
        {
            return GetByKey("navbar.map");
        }
        public static string NavBarConsole()
        {
            return GetByKey("navbar.console");
        }
        public static string NavBarFarm()
        {
            return GetByKey("navbar.farm");
        }
        public static string NavBarFarmer()
        {
            return GetByKey("navbar.farmer");
        }

        public static string NavBarHome()
        {
            return GetByKey("navbar.home");
        }
        public static string NavBarTasks()
        {
            return GetByKey("navbar.tasks");
        }
        public static string NavBarCrops()
        {
            return GetByKey("navbar.crops");
        }
        public static string NavBarLivestock()
        {
            return GetByKey("navbar.livestock");
        }
        public static string NavBarProduction()
        {
            return GetByKey("navbar.production");
        }
        public static string NavBarChestMan()
        {
            return GetByKey("navbar.chestman");
        }
        public static string NavBarInventory()
        {
            return GetByKey("navbar.inventory");
        }
        public static string NavBarLocations()
        {
            return GetByKey("navbar.locations");
        }
        public static string NavBarShipping()
        {
            return GetByKey("navbar.shipping");
        }
        public static string NavBarSocial()
        {
            return GetByKey("navbar.social");
        }
        public static string NavBarCollections()
        {
            return GetByKey("navbar.collections");
        }
        public static string NavBarBundles()
        {
            return GetByKey("navbar.bundles");
        }
        public static string NavBarQuests()
        {
            return GetByKey("navbar.quests");
        }
        public static string NavBarStats()
        {
            return GetByKey("navbar.stats");
        }
        public static string NavBarMods()
        {
            return GetByKey("navbar.mods");
        }
        public static string NavBarLogs()
        {
            return GetByKey("navbar.logs");
        }
        public static string NavBarAbout()
        {
            return GetByKey("navbar.about");
        }
        public static string NavBarMultiPlayer()
        {
            return GetByKey("navbar.multi");
        }
        public static string NavBarModderUtils()
        {
            return GetByKey("navbar.modderutils");
        }
        public static string NavBarWarps()
        {
            return GetByKey("navbar.warps");
        }
        public static string NavBarDailyPic()
        {
            return GetByKey("navbar.dailypic");
        }
        public static string NavBarEvents()
        {
            return GetByKey("navbar.events");
        }

        //
        //  preconditions
        //
        public static string Pre_Pass()
        {
            return GetByKey("precon.pass");
        }
        public static string Pre_Fail()
        {
            return GetByKey("precon.fail");
        }
        public static string Pre_Any()
        {
            return GetByKey("precon.any");
        }
        public static string Pre_Location(string sValue)
        {
            return GetByKey("precon.location", new { l = sValue });
        }
        public static string Pre_sunny()
        {
            return GetByKey("precon.sunny");
        }
        public static string Pre_rainy()
        {
            return GetByKey("precon.rainy");
        }
        public static string Pre_or()
        {
            return GetByKey("precon.or");
        }
        public static string Pre_a(string x, string y)
        {
            return GetByKey("precon.a", new { x = x, y = y });
        }
        public static string Pre_a_Maybe(string x, string y)
        {
            return GetByKey("precon.a.maybe", new { x = x, y = y });
        }
        public static string Pre_b (string sValue){
            return GetByKey("precon.b", new { b = sValue });
        }
        public static string Pre_c(string sValue)
        {
            return GetByKey("precon.c", new { c = sValue });
        }
        public static string Pre_c_Maybe(string sValue)
        {
            return GetByKey("precon.c.maybe", new { c = sValue });
        }
        public static string Pre_d(string sValue)
        {
            return GetByKey("precon.d", new { d = sValue });
        }
        public static string Pre_e(string sValue)
        {
            return GetByKey("precon.e", new { e = sValue });
        }
        public static string Pre_f_fail(string sValue)
        {
            return GetByKey("precon.f.fail", new { f = sValue });
        }
        public static string Pre_f_pass(string sValue)
        {
            return GetByKey("precon.f.pass", new { f = sValue });
        }
        public static string Pre_g(string sValue)
        {
            return GetByKey("precon.g", new { g = sValue });
        }
        public static string Pre_i(string sValue)
        {
            return GetByKey("precon.i", new { i = sValue });
        }
        public static string Pre_j(string sValue)
        {
            return GetByKey("precon.j", new { j = sValue });
        }
        public static string Pre_k(string sValue)
        {
            return GetByKey("precon.k", new { k = sValue });
        }
        public static string Pre_l(string sValue)
        {
            return GetByKey("precon.l", new { l = sValue });
        }
        public static string Pre_m(string sValue)
        {
            return GetByKey("precon.m", new { m = sValue });
        }
        public static string Pre_n(string sValue)
        {
            return GetByKey("precon.n", new { n = sValue });
        }
        public static string Pre_o(string sValue)
        {
            return GetByKey("precon.o", new { o = sValue });
        }
        public static string Pre_p(string sValue)
        {
            return GetByKey("precon.p", new { p = sValue });
        }
        public static string Pre_p_Maybe(string sValue,string sValue1)
        {
            return GetByKey("precon.p", new { p = sValue,l=sValue1 });
        }
        public static string Pre_q(string sValue)
        {
            return GetByKey("precon.q", new { q = sValue });
        }
        public static string Pre_s(string sValue)
        {
            return GetByKey("precon.s", new { s = sValue });
        }
        public static string Pre_t(string mintime, string maxtime)
        {
            return GetByKey("precon.t", new { min = mintime, max = maxtime });
        }
        public static string Pre_t_Maybe(string mintime, string maxtime)
        {
            return GetByKey("precon.t.maybe", new { min = mintime, max = maxtime });
        }
        public static string Pre_u(string sValue)
        {
            return GetByKey("precon.u", new { u = sValue });
        }
        public static string Pre_v(string sValue)
        {
            return GetByKey("precon.v", new { v = sValue });
        }
        public static string Pre_x(string sValue)
        {
            return GetByKey("precon.x", new { x = sValue });
        }
        public static string Pre_y(string sValue)
        {
            return GetByKey("precon.y", new { y = sValue });
        }
        public static string Pre_z(string sValue)
        {
            return GetByKey("precon.z", new { z = sValue });
        }
        public static string Pre_A(string sValue)
        {
            return GetByKey("precon.cA", new { A = sValue });
        }
        public static string Pre_C()
        {
            return GetByKey("precon.cC");
        }
        public static string Pre_D(string sValue)
        {
            return GetByKey("precon.cD", new { D = sValue });
        }
        public static string Pre_F()
        {
            return GetByKey("precon.cF");
        }
        public static string Pre_H()
        {
            return GetByKey("precon.cH");
        }
        public static string Pre_Hl(string sValue)
        {
            return GetByKey("precon.cHl", new { Hl = sValue });
        }
        public static string Pre_Hn(string sValue)
        {
            return GetByKey("precon.cHn", new { Hn = sValue });
        }
        public static string Pre_M(string sValue)
        {
            return GetByKey("precon.cM", new { M = sValue });
        }
        public static string Pre_M_Maybe(string sValue)
        {
            return GetByKey("precon.cM.maybe", new { M = sValue });
        }
        public static string Pre_O(string sValue)
        {
            return GetByKey("precon.cO", new { O = sValue });
        }
        public static string Pre_S(string sValue)
        {
            return GetByKey("precon.cS", new { S = sValue });
        }
        public static string Pre_U(string sValue)
        {
            return GetByKey("precon.cU", new { U = sValue });
        }
        public static string Pre__l(string sValue)
        {
            return GetByKey("precon._l", new { l = sValue });
        }
        public static string Pre__n(string sValue)
        {
            return GetByKey("precon._n", new { n = sValue });
        }

        //
        //  error messages
        //
        public static string Error_SeeLog()
        {
            return GetByKey("erro.seelog");
        }
        public static string PluginError()
        {
            return GetByKey("plugin.error");
        }
        public static string OldPlugin()
        {
            return GetByKey("plugin.old");
        }
        public static string GameNotLoaded()
        {
            return GetByKey("gamenotloaded");
        }
        public static string Farmer_Status_Married()
        {
            return GetByKey("farmer.status.married");
        }
        public static string Farmer_Status_Divorced()
        {
            return GetByKey("farmer.status.divorced");
        }
        public static string Farmer_Status_Engaged()
        {
            return GetByKey("farmer.status.engaged");
        }
        public static string Farmer_Status_Single()
        {
            return GetByKey("farmer.status.single");
        }
        public static string Farmer_Friendship_Status(FriendshipStatus fsValue, bool bNPCIsMale)
        {
            switch (fsValue)
            {
                case FriendshipStatus.Dating:
                    return bNPCIsMale ? GetByKey("farmer.friendship.dating.boyfriend") : GetByKey("farmer.friendship.dating.girlfriend");
                case FriendshipStatus.Divorced:
                    return GetByKey("farmer.friendship.divorced");
                case FriendshipStatus.Engaged:
                    return GetByKey("farmer.friendship.engaged");
                case FriendshipStatus.Friendly:
                    return GetByKey("farmer.friendship.friendly");
                case FriendshipStatus.Married:
                    return GetByKey("farmer.friendship.married");
                default:
                    return "";
            }
        }
        public static string Farmer_Female()
        {
            return GetByKey("farmer.gender.female");
        }
        public static string Farmer_Male()
        {
            return GetByKey("farmer.gender.male");
        }

        //
        //  answers
        //
        public static string Answer_Yes()
        {
            return GetByKey("answer.yes");
        }
        public static string Answer_No()
        {
            return GetByKey("answer.no");
        }
        //
        //  skills
        //
        public static string Skills_Farming()
        {
            return GetByKey("skills.farming");
        }
        public static string Skills_Mining()
        {
            return GetByKey("skills.mining");
        }
        public static string Skills_Foraging()
        {
            return GetByKey("skills.foraging");
        }
        public static string Skills_Fishing()
        {
            return GetByKey("skills.fishing");
        }
        public static string Skills_Combat()
        {
            return GetByKey("skills.combat");
        }
        //
        //  tool tips
        //
        public static string TT_CanUnderstandDwarves()
        {
            return GetByKey("tooltip.canunderstanddwarves");
        }
        public static string TT_HasRustyKey()
        {
            return GetByKey("tooltip.hasrustykey");
        }
        public static string TT_HasMagnifyingGlass()
        {
            return GetByKey("tooltip.hasmagnifyingglass");
        }
        public static string TT_HasClubCard()
        {
            return GetByKey("tooltip.hasclubcard");
        }
        public static string TT_HasSpecialCharm()
        {
            return GetByKey("tooltip.hasspecialcharm");
        }
        public static string TT_HasSkullKey()
        {
            return GetByKey("tooltip.hasskullkey");
        }
        public static string TT_HasDarkTalisman()
        {
            return GetByKey("tooltip.hasdarktalisman");
        }
        public static string TT_HasJojaCard()
        {
            return GetByKey("tooltip.hasjojacard");
        }
        public static string TT_HasMagicInk()
        {
            return GetByKey("tooltip.hasmagicink");
        }
        public static string TT_HasBearPaw()
        {
            return GetByKey("tooltip.hasbearpaw");
        }
        public static string TT_SpringOnionBugs()
        {
            return GetByKey("tooltip.springonionbugs");
        }
        public static string TT_HasTownKey()
        {
            return GetByKey("tooltip.hastownkey");
        }
        //
        //  task reporting
        //
        public static string Tasks_LastDay()
        {
            return GetByKey("tasks.misc.lastday");
        }
        public static string Tasks_NoHay()
        {
            return GetByKey("tasks.misc.nohay");
        }
        public static string Tasks_Hay(int iCount)
        {
            return GetByKey("tasks.misc.hay",new { Count = iCount.ToString() });
        }
        public static string Tasks_NothingToDo()
        {
            return GetByKey("tasks.nothingtodo");
        }
        public static string Tasks_Object_Silver()
        {
            return GetByKey("tasks.object.silver");
        }
        public static string Tasks_Object_Gold()
        {
            return GetByKey("tasks.object.gold");
        }
        public static string Tasks_Object_Iridium()
        {
            return GetByKey("tasks.object.iridium");
        }
        public static string Tasks_Object_With()
        {
            return GetByKey("tasks.object.with");
        }
        public static string Tasks_At()
        {
            return GetByKey("tasks.at");
        }
        public static string Tasks_Pet_Pet()
        {
            return GetByKey("tasks.pet.pet");
        }
        public static string Tasks_Pet_Bowl()
        {
            return GetByKey("tasks.pet.bowl");
        }
        public static string Tasks_Terrain_Fence()
        {
            return GetByKey("tasks.terrain.fence");
        }
        public static string Tasks_Terrain_Damaged()
        {
            return GetByKey("tasks.terrain.damaged");
        }
        public static string Tasks_Complete()
        {
            return GetByKey("tasks.complete");
        }
        public static string Tasks_Object_Crab()
        {
            return GetByKey("tasks.object.crab");
        }
        public static string Tasks_Object_CrabPot()
        {
            return GetByKey("tasks.object.crabpot");
        }
        public static string Tasks_Object_Machine()
        {
            return GetByKey("tasks.object.machine");
        }
        public static string Tasks_Misc_Queen()
        {
            return GetByKey("tasks.misc.queen");
        }
        public static string Tasks_Misc_Birthday(string sNPCName)
        {
            return GetByKey("tasks.misc.birthday", new { NPCName = sNPCName });
        }
        public static string Tasks_Misc_Merchant()
        {
            return GetByKey("tasks.misc.merchant");
        }
        public static string Tasks_Cave_InCave()
        {
            return GetByKey("tasks.cave.incave");
        }
        public static string Tasks_Cave_ItemName_Fruits()
        {
            return GetByKey("tasks.cave.itemname.fruits");
        }
        public static string Tasks_Cave_ItemName_Mushrooms()
        {
            return GetByKey("tasks.cave.itemname.mushrooms");
        }
        public static string Tasks_Crop_NotWatered()
        {
            return GetByKey("tasks.crop.notwatered");
        }
        public static string Tasks_Crop_ReadyToHarvest()
        {
            return GetByKey("tasks.crop.readytoharvest");
        }
        public static string Tasks_Crop_Dead()
        {
            return GetByKey("tasks.crop.dead");
        }
        public static string Tasks_Crop_Fruit()
        {
            return GetByKey("tasks.crop.fruit");
        }
        public static string Tasks_Crop_Label_Unwatered()
        {
            return GetByKey("tasks.crop.label.unwatered");
        }
        public static string Tasks_Crop_Label_Harvest()
        {
            return GetByKey("tasks.crop.label.harvest");
        }
        public static string Tasks_Crop_Label_Crops()
        {
            return GetByKey("tasks.crop.label.crops");
        }
        public static string Tasks_Crop_Label_Fruit()
        {
            return GetByKey("tasks.crop.label.fruits");
        }
        public static string Tasks_Crop_TreeAt()
        {
            return GetByKey("tasks.crop.treeat");
        }
        public static string Tasks_Crop_With()
        {
            return GetByKey("tasks.crop.with");
        }
        public static string Tasks_Crop_WithFruit()
        {
            return GetByKey("tasks.crop.withfruit");
        }
        public static string Tasks_Crop_WithFruits()
        {
            return GetByKey("tasks.crop.withfruits");
        }
        public static string Tasks_Animal_NotPetted()
        {
            return GetByKey("tasks.animal.notpettedanimals");
        }
        public static string Tasks_Animal_Uncollected()
        {
            return GetByKey("tasks.animal.uncollectedanimalproduct");
        }
        public static string Tasks_Animal_EmptyHay()
        {
            return GetByKey("tasks.animal.emptyhay");
        }
        public static string Tasks_Animal_Has()
        {
            return GetByKey("tasks.animal.has");
        }
        public static string Tasks_Animal_MissingHay()
        {
            return GetByKey("tasks.animal.missinghay");
        }
        public static string Tasks_Animal_MissingHays()
        {
            return GetByKey("tasks.animal.missinghays");
        }
        public static string Tasks_Ponds_Attention()
        {
            return GetByKey("tasks.animal.ponds.attention");
        }
        public static string Tasks_Ponds_Collect()
        {
            return GetByKey("tasks.animal.ponds.collect");
        }
        public static string Prod_Egg_Brown()
        {
            return GetByKey("prod.egg.brown");
        }
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
    }

}
