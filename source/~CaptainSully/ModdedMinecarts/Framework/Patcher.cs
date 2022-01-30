/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using HarmonyLib;
using xTile.Dimensions;

namespace ModdedMinecarts
{
    internal class Patcher
    {
        private static readonly Log log = ModEntry.Instance.log;
        //private static ModConfig Config { get; set; } = ModEntry.Config;

        public static void PatchAll()
        {
            var harmony = new Harmony(ModEntry.UID);

            try
            {
                log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Length - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), "performAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchPerformAction))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), "answerDialogueAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchAnswerDialogueAction))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }

        /**
         * From GameLocation : public virtual bool performAction(string action, Farmer who, GameLocation location)
         */
        public static bool PatchPerformAction(ref GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            try
            {
                if (action.StartsWith("MinecartTransport"))
                {
                    switch (action)
                    {
                        case "MinecartTransport":
                            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
                            {
                                // create dialogue box
                                __instance.createQuestionDialogue(answerChoices: (!Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom")) ? new Response[3]
                                    //if craftsroom unlocked, these options
                                    {
                                        new Response("Town", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_Town")),
                                        new Response("Bus", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_BusStop")),
                                        new Response("Cancel", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_Cancel"))
                                    }
                                    //else these
                                    : new Response[4]
                                    {
                                        new Response("Town", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_Town")),
                                        new Response("Bus", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_BusStop")),
                                        new Response("Quarry", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_Quarry")),
                                        new Response("Cancel", Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_Destination_Cancel"))
                                    }, question: Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_ChooseDestination"), dialogKey: "Minecart");
                            }
                            else
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("..\\Mods\\ModdedMinecarts\\Locations:MineCart_OutOfOrder"));
                            }
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                log.E("There was an exception in patch PatchPerformAction", e);
                return true;
            }
        }

        /**
         * From GameLocation: public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
         */
        public static bool PatchAnswerDialogueAction(ref GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer is null || !questionAndAnswer.StartsWith("ModMinecart"))
            {
                return true;
            }

            string []set = questionAndAnswer.Split('_');
            string action = set[0];
            string location = set[1];

            int x = -1;
            int y = -1;
            int dir;
            switch (location)
            {
                case "Mines":
                    Game1.player.Halt();
                    Game1.player.freezePause = 700;
                    x = 13;
                    y = 9;
                    dir = 1;
                    if (Game1.getMusicTrackName() == "springtown")
                    {
                       Game1.changeMusicTrack("none");
                    }
                    break;
                case "Town":
                    x = 105;
                    y = 80;
                    dir = 1;
                    break;
                case "Mountain":
                    x = 124;
                    y = 12;
                    dir = 2;
                    break;
                case "BusStop":
                    x = 4;
                    y = 4;
                    dir = 2;
                    break;
                default:
                    return true;
            }
            Game1.player.Halt();
            Game1.player.freezePause = 700;
            Game1.warpFarmer(location, x, y, dir);
            return false;
        }
    }
}
