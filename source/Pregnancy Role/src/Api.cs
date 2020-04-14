using StardewValley;

namespace PregnancyRole
{
	public enum Role
	{
		// The farmer or NPC can become pregnant.
		Become,

		// The farmer or NPC can make another farmer or NPC pregnant.
		Make,

		// The farmer or NPC would always require adoption to have a baby.
		Adopt,
	}

	public interface IApi
	{
		Role GetPregnancyRole (Farmer farmer);

		Role GetPregnancyRole (NPC npc);

		// Whether the given farmer would require adoption to have a baby with
		// their current spouse, including another farmer.
		bool WouldNeedAdoption (Farmer farmer);

		// Whether the given NPC would require adoption to have a baby with
		// their current farmer spouse.
		bool WouldNeedAdoption (NPC npc);
	}

	public class Api : IApi
	{
		public Role GetPregnancyRole (Farmer farmer) =>
			Model.GetPregnancyRole (farmer);

		public Role GetPregnancyRole (NPC npc) =>
			Model.GetPregnancyRole (npc);

		public bool WouldNeedAdoption (Farmer farmer) =>
			Model.WouldNeedAdoption (farmer);

		public bool WouldNeedAdoption (NPC npc) =>
			Model.WouldNeedAdoption (npc);
	}
}
