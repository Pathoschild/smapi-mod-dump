/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SpaceCore.Events;
using SpaceCore.Interface;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using Object = StardewValley.Object;
using System.Reflection;
using System.Threading;
using StardewValley.Menus;
using StardewValley.Locations;
using System.Xml.Linq;
using System.Runtime.Intrinsics.X86;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using SpaceCore.UI;
using StardewValley.Tools;
using StardewValley.Events;
using System.Timers;
using StardewValley.Buffs;
using System.Net.Mail;

namespace NuclearBombLocations
{
    public partial class Mod : StardewModdingAPI.Mod
    {
        //public readonly StardewValley.Object siren = (Object)ItemRegistry.Create("ApryllForever.NuclearBombCP_SirenSnack");

        public static Mod instance;

        internal static IMonitor ModMonitor { get; set; }

        internal static IModHelper ModHelper { get; set; }

        private readonly string PublicAssetBasePath = "Mods/SlimeTent";

        private float submergeTimer;

        private Texture2D submarineSprites;

        // public static Random myRand;

        public override void Entry(IModHelper helper)
        {
            instance = this;

            //var sc = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            Helper.Events.Specialized.LoadStageChanged += OnLoadStageChanged;

            Helper.Events.Content.AssetRequested += OnAssetRequested;

            //Helper.Events.GameLoop.DayEnding += OnDayEnding;

            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            Helper.Events.Player.Warped += OnWarped;

            SpaceEvents.OnItemEaten += OnItemEaten;

            SpaceEvents.BeforeWarp += BeforeWarped;



            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
             original: AccessTools.Method(typeof(StardewValley.Menus.CarpenterMenu), nameof(StardewValley.Menus.CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
             postfix: new HarmonyMethod(typeof(Mod), nameof(Mod.ReturnMenu_Prefix))
          ); 

            harmony.Patch(
           original: AccessTools.Method(typeof(StardewValley.NPC), nameof(StardewValley.NPC.isAdoptionSpouse)),
           prefix: new HarmonyMethod(typeof(Mod), nameof(Mod.NPC__isGaySpouse__Postfix))
        );
            /*

            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.pickPersonalFarmEvent)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Utility_pickPersonalFarmEvent_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.QuestionEvent_setUp_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.setUp)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.BirthingEvent_setUp_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.tickUpdate)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.BirthingEvent_tickUpdate_Prefix))
            );  */



            harmony.PatchAll();

            HarmonyPatch_UntimedSpecialOrders.ApplyPatch(harmony, helper, Monitor);



          
        }


        public static bool NPC__isGaySpouse__Postfix(StardewValley.NPC __instance, ref bool __result)
        {
            if (!__instance.Name.Equals("MermaidRangerMarisol"))
                return true;

            Farmer spouse = __instance.getSpouse();
            if (spouse.IsMale)
                __result = true;

            else if (spouse != null)
                __result = false;


            return false;

        }



        
        private static void ReturnMenu_Prefix()
        {
            // Not our place, we don't care
            if (Game1.player.currentLocation is not ClairabelleLagoon)
            {
                return;
            }
            else
            {

                LocationRequest locationRequest = Game1.getLocationRequest("Custom_ClairabelleLagoon");
                locationRequest.OnWarp += delegate
                {
                    Game1.displayHUD = true;
                    Game1.player.viewingLocation.Value = null;
                    Game1.viewportFreeze = false;
                    Game1.viewport.Location = new Location(320, 1536);
                    Game1.player.freeze = true;                                 
                    Game1.displayFarmer = true;
                    AnabelleConstructionMessage();
                };
                Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);

            }

        }


        public static void AnabelleConstructionMessage()
        {
            CarpenterMenu menu = new("MermaidRangerAnabelle");

            menu.exitThisMenu();



            Game1.player.forceCanMove();


            string text = "Strings\\StringsFromCSFiles:Annabelle.ConstructionBegin";


            string text2 = "Your work will begin tomorrow!!";


            //string[] array = ArgUtility.SplitBySpace(Blueprint.DisplayName);
            string text3 = "Your work will begin tomorrow!!!";



            Game1.DrawDialogue(Game1.getCharacterFromName("MermaidRangerAnabelle"), text);
        }


        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
           

            Mod.ModHelper = Helper;
            Mod.ModMonitor = Monitor;
       


            var sc = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

            sc.RegisterSerializerType(typeof(NuclearLocation));

            sc.RegisterSerializerType(typeof(ClairabelleLagoon));

            sc.RegisterSerializerType(typeof(MermaidDugoutHouse));

            sc.RegisterSerializerType(typeof(SlimeTent));

            sc.RegisterSerializerType(typeof(NuclearSubmarinePen));

            sc.RegisterSerializerType(typeof(MermaidNuclearSub));

            sc.RegisterSerializerType(typeof(AtarraMountainTop));

            sc.RegisterSerializerType(typeof(NuclearSunkenShip));

            //sc.RegisterCustomProperty(typeof(Building), "[XmlInclude(typeof(SlimeTent))]", typeof(NetRef<SlimeTent>), AccessTools.Method(typeof(SlimeTent), nameof(SlimeTent.get_SlimeTent)), AccessTools.Method(typeof(SlimeTent), nameof(SlimeTent.set_SlimeTent)));

            // var spacecore = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

