using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// A class to track information about the room's bundles
	/// </summary>
	public class RoomInformation
	{
		public CommunityCenterRooms Room { get; set; }
		public int StartingIndex { get; set; }
		public int EndingIndex { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="room">The room</param>
		/// <param name="startingIndex">The room ID to start at</param>
		/// <param name="endingIndex">The index to end at</param>
		public RoomInformation(CommunityCenterRooms room, int startingIndex, int endingIndex)
		{
			Room = room;
			StartingIndex = startingIndex;
			EndingIndex = endingIndex;
		}
	}

	/// <summary>
	/// Randomizes community center bundles
	/// </summary>
	public class BundleRandomizer
	{
		/// <summary>
		/// Information about all the rooms
		/// </summary>
		public static List<RoomInformation> Rooms = new List<RoomInformation>
		{
			new RoomInformation(CommunityCenterRooms.CraftsRoom, 13, 19), // skip 18
			new RoomInformation(CommunityCenterRooms.Pantry, 0, 5),
			new RoomInformation(CommunityCenterRooms.FishTank, 6, 11),
			new RoomInformation(CommunityCenterRooms.BoilerRoom, 20, 22),
			new RoomInformation(CommunityCenterRooms.BulletinBoard, 31, 35),
			new RoomInformation(CommunityCenterRooms.Vault, 23, 26),
			new RoomInformation(CommunityCenterRooms.Joja, 36, 36)
		};

		private readonly static Dictionary<string, string> _randomizedBundles = new Dictionary<string, string>();

		/// <summary>
		/// The randomizing function
		/// </summary>
		/// <returns>A dictionary of bundles to their output string</returns>
		public static Dictionary<string, string> Randomize()
		{
			_randomizedBundles.Clear();
			Bundle.InitializeAllBundleTypes(); // Must be done so that reloading the game is consistent

			if (Globals.Config.RandomizeBundles) { Globals.SpoilerWrite("==== BUNDLES ===="); }
			foreach (RoomInformation room in Rooms)
			{
				if (Globals.Config.RandomizeBundles) { Globals.SpoilerWrite(room.Room.ToString()); }
				CreateBundlesForRoom(room);
			}

			return _randomizedBundles;
		}

		/// <summary>
		/// Creates the bundles for the room
		/// </summary>
		/// <param name="room">The room to create bundles for</param>
		private static void CreateBundlesForRoom(RoomInformation room)
		{
			if (room.StartingIndex > room.EndingIndex)
			{
				Globals.ConsoleError("Community center room information does not have correct indexes defined.");
				return;
			}

			List<Bundle> bundles = new List<Bundle>();
			for (int i = room.StartingIndex; i < room.EndingIndex + 1; i++)
			{
				if (i == 18) { continue; } // That's just how this is set up
				Bundle bundle = CreateBundleForRoom(room.Room, i);
				WriteToSpoilerLog(bundle, i);
			}
		}

		/// <summary>
		/// Writes the given bundle to the spoiler log
		/// </summary>
		/// <param name="bundle">The bundle</param>
		/// <param name="index">The bundle index</param>
		private static void WriteToSpoilerLog(Bundle bundle, int index)
		{
			if (!Globals.Config.RandomizeBundles) { return; }

			Globals.SpoilerWrite($"Bundle index: {index} - {bundle.Name} Bundle");

			if (bundle.Room != CommunityCenterRooms.Vault)
			{
				Globals.SpoilerWrite($"Possible items: " +
					$"{string.Join(", ", bundle.RequiredItems.Select(x => $"{x.Item.Name}: {x.NumberOfItems}").ToList())}"
				);
				int minimumRequiredItems = bundle.MinimumRequiredItems == null ?
					bundle.RequiredItems.Count : bundle.MinimumRequiredItems.Value;
				Globals.SpoilerWrite($"Required: {minimumRequiredItems}/{bundle.RequiredItems.Count}");
			}

			if (bundle.Room != CommunityCenterRooms.Joja)
			{
				Globals.SpoilerWrite($"Reward: {bundle.Reward.Item.Name}: {bundle.Reward.NumberOfItems}");
			}
			Globals.SpoilerWrite($"---");
		}

		/// <summary>
		/// Creates a random bundle for the given room
		/// </summary>
		/// <param name="room">The room to create the bundle for</param>
		/// <param name="roomId">The room id</param>
		/// <returns>The created bundle</returns>
		private static Bundle CreateBundleForRoom(CommunityCenterRooms room, int roomId)
		{
			Bundle bundle = Bundle.Create(room, roomId);
			_randomizedBundles[bundle.Key] = bundle.ToString();
			return bundle;
		}
	}
}

