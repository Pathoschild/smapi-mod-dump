using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace PregnancyRole
{
	internal static class Model
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;

		public static readonly List<string> BaseGameNPCs = new List<string>
		{
			"Abigail",
			"Alex",
			"Caroline",
			"Clint",
			"Demetrius",
			"Dwarf",
			"Elliott",
			"Emily",
			"Evelyn",
			"George",
			"Gus",
			"Haley",
			"Harvey",
			"Jas",
			"Jodi",
			"Kent",
			"Krobus",
			"Leah",
			"Lewis",
			"Linus",
			"Marlon",
			"Marnie",
			"Maru",
			"Pam",
			"Penny",
			"Pierre",
			"Robin",
			"Sam",
			"Sandy",
			"Sebastian",
			"Shane",
			"Vincent",
			"Willy",
			"Wizard",
		};

		private static Dictionary<string, Role> NpcData;

		public static Role GetPregnancyRole (Farmer farmer)
		{
			if (farmer == null)
				throw new ArgumentNullException (nameof (farmer));

			if (farmer.mailReceived.Contains ("kdau.PregnancyRole.become"))
				return Role.Become;
			if (farmer.mailReceived.Contains ("kdau.PregnancyRole.make"))
				return Role.Make;
			if (farmer.mailReceived.Contains ("kdau.PregnancyRole.adopt"))
				return Role.Adopt;
			return farmer.IsMale ? Role.Make : Role.Become;
		}

		public static void SetPregnancyRole (Farmer farmer, Role role)
		{
			if (farmer == null)
				throw new ArgumentNullException (nameof (farmer));

			if (GetPregnancyRole (farmer) == role)
				return;

			Monitor.Log ($"Setting pregnancy role for farmer {farmer.Name} to {role}.",
				Config.VerboseLogging ? LogLevel.Info : LogLevel.Debug);

			farmer.mailReceived.Remove ("kdau.PregnancyRole.become");
			farmer.mailReceived.Remove ("kdau.PregnancyRole.make");
			farmer.mailReceived.Remove ("kdau.PregnancyRole.adopt");

			string roleString = role.ToString ().ToLower ();
			farmer.mailReceived.Add ($"kdau.PregnancyRole.{roleString}");
		}

		public static Role GetPregnancyRole (NPC npc, Farmer spouse = null)
		{
			if (npc == null)
				throw new ArgumentNullException (nameof (npc));
			spouse ??= Game1.player;

			if (BaseGameNPCs.Contains (npc.Name))
			{
				if (spouse.mailReceived.Contains ($"kdau.PregnancyRole.{npc.Name}.become"))
					return Role.Become;
				if (spouse.mailReceived.Contains ($"kdau.PregnancyRole.{npc.Name}.make"))
					return Role.Make;
				if (spouse.mailReceived.Contains ($"kdau.PregnancyRole.{npc.Name}.adopt"))
					return Role.Adopt;
			}

			LoadNpcData ();
			if (NpcData.ContainsKey (npc.Name))
				return NpcData[npc.Name];

			return npc.Gender switch
			{
				NPC.male      => Role.Make,
				NPC.female    => Role.Become,
				NPC.undefined => Role.Adopt,
				_             => Role.Adopt,
			};
		}

		public static void SetPregnancyRole (NPC npc, Role role,
			Farmer spouse = null)
		{
			if (npc == null)
				throw new ArgumentNullException (nameof (npc));
			spouse ??= Game1.player;

			if (!BaseGameNPCs.Contains (npc.Name))
			{
				Monitor.Log ($"Not setting pregnancy role for NPC {npc.Name} because they are not from the base game.",
					LogLevel.Warn);
				return;
			}

			if (GetPregnancyRole (npc) == role)
				return;

			Monitor.Log ($"Setting pregnancy role for NPC {npc.Name} to {role} when married to {spouse.Name}.",
				Config.VerboseLogging ? LogLevel.Info : LogLevel.Debug);

			spouse.mailReceived.Remove ($"kdau.PregnancyRole.{npc.Name}.become");
			spouse.mailReceived.Remove ($"kdau.PregnancyRole.{npc.Name}.make");
			spouse.mailReceived.Remove ($"kdau.PregnancyRole.{npc.Name}.adopt");

			string roleString = role.ToString ().ToLower ();
			spouse.mailReceived.Add ($"kdau.PregnancyRole.{npc.Name}.{roleString}");
		}

		public static bool WouldNeedAdoption (Role role1, Role role2)
		{
			return !(role1 == Role.Become && role2 == Role.Make) &&
				!(role1 == Role.Make && role2 == Role.Become);
		}

		public static bool WouldNeedAdoption (Farmer farmer)
		{
			if (farmer == null)
				throw new ArgumentNullException (nameof (farmer));

			NPC spouse = farmer.getSpouse ();
			if (spouse != null)
			{
				return WouldNeedAdoption (GetPregnancyRole (farmer),
					GetPregnancyRole (spouse));
			}

			long? spouseID = Game1.player.team.GetSpouse
				(Game1.player.UniqueMultiplayerID);
			if (spouseID.HasValue)
			{
				Game1.otherFarmers.TryGetValue (spouseID.Value, out Farmer fSpouse);
				if (fSpouse != null)
				{
					return WouldNeedAdoption (GetPregnancyRole (farmer),
						GetPregnancyRole (fSpouse));
				}
			}

			return false;
		}

		public static bool WouldNeedAdoption (NPC npc)
		{
			if (npc == null)
				throw new ArgumentNullException (nameof (npc));

			Farmer spouse = npc.getSpouse ();
			if (spouse != null)
			{
				return WouldNeedAdoption (GetPregnancyRole (npc),
					GetPregnancyRole (spouse));
			}

			return false;
		}

		private static void LoadNpcData ()
		{
			if (NpcData != null)
				return;
			NpcData = new Dictionary<string, Role> ();

			var dictionary = Helper.Content.Load<Dictionary<string, string>>
				("Data\\NPCDispositions", ContentSource.GameContent);
			foreach (string key in dictionary.Keys)
			{
				List<string> relationships = new List<string>
					(dictionary[key].Split('/') [9].Split (' '));

				int index = relationships.IndexOf ("PregnancyRole");
				if (index == -1 || index + 1 >= relationships.Count)
					continue;

				string role = relationships[index + 1];
				try
				{
					NpcData[key] = (Role) Enum.Parse (typeof (Role), role, true);
				}
				catch (ArgumentException)
				{
					Monitor.Log ($"NPC {key} has invalid PregnancyRole value {role}; ignoring.",
						LogLevel.Warn);
				}
			}
		}
	}
}