           // spacecore.AddEventCommand("NuclearMermaidSubmarineDive", PatchHelper.RequireMethod<Mod>(nameof(Mod.EventCommand_NuclearMermaidSubmarineDive)));


            
            Event.RegisterCommand("NuclearMermaidSubmarineDive", delegate
            {
                MermaidNuclearSub mermaidNuclearSub = new MermaidNuclearSub();
                mermaidNuclearSub.answerDialogueAction("SubmergeQuestion_Yes", LegacyShimsEvil.EmptyArray<string>());


            }
                //Event.RegisterCommand("NuclearMermaidSubmarineDiving", delegate
                //{

                //}

                );

            Event.RegisterCommand("NuclearMermaidSparkles", delegate (Event @event, String[] args, EventContext context)
            {
               

                Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.DarkViolet, 10, 1000));
                @event.CurrentCommand++;
               


            }
            
                );


        }



        private void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.CreatedInitialLocations || e.NewStage == LoadStage.SaveAddedLocations)
            {
                Game1.locations.Add(new ClairabelleLagoon(Helper.ModContent));
                Game1.locations.Add(new SlimeTent(Helper.ModContent));
                Game1.locations.Add(new MermaidDugoutHouse(Helper.ModContent));
                Game1.locations.Add(new NuclearSubmarinePen(Helper.ModContent));
                Game1.locations.Add(new MermaidNuclearSub(Helper.ModContent));
                Game1.locations.Add(new AtarraMountainTop(Helper.ModContent));
                Game1.locations.Add(new NuclearSunkenShip(Helper.ModContent));
            }

        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
          

        }

        private void OnItemEaten(object sender, EventArgs e)
        {

            if (sender != Game1.player)
                return;

            //var siren = "ApryllForever.NuclearBomb/Sirenated";

            //StardewValley.Object siren = new Object();



            if (Game1.player.itemToEat.ItemId == "ApryllForever.NuclearBombCP_SirenSnack")
            //if (e.Equals("ApryllForever.NuclearBombCP_SirenSnack"))

            {

                Buff buff = new Buff(
                id: "ApryllForever.NuclearBomb/Sirenated",
                displayName: "Siren's Kiss", // can optionally specify description text too
                description: "You feel the Kiss of the Goddess Vedyava flowing through your body! Your feet are light, your soul fiercer, and your fortune greater!",
                iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/Sirenated.png"),
                iconSheetIndex: 0,
                duration: 93_000, // 93 seconds
                effects: new BuffEffects()
                     {
                        Speed = { 3 }, // shortcut for buff.Speed.Value = 3
                        CombatLevel = { 23 },
                        Attack = { 3 },

                      }
                                 );
                Game1.player.applyBuff(buff);
            }

            if (Game1.player.itemToEat.ItemId == "ApryllForever.NuclearBombCP_FairySnack")
            {
                Buff buff = new Buff(
        id: "ApryllForever.NuclearBomb/Fairyinated",
        displayName: "Fairy's Kiss", // can optionally specify description text too
        description: "You feel the Kiss of Epona flowing through your heart! You ",
        iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/Sirenated.png"),
        iconSheetIndex: 1,
        duration: 93_000, // 93 seconds
        effects: new BuffEffects()
                {
            Speed = { 3 }, // shortcut for buff.Speed.Value = 3
            LuckLevel = { 3 },
            ForagingLevel = { 3 },

                }
            );
                Game1.player.applyBuff(buff);
            };

            if (Game1.player.itemToEat.ItemId == "ApryllForever.NuclearBombCP_VampyreSnack")
         
            {
                Buff buff3 = new Buff(
        id: "ApryllForever.NuclearBomb/Vampirinated",
        displayName: "Vampyre's Kyss", // can optionally specify description text too
        description: "Myst fills the your eyes as the Kiss of Hekate fills you, the joyful dread of those of both in the realm of the living and dead... ",
        iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/Sirenated.png"),
        iconSheetIndex: 2,
    
        duration: 93_000, // 93 seconds
        
        effects: new BuffEffects()
        {
            Speed = { 6 }, // shortcut for buff.Speed.Value = 3
            LuckLevel = { 3 },
            Attack = { 37 },
            Defense = { -23 },
            MaxStamina = { -43 },
            CombatLevel = { 13 },

            

        }
                                     );

                Game1.player.applyBuff(buff3);
                Game1.player.glowingColor = Color.Indigo;
               
                Game1.ambientLight = Color.OrangeRed;
                Game1.ambientLight = new Color(210, 40, 0) * 120;
                Game1.player.stamina -= 97;
            }
        }
        private void OnUpdateTicked(object sender, EventArgs e)
        {

        }

        public static void EventCommand_NuclearMermaidSubmarineDive(Event instance, GameLocation location, GameTime time, string[] split)
        {


            instance.CurrentCommand++;

            ++instance.CurrentCommand;
        }

        public void BeforeWarped(object sender, EventArgsBeforeWarp e)
        {
            string Stop = "You may not go through this door. Marisol does not know if you can be trusted yet enough to enter here. Only those closest to her may go here.";

            if (e.WarpTargetLocation.Name.Equals("Custom_NuclearSubmarinePen") && !Game1.player.mailReceived.Contains("NuclearBombCP.Marisol10HeartInvite"))
            {
                Game1.drawObjectDialogue(Stop);
                e.Cancel = true;

            }
       
        }

        public void OnWarped(object sender, WarpedEventArgs e)
        {
            // if (e.OldLocation is MermaidDugoutHouse && e.NewLocation is NuclearSubmarinePen && !Game1.player.mailReceived.Contains("NuclearBombCP.Marisol10HeartInvite"))
            //  {

            //   return;

            // }

            if(Game1.player.hasBuff("ApryllForever.NuclearBomb/Vampirinated"))
            {

                Game1.ambientLight = new Color(210, 40, 0) * 120;


            }
        }
    }
}

