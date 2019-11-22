using System.Linq;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomKissingMod
{
    public class CustomKissingModEntry : Mod
    {
        public const string MessageType = "Kissing";
        internal IModHelper ModHelper;
        internal IMonitor monitor;
        internal DataLoader DataLoader;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(ModHelper);

            if (!DataLoader.ModConfig.DisableKissingPlayers)
            {
                ModHelper.Events.Multiplayer.ModMessageReceived += ModMessageReceived;
            }

            var harmony = HarmonyInstance.Create("Digus.CustomKissingMod");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                postfix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkAction))
            );
            if (!DataLoader.ModConfig.DisableKissingPlayers)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.checkAction)),
                    postfix: new HarmonyMethod(typeof(FarmerOverrides), nameof(FarmerOverrides.checkAction))
                );
            }
        }

        private void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID)
            {
                if (e.Type == MessageType)
                {
                    KissingMessage kissingMessage = e.ReadAs<KissingMessage>();
                    if (Game1.player.UniqueMultiplayerID != kissingMessage.Kisser)
                    {
                        Farmer kisser = Game1.getAllFarmers().First(f => f.UniqueMultiplayerID== kissingMessage.Kisser);
                        if (Equals(Game1.player.currentLocation, kisser.currentLocation))
                        {
                            Farmer kissed = Game1.getAllFarmers().First(f => f.UniqueMultiplayerID == kissingMessage.Kissed);
                            bool result = false;
                            FarmerOverrides.checkAction(kissed,ref kisser, ref result, kissed.currentLocation);
                        }
                    }
                }
            }
        }
    }
}
