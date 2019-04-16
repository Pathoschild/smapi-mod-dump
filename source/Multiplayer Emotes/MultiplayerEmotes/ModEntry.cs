
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiplayerEmotes.Framework;
using MultiplayerEmotes.Framework.Network;
using MultiplayerEmotes.Framework.Patches;
using MultiplayerEmotes.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerEmotes {

	public class ModEntry : Mod {

		public static ModConfig Config { get; private set; }
		public static ModData Data { get; private set; }
		public static MultiplayerModMessage MultiplayerMessage { get; private set; }

		private EmotesMenuButton emoteMenuButton;

		public static IMonitor ModMonitor { get; private set; }
		public static IModHelper ModHelper { get; private set; }

		/*
		 * NOTE:
		 * Emotes are only visible by other players with the mod installed.
		 */
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;
			ModHelper = Helper;
			MultiplayerMessage = new MultiplayerModMessage(helper);

			ModPatchManager patchManager = new ModPatchManager(helper);
			patchManager.PatchList.Add(FarmerPatch.DoEmotePatch.CreatePatch(helper.Reflection));
			patchManager.PatchList.Add(CharacterPatch.DoEmotePatch.CreatePatch(helper.Reflection));
			patchManager.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Debug);
			Config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Debug);
			Data = this.Helper.Data.ReadJsonFile<ModData>("data.json") ?? new ModData();

			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

			// TODO: Command to stop emotes from NPC and FarmAnimals
			helper.ConsoleCommands.Add("emote", "Play the emote animation with the passed id.\n\nUsage: emote <value>\n- value: a integer representing the animation id.", this.Emote);
			helper.ConsoleCommands.Add("emote_npc", "Force a npc to play the emote animation with the given id.\n\nUsage: emote_npc <name> <value>\n- name: a string representing the npc name.\n- value: a integer representing the animation id.", this.EmoteNpc);
			helper.ConsoleCommands.Add("emote_animal", "Force a farm animal to play the emote animation with the given id.\n\nUsage: emote_animal <name> <value>\n- name: a string representing the farm animal name.\n- value: a integer representing the animation id.", this.EmoteFarmAnimal);
			helper.ConsoleCommands.Add("stop_emote", "Stop any emote being played by you.\n\nUsage: stop_emote", this.StopEmote);
			helper.ConsoleCommands.Add("stop_all_emotes", "Stop any emote being played.\n\nUsage: stop_all_emotes", this.StopAllEmotes);
			helper.ConsoleCommands.Add("multiplayer_emotes", "List all the players that have this mod and can send and receive emotes.\n\nUsage: multiplayer_emotes", this.MultiplayerEmotesAvailable);

#if DEBUG
			helper.Events.Input.ButtonPressed += DebugActionsKeyBinds;
#endif
		}

		/// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {

			emoteMenuButton = new EmotesMenuButton(Helper, Config, Data);

			// Add EmoteMenuButton to the screen menus
			Game1.onScreenMenus.Insert(0, emoteMenuButton);

#if DEBUG
			// Initialize and setup debug world
			SetupDebugWorld();
