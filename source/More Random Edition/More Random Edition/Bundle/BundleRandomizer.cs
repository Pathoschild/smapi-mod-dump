/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

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
		public List<Bundle> Bundles { get; set; } = new List<Bundle>();

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
        public static RNG Rng { get; set; }

        /// <summary>
        /// Information about all the rooms
        /// </summary>
        public readonly static List<RoomInformation> Rooms = new()
		{
			new RoomInformation(CommunityCenterRooms.CraftsRoom, 13, 19), // skip 18
			new RoomInformation(CommunityCenterRooms.Pantry, 0, 5),
			new RoomInformation(CommunityCenterRooms.FishTank, 6, 11),
			new RoomInformation(CommunityCenterRooms.BoilerRoom, 20, 22),
			new RoomInformation(CommunityCenterRooms.BulletinBoard, 31, 35),
			new RoomInformation(CommunityCenterRooms.Vault, 23, 26),
			new RoomInformation(CommunityCenterRooms.Joja, 36, 36)
		};

        private readonly static Dictionary<string, string> RandomizedBundles = new();

		/// <summary>
		/// A dictionary of the bundle ID to the lastly generated display name
		/// </summary>
		public readonly static Dictionary<int, string> BundleToName = new();
       
        /// <summary>
        /// The randomizing function
        /// </summary>
        /// <returns>A dictionary of bundles to their output string</returns>
        public static Dictionary<string, string> Randomize()
		{
            RandomizedBundles.Clear();
            BundleToName.Clear();
			if (!Globals.Config.Bundles.Randomize)
			{
				return new Dictionary<string, string>();
			}

            Rng = RNG.GetFarmRNG(nameof(BundleRandomizer));
			Bundle.InitializeAllBundleTypes(); // Must be done so that reloading the game is consistent

			Globals.SpoilerWrite("==== BUNDLES ====");
			foreach (RoomInformation room in Rooms)
			{
				Globals.SpoilerWrite(room.Room.ToString());
				room.Bundles.Clear(); // Clear the bundles in case this was ran multiple times in a session
				CreateBundlesForRoom(room);
			}

			return RandomizedBundles;
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

			for (int i = room.StartingIndex; i < room.EndingIndex + 1; i++)
			{
				if (i == 18) { continue; } // That's just how this is set up
				Bundle bundle = CreateBundleForRoom(room.Room, i);
				room.Bundles.Add(bundle);
				BundleToName.Add(bundle.Id, bundle.DisplayName);

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
			if (!Globals.Config.Bundles.Randomize) { return; }

			Globals.SpoilerWrite($"Bundle index: {index} - {bundle.DisplayName} Bundle");

			if (bundle.Room != CommunityCenterRooms.Vault)
			{
				Globals.SpoilerWrite($"Possible items: " +
					$"{string.Join(", ", bundle.RequiredItems.Select(x => $"{x.Item.DisplayName}: {x.NumberOfItems}").ToList())}"
				);
				int minimumRequiredItems = bundle.MinimumRequiredItems == null ?
					bundle.RequiredItems.Count : bundle.MinimumRequiredItems.Value;
				Globals.SpoilerWrite($"Required: {minimumRequiredItems}/{bundle.RequiredItems.Count}");
			}

			if (bundle.Room != CommunityCenterRooms.Joja)
			{
				Globals.SpoilerWrite($"Reward: {bundle.Reward.Item.DisplayName}: {bundle.Reward.NumberOfItems}");
			}
			Globals.SpoilerWrite($"---");
		}

		/// <summary>
		/// Creates a random bundle for the given room
		/// </summary>
		/// <param name="room">The room to create the bundle for</param>
		/// <param name="roomId">The room id</param>
		/// <returns>The created bundle</returns>
		private static Bundle CreateBundleForRoom(
			CommunityCenterRooms room, int roomId)
		{
			Bundle bundle = Bundle.Create(room, roomId);
			RandomizedBundles[bundle.Key] = bundle.ToString();
            return bundle;
		}
	}
}

