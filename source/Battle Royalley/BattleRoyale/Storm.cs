using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BattleRoyale
{
    //Collapse this and never look inside again
    [Serializable]
	public class Phase
	{
		[Serializable]
		public class Location
		{
			public string LocationName { get; set; }
			public Storm.Direction Direction { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public TwoPointRectangle CloseInRectangle { get; set; } = null;

			public Location() { }

			public Location(string locationName, Storm.Direction direction, TwoPointRectangle closeInRectangle = null)
			{
				LocationName = locationName;
				Direction = direction;
				CloseInRectangle = closeInRectangle;
			}
		}

		[Serializable]
		public class TwoPointRectangle
		{
			public int X1 { get; set; } = 0;
			public int Y1 { get; set; } = 0;
			public int X2 { get; set; } = 0;
			public int Y2 { get; set; } = 0;

			public TwoPointRectangle() { }

			public TwoPointRectangle(int x1, int y1, int x2, int y2)
			{
				X1 = x1;
				X2 = x2;
				Y1 = y1;
				Y2 = y2;
			}

			public override string ToString()
			{
				return $"[({X1}, {Y1}), ({X2}, {Y2})]";
			}
		}

		public List<Location> Locations { get; set; }
		public TimeSpan Delay { get; set; }
		public TimeSpan Duration { get; set; }

		//JSON.Net needs a parameter-less constructor
		public Phase() { }

		public Phase(List<object> locations, TimeSpan delay, TimeSpan duration)
		{
			Locations = locations.Select((x) => x is Tuple<string, Storm.Direction, TwoPointRectangle> y ? new Location(
					y.Item1,
					y.Item2,
					y.Item3
				) :
				x is Tuple<string, Storm.Direction> y2 ? 

				new Location(
				y2.Item1,
				y2.Item2   ) : null).ToList();

			Delay = delay;
			Duration = duration;
		}
	}

	public class StormDataModel
	{
		public List<Phase>[] Phases { get; set; } = 
		{
			//Close on town(old)
			/**new List<Phase>()
			{
				new Phase(new List<object>(){
					("FarmCave", Storm.Direction.UpToDown),
					("Greenhouse", Storm.Direction.UpToDown),
					("Cellar", Storm.Direction.DownToUp),
					("HarveyRoom", Storm.Direction.UpToDown),
					("ElliottHouse", Storm.Direction.RightToLeft),
					("FishShop", Storm.Direction.UpToDown),
					("BathHouse_Pool", Storm.Direction.UpToDown),
					("BathHouse_Entry", Storm.Direction.UpToDown),
					("BathHouse_MensLocker", Storm.Direction.RightToLeft),
					("BathHouse_WomensLocker", Storm.Direction.LeftToRight),
					("Tunnel", Storm.Direction.LeftToRight),
					("SkullCave", Storm.Direction.LeftToRight),
					("WitchSwamp", Storm.Direction.UpToDown),
					("WitchHut", Storm.Direction.UpToDown),
					("WitchWarpCave", Storm.Direction.UpToDown),
					("SebastianRoom", Storm.Direction.DownToUp),
					("Tent", Storm.Direction.UpToDown),
					("BugLand", Storm.Direction.UpToDown),
					("Club", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					("ScienceHouse", Storm.Direction.RightToLeft),
					("AdventureGuild", Storm.Direction.UpToDown),
					("FarmHouse", Storm.Direction.UpToDown),
					("Hospital", Storm.Direction.UpToDown),
					("JojaMart", Storm.Direction.LeftToRight),
					("WizardHouseBasement", Storm.Direction.RightToLeft),
					("LeahHouse", Storm.Direction.RightToLeft),
					("Sewer", Storm.Direction.DownToUp),
					("Mine", Storm.Direction.RightToLeft),
					("SandyHouse", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 25), duration: new TimeSpan(0, 0, seconds: 20)),

				new Phase(new List<object>(){
					("JoshHouse", Storm.Direction.UpToDown),
					("HaleyHouse", Storm.Direction.UpToDown),
					("SamHouse", Storm.Direction.UpToDown),
					("Blacksmith", Storm.Direction.UpToDown),
					("ManorHouse", Storm.Direction.RightToLeft),
					("SeedShop", Storm.Direction.RightToLeft),
					("Trailer", Storm.Direction.UpToDown),
					("Trailer_Big", Storm.Direction.UpToDown),
					("ArchaeologyHouse", Storm.Direction.RightToLeft),
					("CommunityCenter", Storm.Direction.UpToDown),
					("Beach", Storm.Direction.DownToUp),
					("Woods", Storm.Direction.LeftToRight),
					("WizardHouse", Storm.Direction.UpToDown),
					("AnimalShop", Storm.Direction.UpToDown),
					("Railroad", Storm.Direction.UpToDown),
					("Desert", Storm.Direction.DownToUp)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 30)),

				new Phase(new List<object>(){
					("Mountain", Storm.Direction.RightToLeft),
					("Forest", Storm.Direction.LeftToRight),
					("BusStop", Storm.Direction.LeftToRight),
					("Backwoods", Storm.Direction.LeftToRight),
					("Farm", Storm.Direction.LeftToRight),
					("Saloon", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 42)),

				new Phase(new List<object>(){
					("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1400, 3990, 2380, 4754))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 35))
			},*/

			//Close on desert
			new List<Phase>()
			{
				new Phase(new List<object>(){
					Tuple.Create("Railroad", Storm.Direction.UpToDown),
					Tuple.Create("ScienceHouse", Storm.Direction.RightToLeft),
					Tuple.Create("SebastianRoom", Storm.Direction.DownToUp),
					Tuple.Create("Mine", Storm.Direction.UpToDown),
					Tuple.Create("AdventureGuild", Storm.Direction.UpToDown),
					Tuple.Create("FishShop", Storm.Direction.UpToDown),
					Tuple.Create("ElliottHouse", Storm.Direction.RightToLeft),
					Tuple.Create("BugLand", Storm.Direction.UpToDown),
					Tuple.Create("HarveyRoom", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("AnimalShop", Storm.Direction.UpToDown),
					Tuple.Create("LeahHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Woods", Storm.Direction.LeftToRight),
					Tuple.Create("WizardHouse", Storm.Direction.UpToDown),
					Tuple.Create("HaleyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("JoshHouse", Storm.Direction.UpToDown),
					Tuple.Create("ManorHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Saloon", Storm.Direction.UpToDown),
					Tuple.Create("SeedShop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(384, 1900, 384, 1900)),
					Tuple.Create("SamHouse", Storm.Direction.UpToDown),
					Tuple.Create("JojaMart", Storm.Direction.UpToDown),
					Tuple.Create("Blacksmith", Storm.Direction.UpToDown),
					Tuple.Create("ArchaeologyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Trailer", Storm.Direction.UpToDown),
					Tuple.Create("Trailer_Big", Storm.Direction.UpToDown),
					Tuple.Create("CommunityCenter", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2048, 1600, 2176, 1600)),
					Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
					Tuple.Create("Sewer", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1056, 704, 1056, 704)),
					Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
					Tuple.Create("Hospital", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 25), duration: new TimeSpan(0, 0, seconds: 25)),

				new Phase(new List<object>(){
					Tuple.Create("FarmCave", Storm.Direction.UpToDown),
					Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(4288, -100, 4608, -100)),
					Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 3392, -100, 3520))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 30)),

				new Phase(new List<object>(){
					Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5150, -100, 5150, 1250))
				}, delay: new TimeSpan(0, 0, seconds: 5), duration: new TimeSpan(0, 0, seconds: 25)),

				new Phase(new List<object>(){
					Tuple.Create("BusStop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 384, -100, 576))
				}, delay: new TimeSpan(0, 0, seconds: 7), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("SkullCave", Storm.Direction.LeftToRight),
					Tuple.Create("Club", Storm.Direction.UpToDown),
					Tuple.Create("SandyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1472, 1792, 1472, 2048))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2176, 512, 2624, 896))
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 25)),
			},

			//Close on town (updated)
			new List<Phase>()
			{
				new Phase(new List<object>(){
					Tuple.Create("Railroad", Storm.Direction.UpToDown),
					Tuple.Create("SandyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("SkullCave", Storm.Direction.LeftToRight),
					Tuple.Create("Club", Storm.Direction.UpToDown),
					Tuple.Create("FarmCave", Storm.Direction.UpToDown),
					Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
					Tuple.Create("Tunnel", Storm.Direction.LeftToRight)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("Mine", Storm.Direction.UpToDown),
					Tuple.Create("AdventureGuild", Storm.Direction.UpToDown),
					Tuple.Create("ScienceHouse", Storm.Direction.RightToLeft),
					Tuple.Create("SebastianRoom", Storm.Direction.DownToUp),
					Tuple.Create("WizardHouse", Storm.Direction.UpToDown),
					Tuple.Create("Woods", Storm.Direction.LeftToRight),
					Tuple.Create("AnimalShop", Storm.Direction.UpToDown),
					Tuple.Create("LeahHouse", Storm.Direction.RightToLeft),
					Tuple.Create("FishShop", Storm.Direction.UpToDown),
					Tuple.Create("ElliottHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Backwoods", Storm.Direction.LeftToRight),
					Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5150, -100, 5150, 1250))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 35)),

				new Phase(new List<object>(){
					Tuple.Create("BusStop", Storm.Direction.LeftToRight),
					Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
					Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
					Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7700, 1450, 7700, 1820)),
					Tuple.Create("BugLand", Storm.Direction.UpToDown),
					Tuple.Create("JojaMart", Storm.Direction.UpToDown),
					Tuple.Create("Blacksmith", Storm.Direction.UpToDown),
					Tuple.Create("ArchaeologyHouse", Storm.Direction.RightToLeft)
				}, delay: new TimeSpan(0, 0, seconds: 7), duration: new TimeSpan(0, 0, seconds: 40)),

				new Phase(new List<object>(){
					Tuple.Create("Sewer", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1056, 704, 1056, 704)),
					Tuple.Create("Trailer", Storm.Direction.UpToDown),
					Tuple.Create("Trailer_Big", Storm.Direction.UpToDown),
					Tuple.Create("CommunityCenter", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2048, 1600, 2176, 1600)),
					Tuple.Create("HaleyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("JoshHouse", Storm.Direction.UpToDown),
					Tuple.Create("SamHouse", Storm.Direction.UpToDown),
					Tuple.Create("ManorHouse", Storm.Direction.RightToLeft),
					Tuple.Create("HarveyRoom", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 25)),

				new Phase(new List<object>(){
					Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1400, 3990, 2380, 4754)),
					Tuple.Create("Saloon", Storm.Direction.UpToDown),
					Tuple.Create("SeedShop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(384, 1900, 384, 1900)),
					Tuple.Create("Hospital", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 35))
			},

			//Close on secret forest
			new List<Phase>()
			{
				new Phase(new List<object>(){
					Tuple.Create("SandyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("SkullCave", Storm.Direction.LeftToRight),
					Tuple.Create("Club", Storm.Direction.UpToDown),
					Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
					Tuple.Create("SebastianRoom", Storm.Direction.DownToUp)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("Railroad", Storm.Direction.UpToDown),
					Tuple.Create("Mine", Storm.Direction.UpToDown),
					Tuple.Create("AdventureGuild", Storm.Direction.UpToDown),
					Tuple.Create("ScienceHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Tent", Storm.Direction.UpToDown),
					Tuple.Create("FishShop", Storm.Direction.UpToDown),
					Tuple.Create("ElliottHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Backwoods", Storm.Direction.UpToDown),
					Tuple.Create("Tunnel", Storm.Direction.LeftToRight),
					Tuple.Create("HarveyRoom", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 20), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("BugLand", Storm.Direction.UpToDown),
					Tuple.Create("FarmCave", Storm.Direction.UpToDown),
					Tuple.Create("BusStop", Storm.Direction.RightToLeft),
					Tuple.Create("Hospital", Storm.Direction.UpToDown),
					Tuple.Create("SeedShop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(384, 1900, 384, 1900)),
					Tuple.Create("Saloon", Storm.Direction.UpToDown),
					Tuple.Create("ManorHouse", Storm.Direction.RightToLeft),
					Tuple.Create("JoshHouse", Storm.Direction.UpToDown),
					Tuple.Create("Trailer", Storm.Direction.UpToDown),
					Tuple.Create("Trailer_Big", Storm.Direction.UpToDown),
					Tuple.Create("CommunityCenter", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2048, 1600, 2176, 1600)),
					Tuple.Create("JojaMart", Storm.Direction.UpToDown),
					Tuple.Create("Blacksmith", Storm.Direction.UpToDown),
					Tuple.Create("ArchaeologyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
					Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750))
				}, delay: new TimeSpan(0, 0, seconds: 30), duration: new TimeSpan(0, 0, seconds: 35)),

				new Phase(new List<object>(){
					Tuple.Create("AnimalShop", Storm.Direction.UpToDown),
					Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 5700, -100, 5950)),
					Tuple.Create("SamHouse", Storm.Direction.UpToDown),
					Tuple.Create("HaleyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Sewer", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(120, 3200, 312, 3200)),
					Tuple.Create("Farm", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 30)),

				new Phase(new List<object>(){
					Tuple.Create("WizardHouse", Storm.Direction.UpToDown),
					Tuple.Create("LeahHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 320, -100, 520))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 35)),

				new Phase(new List<object>(){
					Tuple.Create("Woods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(288, 384, 934, 864))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 25))
			},

			//Close on secret forest
			new List<Phase>()
			{
				new Phase(new List<object>(){
					Tuple.Create("SandyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("SkullCave", Storm.Direction.LeftToRight),
					Tuple.Create("Club", Storm.Direction.UpToDown),
					Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
					Tuple.Create("SebastianRoom", Storm.Direction.DownToUp),
					Tuple.Create("Tunnel", Storm.Direction.LeftToRight)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 15)),

				new Phase(new List<object>(){
					Tuple.Create("Railroad", Storm.Direction.UpToDown),
					Tuple.Create("Mine", Storm.Direction.UpToDown),
					Tuple.Create("AdventureGuild", Storm.Direction.UpToDown),
					Tuple.Create("ScienceHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Tent", Storm.Direction.UpToDown),
					Tuple.Create("FishShop", Storm.Direction.UpToDown),
					Tuple.Create("ElliottHouse", Storm.Direction.RightToLeft),
					Tuple.Create("FarmCave", Storm.Direction.UpToDown),
					Tuple.Create("BusStop", Storm.Direction.UpToDown),
					Tuple.Create("HarveyRoom", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 18)),

				new Phase(new List<object>(){
					Tuple.Create("AnimalShop", Storm.Direction.UpToDown),
					Tuple.Create("Farm", Storm.Direction.UpToDown),
					Tuple.Create("WizardHouse", Storm.Direction.UpToDown),
					Tuple.Create("LeahHouse", Storm.Direction.RightToLeft),
					Tuple.Create("Woods", Storm.Direction.LeftToRight),
					Tuple.Create("Blacksmith", Storm.Direction.UpToDown),
					Tuple.Create("ArchaeologyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("JojaMart", Storm.Direction.UpToDown),
					Tuple.Create("Hospital", Storm.Direction.UpToDown),
					Tuple.Create("SeedShop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(384, 1900, 384, 1900)),
					Tuple.Create("Saloon", Storm.Direction.UpToDown),
					Tuple.Create("JoshHouse", Storm.Direction.UpToDown),
					Tuple.Create("Trailer", Storm.Direction.UpToDown),
					Tuple.Create("Trailer_Big", Storm.Direction.UpToDown),
					Tuple.Create("CommunityCenter", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2048, 1600, 2176, 1600)),
					Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
					Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
					Tuple.Create("Backwoods", Storm.Direction.UpToDown)
				}, delay: new TimeSpan(0, 0, seconds: 15), duration: new TimeSpan(0, 0, seconds: 30)),

				new Phase(new List<object>(){
					Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6047, 6244, 6047, 6800)),
					Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2212, 6144, 2212, 6144)),
					Tuple.Create("SamHouse", Storm.Direction.UpToDown),
					Tuple.Create("HaleyHouse", Storm.Direction.RightToLeft),
					Tuple.Create("ManorHouse", Storm.Direction.RightToLeft)
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 45)),

				new Phase(new List<object>(){
					Tuple.Create("Sewer", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(225, 1044, 225, 1300))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 20)),

				new Phase(new List<object>(){
					Tuple.Create("BugLand", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1536, 200, 2464, 712))
				}, delay: new TimeSpan(0, 0, seconds: 10), duration: new TimeSpan(0, 0, seconds: 30))
			}
		};
	}

	public class Storm
	{
		private static Color StormColor => new Color(0.6f*247 /256, 0.6f * 230 /256, 0.6f * 98 /256, 0.6f);

		private static Texture2D pixelTexture = null;

		public static List<Phase>[] Phases { get; set; } = new StormDataModel().Phases;
        private static TimeSpan totalLengthOfPhases = new TimeSpan(0);

		public enum Direction
		{
			LeftToRight, RightToLeft, UpToDown, DownToUp, CloseIn
		}

		private static DateTime startTime;
		private static int stormIndex = 0;

		public static void StartStorm(int stormIndex)
		{
			startTime = DateTime.Now;
			Storm.stormIndex = stormIndex;

            totalLengthOfPhases = new TimeSpan(0);
            foreach (var phase in Phases[stormIndex])
            {
                totalLengthOfPhases += phase.Delay;
                totalLengthOfPhases += phase.Duration;
            }
        }

		public static int GetRandomStormIndex() => Game1.random.Next(Phases.Length);

		public static void LoadStormData(IModHelper helper)
		{
			StormDataModel data = helper.Data.ReadJsonFile<StormDataModel>("stormPhases.json");
			if (data == null)
			{
				data = new StormDataModel();
				helper.Data.WriteJsonFile("stormPhases.json", data);
			}

			Phases = data.Phases;

			Console.WriteLine("Loaded phases:");
			
			for (int i = 0; i < Phases.Length; i++)
			{
				Console.WriteLine($" Phase set {i+1}:");
				int j = 1;
				foreach (Phase phase in Phases[i].Where(x => x != null))
				{
					Console.WriteLine($"  Phase {j}: Delay={phase.Delay}, Duration={phase.Duration}");
					foreach (Phase.Location location in phase.Locations)
					{
						string s = location.CloseInRectangle != null ? $", CloseInRect={location.CloseInRectangle}" : "";
						Console.WriteLine($"  - {location.LocationName}, {location.Direction} {s}");
					}

					j++;
				}
			}
		}

		public static void SendPhasesData(long targetID)
		{
			var b = new BinaryFormatter();
			var stream = new MemoryStream();
			b.Serialize(stream, Phases);
			byte[] msg = stream.GetBuffer();

			var message = new StardewValley.Network.OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player,
				(int)NetworkUtility.MessageTypes.SEND_PHASE_DATA,
				msg);

			Game1.server.sendMessage(targetID, message);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{

			if (!ModEntry.BRGame.IsGameInProgress)
				return;

			//var (currentPhase, currentPhaseAmount) = GetCurrentPhase();
			var tp = GetCurrentPhase();
			var currentPhase = tp.Item1;
			var currentPhaseAmount = tp.Item2;

			if (Game1.currentLocation != null)
			{
				//var (amount, direction, closeInRectangle) = GetStormAmountInLocation(currentPhase, currentPhaseAmount, Game1.currentLocation);
				var tp2 = GetStormAmountInLocation(currentPhase, currentPhaseAmount, Game1.currentLocation);
				var amount = tp2.Item1;
				var direction = tp2.Item2;
				var closeInRectangle = tp2.Item3;

				DrawSingleStorm(direction, amount, Game1.currentLocation, spriteBatch, closeInRectangle);
			}
		}

		//(double amount, Direction direction, Phase.TwoPointRectangle closeInRectangle)
		private static Tuple<double, Direction, Phase.TwoPointRectangle> GetStormAmountInLocation(int currentPhase, double currentPhaseAmount, GameLocation gameLocation)
		{
			for (int i = 0; i <= currentPhase; i++)
			{
				Phase phase = Phases[stormIndex][i];
				foreach (Phase.Location location in phase.Locations)
				{
					if (gameLocation.Name == location.LocationName)
					{
						return Tuple.Create((i == currentPhase) ? currentPhaseAmount : 1, location.Direction, location.CloseInRectangle);
					}
				}
			}

            foreach (Phase.Location location in Phases[stormIndex].SelectMany(x => x.Locations))
            {
                if (gameLocation.Name == location.LocationName)
                {
                    return Tuple.Create<double, Direction, Phase.TwoPointRectangle>(0, Direction.UpToDown, null);
                }
            }

            return Tuple.Create<double, Direction, Phase.TwoPointRectangle>(1, Direction.UpToDown, null);//For unregistered locations, e.g. sheds/barns
		}


		//(int currentPhase, double currentPhaseAmount)
		private static Tuple<int, double> GetCurrentPhase()
		{
			int currentPhase = -1;
			double currentPhaseAmount = 1;
			{
				//TODO: multiply speedMultiplier to speed it up when there are fewer players?
				double speedMultiplier = 1;
				TimeSpan gameDuration = DateTime.Now - startTime;
				gameDuration = new TimeSpan((long)(gameDuration.Ticks * speedMultiplier));

				TimeSpan n = TimeSpan.Zero;

				for (int i = 0; i < Phases[stormIndex].Count; i++)
				{
					Phase phase = Phases[stormIndex][i];

					n += phase.Delay;
					if (n >= gameDuration)
					{
						break;
					}

					currentPhase = i;
					n += phase.Duration;
					if (n >= gameDuration)
					{
						currentPhaseAmount = Math.Max(0, Math.Min(1, 1 - (n - gameDuration).TotalMilliseconds / phase.Duration.TotalMilliseconds));
						break;
					}
				}
			}

			return Tuple.Create(currentPhase, currentPhaseAmount);
		}

		/// <summary>
		/// amount: 0 to 1
		/// </summary>
		public static void DrawSingleStorm(Direction direction, double amount, GameLocation gameLocation, SpriteBatch spriteBatch, Phase.TwoPointRectangle closeInRectangle)
		{
			if (gameLocation == null)
				return;
			
			if (pixelTexture == null)
			{
				pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
				pixelTexture.SetData(new Color[1] { StormColor });
			}

			if (SpectatorMode.InSpectatorMode && Game1.activeClickableMenu != null)
				return;

			Rectangle b = GetStormBounds(direction, amount, gameLocation, closeInRectangle);

			if (direction != Direction.CloseIn)
			{
				spriteBatch.Draw(pixelTexture,
					new Rectangle(b.X - Game1.viewport.X, b.Y - Game1.viewport.Y, b.Width, b.Height),
					Color.White);
					//StormColor);
			}else
			{
				int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
				int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;
				
				Rectangle[] rectangles = new Rectangle[4]
				{
					new Rectangle(0 - Game1.viewport.X, 0 - Game1.viewport.Y, b.X, locationHeight),
					new Rectangle(b.Right - Game1.viewport.X, 0 - Game1.viewport.Y, locationWidth - b.Right, locationHeight),
					new Rectangle(b.X - Game1.viewport.X, 0 - Game1.viewport.Y, b.Width, b.Y),
					new Rectangle(b.X - Game1.viewport.X, b.Y + b.Height - Game1.viewport.Y, b.Width, locationHeight - (b.Y + b.Height)),
				};

				foreach (Rectangle rectangle in rectangles)
				{
					spriteBatch.Draw(pixelTexture, rectangle,
						Color.White);
						//StormColor);
				}
			}
		}

		private static Rectangle GetStormBounds(Direction direction, double amount, GameLocation gameLocation, Phase.TwoPointRectangle closeInRectangle)
		{
			if (amount < 0 || amount > 1)
				throw new ArgumentException("Must be between 0 and 1", nameof(amount));
			
			int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
			int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;

			int x, y, w, h;
			x = y = w = h = 0;

			switch (direction)
			{
				case Direction.LeftToRight:
					x = 0;
					y = 0;
					w = (int)(amount * locationWidth);
					h = locationHeight;
					break;
				case Direction.RightToLeft:
					x = (int)((1 - amount) * locationWidth);
					y = 0;
					w = locationWidth; h = locationHeight;
					break;
				case Direction.UpToDown:
					x = 0;
					y = 0;
					w = locationWidth;
					h = (int)(amount * locationHeight);
					break;
				case Direction.DownToUp:
					x = 0;
					y = (int)((1 - amount) * locationHeight);
					w = locationWidth;
					h = locationHeight;
					break;
				case Direction.CloseIn:
					x = (int)(amount * closeInRectangle.X1);
					y = (int)(amount * closeInRectangle.Y1);
					w = (int)(locationWidth - amount * (locationWidth - closeInRectangle.X2)) - x;
					h = (int)(locationHeight - amount * (locationHeight - closeInRectangle.Y2)) - y;
					break;
			}

			return new Rectangle(x, y, w, h);
		}

		public static void QuarterSecUpdate(List<long> alivePlayers)
		{
			if (!Game1.IsServer)
				return;

			//var (currentPhase, currentPhaseAmount) = GetCurrentPhase();
			var tp = GetCurrentPhase();
			var currentPhase = tp.Item1;
			var currentPhaseAmount = tp.Item2;

			//var amounts = new Dictionary<GameLocation, (double amount, Direction direction, Phase.TwoPointRectangle closeInRectangle)>();
			var amounts = new Dictionary<GameLocation, Tuple<double, Direction, Phase.TwoPointRectangle>>();
            foreach (Farmer player in Game1.getOnlineFarmers())
            {
                var currentLocation = player.currentLocation;
                if (currentLocation != null && !amounts.ContainsKey(currentLocation))
                {
                    amounts.Add(currentLocation, GetStormAmountInLocation(currentPhase, currentPhaseAmount, currentLocation));
                }
            }

            foreach (var pair in amounts)
			{
                var gameLocation = pair.Key;
				//var (amount, direction, closeInRectangle) = pair.Value;
				var t = pair.Value;
				var amount = t.Item1;
				var direction = t.Item2;
				var closeInRectangle = t.Item3;
                
				Rectangle bounds = GetStormBounds(direction, amount, gameLocation, closeInRectangle);

				foreach (Farmer farmer in gameLocation.farmers.Where(x => alivePlayers.Contains(x.UniqueMultiplayerID)))
				{
					bool contains = bounds.Contains(Utility.Vector2ToPoint(farmer.Position));
					if (contains && (closeInRectangle == null) || (!contains) && (closeInRectangle != null))
					{
						int damage = (int)(ModEntry.Config.StormDamagePerSecond * 1.2);//Effectively 20 damage per 1.2 sec because invincibility lasts 1.2 sec

						if (Game1.MasterPlayer == farmer)
							ModEntry.BRGame.TakeDamage(damage);
						else
							NetworkUtility.SendDamageToPlayer(damage, farmer);
					}
				}
			}

            //When the phase has closed in, two players could not kill eachother and the game would last indefinitely.
            //When time > phasetime*1.3, damage them anyway
            if (DateTime.Now - startTime >=  TimeSpan.FromTicks((long)(totalLengthOfPhases.Ticks * 2)))
            {
                int damage = 10;

                foreach (long id in ModEntry.BRGame.alivePlayers.ToList())
                {
                    if (Game1.MasterPlayer.UniqueMultiplayerID == id)
                        ModEntry.BRGame.TakeDamage(damage);
                    else
                        NetworkUtility.SendDamageToPlayerServerOnly(damage, id);
                }
            }
		}
	}
}