#endif

		}

		private void Emote(string command, string[] args) {

			if(args.Length < 1) {
				this.Monitor.Log($"Missing parameters.\n\nUsage: emote <value>\n- value: a integer representing the emote id.", LogLevel.Info);
				return;
			}

			if(!int.TryParse(args[0], out int id)) {
				this.Monitor.Log($"The emote id must be a integer.", LogLevel.Info);
				return;
			}

			if(id > 0) {
				Game1.player.doEmote(id * 4);
				this.Monitor.Log($"Playing emote: {id}", LogLevel.Info);
#if DEBUG
			} else if(id < 0) {
				EmoteTemporaryAnimation emoteTempAnim = new EmoteTemporaryAnimation(Helper.Reflection, Helper.Events);
				emoteTempAnim.BroadcastEmote(id * -1);
				this.Monitor.Log($"Playing emote (workarround test): {id * -1}");
#endif
			} else {
				this.Monitor.Log($"The emote id value must be greater than 0.", LogLevel.Info);
			}

		}

		private void EmoteNpc(string command, string[] args) {

			if(!Context.IsMainPlayer && !Config.AllowNonHostEmoteNpcCommand) {
				this.Monitor.Log("Permission denied. You dont have enough permissions to run this command.", LogLevel.Info);
				return;
			}

			if(args.Length < 2) {
				this.Monitor.Log("Missing parameters.\n\nUsage: emote_npc <name> <value>\n- name: a string representing the npc name.\n- value: a integer representing the animation id.", LogLevel.Info);
				return;
			}

			NPC npc = Game1.getCharacterFromName(args[0]);
			if(npc == null) {
				this.Monitor.Log($"Could not find any NPC with the name \"{args[0]}\".", LogLevel.Info);
				return;
			}

			if(!int.TryParse(args[1], out int id)) {
				this.Monitor.Log("The emote id must be a integer.", LogLevel.Info);
				return;
			}

			if(id <= 0) {
				this.Monitor.Log("The emote id value must be greater than 0.", LogLevel.Info);
				return;
			}

			npc.doEmote(id * 4);

#if DEBUG
			this.Monitor.Log($"[id: {npc.id}, name: \"{npc.Name}\"] Playing emote: {id}", LogLevel.Info);
#else
			this.Monitor.Log($"[\"{npc.Name}\"] Playing emote: {id}", LogLevel.Info);
#endif
			

		}

		private void EmoteFarmAnimal(string command, string[] args) {

			if(!Context.IsMainPlayer && !Config.AllowNonHostEmoteAnimalCommand) {
				this.Monitor.Log("Permission denied. You dont have enough permissions to run this command.", LogLevel.Info);
				return;
			}

			if(args.Length < 2) {
				this.Monitor.Log("Missing parameters.\n\nUsage: emote_animal <name> <value>\n- name: a string representing the farm animal name.\n- value: a integer representing the animation id.", LogLevel.Info);
				return;
			}

			FarmAnimal farmAnimal = Game1.getFarm().getAllFarmAnimals().FirstOrDefault(x => x.Name == args[0]);
			if(farmAnimal == null) {
				this.Monitor.Log($"Could not find any FarmAnimal with \"{args[0]}\".", LogLevel.Info);
				return;
			}

			if(!int.TryParse(args[1], out int id)) {
				this.Monitor.Log("The emote id must be a integer.", LogLevel.Info);
				return;
			}

			if(id <= 0) {
				this.Monitor.Log("The emote id value must be greater than 0.", LogLevel.Info);
				return;
			}

			farmAnimal.doEmote(id * 4);

#if DEBUG
			this.Monitor.Log($"[id: {farmAnimal.myID.Value}, name: \"{farmAnimal.Name}\"] Playing emote: {id}", LogLevel.Info);
#else
			this.Monitor.Log($"[\"{farmAnimal.Name}\"] Playing emote: {id}", LogLevel.Info);
#endif

		}

		private void StopEmote(string command, string[] args) {

			if(Game1.player.IsEmoting) {
				this.Monitor.Log("Stoping playing emote...", LogLevel.Info);
				Game1.player.IsEmoting = false;
			} else {
				this.Monitor.Log("No emote is playing.", LogLevel.Info);
			}

		}

		private void StopAllEmotes(string command, string[] args) {

			this.Monitor.Log("Stoping any playing emotes...", LogLevel.Info);
			foreach(Farmer farmer in Game1.getAllFarmers()) {
				farmer.IsEmoting = false;
			}

			foreach(GameLocation location in Game1.locations) {
				foreach(NPC npc in location.getCharacters()) {
					npc.IsEmoting = false;
				}
			}

			foreach(FarmAnimal farmAnimal in Game1.getFarm().getAllFarmAnimals()) {
				farmAnimal.IsEmoting = false;
			}

		}

		private void MultiplayerEmotesAvailable(string command, string[] args) {

			if(!Context.IsMultiplayer) {
				this.Monitor.Log("You are not currently in a online session.", LogLevel.Info);
				return;
			}

			// Number of players excluding the host
			int numPlayers = Game1.getOnlineFarmers().Count - 1;

			if(numPlayers <= 0) {
				this.Monitor.Log("No players connected in the current session.", LogLevel.Info);
				return;
			}

			int playersWithMod = 0;
			StringBuilder sb = new StringBuilder();

			foreach(Farmer farmer in Game1.getOnlineFarmers()) {

				// Check that is not the current player
				if(Game1.player.UniqueMultiplayerID != farmer.UniqueMultiplayerID) {

					IMultiplayerPeer peer = this.Helper.Multiplayer.GetConnectedPlayer(farmer.UniqueMultiplayerID);

					if(peer.HasSmapi && peer.GetMod(this.ModManifest.UniqueID) != null) {
						playersWithMod++;
						sb.Append($"{playersWithMod}: \"{Game1.getFarmer(peer.PlayerID).Name}\"");
					}

				}

			}

			if(playersWithMod > 0) {
				this.Monitor.Log($"From {numPlayers} player(s), {playersWithMod} have this mod:\n{sb.ToString()}", LogLevel.Info);
			} else {
				this.Monitor.Log($"From {numPlayers} player(s), none has this mod.", LogLevel.Info);
			}

		}

#if DEBUG
		private void DebugActionsKeyBinds(object sender, ButtonPressedEventArgs e) {
			switch(e.Button) {
				case SButton.NumPad0:
					Game1.game1.parseDebugInput("setUpBigFarm");
					break;
				case SButton.NumPad1:
					Game1.game1.parseDebugInput("cat 64 15");
					Game1.game1.parseDebugInput("dog 64 15");
					Game1.game1.parseDebugInput("petToFarm");
					break;
				case SButton.NumPad2:
					Game1.game1.parseDebugInput("wc Lewis 63 18 2");
					break;
			}
		}

		private void SetupDebugWorld() {

			Game1.game1.parseDebugInput("zoomLevel 40");

			if(Context.IsMainPlayer) {

				// Pause time and set it to 09:00
				Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
				Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });

				Game1.game1.parseDebugInput("warp Farm 64 15");
				Game1.game1.parseDebugInput("nosave");
				/*
				Game1.game1.parseDebugInput("pet 64 15");
				Game1.game1.parseDebugInput("petToFarm");
				
				//Game1.game1.parseDebugInput("setUpBigFarm");
				Game1.game1.parseDebugInput("setUpFarm");

				// Coop Animals
				Game1.game1.parseDebugInput("animal Chicken");
				Game1.game1.parseDebugInput("animal Duck");
				Game1.game1.parseDebugInput("animal Rabbit");
				Game1.game1.parseDebugInput("animal Dinosaur");

				// Barn Animals
				Game1.game1.parseDebugInput("animal Cow");
				Game1.game1.parseDebugInput("animal Goat");
				Game1.game1.parseDebugInput("animal Sheep");
				Game1.game1.parseDebugInput("animal Pig");
				*/
			}

		}
#endif

	}

}
